using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using WebSearch.Maths.Net;
using System.Collections;

namespace WebSearch.DataMiner.Net.DT
{
    public class DecisionTreeID3
    {
        #region Properties

        protected DataTable _samples;

        protected int _totalPositives = 0;

        protected int _total = 0;

        protected string _targetAttribute = "result";

        protected double _entropySet = 0.0;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="targetAttribute"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public TreeNode MountTree(DataTable samples, string targetAttribute, Attribute[] attributes)
        {
            this._samples = samples;
            return InternalMountTree(_samples, targetAttribute, attributes);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        protected int CountTotalPositives(DataTable samples)
        {
            int result = 0;
            foreach (DataRow row in samples.Rows)
            {
                if ((bool)row[_targetAttribute] == true)
                    result++;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <param name="positives"></param>
        /// <param name="negatives"></param>
        protected void GetValuesToAttribute(DataTable samples, Attribute attribute,
            string value, out int positives, out int negatives)
        {
            positives = 0;
            negatives = 0;

            foreach (DataRow row in samples.Rows)
            {
                if (((string)row[attribute.Name] == value))
                {
                    if ((bool)row[_targetAttribute] == true)
                        positives++;
                    else
                        negatives++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        protected double GetGain(DataTable samples, Attribute attribute)
        {
            string[] values = attribute.GetStringValues();
            double sum = 0.0;

            for (int i = 0; i < values.Length; i++)
            {
                int positives = 0, negatives = 0;

                GetValuesToAttribute(samples, attribute, values[i], out positives, out negatives);

                double entropy = Statistics.Entropy(positives, negatives);
                sum += -(double)(positives + negatives) / _total * entropy;
            }
            return _entropySet + sum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        protected Attribute GetBestAttribute(DataTable samples, Attribute[] attributes)
        {
            double maxGain = 0.0;
            Attribute result = null;

            foreach (Attribute attribute in attributes)
            {
                double aux = GetGain(samples, attribute);
                // the attribute with the largest Gain is the best.
                if (aux > maxGain)
                {
                    maxGain = aux;
                    result = attribute;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="targetAttribute"></param>
        /// <returns></returns>
        protected bool IsAllPositives(DataTable samples, string targetAttribute)
        {
            foreach (DataRow row in samples.Rows)
            {
                if ((bool)row[targetAttribute] == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="targetAttribute"></param>
        /// <returns></returns>
        protected bool IsAllNegatives(DataTable samples, string targetAttribute)
        {
            foreach (DataRow row in samples.Rows)
            {
                if ((bool)row[targetAttribute] == true)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="targetAttribute"></param>
        /// <returns></returns>
        protected ArrayList GetDistinctValues(DataTable samples, string targetAttribute)
        {
            ArrayList distinctValues = new ArrayList(samples.Rows.Count);
            foreach (DataRow row in samples.Rows)
            {
                if (distinctValues.IndexOf(row[targetAttribute]) == -1)
                    distinctValues.Add(row[targetAttribute]);
            }
            return distinctValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="targetAttribute"></param>
        /// <returns></returns>
        private object GetMostCommonValue(DataTable samples, string targetAttribute)
        {
            ArrayList distinctValues = GetDistinctValues(samples, targetAttribute);
            int[] count = new int[distinctValues.Count];

            foreach (DataRow row in samples.Rows)
            {
                int index = distinctValues.IndexOf(row[targetAttribute]);
                count[index]++;
            }

            int maxIndex = 0, maxCount = 0;

            for (int i = 0; i < count.Length; i++)
            {
                if (count[i] > maxCount)
                {
                    maxCount = count[i];
                    maxIndex = i;
                }
            }

            return distinctValues[maxIndex];
        }

        private TreeNode InternalMountTree(DataTable samples, string targetAttribute, Attribute[] attributes)
        {
            if (IsAllPositives(samples, targetAttribute))
                return new TreeNode(new Attribute(true));

            if (IsAllNegatives(samples, targetAttribute))
                return new TreeNode(new Attribute(false));

            if (attributes.Length == 0)
                return new TreeNode(new Attribute(GetMostCommonValue(samples, targetAttribute)));

            this._total = samples.Rows.Count;
            this._targetAttribute = targetAttribute;
            this._totalPositives = CountTotalPositives(samples);

            this._entropySet = Statistics.Entropy(_totalPositives, _total - _totalPositives);

            Attribute bestAttribute = GetBestAttribute(samples, attributes);

            TreeNode root = new TreeNode(bestAttribute);

            DataTable aSample = samples.Clone();

            foreach (string value in bestAttribute.Values)
            {
                aSample.Rows.Clear();

                DataRow[] rows = samples.Select(bestAttribute.Name + " = " + "'" + value + "'");

                foreach (DataRow row in rows)
                    aSample.Rows.Add(row.ItemArray);
                
                ArrayList aAttributes = new ArrayList(attributes.Length - 1);
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i].Name != bestAttribute.Name)
                        aAttributes.Add(attributes[i]);
                }

                if (aSample.Rows.Count == 0)
                    return new TreeNode(new Attribute(GetMostCommonValue(aSample, targetAttribute)));
                else
                {
                    DecisionTreeID3 dc3 = new DecisionTreeID3();
                    TreeNode childNode = dc3.MountTree(aSample, targetAttribute,
                        (Attribute[])aAttributes.ToArray(typeof(Attribute)));
                    root.AddChild(childNode, value);
                }
            }

            return root;
        }

        #endregion
    }
}
