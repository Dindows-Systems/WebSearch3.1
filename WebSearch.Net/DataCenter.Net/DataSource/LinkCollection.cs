using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.DataCenter.Net.DS
{
    public class LinkCollection : DataSource
    {
        internal LinkCollection(string name)
        {
            this._type = DataSourceType.LinkCollection;
            this._name = name;
        }
    }
}
