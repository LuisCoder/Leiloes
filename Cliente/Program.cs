using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassesComuns;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Cliente
{
    class Program
    {
        //Variaveis para definir a posicao da consola
        const int SWP_NOSIZE = 0x0001;

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        private static IntPtr MyConsole = GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        /// <summary>
        /// Para simplicar o codigo que permite comunicar entre as duas aplicacoes
        /// </summary>
        private static SockComunica SC;

        static void Main(string[] args)
        {
            //codigo para defenir a posicao da janela
            int xpos = 300;
            int ypos = 300;
            SetWindowPos(MyConsole, 0, xpos, ypos, 0, 0, SWP_NOSIZE);

            Console.Title = "Cliente";
            Console.WriteLine("**********Cliente**********");

            //Variaveis auxilixares
            string login = "";
            string password = "";
            bool utilExist=false;//para guardar a resposta que vem do servidor leiloes se existe um utilizador com um determinado login
            bool passCorrecta=false;//para guadar a resposta que vem do servidor leiloes se a password é correcta ao nao
            bool exit=false;//quando esta variavel é colocada a true significa que o utilizador vai sair do programa
            string cmd="";//para guardar a variavel do comando do utilizador

            //o servidor dos leiloes é na porta 6000
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);

            //codigo que trata do Envio
            Socket sockEnvio = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sockEnvio.Connect(ipe);
            //Console.WriteLine("Socket de envio aberta com sucesso");

            //codigo que trata da Recepcao
            Socket sockRecepcao = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sockRecepcao.Connect(ipe);
            //Console.WriteLine("Socket de recepcao aberta com sucesso");

            //codigo que trata da desserializacao na socket
            SC = new SockComunica(sockEnvio,sockRecepcao);

            //Recebe uma mensagem de confirmacao de que o cliente foi conectado ao servidor com sucesso
            Console.WriteLine(SC.RecebeSer());
            SC.EnviaSer("O cliente conectou-se com sucesso a este servidor");

            do{
                //Introduzir o login
                Console.WriteLine("Introduza o login: ");
                login = Console.ReadLine();

                //Enviar o login(string)
                SC.EnviaSer(login);
                SC.RecebeSer();

                //Receber a confirmacao de que existe um utilizador ou nao com esse login(boolean)
                utilExist = (bool)SC.RecebeSer();
                SC.EnviaSer("ack");

                Console.WriteLine(utilExist?"Login existe":"Esse login nao existe");
            }while(!utilExist);

            do{
                //enviar a password(string)
                Console.WriteLine("Introduza a password:");
                password = Console.ReadLine();

                //encriptar a password usando md5
                System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] data=System.Text.Encoding.ASCII.GetBytes(password);
                data = x.ComputeHash(data);
                password = System.Text.Encoding.ASCII.GetString(data);

                //Enviar a password
                SC.EnviaSer(password);
                SC.RecebeSer();

                //Receber a confirmacao de que password é correcta ao nao(boolean)
                passCorrecta = (bool)SC.RecebeSer();
                SC.EnviaSer("ack");

                Console.WriteLine(passCorrecta?"Password correcta":"Password incorrecta");
            }while(!passCorrecta);

            Console.WriteLine("Utilizador autenticado com sucesso");
            Console.ReadLine();

            //mostrar entao um menu de opcoes
            do{
                //Limpar o ecran da consola
                Console.Clear();

                //Imprimir as opcoes de menu
                PrintMenu();

                //Receber a opcao da consola que o utilizador deseja efectuar
                cmd = Console.ReadLine();

                //enviar a operacao que o utilizador vai efectuar no servidor de leiloes
                //SC.EnviaSer(cmd);
                //Console.WriteLine(SC.RecebeSer());

                if (cmd.Equals("saldo"))
                    VerSaldo(cmd);
                else if (cmd.Equals("depDin"))
                    DepositarDinheiro(cmd);
                else if (cmd.Equals("not"))
                    VerNotificacoes(cmd);
                else if (cmd.Equals("verLei"))
                    VerListaLeiloes(cmd);
                else if (cmd.Equals("verLeiAct"))
                    VerListaLeiloesActivos(cmd);
                else if (cmd.Equals("criarLei"))
                    CriarLeilao(cmd);
                else if (cmd.Equals("infoLei"))
                    InfoLeilao(cmd);
                else if (cmd.Equals("licitar"))
                    Licitar(cmd);
                else if (cmd.Equals("valorLic"))
                    ValorMaximoLic(cmd);
                else if (cmd.Equals("verLeiFech"))
                    VerListaLeiloesFech(cmd);
                else if (cmd.Equals("listarLei"))
                    VerListaTodosLeiloes(cmd);
                else if (cmd.Equals("exit"))
                    exit = true;
                else
                {
                    Console.WriteLine("Esse comando nao e reconhecido. Prima enter.");
                    Console.ReadLine();
                }

                //Console.ReadLine();

            }while(!exit);

            //Enviar o comando exit para o servidor de leiloes
            SC.EnviaSer("exit");
            SC.RecebeSer();
            SC.RecebeSer();

            //Fechar ligacoes com o servidor de leiloes
            sockEnvio.Close();
            sockRecepcao.Close();
        }

        /// <summary>
        /// Permite ver o saldo do utilizador
        /// </summary>
        public static void VerSaldo(string cmd){
            //enviar a operacao que o utilizador vai efectuar no servidor de leiloes
            Console.WriteLine("A enviar comando para o servidor de leiloes");
            SC.EnviaSer(cmd);
            SC.RecebeSer();

            //Receber saldo do servidor de leiloes
            Console.WriteLine("A espera de receber o saldo");
            Console.WriteLine("Saldo: {0:0.00}",(float)SC.RecebeSer());
            //Console.WriteLine("Saldo: "+ SC.RecebeSer());
            SC.EnviaSer("ack");

            Console.WriteLine("A operacao de ver saldo foi realizada com sucesso. Prima enter");
            Console.ReadLine();
        }

        /// <summary>
        /// Permite despositar dinheiro
        /// </summary>
        public static void DepositarDinheiro(string cmd)
        {
            //variaveis auxiliares
            Console.WriteLine("Introduza o valor a incrementar no saldo: ");
            float f=float.Parse(Console.ReadLine());

            //enviar a operacao que o utilizador vai efectuar no servidor de leiloes
            Console.WriteLine("A enviar comando para o servidor de leiloes");
            SC.EnviaSer(cmd);
            SC.RecebeSer();

            //enviar o saldo a ser incrementado
            SC.EnviaSer(f);
            SC.RecebeSer();

            //Receber valor do saldo final
            Console.WriteLine("Saldo: {0:0.00}",SC.RecebeSer());
            SC.EnviaSer("ack");

            Console.WriteLine("A operacao de depositar dinheiro foi realizada com sucesso. Prima enter");
            Console.ReadLine();
        }

        /// <summary>
        /// Ver notificacoes
        /// </summary>
        public static void VerNotificacoes(string cmd)
        {
            //Enviar a operacao que o utilizador vai efectuar no servidor de leiloes
            Console.WriteLine("Enviar comando para o servidor de leiloes");
            SC.EnviaSer(cmd);
            SC.RecebeSer();

            //Receber notificacoes do servidor de leiloes
            Console.WriteLine("Notificacoes do utilizador:");
            Console.WriteLine((string)SC.RecebeSer());
            SC.EnviaSer("ack");

            Console.WriteLine("A operacao de ver notificacoes foi realizada com sucesso. Prima enter");
            Console.ReadLine();
        }

        /// <summary>
        /// Ver lista de leiloes em que o utilizador ainda nao fez licitacoes
        /// </summary>
        public static void VerListaLeiloes(string cmd)
        {
            //variaveis auxiliares
            string lista;

            //enviar a operacao que o utilizador vai efectuar no servidor de leiloes
            Console.WriteLine("A enviar o comando para o servidor de leiloes");
            SC.EnviaSer(cmd);
            Console.WriteLine((string)SC.RecebeSer());

            //Receber a lista de leiloes em que o utilizador ainda nao licitou
            //Console.WriteLine("A receber lista de todos os leiloes");
            lista = (string)SC.RecebeSer();
            //Console.WriteLine("Lista de todos os leiloes recebida com sucesso");
            SC.EnviaSer("ack");

            //Imprimir a lista de todos os leiloes no ecran
            Console.WriteLine();
            Console.WriteLine("Lista de leiloes que o utilizador nao licitou:");
            Console.WriteLine(lista);

            Console.WriteLine("A operacao de ver todos os leiloes em que o utilizador nao licitou realizada com sucesso. Prima enter");
            Console.ReadLine();
        }

        /// <summary>
        /// Ver lista de leiloes em que o utilizador fez licitacoes
        /// </summary>
        public static void VerListaLeiloesActivos(string cmd)
        {
            //variaveis auxiliares
            string lista;

            //enviar a operacao que o utilizador vai efectuar no servidor de leiloes
            Console.WriteLine("A enviar o comando para o servidor de leiloes");
            SC.EnviaSer(cmd);
            SC.RecebeSer();

            //Receber a lista de leiloes em que o utilizador ja licitou
            lista = (string)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Imprimir a lista de todos os leiloes no ecran
            Console.WriteLine();
            Console.WriteLine("Lista de leiloes que o utilizador ja licitou:");
            Console.WriteLine(lista);

            Console.WriteLine("A operacao de ver todos os leiloes em que o utilizador ja licitou realizada com sucesso. Prima enter");
            Console.ReadLine();
        }

        /// <summary>
        /// Criar um leilao
        /// </summary>
        public static void CriarLeilao(string cmd)
        {
            //variaveis auxiliares
            Console.WriteLine();
            Console.WriteLine("Introduza o titulo do leilao: ");
            string titulo = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("Introduza uma descricao do artigo");
            string descricaoArtigo = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Introduza a base de licitacao");
            float baseLic = float.Parse(Console.ReadLine());
            Console.WriteLine();

            Console.WriteLine("Introduza a duracao do leilao");
            int duracao=Int32.Parse(Console.ReadLine());
            Console.WriteLine();

            //Enviar a operacao que o utilizador vai efectuar no servidor de leiloes
            Console.WriteLine("A enviar comando para o servidor de leiloes");
            SC.EnviaSer(cmd);
            SC.RecebeSer();

            //Enviar titulo do leilao
            SC.EnviaSer(titulo);
            SC.RecebeSer();

            //Enviar descricao do artigo de leilao
            SC.EnviaSer(descricaoArtigo);
            SC.RecebeSer();

            //Enviar base de licitacao
            SC.EnviaSer(baseLic);
            SC.RecebeSer();

            //Enviar duracao do leilao
            SC.EnviaSer(duracao);
            SC.RecebeSer();

            //Receber informacao de que a operacao foi realizada com sucesso
            Console.WriteLine((string)SC.RecebeSer()+" Prima enter.");
            SC.EnviaSer("ack");

            Console.ReadLine();
        }

        /// <summary>
        /// Obter informacao completa do leilao especifico
        /// </summary>
        public static void InfoLeilao(string cmd)
        {
            //variaveis auxiliares
            Console.WriteLine();
            Console.WriteLine("Introduza o titulo do leilao");
            string nomeLeilao = Console.ReadLine();

            //enviar operacao que o utilizadore vai efectuar no servidor de leiloes
            Console.WriteLine("A enviar comando para o servidor de leiloes");
            SC.EnviaSer(cmd);
            SC.RecebeSer();

            //Enviar o titulo do leilao

            SC.EnviaSer(nomeLeilao);
            SC.RecebeSer();

            //receber a lista de detalhes do leilao
            Console.WriteLine();
            Console.WriteLine(SC.RecebeSer());
            SC.EnviaSer("ack");

            Console.WriteLine("A operacao de obter informacao foi realizada com sucesso. Prima enter");
            Console.ReadLine();
        }

        /// <summary>
        /// Licitar num determinado leilao
        /// </summary>
        public static void Licitar(string cmd)
        {
            //variaveis auxiliares
            Console.WriteLine("Introduza o titulo do leilao a licitar");
            string titulo = Console.ReadLine();
            Console.WriteLine("Introduza o valor a licitar");
            float valorLicitado = float.Parse(Console.ReadLine());

            //Enviar a operacao que o utilizador vai efectuar no servidor de leiloes
            Console.WriteLine("A enviar comando para o servidor de leiloes");
            SC.EnviaSer(cmd);
            SC.RecebeSer();

            //Enviar o titulo do leilao a licitar
            SC.EnviaSer(titulo);
            SC.RecebeSer();

            //Enviar valor a licitar para o servidor de leiloes
            SC.EnviaSer(valorLicitado);
            SC.RecebeSer();

            //Receber informacao que a operacao foi realizada com sucesso
            Console.WriteLine(SC.RecebeSer()+" Prima enter.");
            SC.EnviaSer("ack");

            Console.ReadLine();
        }

        /// <summary>
        /// Para mostrar o valor maximo que o utilizador pode licitar
        /// </summary>
        public static void ValorMaximoLic(string cmd)
        {
            //variaveis auxiliares
            float saldo;
            float valorTotal;

            //enviar a operacao que o utilizador vai efectuar no servidor de leiloes
            Console.WriteLine("A enviar comando para o servidor de leiloes");
            SC.EnviaSer(cmd);
            SC.RecebeSer();

            //Receber saldo do servidor
            saldo=(float)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Receber valor total licitado em leiloes
            valorTotal = (float)SC.RecebeSer();
            SC.EnviaSer("ack");
            
            Console.WriteLine("Saldo do utilizador:    {0:0.00}",saldo);
            Console.WriteLine("Valor total licitado:   {0:0.00}",valorTotal);
            Console.WriteLine("valor que pode licitar: {0:0.00}",saldo-valorTotal);

            Console.WriteLine("A operacao de ver saldo foi realizada com sucesso. Prima enter");
            Console.ReadLine();
        }

        /// <summary>
        /// Para mostrar uma lista de leiloes pertences a um determinado utilizador que ja se encontram fechados 
        /// </summary>
        public static void VerListaLeiloesFech(string cmd)
        {
            //variaveis auxiliares
            string lista;

            //enviar a operacao que o utilizador vai efectuar no servidor de leiloes
            Console.WriteLine("A enviar o comando para o servidor de leiloes");
            SC.EnviaSer(cmd);
            SC.RecebeSer();

            //Receber a lista de leiloes fechados do utilizador
            lista = (string)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Imprimir a lista de todos os leiloes no ecran
            Console.WriteLine();
            Console.WriteLine("Lista de leiloes do utilizador ja encerrados:");
            Console.WriteLine(lista);

            Console.WriteLine("A operacao de ver todos os leiloes do utilizador ja encerrados realizada com sucesso. Prima enter");
            Console.ReadLine();
        }

        /// <summary>
        /// Para mostrar uma lista de todos os leiloes que se encontram no servidor de leiloes
        /// </summary>
        public static void VerListaTodosLeiloes(string cmd)
        {
            //variaveis auxiliares
            string lista;

            //Enviar a operacao que o utilizador vai efectuar no servidor de leiloes
            Console.WriteLine("A enviar comando para o servidor de leiloes");
            SC.EnviaSer(cmd);
            SC.RecebeSer();

            //Receber a lista de todos os leiloes
            lista = (string)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Imprimir lista de todos os leiloes no ecran
            Console.WriteLine();
            Console.WriteLine("Lista de todos os leiloes do servido de leiloes:");
            Console.WriteLine(lista);

            Console.WriteLine("A operacao de ver todos os leiloes realizada com sucesso. Prima enter");
            Console.ReadLine();
        }

        /// <summary>
        /// Para imprimir o a lista de opcoes do utilizador
        /// </summary>
        public static void PrintMenu()
        {
            Console.WriteLine("Comandos:");
            Console.WriteLine();
            Console.WriteLine("Operacoes Bancarias:");
            Console.WriteLine("saldo     ->Ver saldo(feito)");
            Console.WriteLine("depDin    ->Depositar Dinheiro(feito)");
            Console.WriteLine();
            Console.WriteLine("Operacoes de Leilao");
            Console.WriteLine("not       ->Ver notificacoes");
            Console.WriteLine("verLei    ->Ver Leiloes disponiveis (leiloes em que o utilizador ainda nao fez licitacoes)(feito)");
            Console.WriteLine("verLeiAct ->Ver Leiloes activos (leiloes em que o utilizador fez licitacoes)(feito)");
            Console.WriteLine("criarLei  ->Criar Leilao(feito)");
            Console.WriteLine("infoLei   ->Obter informacao de um leilao especifico(feito)");
            Console.WriteLine("licitar   ->Licitar");
            Console.WriteLine();
            Console.WriteLine("Comandos que nao se encontram no enunciado mas que dao jeito");
            Console.WriteLine("valorLic  ->Valor maximo que o utilizador pode licitar");
            Console.WriteLine("verLeiFech->Mostrar lista de leiloes percentes ao utilizador logado que ja se encontram fechados");
            Console.WriteLine("listarLei ->Listar todos os leiloes no sistema(feito)");
            Console.WriteLine();
            Console.WriteLine("exit      ->Sair do programa");
            Console.WriteLine();
            Console.WriteLine("Escreva um dos possiveis comandos: ");
        }
    }
}
