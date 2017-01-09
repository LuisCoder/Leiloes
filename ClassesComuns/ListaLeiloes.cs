using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassesComuns
{
    /// <summary>
    /// Esta classe é apenas instanciada no servidor de leiloes
    /// Para alem de guardar a lista de leiloes vai guardar tambem todas as operacoes relativas a leiloes
    /// </summary>
    public class ListaLeiloes
    {
        /// <summary>
        /// Lista para guardar todos os leiloes
        /// </summary>
        public static List<Leilao> Leiloes = new List<Leilao> {
            new Leilao
                {
                    Titulo = "telemovel nokia",
                    DescricaoArtigo="telemovel todo lixado",
                    Activo = true,
                    BaseDeLicitacao = 6,
                    Duracao = 240,
                    TempoFinal=DateTime.Now.AddSeconds(240),
                    Utilizador = "Joao",
                    Licitadores=new List<Licitacao> {
                        new Licitacao{
                            Login="Luis",
                            ValorLicitado=7.00f
                        },
                        new Licitacao{
                            Login="Antonio",
                            ValorLicitado=500.00f
                        }
                    }
                },
                new Leilao
                {
                    Titulo = "portatil asus",
                    DescricaoArtigo="portatil a desfazer-se aos bocados",
                    Activo = true,
                    BaseDeLicitacao = 10,
                    Duracao = 240,
                    TempoFinal=DateTime.Now.AddSeconds(240),
                    Utilizador = "Joao",
                    Licitadores=new List<Licitacao> {
                        new Licitacao{
                            Login="Luis",
                            ValorLicitado=11.00f
                        }
                    }
                },
                new Leilao
                {
                    Titulo = "opel corsa",
                    DescricaoArtigo="carro em perfeito estado a largar pecas",
                    Activo = false,
                    BaseDeLicitacao = 998,
                    Duracao = 480,
                    TempoFinal=DateTime.Now.AddSeconds(480),
                    Utilizador = "Joao",
                    Licitadores=new List<Licitacao> {
                        new Licitacao{
                            Login="Antonio",
                            ValorLicitado=1000.00f
                        }
                    }
                },
        };

        /// <summary>
        /// Construtor para numa fase inicial deste projecto colocar leiloes na lista de leiloes
        /// </summary>
        public ListaLeiloes()
        {
            //Console.WriteLine("A chamar construtor da lista de leiloes");
            //Dados dummy do servidor de leiloes
            //Utilizadores dummy
            //Leiloes.Add(
            //    new Leilao
            //    {
            //        Titulo = "telemovel nokia",
            //        DescricaoArtigo="telemovel todo lixado",
            //        Activo = true,
            //        BaseDeLicitacao = 6,
            //        Duracao = 240,
            //        TempoFinal=DateTime.Now.AddSeconds(240),
            //        Utilizador = "Joao",
            //    }
            //    );

            //Leiloes.Add(
            //    new Leilao
            //    {
            //        Titulo = "portatil asus",
            //        DescricaoArtigo="portatil a desfazer-se aos bocados",
            //        Activo = true,
            //        BaseDeLicitacao = 10,
            //        Duracao = 240,
            //        TempoFinal=DateTime.Now.AddSeconds(240),
            //        Utilizador = "Joao",
            //    }
            //    );
            //Leiloes.Add(
            //    new Leilao
            //    {
            //        Titulo = "opel corsa",
            //        DescricaoArtigo="carro em perfeito estado a largar pecas",
            //        Activo = false,
            //        BaseDeLicitacao = 1000,
            //        Duracao = 480,
            //        TempoFinal=DateTime.Now.AddSeconds(480),
            //        Utilizador = "Joao",
            //    }
            //    );
            //Licitacoes dummy
            //Leiloes[0].Licitar("Luis", 7.00f);
            //Leiloes[0].Licitar("Antonio", 500.00f);
            //Leiloes[2].Licitar("Antonio", 1000.00f);

        }

        /// <summary>
        /// Metodo para obter o numero de leiloes existente 
        /// </summary>
        /// <returns>Numero de leiloes</returns>
        public int NumLeiloes()
        {
            return Leiloes.Count();
        }

        /// <summary>
        /// Permite verificar se um utilizador ja fez licitacoes em algum leilao do servidor
        /// </summary>
        /// <param name="login">login do utilizador</param>
        /// <returns>retorna true se o utilizador ja fez alguma licitacao, false se ainda nao fez nenhuma licitacao</returns>
        public bool VerificaLicitador(string login)
        {
            foreach (Leilao lei in Leiloes)
            {
                if (lei.VerificaLicitador(login))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Permite obter um leilao com base num indice
        /// </summary>
        /// <param name="pos">recebe a posicao que queremos obter</param>
        /// <returns>Retorna o respectivo leilao</returns>
        public Leilao GetLeilao(int pos)
        {
            return Leiloes[pos];
        }

        /// <summary>
        /// Para obter uma lista de todos os leloes em que o utilizador ainda nao fez licitacoes
        /// </summary>
        /// <param name="Login">Login de utilizador para realizar a procura de leloes em que o utilizador nao licitou</param>
        /// <returns>Retorna uma string com os leiloes em que o utilizador nao licitou</returns>
        public string VerLeiloesDisp(string login)
        {
            return ToStringLeiloes(Leiloes.FindAll(a => !a.VerificaLicitador(login)).FindAll(a => a.Activo));
        }

        /// <summary>
        /// Ver lista de leiloes em que o utilizador fez licitacoes e ainda nao terminaram.
        /// Ou seja onde o atributo Activo ainda nao foi colocado a false e
        /// onde o utilizador logado foi o ultimo a licitar
        /// </summary>
        /// <param name="Login">Login de utilizador para realizar a procura de leiloes em que o utilizador licitou</param>
        /// <returns>Retorna uma string com os leiloes em que o utilizador licitou</returns>
        public string VerLeiloesActivos(string login)
        {
            return ToStringLeiloes(Leiloes.FindAll(a => a.VerificaLicitador(login)).FindAll(a => a.Activo));
        }

        /// <summary>
        /// Para adicionar um leilao ao sistema
        /// </summary>
        /// <param name="titulo">Titulo do leilao</param>
        /// <param name="descricaoArtigo">Descricao do artigo de leilao</param>
        /// <param name="baseDeLicitacao">Base inicial de licitacao</param>
        /// <param name="duracao">Tempo em segundos da duracao do leilao</param>
        /// <param name="utilizador">Utilizador que vai criar o leilao</param>
        /// <returns>retorna a posicao do elemento onde e colocado</returns>
        public int CriarLeilao(string titulo,string descricaoArtigo, float baseDeLicitacao, int duracao, string utilizador)
        {
            Leiloes.Add(
                new Leilao
                {
                    Titulo = titulo,
                    DescricaoArtigo=descricaoArtigo,
                    Activo = true,
                    BaseDeLicitacao = baseDeLicitacao,
                    Duracao = duracao,
                    TempoFinal=DateTime.Now.AddSeconds(duracao),
                    Utilizador = utilizador
                }
                );

            //no fim retorna a posicao do elemento que acabou de ser colocado na lista
            return Leiloes.Count();
        }

        /// <summary>
        /// Para imprimir todos os leiloes que tem um titulo respectivo
        /// </summary>
        /// <param name="Titulo">Titulo do leilao que se quer filtrar da lista de leiloes</param>
        /// <returns>uma string com a lista de leiloes com esse respectivo nome</returns>
        public string InfoLeilao(string titulo)
        {
            return Leiloes.Where(a => a.Titulo.Equals(titulo)).Single().ToString();
        }

        /// <summary>
        /// Metodo que permite licitar um determinado leilao
        /// </summary>
        /// <param name="NumLeilao">titulo do leilao a licitar</param>
        /// <param name="Login">Login de utilizador que vai licitar</param>
        /// /// <param name="Login">Valor licitado pelo utilizador</param>
        public void Licitar(String titulo, string login, float valorLicitado)
        {
            //Leiloes[NumLeilao].Licitadores.Add(Login);

            Leiloes.Where(a => a.Titulo.Equals(titulo)).Single().Licitar(login, valorLicitado);
        }

        /// <summary>
        /// Obtem o valor que o utilizador ja licitou em todos os leiloes
        /// </summary>
        /// <param name="login">login de utilizador para percorrer todos os leiloes</param>
        /// <returns>Uma soma dos valores licitados para todos os leiloes</returns>
        public float ValorLic(String login)
        {
            float valor = 0.00f;
            foreach (Leilao lei in Leiloes.Where(a => a.VerificaLicitador(login)))
            {
                //if (lei.VerificaLicitador(login))
                valor += lei.ValorLic(login);
            }
            return valor;
        }

        /// <summary>
        /// Para obter uma lista completa de leiloes fechados de um determinado utilizador
        /// </summary>
        /// <param name="login">login de utilizador para filtrar os leiloes criados por um determinado utilizador</param>
        /// <returns>Uma string com todos os leiloes que ja se encontram fechados de um determinado utilizador</returns>
        public string VerListaLeiloesFech(String login)
        {
            //string s = "";
            //foreach (var l in Leiloes)
            //{
            //    if(l.Utilizador.Equals(login) && !l.Activo)
            //    s += l.ToString() + "\n\n";
            //}
            return ToStringLeiloes(Leiloes.FindAll(a => a.Utilizador.Equals(login)).FindAll(a => !a.Activo));
            //return s;
        }

        /// <summary>
        /// Para obter uma lista de todos os leiloes no servidor
        /// </summary>
        /// <returns>Uma lista com todos os leiloes</returns>
        public String VerListaTodosLeiloes()
        {
            return ToStringLeiloes(Leiloes);
        }

        /// <summary>
        /// Recebe a uma lista de Leiloes e converte a mesma numa string. Esta classe ajuda a simplificar o codigo
        /// </summary>
        /// <returns></returns>
        private string ToStringLeiloes(List<Leilao> listaL)
        {
            if (listaL.Count > 0)
            {
                string s = "";
                s += "Numero de leiloes: "+listaL.Count+"\n";
                foreach (var l in listaL)
                {
                    s += "*********************************************\n";
                    s += l.ToString() + "";
                    s += "\n*********************************************\n";
                }
                return s;
            }
            else
                return "Não existe nenhum leilao";
        }
    }
}
