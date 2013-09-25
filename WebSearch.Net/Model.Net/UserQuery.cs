using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Maths.Net;
using WebSearch.Common.Net;

namespace WebSearch.Model.Net
{
    public class UserQuery : BaseModel
    {
        #region Properties

        private string _value;

        /// <summary>
        /// Query Text
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

        #region Constructors

        public UserQuery()
        {
        }

        public UserQuery(string query)
        {
            this._value = query;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return this._value;
        }

        public override bool Equals(object obj)
        {
            if (obj is UserQuery)
                return (this._value == ((UserQuery)obj).Value);
            return false;
        }

        #endregion
    }

    #region class: Query Type

    /// <summary>
    /// Query Type
    /// </summary>
    public class QueryType
    {
        #region Instances

        public static QueryType Informational
        {
            get { return new QueryType(1); }
        }

        public static QueryType GetInformational(float accuracy)
        {
            return new QueryType(1, new Degree(accuracy));
        }

        public static QueryType GetInformational(Degree accuracy)
        {
            return new QueryType(1, accuracy);
        }

        public static QueryType Navigatinoal
        {
            get { return new QueryType(2); }
        }

        public static QueryType GetNavigatinoal(float accuracy)
        {
            return new QueryType(2, new Degree(accuracy));
        }

        public static QueryType GetNavigatinoal(Degree accuracy)
        {
            return new QueryType(2, accuracy);
        }

        public static QueryType Transactional
        {
            get { return new QueryType(3); }
        }

        public static QueryType GetTransactional(float accuracy)
        {
            return new QueryType(3, new Degree(accuracy));
        }

        public static QueryType GetTransactional(Degree accuracy)
        {
            return new QueryType(3, accuracy);
        }

        public static QueryType Unknown
        {
            get { return new QueryType(Const.Unknown); }
        }

        #endregion

        #region Constructors

        internal QueryType(int id)
        {
            switch (id)
            {
                case 1:
                    _id = 1;
                    _name = "Info.";
                    break;
                case 2:
                    _id = 2;
                    _name = "Navi.";
                    break;
                case 3:
                    _id = 3;
                    _name = "Tran.";
                    break;
                default:
                    _id = Const.Unknown;
                    _name = "N/A";
                    break;
            }
        }

        internal QueryType(int id, Degree accuracy)
            : this(id)
        {
            this._accuracy = accuracy;
        }

        #endregion

        #region Properties

        private int _id;

        /// <summary>
        /// the id of the query type
        /// </summary>
        public int ID
        {
            get { return _id; }
        }

        private string _name;

        /// <summary>
        /// the name of the query type
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        private Degree _accuracy = Degree.Good;

        /// <summary>
        /// Accuracy of the type
        /// </summary>
        public Degree Accuracy
        {
            get { return _accuracy; }
            set { _accuracy = value; }
        }

        #endregion

        #region Overrided Operators

        public static bool operator ==(QueryType type1, QueryType type2)
        {
            return (type1.ID == type2.ID);
        }

        public static bool operator !=(QueryType type1, QueryType type2)
        {
            return (type1.ID != type2.ID);
        }

        public static explicit operator QueryType(int id)
        {
            switch (id)
            {
                case 1:
                    return QueryType.Informational;
                case 2:
                    return QueryType.Navigatinoal;
                case 3:
                    return QueryType.Transactional;
                default:
                    return QueryType.Unknown;
            }
        }

        public override string ToString()
        {
            return this._name;
        }

        public override bool Equals(object obj)
        {
            if (obj is QueryType)
                return (this._id == ((QueryType)obj)._id);

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }

    #endregion
}
