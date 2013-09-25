using System;
using System.Data;
using System.Configuration;
using WebSearch.DataCenter.Net.DS;
using WebSearch.Common.Net;

namespace WebSearch.DataCenter.Net.Lucene
{
    /// <summary>
    /// Summary description for LuceneHelper
    /// </summary>
    public class LuceneHelper
    {
        #region Lucene Index Searcher

        protected LuceneSearcher _searcher = null;

        /// <summary>
        /// The searcher object for the index
        /// </summary>
        public LuceneSearcher Searcher
        {
            get { return this._searcher; }
        }

        /// <summary>
        /// Get the index searcher object
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static LuceneSearcher GetIndexSearcher(
            String path, SupportedLanguage lang)
        {
            return new LuceneSearcher(path, lang);
        }

        #endregion

        #region LuceneHelper Constructors

        public LuceneHelper(DataSource ds)
        {
            try
            {
                this._searcher = GetIndexSearcher(
                    ds.Path, ds.Language);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}