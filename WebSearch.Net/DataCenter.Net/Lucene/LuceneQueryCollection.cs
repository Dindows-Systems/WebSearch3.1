using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.DataCenter.Net.DS;
using WebSearch.Model.Net;

namespace WebSearch.DataCenter.Net.Lucene
{
    public class LuceneQueryCollection : LuceneHelper, IQueryCollection
    {
        #region Data Source

        protected QueryCollection _dataSource;

        public QueryCollection DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        #endregion

        #region LuceneQueryCollection Constructors

        public LuceneQueryCollection(QueryCollection ds) : base(ds)
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
