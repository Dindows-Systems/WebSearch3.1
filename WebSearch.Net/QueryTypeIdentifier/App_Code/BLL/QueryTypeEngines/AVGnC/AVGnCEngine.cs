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
using DataRetriever.Net.DataSource;

namespace SJTU.CS.Apex.WebSearch.BLL.QueryTypeEngines
{
    /// <summary>
    /// Summary description for AVGnC
    /// </summary>
    public class AVGnCEngine : IQueryTypeEngine
    {
        #region Constructors

        public AVGnCEngine() : base()
        {
            this._resultType = RetrievalType.BySession;
        }

        public AVGnCEngine(QueryLog queryLog)
            : base(queryLog)
        {
            this._resultType = RetrievalType.BySession;
        }

        public AVGnCEngine(DataTable queryLogResultTable)
            : base(queryLogResultTable)
        {
            this._resultType = RetrievalType.BySession;
        }

        #endregion

        public override QueryType GetQueryType()
        {
            // must call PrepareQueryLogResultTable first!
            this.PrepareQueryLogResultTable(RetrievalType.BySession);

            if (this.SessionNum == 0)
                return QueryTypes.Unknown;

            if (this.Feature <= AVGnCEngine.Valve)
                return QueryTypes.Navigatinoal;
            else
                return QueryTypes.Informational;
        }

        public override float Feature
        {
            get
            {
                if (_feature == Const.Unknown)
                {
                    this.PrepareQueryLogResultTable(RetrievalType.BySession);

                    // calculate the total click number in the result set
                    _feature = float.Parse(this._queryLogResultTable.Compute(
                        "AVG(" + QueryLogFields.Order + ")", "").ToString());
                }
                return _feature;
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
                if (AVGnCEngine._valve == Const.Unknown)
                {
                    // read the valve value from xml
                    _valve = float.Parse(XmlHelper.ReadNode(HttpContext.Current.
                        Server.MapPath(Config.QueryTypeEnginesLayerPath +
                        "AVGnC/Setting.xml"), null, "Valve"));
                }
                return AVGnCEngine._valve;
            }
            set
            {
                AVGnCEngine._valve = value;

                // write the new valve value into xml
                XmlHelper.ModifyNode(HttpContext.Current.Server.MapPath(
                    Config.QueryTypeEnginesLayerPath + "AVGnC/Setting.xml"),
                    null, "Valve", value.ToString());
            }
        }
    }
}