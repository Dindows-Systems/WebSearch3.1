using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using WebSearch.Model.Net;
using WebSearch.DataCenter.Net.DS;

namespace WebSearch.DataCenter.Net.MSSQL
{
    /// <summary>
    /// Summary description for CAnalysisDAO
    /// </summary>
    public class MSSQLQueryCollection : MSSQLHelper, IQueryCollection
    {
        #region Data Source

        protected QueryCollection _dataSource;

        public QueryCollection DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }
    
        #endregion

        #region MSSQLQueryCollection Constructors

        public MSSQLQueryCollection(QueryCollection ds) : base(ds)
        {
            this._dataSource = ds;
        }

        #endregion

        #region IQueryCollection Members

        public IList<UserQuery> Retrieve(string query, MatchType match, int count, string orderBy)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}