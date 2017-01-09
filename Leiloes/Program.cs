using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClassesComuns;
using System.Runtime.InteropServices;

namespace Leiloes
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
        /// Para guardar os sockets de recepcao
        /// </summary>
        public static Socket[] SARecep = new Socket[20];

        /// <summary>
        /// Para guardar os sockets de envio
        /// </summary>
        public static Socket[] SAEnvio = new Socket[20];

        static void Main(string[] args)
        {
            //codigo para defenir a posicao da janela
            int xpos = 150;
            int ypos = 150;
            SetWindowPos(MyConsole, 0, xpos, ypos, 0, 0, SWP_NOSIZE);

            Console.Title = "Servidor de Leiloes";
            Console.WriteLine("**********Servidor de Leiloes**********");

            //contador para contar o numero de ligacoes duplas ao cliente(Recepcao e envio)
            int i = 0;

            //Criacao de um novo socket
            Socket newSock = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            
            //Criacao de um IPEndPoint
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any,6000);

            //Associar o socket ao IPEndPoint
            newSock.Bind(ipep);

            //Colocar o socket a escuta de, no maximo 20 ligacoes
            newSock.Listen(20);

            //Colocar a thread para as notificacoes
            ConnectionThread clock = new ConnectionThread();
            
            Console.WriteLine("Em espera...");

            do
            {
                //Aceitar uma conexao pedida por um socket para recepcao de mensagens
                SARecep[i] = newSock.Accept();

                //Aceitar uma conexao pedida por um socket para envio de mensagens
                SAEnvio[i] = newSock.Accept();

                //Criacao de um objecto newconnection da classe ConnectionThread
                ConnectionThread dedicatedHandle = new ConnectionThread(SARecep[i],SAEnvio[i]);

                //Criacao de uma thread para executar o codigo contido no metodo
                //HandleConnection() da classe ConnectionThread
                //Thread ht = new Thread(dedicatedHandle.handleConnection);
                Thread ht = new Thread(new ThreadStart(dedicatedHandle.handleConnection));
                
                //iniciacao da thread
                ht.Start();

                i++;
            } while (true);

        }
        //Isto era apenas para fazer o broadcast das mensagens no servidor para 
        //public static void Broadcast(String msg)
        //{
        //    for (int i = 0; i < 1; i++)
        //    {
        //        SA[i].Send(Encoding.ASCII.GetBytes(msg));
        //    }
        //}
    }
}
