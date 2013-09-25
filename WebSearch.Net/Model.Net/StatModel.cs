using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Common.Net;
using WebSearch.Maths.Net;

namespace WebSearch.Model.Net
{
    public class StatModel
    {
        #region Properties

        private double[] _data;

        public double[] Data
        {
            get { return _data; }
        }

        private int _size;

        /// <summary>
        /// Size of the data array
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        private double _min = double.MaxValue;

        /// <summary>
        /// Max of the data array
        /// </summary>
        public double Min
        {
            get { return Math.Round(_min, 5); }
        }

        private double _max = double.MinValue;

        /// <summary>
        /// Min of the data array
        /// </summary>
        public double Max
        {
            get { return Math.Round(_max, 5); }
        }

        private double _sum = 0;

        /// <summary>
        /// Sum of the data array
        /// </summary>
        public double Sum
        {
            get { return Math.Round(_sum, 5); }
        }

        private double _mean = 0;

        /// <summary>
        /// Mean of the data array
        /// </summary>
        public double Mean
        {
            get { return Math.Round(_mean, 5); }
        }

        private double _stdDev = 0;

        /// <summary>
        /// Standard Deviation
        /// </summary>
        public double StdDev
        {
            get { return Math.Round(_stdDev, 5); }
        }

        private double _median;

        /// <summary>
        /// Median of the data array
        /// </summary>
        public double Median
        {
            get { return Math.Round(_median, 5); }
        }

        private double _variance = 0;

        /// <summary>
        /// Variance of the data array
        /// </summary>
        public double Variance
        {
            get { return Math.Round(_variance, 5); }
        }

        private double _entropy = 0;
        /// <summary>
        /// Entropy of the data array
        /// </summary>
        public double Entropy
        {
            get { return Math.Round(_entropy, 5); }
        }

        #endregion

        #region Constructors

        public StatModel(double[] data)
        {
            this._data = data;
            // initialize the stat. features
            InitializeStatModel();
        }

        public StatModel(float[] data)
        {
            this._data = new double[data.Length];
            for (int i = 0; i < _data.Length; i++)
                _data[i] = (double)data[i];
            // initialize the stat. features
            InitializeStatModel();
        }

        public StatModel(int[] data)
        {
            this._data = new double[data.Length];
            for (int i = 0; i < _data.Length; i++)
                _data[i] = (double)data[i];
            // initialize the stat. features
            InitializeStatModel();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculate the stat. features
        /// </summary>
        protected void InitializeStatModel()
        {
            if (_data.Length <= 0)
                return; // no necessity to continue

            // because we want to get all the features in less iterations, 
            // we do not call Statistics's methods here
            this._size = this._data.Length;
            double squareSum = 0;
            foreach (double v in this._data)
            {
                _min = (v < _min) ? v : _min;
                _max = (v > _max) ? v : _max;
                this._sum += v;
                squareSum += (v * v);
            }
            // get mean value
            this._mean = _sum / (double)_size;

            if (_sum == 0)
            {
                this._stdDev = 0;
                this._entropy = -1;
            }
            else
            {
                // get stddev and entropy value
                for (int i = 0; i < _size; i++)
                {
                    double v = _data[i];
                    double t = (double)i - _mean;

                    // accumulate mean
                    _stdDev += t * t * v;

                    double p = _data[i] / _sum;
                    if ((float)p != 0F)
                        _entropy += (-p * (double)Math.Log((double)p, 2));
                }
                this._stdDev = (double)Math.Sqrt((double)(_stdDev / _sum));
            }
            // get median value
            double[] copy = (double[])_data.Clone();
            Array.Sort<double>(copy);
            int mid = _size / 2;
            this._median = (_size % 2 != 0) ? copy[mid] :
                (copy[mid] + copy[mid - 1]) / (double)2;
            // get variance value
            this._variance = (squareSum / (double)_size) - _mean * _mean;
        }

        #endregion
    }
}
