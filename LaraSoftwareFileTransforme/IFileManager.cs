using System.IO;
using System.Text;

namespace LaraSoftware.FileTransform
{
    public interface IFileManager
    {
        void MoveDirectory(string source, string target);
        void DirectoryCopy(DirectoryInfo directoryInfo, string destDirName, bool copySubDirs,
             string searchText = "", string replaceText = "");
        void Replace(DirectoryInfo directoryInfo, string searchText, string replaceText);
        void ReplaceInFile(string filePath, string searchText, string replaceText);
        byte[] ZipFolder(DirectoryInfo directoryInfo, Encoding AlternateEncoding, string zipFileName = "zipFileName");
    }
}