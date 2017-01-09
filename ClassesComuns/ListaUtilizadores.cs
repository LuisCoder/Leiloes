using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassesComuns
{
    /// <summary>
    /// Esta classe e apenas instanciada no servidor de leiloes
    /// </summary>
    public class ListaUtilizadores
    {
        /// <summary>
        /// Lista de todos os utilizadores do sistema
        /// </summary>
        public static List<Utilizador> Utilizadores = new List<Utilizador> {
            new Utilizador { 
                Login = "Luis", 
                Password = System.Text.Encoding.ASCII.GetString(System.Security.Cryptography.MD5CryptoServiceProvider.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes("Costa"))),
                Mail="lcostasines@hotmail.com"
            },
            new Utilizador { 
                Login = "Joao", 
                Password = System.Text.Encoding.ASCII.GetString(System.Security.Cryptography.MD5CryptoServiceProvider.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes("Costa"))),
                Mail="lcostasines@hotmail.com"
            },
            new Utilizador { 
                Login = "Antonio" , 
                Password = System.Text.Encoding.ASCII.GetString(System.Security.Cryptography.MD5CryptoServiceProvider.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes("Costa"))),
                Mail="lcostasines@hotmail.com"
            }
        };

        public ListaUtilizadores() { }

        /// <summary>
        /// Para obter um utilizador com um dado login
        /// </summary>
        /// <param name="login">login de utilizador</param>
        /// <returns>um objecto do tipo utilizador</returns>
        public Utilizador GetUtilizador(string login)
        {
            return Utilizadores.Where(a => a.Login.Equals(login)).Single();
        }

        /// <summary>
        /// Para verificar se existe um determinado utilizador com esse login
        /// </summary>
        /// <param name="Utilizador">login de utilizador a verificar se existe</param>
        /// <returns>return true se existir um utilizador com esse login. False se não existir</returns>
        public bool VerificarUtilizador(string Login)
        {
            foreach (Utilizador Util in Utilizadores)
            {
                if (Util.Login.Equals(Login))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Para verficar se existe um determinado login e respectiva password
        /// </summary>
        /// <param name="Login">Login de utilizador a verficar</param>
        /// <param name="Password">Password de utilizador</param>
        /// <returns></returns>
        public bool VerificarPassWord(string Login, string Password)
        {
            foreach (Utilizador Util in Utilizadores)
            {
                if (Util.Login.Equals(Login) && Util.Password.Equals(Password))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Para obter o endereço mail de um determinado utilzador
        /// </summary>
        /// <param name="login">login de utilizador</param>
        /// <returns>endereco mail</returns>
        public string ObterEnderecoMail(string login)
        {
            return Utilizadores.Where(a => a.Login.Equals(login)).Single().Mail;
        }

        /// <summary>
        /// Para obter o id de utilizador
        /// </summary>
        /// <param name="Login">login de um utilizador</param>
        /// <returns></returns>
        public int ObterIdUtilizador(string Login)
        {
            int n = 0;
            foreach (Utilizador Util in Utilizadores)
            {
                if (Util.Login.Equals(Login))
                    return n;
                else
                    n++;
            }
            return -1;
        }
    }
}
