using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace SJTU.CS.Apex.WebSearch.Util
{
    /// <summary>
    /// Summary description for Util
    /// </summary>
    public static class Utility
    {
        private static Random _rand = new Random();

        public static Random Rand
        {
            get { return _rand; }
        }
    }
}