using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.DataCenter.Net.DS
{
    public class Corpus : DataSource
    {
        internal Corpus(string name)
        {
            this._type = DataSourceType.Corpus;
            this._name = name;
        }
    }
}
