using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace SJTU.CS.Apex.WebSearch.BLL.AutoCompleteEngines
{
    /// <summary>
    /// Summary description for IAutoCompleteEngine
    /// </summary>
    public abstract class IAutoCompleteEngine
    {
        protected String _prefixText;

        #region Constructors

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the possible complete queries according to the given query prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public String[] GetCompleteQueries(String prefix, int count)
        {
            this._prefixText = prefix;
            return GetCompleteQueries(count);
        }

        /// <summary>
        /// Get the possible complete queries according to the given query prefix
        /// </summary>
        /// <returns></returns>
        public abstract String[] GetCompleteQueries(int count);

        #endregion

        #region Public Properties

        /// <summary>
        /// Prefix text of the query.
        /// </summary>
        public String PrefixText
        {
            get { return this._prefixText; }
            set { this._prefixText = value; }
        }

        #endregion
    }
}