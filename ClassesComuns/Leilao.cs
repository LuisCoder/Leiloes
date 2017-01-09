using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ClassesComuns
{
    /// <summary>
    /// Classe que guarda todas as informacoes de um leilao
    /// </summary>
    public class Leilao
    {
        /// <summary>
        /// Titulo do Leilao
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Descricao do artigo
        /// </summary>
        public string DescricaoArtigo { get; set; }

        /// <summary>
        /// Variavel que colocada a true quando o leilao e inserido no sistema. Quando o leilao termina e colocada a false
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Valor minimo de licitacao, o artigo nunca será vendido por menos que esse valor)
        /// </summary>
        public float BaseDeLicitacao { get; set; }

        /// <summary>
        /// Duracao de tempo em segundos ate o leilao terminar
        /// </summary>
        public int Duracao { get; set; }

        /// <summary>
        /// Para guardar o tempo em que vai terminar o leilao
        /// </summary>
        public DateTime TempoFinal { get; set; }

        /// <summary>
        /// Utilizador ao vendedor do artigo que registou o leilao na base de dados
        /// </summary>
        public string Utilizador { get; set; }

        /// <summary>
        /// Lista de licitadores onde e guardado instancia da classe Licitacao onde esta guardado o login do utilizador que licitou e o valor licitado
        /// </summary>
        public List<Licitacao> Licitadores = new List<Licitacao> { };

        /// <summary>
        /// Metodo que permite obter o numero de licitacoes do leilao
        /// </summary>
        /// <returns>Numero de licitacoes do leilao</returns>
        public int NumLicitacoes()
        {
            return Licitadores.Count;
        }

        /// <summary>
        /// Metodo que permite fazer a licitacao num determinado leilao
        /// </summary>
        /// <param name="login">login do utilizador que vai licitar</param>
        /// <param name="valorLicitado">valor que o utilizador vai licitar</param>
        /// <returns>retorna true se conseguir licitar ou seja se o valor que licitar for superior ao actual, false se contrario</returns>
        public bool Licitar(String login, float valorLicitado)
        {
            //verifica se o valor é maior do que o actual
            if (Licitadores.Count == 0 || valorLicitado > Licitadores.Last().ValorLicitado)
            {
                Licitadores.Add(
                    new Licitacao
                    {
                        Login = login,
                        ValorLicitado = valorLicitado
                    }
                    );
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Metodo que serve para verificar se um determinado utilizador ja fez licitacoes no leilao
        /// </summary>
        /// <param name="login"></param>
        /// <returns>return true se existe um determinado utilizador na lista de utlizadores, false se contrario</returns>
        public bool VerificaLicitador(string login)
        {
            foreach (Licitacao log in Licitadores)
            {
                if (log.Login.Equals(login))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Para obter uma soma dos valores licitados de um utilizador para um determinado leilao
        /// </summary>
        /// <param name="login">login de utilizador para pesquisar na lista as lici</param>
        /// <returns>Uma soma dos valores licitados para um determinado leilao</returns>
        public float ValorLic(string login)
        {
            float valor = 0.00f;
            foreach (Licitacao lic in Licitadores)
            {
                if (lic.Login.Equals(login))
                    valor += lic.ValorLicitado;
            }
            return valor;
        }

        /// <summary>
        /// Para verificar se o leilao ja chegou ao fim e se chegou coloca a variavel Activo a false
        /// </summary>
        /// <param name="tempo">tempo actual</param>
        /// <returns>return true se o leilao ja chegou ao fim false se contrario</returns>
        public bool VerificaTempoFinal(DateTime tempo)
        {
            if (tempo >= TempoFinal)
            {
                Activo = false;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Para obter uma lista de licitadores que ja licitaram no passado. Ou seja retorna toda lista de licitadores excepto o vencedor do leilao
        /// </summary>
        /// <returns>lista de string dos licitadores excepto o vencedor do leilao</returns>
        public List<string> LicitadoresPassados()
        {
            List<string> lict = new List<string>();
            for (int i = 0; i < Licitadores.Count - 1; i++)
            {
                //adiciona os logins de utilizadores que ainda nao se encontram na lista
                if(lict.Where(a => a.Equals(Licitadores[i].Login)).ToList().Count()==0)
                    lict.Add(Licitadores[i].Login);
            }
            return lict;
        }

        /// <summary>
        /// Obter licitador que esta a ganhar ou ganhou o leilao
        /// </summary>
        /// <returns>Ultimo licitador</returns>
        public string Comprador()
        {
            return Licitadores[Licitadores.Count - 1].Login;
        }

        /// <summary>
        /// Para obter o valor final das licitacoes
        /// </summary>
        /// <returns></returns>
        public float ValorFinalLicitacoes()
        {
            return Licitadores[Licitadores.Count - 1].ValorLicitado;
        }

        /// <summary>
        /// Obter o ultimo valor licitado por um utilizador
        /// </summary>
        /// <param name="login">login de utilizador</param>
        /// <returns>ultimo valor licitado pelo utilizador 0.00 se nao tiver licitado nada</returns>
        public float UltimoValorLicitado(string login)
        {
            float ultValLic=0.00f;
            foreach (Licitacao lic in Licitadores)
            {
                if (lic.Login.Equals(login))
                    ultValLic = lic.ValorLicitado;
            }
            return ultValLic;
        }

        /// <summary>
        /// Para gerar a string de notificacao A
        /// para os compradores que fizeram licitacoes mas nao ganharam o leilao
        /// </summary>
        /// <param name="compradorLogin">login do licitar </param>
        /// <returns>string com a notificacao A</returns>
        public string NotificacaoA(string compradorLogin)
        {
            string s = "\n";
            s += "Notificacao A\n";
            s += "Fim de leilao sem sucesso\n";
            s += "Titulo:                     "+Titulo+"\n";
            s += "Descricao do artigo:        "+DescricaoArtigo+"\n";
            s += "Valor final das licitacoes: " + ValorFinalLicitacoes()+"\n";
            s += "Ultimo valor licitado:      " + UltimoValorLicitado(compradorLogin)+"\n";
            return s;
        }

        /// <summary>
        /// Para gerar a string de notificacao B
        /// para o comprador que ganhou o leilao
        /// </summary>
        /// <returns>string com a notificao B</returns>
        public string NotificacaoB()
        {
            string s = "\n";
            s += "Notificacao B\n";
            s += "Fim de leilao com sucesso\n";
            s += "Titulo:                     " + Titulo + "\n";
            s += "Descricao do artigo:        " + DescricaoArtigo + "\n";
            s += "Valor final das licitacoes: " + ValorFinalLicitacoes() + "\n";
            return s;
        }

        /// <summary>
        /// Para gerar a string de notificacao C
        /// para o vendedor quando o tempo de duracao do leilao chega ao fim sem licitacoes
        /// </summary>
        /// <returns>string com a notificacao C</returns>
        public string NotificacaoC()
        {
            string s="\n";
            s += "Notificacao C\n";
            s += "Fim de leilao sem sucesso\n";
            s += "Titulo:              " + Titulo + "\n";
            s += "Descricao do artigo: " + DescricaoArtigo + "\n";
            return s;
        }

        /// <summary>
        /// Para gerar a string de notificacao D
        /// para o vendedor quando o tempo de duracao do leilao chega ao fim e existiram licitacoes
        /// </summary>
        /// <returns>string com a notificacao D</returns>
        public string NotificacaoD()
        {
            string s="\n";
            s += "Notificacao D\n";
            s += "Fim de leilao com sucesso\n";
            s += "Titulo:              " + Titulo + "\n";
            s += "Descricao do artigo: " + DescricaoArtigo + "\n";
            s += "Valor final:         " + ValorFinalLicitacoes() + "\n\n";
            s += "Informacao do comprador:\n";
            s += "Login:               "+Comprador();
            return s;
        }

        /// <summary>
        /// Para mandar uma notificacao por mail ao vencedor e comprador do artigo do leilao quando o mesmo chega ao fim
        /// </summary>
        /// <param name="mailVendedor">mail do vendedor do artigo deste leilao</param>
        /// <param name="mailComprador">mail do comprador do artigo deste leilao</param>
        public void MandarMail(string mailVendedor,string mailComprador)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add(mailVendedor);//Para quem se vai enviar o mail
            mailMsg.To.Add(mailComprador);//Para quem se vai enviar o mail
            // From
            MailAddress mailAddress = new MailAddress("lcostasines@hotmail.com");
            mailMsg.From = mailAddress;

            // Subject and Body
            mailMsg.Subject = Titulo;
            mailMsg.Body = "O leilao "+Titulo+" terminou com "+ValorFinalLicitacoes();

            // Init SmtpClient and send on port 587 in my case. (Usual=port25)
            SmtpClient smtpClient = new SmtpClient("smtp.live.com", 587);
            smtpClient.Credentials = new System.Net.NetworkCredential("lcostasines@hotmail.com", "");//falta colocar aqui a password do conta
            smtpClient.EnableSsl = true;

            //Envio do mail
            try
            {
                smtpClient.Send(mailMsg);
            }
            catch (Exception e)
            {
                Console.WriteLine("falha ao entregar a notificacao com o seguinte erro:");
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Redefenicao do metodo tostring para imprimir a descricao de um leilao
        /// </summary>
        /// <returns>string com a descricao dos atributos do leilao</returns>
        public override string ToString()
        {
            String s = "";
            s += "Titulo:              " + Titulo + "\n";
            s += "Descricao do artigo: " + DescricaoArtigo + "\n";
            s += "Estado :             " + (Activo ? "Leilao esta a decorrer\n" : "Leilao terminou\n");
            s += "Base de licitacao:   " + BaseDeLicitacao + "\n";
            s += "Duracao:             " + Duracao + "\n";
            s += "Tempo Final:         " + TempoFinal + "\n";
            s += "Utilizador:          " + Utilizador + "\n";

            if (NumLicitacoes() != 0)
            {
                //Imprimir todos os licitadores
                s += "\nLicitadores:\n";
                foreach (Licitacao lic in Licitadores)
                {
                    s += "Nome:           " + lic.Login + "\n";
                    s += "Valor licitado: " + lic.ValorLicitado.ToString("0.00") + "\n\n";
                }
            }
            else
            {
                s += "\nEste leilao ainda nao tem licitacoes";
            }

            return s;
        }
    }
}
