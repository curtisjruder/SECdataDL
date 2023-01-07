using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite; 

namespace SECgovDataImport
{
    class DBCol
    {
        private string _colName;
        private string _dataType;
        public DBCol(SQLiteDataReader reader)
        {
            _colName = reader.GetString(1);
            _dataType = reader.GetString(2);
        }

        private bool isText()
        {
            return _dataType.Contains("Text");
        }
        public string convertValue(string input)
        {
            if (input.Length == 0) return "null";
            input = input.Replace("'", "");
            if (isText()) return "'" + input + "'";
            return input;
        }
        public string getName()
        {
            return _colName;
        }
    }
}
