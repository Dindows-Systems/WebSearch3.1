using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.Model.Net
{
    public class SearchResultList : List<SearchResult>
    {
        #region Properties

        private int _hitCount;

        /// <summary>
        /// Total hit count in the web collection
        /// </summary>
        public int HitCount
        {
            get { return _hitCount; }
            set { _hitCount = value; }
        }

        private int _sourceSize;

        /// <summary>
        /// The size of data source for this search
        /// </summary>
        public int SourceSize
        {
            get { return _sourceSize; }
            set { _sourceSize = value; }
        }

        #endregion

        #region Constructors

        public SearchResultList(int capacity)
            : base(capacity)
        {
        }

        public SearchResultList()
            : base()
        {
        }

        #endregion
    }
}
