using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using Shiro.Interpreter;
using Shiro.Interop;

namespace Shiro.Libraries
{
    [ShiroClass("SQL", KeepClassLoaded=true)]
    public class SQL
    {
		protected Dictionary<int, SqlConnection> conns = new Dictionary<int, SqlConnection>();
        protected int LastIndex = 0;

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
