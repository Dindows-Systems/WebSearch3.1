using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.DataCenter.Net.DS
{
    public class UserCollection : DataSource
    {
        #region Constructors

        internal UserCollection(string name)
        {
            this._type = DataSourceType.UserCollection;
            this._name = name;
        }

        #endregion

        #region User Collection Field Names
        #endregion
    }
}
