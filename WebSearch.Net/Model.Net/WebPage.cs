using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using WebSearch.Common.Net;
using System.Collections;

namespace WebSearch.Model.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class WebPage : BaseModel
    {
        public static int CurrentID = 0;

        public static Queue<int> UnusedIDs = new Queue<int>(50);

        #region Properties

        private int _id;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _url;

        /// <summary>
        /// Page's Url
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private string _html;

        /// <summary>
        /// Page's content in html form
        /// </summary>
        public string Html
        {
            get { return _html; }
            set { _html = value; }
        }

        private Encoding _encoding;

        /// <summary>
        /// Page's encoding
        /// </summary>
        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        private string _contentType;

        /// <summary>
        /// 
        /// </summary>
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        private string _title = null;

        /// <summary>
        /// The title of the page
        /// </summary>
        public string Title
        {
            get
            {
                if (_title == null)
                    _title = HtmlParser.FindTagContent(_html, "title");
                return _title;
            }
            set { _title = value; }
        }

        private string _keywords = null;

        /// <summary>
        /// Page's keywords
        /// </summary>
        public string Keywords
        {
            get 
            { 
                if (_keywords == null)
                    _keywords = HtmlParser.FindKeywords(_html); 
                return _keywords;
            }
            set { _keywords = value; }
        }

        private string _description = null;

        /// <summary>
        /// Page's description
        /// </summary>
        public string Description
        {
            get
            {
                if (_description == null)
                    _description = HtmlParser.FindDescription(_html); 
                return _description;
            }
            set { _description = value; }
        }

        private List<Hyperlink> _links = null;

        /// <summary>
        /// 
        /// </summary>
        public List<Hyperlink> Links
        {
            get 
            {
                if (_links == null)
                {
                    // parse the anchor links:
                    ArrayList anchors = HtmlParser.FindTag(Html, "a");
                    string decendUrl = null, path = Hyperlink.GetAbsolutePath(_url);
                    this._links = new List<Hyperlink>();
                    foreach (string anchorTag in anchors)
                    {
                        decendUrl = HtmlParser.FindAttribute(anchorTag, "href");
                        if (decendUrl == "") continue;
                        // check if the url is valid
                        Hyperlink link = new Hyperlink(decendUrl, _url, path);
                        if (link.IsValid && !link.IsEmail && !link.IsJavascript)
                            this._links.Add(link);
                    }
                }
                return this._links;
            }
            set { _links = value; }
        }

        #endregion

        #region Constructors

        public WebPage()
        {
            if (UnusedIDs.Count > 0)
                this._id = UnusedIDs.Dequeue();
            else
                this._id = ++CurrentID;
        }

        /// <summary>
        /// id, url, html and encoding are the musts for a web page
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="html"></param>
        /// <param name="encoding"></param>
        public WebPage(int id, string url, string html, Encoding encoding)
        {
            this._id = id;
            this._url = url;
            this._html = html;
            this._encoding = encoding;
        }

        #endregion
    }
}
