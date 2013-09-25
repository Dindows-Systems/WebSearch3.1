using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WebSearch.Model.Net
{
    public class SearchResult : BaseModel
    {
        #region Properties

        private UserQuery _query = null;

        /// <summary>
        /// The reference to the user query
        /// </summary>
        public UserQuery Query
        {
            get { return _query; }
            set { _query = value; }
        }

        private string _url;

        /// <summary>
        /// Search result url
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private string _anchor;

        /// <summary>
        /// Search result anchor
        /// </summary>
        public string Anchor
        {
            get { return _anchor; }
            set { _anchor = value; }
        }

        private int _rank;

        /// <summary>
        /// Rank of the search result
        /// </summary>
        public int Rank
        {
            get { return _rank; }
            set { _rank = value; }
        }

        private float _score;

        public float Score
        {
            get { return _score; }
            set { _score = value; }
        }

        private WebPage _page;

        /// <summary>
        /// The result web page
        /// </summary>
        public WebPage Page
        {
            get { return _page; }
            set { _page = value; }
        }

        private List<string> _snippets = new List<string>();

        /// <summary>
        /// Search result snippets
        /// </summary>
        public List<string> Snippets
        {
            get { return _snippets; }
            set { _snippets = value; }
        }

        private Dictionary<string, double> _tfs = null;

        /// <summary>
        /// Term Frequencies
        /// </summary>
        public Dictionary<string, double> TFs
        {
            get { return _tfs; }
            set { _tfs = value; }
        }

        public const string HitTermTag = "h";

        #endregion

        #region Costructors

        public SearchResult()
        {
        }

        public SearchResult(UserQuery query, string url, 
            string anchor, int rank, string snippet)
        {
            this._query = query;
            this._url = url;
            this._anchor = anchor;
            this._rank = rank;
            this._snippets.Add(snippet);
        }

        #endregion
    }
}
