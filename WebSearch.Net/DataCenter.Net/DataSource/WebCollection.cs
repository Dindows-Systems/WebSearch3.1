using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.DataCenter.Net.DS
{
    public class WebCollection : DataSource
    {
        #region Constructors

        internal WebCollection(string name)
        {
            this._type = DataSourceType.WebCollection;
            this._name = name;
        }

        #endregion

        #region Web Collection Field Names

        public const string ID = "id";

        public const string Url = "url";

        public const string Title = "title";

        public const string Description = "description";

        public const string Keywords = "keywords";

        public const string Html = "html";

        public const string Encoding = "encoding";

        #endregion
    }
}
