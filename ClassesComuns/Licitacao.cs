using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassesComuns
{
    /// <summary>
    /// Classe usada para representar as licitacoes feitas por um utilizador
    /// </summary>
    public class Licitacao
    {
        /// <summary>
        /// Login do utilizador que fez a licitacao
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Valor licitado por esse mesmo utilizador
        /// </summary>
        public float ValorLicitado { get; set; }
    }
}
