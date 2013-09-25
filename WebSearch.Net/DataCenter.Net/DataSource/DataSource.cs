using System;
using System.Data;
using System.Configuration;
using System.Xml;
using System.Collections.Generic;
using WebSearch.Common.Net;

namespace WebSearch.DataCenter.Net.DS
{
    /// <summary>Data source type</summary>
    public enum DataSourceType
    {
        QueryCollection,    // user's query collection
        WebCollection,      // web collection
        LinkCollection,     // link collection
        QueryLog,           // query log
        UserCollection,     // user collection
        Corpus              // corpus
    }

    /// <summary>Store type</summary>
    public enum StoreType
    {
        MSSQL, Lucene, SEngine
    }

    /// <summary>Match type</summary>
    public enum MatchType
    {
        Normal, Exact, Prefix
    }

    /// <summary>
    /// Representing a data source
    /// </summary>
    public abstract class DataSource
    {
        #region Properties

        protected DataSourceType _type;

        /// <summary>
        /// Data source type
        /// </summary>
        /// <example>WebCollection</example>
        /// <remarks>key1 for the data source</remarks>
        public DataSourceType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        protected string _name;

        /// <summary>
        /// Name of the data source
        /// </summary>
        /// <remarks>key2 for the data source</remarks>
        /// <example>Sogou</example>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected string _path;

        /// <summary>
        /// Path for the data source
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        protected string _target;

        /// <summary>
        /// Target for the data source path
        /// </summary>
        /// <example>
        /// if the store type is database, 
        /// target will be db table name
        /// </example>
        public string Target
        {
            get { return _target; }
            set { _target = value; }
        }

        protected StoreType _storeType;

        /// <summary>
        /// Storage type for the data source
        /// </summary>
        public StoreType Store
        {
            get { return _storeType; }
            set { _storeType = value; }
        }

        protected SupportedLanguage _lang;

        /// <summary>
        /// Language for the data source
        /// </summary>
        public SupportedLanguage Language
        {
            get { return _lang; }
            set { _lang = value; }
        }

        protected int _size;

        /// <summary>
        /// The size of the data source
        /// </summary>
        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        protected List<DataSource> _dataSources = new List<DataSource>();

        /// <summary>
        /// The data sources for this data source
        /// </summary>
        public List<DataSource> DataSources
        {
            get { return _dataSources; }
        }

        #endregion

        public static DataSource Get(DataSourceType type, string name)
        {
            return DataSource.Get(type, name, 1);
        }

        private static DataSource Get(DataSourceType type, string name, int level)
        {
            // according to the type, decide which xml to read
            string xmlPath = Config.DataSourcePath + type.ToString() + ".xml";

            XmlElement elem = XmlHelper.ReadNode(xmlPath, name);
            if (elem == null) return null;
            // parse the xml element and init the data source object
            DataSource source = null;
            switch (type)
            {
                case DataSourceType.QueryLog:
                    source = new QueryLog(name,
                        DateTime.Parse(elem.Attributes["_StartDate"].Value),
                        DateTime.Parse(elem.Attributes["_EndDate"].Value));
                    break;
                case DataSourceType.QueryCollection:
                    source = new QueryCollection(name);
                    break;
                case DataSourceType.WebCollection:
                    source = new WebCollection(name);
                    break;
                case DataSourceType.LinkCollection:
                    source = new LinkCollection(name);
                    break;
                case DataSourceType.UserCollection:
                    source = new UserCollection(name);
                    break;
            }
            // common features for all data sources (features in data source)
            source.Path = elem.Attributes["_Path"].Value;
            source.Target = elem.Attributes["_Target"].Value;
            source.Store = (StoreType)EnumHelper.StringToEnum(
                typeof(StoreType), elem.Attributes["_Store"].Value);
            source.Language = (SupportedLanguage)EnumHelper.StringToEnum(
                typeof(SupportedLanguage), elem.Attributes["_Language"].Value);
            source.Size = int.Parse(elem.Attributes["_Size"].Value);
            /*
            if (level > 0) // to avoid unlimited recursion
            {
                // load the sub data sources
                XmlNodeList subList = elem.ChildNodes;
                XmlElement subElem = null;
                for (int i = 0; i < subList.Count; i++)
                {
                    subElem = (XmlElement)subList.Item(i);
                    source.DataSources.Add(Get((DataSourceType)
                        EnumHelper.StringToEnum(typeof(DataSourceType), 
                        subElem.Attributes["_SubType"].Value),
                        subElem.Attributes["_SubName"].Value, level - 1)
                        );
                }
            }*/
            return source;
        }
    }
}