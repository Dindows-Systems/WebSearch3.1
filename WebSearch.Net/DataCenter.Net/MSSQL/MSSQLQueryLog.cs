using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using WebSearch.DataCenter.Net.DS;
using WebSearch.Model.Net;
using System.Collections.Generic;
using WebSearch.DataCenter.Net.Properties;
using System.Collections;
using WebSearch.Common.Net;

namespace WebSearch.DataCenter.Net.MSSQL
{
    /// <summary>
    /// Summary description for CLogDAO
    /// </summary>
    public class MSSQLQueryLog : MSSQLHelper, IQueryLog
    {
        #region Data Source

        protected QueryLog _dataSource;

        public QueryLog DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        #endregion

        #region MSSQLQueryLog Constructors

        public MSSQLQueryLog(QueryLog ds) : base(ds)
        {
            this._dataSource = ds;
        }

        #endregion

        #region IQueryLog Members

        public void SetRetrievalType(QueryLog.RetrievalType type)
        {
            this._dataSource.RetrieveType = type;
        }

        public IList<ClickThrough> Retrieve(int count, String orderBy)
        {
            try
            {
                // get the command object
                SqlCommand cmd = this.GetSqlCommand(
                    BuildSQLFTSCommand(null, MatchType.Normal, null, 
                    count, orderBy, DataSource.RetrieveType));
                // fill the click through data
                return Fill(cmd.ExecuteReader());
            }
            catch (Exception exception)
            {
                this._conn.Close();
                throw exception;
            }
        }

        public IList<ClickThrough> Retrieve(String query,
            MatchType match, String otherCond, int count, String orderBy)
        {
            try
            {
                // get the command object
                Hashtable conditions = new Hashtable();
                conditions.Add(QueryLog.Query, query);

                SqlCommand cmd = this.GetSqlCommand(
                    BuildSQLFTSCommand(conditions, match, otherCond,
                    count, orderBy, DataSource.RetrieveType));
                // fill the click through data
                return Fill(cmd.ExecuteReader());
            }
            catch (Exception exception)
            {
                this._conn.Close();
                throw exception;
            }
        }

        public IList<ClickThrough> Retrieve(Hashtable conditions,
            MatchType match, String otherCond, int count, String orderBy)
        {
            try
            {
                // get the command object
                SqlCommand cmd = this.GetSqlCommand(
                    BuildSQLFTSCommand(conditions, match, otherCond,
                    count, orderBy, DataSource.RetrieveType));
                // fill the click through data
                return Fill(cmd.ExecuteReader());
            }
            catch (Exception exception)
            {
                this._conn.Close();
                throw exception;
            }
        }

        #endregion

        #region Protected Members
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected IList<ClickThrough> Fill(SqlDataReader reader)
        {
            IList<ClickThrough> results = new List<ClickThrough>();
            List<string> columns = null;
            while (reader.Read()) // read one by one
            {
                if (columns == null)
                {
                    columns = new List<string>(reader.FieldCount);
                    for (int i = 0; i < reader.FieldCount; i++)
                        columns.Add(reader.GetName(i));
                }

                string user = columns.Contains(QueryLog.User) ? 
                    reader[QueryLog.User].ToString() : null;
                string query = columns.Contains(QueryLog.Query) ? 
                    reader[QueryLog.Query].ToString() : null;
                string link = columns.Contains(QueryLog.Link) ? 
                    reader[QueryLog.Link].ToString() : null;
                int rank = columns.Contains(QueryLog.Rank) ? 
                    int.Parse(reader[QueryLog.Rank].ToString()) : Const.Invalid;
                int order = columns.Contains(QueryLog.Order) ? 
                    int.Parse(reader[QueryLog.Order].ToString()) : Const.Invalid;
                int count = columns.Contains(QueryLog.Count) ? 
                    int.Parse(reader[QueryLog.Count].ToString()) : Const.Invalid;
                DateTime time = columns.Contains(QueryLog.Time) ?
                    DateTime.Parse(reader[QueryLog.Time].ToString()) : DateTime.MinValue;

                results.Add(new ClickThrough(user, query,
                    rank, order, link, time, count));
            }
            reader.Close();
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="match"></param>
        /// <param name="otherCond"></param>
        /// <param name="count"></param>
        /// <param name="orderBy"></param>
        /// <param name="retrieveType"></param>
        /// <returns></returns>
        protected string BuildSQLFTSCommand(Hashtable conditions, MatchType match, string otherCond, 
            int count, string orderBy, QueryLog.RetrievalType retrieveType)
        {
            // build the condition sql statement
            string conditionSql = BuildSQLCondition(conditions, match, otherCond);
            return BuildSQLFTSCommand(conditionSql, count, orderBy, retrieveType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditionSql"></param>
        /// <param name="count"></param>
        /// <param name="orderBy"></param>
        /// <param name="retrieveType"></param>
        /// <returns></returns>
        protected string BuildSQLFTSCommand(String conditionSql,
            int count, string orderBy, QueryLog.RetrievalType retrieveType)
        {
            StringBuilder str = new StringBuilder("SELECT ");
            int tempSize = Const.Invalid;
            if (count >= 0)
            {
                str.Append("TOP " + count.ToString() + " ");
                tempSize = (int)(count * Settings.Default.AverageClickPerSession);
            }
            
            // build the order by sql statement
            string orderBySql = (orderBy == null || orderBy == "")
                ? "" : " ORDER BY " + orderBy;

            switch (retrieveType)
            {
                case QueryLog.RetrievalType.Normal:
                    str.Append(" * FROM " + _dataSource.Target + conditionSql + orderBySql);
                    return str.ToString();
                case QueryLog.RetrievalType.BySession:
                    // user, query, max rank, click count per session
                    str.Append(QueryLog.User + "," + QueryLog.Query + "," + QueryLog.Time + ",");
                    str.Append(" MAX(" + QueryLog.Rank + ") AS " + QueryLog.Rank);
                    str.Append(",MAX(" + QueryLog.Order + ") AS " + QueryLog.Order);
                    if (tempSize != Const.Invalid)
                    {
                        str.Append(" FROM (SELECT TOP " + tempSize.ToString() + " ");
                        str.Append(QueryLog.User + "," + QueryLog.Rank + ",");
                        str.Append(QueryLog.Query + "," + QueryLog.Order + ",");
                        str.Append(QueryLog.Time + " FROM ");
                        str.Append(_dataSource.Target + conditionSql + ") AS TABLE1");
                    }
                    else
                        str.Append(" FROM " + _dataSource.Target + conditionSql);

                    str.Append(" GROUP BY " + QueryLog.User + "," +
                        QueryLog.Query + "," + QueryLog.Time);
                    str.Append(orderBySql);
                    return str.ToString();
                case QueryLog.RetrievalType.ByClickOrder:
                    // avg rank, count, click order per click order
                    str.Append(" AVG(" + QueryLog.Rank + ") AS " + QueryLog.Rank);
                    str.Append(",COUNT(*) AS " + QueryLog.Count);
                    str.Append("," + QueryLog.Order + " FROM ");
                    if (tempSize != Const.Invalid)
                    {
                        str.Append(" (SELECT TOP " + tempSize.ToString());
                        str.Append(" " + QueryLog.Rank + "," + QueryLog.Order);
                        str.Append(" FROM " + _dataSource.Target + conditionSql);
                        str.Append(" ) AS TABLE1");
                    }
                    else
                        str.Append(_dataSource.Target + conditionSql);

                    str.Append(" GROUP BY " + QueryLog.Order);
                    str.Append(orderBySql);
                    return str.ToString();
                case QueryLog.RetrievalType.ByLink:
                    // avg rank, avg click order, count of clicks, link per link
                    str.Append(" AVG(" + QueryLog.Rank + ") AS " + QueryLog.Rank);
                    str.Append(",AVG(" + QueryLog.Order + ") AS " + QueryLog.Order);
                    str.Append(",COUNT(" + QueryLog.ID + ") AS " + QueryLog.Count);
                    str.Append("," + QueryLog.Link + " FROM ");
                    if (tempSize != Const.Invalid)
                    {
                        str.Append("(SELECT TOP " + tempSize.ToString() + " ");
                        str.Append(QueryLog.Rank + "," + QueryLog.Order + ",");
                        str.Append(QueryLog.ID + "," + QueryLog.Link);
                        str.Append(" FROM " + _dataSource.Target + conditionSql);
                        str.Append(" ) AS TABLE1");
                    }
                    else
                        str.Append(_dataSource.Target + conditionSql);
                    str.Append(" GROUP BY " + QueryLog.Link);
                    str.Append(orderBySql);
                    return str.ToString();
                case QueryLog.RetrievalType.ByQuery:
                    // count (of session), avg rank, avg click count per query
                    str.Append(" COUNT(" + QueryLog.User + ") AS " + QueryLog.Count);
                    str.Append(",AVG(" + QueryLog.Rank + ") AS " + QueryLog.Rank);
                    str.Append(",AVG(" + QueryLog.Order + ") AS " + QueryLog.Order);
                    str.Append("," + QueryLog.Query);
                    // need the temp result of bySession
                    str.Append(" FROM (" + BuildSQLFTSCommand(conditionSql,
                        -1, null, QueryLog.RetrievalType.BySession) + ") AS TABLE2");
                    str.Append(" GROUP BY " + QueryLog.Query);
                    str.Append(orderBySql);
                    return str.ToString();
                case QueryLog.RetrievalType.ByRank:
                    // avg click order, click count
                    str.Append(" AVG(" + QueryLog.Order + ") AS " + QueryLog.Order);
                    str.Append(",COUNT(" + QueryLog.ID + ") AS " + QueryLog.Count);
                    str.Append("," + QueryLog.Rank + " FROM ");
                    if (tempSize != Const.Invalid)
                    {
                        str.Append("(SELECT TOP " + tempSize.ToString() + " ");
                        str.Append(QueryLog.Order + "," + QueryLog.ID + "," + QueryLog.Rank);
                        str.Append(" FROM " + _dataSource.Target + conditionSql);
                        str.Append(" ) AS TABLE1");
                    }
                    else
                        str.Append(_dataSource.Target + conditionSql);
                    str.Append(" GROUP BY " + QueryLog.Rank);
                    str.Append(orderBySql);
                    return str.ToString();
                default:
                    return null;
            }
        }

        #endregion
    }
}