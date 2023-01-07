using System;
using System.Collections.Generic;
using System.Text;

namespace SECgovDataImport
{
    class Exclusion
    {
        private List<string> input;
        private string _tableName;
        private string _fileName;

        private Dictionary<string, Dictionary<string, string>> validInput;
        private Dictionary<string, string> uniqueKeys;

        public Exclusion(List<string> data)
        {
            input = data;
            _tableName = getInputValue("Table:");
            _fileName = getInputValue("File:");
            setExclusions();
            uniqueKeys = new Dictionary<string, string>();
        }

        private void setExclusions()
        {
            validInput = new Dictionary<string, Dictionary<string, string>>();
            int iSt = getSearchIndex("{");

            for(int i = iSt; i < input.Count; i++)
            {
                setExclusion(i);
            }
        }

        private void setExclusion(int i)
        {
            int index = input[i].IndexOf(":");
            if (index == -1) return;

            string label = input[i].Substring(0, index).Trim();
            validInput.Add(label, new Dictionary<string, string>());

            string values = input[i].Substring(index + 1).Trim(); // Get the full line of text
            values = values.Substring(1, values.Length - 2); // Remove the brackets

            foreach(string x in values.Split(","))
            {
                validInput[label].Add(x.Trim(), x.Trim());
            }
        }

        private int getSearchIndex(string search)
        {

            for(int i = 0; i < input.Count; i++)
            {
                if (input[i].Contains(search)) return i;
            }
            return -1;
        }

        private string getInputValue(string search)
        {
            int i = getSearchIndex(search);
            if (i == -1) return ""; // FAIL
            return input[i].Substring(search.Length).Trim();  // SUCCESS          
        }


        internal string getFileName()
        {
            return _fileName;
        }

        internal string getTableName()
        {
            return _tableName;
        }
               
        internal bool isValidInput(List<string> columns, List<string> values)
        {
            string col, val;
            val = "";
            for(int i = 0; i < columns.Count; i++)
            {
                col = columns[i];
                if (!isValidInput(columns[i], values[i])) return false;
                if (col == "tag" || col == "adsh") val = values[i];
            }

            addUniqueKey(val);
            return true;
        }

        private bool isValidInput(string column, string value)
        {
            
            if (!validInput.ContainsKey(column)) return true;
            return validInput[column].ContainsKey(value);
        }

        private void addUniqueKey(string value)
        {
            if (_tableName == "_NUM") return;
            if (value == "") return;
            if (uniqueKeys.ContainsKey(value)) return;

            uniqueKeys.Add(value, value);
        }

        internal Dictionary<string, string> getUniqueKeys()
        {
            return uniqueKeys;
        }

        internal void addUniqueKeys(string label, Dictionary<string, string> xKeys)
        {
            validInput.Add(label, xKeys);
        }
    }
}
