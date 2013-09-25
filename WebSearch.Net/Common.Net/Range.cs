using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.Common.Net
{
    public class Range
    {
        #region Properties

        private int _min;

        /// <summary>
        /// The lower bound of the range
        /// </summary>
        public int Min
        {
            get { return _min; }
            set { _min = value; }
        }

        private bool _minInclude = true;

        /// <summary>
        /// 
        /// </summary>
        public bool MinInclude
        {
            get { return _minInclude; }
            set { _minInclude = value; }
        }

        private int _max;

        /// <summary>
        /// The upper bound of the range
        /// </summary>
        public int Max
        {
            get { return _max; }
            set { _max = value; }
        }

        private bool _maxInclude = true;

        /// <summary>
        /// 
        /// </summary>
        public bool MaxInclude
        {
            get { return _maxInclude; }
            set { _maxInclude = value; }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="minInclude"></param>
        /// <param name="max"></param>
        /// <param name="maxInclude"></param>
        public Range(int min, bool minInclude, int max, bool maxInclude)
            : this(min, max)
        {
            this._minInclude = minInclude;
            this._maxInclude = maxInclude;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Includes(int value)
        {
            if (value < this._min || value > this._max)
                return false;
            if (!this._minInclude && _min == value)
                return false;
            if (!this._maxInclude && _max == value)
                return false;

            return true;
        }

        public static bool operator ==(Range range1, Range range2)
        {
            return (range1._max == range2._max &&
                range1._min == range2._min &&
                range1._maxInclude == range2._maxInclude &&
                range1._minInclude == range2._minInclude);
        }

        public static bool operator !=(Range range1, Range range2)
        {
            return !(range1 == range2);
        }

        #endregion
    }

    public class RangeF
    {
        #region Properties

        private float _min;

        /// <summary>
        /// The lower bound of the range
        /// </summary>
        public float Min
        {
            get { return _min; }
            set { _min = value; }
        }

        private bool _minInclude = true;

        /// <summary>
        /// 
        /// </summary>
        public bool MinInclude
        {
            get { return _minInclude; }
            set { _minInclude = value; }
        }

        private float _max;

        /// <summary>
        /// The upper bound of the range
        /// </summary>
        public float Max
        {
            get { return _max; }
            set { _max = value; }
        }

        private bool _maxInclude = true;

        /// <summary>
        /// 
        /// </summary>
        public bool MaxInclude
        {
            get { return _maxInclude; }
            set { _maxInclude = value; }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="minInclude"></param>
        /// <param name="max"></param>
        /// <param name="maxInclude"></param>
        public RangeF(float min, bool minInclude, float max, bool maxInclude)
            : this(min, max)
        {
            this._minInclude = minInclude;
            this._maxInclude = maxInclude;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Includes(float value)
        {
            if (value < this._min || value > this._max)
                return false;
            if (!this._minInclude && _min == value)
                return false;
            if (!this._maxInclude && _max == value)
                return false;

            return true;
        }

        public static bool operator ==(RangeF range1, RangeF range2)
        {
            return (range1._max == range2._max &&
                range1._min == range2._min &&
                range1._maxInclude == range2._maxInclude &&
                range1._minInclude == range2._minInclude);
        }

        public static bool operator !=(RangeF range1, RangeF range2)
        {
            return !(range1 == range2);
        }

        #endregion
    }
}
