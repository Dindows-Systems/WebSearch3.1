using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.DataCenter.Net.DS;
using WebSearch.Model.Net;

namespace WebSearch.DataCenter.Net.MSSQL
{
    public class MSSQLWebCollection : MSSQLHelper, IWebCollection
    {
        #region Data Source

        protected WebCollection _dataSource;

        public WebCollection DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        #endregion

        #region MSSQLWebCollection Constructors

        public MSSQLWebCollection(WebCollection ds) : base(ds)
        {
            this._dataSource = ds;
        }

        #endregion

        #region IWebCollection Members

        public SearchResultList Search(string query, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string[] SearchUrls(string query, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
