using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.DataCenter.Net.DS;
using System.Text.RegularExpressions;
using WebSearch.Model.Net;
using System.Web;

namespace WebSearch.DataCenter.Net.SEngine
{
    public class GoogleWebCollection : SEngineWebCollection
    {
        #region Regular Expressions

        private const string _searchResultPatten = "<a\\shref=\"(?<link>[^\"]*)\"" +
            "\\s[^>]*class=l>(?<anchor>.*?)</a>.*?<table[^>]*><tr><td class=j>" +
            "<font size=-1>(?<snippet>(?:.|\\s)*?)(<br>)?<span";

        private static readonly Regex _searchResultRegex =
            new Regex(_searchResultPatten, RegexOptions.Compiled);

        private const string _urlResultPattern = "<a\\shref=\"(?<link>[^\"]*)\"\\s[^>]*class=l>";

        private static readonly Regex _urlResultRegex =
            new Regex(_urlResultPattern, RegexOptions.Compiled);

        private static readonly Regex _postProcessRegex = 
            new Regex(@"\s\(\*\)", RegexOptions.Compiled);

        #endregion

        #region GoogleWebCollection Constructors

        public GoogleWebCollection(WebCollection ds)
            : base(ds)
        {
        }

        #endregion

        #region GoogleWebCollection Members

        public override SearchResultList Search(string query, int count)
        {
            UserQuery userQuery = new UserQuery(query);
            // 1. encode the query
            query = HttpUtility.UrlEncode(query, Encoding.GetEncoding("gb2312"));
            string url = "";
            SearchResultList results = Search(userQuery, url, _searchResultRegex, null);
            if (results == null) return null;
            // as for google, google uses \r(*) as line breaker
            // here, we need to filter such signs
            foreach (SearchResult result in results)
            {
                result.Url = _postProcessRegex.Replace(result.Url, "");
                result.Anchor = _postProcessRegex.Replace(result.Anchor, "");
                for (int i = 0; i < result.Snippets.Count; i++)
                    result.Snippets[i] = _postProcessRegex.Replace(result.Snippets[i], "");
            }
            return results;
        }

        public override string[] SearchUrls(string query, int count)
        {
            // 1. encode the query
            query = HttpUtility.UrlEncode(query, Encoding.GetEncoding("gb2312"));
            string url = "";
            string[] results = SearchUrls(url, _urlResultRegex);
            if (results == null) return null;
            // as for google, google uses \r(*) as line breaker
            // here, we need to filter such signs
            for (int i = 0; i < results.Length; i++)
                results[i] = _postProcessRegex.Replace(results[i], "");
            return results;
        }

        #endregion
    }
}
