using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WebSearch.DataMiner.Net
{
    public class Attribute
    {
        #region Properties

        protected ArrayList _values;

        public ArrayList Values
        {
            get { return _values; }
            set { _values = value; }
        }

        public string[] GetStringValues()
        {
            if (_values != null)
                return (string[])_values.ToArray(typeof(string));
            return null;
        }

        public int[] GetIntValues()
        {
            if (_values != null)
                return (int[])_values.ToArray(typeof(int));
            return null;
        }

        public float[] GetFloatValues()
        {
            if (_values != null)
                return (float[])_values.ToArray(typeof(float));
            return null;
        }

        public double[] GetDoubleValues()
        {
            if (_values != null)
                return (double[])_values.ToArray(typeof(double));
            return null;
        }

        protected string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected object _label;

        public object Label
        {
            get { return _label; }
            set { _label = value; }
        }

        #endregion

        #region Constructors

        public Attribute(string name, string[] values)
        {
            this._name = name;
            this._values = new ArrayList(values);
            this._values.Sort();
        }

        public Attribute(object Label)
        {
            this._label = Label;
            this._name = string.Empty;
            this._values = null;
        }

        #endregion

        #region Methods

        public int GetIndex(string value)
        {
            if (_values != null)
                return _values.BinarySearch(value);
            else
                return -1;
        }

        public bool IsValidValue(string value)
        {
            return GetIndex(value) >= 0;
        }

        public override string ToString()
        {
            if (_name != null)
                return _name;
            return _label.ToString();
        }

        #endregion
    }
}
