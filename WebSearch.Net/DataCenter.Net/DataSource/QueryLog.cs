using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.DataCenter.Net.DS
{
    public class QueryLog : DataSource
    {
        #region Constructors

        internal QueryLog(string name)
        {
            this._type = DataSourceType.QueryLog;
            this._name = name;
        }

        internal QueryLog(string name, DateTime startDate, DateTime endDate)
            : this(name)
        {
            this._startDate = startDate;
            this._endDate = endDate;
            this._dateSpan = endDate - startDate;
        }

        #endregion

        #region Properties

        private RetrievalType _retrieveType = RetrievalType.Normal;

        public RetrievalType RetrieveType
        {
            get { return _retrieveType; }
            set { _retrieveType = value; }
        }

        private DateTime _startDate;

        /// <summary>
        /// 
        /// </summary>
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        private DateTime _endDate;

        /// <summary>
        /// 
        /// </summary>
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }

        private TimeSpan _dateSpan;

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan DateSpan
        {
            get { return _dateSpan; }
        }

        #endregion

        #region Query Log Field Names

        public const string ID = "logID";

        public const string User = "userID";

        public const string Query = "queryText";

        public const string Rank = "rank";

        public const string Order = "sequence";

        public const string Anchor = "";

        public const string Link = "url";

        public const string Time = "ntime";

        public const string Count = "ncount";

        #endregion

        /// <summary>
        /// Retrieval type
        /// </summary>
        public enum RetrievalType
        {
            Unknown = -2,
            Normal = 0,
            BySession,
            ByQuery,
            ByClickOrder,
            ByLink,
            ByRank
        }
    }
}
