using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.DataCenter.Net.DS;
using WebSearch.Model.Net;
using WebSearch.Common.Net;
using WebSearch.DataCenter.Net;
using WebSearch.Feature.Net.Properties;
using WebSearch.Maths.Net;

namespace WebSearch.Feature.Net
{
    public class ClickThroughFeature
    {
        #region Constructors

        private QueryLog _queryLog = null;

        private UserQuery _userQuery = null;

        public static ClickThroughFeature Get(UserQuery userQuery, QueryLog queryLog)
        {
            return new ClickThroughFeature(userQuery, queryLog);
        }

        public static ClickThroughFeature Get(UserQuery userQuery, string queryLog)
        {
            QueryLog ds = (QueryLog)DataSource.Get(DataSourceType.QueryLog, queryLog);
            if (ds == null) return null;
            return new ClickThroughFeature(userQuery, ds);
        }

        protected ClickThroughFeature(UserQuery userQuery, QueryLog queryLog)
        {
            this._userQuery = userQuery;
            this._queryLog = queryLog;
        }

        #endregion

        #region Click Through Features By Session

        protected void InitializeFeaturesBySession()
        {
            IQueryLog inf = DataRetriever.GetQueryLog(this._queryLog);
            inf.SetRetrievalType(QueryLog.RetrievalType.BySession);

            // search the click through list in query log
            List<ClickThrough> results = (List<ClickThrough>)inf.Retrieve(
                _userQuery.Value, MatchType.Exact, "",
                (int)Settings.Default.TestSetSize, "");
            
            float sessionNum = (float)results.Count;
            if (sessionNum == 0)
                return;
            
            int clickTotal = 0, nRSatisfied = 0, nCSatisfied = 0,
                nSucc1 = 0, nSucc5 = 0, nSucc10 = 0;
            DateTime startDate = results[0].Time, endDate = startDate;
            foreach (ClickThrough data in results)
            {
                if (data.Time < startDate)
                    startDate = data.Time;
                else if (data.Time > endDate)
                    endDate = data.Time;

                clickTotal += data.ClickOrder;
                if (data.ClickOrder < NClicks)
                    nCSatisfied++;
                if (data.ResultRank < NResults)
                    nRSatisfied++;
                if (data.ResultRank == 1)
                    nSucc1++;
                if (data.ResultRank <= 5)
                    nSucc5++;
                if (data.ResultRank <= 10)
                    nSucc10++;
            }
            // get the time span
            TimeSpan span = (endDate - startDate);
            // get the average request count for the query per day
            if (sessionNum == (int)Settings.Default.TestSetSize)
                this._AVGnR = (sessionNum / (float)(span.Days + 1)) * 30;
            else
                this._AVGnR = sessionNum;
            // get the average click count for the query per session
            this._AVGnC = (float)clickTotal / sessionNum;
            // get nCS and nRS feature values
            this._nCS = (float)nCSatisfied / sessionNum;
            this._nRS = (float)nRSatisfied / sessionNum;
            // get the succ@1, succ@5, succ@10
            this._successAt1 = (float)nSucc1 / sessionNum;
            this._successAt5 = (float)nSucc5 / sessionNum;
            this._successAt10 = (float)nSucc10 / sessionNum;
        }

        protected float _AVGnR = Const.Invalid;

        /// <summary>
        /// Average request count for the query per day
        /// account how many times the query is requested per day
        /// </summary>
        /// <remarks>the higher the AVGnR is, it's more 
        /// likely to be a popular query</remarks>
        public float AVGnR
        {
            get
            {
                if (_AVGnR == Const.Invalid) 
                    InitializeFeaturesBySession();
                return _AVGnR;
            }
            set { _AVGnR = value; }
        }

        protected float _AVGnC = Const.Invalid;

        /// <summary>
        /// Average click number per session for the query 
        /// account how many results a user clicks on after the query is issued.
        /// </summary>
        /// <remarks>the lower the AVGnC is, it's more  
        /// likely to be a navigational query</remarks>
        public float AVGnC
        {
            get 
            {
                if (_AVGnC == Const.Invalid)
                    InitializeFeaturesBySession(); 
                return _AVGnC;
            }
            set { _AVGnC = value; }
        }

        protected float _nRS = Const.Invalid;
        public const int NResults = 5;

        /// <summary>
        /// Top n Results Satisfied
        /// #session involving (max (rank) &lt; n) / #session
        /// </summary>
        /// <remarks>the higher the nRS is, it's more 
        /// likely to be a navigational query</remarks>
        public float nRS
        {
            get
            {
                if (_nRS == Const.Invalid)
                    InitializeFeaturesBySession();
                return _nRS;
            }
            set { _nRS = value; }
        }

        protected float _nCS = Const.Invalid;
        public const int NClicks = 2;

        /// <summary>
        /// N Clicks Satisfied
        /// #session involving (#click &lt; n) / #session 
        /// </summary>
        /// <remarks>the higher the nCS is, it;s more 
        /// likely to be a navigational query</remarks>
        public float nCS
        {
            get
            {
                if (_nCS == Const.Invalid)
                    InitializeFeaturesBySession();
                return _nCS;
            }
            set { _nCS = value; }
        }

        protected float _successAt1 = Const.Invalid;

        /// <summary>
        /// Success at Rank 1
        /// the proportion of queries for which a good answer was at rank 1
        /// </summary>
        /// <remarks> it is the first result the user sees.</remarks>
        public float SuccessAt1
        {
            get
            {
                if (_successAt1 == Const.Invalid)
                    InitializeFeaturesBySession();
                return _successAt1;
            }
            set { _successAt1 = value; }
        }

        protected float _successAt5 = Const.Invalid;

        /// <summary>
        /// Success in Top 5
        /// the proportion of queries for which one or
        /// more good answers were in the top 5
        /// </summary>
        /// <remarks>the top 5 results appear without 
        /// page scrolling.</remarks>
        public float SuccessAt5
        {
            get
            {
                if (_successAt5 == Const.Invalid)
                    InitializeFeaturesBySession();
                return _successAt5;
            }
            set { _successAt5 = value; }
        }

        protected float _successAt10 = Const.Invalid;

        /// <summary>
        /// Success in Top 10
        /// the proportion of queries for which one or 
        /// more good answers were in the top 10
        /// </summary>
        /// <remarks>the top 10 results appear without
        /// the need to turn the page</remarks>
        public float SuccessAt10
        {
            get
            {
                if (_successAt10 == Const.Invalid)
                    InitializeFeaturesBySession();
                return _successAt10;
            }
            set { _successAt10 = value; }
        }

        #endregion

        #region Click Through Features By Link

        protected void InitializeFeaturesByLink()
        {
            IQueryLog inf = DataRetriever.GetQueryLog(this._queryLog);
            inf.SetRetrievalType(QueryLog.RetrievalType.ByLink);

            // search the click through list in query log
            List<ClickThrough> results = (List<ClickThrough>)inf.Retrieve(
                _userQuery.Value, MatchType.Exact, "", 
                (int)Settings.Default.TestSetSize, QueryLog.Count + " DESC");

            // get the url result number in query log
            this._urlResultNum = results.Count;
            if (_urlResultNum == 0)
                return;

            // get the best url
            string url = results[0].ResultUrl.ToLower();
            if (!url.StartsWith("http") && !url.StartsWith("ftp"))
                url = "http://" + url;
            this._bestUrlResult = new Hyperlink(url);
            this._bestUrlAVGRank = results[0].ResultRank;
            this._bestUrlAVGOrder = results[0].ClickOrder;
            this._bestUrlClickNum = results[0].Count;
        }

        protected int _urlResultNum = Const.Invalid;

        /// <summary>
        /// Url Result Number
        /// the number of distinct result urls for the query
        /// </summary>
        public int UrlResultNum
        {
            get
            {
                if (_urlResultNum == Const.Invalid)
                    InitializeFeaturesByLink();
                return _urlResultNum;
            }
            set { _urlResultNum = value; }
        }

        protected Hyperlink _bestUrlResult = null;

        /// <summary>
        /// Best Url Result
        /// having the highest click count
        /// </summary>
        public Hyperlink BestUrlResult
        {
            get
            {
                if (_bestUrlResult == null)
                    InitializeFeaturesByLink();
                return _bestUrlResult;
            }
            set { _bestUrlResult = value; }
        }

        protected float _bestUrlClickNum = Const.Invalid;

        /// <summary>
        /// Best Url's Click Num
        /// </summary>
        public float BestUrlClickNum
        {
            get
            {
                if (_bestUrlClickNum == Const.Invalid)
                    InitializeFeaturesByLink();
                return _bestUrlClickNum;
            }
            set { _bestUrlClickNum = value; }
        }

        protected float _bestUrlAVGRank;

        /// <summary>
        /// Average Rank of the best Url
        /// </summary>
        public float BestUrlAVGRank
        {
            get
            {
                if (_bestUrlAVGRank == Const.Invalid)
                    InitializeFeaturesByLink();
                return _bestUrlAVGRank;
            }
            set { _bestUrlAVGRank = value; }
        }

        protected float _bestUrlAVGOrder;

        /// <summary>
        /// Average click order of the best Url
        /// </summary>
        public float BestUrlAVGOrder
        {
            get
            {
                if (_bestUrlAVGOrder == Const.Invalid)
                    InitializeFeaturesByLink();
                return _bestUrlAVGOrder;
            }
            set { _bestUrlAVGOrder = value; }
        }

        #endregion

        #region Click Through Features By Rank

        protected void InitializeFeaturesByRank()
        {
            IQueryLog inf = DataRetriever.GetQueryLog(this._queryLog);
            inf.SetRetrievalType(QueryLog.RetrievalType.ByRank);

            // search the click through data by rank
            List<ClickThrough> results = (List<ClickThrough>)inf.Retrieve(
                _userQuery.Value, MatchType.Exact, "rank<=10", 
                (int)Settings.Default.TestSetSize, QueryLog.Rank + " DESC");

            float[] histogram = new float[10]; // 10
            int totalClicks = 0;
            foreach (ClickThrough data in results)
            {
                // the rank is 1~10, here is 0~9
                histogram[data.ResultRank - 1] = data.Count;
                totalClicks += data.Count;
            }
            // normalize the histogram
            for (int i = 0; i < 10; i++)
                histogram[i] /= totalClicks;

            this._meanCD = Statistics.Mean(histogram) * 10;
            this._medianCD = Statistics.Median(histogram) * 10;
            this._stdDevCD = Statistics.StdDev(histogram) * 10;
        }

        protected float _medianCD = Const.Invalid;

        /// <summary>
        /// Click Distribution
        /// the median of click frequency along the answer rank
        /// </summary>
        public float MedianCD
        {
            get
            {
                if (_medianCD == Const.Invalid)
                    InitializeFeaturesByRank();
                return _medianCD;
            }
            set { _medianCD = value;}
        }

        protected float _meanCD = Const.Invalid;

        /// <summary>
        /// Click Distribution
        /// the mean of click frequency along the answer rank
        /// </summary>
        public float MeanCD
        {
            get
            {
                if (_meanCD == Const.Invalid)
                    InitializeFeaturesByRank();
                return _meanCD;
            }
            set { _meanCD = value; }
        }

        protected float _stdDevCD = Const.Invalid;

        /// <summary>
        /// Click Distribution
        /// the std dev of click frequency along the answer rank
        /// </summary>
        public float StdDevCD
        {
            get
            {
                if (_stdDevCD == Const.Invalid)
                    InitializeFeaturesByRank();
                return _stdDevCD;
            }
            set { _stdDevCD = value; }
        }

        #endregion

        #region Click Through Features By Click Order

        protected void InitializeFeaturesByClickOrder()
        {
            IQueryLog inf = DataRetriever.GetQueryLog(this._queryLog);
            inf.SetRetrievalType(QueryLog.RetrievalType.ByClickOrder);
            
            // retrieve the click through data by click order
            List<ClickThrough> results = (List<ClickThrough>)inf.Retrieve(
                _userQuery.Value, MatchType.Exact, "",
                (int)Settings.Default.TestSetSize, "");

            float sigma = 0;
            foreach (ClickThrough data in results)
                sigma += 1F / (float)data.ResultRank;
            this._meanRR = sigma / (float)results.Count;
        }

        protected float _meanRR = Const.Invalid;

        /// <summary>
        /// Mean Reciprocal Rank
        /// </summary>
        public float MRR
        {
            get 
            {
                if (_meanRR == Const.Invalid)
                    InitializeFeaturesByClickOrder();
                return _meanRR;
            }
            set { _meanRR = value; }
        }

        #endregion

        #region Other Click Through Features

        protected void InitializeOtherFeatures()
        {
            IQueryLog inf = DataRetriever.GetQueryLog(this._queryLog);
            inf.SetRetrievalType(QueryLog.RetrievalType.Normal);
        }

        protected HyperlinkType _lTat1C = HyperlinkType.UnKnown;

        /// <summary>
        /// Link Type at 1st Click
        /// </summary>
        /// <remarks> 1st click is the representative result 
        /// of the query in client user¡¯s view</remarks>
        public HyperlinkType LTat1C
        {
            get
            { 
                if (_lTat1C == HyperlinkType.UnKnown)
                    InitializeOtherFeatures();
                return _lTat1C;
            }
            set { _lTat1C = value; }
        }

        #endregion
    }
}
