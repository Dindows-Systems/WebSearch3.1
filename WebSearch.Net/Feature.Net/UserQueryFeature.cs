using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Model.Net;
using WebSearch.Maths.Net;
using WebSearch.DataCenter.Net.DS;
using System.Data;
using WebSearch.Common.Net;
using System.Text.RegularExpressions;

namespace WebSearch.Feature.Net
{
    public class UserQueryFeature
    {
        #region Constructors

        private UserQuery _userQuery;

        public static UserQueryFeature Get(UserQuery userQuery)
        {
            return new UserQueryFeature(userQuery);
        }

        public static UserQueryFeature Get(string query)
        {
            return new UserQueryFeature(query);
        }

        protected UserQueryFeature(UserQuery userQuery)
        {
            this._userQuery = userQuery;
        }

        protected UserQueryFeature(string query)
        {
            this._userQuery = new UserQuery(query);
        }

        #endregion

        #region Regular Expressions
        
        public static readonly Regex fileNameRegex = new Regex(
            @"\b[^\.]+\.[a-zA-Z]\w{1,3}\b", RegexOptions.None);

        #endregion

        #region User Query Features

        protected void InitializeQueryFeatures()
        {
            // get the _isUrl and _isFileName
            Match m = Hyperlink.urlBlurRegex.Match(_userQuery.Value);
            this._isUrl = m.Success;
            string urlPart = (m.Success) ? m.Value : "";

            m = fileNameRegex.Match(_userQuery.Value);
            this._isFileName = (m.Success && m.Value == _userQuery.Value);

            if (this._isFileName != false && this._isUrl != false)
                // it may be not a url
                if (urlPart.IndexOf('.') == urlPart.LastIndexOf('.'))
                    this._isUrl = false;

            // initialize other features
        }

        private QueryType _type;

        /// <summary>
        /// Query Type
        /// </summary>
        public QueryType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private Degree _difficulty;

        /// <summary>
        /// The difficulty of the query
        /// </summary>
        public Degree Difficulty
        {
            get { return _difficulty; }
            set { _difficulty = value; }
        }

        private Degree _popularity;

        /// <summary>
        /// The popularity of the query
        /// </summary>
        public Degree Popularity
        {
            get { return _popularity; }
            set { _popularity = value; }
        }

        private Degree _support;

        /// <summary>
        /// The support of the query
        /// this is a measure of how relevant is the query in the cluster. 
        /// We measure the support  of the query as the fraction of the documents 
        /// returned by the query that captured the attention of users. 
        /// It is estimated from query log as well.
        /// </summary>
        public Degree Support
        {
            get { return _support; }
            set { _support = value; }
        }

        private Degree _similarity;

        /// <summary>
        /// Query Similarity
        /// the similarity of the query to the input query
        /// </summary>
        public Degree Similarity
        {
            get { return _similarity; }
            set { _similarity = value; }
        }

        private bool? _isUrl = null;

        /// <summary>
        /// Query is a URL itself
        /// </summary>
        public bool? IsUrl
        {
            get
            {
                if (_isUrl == null)
                    InitializeQueryFeatures();
                return _isUrl;
            }
            set { _isUrl = value; }
        }

        private bool? _isFileName = null;

        /// <summary>
        /// Query is a file name itself
        /// </summary>
        public bool? IsFileName
        {
            get 
            { 
                if (_isFileName == null)
                    InitializeQueryFeatures();
                return _isFileName;
            }
            set { _isFileName = value; }
        }

        private float _osubQ;

        /// <summary>
        /// Overlap between Sub-Queries
        /// </summary>
        public float OSubQ
        {
            get { return _osubQ; }
            set { _osubQ = value; }
        }

        #endregion

        #region Statistics

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trainsetName"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        DataTable GetQueryTypeDistribution(QueryType type, float step)
        {
            //step = (step <= 0) ? 0.5F : step;

            //int rowNum = 10;

            //DataTable result = new DataTable();
            //result.Columns.Add("dvariance");
            //result.Columns.Add("queryNum");
            //DataRow row = null;

            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = this._conn;
            //cmd.CommandText = "SELECT COUNT(queryID) FROM [" + datasetName + "] WHERE" +
            //    " queryType=@queryType AND dvariance >= @left AND dvariance < @right";
            //cmd.Parameters.Add("@queryType", SqlDbType.SmallInt).Value = type.ID;
            //cmd.Parameters.Add("@left", SqlDbType.Float);
            //cmd.Parameters.Add("@right", SqlDbType.Float);

            //float left = 0, right = step;
            //try
            //{
            //    for (int i = 0; i < rowNum; i++, left = right, right += step)
            //    {
            //        row = result.NewRow();
            //        row["dvariance"] = (i == rowNum - 1) ?
            //            "[" + left + ",...)" :
            //            "[" + left + "," + right + ")";

            //        cmd.Parameters["@left"].Value = left;
            //        cmd.Parameters["@right"].Value =
            //            (i == rowNum - 1) ? 1000 : right;
            //        row["queryNum"] = cmd.ExecuteScalar();
            //        result.Rows.Add(row);
            //    }
            //    return result;
            //}
            //catch (SqlException ex)
            //{
            //    this._conn.Close();
            //    throw ex;
            //}
            return null;
        }

        #endregion

        #region Click Through Features

        /// <summary>
        /// Get click-through features for the query
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public ClickThroughFeature GetClickThroughFeatures(QueryLog ds)
        {
            return ClickThroughFeature.Get(_userQuery, ds);
        }

        /// <summary>
        /// Get click-through features for the query
        /// </summary>
        /// <param name="queryLog"></param>
        /// <returns></returns>
        public ClickThroughFeature GetClickThroughFeatures(string queryLog)
        {
            return ClickThroughFeature.Get(_userQuery, queryLog);
        }

        #endregion

        #region Linguisitic Features

        public LinguisticFeature GetLinguisticFeatures()
        {
            // queries are always in the form of phrases
            return LinguisticFeature.Get(_userQuery.Value, true);
        }

        #endregion

        #region Search Result List Features

        public SearchResultListFeature GetSearchResultFeatures(WebCollection ds)
        {
            return SearchResultListFeature.Get(_userQuery, ds);
        }

        #endregion
    }
}
