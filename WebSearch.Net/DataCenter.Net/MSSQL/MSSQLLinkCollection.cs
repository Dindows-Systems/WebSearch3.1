using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.DataCenter.Net.DS;

namespace WebSearch.DataCenter.Net.MSSQL
{
    public class MSSQLLinkCollection : MSSQLHelper, ILinkCollection
    {
        #region Data Source

        protected LinkCollection _dataSource;

        public LinkCollection DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        #endregion

        #region MSSQLLinkCollection Constructors

        public MSSQLLinkCollection(LinkCollection ds)
            : base(ds)
        {
            this._dataSource = ds;
        }

        #endregion
    }
}
