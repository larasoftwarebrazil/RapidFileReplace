# RapidFileReplace

RapidFileReplace

How using

    _currentFileProcessingHandler += new CurrentFileProcessing(readConsole);

    var build = new FileBuilder(@"C:\temp\", "ZipFileName", _currentFileProcessingHandler);
    build.Transforme(@"C:\temp\MyFiles", "MyText", "MyTextNew", true, true);
    build.GetZipFile(Encoding.UTF8);
     
