using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;
using System.IO;

using Shiro.Interpreter;
using Shiro.Interop;

namespace Shiro.Libraries
{
    [ShiroClass("SQL", KeepClassLoaded=true)]
    public class SQL
    {
		protected Dictionary<int, SqlConnection> conns = new Dictionary<int, SqlConnection>();
        protected int LastIndex = 0;
        protected SQLiteConnection sqLiteCon = null;

        #region SQLite

        [ShiroMethod("dbOpen", 1)]
        public bool dbOpen(string path)
        {
            if (sqLiteCon != null)
                return false;

            if (!System.IO.File.Exists(path))
                SQLiteConnection.CreateFile(path);

            sqLiteCon = new SQLiteConnection("Data Source=" + path + ";Version=3;");
            sqLiteCon.Open();
            dbExec(@"CREATE TABLE IF NOT EXISTS shiroObjectStore (ID varchar(24), Value TEXT, Tuple TEXT)");

            return true;
        }

        [ShiroMethod("dbClose", 0)]
        public bool dbClose()
        {
            if (sqLiteCon == null)
                return false;
            
            sqLiteCon.Close();
            sqLiteCon = null;
            return true;
        }

        [ShiroMethod("dbSave", 2)]
        public bool dbSave(string id, Token obj)
        {
            if (sqLiteCon == null)
                return false;

            string tuple = "";
            foreach (var t in obj.tuple)
                tuple += t + " | ";

            dbExec(string.Format("Insert into shiroObjectStore (ID, Value, Tuple) Values ('{0}', '{1}', '{2}')", id, obj.ToString(), tuple));
            return true;
        }
        [ShiroMethod("dbLoad", 1)]
        public Token dbLoad(string id)
        {
            if (sqLiteCon == null)
                throw new ShiroException("[In DB Library] Attempt to load from unopened SQLite database");

            SQLiteCommand command = new SQLiteCommand("select Value, Tuple from shiroObjectStore where ID='" + id + "'", sqLiteCon);
            SQLiteDataReader reader = command.ExecuteReader();

            if(reader.Read())
            {
                string val = reader[0].ToString();
                var retVal = Token.FromString(val);
                try
                {
                    retVal.tuple =
                        new List<string>(reader[1].ToString()
                            .Split(new string[] {" | "}, StringSplitOptions.RemoveEmptyEntries));
                }
                catch (Exception ex)
                {
                    
                }

                return retVal;
            }
            return Token.FromString("0");
        }

        [ShiroMethod("dbQ", 1)]
        public Token dbQuery(string sql)
        {
            if (sqLiteCon == null)
				throw new ShiroException("[In DB Library] Attempt to query unopened SQLite database");

            SQLiteCommand command = new SQLiteCommand(sql, sqLiteCon);
            SQLiteDataReader reader = command.ExecuteReader();

            List<string> tuple = new List<string>(); ;
            List<Token> table = new List<Token>();
            while (reader.Read())
            {
                if (tuple.Count == 0)
                    for (int i = 0; i < reader.FieldCount; i++)
                        tuple.Add(reader.GetName(i));

                Token row = new Token();
                row.tuple = tuple;
                row.vt = Shiro.Interpreter.ValueType.List;
                row.list = new List<Token>();

                for (int i = 0; i < reader.FieldCount; i++)
                    row.list.Add(Token.FromString(reader[i].ToString()));
                table.Add(row);
            }

            Token retVal = new Token();
            retVal.vt = Interpreter.ValueType.List;
            retVal.tt = TokenType.Value;
            retVal.list = table;

            return retVal;
        }

        [ShiroMethod("dbExec", 1)]
        public bool dbExec(string query)
        {
            if (sqLiteCon == null)
                return false;

            var command = new SQLiteCommand(query, sqLiteCon);
            command.ExecuteNonQuery();
            return true;
        }

        #endregion

        #region Basics (Procedural DB Access)

        [ShiroMethod("sqlOpen", 1)]
        public string conDB(string conStr)
        {
            try
            {
				SqlConnection conn = new SqlConnection(conStr);
                conn.Open();
                conns.Add(++LastIndex, conn);
                return LastIndex.ToString();
            }
            catch (Exception ex)
            {
                throw new ShiroException("[In SQL Library] Failed to initiate DSN '" + conStr + "': " + ex.Message);
            }
        }

        [ShiroMethod("sqlClose", 1)]
        public string closeDB(int index)
        {
            if (!conns.ContainsKey(index))
                return "false";
            else
            {
                conns[index].Close();
                conns[index].Dispose();
                conns.Remove(index);

                return "true";
            }
        }

        [ShiroMethod("sqlExec", 2)]
		public string execNonReader(int index, string sql)
        {
            if (!conns.ContainsKey(index))
				throw new ShiroException("[In SQL Library] Invalid Database Index passed to sqlExec (make sure to sqlOpen it first)");

			SqlCommand cmd = new SqlCommand(sql, conns[index]);
            int count = cmd.ExecuteNonQuery();
            
            return count.ToString();
        }

        [ShiroMethod("sqlQuery", 2)]
		public Token execQuery(int index, string sql)
        {
            if (!conns.ContainsKey(index))
				throw new ShiroException("[In SQL Library] Invalid Database Index passed to sqlQuery (make sure to sqlOpen it first)");

			SqlCommand cmd = new SqlCommand(sql, conns[index]);
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                List<string> tuple = new List<string>(); ;
                List<Token> table = new List<Token>();
                while (dr.Read())
                {
                    if (tuple.Count == 0)
                        for (int i = 0; i < dr.FieldCount; i++)
                            tuple.Add(dr.GetName(i));

                    Token row = new Token();
                    row.tuple = tuple;
                    row.vt = Shiro.Interpreter.ValueType.List;
                    row.list = new List<Token>();

                    for (int i = 0; i < dr.FieldCount; i++)
                        row.list.Add(Token.FromString(dr[i].ToString()));
                    table.Add(row);
                }

                Token retVal = new Token();
                retVal.vt = Shiro.Interpreter.ValueType.List;
                retVal.tt = TokenType.Value;
                retVal.list = table;
                
                return retVal;
            }
        }
        
        #endregion
    }
}
