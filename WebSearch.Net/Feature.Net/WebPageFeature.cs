using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Model.Net;
using WebSearch.Common.Net;
using System.Collections;
using WebSearch.Feature.Net.Properties;

namespace WebSearch.Feature.Net
{
    public class WebPageFeature
    {
        #region Constructors

        private WebPage _webPage = null;

        public static WebPageFeature Get(WebPage page)
        {
            return new WebPageFeature(page);
        }

        protected WebPageFeature(WebPage page)
        {
            this._webPage = page;
        }

        #endregion

        #region Web Page Features

        protected void InitializeWebPageFeatures()
        {
            this._wordNum = GetContentLinguisticFeatures().WordNum;
            // parse the link number
            this._lnkNum = this._webPage.Links.Count;
            this._inSiteLnkNum = 0;
            int serviceLnkNum = 0;
            
            foreach (Hyperlink link in _webPage.Links)
            {
                // update the in-site link number
                if (link.IsFromSameSite()) _inSiteLnkNum++;

                // update the service link number
                if (Hyperlink.IsServiceLink(link.GetHyperlinkType()))
                    serviceLnkNum++;
            }
            // get the service link information
            this._serviceLnkInfo = (float)serviceLnkNum / 
                ((float)serviceLnkNum + 0.5F + 1.5F * 
                ((float)_lnkNum / (float)AVGLnkNum));

            // parse the images
            this._imgNum = HtmlParser.FindTagNum(_webPage.Html, "img");
        }

        private int _wordNum = Const.Invalid;

        public int WordNum
        {
            get
            {
                if (_wordNum == Const.Invalid)
                    InitializeWebPageFeatures();
                return _wordNum;
            }
            set { _wordNum = value; }
        }

        private int _lnkNum = Const.Invalid;

        /// <summary>
        /// Link number
        /// </summary>
        public int LinkNum
        {
            get 
            {
                if (_lnkNum == Const.Invalid)
                    InitializeWebPageFeatures();
                return _lnkNum; 
            }
            set { _lnkNum = value; }
        }

        private static float _avgLnkNum = (float)Const.Invalid;

        /// <summary>
        /// Average Link Number for web pages
        /// </summary>
        public static float AVGLnkNum
        {
            get
            {
                if (_avgLnkNum == (float)Const.Invalid)
                {
                    // read the value from setting file
                    _avgLnkNum = Settings.Default.AVGLinkNum;
                }
                return _avgLnkNum;
            }
            set { _avgLnkNum = value; }
        }

        /// <summary>
        /// Link number / Out degree
        /// </summary>
        public int OutDegree
        {
            get { return LinkNum; }
            set { LinkNum = value; }
        }

        private int _imgNum = Const.Invalid;

        /// <summary>
        /// The number of images
        /// </summary>
        public int ImageNum
        {
            get
            {
                if (_imgNum == Const.Invalid)
                    InitializeWebPageFeatures();
                return _imgNum;
            }
            set { _imgNum = value; }
        }

        /// <summary>
        /// Ratio of image along the plain text word number
        /// </summary>
        public double ImageRatio
        {
            get { return (double)ImageNum / (double)WordNum; }
        }

        private int _citesNum = Const.Invalid;

        /// <summary>
        /// Citation collection size
        /// </summary>
        public int CiteNum
        {
            get { return _citesNum; }
            set { _citesNum = value; }
        }

        /// <summary>
        /// Citation collection size / In degree
        /// </summary>
        public int InDegree
        {
            get { return CiteNum; }
            set { CiteNum = value; }
        }

        private int _inSiteLnkNum;

        /// <summary>
        /// In-site Out-link Number
        /// </summary>
        public int InSiteLnkNum
        {
            get
            {
                if (_inSiteLnkNum == Const.Invalid)
                    InitializeWebPageFeatures();
                return _inSiteLnkNum;
            }
            set { _inSiteLnkNum = value; }
        }

        /// <summary>
        /// Out-site Out-link Number
        /// </summary>
        public int OutSiteLnkNum
        {
            get { return LinkNum - InSiteLnkNum; }
        }

        /// <summary>
        /// Page's size (in characters)
        /// </summary>
        public int Size
        {
            get { return WordNum; }
            set { WordNum = value; }
        }

        private float _serviceLnkInfo = (float)Const.Invalid;

        /// <summary>
        /// Service Link Information
        /// </summary>
        public float ServiceLnkInfo
        {
            get
            {
                if (_serviceLnkInfo == (float)Const.Invalid)
                    InitializeWebPageFeatures();
                return _serviceLnkInfo;
            }
            set { _serviceLnkInfo = value; }
        }

        private int _pageRank = Const.Invalid;

        /// <summary>
        /// Page rank
        /// </summary>
        public int PageRank
        {
            get
            {
                if (_pageRank == Const.Invalid)
                {
                    // get page rank from PageRanker
                }
                return _pageRank;
            }
            set { _pageRank = value; }
        }

        protected WebClassification _pageClassification = WebClassification.Invalid;

        /// <summary>
        /// Web page classifiation
        /// </summary>
        public WebClassification PageClassification
        {
            get
            {
                if (_pageClassification == WebClassification.Invalid)
                {
                    // get page classification from PageClassifier
                }
                return _pageClassification;
            }
            set { _pageClassification = value; }
        }

        #endregion

        #region Linguistic Features for Title

        public int TitleLen
        {
            get { return _webPage.Title.Length; }
        }

        public LinguisticFeature GetTitleLinguisticFeatures()
        {
            // title are always in the form of phrases
            return LinguisticFeature.Get(_webPage.Title, true);
        }

        #endregion

        #region Linguistic Features for Content

        public LinguisticFeature GetContentLinguisticFeatures()
        {
            // html content are always in the form of passenges
            return LinguisticFeature.Get(HtmlParser.Filter(_webPage.Html, true), false);
        }

        #endregion
    }
}
