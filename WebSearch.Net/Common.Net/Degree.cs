using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.Common.Net
{
    public class Degree
    {
        public static Degree Best
        {
            get { return new Degree(0.90F); }
        }

        public static Degree Good
        {
            get { return new Degree(0.70F); }
        }

        public static Degree Normal
        {
            get { return new Degree(0.50F); }
        }

        public static Degree Bad
        {
            get { return new Degree(0.30F); }
        }

        public static Degree Worst
        {
            get { return new Degree(0.10F); }
        }

        #region Private Members

        private static readonly RangeF _bestRange =
            new RangeF(0.8F, true, 1.0F, true);

        private static readonly RangeF _goodRange =
            new RangeF(0.6F, true, 0.8F, false);

        private static readonly RangeF _normalRange =
            new RangeF(0.4F, true, 0.6F, false);

        private static readonly RangeF _badRange =
            new RangeF(0.2F, true, 0.4F, false);

        private static readonly RangeF _worstRange =
            new RangeF(0.0F, true, 0.2F, false);

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float Increase()
        {
            return Increase(0.1F);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public float Increase(float step)
        {
            this._value += step;
            _value = (_value > 1.0F) ? 1.0F : _value;
            return this._value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float Decrease()
        {
            return Decrease(0.1F);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public float Decrease(float step)
        {
            this._value -= step;
            _value = (_value < 0.0F) ? 0.0F : _value;
            return this._value;
        }

        #endregion

        #region Properties

        private float _value;

        /// <summary>
        /// The value of the degree [0.0F,1.0F]
        /// </summary>
        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public bool IsBest
        {
            get { return _bestRange.Includes(_value); }
        }

        public bool IsGood
        {
            get { return _goodRange.Includes(_value); }
        }

        public bool IsNormal
        {
            get { return _normalRange.Includes(_value); }
        }

        public bool IsBad
        {
            get { return _badRange.Includes(_value); }
        }

        public bool IsWorst
        {
            get { return _worstRange.Includes(_value); }
        }

        #endregion

        #region Constructors

        public Degree(float degree)
        {
            degree = (degree > 1) ? 1 : degree;
            degree = (degree < 0) ? 0 : degree;
            this._value = degree;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degree1"></param>
        /// <param name="degree2"></param>
        /// <returns></returns>
        public static bool operator ==(Degree degree1, Degree degree2)
        {
            if ((object)degree1 == (object)degree2)
                return true;
            if ((object)degree1 == null ||
                (object)degree2 == null)
                return false;
            if ((degree1.IsBest && degree2.IsBest) ||
                (degree1.IsGood && degree2.IsGood) ||
                (degree1.IsNormal && degree2.IsNormal) ||
                (degree1.IsBad && degree2.IsBad) ||
                (degree1.IsWorst && degree2.IsWorst))
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degree1"></param>
        /// <param name="degree2"></param>
        /// <returns></returns>
        public static bool operator !=(Degree degree1, Degree degree2)
        {
            return !(degree1 == degree2);
        }

        /// <summary>
        /// Type conversion: float->Degree
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Degree(float value)
        {
            return new Degree(value);
        }

        /// <summary>
        /// Type conversion: Degree->float
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static explicit operator float(Degree degree)
        {
            return degree.Value;
        }

        #endregion
    }
}
