using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections;
using System.Xml;
using System.Threading;
using System.Diagnostics;
using WebSearch.Model.Net;
using WebSearch.Common.Net;

namespace WebSearch.Crawler.Net
{
    public class WebCrawler
    {
        public enum FilterType
        {
            None,       // leaving the original html
            Normal,     // filter unecessary tags
            PlainText   // leaving plain text only
        }

        #region Public Events

        public delegate void __NewPageCallback(WebPage page);

        private __NewPageCallback _newPageFoundEvent = null;

        public __NewPageCallback NewPageFound
        {
            set { _newPageFoundEvent = value; }
        }

        public delegate void __WebCrawlerCallback();

        private __WebCrawlerCallback _webCrawlerFinishEvent = null;

        public __WebCrawlerCallback WebCrawlerFinish
        {
            set { _webCrawlerFinishEvent = value; }
        }

        #endregion

        #region Crawler's Timer

        private Timer _timer1 = null;

        private int _interval = 100;

        private void On_Timer(object state)
        {
            if (this._urlQueue.Count != 0)
                this.Crawl(_urlQueue.Dequeue());
            else
                this.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        public void SetTimer(int interval)
        {
            interval = (interval < 100) ? 100 : interval;
            if (interval != this._interval)
            {
                this._interval = interval;
                this._timer1.Change(0, interval);
            }
        }

        #endregion

        #region Public Properties

        private bool _needSpread = true;

        public bool NeedSpread
        {
            get { return _needSpread; }
            set { _needSpread = value; }
        }

        private bool _dontLeaveSite = false;

        /// <summary>
        /// Don't leave the seed site
        /// </summary>
        public bool DontLeaveSite
        {
            get { return _dontLeaveSite; }
            set { _dontLeaveSite = value; }
        }

        #region Web Request Proxy Setting

        private bool _useProxy = false;

        public bool UseProxy
        {
            get { return _useProxy; }
            set { _useProxy = value; }
        }

        private WebProxy _proxy = new WebProxy();

        public WebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        public void EnableProxy(WebProxy proxy)
        {
            this._useProxy = true;
            this._proxy = proxy;
        }

        #endregion

        private int _startPageID = 0;

        /// <summary>
        /// 
        /// </summary>
        public int StartPageID
        {
            get { return _startPageID; }
            set
            {
                _startPageID = value;
                WebPage.CurrentID = _startPageID;
            }
        }

        private int _maxCrawlPageNum = -1;

        /// <summary>
        /// Max crawling page num
        /// </summary>
        public int MaxCrawlPageNum
        {
            get { return _maxCrawlPageNum; }
            set { _maxCrawlPageNum = value; }
        }

        private Hyperlink _startUrl;

        public Hyperlink StartUrl
        {
            get { return _startUrl; }
            set { _startUrl = value; }
        }

        private string _crawledUrlsDirectory;

        /// <summary>
        /// 
        /// </summary>
        public string CrawledUrlsDirectory
        {
            get { return _crawledUrlsDirectory; }
            set { _crawledUrlsDirectory = value; }
        }

        private FilterType _filterType = FilterType.Normal;

        /// <summary>
        /// Filter type
        /// </summary>
        public FilterType Filter
        {
            get { return _filterType; }
            set { _filterType = value; }
        }

        #endregion

        #region Data Structures

        private Queue<Hyperlink> _urlQueue = new Queue<Hyperlink>(5000);

        private const int MaxQueueSize = 5000;

        #endregion

        #region Constructors

        public WebCrawler(WebProxy proxy)
        {
            this._useProxy = true;
            this._proxy = proxy;
        }

        public WebCrawler()
        {
        }

        public WebCrawler(string urlDir)
        {
            // check whether the directory exists
            if (!Directory.Exists(urlDir))
                Directory.CreateDirectory(urlDir);
            this._crawledUrlsDirectory = urlDir;
            // prepare the dtd for the xmls
            if (!File.Exists(urlDir + "PageSchema.dtd"))
                File.Copy(@".\Templates\PageSchema.dtd", 
                    urlDir + "PageSchema.dtd");
        }

        public WebCrawler(string url, string urlDir) : this(urlDir)
        {
            // the seed url to crawl
            this._startUrl = new Hyperlink(url);
            if (!_startUrl.IsValid && !_startUrl.IsPage)
                throw new Exception("Invalid Seed Url");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starting the crawling
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            bool firstPage = this.Crawl(_startUrl);
            if (!firstPage)
                return false;

            this._timer1 = new Timer(new TimerCallback(On_Timer));
            this._timer1.Change(0, _interval);
            
            return true;
        }

        /// <summary>
        /// Resume the crawling
        /// </summary>
        /// <returns></returns>
        public bool Resume()
        {
            this._timer1 = new Timer(new TimerCallback(On_Timer));
            return this._timer1.Change(0, _interval);
        }

        /// <summary>
        /// Pause the crawling
        /// </summary>
        /// <returns></returns>
        public bool Pause()
        {
            this._timer1.Dispose();
            return true;
        }

        /// <summary>
        /// Stop the crawling
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if (this._timer1 != null)
                this._timer1.Dispose();

            if (this._webCrawlerFinishEvent != null)
                _webCrawlerFinishEvent();

            return true;
        }

        public void AddUrl(string url)
        {
            this._urlQueue.Enqueue(new Hyperlink(url));
        }

        #endregion

        #region Private Methods

        private bool Crawl(Hyperlink link)
        {
            try
            {
                // 1. check whether the url has been crawled:
                string fn = _crawledUrlsDirectory + link.URI.Host + ".xml";
                string url = link.URI.AbsoluteUri;

                string urlPath = Hyperlink.GetAbsolutePath(url);
                if (urlPath == null || urlPath == "")
                    return false;

                // if file exists, check the node
                if (File.Exists(fn) && XmlHelper.ReadNode(fn, url) != null)
                    return false;
                
                // 2. get the web page object
                WebPage page = Retrieve(link.URI);
                if (page == null) return false;

                // 3. parse the html
                /// page.ParseHtml();

                // 4. mark the url's been crawled
                #region Get Page's Attributes
                Hashtable attributes = new Hashtable();
                attributes.Add("_Url", url);
                attributes.Add("_ID", page.ID);
                // calculate other attributes for the page
                // 4.1 get page's out degree (link number)
                //attributes.Add("_OutDegree", page.OutDegree);
                // 4.2 get page's image number
                //attributes.Add("_ImgNum", page.ImageNum);
                // 4.3 get page's plain word number
                //attributes.Add("_WordNum", page.WordNum);
                // 4.4 get page's title
                attributes.Add("_Title", page.Title);
                // 4.5 get page's content type
                attributes.Add("_ContentType", page.ContentType);
                // 4.6 get page's encoding
                attributes.Add("_Encoding", page.Encoding.HeaderName);
                // 4.7 get page's in-site link number
                //attributes.Add("_InSiteLnk", page.InSiteLnkNum);
                // 4.7 get page's size 
                //attributes.Add("_Size", page.Size);
                // 4.8 add the in_degree
                //if (link.CiteUrl != "")
                //    attributes.Add("_InDegree", 1);
                //else
                //    attributes.Add("_InDegree", 0);
                #endregion

                // 5. add the decendent anchors into queue
                if (this._needSpread)
                    this.HandleDecendentAnchors(page.Links);

                if (!File.Exists(fn))
                    // create the xml for the host
                    File.Copy(@".\Templates\template1.xml", fn);

                XmlHelper.AddNode(fn, "Site", "Page", attributes);
                if (link.CiteUrl != "")   // add the in node
                    XmlHelper.AddNode(fn, url, "In", link.CiteUrl);

                // 6. inform that the url's been crawled
                if (this._newPageFoundEvent != null)
                    _newPageFoundEvent(page);

                if (page.ID >= StartPageID + MaxCrawlPageNum)
                    this.Stop();
                return true;
            }
            catch (WebException webex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void HandleDecendentAnchors(List<Hyperlink> links)
        {
            ArrayList distinctUrls = new ArrayList();
            foreach (Hyperlink link in links)
            {
                // check if the url is valid
                if (link.IsValid && link.IsPage)
                {
                    if (_dontLeaveSite && !link.IsFromSameSite()) continue;
                    if (link.IsSameUrl()) continue;

                    if (distinctUrls.Contains(link.URI.AbsoluteUri))
                        continue;
                    else
                        distinctUrls.Add(link.URI.AbsoluteUri);

                    // check whether the url already crawled
                    string fn = _crawledUrlsDirectory + link.URI.Host + ".xml";

                    // update its indegree if possible
                    if (File.Exists(fn) && XmlHelper.IncreaseNode(fn, 
                        link.URI.AbsoluteUri, "_InDegree"))
                        // add the in node 
                        XmlHelper.AddNode(fn, link.URI.AbsoluteUri,
                            "In", link.CiteUrl);
                    else 
                        // add the uri into the queue
                        _urlQueue.Enqueue(link);
                }
            }
        }

        #endregion

        #region Utilities

        public HttpWebResponse GetWebResponse(Uri url)
        {
            // handling the web request and web response
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url.AbsoluteUri);
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.01; Windows NT 5.0)";
            if (this._useProxy) request.Proxy = this._proxy; // set the proxy
            request.AllowAutoRedirect = true;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response == null || response.StatusCode != HttpStatusCode.OK)
                return null;
            return response;
        }

        /// <summary>
        /// Retrieve the html for the given url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public WebPage Retrieve(String url)
        {
            return Retrieve(url, _filterType);
        }

        /// <summary>
        /// Retrieve the html for the given url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public WebPage Retrieve(Uri url)
        {
            return Retrieve(url, _filterType);
        }

        /// <summary>
        /// Retrieve the html for the given url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public WebPage Retrieve(String url, FilterType filterType)
        {
            Uri uri = null;
            if (Uri.TryCreate(url.Trim(), UriKind.Absolute,
                out uri) && !uri.IsFile)
                return Retrieve(uri, filterType);
            return null; // invalid url or is file
        }

        /// <summary>
        /// Retrieve the html for the given uri
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public WebPage Retrieve(Uri url, FilterType filterType)
        {
            Hyperlink link = new Hyperlink(url.AbsoluteUri);
            if (!(link.IsValid && link.IsPage)) return null;
            try
            {
                HttpWebResponse response = GetWebResponse(url);
                if (response == null) return null;

                WebPage page = new WebPage(); // the result web page object
                // prepare the web page object
                page.ContentType = response.ContentType;
                page.Url = url.AbsoluteUri;
                // get the page's encoding and html content in string
                string content = null;
                page.Encoding = EncodingHelper.GetEncoding(response, out content);
                page.Html = content;

                switch (filterType) // filter type
                {
                    case FilterType.Normal:
                        page.Html = HtmlParser.Filter(page.Html, false);
                        break;
                    case FilterType.PlainText:
                        page.Html = HtmlParser.Filter(page.Html, true);
                        break;
                }
                response.Close();
                return page;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        #endregion
    }
}
