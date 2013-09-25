using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.DataCenter.Net.DS;

namespace WebSearch.DataCenter.Net
{
    public static class DataBuilder
    {
        #region For Query Collection

        /// <summary>
        /// Create a query collection according to the given target
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IQueryCollection CreateQueryCollection(QueryCollection target)
        {
            // add the target info into QueryCollection.xml

            return null;
        }

        /// <summary>
        /// Create a query collection from the given queries
        /// </summary>
        /// <param name="target"></param>
        /// <param name="queries"></param>
        /// <returns></returns>
        public static IQueryCollection CreateQueryCollection(
            QueryCollection target, List<String> queries)
        {
            return null;
        }

        #endregion

        #region For Link Collection

        /// <summary>
        /// Create a link collection according to the given target
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ILinkCollection CreateLinkCollection(LinkCollection target)
        {
            return null;
        }

        #endregion

        #region For Web Collection
        #endregion
    }
}
