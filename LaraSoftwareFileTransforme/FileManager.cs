using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LaraSoftware.FileTransform
{
    public class FileManager : IFileManager
    {
        public event CurrentFileProcessing _currentFileProcessingHandler;


        public FileManager(CurrentFileProcessing currentFileProcessingHandler)
        {
            _currentFileProcessingHandler = currentFileProcessingHandler;
        }

        public void MoveDirectory(string source, string target)
        {
            var stack = new Stack<Folders>();
            stack.Push(new Folders(source, target));

            while (stack.Count > 0)
            {
                var folders = stack.Pop();
                Directory.CreateDirectory(folders.Target);
                foreach (var file in Directory.GetFiles(folders.Source, "*.*"))
                {
                    string targetFile = Path.Combine(folders.Target, Path.GetFileName(file));
                    if (File.Exists(targetFile)) File.Delete(targetFile);
                    File.Move(file, targetFile);

                    _currentFileProcessingHandler(targetFile);
                }

                foreach (var folder in Directory.GetDirectories(folders.Source))
                {
                    stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
                    _currentFileProcessingHandler(folders.Target);
                }
            }
            Directory.Delete(source, true);
        }
        public class Folders
        {
            public string Source { get; private set; }
            public string Target { get; private set; }

            public Folders(string source, string target)
            {
                Source = source;
                Target = target;
            }
        }
        public void DirectoryCopy(DirectoryInfo directoryInfo, string destDirName, bool copySubDirs,
             string searchText = "", string replaceText = "")
        {
            string temppath = "";
            DirectoryInfo[] dirs = directoryInfo.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = directoryInfo.GetFiles();

            foreach (FileInfo file in files)
            {
                if (string.IsNullOrEmpty(destDirName) || String.IsNullOrEmpty(searchText))
                    temppath = Path.Combine(destDirName, file.Name);
                else
                    temppath = Path.Combine(destDirName, file.Name.Replace(searchText, replaceText));

                file.CopyTo(temppath, false);
                _currentFileProcessingHandler(temppath);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    if (string.IsNullOrEmpty(destDirName) || String.IsNullOrEmpty(searchText))
                        temppath = Path.Combine(destDirName, subdir.Name);
                    else
                        temppath = Path.Combine(destDirName, subdir.Name.Replace(searchText, replaceText));

                    DirectoryCopy(subdir, temppath, copySubDirs, searchText, replaceText);
                    _currentFileProcessingHandler(temppath);
                }
            }

        }

        public void Replace(DirectoryInfo directoryInfo, string searchText, string replaceText)
        {

            DirectoryInfo[] dirs = directoryInfo.GetDirectories();
            FileInfo[] files = directoryInfo.GetFiles();

            foreach (FileInfo file in files)
            {
                ReplaceInFile(file.FullName, searchText, replaceText);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                Replace(subdir, searchText, replaceText);
            }
        }

        public void ReplaceInFile(
                   string filePath, string searchText, string replaceText)
        {

            var content = string.Empty;
            using (StreamReader reader = new StreamReader(filePath))
            {
                content = reader.ReadToEnd();
                reader.Close();
            }

            content = Regex.Replace(content, searchText, replaceText);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(content);
                writer.Close();
            }
        }


        /// <summary>
        /// ZipFolder
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="AlternateEncoding"></param>
        /// <returns></returns>
        public byte[] ZipFolder(DirectoryInfo directoryInfo, Encoding AlternateEncoding, string zipFileName = "ZipFileName")
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncoding = AlternateEncoding;
                zip.AddDirectory(directoryInfo.FullName);
                zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
                var localizationFile = directoryInfo.FullName + @"\" + zipFileName + ".zip";
                zip.Save(localizationFile);
                byte[] fileBytes = File.ReadAllBytes(localizationFile);
                return fileBytes;
            }
        }
    }
}
