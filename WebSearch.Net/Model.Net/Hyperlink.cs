using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Xml;
using System.Collections;
using System.Text.RegularExpressions;
using WebSearch.Common.Net;

namespace WebSearch.Model.Net
{
    public enum LinkProtocol
    {
        Http, Https, Ftp
    }

    /// <summary>
    /// Hyperlink Type
    /// </summary>
    public enum HyperlinkType
    {
        UnKnown = 0, Site, Subsite, Media,
        Picture, Text, Application, Html,
        Service, File, Email, Javascript
    };

    /// <summary>
    /// Summary description for HyperlinkType
    /// </summary>
    public class Hyperlink : BaseModel
    {
        /// <summary>
        /// TLD Helper
        /// </summary>
        private class TLDHelper
        {
            #region Properties

            private static ArrayList _gTLDsCache = null;

            public static ArrayList gTLDs
            {
                get
                {
                    if (_gTLDsCache == null)
                    {
                        ArrayList list = new ArrayList(
                            XmlHelper.ReadNode(
                            Config.SettingPath + "TLDs.xml",
                            "/root/TLD/gTLDs",
                            null).Split(dotSpliter));
                        _gTLDsCache = list;
                    }
                    return _gTLDsCache;
                }
            }

            private static ArrayList _ccTLDsCache = null;

            public static ArrayList ccTLDs
            {
                get
                {
                    if (_ccTLDsCache == null)
                    {
                        ArrayList list = new ArrayList(
                            XmlHelper.ReadNode(
                            Config.SettingPath + "TLDs.xml",
                            "/root/TLD/ccTLDs",
                            null).Split(dotSpliter));
                        _ccTLDsCache = list;
                    }
                    return _ccTLDsCache;
                }
            }

            private static ArrayList _subccTLDsCache = null;

            public static ArrayList subccTLDs
            {
                get
                {
                    if (_subccTLDsCache == null)
                    {
                        ArrayList list = new ArrayList(
                            XmlHelper.ReadNode(
                            Config.SettingPath + "TLDs.xml",
                            "/root/TLD/subccTLDs",
                            null).Split(dotSpliter));
                        _subccTLDsCache = list;
                    }
                    return _subccTLDsCache;
                }
            }

            #endregion

            #region Methods

            public static bool gTLDsHave(string tld)
            {
                return gTLDs.Contains(tld);
            }

            public static bool ccTLDsHave(string tld)
            {
                // all the ccTLDs are composed of 2 chars.
                if (tld.Length != 2)
                    return false;
                // using binary search
                return ccTLDs.BinarySearch(tld) >= 0 ? true : false;
            }

            public static bool subccTLDsHave(string tld)
            {
                // all the subccTLDs are composed of 2 chars.
                if (tld.Length != 2)
                    return false;

                return subccTLDs.Contains(tld);
            }

            #endregion
        }

        private static string[] _siteStarts = new string[] {
            "www", "www2" };
        private static string[] _siteEnds = new string[] {
            "/", "/index.html", "/index.htm", "/default.htm", 
            "/default.html", "/default.aspx", "/default.asp" };
        private static string[] _servMids = new string[] {
            "?", "asp", "aspx", "php", "jsp", "pl", "cgi" };
        private static char[] slashSpliter = new char[] { '/' };
        private static char[] dotSpliter = new char[] { '.' };

        #region Regular Expressions

        /// <summary>
        /// ^(https?://)(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?(([0-9]{1,3}\.)
        /// {3}[0-9]{1,3}|([0-9a-z_!~*'()-]+\.)*([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\.[a-z]{2,6})
        /// (:[0-9]{1,4})?((/?)|(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$
        /// </summary>
        public static readonly Regex urlRegex = new Regex("^(https?://)"
            + "(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //user@ 
            + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184 
            + "|" // allows either IP or domain 
            + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www. 
            + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\." // second level domain 
            + "[a-z]{2,6})" // first level domain- .com or .museum 
            + "(:[0-9]{1,4})?" // port number- :80 
            + "((/?)|" // a slash isn't required if there is no file name 
            + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$", RegexOptions.IgnoreCase);

        /// <summary>
        /// (https?://)?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?(([0-9]{1,3}\.)
        /// {3}[0-9]{1,3}|([0-9a-z_!~*'()-]+\.)*([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\.[a-z]{2,6})
        /// (:[0-9]{1,4})?((/?)|(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$
        /// </summary>
        public static readonly Regex urlBlurRegex = new Regex("(https?://)?"
            + "(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //user@ 
            + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184 
            + "|" // allows either IP or domain 
            + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www. 
            + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\." // second level domain 
            + "[a-z]{2,6})" // first level domain- .com or .museum 
            + "(:[0-9]{1,4})?" // port number- :80 
            + "((/?)|" // a slash isn't required if there is no file name 
            + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?).*", RegexOptions.IgnoreCase);

        public static readonly Regex emailRegex = new Regex(
            @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase);

        public static readonly Regex pageRegex = new Regex(
            @"(.*\.((html)|(htm)|(aspx)|(asp)|(php)|(jsp))(\?.*)?)|[^.]*",
            RegexOptions.IgnoreCase);

        public static readonly Regex homeRegex = new Regex(
            @"(/?)((default|index|home)\.(aspx|asp|html|htm|php|jsp))?#?",
            RegexOptions.IgnoreCase);

        public static readonly Regex ipRegex = new Regex(
            @"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b", RegexOptions.IgnoreCase);

        public static readonly Regex validIpRegex = new Regex(
            @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|" +
            @"[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\." + 
            @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b", RegexOptions.IgnoreCase);

        #endregion

        #region Properties

        private string _url;

        /// <summary>
        /// Url of the link (ensured to be lower case in constructor)
        /// </summary>
        public string Url
        {
            get { return _url; }
        }

        /// <summary>
        /// Url Length
        /// </summary>
        public int Length
        {
            get { return _url.Length; }
        }

        private Uri _uri = null;

        public Uri URI
        {
            get { return _uri; }
        }

        private string _citeUrl = "";
        private string _citePath = "";

        /// <summary>
        /// the url that cite this url
        /// </summary>
        public string CiteUrl
        {
            get { return _citeUrl; }
        }

        private bool _isValid = true;

        /// <summary>
        /// 
        /// </summary>
        public bool IsValid
        {
            get { return _isValid; }
        }

        private bool _isEmail = false;

        /// <summary>
        /// Whether the link is in the form of email address
        /// </summary>
        public bool IsEmail
        {
            get { return _isEmail; }
        }

        private bool _isJavascript = false;

        /// <summary>
        /// Whether the link is for a javascript
        /// </summary>
        public bool IsJavascript
        {
            get { return _isJavascript; }
        }

        private bool _isPage = true;

        /// <summary>
        /// Whether the link is for a page
        /// </summary>
        public bool IsPage
        {
            get { return _isPage; }
        }
        
        private LinkProtocol _protocol;

        /// <summary>
        /// The protocol used in this url
        /// </summary>
        public LinkProtocol Protocol
        {
            get { return _protocol; }
        }

        private bool _isSecure = false;

        /// <summary>
        /// Whether the url uses a secure protocol
        /// </summary>
        public bool IsSecure
        {
            get { return _isSecure;}
        }

        private bool _isFtp = false;

        /// <summary>
        /// Whether the url is a ftp link
        /// </summary>
        public bool IsFtp
        {
            get { return _isFtp; }
        }

        private string[] _segments = null;

        /// <summary>
        /// Url Hierarchy segments
        /// </summary>
        public string[] Segments
        {
            get { return _segments; }
        }

        /// <summary>
        /// Count of Url Segments
        /// </summary>
        public int Depth
        {
            get { return Segments.Length; }
        }

        /// <summary>
        /// Domain name of the hyperlink
        /// </summary>
        public string Host
        {
            get { return _segments[0]; }
        }

        #region Host Properties

        private bool _hostIsIP = false;

        /// <summary>
        /// Whether the host is in the form of ip
        /// </summary>
        /// <example>
        /// http://192.168.1.7/test/Default.aspx
        /// </example>
        public bool HostIsIP
        {
            get 
            {
                if (!_analyzeHost)
                    InitializeUrlHost();
                return _hostIsIP; 
            }
        }

        private string _siteLocation = "";

        /// <summary>
        /// The possible location (coountry) of the page
        /// </summary>
        public string SiteLocation
        {
            get 
            {
                if (!_analyzeHost)
                    InitializeUrlHost(); 
                return _siteLocation;
            }
        }

        private string _siteType = "";

        /// <summary>
        /// Site Type: edu, org, travel, etc.
        /// </summary>
        public string SiteType
        {
            get
            {
                if (!_analyzeHost)
                    InitializeUrlHost(); 
                return _siteType;
            }
        }

        /// <summary>
        /// Whether it's a root site (no sub site name)
        /// </summary>
        public bool IsRootSite
        {
            get { return SubSiteName == ""; }
        }

        private string _siteName;

        /// <summary>
        /// The root site name
        /// </summary>
        /// <example>
        /// 'sjtu' in lib.sjtu.edu.cn
        /// </example>
        public string SiteName
        {
            get
            {
                if (!_analyzeHost)
                    InitializeUrlHost(); 
                return _siteName;
            }
        }

        private string _subSiteName = "";

        /// <summary>
        /// The sub site name
        /// </summary>
        /// <example>
        /// 'lib' in lib.sjtu.edu.cn
        /// </example>
        public string SubSiteName
        {
            get
            {
                if (!_analyzeHost)
                    InitializeUrlHost(); 
                return _subSiteName;
            }
        }

        #endregion

        /// <summary>
        /// The file name
        /// </summary>
        /// <example>
        /// 'test.pdf' in http://lib.sjtu.edu.cn/test.pdf
        /// </example>
        public string FileName
        {
            get { return _segments[_segments.Length - 1]; }
        }

        #endregion

        #region Private Members

        private bool _analyzeHost = false;

        /// <summary>
        /// Initialize the Url features
        /// </summary>
        /// <param name="analyzeHost">analyze host or not</param>
        private void InitializeUrl()
        {
            // 1. dealing with the url head
            if (_url.StartsWith("mailto://") ||
                emailRegex.IsMatch(_url))
            {
                this._isPage = false;
                this._isEmail = true;
                return;
            }
            if (_url.StartsWith("javascript://"))
            {
                this._isPage = false;
                this._isJavascript = true;
                return;
            }
            //
            // assert: to be a http page
            // 
            // complete the url:
            if (!_url.StartsWith("http://") &&
                !_url.StartsWith("https://"))
            {
                if (!_citePath.EndsWith("/"))
                    _citePath += '/';
                if (_url.StartsWith("/"))
                    _url.Remove(0, 1);
                _url = _citePath + _url;
            }
            // try to create the uri object
            _isValid = Uri.TryCreate(_url, UriKind.Absolute, out _uri);

            string urlStr = this._url;
            if (urlStr.StartsWith("http://"))
            {
                urlStr = urlStr.Remove(0, 7);
                _protocol = LinkProtocol.Http;
                _isSecure = false;
            }
            if (urlStr.StartsWith("https://"))
            {
                urlStr = urlStr.Remove(0, 8);
                _protocol = LinkProtocol.Https;
                _isSecure = true;
            }
            if (urlStr.StartsWith("ftp://"))
            {
                urlStr = urlStr.Remove(0, 6);
                _protocol = LinkProtocol.Ftp;
                _isFtp = true;
            }
            // whether it's a page
            if (!urlStr.EndsWith("/"))
            {
                // get the file name from the url
                string fn = urlStr.Substring(
                    urlStr.LastIndexOf('/') + 1);

                // whether the url match the page regex
                Match match = pageRegex.Match(fn);
                if (match == null || match.Value != fn)
                    this._isPage = false;
            }
            
            int i = 0;
            // 2. seperate the url body
            for (; i < _siteEnds.Length; i++)
            {
                // a. filter the site ends
                if (urlStr.EndsWith(_siteEnds[i]))
                {
                    urlStr = urlStr.Substring(0, 
                        urlStr.Length - _siteEnds[i].Length);
                    urlStr = urlStr + "/";
                    break;
                }
            }
            // b. split the url
            _segments = urlStr.Split(slashSpliter);

            // 3. analyze the url body's segments
            if (this._analyzeHost)
                this.InitializeUrlHost();
        }

        /// <summary>
        /// Segment the url (must be called after InitializeUrl)
        /// </summary>
        private void InitializeUrlHost()
        {
            this._analyzeHost = true; // the host's been analyzed

            // check whether the host is in the form of ip
            if (ipRegex.IsMatch(this.Host))
            {
                this._hostIsIP = true;
                // check whether it's a valid ip
                if (!validIpRegex.IsMatch(this.Host))
                    this._isValid = false;

                return; // no need to continue
            }
            
            // get the site name and site type from host
            // splitting the hosting name by dotSpliter
            string[] hostParts = this.Host.Split(dotSpliter);

            // traverse from the end to head
            int i = hostParts.Length - 1;
            for (string nodeStr = null; i >= 0; i--)
            {
                // analyze the nodes in host parts
                nodeStr = hostParts[i];

                // first see if the sitetype's not been determined
                if (this._siteType == "")
                {
                    // judge whether the current str in gTLDs
                    if (TLDHelper.gTLDsHave(nodeStr))
                    {
                        // if exists in gTLDs
                        this._siteType = nodeStr;
                        continue; // to next node
                    }
                }
                // assert: domainParts[i] is not site type
                // second, see if the site location's not determined
                if (this._siteLocation == "")
                {
                    if (TLDHelper.ccTLDsHave(nodeStr))
                    {
                        // if exists in ccTLDs
                        this._siteLocation = nodeStr;
                        continue; // to next node
                    }
                }
                else // site location is not empty:
                {
                    // check whether it's a sub location
                    if (TLDHelper.subccTLDsHave(nodeStr))
                    {
                        // if exist in SubccTLDs.xml
                        // this is a sub location
                        continue; // to next node
                    }
                }
                // assert: domainParts[i] is not a site location or sub location
                // third, this must be a site name
                this._siteName = nodeStr;
                if (i <= 0)
                    break; // stop the analysis
                    
                // the node before siteName may be sub site name
                this._subSiteName = hostParts[i - 1];
                for (int j = 0; j < _siteStarts.Length && 
                    _subSiteName != ""; j++)
                {
                    // check if it's a sub site name
                    if (_subSiteName == _siteStarts[j])
                        _subSiteName = ""; // failed
                }
                break; // We've got all necessary elements
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public Hyperlink(string url)
        {
            this._url = url.ToLower().Trim();
            // by default, using late initialization
            this.InitializeUrl();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="citeUrl"></param>
        public Hyperlink(string url, string citeUrl)
        {
            this._url = url.ToLower().Trim();
            this._citeUrl = citeUrl;
            this._citePath = GetAbsolutePath(_citeUrl);
            // by default, using late initialization
            this.InitializeUrl();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="citeUrl"></param>
        /// <param name="citePath"></param>
        public Hyperlink(string url, string citeUrl, string citePath)
        {
            this._url = url.ToLower().Trim();
            this._citeUrl = citeUrl;
            this._citePath = citePath;
            // by default, using late initialization
            this.InitializeUrl();
        }

        #endregion

        #region Static Members

        /// <summary>
        /// Get the domain name of the url
        /// </summary>
        /// <example>
        /// 'www.sjtu.edu.cn' in 
        /// 'http://www.sjtu.edu.cn/lib/default.aspx'
        /// </example>
        /// <param name="url"></param>
        /// <returns></returns>
        public static String GetDomainName(String url)
        {
            string urlDomain;
            int start = url.IndexOf("//");
            if (start < 0)
                start = 0;
            else
                start += 2;

            int end = url.IndexOf('/', start);
            if (end < 0)
                urlDomain = url.Substring(start);
            else
                urlDomain = url.Substring(start, end - start);
            return urlDomain;
        }

        /// <summary>
        /// Get the absolute path of the given url
        /// </summary>
        /// <example>
        /// 'http://www.sjtu.edu.cn/lib/' in
        /// 'http://www.sjtu.edu.cn/lib/test.aspx'
        /// </example>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetAbsolutePath(String url)
        {
            url = url.Trim();
            // 1. ensure the url is a valid url
            if (!urlRegex.IsMatch(url))
            {
                url = "http://" + url;
                if (!urlRegex.IsMatch(url))
                    return "";
            }
            // 2. analyize the url
            int firstSlash = url.IndexOf("/", 8);
            if (firstSlash < 0)         // http://www.sjtu.edu.cn
                return url + "/";       // http://www.sjtu.edu.cn/
            if (firstSlash == url.Length - 1) // http://www.sjtu.edu.cn/
                return url;             // http://www.sjtu.edu.cn/
            int lastSlash = url.IndexOf("/", firstSlash + 1);
            if (lastSlash < 0)          // http://www.sjtu.edu.cn/lib
                lastSlash = firstSlash;
            if (lastSlash == url.Length - 1) // http://www.sjtu.edu.cn/lib/
                return url;             // http://www.sjtu.edu.cn/lib/
            // get the file name
            string fileName = url.Substring(lastSlash + 1);
            if (fileName.IndexOf('.') < 0) // http://www.sjtu.edu.cn/lib
                return url + "/";       // http://www.sjtu.edu.cn/lib/
            // remove the file name
            return url.Remove(lastSlash + 1);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the hyper-link type
        /// </summary>
        /// <returns></returns>
        public HyperlinkType GetHyperlinkType()
        {
            if (this.IsEmail) // for email
                return HyperlinkType.Email;

            if (this.IsJavascript) // for javascript
                return HyperlinkType.Javascript;

            // for Site and Subsite
            if (this.FileName == "")
            {
                if (this.Segments.Length == 2)
                    return HyperlinkType.Site;
                else
                    return HyperlinkType.Subsite;
            }

            // for Service
            for (int i = 0; i < _servMids.Length; i++)
            {
                if (FileName.Contains(_servMids[i]))
                    return HyperlinkType.Service;
            }

            // get the file type
            string[] fparts = FileName.Split(dotSpliter);
            XmlElement elem = XmlHelper.ReadNode(
                Config.SettingPath + "FileTypes.xml",
                fparts[fparts.Length - 1]);
            // return the hyperlink type
            if (elem != null)
                return (HyperlinkType)int.Parse(
                    elem.Attributes["_Type"].Value);
            else
                return HyperlinkType.File;
        }

        /// <summary>
        /// Whether the link is a service link
        /// link type is music/picture/text/application/service/file
        /// </summary>
        /// <returns></returns>
        public static bool IsServiceLink(HyperlinkType type)
        {
            if (type == HyperlinkType.Media || type == HyperlinkType.Picture ||
                type == HyperlinkType.Text || type == HyperlinkType.Application ||
                type == HyperlinkType.Service || type == HyperlinkType.File ||
                type == HyperlinkType.Email || type == HyperlinkType.Javascript)
                return true;
            else return false;
        }

        public static bool IsSiteLink(HyperlinkType type)
        {
            if (type == HyperlinkType.Site || type == HyperlinkType.Subsite)
                return true;
            else return false;
        }

        /// <summary>
        /// Whethrer the two links are from similar site 
        /// (with the same site type and root site name)
        /// </summary>
        /// <example>'http://www.sjtu.edu.cn' and
        /// 'http://lib.sjtu.edu.cn'</example>
        /// <param name="link"></param>
        /// <returns></returns>
        public bool IsFromSimilarSite(Hyperlink link)
        {
            if (this.SiteType == link.SiteType &&
                this.SiteName == link.SiteName)
                return true;
            return false;
        }

        /// <summary>
        /// Whethrer the two links are from similar site 
        /// (with the same site type and root site name)
        /// </summary>
        /// <example>'http://www.sjtu.edu.cn' and
        /// 'http://lib.sjtu.edu.cn'</example>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsFromSimilarSite(String url)
        {
            // get the url's domain
            Hyperlink link = new Hyperlink(url);

            return IsFromSimilarSite(link);
        }

        /// <summary>
        /// Whethrer the two links are from similar site 
        /// (with the same site type and root site name)
        /// </summary>
        /// <returns></returns>
        public bool IsFromSimilarSite()
        {
            return IsFromSimilarSite(this._citeUrl);
        }

        /// <summary>
        /// Whether the two links are from same site
        /// (with the same domain name/host)
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public bool IsFromSameSite(Hyperlink link)
        {
            return (this.Host == link.Host);
        }

        /// <summary>
        /// Whether the two links are from same site
        /// (with the same domain name/host)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsFromSameSite(String url)
        {
            return (this.Host == GetDomainName(url));
        }

        /// <summary>
        /// Whether the two links are from same site
        /// (with the same domain name/host)
        /// </summary>
        /// <returns></returns>
        public bool IsFromSameSite()
        {
            return IsFromSameSite(this._citeUrl);
        }
        
        /// <summary>
        /// Whether the two urls are actually the same
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsSameUrl(String url)
        {
            url = url.ToLower().Trim();
            if (this.Url.Length == url.Length)
                return this.Url == url;

            // when the len of the two urls differ:
            int length = Math.Min(this.Url.Length, url.Length);

            // determine the smaller url
            string smallUrl = this.Url, bigUrl = url;
            if (this.Url.Length > url.Length)
            {
                smallUrl = url;
                bigUrl = this.Url;
            }
            if (bigUrl.Substring(0, length) != smallUrl)
                return false;
            // get the file name
            string file = bigUrl.Substring(length);
            if (homeRegex.Match(file).Value == file)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Whether the two urls are actually the same
        /// </summary>
        /// <returns></returns>
        public bool IsSameUrl()
        {
            return IsSameUrl(this._citeUrl);
        }

        #endregion

        #region Other Utilities

        public static Hashtable GetHyperlinkTypeBuckets()
        {
            Hashtable buckets = new Hashtable();
            buckets.Add(HyperlinkType.UnKnown, 0);
            buckets.Add(HyperlinkType.Site, 0);
            buckets.Add(HyperlinkType.Subsite, 0);
            buckets.Add(HyperlinkType.Media, 0);
            buckets.Add(HyperlinkType.Picture, 0);
            buckets.Add(HyperlinkType.Text, 0);
            buckets.Add(HyperlinkType.Application, 0);
            buckets.Add(HyperlinkType.Html, 0);
            buckets.Add(HyperlinkType.Service, 0);
            buckets.Add(HyperlinkType.File, 0);
            buckets.Add(HyperlinkType.Email, 0);
            buckets.Add(HyperlinkType.Javascript, 0);
            return buckets;
        }

        #endregion
    }
}