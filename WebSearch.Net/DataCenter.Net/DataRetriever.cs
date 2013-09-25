using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using WebSearch.DataCenter.Net.SEngine;
using WebSearch.DataCenter.Net.Lucene;
using WebSearch.DataCenter.Net.MSSQL;
using System.Collections.Generic;
using WebSearch.DataCenter.Net.DS;

namespace WebSearch.DataCenter.Net
{
    /// <summary>
    /// Data retriever, the facet for this assembly
    /// </summary>
    public static class DataRetriever
    {
        #region For Query Collections

        /// <summary>
        /// Get query collection according to name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IQueryCollection GetQueryCollection(string name)
        {
            // check xml, retrieve data source
            DataSource ds = DataSource.Get(DataSourceType.QueryCollection, name);
            // return the IQueryCollection
            return DataRetriever.GetQueryCollection((QueryCollection)ds);
        }

        /// <summary>
        /// Get query collection according to the data source
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static IQueryCollection GetQueryCollection(QueryCollection ds)
        {
            // init the class object according to store type
            switch (ds.Store)
            {
                case StoreType.MSSQL:
                    return new MSSQLQueryCollection(ds);
                case StoreType.Lucene:
                    return new LuceneQueryCollection(ds);
            }
            return null;
        }

        #endregion

        #region For Query Logs

        /// <summary>
        /// Get query log according to name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IQueryLog GetQueryLog(string name)
        {
            // check xml, retrieve data source
            DataSource ds = DataSource.Get(DataSourceType.QueryLog, name);
            // return the IQueryLog
            return DataRetriever.GetQueryLog((QueryLog)ds);
        }

        /// <summary>
        /// Get query log according to the data source
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static IQueryLog GetQueryLog(QueryLog ds)
        {
            // init the class object according to store type
            switch (ds.Store)
            {
                case StoreType.MSSQL:
                    return new MSSQLQueryLog(ds);
                case StoreType.Lucene:
                    return new LuceneQueryLog(ds);
            }
            return null;
        }

        #endregion

        #region For Web Collections

        /// <summary>
        /// Get web collection according to name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IWebCollection GetWebCollection(string name)
        {
            // check xml, retrieve data source
            DataSource ds = DataSource.Get(DataSourceType.WebCollection, name);
            // return the IWebCollection
            return DataRetriever.GetWebCollection((WebCollection)ds);
        }

        /// <summary>
        /// Get web collection according to data source
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static IWebCollection GetWebCollection(WebCollection ds)
        {
            // init the class object according to store type
            switch (ds.Store)
            {
                case StoreType.MSSQL:
                    return new MSSQLWebCollection(ds);
                case StoreType.Lucene:
                    return new LuceneWebCollection(ds);
                case StoreType.SEngine:
                    return SEngineWebCollection.Get(ds);
            }
            return null;
        }

        #endregion

        #region For Link Collections

        /// <summary>
        /// Get link collection according to name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILinkCollection GetLinkCollection(string name)
        {
            // check xml, retrieve data source
            DataSource ds = DataSource.Get(DataSourceType.LinkCollection, name);
            // return the ILinkCollection
            return DataRetriever.GetLinkCollection((LinkCollection)ds);
        }

        /// <summary>
        /// Get link collection according to the data source
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static ILinkCollection GetLinkCollection(LinkCollection ds)
        {
            // init the class object according to store type
            switch (ds.Store)
            {
                case StoreType.MSSQL:
                    return new MSSQLLinkCollection(ds);
                case StoreType.Lucene:
                    return new LuceneLinkCollection(ds);
            }
            return null;
        }

        #endregion

        #region For User Collection

        /// <summary>
        /// Get user collection according to name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IUserCollection GetUserCollection(string name)
        {
            // check xml, retrieve data source
            DataSource ds = DataSource.Get(DataSourceType.UserCollection, name);
            // return the IUserCollection
            return DataRetriever.GetUserCollection((UserCollection)ds);
        }

        /// <summary>
        /// Get user collection according to the data source
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static IUserCollection GetUserCollection(UserCollection ds)
        {
            // init the class object according to store type
            switch (ds.Store)
            {
                case StoreType.MSSQL:
                    return new MSSQLUserCollection(ds);
            }
            return null;
        }

        #endregion

        #region For Corpus

        /// <summary>
        /// Get corpus according to the data source
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static ICorpus GetCorpus(Corpus ds)
        {
            return null;
        }

        #endregion
    }
}