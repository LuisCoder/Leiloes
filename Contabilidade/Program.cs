using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Contabilidade
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
        public static Socket[] SARecep=new Socket[20];

        /// <summary>
        /// Para guardar os sockets de envio
        /// </summary>
        public static Socket[] SAEnvio = new Socket[20];

        static void Main(string[] args)
        {
            //codigo para defenir a posicao da janela
            int xpos = 0;
            int ypos = 0;
            SetWindowPos(MyConsole, 0, xpos, ypos, 0, 0, SWP_NOSIZE);

            Console.Title = "Servidor de Contabilidade";
            Console.WriteLine("**********Servidor de Contabilidade**********");

            //contador para contar o numero de ligacoes ao cliente
            int i = 0;

            //Criacao de novo socket
            Socket newSock = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            //Criacao de um IPEndPoint
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any,6001);

            //Associar o socket ao IPEndPoint
            newSock.Bind(ipep);

            //Colocar o socket a escuta de, no maximo 20 ligacoes
            newSock.Listen(20);

            Console.WriteLine("Em espera");

            do{
                //Aceitar uma conexao pedida por um socket para recepcao de mensagens
                SARecep[i] = newSock.Accept();

                //Aceitar uma conexao pedida por um socket para envio de mensagegens
                SAEnvio[i] = newSock.Accept();

                //Criacao de um objecto newconnection da classe ConnectionThread
                ConnectionThread dedicatedHandle = new ConnectionThread(SARecep[i],SAEnvio[i]);

                //Criacao de uma thread para executar o codigo contido no metodo
                //HandleConnection() da classe ConnectionThread
                Thread ht = new Thread(new ThreadStart(dedicatedHandle.handleConnection));

                //iniciacao da thread
                ht.Start();
                i++;
            }while(true);
        }
    }
}
