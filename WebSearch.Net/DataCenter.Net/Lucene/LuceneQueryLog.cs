using System;
using System.Data;
using System.Configuration;
using System.Web;
using WebSearch.DataCenter.Net.DS;
using System.Collections.Generic;
using WebSearch.Model.Net;

namespace WebSearch.DataCenter.Net.Lucene
{
    /// <summary>
    /// Summary description for LuceneQueryLogDAO
    /// </summary>
    public class LuceneQueryLog : LuceneHelper, IQueryLog
    {
        #region Data Source

        protected QueryLog _dataSource;

        public QueryLog DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        #endregion

        #region LuceneQueryLog Constructors

        public LuceneQueryLog(QueryLog ds) : base(ds)
        {
            this._dataSource = ds;
        }

        #endregion

        #region IQueryLog Members

        public void SetRetrievalType(QueryLog.RetrievalType type)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IList<ClickThrough> Retrieve(int count, string orderBy)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IList<ClickThrough> Retrieve(string query, MatchType match, string otherCond, int count, string orderBy)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IList<ClickThrough> Retrieve(System.Collections.Hashtable conditions, MatchType match, string otherCond, int count, string orderBy)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void End()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}