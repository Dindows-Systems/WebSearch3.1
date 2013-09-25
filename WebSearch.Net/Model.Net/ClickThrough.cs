using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.Model.Net
{
    /// <summary>
    /// User's click through data
    /// </summary>
    public class ClickThrough : BaseModel
    {
        #region Properties

        private long _id;
        
        public long ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _userID;
        
        /// <summary>
        /// User id
        /// </summary>
        public string UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        private string _query;
        
        /// <summary>
        /// User's query text
        /// </summary>
        public string Query
        {
            get { return _query; }
            set { _query = value; }
        }

        private int _resultRank;
        
        /// <summary>
        /// Result url rank or AVG/MAX
        /// </summary>
        public int ResultRank
        {
            get { return _resultRank; }
            set { _resultRank = value; }
        }

        private string _resultUrl;
        
        /// <summary>
        /// Result url
        /// </summary>
        public string ResultUrl
        {
            get { return _resultUrl; }
            set { _resultUrl = value; }
        }

        private int _clickOrder;
        
        /// <summary>
        /// User's click order or AVG/MAX
        /// </summary>
        public int ClickOrder
        {
            get { return _clickOrder; }
            set { _clickOrder = value; }
        }

        private DateTime _time;
        
        /// <summary>
        /// User's query time
        /// </summary>
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }

        private int _count;

        /// <summary>
        /// count of click through items
        /// </summary>
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }
        
        #endregion

        #region Constructors

        public ClickThrough()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="query"></param>
        /// <param name="rank"></param>
        /// <param name="order"></param>
        /// <param name="url"></param>
        /// <param name="time"></param>
        public ClickThrough(string userID, string query, int rank, 
            int order, string url, DateTime time, int count)
        {
            this._userID = userID;
            this._query = query;
            this._resultRank = rank;
            this._clickOrder = order;
            this._resultUrl = url;
            this._time = time;
            this._count = count;
        }

        #endregion
    }
}
