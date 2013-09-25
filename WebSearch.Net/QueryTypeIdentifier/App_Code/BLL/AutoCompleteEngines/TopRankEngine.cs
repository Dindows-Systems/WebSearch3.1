using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using DataRetriever.Net;
using SJTU.CS.Apex.WebSearch.Util;

namespace SJTU.CS.Apex.WebSearch.BLL.AutoCompleteEngines
{
    /// <summary>
    /// Summary description for RankEngine
    /// </summary>
    public class TopRankEngine : IAutoCompleteEngine
    {
        #region Constructors

        public TopRankEngine()
        {
        }

        #endregion

        public override string[] GetCompleteQueries(int count)
        {
            // count is default to be 4
            count = (count == 0) ? 4 : count;

            IQueryTextDAO dao = DALHelper.GetQueryTextDAO();
            return dao.GetCompleteQueryTexts(_prefixText, count).ToArray();
        }
    }
}
