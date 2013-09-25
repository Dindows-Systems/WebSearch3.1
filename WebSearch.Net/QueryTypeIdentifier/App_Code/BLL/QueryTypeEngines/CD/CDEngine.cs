using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SJTU.CS.Apex.WebSearch.Util;
using Model.Net;

namespace SJTU.CS.Apex.WebSearch.BLL.QueryTypeEngines
{
    /// <summary>
    /// Summary description for CDEngine
    /// </summary>
    public class CDEngine : IQueryTypeEngine
    {
        #region Constructors

        public CDEngine() : base()
        {
            this._resultType = RetrievalType.Normal;
        }

        public CDEngine(QueryLog db)
            : base(db)
        {
            this._resultType = RetrievalType.Normal;
        }

        public CDEngine(DataTable queryLogResultTable)
            : base(queryLogResultTable)
        {
            this._resultType = RetrievalType.Normal;
        }

        public CDEngine(DataTable queryLogResultTable, StatMethod statMethod)
            : base(queryLogResultTable, statMethod)
        {
            this._resultType = RetrievalType.Normal;
        }

        #endregion

        /// <summary>
        /// i(q)
        /// according to the current statical method
        /// </summary>
        public override float Feature
        {
            get
            {
                if (this._feature == Const.Unknown)
                {
                    this.PrepareQueryLogResultTable(RetrievalType.Normal);

                    // calculate the histogram of the rank and click 
                    float[] histogram = new float[10];
                    int rank = 0;
                    foreach (DataRow row in this._queryLogResultTable.Rows)
                    {
                        // the index of the array is the rank of the result
                        //  (FROM histogram[0])
                        // the value of the array is the # of click times
                        rank = int.Parse(row[QueryLogFields.Rank].ToString()) - 1;
                        if (rank < 10)
                            histogram[rank]++;
                    }

                    // normalize the histogram
                    float totalClick = 0;
                    for (int i = 0; i < 10; i++)
                        totalClick += histogram[i];
                    for (int i = 0; i < 10; i++)
                        histogram[i] /= totalClick;

                    // calculate the feature according to the statistical method
                    switch (_statMethod)
                    {
                        case StatMethod.Median:
                            _feature = Statistics.Median(histogram);
                            break;
                        case StatMethod.Mean:
                            _feature = Statistics.Mean(histogram);
                            break;
                        case StatMethod.Skewness:
                            _feature = Statistics.StdDev(histogram);
                            break;
                        case StatMethod.Kurtosis:
                            _feature = Statistics.StdDev(histogram);
                            break;
                        default:
                            throw new Exception("Unknow Statistical Method");
                    }
                    _feature *= 10;
                }
                return this._feature;
            }
            set { this._feature = value; }
        }

        protected static float _valve = Const.Unknown;

        /// <summary>
        /// The valve of the feature for judgement. It should be trained in tests
        /// </summary>
        public static float Valve
        {
            get
            {
                if (CDEngine._valve == Const.Unknown)
                {
                    // read the valve value from xml
                    _valve = float.Parse(XmlHelper.ReadNode(HttpContext.Current.
                        Server.MapPath(Config.QueryTypeEnginesLayerPath +
                        "CD/Setting.xml"), null, "Valve"));
                }
                return _valve;
            }
            set
            {
                CDEngine._valve = value;

                // write the new valve value into xml
                XmlHelper.ModifyNode(HttpContext.Current.Server.MapPath(
                    Config.QueryTypeEnginesLayerPath + "CD/Setting.xml"),
                    null, "Valve", value.ToString());
            }
        }

        public override StatMethod StatisticalMethod
        {
            get { return this._statMethod; }
            set
            {
                this._statMethod = value;
                // since the statical method is changed, 
                // we need to recalculate the feature value
                this._feature = Const.Unknown;
                CDEngine.Valve = Const.Unknown;
            }
        }

        public override QueryType GetQueryType()
        {
            PrepareQueryLogResultTable(RetrievalType.Normal);

            if (this.Feature < CDEngine.Valve)
                return QueryTypes.Navigatinoal;
            else
                return QueryTypes.Informational;
        }
    }
}