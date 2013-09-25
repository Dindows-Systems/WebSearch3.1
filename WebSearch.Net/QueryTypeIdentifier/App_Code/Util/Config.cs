using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SJTU.CS.Apex.WebSearch.Model;


namespace SJTU.CS.Apex.WebSearch.Util
{
    /// <summary>
    /// Full Text Search Types
    /// </summary>
    public enum FTS
    {
        UnknownType = -2,
        CONTAINS = 0,
        CONTAINSTABLE,
        FREETEXT,
        FREETEXTTABLE
    }

    public enum AnalysisType
    {
        Test = 1,
        Train = 2
    }

    /// <summary>
    /// Current Setting of the System
    /// </summary>
    public class Config
    {
        #region Query Log Table Settings

        private static QueryLog _currentQueryLog = null;

        public static QueryLog CurrentQueryLog
        {
            get
            {
                if (_currentQueryLog == null)
                {
                    String logName = ConfigurationManager.AppSettings["QueryLog"];
                    if (logName == "Sogou")
                        _currentQueryLog = QueryLogs.Sogou;
                    else if (logName == "Baidu")
                        _currentQueryLog = QueryLogs.Baidu;
                    else if (logName == "Yahoo")
                        _currentQueryLog = QueryLogs.Yahoo;
                    else if (logName == "Google")
                        _currentQueryLog = QueryLogs.Google;
                    else
                        throw new Exception("Unknown Query Log");
                }
                return _currentQueryLog;
            }
        }

        private static FTS _ftsType = FTS.UnknownType;

        public static FTS FTSType
        {
            get // using the Singleton design pattern.
            {
                if (_ftsType == FTS.UnknownType)  // if the queryColumn is null, prepare the queryColumn info
                {
                    String type = ConfigurationManager.AppSettings["FTSType"].ToUpper();
                    if (type == "CONTAINS")
                        _ftsType = FTS.CONTAINS;
                    else if (type == "CONTAINSTABLE")
                        _ftsType = FTS.CONTAINSTABLE;
                    else if (type == "FREETEXT")
                        _ftsType = FTS.FREETEXT;
                    else if (type == "FREETEXTTABLE")
                        _ftsType = FTS.FREETEXTTABLE;
                    else
                        throw new Exception("FTSType error");
                }
                return _ftsType;
            }
        }

        private static int _ftsMinRank = Const.Invalid;

        public static int FTSMinRank
        {
            get
            {
                if (_ftsMinRank == Const.Invalid)
                {
                    _ftsMinRank = int.Parse(ConfigurationManager.
                        AppSettings["FTSMinRank"]);
                }
                return _ftsMinRank;
            }
        }

        private static int _analysisSetSize = Const.Invalid;

        public static int AnalysisSetSize
        {
            get
            {
                if (_analysisSetSize == Const.Invalid)
                {
                    _analysisSetSize = int.Parse(ConfigurationManager.
                        AppSettings["AnalysisSetSize"]);
                }
                return _analysisSetSize;
            }
        }

        private static int _averageClickPerSession = Const.Invalid;

        public static int AverageClickPerSession
        {
            get
            {
                if (_averageClickPerSession == Const.Invalid)
                {
                    _averageClickPerSession = int.Parse(ConfigurationManager.
                        AppSettings["AverageClickPerSession"]);
                }
                return _averageClickPerSession;
            }
        }

        private static int _totalLogNum = Const.Invalid;

        public static int TotalLogNum
        {
            get
            {
                if (_totalLogNum == Const.Invalid)
                {
                    _totalLogNum = int.Parse(ConfigurationManager.
                        AppSettings["TotalLogNum"]);
                }
                return _totalLogNum;
            }
        }

        #endregion

        #region Path of Web Server

        private static String _queryTypeEnginesLayerPath = null;

        public static String QueryTypeEnginesLayerPath
        {
            get
            {
                if (_queryTypeEnginesLayerPath == null)
                {
                    _queryTypeEnginesLayerPath = ConfigurationManager.
                        AppSettings["QueryTypeEnginesLayerPath"];
                }
                return _queryTypeEnginesLayerPath;
            }
        }

        private static String _autoCompleteEnginesPath = null;

        public static String AutoCompleteEnginesPath
        {
            get 
            {
                if (_autoCompleteEnginesPath == null)
                {
                    _autoCompleteEnginesPath = ConfigurationManager.
                        AppSettings["AutoCompleteEnginesPath"];
                }
                return _autoCompleteEnginesPath;
            }
        }

        #endregion

        /// <summary>
        /// Refresh all the setting according to the current config
        /// </summary>
        public static void Refresh()
        {
            // set every thing to null
            Config._ftsType = FTS.UnknownType;
            Config._analysisSetSize = Const.Invalid;
            Config._averageClickPerSession = Const.Invalid;
        }
    }
}