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

        public string teste { get { return ""; } }
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
                {
                     if (file.Name.Contains("-") && file.Name.Contains(".ts"))
                        temppath = Path.Combine(destDirName, file.Name.Replace(SnakeCase(searchText), SnakeCase(replaceText)));
                     else
                        temppath = Path.Combine(destDirName, file.Name.Replace(searchText, replaceText));

                }
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

                if (file.FullName.Contains("png") ||
                    file.FullName.Contains("jpg") ||
                    file.FullName.Contains("gif") ||
                    file.FullName.Contains("bmp") || file.FullName.Contains("ico"))
                    continue;



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

            content = Regex.Replace(content, UpperCase(searchText), UpperCase(replaceText));
            content = Regex.Replace(content, LowerCase(searchText), LowerCase(replaceText));
            content = Regex.Replace(content, FirstCharToLower(searchText), FirstCharToLower(replaceText));
            content = Regex.Replace(content, SnakeCase(searchText), SnakeCase(replaceText));

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

        public static string FirstCharToLower(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("is null!");
            return input.First().ToString().ToLower() + String.Join("", input.Skip(1));
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("is null!");
            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
        }

        public static string SnakeCase(string replaceText)
        {
            var regex = new Regex(@"[a-z]", RegexOptions.IgnoreCase);
            var text = regex.Replace(replaceText, m => separateword(m.ToString()));

            return text.First().ToString().Replace("-", "") + String.Join("", text.Skip(1));
        }

        private static string separateword(string x)
        {
            if (x.Any(char.IsUpper))
                x = "-" + x.ToString().ToLower();

            return x;   
        }

        public static string UpperCase(string replaceText)
        {
            var regex = new Regex(@"\b[A-Z]", RegexOptions.IgnoreCase);
            return regex.Replace(replaceText, m => m.ToString().ToUpper());
        }

        public static string LowerCase(string replaceText)
        {
            var regex = new Regex(@"[a-z]", RegexOptions.IgnoreCase);
            return regex.Replace(replaceText, m => m.ToString().ToLower());
        }

    }
}
