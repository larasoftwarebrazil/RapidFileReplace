# RapidFileReplace

RapidFileReplace

How using

        
        public static event CurrentFileProcessing _currentFileProcessingHandler;
        
        static void Main(string[] args)
        {

            _currentFileProcessingHandler += new CurrentFileProcessing(readConsole);

            var build = new FileBuilder(@"C:\temp\", "NewProjectName", _currentFileProcessingHandler);
           
            build.Transform(@"C:\temp\ProjectTemplate", "ProjectName", "NewProjectName",true, true);
            
            build.GetZipFile(Encoding.UTF8);
        }
