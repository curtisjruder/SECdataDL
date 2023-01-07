using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SECgovDataImport
{
    internal class Tool_FileExplorer
    {
        private Tool_FileExplorer() { }

        internal static bool exists_File(string filePath)
        {
            return File.Exists(filePath);
        }

        internal static bool exists_Folder(string filePath)
        {
            filePath = getFilePath(filePath);
            if (filePath == "") return false;
            return Directory.Exists(filePath);
        }

        internal static string myPath()
        {
            return getFilePath(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8).Replace("/", "\\"));
        }

        internal static string desktopPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + '\\';
        }

        internal static string documentPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + '\\';
        }

        internal static string getFilePath(string filePath)
        {
            if (filePath.IndexOf("\\") == -1) return "";// filePath + "\\";
            return filePath.Substring(0, filePath.LastIndexOf('\\')) + '\\';
        }

        internal static string getFileName(string filePath)
        {
            int index = filePath.LastIndexOf("\\");
            if (index == -1) return "";
            if (index == filePath.Length - 1) return "";
            return filePath.Substring(index + 1);
        }
        
        internal static string getParentPath(string filePath)
        {
            filePath = getFilePath(filePath);
            return getFilePath(filePath.Substring(0, filePath.Length - 1));            
        }
    }
}
