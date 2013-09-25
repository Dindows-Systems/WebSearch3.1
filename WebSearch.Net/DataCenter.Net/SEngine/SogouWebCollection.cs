using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.DataCenter.Net.DS;
using System.Text.RegularExpressions;
using WebSearch.Model.Net;
using System.Web;

namespace WebSearch.DataCenter.Net.SEngine
{
    /// <summary>
    /// http://www.sogou.com/web?query=test&num=100&page=2
    /// </summary>
    public class SogouWebCollection : SEngineWebCollection
    {
        #region Regular Expressions

        private const string _searchResultPatten = "<!--awbg(?<rank>\\d+)-->" +
            "<a[^h]*href=\"(?<link>[^\"]*)\"[^<]*<!--awed\\d*-->(?<anchor>.*?)" +
            "</a></h2><p class=\"ff\" name=\"dsum\">(?<snippet>.*?)</p><p";

        private static readonly Regex _searchResultRegex =
            new Regex(_searchResultPatten, RegexOptions.Compiled);

        private const string _urlResultPattern = "<!--awbg(?<rank>\\d+)-->" +
            "<a[^h]*href=\"(?<link>[^\"]*)\"";

        private static readonly Regex _urlResultRegex =
            new Regex(_urlResultPattern, RegexOptions.Compiled);

        private const string _totalHitPattern = "";

        private static readonly Regex _totalHitRegex =
            new Regex(_totalHitPattern, RegexOptions.Compiled);

        #endregion

        #region SogouWebCollection Constructors

        protected static int MaxResultNumPerRequest = 100;

        public SogouWebCollection(WebCollection ds)
            : base(ds)
        {
        }

        #endregion

        #region SogouWebCollection Members

        public override SearchResultList Search(string query, int count)
        {
            UserQuery useQuery = new UserQuery(query);
            // 1. encode the query
            query = HttpUtility.UrlEncode(query, Encoding.GetEncoding("gb2312"));
            // 2. prepare the result list
            SearchResultList searchResults = new SearchResultList(), tempCache = null;
            // 3. get the request count
            int requestCount = (int)Math.Ceiling((double)count / (double)MaxResultNumPerRequest);
            for (int pageID = 1; pageID <= requestCount; pageID++)
            {
                // 2. build the request url
                string url = String.Format(DataSource.Path, query,
                    MaxResultNumPerRequest, pageID);
                tempCache = Search(useQuery, url, _searchResultRegex, _totalHitRegex);
                foreach (SearchResult result in tempCache)
                {
                    // post process search result's snippets
                    //List<string> snippets = new List<string>();
                    //foreach (string sn in result.Snippets)
                    //{
                    //    sn.Split("...");
                    //    // replace term's hit tag to uniform
                    //}
                    searchResults.Add(result);
                }

                // check whether there's no more search results
                if (tempCache.Count < MaxResultNumPerRequest - 8)
                    break;
            }
            return searchResults;
        }

        public override string[] SearchUrls(string query, int count)
        {
            // 1. encode the query
            query = HttpUtility.UrlEncode(query, Encoding.GetEncoding("gb2312"));
            // 2. prepare the result list
            List<string> searchResults = new List<string>();
            // 3. get the request count
            int requestCount = (int)Math.Ceiling((double)count / (double)MaxResultNumPerRequest);
            for (int pageID = 1; pageID <= requestCount; pageID++)
            {
                // 2. build the request url
                string url = String.Format(DataSource.Path, query,
                    MaxResultNumPerRequest, pageID);
                string[] tempCache = SearchUrls(url, _urlResultRegex);
                if (tempCache == null)
                    break;
                foreach (string result in tempCache)
                    searchResults.Add(result);

                // check whether there's no more search results
                if (tempCache.Length < MaxResultNumPerRequest - 8)
                    break;
            }
            return searchResults.ToArray();
        }

        #endregion
    }
}
