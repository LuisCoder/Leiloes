using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ClassesComuns;

namespace Contabilidade
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
        /// Lista de saldos dos utilizadores onde o indice de cada elemento corresponde ao id de utilizador
        /// </summary>
        public static List<float> ListaSaldos = new List<float> {
            50.00f,
            60.00f,
            2000.00f
        };

        /// <summary>
        /// Metodo construtor
        /// </summary>
        /// <param name="sock">Recebe a socket de uma dada conneccao</param>
        public ConnectionThread(Socket sockRecep,Socket sockEnvio)
        {
            this.sockRecep = sockRecep;
            this.sockEnvio = sockEnvio;

            //codigo que trata da desserializacao na socket
            SC = new SockComunica(sockEnvio,sockRecep);

            //Colocar valores dummy na lista de saldos
            //ListaSaldos.Add(10.00f);
            //ListaSaldos.Add(0.00f);
            //ListaSaldos.Add(2000.00f);
        }

        public void handleConnection()
        {
            //incrementa o numero de utilizadores ligados ao servidor central
            cont++;

            //Variaveis auxiliares
            bool exit=false;//quando esta variavel é colocada a true significa que o utilizador vai sair do programa
            string cmd="";//para guardar a variavel do comando do utilizador
            int idUtil;//para guardar o id de utilizador
            
            //Envia a informacao ao cliente que foi conectado com sucesso
            SC.EnviaSer("Connectado com sucesso ao servidor de contabilidade");
            SC.RecebeSer();

            //Receber o id de utilizador
            idUtil=(int)SC.RecebeSer();
            SC.EnviaSer("ack");
            //Console.WriteLine("O id de utilizador: "+idUtil);
            //Console.ReadLine();

            //Menu de opccoes do utilizador
            do{
                //Limpar o ecran da consola
                //Console.Clear();

                //Receber a opcao do menu do utilizador
                //Console.WriteLine("A espera de uma escolha do utilizador");

                //Receber o comando que o utilizador selecionou
                cmd = (string)SC.RecebeSer();
                SC.EnviaSer("ack");

                if (cmd.Equals("saldo"))
                    VerSaldo(idUtil);
                else if (cmd.Equals("depDin"))
                    DepositarDinheiro(idUtil);
                else if (cmd.Equals("trans"))
                    RealizarTransicao(idUtil);


            }while(!exit);

            //Console.WriteLine("A coneccao com o servidor de contabilidade esta aberta");
            //Console.ReadLine();
        }

        /// <summary>
        /// Permite ver o saldo do utilizador
        /// </summary>
        /// <param name="idUtil">id do utilizador que ja foi recebido do servidor de leiloes</param>
        public void VerSaldo(int idUtil)
        {
            Console.WriteLine("O utilizador selecionou a opcao de ver o saldo");

            //Enviar o saldo do utilizador(float)
            Console.WriteLine("Saldo a enviar" + ListaSaldos[idUtil]);
            SC.EnviaSer(ListaSaldos[idUtil]);
            SC.RecebeSer();

            Console.WriteLine("Operacao de ver saldo realizada com sucesso");
            Console.ReadLine();
        }

        /// <summary>
        /// Permite depositar dinheiro
        /// </summary>
        /// <param name="idUtil">id do utilizador que foi recebido do servidor de leiloes</param>
        public void DepositarDinheiro(int idUtil)
        {
            //variaveis auxiliares
            float saldoIncre;

            Console.WriteLine("O utilizador selecionou a opcao de depositar dinheiro");

            //Receber o valor a incrementar ao saldo
            saldoIncre=(float)SC.RecebeSer();
            SC.EnviaSer("ack");

            //incrementar esse valor ao valor de saldo existente
            ListaSaldos[idUtil] += saldoIncre;

            //Enviar o valor do saldo final
            SC.EnviaSer(ListaSaldos[idUtil]);
            SC.RecebeSer();

            Console.WriteLine("Operacao de depositar dinheiro realizado com sucesso");
            Console.ReadLine();
        }

        /// <summary>
        /// Permite realizar uma transaccao quando um leilao chega ao fim
        /// </summary>
        /// <param name="idUtilComprador">Id do comprador do artigo do leilao</param>
        public void RealizarTransicao(int idUtilComprador)
        {
            //variaveis auxiliares
            float saldoDecre;
            int idUtilVendedor;

            Console.WriteLine("O servidor de leiloes pediu a operacao de levantar dinheiro para pagar um artigo");

            //Receber o id do vendedor
            idUtilVendedor = (int)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Receber o valor a decrementar no saldo do comprador e a ser incrementado no saldo do vendedor
            saldoDecre = (float)SC.RecebeSer();
            SC.EnviaSer("ack");

            //Decrementar esse valor ao valor de saldo do comprador
            ListaSaldos[idUtilComprador] -= saldoDecre;

            //Incrementar esse valor ao valor do saldo do vendedor
            ListaSaldos[idUtilComprador] += saldoDecre;

            Console.WriteLine("Operacao de levantar dinheiro realizada com sucesso");
            Console.ReadLine();
        }
    }
}
