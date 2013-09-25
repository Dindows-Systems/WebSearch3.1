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
    /// Summary description for DTreeEngine1
    /// </summary>
    public class DTreeEngine1 : IQueryTypeEngine
    {
        #region Constructors

        public DTreeEngine1() : base()
        {
        }

        public DTreeEngine1(DataTable queryLogResultTable)
            : base(queryLogResultTable)
        {
        }

        #endregion

        public override QueryType GetQueryType()
        {
            // must call PrepareQueryLogResultTable first!
            this.PrepareQueryLogResultTable(RetrievalType.BySession);

            if (this.SessionNum == 0)
                return QueryTypes.Unknown;

            IQueryTypeEngine nRSEngine = new NRSEngine(_queryLogResultTable);
            nRSEngine.SessionNum = this.SessionNum;
            if (nRSEngine.GetQueryType(_queryText) == QueryTypes.Navigatinoal)
                return QueryTypes.Navigatinoal;

            IQueryTypeEngine nCSEngine = new NCSEngine(_queryLogResultTable);
            nCSEngine.SessionNum = this.SessionNum;
            if (nCSEngine.GetQueryType(_queryText) == QueryTypes.Navigatinoal)
                return QueryTypes.Navigatinoal;

            IQueryTypeEngine avgnCEngine = new AVGnCEngine();
            if (avgnCEngine.GetQueryType(_queryText) == QueryTypes.Navigatinoal)
                return QueryTypes.Navigatinoal;

            return QueryTypes.Informational;
        }
    }
}
