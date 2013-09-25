using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using WebSearch.Model.Net;
using WebSearch.DataCenter.Net.DS;
using System.Collections;

namespace WebSearch.DataCenter.Net
{
    /// <summary>
    /// Summary description for ILogDAO
    /// </summary>
    public interface IQueryLog
    {
        /// <summary>
        /// Set the data retrieve type
        /// </summary>
        /// <param name="type"></param>
        void SetRetrievalType(QueryLog.RetrievalType type);

        /// <summary>
        /// Get the click through data without conditions
        /// </summary>
        /// <param name="count"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IList<ClickThrough> Retrieve(int count, String orderBy);

        /// <summary>
        /// Get the click through data according to the query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IList<ClickThrough> Retrieve(String query, MatchType match,
            String otherCond, int count, String orderBy);

        /// <summary>
        /// Get the click through data that pertain the given conditions
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="exactMatch"></param>
        /// <param name="count"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IList<ClickThrough> Retrieve(Hashtable conditions, MatchType match,
             String otherCond, int count, String orderBy);

        /// <summary>
        /// End the current data access object
        /// </summary>
        void End();
    }
}
