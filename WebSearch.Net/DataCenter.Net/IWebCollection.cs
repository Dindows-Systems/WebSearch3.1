using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections.Generic;
using WebSearch.Model.Net;

namespace WebSearch.DataCenter.Net
{
    /// <summary>
    /// Summary description for ISearchEngineDAO
    /// </summary>
    public interface IWebCollection
    {
        /// <summary>
        /// Search a certain query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        SearchResultList Search(String query, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        string[] SearchUrls(string query, int count);
    }
}