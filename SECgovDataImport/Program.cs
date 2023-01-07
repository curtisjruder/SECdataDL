using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace SECgovDataImport
{
    class Program
    {
        private static string zipPath;
        private static string dbPath;

        static void Main(string[] args)
        {
            Exclusions x = new Exclusions();
      
            if (!isValidArgs(args)) return;
            if (!hasValidFiles(x.getFileList())) return;

            processFiles(x);
        }

        private static bool isValidArgs(string[] args)
        {
            if (args.Length != 2)
            {
               Console.WriteLine("Error - Arg[0] = zipPath  |  Arg[1] = dbPath");
                return false;
            }

            zipPath = args[0];

            if (!System.IO.File.Exists(zipPath))
            {
                Console.WriteLine("Error - Zip path not found");
                return false;
            }

            dbPath = args[1];
            if (!System.IO.File.Exists(dbPath))
            {
                Console.WriteLine("Error - Database path not found");
                return false;
            }

            return true;
        }

        private static bool hasValidFiles(List<string> files)
        {

            using (ZipArchive x = ZipFile.OpenRead(zipPath))
            {                
                foreach(ZipArchiveEntry zipFile in x.Entries){
                    if (files.Contains(zipFile.Name))
                    {
                        files.Remove(zipFile.Name);
                    }                   
                }
            }

            if (files.Count != 0)
            {
                Console.WriteLine("Error - Missing file exception");
                Console.Write("");

                foreach (string strX in files)
                    Console.Write('\t' + strX);
            }
            return files.Count == 0;            
        }        

        private static void processFiles(Exclusions x)
        {
            DB database;
            try
            {
                database = new DB(dbPath, x);
            }
            catch
            {
                Console.WriteLine("ERROR - DB failed to open");
                return;
            }

            List<string> fileList = x.getFileList();
            List<string> tableList = x.getTableList();

            List<InputTable> tables = new List<InputTable>();
            for (int i = 0; i < fileList.Count; i++)
                tables.Add(new InputTable(database, fileList[i], tableList[i]));

            using ZipArchive zips = ZipFile.OpenRead(zipPath);
            foreach (InputTable table in tables)
                table.processFile(zips);
        }
    }
}
