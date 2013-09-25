using System;
using System.Data;
using System.Configuration;
using WebSearch.Model.Net;
using System.Collections.Generic;
using WebSearch.DataCenter.Net.DS;

namespace WebSearch.DataCenter.Net
{
    /// <summary>
    /// Summary description for IAnalysisDAO
    /// </summary>
    public interface IQueryCollection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="match"></param>
        /// <param name="count"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IList<UserQuery> Retrieve(String query, MatchType match,
            int count, String orderBy);
    }
}