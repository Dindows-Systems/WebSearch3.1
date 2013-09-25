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
using DataCenter.Net.Data;

namespace SJTU.CS.Apex.WebSearch.BLL.QueryTypeEngines
{
    /// <summary>
    /// Summary description for nRSEngine
    /// </summary>
    public class NRSEngine : IQueryTypeEngine
    {
        #region Constructors

        public NRSEngine() : base()
        {
            this._resultType = RetrievalType.BySession;
        }

        public NRSEngine(QueryLog queryLog)
            : base(queryLog)
        {
            this._resultType = RetrievalType.BySession;
        }

        public NRSEngine(DataTable queryLogResultTable) : base(queryLogResultTable)
        {
            this._resultType = RetrievalType.BySession;
        }

        #endregion

        #region Useful Calculations (can only be called inside GetQueryType function)

        protected int _nResults = Const.Invalid;

        /// <summary>
        /// 
        /// </summary>
        protected int NResults
        {
            get
            {
                if (_nResults == Const.Invalid)
                {
                    // read the nResults in Setting.xml
                    DataSet ds = new DataSet();
                    ds.ReadXml(HttpContext.Current.Server.MapPath(Config.
                        QueryTypeEnginesLayerPath + "nRS/Setting.xml"));
                    _nResults = int.Parse(ds.Tables[0].Rows[0]["nResults"].ToString());
                    ds.Clear();
                }
                return _nResults;
            }
        }

        protected int _nRSSessionNum = Const.Invalid;

        /// <summary>
        /// #(Session of q that involves clicks only on top n results)
        /// </summary>
        protected int NRSSessionNum
        {
            get
            {
                if (_nRSSessionNum == Const.Invalid)
                {
                    // first, calculate the count of sessions having click num bigger than n
                    int rest = int.Parse(this._queryLogResultTable.Compute(
                        "COUNT(" + QueryLogFields.User + ")",
                        QueryLogFields.Rank + ">=" + NResults.ToString()).ToString());
                    // second, result = total session number - unsatisfied session number
                    _nRSSessionNum = this.SessionNum - rest;
                }
                return _nRSSessionNum;
            }
        }

        #endregion

        public override float Feature
        {
            get
            {
                if (_feature == Const.Unknown)
                {
                    this.PrepareQueryLogResultTable(RetrievalType.BySession);

                    _feature = (float)NRSSessionNum / (float)(SessionNum);
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
                if (NRSEngine._valve == Const.Unknown)
                {
                    // read the valve value from xml
                    _valve = float.Parse(XmlHelper.ReadNode(HttpContext.Current.
                        Server.MapPath(Config.QueryTypeEnginesLayerPath +
                        "nRS/Setting.xml"), null, "Valve"));
                }
                return NRSEngine._valve;
            }
            set
            {
                NRSEngine._valve = value;

                // write the new valve value into xml
                XmlHelper.ModifyNode(HttpContext.Current.Server.MapPath(
                    Config.QueryTypeEnginesLayerPath + "nRS/Setting.xml"),
                    null, "Valve", value.ToString());
            }
        }

        public override QueryType GetQueryType()
        {
            // must call PrepareQueryLogResultTable first!
            this.PrepareQueryLogResultTable(RetrievalType.BySession);

            if (this.SessionNum == 0)
                return QueryTypes.Unknown;

            if (this.Feature > NRSEngine.Valve)
                return QueryTypes.Navigatinoal;
            else
                return QueryTypes.Informational;
        }
    }
}