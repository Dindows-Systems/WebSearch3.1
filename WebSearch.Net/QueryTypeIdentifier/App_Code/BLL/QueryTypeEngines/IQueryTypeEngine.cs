using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Model.Net;
using Common.Net;
using SJTU.CS.Apex.WebSearch.Util;

namespace SJTU.CS.Apex.WebSearch.BLL.QueryTypeEngines
{
    /// <summary>
    /// Summary description for IQueryTypeEngine
    /// </summary>
    public abstract class IQueryTypeEngine
    {
        protected String _queryText = null;

        protected float _feature = Const.Unknown;

        protected StatMethod _statMethod = StatMethod.Median;

        protected RetrievalType _resultType = RetrievalType.Unknown;

        #region For Data Resource: QueryLog:

        protected DataTable _queryLogResultTable = null;

        protected QueryLog _queryLog = Config.CurrentQueryLog;

        #endregion

        #region For Data Resource: SearchEngine:

        protected IList<SearchEngineResult> _searchEngineResults = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor. Every param depend on web.config
        /// </summary>
        public IQueryTypeEngine() { }

        #region Constructors For Data Resource: QueryLog

        public IQueryTypeEngine(QueryLog queryLog)
        {
            this._queryLog = queryLog;
        }
        
        /// <summary>
        /// Accept the query log result data table as param
        /// </summary>
        /// <param name="queryLogResultTable"></param>
        public IQueryTypeEngine(DataTable queryLogResultTable)
        { _queryLogResultTable = queryLogResultTable; }

        /// <summary>
        /// Specify the method of statistics used in the engine
        /// </summary>
        /// <param name="queryLogResultTable"></param>
        /// <param name="statMethod"></param>
        public IQueryTypeEngine(DataTable queryLogResultTable, StatMethod statMethod)
        {
            _queryLogResultTable = queryLogResultTable;
            this._statMethod = statMethod;
        }

        #endregion

        #region Constructors For Data Resource: SearchEngine

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchEngineResult"></param>
        public IQueryTypeEngine(IList<SearchEngineResult> searchEngineResults)
        { _searchEngineResults = searchEngineResults; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchEngineResults"></param>
        /// <param name="statMethod"></param>
        public IQueryTypeEngine(IList<SearchEngineResult> searchEngineResults, StatMethod statMethod)
        {
            _searchEngineResults = searchEngineResults;
            this._statMethod = statMethod;
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the query type according to the query text
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public QueryType GetQueryType(String query)
        {
            this._queryText = query;
            return GetQueryType();
        }

        /// <summary>
        /// Get the query type according to the query text
        /// </summary>
        /// <returns></returns>
        public abstract QueryType GetQueryType();

        #endregion

        /// <summary>
        /// Should be called at the first sentence of GetQueryType()
        /// </summary>
        protected void PrepareQueryLogResultTable(RetrievalType type)
        {
            if (this._queryText == null || _queryText == "")
                throw new Exception("unknown query text");

            if (this._queryLogResultTable == null || this._resultType != type)
            {
                IQueryLogDAO dao = DALHelper.GetQueryLogDAO(this._queryLog, type);

                // get/update the query log result table.
                switch (Config.FTSType)
                {
                    case FTS.CONTAINS:
                        _queryLogResultTable = dao.Search(_queryText, 
                            Config.AnalysisSetSize); break;
                    case FTS.CONTAINSTABLE:
                        _queryLogResultTable = dao.RankedSearch(_queryText, 50,
                            Config.AnalysisSetSize); break;
                    case FTS.FREETEXT:
                        _queryLogResultTable = dao.FreeTextSearch(_queryText, 
                            Config.AnalysisSetSize); break;
                    case FTS.FREETEXTTABLE:
                        _queryLogResultTable = dao.RankedFreeTextSearch(_queryText,
                            50, Config.AnalysisSetSize); break;
                    default:
                        throw new Exception("PrepareQueryLogResultTable->FTSType error");
                }
            }
        }

        public void PrepareSearchEngineResults(RetrievalType type)
        {
            if (this._queryText == null || _queryText == "")
                throw new Exception("unknown query text");

            if (this._searchEngineResults == null || this._resultType != type)
            {
                ISearchEngineDAO dao = DALHelper.GetSearchEngineDAO(type);
                _searchEngineResults = dao.Search(_queryText, Config.AnalysisSetSize);
            }
        }

        #region Public Properties

        /// <summary>
        /// Access to the data table of query result in log db
        /// </summary>
        public DataTable QueryLogResultTable 
        {
            get { return this._queryLogResultTable; }
            set { this._queryLogResultTable = value; }
        }

        /// <summary>
        /// The query text
        /// </summary>
        public String QueryText 
        {
            get { return this._queryText; }
            set { this._queryText = value; }
        }

        /// <summary>
        /// The feature abstracted
        /// </summary>
        public virtual float Feature
        {
            get { return this._feature; }
            set { this._feature = value; }
        }

        /// <summary>
        /// The statistical method used in the engine
        /// </summary>
        public virtual StatMethod StatisticalMethod
        {
            get { return this._statMethod; }
            set
            {
                this._statMethod = value;
                // since the statical method is changed, 
                // we need to recalculate the feature value
                this._feature = Const.Unknown;
            }
        }

        public QueryLog CurrentQueryLog
        {
            get { return _queryLog; }
            set { _queryLog = value; }
        }

        #endregion

        #region Useful Calculations (can only be called inside GetQueryType function)

        protected int _sessionNum = Const.Invalid;

        /// <summary>
        /// Session number for the query text
        /// </summary>
        public int SessionNum
        {
            get
            {
                if (_sessionNum == Const.Invalid)
                {
                    // update the session num
                    _sessionNum = int.Parse(this._queryLogResultTable.Compute(
                        "COUNT(" + QueryLogFields.User + ")", "").ToString());
                }
                return _sessionNum;
            }
            set { this._sessionNum = value; }
        }
        
        #endregion
    }
}