using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Common.Net;
using WebSearch.Model.Net;

namespace WebSearch.Feature.Net
{
    public class WebSiteFeature
    {
        #region Constructors

        protected WebSite _website = null;

        public static WebSiteFeature Get(WebSite site)
        {
            return new WebSiteFeature(site);
        }

        protected WebSiteFeature(WebSite site)
        {
            this._website = site;
        }

        #endregion

        #region Web Site Features

        protected void InitializeWebSiteFeatures()
        {
        }

        #endregion
    }
}
