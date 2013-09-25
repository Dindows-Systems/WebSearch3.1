using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using WebSearch.DataCenter.Net.DS;
using System.Collections;

namespace WebSearch.DataCenter.Net.MSSQL
{
    /// <summary>
    /// Summary description for MSSQLHelper
    /// </summary>
    public class MSSQLHelper
    {
        #region Microsoft SQL Server Connection

        protected SqlConnection _conn = null;

        /// <summary>
        /// The connection object of the db
        /// </summary>
        public SqlConnection Connection
        {
            get { return this._conn; }
        }

        /// <summary>
        /// Get the connection to the specified path. The connection has already been opened when returned.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SqlConnection GetConnection(String path)
        {
            SqlConnection connection = new SqlConnection(path);
            connection.ConnectionString += ";async=true";
            connection.Open();

            // It's optional to evoke '*DAO.end' after use of the connection
            return connection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public SqlCommand GetSqlCommand(string cmdText)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = this._conn;
            cmd.CommandText = cmdText;
            cmd.CommandTimeout = int.MaxValue;
            return cmd;
        }

        #endregion

        #region MSSQLHelper Constructors

        protected MSSQLHelper(DataSource ds)
        {
            try
            {
                this._conn = GetConnection(ds.Path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region MSSQLHelper Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        protected string BuildSQLFTSCondition(Hashtable conditions, MatchType match)
        {
            if (conditions == null) return ""; // return "" for no condition

            string conditionSql = "";
            foreach (string field in conditions.Keys)
            {
                conditionSql += (conditionSql == "") ? " WHERE " : " AND ";

                conditionSql += "CONTAINS(" + field + ",'" +
                    MSSQLQueryFilter.Filter((string)conditions[field]) + "')";

                switch (match)
                {
                    case MatchType.Exact:
                        conditionSql += " AND LOWER(" + field + ") = '" +
                        ((string)conditions[field]).ToLower() + "'";
                        break;
                    case MatchType.Prefix:
                        conditionSql += " AND LOWER(" + field + ") LIKE '" +
                        ((string)conditions[field]).ToLower() + "%'";
                        break;
                }
            }
            return conditionSql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="match"></param>
        /// <param name="otherCond"></param>
        /// <returns></returns>
        protected string BuildSQLCondition(Hashtable conditions, MatchType match, string otherCond)
        {
            string conditionSql = BuildSQLFTSCondition(conditions, match);
            if (otherCond != null & otherCond != "")
            {
                if (conditionSql != "")
                    conditionSql += " AND " + otherCond;
                else
                    conditionSql += " WHERE " + otherCond;
            }
            return conditionSql;
        }

        #endregion

        #region MSSQLHelper Destructors

        public void End()
        {
            if (this._conn != null && this._conn.State !=
                ConnectionState.Closed)
            {
                try
                {
                    // close the connection.
                    this._conn.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void Dispose()
        {
            this.End();
        }

        #endregion
    }
}