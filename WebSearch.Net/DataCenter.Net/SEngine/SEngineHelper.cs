using System;
using System.Data;
using System.Configuration;
using System.Xml;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using WebSearch.Model.Net;
using WebSearch.Crawler.Net;

namespace WebSearch.DataCenter.Net.SEngine
{
    /// <summary>
    /// For multi-search engine system
    /// Data resource can only be search engines
    /// </summary>
    public class SEngineHelper
    {
        #region Search Engine Crawler

        /// <summary>web crawler for search engine</summary>
        protected WebCrawler _sEngineCrawler = null;

        public WebCrawler SEngineCrawler
        {
            get { return _sEngineCrawler; }
        }

        #endregion

        #region Constructors

        public SEngineHelper()
        {
            // init the crawler, using the web proxy
            _sEngineCrawler = new WebCrawler(WebProxies.Get("SJTU"));
            // not to filter any html tag
            _sEngineCrawler.Filter = WebCrawler.FilterType.None;
        }

        #endregion
    }
}