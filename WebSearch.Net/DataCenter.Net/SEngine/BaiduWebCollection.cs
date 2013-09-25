using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.DataCenter.Net.DS;
using WebSearch.Model.Net;
using System.Text.RegularExpressions;

namespace WebSearch.DataCenter.Net.SEngine
{
    public class BaiduWebCollection : SEngineWebCollection
    {
        #region Regular Expressions

        private const string _searchResultPatten = "<td\\sclass=f>.*?<a\\sonclick=\"" +
            "[^\"]*,(?<rank>\\d+)\\)\"\\shref=\"(?<link>[^\"]*)\"[^>]*>(?<anchor>.*?)" +
            "</a><br><font size=-1>(?<snippet>.*?)<br>";

        private static readonly Regex _searchResultRegex = 
            new Regex(_searchResultPatten, RegexOptions.Compiled);

        private const string _urlResultPattern = "<td\\sclass=f>.*?<a\\sonclick=\"" +
            "[^\"]*,(?<rank>\\d+)\\)\"\\shref=\"(?<link>[^\"]*)\"";

        private static readonly Regex _urlResultRegex =
            new Regex(_urlResultPattern, RegexOptions.Compiled);

        #endregion

        #region BaiduWebCollection Constructors

        public BaiduWebCollection(WebCollection ds) : base(ds)
        {
        }

        #endregion

        #region BaiduWebCollection Members

        public override SearchResultList Search(string query, int count)
        {
            UserQuery userQuery = new UserQuery(query);
            string url = "";
            return Search(userQuery, url, _searchResultRegex, null);
        }

        public override string[] SearchUrls(string query, int count)
        {
            string url = "";
            return SearchUrls(url, _urlResultRegex);
        }

        #endregion
    }
}
