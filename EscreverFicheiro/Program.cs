using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscreverFicheiro
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("teste.txt");
            File.WriteAllText("teste.xml", text + "DERP");
        }
    }
}
