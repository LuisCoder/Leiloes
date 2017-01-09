using System.Collections.Generic;
namespace ClassesComuns
{
    /// <summary>
    /// Para guardar as informacoes de utilizador.
    /// Inicialmente so vai ter este dois atributos mas mais tarde poderao a surgir mais 
    /// </summary>
    public class Utilizador
    {
        /// <summary>
        ///     Login do utilizador
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        ///     Password do utilizador
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Para notificacao externa quando o leilao termina
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// Para guardar as notificacoes do utilizador
        /// </summary>
        public List<string> Notificacoes = new List<string>();

        /// <summary>
        /// Metodo que apenas serve para se perceber melhor o codigo quando é inserido uma nova notificacao no utilizador
        /// </summary>
        /// <param name="not"></param>
        public void InserirNotificacao(string not)
        {
            Notificacoes.Add(not);
        }
    }
}