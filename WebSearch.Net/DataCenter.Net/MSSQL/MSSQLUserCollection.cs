using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.DataCenter.Net.DS;

namespace WebSearch.DataCenter.Net.MSSQL
{
    public class MSSQLUserCollection : MSSQLHelper, IUserCollection
    {
        #region Data Source

        protected UserCollection _dataSource;

        public UserCollection DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        #endregion

        #region MSSQLUserCollection Constructors

        public MSSQLUserCollection(UserCollection ds)
            : base(ds)
        {
            this._dataSource = ds;
        }

        #endregion
    }
}
