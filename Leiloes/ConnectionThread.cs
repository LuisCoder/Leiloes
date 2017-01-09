using ClassesComuns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Leiloes
{
    class ConnectionThread
    {
        /// <summary>
        /// Para guardar a referencia da socket de Recepcao
        /// </summary>
        private Socket sockRecep;

        /// <summary>
        /// Para guardar a referencia da socket de Envio
        /// </summary>
        private Socket sockEnvio;

        /// <summary>
        /// Para simplificar o codigo de comunicacao
        /// </summary>
        private SockComunica SC;

        /// <summary>
        /// Para guardar o numero de clientes ligados ao servidor central
        /// </summary>
        private static int cont = 0;

        /// <summary>
        /// Atributo que guarda uma lista de utilizadores
        /// </summary>
        private ListaUtilizadores Utilizadores = new ListaUtilizadores();
        
        /// <summary>
        /// Atributo que guarda uma lista de leiloes
        /// </summary>
        private ListaLeiloes Leiloes = new ListaLeiloes();

        /// <summary>
        /// Para verificar quando os leiloes terminam
        /// </summary>
        /// <returns></returns>
        private static Timer stateTimer;
        
        /// <summary>
        /// Metodo construtor que e chamado para os varios clientes
        /// </summary>
        /// <param name="sockRecep"></param>
        /// <param name="sockEnvio"></param>
        public ConnectionThread(Socket sockRecep,Socket sockEnvio)
        {
            this.sockRecep = sockRecep;
            this.sockEnvio = sockEnvio;

            //codigo que trata da desserializacao na socket
            SC = new SockComunica(sockEnvio,sockRecep);
        }

        /// <summary>
        /// Metodo construtor que e chamado apenas para criar uma thread para ver quando o leilao termina
        /// </summary>
        public ConnectionThread()
        {
            //Para verificar quando os leiloes terminam
            TimerCallback callback = new TimerCallback(Tick);
            stateTimer = new Timer(callback, null, 0, 1000);
        }

        /// <summary>
        /// De cada vez que passa um segundo este metodo e chamado
        /// </summary>
        /// <param name="stateInfo"></param>
        private void Tick(Object stateInfo)
        {
            //obter o tempo actual
            DateTime relogio=DateTime.Now;
            //Para imprimir o tempo por uma questao de testes
            //Console.WriteLine("Tick: {0}", relogio.ToString("h:mm:ss"));

            //Percorrer todos os leiloes para ver o tempo chegou ao fim
            foreach (Leilao l in ListaLeiloes.Leiloes.Where(a=>a.Activo))
            {
                if (l.VerificaTempoFinal(DateTime.Now))
                {
                    //a. Notificacao para os compradores que fizeram licitacoes mas não ganharam o leilao
                    if (l.NumLicitacoes() > 1)
                    {
                        foreach (string lict in l.LicitadoresPassados())
                        {
                            Utilizadores.GetUtilizador(lict).InserirNotificacao(l.NotificacaoA(lict));
                        }
                    }
                    
                    //b. Notificacao para o comprador que ganhou o leilao
                    if(l.NumLicitacoes()>=1)
                        Utilizadores.GetUtilizador(l.Comprador()).InserirNotificacao(l.NotificacaoB());

                    //c. Para o vendedor quando o tempo de duracao do leilao chega ao fim sem licitacoes
                    if (l.NumLicitacoes() == 0)
                        Utilizadores.GetUtilizador(l.Utilizador).InserirNotificacao(l.NotificacaoC());

                    //d. Para o vendedor quando o tempo de duracao do leilao chega ao fim e existiram licitacoes
                    if (l.NumLicitacoes() >= 0)
                        Utilizadores.GetUtilizador(l.Utilizador).InserirNotificacao(l.NotificacaoD());

                    //Fazer a transacao bancaria se existir pelo menos um licitador
                    if (l.NumLicitacoes() >= 1)
                    {
                        //Estabelecer uma coneccao com o servidor de contabilidade
                        Console.WriteLine("Tentando abrir uma conneccao com o servidor de contabilidade");

                        //Para envio de mensagens
                        Socket newSEnvio = AbrirConecSerCont();
                        //Para recepcao de mensagens
                        Socket newSRecepcao = AbrirConecSerCont();
                        //Para facilitar o codigo de comunicacao com o servidor de contabilidade
                        SockComunica SC1 = new SockComunica(newSEnvio,newSRecepcao);

                        //receber uma confirmacao se o servidor de contabilidade esta online
                        Console.WriteLine(SC1.RecebeSer());
                        SC1.EnviaSer("ack");

                        //Enviar o id do Comprador
                        SC1.EnviaSer(Utilizadores.ObterIdUtilizador(l.Comprador()));
                        SC1.RecebeSer();

                        //Enviar a operacao que neste caso e realizar Transaccao
                        SC1.EnviaSer("trans");
                        SC1.RecebeSer();

                        //**********************************

                        //Enviar o id do vendedor
                        SC1.EnviaSer(Utilizadores.ObterIdUtilizador(l.Utilizador));
                        SC1.RecebeSer();

                        //Enviar o valor a decrementar no saldo do comprador e a ser incrementado no saldo do vendedor
                        SC1.EnviaSer(l.ValorFinalLicitacoes());
                        SC1.RecebeSer();
                    }

                    //Enviar um mail se existir pelo menos um licitador
                    if (l.NumLicitacoes() >= 1)
                        l.MandarMail(Utilizadores.ObterEnderecoMail(l.Utilizador), Utilizadores.ObterEnderecoMail(l.Comprador()));
                    
                    Console.WriteLine("Acabou de fechar o leilao com o seguinte titulo: "+l.Titulo);
                }
            }
        }

        /// <summary>
        /// Metodo que e chamado cada vez que e necessario 
        /// </summary>
        public void handleConnection()
        {
            //incrementa o numero de utilizadores ligados ao servidor central
            cont++;
            
            //Variaveis auxiliares
            //Utilizador util = new Utilizador { };
            string login="";//para guardar o login que vem da aplicacao cliente
            string password = "";//para guarda a password que vem da aplicacao cliente
            bool utilExist=false;//para guardar a resposta que vai para o servidor se existe um utilizador com um determinado login
            bool passCorrecta=false;//para guardar a resposta que vai para o servidor de leiloes se a password é correcta ao nao
            bool exit = false;//quando esta variavel é colocada a true significa que o utilizador vai sair do programa
            string cmd = "";//para guardar a variavel do comando do utilizador
            int idUtil;//para guardar o id de utilizador que vai ser utilizado para obter o saldo no servidor de contabilidade

            //codigo que trata da desserializacao na socket
            //SC = new SockComunica(sock);

            //IPEndPoint clientep = (IPEndPoint) client.RemoteEndPoint;
            //Console.WriteLine("Ligado a {0} no porto {1}",clientep.Address,clientep.Port);

            //Envia a informacao ao cliente que foi conectado com sucesso
            SC.EnviaSer("Connectado com sucesso ao servidor de leiloes");
            Console.WriteLine(SC.RecebeSer());

            do{
                //Receber login de utilizador(string)
                login = (string)SC.RecebeSer();
                SC.EnviaSer("ack");

                //Verificar se existe um utilizador com esse login
                Console.WriteLine("A verificar o login de utilizador");
                utilExist=Utilizadores.VerificarUtilizador(login);

                //Enviar a confirmacao de que existe ou nao um utilizador com esse login(boolean)
                SC.EnviaSer(utilExist);
                SC.RecebeSer();
            }while(!utilExist);

            //Console.WriteLine("Confirmado que existe login de utilizador");

            do{
                //Receber a password(string)
                password = (string)SC.RecebeSer();
                SC.EnviaSer("ack");

                //Verificar se a password de utilizador é a correcta
                Console.WriteLine("A verificar a password de utilizador");
                passCorrecta = Utilizadores.VerificarPassWord(login,password);

                //Enviar a confirmacao de que a password é correcta ao nao(boolean)
                SC.EnviaSer(passCorrecta);
                SC.RecebeSer();
            }while(!passCorrecta);

            //obter o id de utilizador para obter o saldo e depositar dinheiro
            idUtil = Utilizadores.ObterIdUtilizador(login);

            //menu de opcoes do utilizador
            do{
                //Limpar o ecran da consola
                //Console.Clear();

                //Receber a opcao de menu do utilizador
                Console.WriteLine("A espera de uma escolha do utilizador");

                //Receber o comando que o utilizador selecionou
                cmd=(string)SC.RecebeSer();
                SC.EnviaSer("ack1");
                //SC.EnviaSer("ack2");

                //Console.WriteLine("O utilizador introduzio o seguinte comando: |"+cmd+"|");
                //Console.ReadLine();//Por alguma razao so funciona com este readline

                if (cmd.Equals("saldo"))
                    VerSaldo(idUtil);
                else if (cmd.Equals("depDin"))
                    DepositarDinheiro(idUtil);
                else if (cmd.Equals("not"))
                    VerNotificacoes(login);
                else if (cmd.Equals("verLei"))
                    VerListaLeiloes(login);
                else if (cmd.Equals("verLeiAct"))
                    VerListaLeiloesActivos(login);
                else if (cmd.Equals("criarLei"))
                    CriarLeilao(login);
                else if (cmd.Equals("infoLei"))
                    InfoLeilao();
                else if (cmd.Equals("licitar"))
                    Licitar(login);
                else if (cmd.Equals("valorLic"))
                    ValorMaximoLic(idUtil,login);
                else if (cmd.Equals("verLeiFech"))
                    VerListaLeiloesFech(login);
                else if (cmd.Equals("listarLei"))
                    VerListaTodosLeiloes();
                else if (cmd.Equals("exit"))
                    exit = true;
                else
                    Console.WriteLine("Erro comando nao e reconhecido pelo servidor");

            }while(!exit);

            //do
            //{
            //    recebe = client.Receive(data, data.Length, SocketFlags.None);

            //    txt = Encoding.ASCII.GetString(data,0,recebe);
                
            //    //Isto é o que fazia broadcast das mensagens pelo que ja nao é necessario no entanto vou deixar comentado
            //    //Program.Broadcast(txt);
            //} while (txt!="exit");

            cont--;
            Console.WriteLine("O cliente desconectou-se");
            SC.EnviaSer("ack");
        }

        /// <summary>
        /// Permite ver o saldo do utilizador
        /// </summary>
        public void VerSaldo(int idUtil)
        {
            //variaveis auxiliares
            float saldo;

            Console.WriteLine("O utilizador selecionou a opcao de ver o saldo");

            //Estabelecer uma coneccao com o servidor de contabilidade
            Console.WriteLine("Tentando abrir uma conneccao com o servidor de contabilidade");
            
            //Para envio de mensagens
            Socket newSEnvio = AbrirConecSerCont();
            //Para recepcao de mensagens
            Socket newSRecepcao = AbrirConecSerCont();
            //Para facilitar o codigo de comunicacao com o servidor de contabilidade
            SockComunica SC1 = new SockComunica(newSEnvio,newSRecepcao);

            //receber uma confirmacao se o servidor de contabilidade esta online
            Console.WriteLine(SC1.RecebeSer());
            SC1.EnviaSer("ack");
            
            //Enviar a id de utilizador
            SC1.EnviaSer(idUtil);
            SC1.RecebeSer();

            //Enviar a operecao do utilizador que neste caso é verificar o saldo
            SC1.EnviaSer("saldo");
            SC1.RecebeSer();

            //**********************************

            //Receber o saldo do utilizador(float)
            saldo=(float)SC1.RecebeSer();
            SC1.EnviaSer("ack");
            //SC1.EnviaSer("ack");

            Console.WriteLine("O saldo do cliente: "+saldo);
            //Console.ReadLine();

            //Enviar o saldo para a aplicacao cliente
            //SC.EnviaSer(saldo);
            //SC.RecebeSer();

            //Fechar a coneccao com o servidor de contabilidade
            Console.WriteLine("A fechar ligacao com o servidor de contabilidade");
            newSEnvio.Close();
            newSRecepcao.Close();

            Console.WriteLine("Operacao de ver saldo realizada com sucesso");
            //Console.ReadLine();

            //Enviar o saldo para a aplicacao cliente
            SC.EnviaSer(saldo);
            //SC.EnviaSer(saldo);
            SC.RecebeSer();
        }

        /// <summary>
        /// Permite despositar dinheiro
        /// </summary>
        public void DepositarDinheiro(int idUtil)
        {
            //Variaveis auxiliares
            float saldoIncre;
            float saldo;

            //Receber da aplicacao cliente o valor a incrementar ao saldo
            saldoIncre = (float)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Estabelecer uma coneccao com o servidor de contabilidade
            Console.WriteLine("Tentando abrir uma conneccao com o servidor de contabilidade");
            //Para envio de mensagens
            Socket newSEnvio = AbrirConecSerCont();
            //Para recepcao de mensagens
            Socket newSRecepcao = AbrirConecSerCont();
            //Para facilitar o codigo de comunicacao com o servidor de contabilidade
            SockComunica SC1 = new SockComunica(newSEnvio,newSRecepcao);

            //receber uma confirmacao se o servidor de contabilidade esta online
            Console.WriteLine(SC1.RecebeSer());
            SC1.EnviaSer("ack");

            //Enviar a operacao que o id de utilizador
            SC1.EnviaSer(idUtil);
            SC1.RecebeSer();

            //Enviar a operacao do utilizador que neste caso é incrementar o saldo
            SC1.EnviaSer("depDin");
            SC1.RecebeSer();

            //*************************

            //Enviar o valor a incrementar ao saldo
            SC1.EnviaSer(saldoIncre);
            SC1.RecebeSer();

            //Receber o valor final do saldo
            saldo = (float)SC1.RecebeSer();
            SC1.EnviaSer("ack");

            //Fechar a coneccao com o servidor de contabilidade
            Console.WriteLine("A fechar ligacao com o servidor de contabilidade");
            newSEnvio.Close();
            newSRecepcao.Close();

            //Enviar o valor do saldo final para a aplicacao cliente
            SC.EnviaSer(saldo);
            SC.RecebeSer();

            Console.WriteLine("Operacao de ver o depositar dinheiro realizada com sucesso");
        }

        /// <summary>
        /// Ver notificacoes
        /// </summary>
        public void VerNotificacoes(string login)
        {
            Console.WriteLine("O utilizador selecionou a opcao de ver uma lista das notificacoes");

            //Enviar lista de notificacoes do utilizador para a aplicacao cliente na forma de uma string
            SC.EnviaSer(Utilizadores.GetUtilizador(login).Notificacoes.ToString());
            SC.RecebeSer();

            Console.WriteLine("Operacao de ver todas as notificacoes de um utilizador realizada com sucesso");
        }

        /// <summary>
        /// Ver lista de leiloes em que o utilizador ainda nao fez licitacoes
        /// </summary>
        public void VerListaLeiloes(string login)
        {
            Console.WriteLine("O utilizador selecionou a opcao de ver uma lista de todos os leiloes em que ainda nao fez licitacoes");
            
            //Enviar lista dos leiloes em que o utilizador nao fez licitacoes para a aplicacao cliente na forma de uma string
            SC.EnviaSer(Leiloes.VerLeiloesDisp(login));
            SC.RecebeSer();

            Console.WriteLine("Operacao de ver todos os leiloes sem licitacoes do utilizador realizada com sucesso");
            //Console.ReadLine();
        }

        /// <summary>
        /// Ver lista de leiloes em que o utilizador 
        /// </summary>
        public void VerListaLeiloesActivos(string login)
        {
            Console.WriteLine("O utilizador selecionou a opcao de ver uma lista de todos os leiloes em que ja fez licitacoes");

            //Enviar lista dos leiloes em que o utilizador nao fez licitacoes para a aplicacao cliente na forma de uma string
            SC.EnviaSer(Leiloes.VerLeiloesActivos(login));
            SC.RecebeSer();

            Console.WriteLine("Operacao de ver todos os leiloes em que o utilizador ja licitou realizada com sucesso");
            //Console.ReadLine();
        }

        /// <summary>
        /// Criar um leilao no sistema
        /// </summary>
        public void CriarLeilao(string login)
        {
            Console.WriteLine("Numero de leiloes no sistema: "+Leiloes.NumLeiloes());
            //variaveis auxiliares
            string titulo;
            string descricaoArtigo;
            float baseLic;
            int duracao;
            int pos;//para guardar a posicao do leilao na lista de leiloes porque entretanto pode haver outra thread que tambem vai adicionar um leilao

            Console.WriteLine("O utilizador selecionou a opcao de criar leilao");

            //Receber o titulo do leilao
            titulo=(string)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Receber a descricao do artigo de leilao
            descricaoArtigo = (string)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Receber a base de licitacao
            baseLic = (float)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Receber duracao do leilao
            duracao = (int)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Colocar o leilao na lista de leiloes
            pos=Leiloes.CriarLeilao(titulo,descricaoArtigo, baseLic, duracao, login);

            //Enviar informacao ao cliente que a operacao foi realizada com sucesso
            SC.EnviaSer("A operacao de criar leilao foi efectuada com sucesso");
            SC.RecebeSer();

            Console.WriteLine("Operacao de criar leilao realizada com sucesso.");
            Console.WriteLine("Numero de leiloes no sistema: " + Leiloes.NumLeiloes());
        }

        /// <summary>
        /// Obter informacao completa do leilao especifico
        /// </summary>
        public void InfoLeilao()
        {
            //variaveis auxiliares
            string titulo;//titulo do leilao que queremos obter informacoes
            string leilao;//informacao do leilao a enviar para a aplicacao cliente

            Console.WriteLine("O utilizador selicionou a opcao de obter a informacao do leilao");

            //Receber o titulo do utilizador
            titulo=(string)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Obter informacao do leilao da lista
            leilao=Leiloes.InfoLeilao(titulo);

            //Enviar a lista de detalhes do leilao
            SC.EnviaSer(leilao);
            SC.RecebeSer();
        }

        /// <summary>
        /// Licitar num determinado leilao
        /// </summary>
        public void Licitar(string login)
        {
            //variaveis auxiliares
            string titulo;
            float valorLicitado;

            Console.WriteLine("O utilizador selecionou a opcao de licitar o leilao");

            //Receber o titulo do leilao
            titulo = (string)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Receber o valor a licitar no leilao
            valorLicitado = (float)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Colocar os valores na lista de licitados
            Leiloes.Licitar(titulo, login, valorLicitado);

            //Enviar informacao ao cliente que a operacao foi realizada com sucesso
            SC.EnviaSer("A operacao de licitar leilao foi efectuada com sucesso");
            SC.RecebeSer();

            Console.WriteLine("Operacao de licitar leilao realizada com sucesso");
        }

        /// <summary>
        /// Para obter o valor maximo que o utilizador pode licitar
        /// </summary>
        public void ValorMaximoLic(int idUtil,String login)
        {
            //variaveis auxiliares
            float valorTotal;//valor total licitado em todos os leiloes
            float saldo;//saldo que o utilizador tem para investir

            Console.WriteLine("O utilizador selecionou a opcao de ver o valor maximo que pode licitar");

            //Estabelecer uma conneccao com o servidor de contabilidade
            Console.WriteLine("Tentando abrir uma conneccao com o servidor de contabilidade");

            //Para o envio de mensagens
            Socket newEnvio = AbrirConecSerCont();
            //Para recepcao de mensagens
            Socket newSRecepcao = AbrirConecSerCont();
            //Para facilitar o codigo de comunicacao com o servidor de contaibilidade
            SockComunica SC1 = new SockComunica(newEnvio,newSRecepcao);

            //Receber uma confirmacao se o servidor de contabilidade esta online
            Console.WriteLine(SC1.RecebeSer());
            SC1.EnviaSer("ack");

            //Enviar o id de utilizador
            SC1.EnviaSer(idUtil);
            SC1.RecebeSer();

            //Enviar a operacao do utilizador que neste caso é verificar o saldo(operacao é a mesma que em cima)
            SC1.EnviaSer("saldo");
            SC1.RecebeSer();

            //Receber o saldo do utilizador(float)
            saldo = (float)SC1.RecebeSer();
            SC1.EnviaSer("ack");

            //Vai buscar o valor total das licitacoes ja feitas
            valorTotal = Leiloes.ValorLic(login);

            //Enviar o saldo do utilizador
            SC.EnviaSer(saldo);
            SC.RecebeSer();

            //Enviar o valor total licitado em leiloes
            SC.EnviaSer(valorTotal);
            SC.RecebeSer();

            Console.WriteLine("Operacao de ver o valor maximo que pode licitar realizada com sucesso");
            //Console.ReadLine();
        }

        /// <summary>
        /// Para ver a lista de leiloes fechados de um determinado utilizador
        /// </summary>
        public void VerListaLeiloesFech(string login)
        {
            Console.WriteLine("O utilizador selecionou a opcao de a sua lista de leiloes ja encerrados");

            //Enviar lista de todos os leiloes no servidor de leiloes para a aplicacao cliente na forma de uma string
            SC.EnviaSer(Leiloes.VerListaLeiloesFech(login));
            SC.RecebeSer();

            Console.WriteLine("Operacao de ver todos os leiloes realizada com sucesso");
            //Console.ReadLine();
        }

        /// <summary>
        /// Para obter uma lista de todos os leiloes existentes no servidor de leiloes
        /// </summary>
        public void VerListaTodosLeiloes()
        {
            Console.WriteLine("O utilizador selecionou a opcao de ver uma lista de todos os leiloes");

            //Enviar lista de todos os leiloes no servidor de leiloes para a aplicacao cliente na forma de uma string
            Console.WriteLine("A enviar lista de leiloes para o cliente");
            SC.EnviaSer(Leiloes.VerListaTodosLeiloes());
            Console.WriteLine((string)SC.RecebeSer());

            Console.WriteLine("Operacao de ver leiloes fechados realizada com sucesso");
            //Console.ReadLine();
        }

        /// <summary>
        /// metodo que permite estabelecer uma conecxao com o servidor de contabilidade
        /// </summary>
        public static Socket AbrirConecSerCont()
        {
            //codigo que trata da conneccao
            Socket sock = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            //o servidor da contabilidade é na porta 6001
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"),6001);
            sock.Connect(ipe);
            return sock;
        }
    }
}
