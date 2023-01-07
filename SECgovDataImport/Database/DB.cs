using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace SECgovDataImport
{
    class DB
    {
        private SQLiteConnection _db;
        private SQLiteTransaction _dbTrans;
        private Exclusions _exclusions;

        private string _dbPath;

        private Dictionary<string, Dictionary<string, DBCol>> schema;
       
        public DB(string dbPath, Exclusions exclusions)
        {
            _exclusions = exclusions;

            _db = new SQLiteConnection();
            _db.ConnectionString = "Data Source=" + dbPath + ";";
            _db.Open();
            
            if (_db.State != System.Data.ConnectionState.Open) throw new Exception("ERROR - Invalid DB Connection");

            _dbPath = dbPath;

            processTableSchema();
            processColumnSchema();
        }

        private void processTableSchema()
        {
            schema = new Dictionary<string, Dictionary<string, DBCol>>();

            using SQLiteCommand cmd = _db.CreateCommand();
            cmd.CommandText = "PRAGMA table_list";
            using SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string name = reader.GetString(1);
                if(!name.StartsWith("sqlite")) schema.Add(name, new Dictionary<string, DBCol>());
            }                    
        }

        private void processColumnSchema()
        {
            foreach(string tbl in schema.Keys)
            {
                processColumnSchema(tbl);
            }
        }

        private void processColumnSchema(string tableName)
        {
            using SQLiteCommand cmd = _db.CreateCommand();
            cmd.CommandText = "PRAGMA table_info(" + tableName + ")";
            using SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DBCol x = new DBCol(reader);
                schema[tableName].Add(x.getName(), x);
             }
         }

        public void executeCommand(string cmd) {
            if (_dbTrans is null) _dbTrans = _db.BeginTransaction();

            using SQLiteCommand sqlCmd = _db.CreateCommand();

            sqlCmd.CommandText = cmd;
            try
            {
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                writeErrorMsg(cmd, e.Message);
            }            
        }

        private void writeErrorMsg(string cmd, string err)
        {
            using (StreamWriter output = File.AppendText(getPath() + "DBError.txt"))
            {
                output.WriteLine(new String('-', 90));
                output.WriteLine(new String('-', 90));
                output.WriteLine("");
                output.WriteLine(cmd);
                output.WriteLine("");
                output.WriteLine(err);
                output.WriteLine("");
            }
        }

        private string getPath()
        {
            return _dbPath.Substring(0,_dbPath.LastIndexOf('\\')+1);
        }

        public void commitUpdate()
        {
            if (_dbTrans is null) return;
            _dbTrans.Commit();
            _dbTrans = null;
        }

        public bool isDBColumn(string tblName, string colName)
        {
            return schema[tblName].ContainsKey(colName);
        }

        public void insertValues(string tableName, List<string> colNames,List<string> values)
        {            
            for(int i = 0; i < colNames.Count; i++)
            {
                values[i] = schema[tableName][colNames[i]].convertValue(values[i]);
            }

            if (!_exclusions.isValidInput(tableName, colNames, values)) return;
            executeCommand("Insert into " + tableName + convertColNames(colNames) + " values (" + String.Join(",", values) + ")");
        }


        private string convertColNames(IEnumerable<string> colNames)
        {
            return "(" + String.Join(",", colNames) + ")";
        }
    }
}
