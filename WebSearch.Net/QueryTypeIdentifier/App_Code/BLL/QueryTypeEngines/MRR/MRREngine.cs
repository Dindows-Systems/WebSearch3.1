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
using Model.Net.Utility;
using Model.Net;

namespace SJTU.CS.Apex.WebSearch.BLL.QueryTypeEngines
{
    /// <summary>
    /// Summary description for MRREngine
    /// </summary>
    public class MRREngine : IQueryTypeEngine
    {
        #region Constructors

        public MRREngine() : base()
        {
            this._resultType = RetrievalType.ByOrder;
        }

        public MRREngine(QueryLog queryLog)
            : base(queryLog)
        {
            this._resultType = RetrievalType.ByOrder;
        }

        public MRREngine(DataTable queryLogResultTable)
            : base(queryLogResultTable)
        {
            this._resultType = RetrievalType.ByOrder;
        }

        #endregion

        public override float Feature
        {
            get
            {
                if (_feature == Const.Unknown)
                {
                    PrepareQueryLogResultTable(RetrievalType.ByOrder);

                    int total = this._queryLogResultTable.Rows.Count;
                    float sigma = 0;
                    foreach (DataRow row in _queryLogResultTable.Rows)
                        sigma += 1F / float.Parse(row[QueryLogFields.Rank].ToString());
                    this._feature = sigma / (float)total;
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
                if (MRREngine._valve == Const.Unknown)
                {
                    // read the valve value from xml
                    _valve = float.Parse(XmlHelper.ReadNode(HttpContext.Current.
                        Server.MapPath(Config.QueryTypeEnginesLayerPath +
                        "MRR/Setting.xml"), null, "Valve"));
                }
                return MRREngine._valve;
            }
            set
            {
                MRREngine._valve = value;

                // write the new valve value into xml
                XmlHelper.ModifyNode(HttpContext.Current.Server.MapPath(
                    Config.QueryTypeEnginesLayerPath + "MRR/Setting.xml"),
                    null, "Valve", value.ToString());
            }
        }

        public override QueryType GetQueryType()
        {
            PrepareQueryLogResultTable(RetrievalType.ByOrder);

            if (this.Feature > MRREngine.Valve)
                return QueryTypes.Navigatinoal;
            else
                return QueryTypes.Informational;
        }
    }
}