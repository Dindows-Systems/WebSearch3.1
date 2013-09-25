using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SJTU.CS.Apex.WebSearch.Model;
using System.Xml;
using SJTU.CS.Apex.WebSearch.Util;
using Model.Net.Utility;
using Model.Net;
using DataRetriever.Net.DataSource;

namespace SJTU.CS.Apex.WebSearch.BLL.QueryTypeEngines
{
    /// <summary>
    /// Summary description for nCSEngine
    /// </summary>
    public class NCSEngine : IQueryTypeEngine
    {
        #region Constructors

        public NCSEngine() : base()
        {
            this._resultType = RetrievalType.BySession;
        }

        public NCSEngine(QueryLog queryLog)
            : base(queryLog)
        {
            this._resultType = RetrievalType.BySession;
        }

        public NCSEngine(DataTable queryLogResultTable) : base(queryLogResultTable)
        {
            this._resultType = RetrievalType.BySession;
        }

        #endregion

        #region Useful Calculations (can only be called inside GetQueryType function)

        protected int _nClicks = Const.Invalid;

        /// <summary>
        /// 
        /// </summary>
        protected int NClicks
        {
            get
            {
                if (_nClicks == Const.Invalid)
                {
                    // read the nClicks in Setting.xml
                    DataSet ds = new DataSet();
                    ds.ReadXml(HttpContext.Current.Server.MapPath(Config.
                        QueryTypeEnginesLayerPath + "nCS/Setting.xml"));
                    _nClicks = int.Parse(ds.Tables[0].Rows[0]["nClicks"].ToString());
                    ds.Clear();
                }
                return _nClicks;
            }
        }

        protected int _nCSSessionNum = Const.Invalid;

        /// <summary>
        /// #(Session of q that involves less than n clicks)
        /// </summary>
        protected int NCSSessionNum
        {
            get
            {
                if (_nCSSessionNum == Const.Invalid)
                {
                    // first, calculate the count of sessions having click num bigger than n
                    int rest = int.Parse(this._queryLogResultTable.Compute(
                        "COUNT(" + QueryLogFields.User + ")",
                        QueryLogFields.Order + ">=" + NClicks.ToString()).ToString());
                    // second, result = total session number - unsatisfied session number
                    _nCSSessionNum = this.SessionNum - rest;
                }
                return _nCSSessionNum;
            }
        }

        #endregion

        public override float Feature
        {
            get
            {
                if (_feature == Const.Unknown)
                {
                    PrepareQueryLogResultTable(RetrievalType.BySession);

                    this._feature = (float)NCSSessionNum / (float)(SessionNum);
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
                if (NCSEngine._valve == Const.Unknown)
                {
                    // read the valve value from xml
                    _valve = float.Parse(XmlHelper.ReadNode(HttpContext.Current.
                        Server.MapPath(Config.QueryTypeEnginesLayerPath +
                        "nCS/Setting.xml"), null, "Valve"));
                }
                return NCSEngine._valve;
            }
            set
            {
                NCSEngine._valve = value;

                // write the new valve value into xml
                XmlHelper.ModifyNode(HttpContext.Current.Server.MapPath(
                    Config.QueryTypeEnginesLayerPath + "nCS/Setting.xml"),
                    null, "Valve", value.ToString());
            }
        }

        public override QueryType GetQueryType()
        {
            PrepareQueryLogResultTable(RetrievalType.BySession);

            if (this.SessionNum == 0)
                return QueryTypes.Unknown;

            if (this.Feature > NCSEngine.Valve)
                return QueryTypes.Navigatinoal;
            else
                return QueryTypes.Informational;
        }
    }
}