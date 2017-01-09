using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ConsoleApplication1
{
    public class clsPerson
    {
        public string FirstName { get; set; }
        public string MI { get; set; }
        public string LastName { get; set; }
    }

    class Program
    {
        private static void Main(string[] args)
        {
            //fonte que me ajudou a escrever isto http://tech.pro/tutorial/798/csharp-tutorial-xml-serialization
            // comentar/descomentar para testar
            //escrever();
            // comentar/descomentar para testar
            ler();
        }

        public static void escrever()
        {
            //criar objectos e colocar os atributos

            var p1 = new clsPerson
                {
                    FirstName = "Luis",
                    MI = "LC",
                    LastName = "Costa"
                };

            var p2 = new clsPerson
                {
                    FirstName = "Joao",
                    MI = "JC",
                    LastName = "Costa"
                };

            var persons = new List<clsPerson> {p1, p2};

            //criar um objecto do tipo XmlSerializer
            var x = new XmlSerializer(typeof (List<clsPerson>));

            //Stream para escrever para ficheiro
            TextWriter textWriter = new StreamWriter("leituras.xml", true);

            //Serializar entao o objecto
            x.Serialize(textWriter, persons);

            //Fechar a stream de escrita para o ficheiro
            textWriter.Close();

            Console.ReadLine();
        }

        public static void ler()
        {
            //Criar o objecto para dessrializar
            var x = new XmlSerializer(typeof (List<clsPerson>));

            //Stream para ler de um ficheiro
            TextReader textReader = new StreamReader("leituras.xml");

            //Desserializar entao o objecto
            var persons = (List<clsPerson>)x.Deserialize(textReader);

            //fechar a stream de leitura do ficheiro
            textReader.Close();

            //Mostrar na consola o objecto
            foreach (clsPerson p in persons)
            {
                Console.WriteLine("First Name: "+p.FirstName);
                Console.WriteLine("MI: "+p.MI);
                Console.WriteLine("Last Name: "+p.LastName);

            }
            Console.ReadLine();
        }
    }
}