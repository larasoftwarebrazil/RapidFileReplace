using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace LaraSoftware.FileTransform
{
    public delegate void CurrentFileProcessing(string nameFile);

    public class FileBuilder : IFileBuilder
    {
        private readonly FileManager _fileManager;
        private readonly string _pathTemp;
        private readonly string _zipFileName;

        /// <summary>
        /// File Builder
        /// </summary>
        /// <param name="OutPathTemp">Path Destination Files and Zip</param>
        /// <param name="zipFileName">Name File Zip</param>
        public FileBuilder(string OutPathTemp, string zipFileName, CurrentFileProcessing currentFileProcessingHandler)
        {
            _fileManager = new FileManager(currentFileProcessingHandler);
            _pathTemp += OutPathTemp += @"\FileTransform\" + zipFileName;
            _zipFileName = zipFileName;

            if (Directory.Exists(_pathTemp))
                Directory.Delete(_pathTemp, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="searchText">Optional</param>
        /// <param name="replaceText">Optional</param>
        /// <param name="replaceContent">Optional</param>
        /// <param name="copySubDirs">Optional</param>
        /// <returns></returns>
        public void Transform(string sourceDirName, string searchText = "", string replaceText = "", bool replaceContent = false,
             bool copySubDirs = false)
        {
            //Check directory
            #region [Check Directory]
            //Verify Exists Directory
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            #endregion


            //Copy Files with Replace Directory
            _fileManager.DirectoryCopy(dir, _pathTemp, copySubDirs, searchText, replaceText);

            //Replaces Subdirectory Files
            if (replaceContent)
            {
                dir = new DirectoryInfo(_pathTemp);

                _fileManager.Replace(dir, searchText, replaceText);
            }
        }

        /// <summary>
        /// return zip bytes
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public byte[] GetZipFile(Encoding encoding)
        {
            return _fileManager.ZipFolder(new DirectoryInfo(_pathTemp), encoding, _zipFileName);
        }

        /// <summary>
        /// Get Name Path
        /// </summary>
        /// <returns></returns>
        private string GetNamePath()
        {
            var randomTest = new Random();
            TimeSpan newSpan = new TimeSpan(0, randomTest.Next(0, (int)DateTime.Now.TimeOfDay.TotalMinutes), 0);

            return newSpan.ToString();
        }
    }
}
