using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// Expected format for exclusions file

/*
 
     Table:_TABLE           // Table: required prefix for the table name
     File:table.txt         // File: required prefix for the file name (optional for now)
     {
        Col1:[val1, val2]   // Col1 corresponds to the column name
        Col2:[valY]         // valY corresponds to the acceptable value(s)
     }

 */



namespace SECgovDataImport
{
    class Exclusions
    {
        private string _path;
        private Dictionary<string, Exclusion> _exclusions;
        
        internal Exclusions()
        {
            if (!setPath()) throw new Exception("Error - Couldn't find exclusions file");
            processTables();
            if(_exclusions.Count == 0) throw new Exception("Error - Couldn't process exclusions file");
        }

        private bool setPath()
        {
            string path = Tool_FileExplorer.myPath();

            while(path != "")
            {
                if (Tool_FileExplorer.exists_File(path + "exclusions")) break;
                path = Tool_FileExplorer.getParentPath(path);                
            }

            _path = path + "exclusions";
            return path != "";
        }
        
        private void processTables()
        {
            _exclusions = new Dictionary<string, Exclusion>();

            using (StreamReader file = new StreamReader(_path))
            {                
                while(!file.EndOfStream)
                {
                    processTable(file);
                }
            }

            _exclusions["_NUM"].addUniqueKeys("tag", _exclusions["_TAG"].getUniqueKeys());
            _exclusions["_NUM"].addUniqueKeys("adsh", _exclusions["_SUB"].getUniqueKeys());
        }

        private void processTable(StreamReader file)
        {
            string line = file.ReadLine();
            while (!line.Contains("Table:") && !file.EndOfStream)
                line = file.ReadLine();

            if (file.EndOfStream) return;

            List<string> tableData = new List<string>();

            while (!line.Contains("}") && !file.EndOfStream)
            {
                tableData.Add(line.Trim());
                line = file.ReadLine();
            }

            tableData.Add(line.Trim());

            if (tableData.Count == 0) return;

            _exclusions.Add(tableData[0].Substring(6).Trim(), new Exclusion(tableData));
        }

        internal List<string> getFileList()
        {
            List<string> output = new List<string>();
            foreach (Exclusion x in _exclusions.Values)
                output.Add(x.getFileName());

            return output;
        }

        internal List<string> getTableList()
        {
            List<string> output = new List<string>();
            foreach (Exclusion x in _exclusions.Values)
                output.Add(x.getTableName());

            return output;
        }

        internal bool isValidInput(string tableName, List<string> columns, List<string> values)
        {
            return _exclusions[tableName].isValidInput(columns, values);
        }
    }
}
