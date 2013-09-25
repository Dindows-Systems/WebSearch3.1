using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.DataCenter.Net.DS
{
    public class QueryCollection : DataSource
    {
        internal QueryCollection(string name)
        {
            this._type = DataSourceType.QueryCollection;
            this._name = name;
        }

        #region Query Collection Field Names

        public const string ID = "";

        public const string Query = "";

        #endregion
    }
}
