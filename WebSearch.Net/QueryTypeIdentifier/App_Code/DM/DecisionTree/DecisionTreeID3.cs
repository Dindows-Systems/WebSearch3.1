using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;

namespace SJTU.CS.Apex.WebSearch.DM.DecisionTree
{
    /// <summary>
    /// Summary description for DecisionTreeID3
    /// </summary>
    public class DecisionTreeID3
    {
        protected DataTable _samples;

        protected int _totalPositives = 0;

        protected int _total = 0;

        protected string _targetAttribute = "result";

        protected double _entropySet = 0.0;

        public DecisionTreeID3()
        {
        }

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        protected virtual int CountTotalPositives(DataTable samples)
        {
            int result = 0;
            foreach (DataRow row in samples.Rows)
                if ((bool)row[_targetAttribute])
                    result++;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="positives"></param>
        /// <param name="negatives"></param>
        /// <returns></returns>
        protected virtual double GetEntropy(int positives, int negatives)
        {
            int total = positives + negatives;
            double ratioPositive = (double)positives / total;
            double ratioNegative = (double)negatives / total;

            if (ratioPositive != 0)
                ratioPositive = -(ratioPositive) * System.Math.Log(ratioPositive, 2);
            if (ratioNegative != 0)
                ratioNegative = -(ratioNegative) * System.Math.Log(ratioNegative, 2);

            return ratioPositive + ratioNegative;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        protected virtual double GetGain(DataTable samples, Attribute attribute)
        {
            string[] values = attribute.Values;
            double sum = 0.0;

            for (int i = 0, positives, negatives; i < values.Length; i++)
            {
                positives = negatives = 0; // reset positives and negatives

                GetValuesToAttribute(samples, attribute, values[i],
                    out positives, out negatives);

                double entropy = GetEntropy(positives, negatives);
                sum += -(double)(positives + negatives) / _total * entropy;
            }
            return _entropySet + sum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <param name="positives"></param>
        /// <param name="negatives"></param>
        protected virtual void GetValuesToAttribute(DataTable samples, Attribute attribute, 
            string value, out int positives, out int negatives)
        {
            positives = negatives = 0;

            foreach (DataRow row in samples.Rows)
            {
                if ((row[attribute.Name] == value))
                {
                    if ((bool)row[_targetAttribute])
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
        /// <param name="attributes"></param>
        /// <returns></returns>
        protected virtual Attribute GetBestAttribute(DataTable samples, 
            Attribute[] attributes)
        {
            Attribute result = null;
            double maxGain = 0.0;

            foreach (Attribute attr in attributes)
            {
                double aux = GetGain(samples, attr);
                if (aux > maxGain)
                {
                    maxGain = aux;
                    result = attr;
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
        protected virtual bool AllSamplesPositives(DataTable samples, string targetAttribute)
        {
            foreach (DataRow row in samples.Rows)
                if (!(bool)row[targetAttribute])
                    return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="targetAttribute"></param>
        /// <returns></returns>
        protected virtual bool AllSamplesNegatives(DataTable samples, string targetAttribute)
        {
            foreach (DataRow row in samples.Rows)
                if ((bool)row[targetAttribute])
                    return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="targetAttribute"></param>
        /// <returns></returns>
        protected virtual ArrayList GetDistinctValues(DataTable samples, string targetAttribute)
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
        protected virtual object GetMostCommonValue(DataTable samples, string targetAttribute)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="targetAttribute"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        protected virtual TreeNode InternalMountTree(DataTable samples, string targetAttribute, 
            Attribute[] attributes)
        {
            // if sample is empty, return null
            if (samples.Rows.Count == 0)
                return null;

            // if all targetAttribute value 'result' is true in the sample data set
            if (AllSamplesPositives(samples, targetAttribute) == true)
                return new TreeNode(new Attribute(true));

            // if all targetAttribute value 'result' is false in the sample data set
            if (AllSamplesNegatives(samples, targetAttribute) == true)
                return new TreeNode(new Attribute(false));

            // if no attribute in the sample data set (only targetAttribute is in the samples)
            if (attributes.Length == 0)
                // return the most common target attribute value as result node
                return new TreeNode(new Attribute(GetMostCommonValue(samples, targetAttribute)));

            // normal conditions: -------------------------------------------

            this._total = samples.Rows.Count;       // sample size
            this._targetAttribute = targetAttribute;
            this._totalPositives = CountTotalPositives(samples);
            this._entropySet = GetEntropy(_totalPositives, _total - _totalPositives);

            // get the best attribute according to Gain
            Attribute bestAttribute = GetBestAttribute(samples, attributes);
            // put the best attribute the root of this subtree
            TreeNode root = new TreeNode(bestAttribute);
            DataTable cpsamples = samples.Clone();

            // for each distinct value that is allowed in this attribute
            foreach (string value in bestAttribute.Values)
            {
                // Seleciona todas os elementos com o valor deste atributo				
                cpsamples.Rows.Clear();

                // divide the sample according to the value in bestAttibute
                DataRow[] rows = samples.Select(bestAttribute.Name + "='" + value + "'");
                foreach (DataRow row in rows)
                    cpsamples.Rows.Add(row.ItemArray);
                
                ArrayList aAttributes = new ArrayList(attributes.Length - 1);
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i].Name != bestAttribute.Name)
                        aAttributes.Add(attributes[i]);
                }
                // Cria uma nova lista de atributos menos o atributo corrente que ?o melhor atributo

                if (cpsamples.Rows.Count == 0)
                    return new TreeNode(new Attribute(GetMostCommonValue(
                        cpsamples, targetAttribute)));
                else
                {
                    // recursively contruct the (sub)tree
                    DecisionTreeID3 dc3 = new DecisionTreeID3();
                    TreeNode childNode = dc3.MountTree(cpsamples, targetAttribute,
                        (Attribute[])aAttributes.ToArray(typeof(Attribute)));
                    root.AddTreeNode(childNode, value);
                }
            }
            return root;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="targetAttribute"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public TreeNode MountTree(DataTable samples, string targetAttribute, 
            Attribute[] attributes)
        {
            this._samples = samples;
            // 'attributes' is somehow the column name for _samples
            return InternalMountTree(_samples, targetAttribute, attributes);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TreeNode
    {
        #region Properties

        private ArrayList _children = null;

        /// <summary>
        /// Return the total num of children
        /// </summary>
        public int TotalChildren
        {
            get { return _children.Count; }
        }

        private Attribute _attribute = null;

        /// <summary>
        /// 
        /// </summary>
        public Attribute NodeAttribute
        {
            get { return _attribute; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        public TreeNode(Attribute attribute)
        {
            if (attribute.Values != null)
            {
                _children = new ArrayList(attribute.Values.Length);
                for (int i = 0; i < attribute.Values.Length; i++)
                    _children.Add(null);
            }
            else
            {
                _children = new ArrayList(1);
                _children.Add(null);
            }
            _attribute = attribute;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeNode"></param>
        /// <param name="ValueName"></param>
        public void AddTreeNode(TreeNode treeNode, string valueName)
        {
            int index = _attribute.IndexValue(valueName);
            _children[index] = treeNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TreeNode GetChild(int index)
        {
            return (TreeNode)_children[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns></returns>
        public TreeNode GetChildByBranchName(string branchName)
        {
            int index = _attribute.IndexValue(branchName);
            return (TreeNode)_children[index];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Attribute
    {
        #region Properties

        private ArrayList _values;

        /// <summary>
        /// the values that are allowed for this attribute
        /// </summary>
        public string[] Values
        {
            get
            {
                return (_values != null) ? (string[])_values.
                    ToArray(typeof(string)) : null;
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private object _label;

        public object Label
        {
            get { return _label; }
            set { _label = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        public Attribute(string name, string[] values)
        {
            this._name = name;
            this._values = new ArrayList(values);
            this._values.Sort();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Label"></param>
        public Attribute(object label)
        {
            this._label = label;
            this._name = string.Empty;
            this._values = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsValidValue(string value)
        {
            return IndexValue(value) >= 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexValue(string value)
        {
            if (_values != null)
                return _values.BinarySearch(value);
            else
                return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_name != string.Empty)
                return _name;
            else { return _label.ToString(); }
        }

        #endregion
    }
}
