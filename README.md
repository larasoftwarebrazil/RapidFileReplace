# RapidFileReplace

RapidFileReplace

How using

namespace FileTransform
{
    class Program
    {
        public static event CurrentFileProcessing _currentFileProcessingHandler;
        static void Main(string[] args)
        {
        
            _currentFileProcessingHandler += new CurrentFileProcessing(readConsole);


            var build = new FileBuilder(@"C:\temp\", "EafManager", _currentFileProcessingHandler);
            build.Transforme(@"C:\Projetos\ProjectTemplate", "ProjectName", "EafManager", true, true);
            build.GetZipFile(Encoding.UTF8);
        }

        public static void readConsole(string fileName)
        {
            Console.WriteLine(fileName);
        }
    }
}

