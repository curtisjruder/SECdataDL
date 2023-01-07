using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.IO.Compression;
using System.Linq;

namespace SECgovDataImport
{
    class InputTable
    {
        private DB _db;
        private Dictionary<string, int> columnData;
        private char delimiter = '\t';

        private string _fileName;
        private string _tblName;

        public InputTable(DB db, string fileName, string tblName)
        {
            _db = db;
            _fileName = fileName;
            _tblName = tblName;
        }

        public void processFile(ZipArchive zips)
        {
            foreach (ZipArchiveEntry zip in zips.Entries)
            {
                if (_fileName == zip.Name)
                {
                    using StreamReader reader = new StreamReader(zip.Open());
                    processFile(reader);
                    break;
                }
            }
        }

        private void processFile(StreamReader reader)
        {
            configColumnData(reader.ReadLine());

            while (!reader.EndOfStream)
            {
                processLine(reader.ReadLine());
            }

            _db.commitUpdate();
                
        }

        private void configColumnData(string firstLine)
        {
            columnData = new Dictionary<string, int>();

            int i = 0;
            foreach(string x in firstLine.Split(delimiter))
            {
                if (_db.isDBColumn(_tblName, x)) columnData.Add(x, i);
                i++;
            }
        }

        private void processLine(string line)
        {
            _db.insertValues(_tblName, getQuery_Column(), getQuery_Values(line));   
        }

        private List<string> getQuery_Column()
        {
            return columnData.Keys.ToList();
        }

        private List<string> getQuery_Values(string line)
        {
            string[] values = line.Split(delimiter);

            List<string> output = new List<string>();
            foreach (int i in columnData.Values)
            {   
                
                output.Add(values[i]);
            }

            return output;
        }        
    }
}
