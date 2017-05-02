using Ionic.Zip;
using LaraSoftware.FileTransform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileTransform
{
    class Program
    {
        public static event CurrentFileProcessing _currentFileProcessingHandler;
        static void Main(string[] args)
        {
        
            _currentFileProcessingHandler += new CurrentFileProcessing(readConsole);


            var build = new FileBuilder(@"C:\Projetos novos\", "EafManager", _currentFileProcessingHandler);
            build.Transforme(@"C:\Projetos novos\EafManager\", "EafManager", "Manager", true, true);
            build.GetZipFile(Encoding.UTF8);
        }

        public static void readConsole(string fileName)
        {
            Console.WriteLine(fileName);
        }
    }
}
