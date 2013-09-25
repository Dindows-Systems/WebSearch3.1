using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for Range
/// </summary>
namespace SJTU.CS.Apex.WebSearch.Util
{
    public class Range
    {
        private int _min;
        private int _max;

        #region Properties

        /// <summary>
        /// The lower bound of the range
        /// </summary>
        public int Min
        {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        /// The upper bound of the range
        /// </summary>
        public int Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public int Mid
        {
            get { return (int)Math.Round((float)(_min + _max) / 2F); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public Range(int min, int max)
        {
            this._min = min;
            this._max = max;
        }

        #endregion

        public bool Include(int value)
        {
            return (value <= _max && value >= _min);
        }
    }

    public class RangeF
    {
        private float _min;
        private float _max;

        #region Properties

        /// <summary>
        /// The lower bound of the range
        /// </summary>
        public float Min
        {
            get { return _min; }
            set { _min = value; }
        }

        public float Max
        {
            get { return _max; }
            set { _max = value; }
        }

        private float Mid
        {
            get { return (_min + _max) / 2F; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public RangeF(float min, float max)
        {
            this._min = min;
            this._max = max;
        }

        #endregion

        public bool Include(float value)
        {
            return (value <= _max && value >= _min);
        }
    }
}
