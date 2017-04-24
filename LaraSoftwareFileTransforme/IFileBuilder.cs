using System.Text;

namespace LaraSoftware.FileTransform
{
    public interface IFileBuilder
    {
        byte[] GetZipFile(Encoding encoding);
        void Transforme(string sourceDirName, string searchText = "", string replaceText = "", bool replaceContent = false, bool copySubDirs = false);
    }
}