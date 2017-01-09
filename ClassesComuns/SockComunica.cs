using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ClassesComuns
{
    /// <summary>
    /// Classe para facilitar a comunicacao atraves da serializacao
    /// </summary>
    public class SockComunica
    {
        //atributos para o envio
        private Socket sEnvio;
        private byte[] bufferEnvio;

        //atributos para a recepcao
        private Socket sRecepcao;
        private byte[] bufferRecepcao;

        /// <summary>
        /// Construtor que recebe como argumento a referencia do socket do qual envia ao recebe dados
        /// </summary>
        public SockComunica(Socket sEnvio, Socket sRecepcao)
        {
            this.sEnvio = sEnvio;
            bufferEnvio = BufferIni();

            this.sRecepcao = sRecepcao;
            bufferRecepcao=BufferIni();
        }

        /// <summary>
        /// Recebe como argumento qualquer tipo de objecto desde que seja um tipo permitivo do C# e envia o mesmo pela socket
        /// Usa para isso a serialização de objectos
        /// </summary>
        /// <param name="o"></param>
        public void EnviaSer(Object o)
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, o);
            bufferEnvio = mem.ToArray();
            sEnvio.Send(bufferEnvio);
            System.Threading.Thread.Sleep(250);//sleep apenas para ver o codigo a andar devagar
            bufferEnvio = BufferIni();
        }

        /// <summary>
        /// Metodo que retorna um objecto que recebe pela socket
        /// Usa para isso a serialização de objectos
        /// </summary>
        /// <returns>Retorna um objecto. É necessario fazer um cast quando for necessario usar este metodo</returns>
        public Object RecebeSer()
        {
            sRecepcao.Receive(bufferRecepcao);
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            mem.Write(bufferRecepcao, 0, bufferRecepcao.Length);
            mem.Seek(0, 0);
            Object o = bin.Deserialize(mem);
            System.Threading.Thread.Sleep(250);//sleep apenas para ver o codigo a andar devagar
            bufferRecepcao = BufferIni();
            return o;
        }

        /// <summary>
        /// Metodo privado que serve para inicialiar os buffers desta classe
        /// </summary>
        /// <returns></returns>
        private byte[] BufferIni()
        {
            return new byte[10240];
        }

        ///// <summary>
        ///// metodo que recebe da stream uma string e a retorna
        ///// </summary>
        ///// <returns>mensagem que vem da socket</returns>
        //public string RecebeString()
        //{
        //    string mensagem = Encoding.ASCII.GetString(buffer, 0, s.Receive(buffer));
        //    buffer = new byte[1024];
        //    return mensagem;
        //}

        ///// <summary>
        ///// Metodo que permite enviar uma string pela socket
        ///// </summary>
        ///// <param name="mensagem">mensagem que queremos enviar pela socket</param>
        //public void EnviaString(string mensagem)
        //{
        //    buffer = Encoding.ASCII.GetBytes(mensagem);
        //    s.Send(buffer);
        //    buffer = new byte[1024];
        //}

        //public int RecebeInt()
        //{
        //    return 0;
        //}

        //public void EnviaInt(int i)
        //{
            
        //}
    }
}
