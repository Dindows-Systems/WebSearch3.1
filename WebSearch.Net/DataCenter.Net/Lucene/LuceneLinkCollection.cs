using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.DataCenter.Net.DS;

namespace WebSearch.DataCenter.Net.Lucene
{
    public class LuceneLinkCollection : LuceneHelper, ILinkCollection
    {
        #region Data Source

        protected LinkCollection _dataSource;

        public LinkCollection DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        #endregion

        #region LuceneLinkCollection Constructors

        public LuceneLinkCollection(LinkCollection ds)
            : base(ds)
        {
            this._dataSource = ds;
        }

        #endregion
    }
}
