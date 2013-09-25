using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Xml;
using System.Text;
using WebSearch.Model.Net;
using WebSearch.DataCenter.Net.DS;
using System.Text.RegularExpressions;
using System.Web;
using WebSearch.Common.Net;

namespace WebSearch.DataCenter.Net.SEngine
{
    /// <summary>
    /// Summary description for MSESearchEngineDAO
    /// </summary>
    public class SEngineWebCollection : SEngineHelper, IWebCollection
    {
        #region Data Source

        protected WebCollection _dataSource;

        public WebCollection DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        #endregion

        protected SearchResultList _searchResults = new SearchResultList(10);

        #region SEngineWebCollection Constructors

        public static SEngineWebCollection Get(WebCollection ds)
        {
            if (ds.Name.ToLower() == "baidu")
                return new BaiduWebCollection(ds);
            if (ds.Name.ToLower() == "google")
                return new GoogleWebCollection(ds);
            if (ds.Name.ToLower() == "sogou")
                return new SogouWebCollection(ds);
            return new SEngineWebCollection(ds);
        }

        public SEngineWebCollection(WebCollection ds) : base()
        {
            this._dataSource = ds;
        }

        #endregion

        #region ISearchEngineDAO Members

        public virtual SearchResultList Search(string query, int count)
        {
            return null;
        }

        public virtual string[] SearchUrls(string query, int count)
        {
            return null;
        }

        #endregion

        #region Protected Members

        protected virtual SearchResultList Search(UserQuery query, string url,
            Regex hitRegex, Regex totalHitRegex)
        {
            // 1. crawl the search result page
            WebPage page = this._sEngineCrawler.Retrieve(url);
            if (page == null)
                return null;
            // 2. analyze the search result
            this._searchResults.Clear();
            MatchCollection matches = hitRegex.Matches(page.Html);
            if (matches.Count == 0)
                return this._searchResults; 
            int rank = 0, mode; // mode 
            string firstRank = matches[0].Groups["rank"].Value;
            if (firstRank == "0") mode = 0;         // start from 0
            else if (firstRank == "1") mode = 1;    // start from 1
            else mode = 3;                          // no rank info
            foreach (Match match in matches)
            {
                switch (mode)
                {
                    case 0: rank = int.Parse(match.Groups["rank"].Value) + 1; break;
                    case 1: rank = int.Parse(match.Groups["rank"].Value); break;
                    default: rank++; break;
                }
                string link = match.Groups["link"].Value;
                string anchor = match.Groups["anchor"].Value;
                string snippet = match.Groups["snippet"].Value;

                this._searchResults.Add(new SearchResult(query, link, anchor, rank, snippet));
            }
            // 3. get the total hit num in web collection
            Match totalm = totalHitRegex.Match(page.Html);
            if (totalm != null && totalm.Success)
                this._searchResults.HitCount = int.Parse(totalm.Groups["total"].Value);
            else
                this._searchResults.HitCount = Const.Invalid;

            return this._searchResults;
        }

        protected virtual string[] SearchUrls(string url, Regex regex)
        {
            // 1. crawl the search result page
            WebPage page = this._sEngineCrawler.Retrieve(url);
            if (page == null)
                return null;
            // 2. analyze the search result
            MatchCollection matches = regex.Matches(page.Html);
            this._searchResults.Clear();
            if (matches.Count == 0)
                return new string[0];

            string[] results = new string[matches.Count];
            int rank = -1, mode; // mode 
            string firstRank = matches[0].Groups["rank"].Value;
            if (firstRank == "0") mode = 0;         // start from 0
            else if (firstRank == "1") mode = 1;    // start from 1
            else mode = 3;                          // no rank info
            foreach (Match match in matches)
            {
                switch (mode)
                {
                    case 0: rank = int.Parse(match.Groups["rank"].Value); break;
                    case 1: rank = int.Parse(match.Groups["rank"].Value) - 1; break;
                    default: rank++; break;
                }
                results[rank] = match.Groups["link"].Value;
            }
            return results;
        }

        #endregion
    }
}