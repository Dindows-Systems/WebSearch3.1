using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Model.Net;
using WebSearch.DataCenter.Net.DS;
using WebSearch.DataCenter.Net;
using WebSearch.Common.Net;

namespace WebSearch.Feature.Net
{
    public class SearchResultListFeature
    {
        #region Constructors

        private UserQuery _userQuery = null;
        private SearchResultList _searchResults = null;

        public static SearchResultListFeature Get(UserQuery userQuery, WebCollection webCollection)
        {
            return new SearchResultListFeature(userQuery, webCollection);
        }

        public static SearchResultListFeature Get(UserQuery userQuery, SearchResultList searchResults)
        {
            return new SearchResultListFeature(userQuery, searchResults);
        }
        
        protected SearchResultListFeature(UserQuery userQuery, WebCollection webCollection)
        {
            this._userQuery = userQuery;
            // get the search result list from web collection
            IWebCollection webs = DataRetriever.GetWebCollection(webCollection);
            if (webs == null)
                throw new Exception("Invalid web collection resource");
            this._searchResults = webs.Search(_userQuery.Value, 20);
        }

        protected SearchResultListFeature(UserQuery userQuery, SearchResultList searchResults)
        {
            this._userQuery = userQuery;
            this._searchResults = searchResults;
        }

        #endregion

        protected List<SearchResultFeature> _srFeatures = null;
        protected List<SearchResultFeature> GetSearchResultFeatures()
        {
            if (_srFeatures == null)
            {
                _srFeatures = new List<SearchResultFeature>();
                foreach (SearchResult result in _searchResults)
                    _srFeatures.Add(SearchResultFeature.Get(result));
            }
            return _srFeatures;
        }

        #region Search Result List Features

        protected void InitializeSearchResultListFeatures()
        {
            List<SearchResultFeature> srFeatures = GetSearchResultFeatures();

            // get all the stat. model features:
            double[] tfArray = new double[_searchResults.Count];
            double[] uqProxArray = new double[_searchResults.Count];
            int[] uqProxPosArray = new int[_searchResults.Count];
            double[] tqProxArray = new double[_searchResults.Count];
            for (int i = 0; i < srFeatures.Count; i++)
            {
                tfArray[i] = srFeatures[i].OverallTF;
                uqProxArray[i] = (double)srFeatures[i].UrlQueryProximity.Value;
                uqProxPosArray[i] = srFeatures[i].UrlQueryProxiPos;
                tqProxArray[i] = (double)srFeatures[i].TitleQueryProximity.Value;
            }
            this._tfStat = new StatModel(tfArray);
            this._urlQueryProximityStat = new StatModel(uqProxArray);
            this._urlQueryProxiPosStat = new StatModel(uqProxPosArray);
            this._titleQueryProximityStat = new StatModel(tqProxArray);
        }

        private StatModel _tfStat = null;

        /// <summary>
        /// A measure of how often a term is found in a collection of documents.
        /// TF is combined with inverse document frequency (IDF) as a means 
        /// of determining which documents are most relevant to a query. 
        /// TF is sometimes also used to measure how often a word appears 
        /// in a specific document.
        /// </summary>
        public StatModel TFStat
        {
            get
            {
                if (_tfStat == null)
                    InitializeSearchResultListFeatures();
                return _tfStat;
            }
            set { _tfStat = value; }
        }

        private StatModel _urlQueryProximityStat = null;

        /// <summary>
        /// 
        /// </summary>
        public StatModel UrlQueryProximityStat
        {
            get
            {
                if (_urlQueryProximityStat == null)
                    InitializeSearchResultListFeatures();
                return _urlQueryProximityStat;
            }
        }

        private StatModel _urlQueryProxiPosStat = null;

        /// <summary>
        /// 
        /// </summary>
        public StatModel UrlQueryProxiPosStat
        {
            get
            {
                if (_urlQueryProxiPosStat == null)
                    InitializeSearchResultListFeatures();
                return _urlQueryProxiPosStat;
            }
        }

        private StatModel _titleQueryProximityStat = null;

        public StatModel TitleQueryProximityStat
        {
            get
            {
                if (_titleQueryProximityStat == null)
                    InitializeSearchResultListFeatures();
                return _titleQueryProximityStat;
            }
        }

        /// <summary>
        /// A measure of how rare a term is in a collection, calculated by total collection
        /// size divided by the number of documents containing the term. Very common terms 
        /// ("the", "and" etc.) will have a very low IDF and are therefor often excluded 
        /// from search results. These low IDF words are commonly referred to as "stop words". 
        /// </summary>
        public double IDF
        {
            get
            {
                return (double)_searchResults.HitCount / 
                    (double)_searchResults.SourceSize;
            }
        }

        #region Features from Stat of Snippets

        protected void InitializeSnippetFeatureStat()
        {
            List<SearchResultFeature> srFeatures = GetSearchResultFeatures();

            // get all the stat. model features:
            int[] termNuminTitleArray = new int[_searchResults.Count];
            int[] termNuminAnchorArray = new int[_searchResults.Count];
            int[] termNuminTextFArray = new int[_searchResults.Count];
            int[] termNuminListArray = new int[_searchResults.Count];
            int[] termNuminImageArray = new int[_searchResults.Count];
            int[] termNuminMetaArray = new int[_searchResults.Count];
            double[] servLnkInfoArray = new double[_searchResults.Count];
            for (int i = 0; i < srFeatures.Count; i++)
            {
                termNuminTitleArray[i] = srFeatures[i].HitHtmlTagTypes[HtmlTagType.Title];
                termNuminAnchorArray[i] = srFeatures[i].HitHtmlTagTypes[HtmlTagType.Anchor];
                termNuminTextFArray[i] = srFeatures[i].HitHtmlTagTypes[HtmlTagType.TextFormat];
                termNuminListArray[i] = srFeatures[i].HitHtmlTagTypes[HtmlTagType.List];
                termNuminImageArray[i] = srFeatures[i].HitHtmlTagTypes[HtmlTagType.Image];
                termNuminMetaArray[i] = srFeatures[i].HitHtmlTagTypes[HtmlTagType.Meta];
            }
            this._termNuminTitleStat = new StatModel(termNuminTitleArray);
            this._termNuminAnchorStat = new StatModel(termNuminAnchorArray);
            this._termNuminTextFormatStat = new StatModel(termNuminTextFArray);
            this._termNuminListStat = new StatModel(termNuminListArray);
            this._termNuminImageStat = new StatModel(termNuminImageArray);
            this._termNuminMetaStat = new StatModel(termNuminMetaArray);
        }

        private StatModel _termNuminTitleStat = null;

        public StatModel TermNuminTitleStat
        {
            get
            {
                if (_termNuminTitleStat == null)
                    InitializeSnippetFeatureStat();
                return _termNuminTitleStat;
            }
        }

        private StatModel _termNuminAnchorStat = null;

        public StatModel TermNuminAnchorStat
        {
            get
            {
                if (_termNuminAnchorStat == null)
                    InitializeSnippetFeatureStat();
                return _termNuminAnchorStat;
            }
        }

        private StatModel _termNuminTextFormatStat = null;

        public StatModel TermNuminTextFormatStat
        {
            get
            {
                if (_termNuminTextFormatStat == null)
                    InitializeSnippetFeatureStat();
                return _termNuminTextFormatStat;
            }
        }

        private StatModel _termNuminListStat = null;

        public StatModel TermNuminListStat
        {
            get
            {
                if (_termNuminListStat == null)
                    InitializeSnippetFeatureStat();
                return _termNuminListStat;
            }
        }

        private StatModel _termNuminImageStat = null;

        public StatModel TermNuminImageStat
        {
            get
            {
                if (_termNuminImageStat == null)
                    InitializeSnippetFeatureStat();
                return _termNuminImageStat;
            }
        }

        private StatModel _termNuminMetaStat = null;

        public StatModel TermNuminMetaStat
        {
            get
            {
                if (_termNuminMetaStat == null)
                    InitializeSnippetFeatureStat();
                return _termNuminMetaStat;
            }
        }

        #endregion

        #region Features from Stat of Web Page Features

        protected void InitializeWebPageFeatureStats()
        {
            List<SearchResultFeature> srFeatures = GetSearchResultFeatures();

            // get all the stat. model features:
            int[] titleLenArray = new int[_searchResults.Count];
            int[] pageSizeArray = new int[_searchResults.Count];
            int[] pageOutsArray = new int[_searchResults.Count];
            int[] pageInsArray = new int[_searchResults.Count];
            double[] imgRatioArray = new double[_searchResults.Count];
            int[] pageInLnkArray = new int[_searchResults.Count];
            double[] servLnkInfoArray = new double[_searchResults.Count];
            for (int i = 0; i < srFeatures.Count; i++)
            {
                WebPageFeature wf = srFeatures[i].GetWebPageFeatures();
                
                titleLenArray[i] = wf.TitleLen;
                pageSizeArray[i] = wf.Size;
                pageOutsArray[i] = wf.OutDegree;
                pageInsArray[i] = wf.InDegree;
                imgRatioArray[i] = wf.ImageRatio;
                pageInLnkArray[i] = wf.InSiteLnkNum;
                servLnkInfoArray[i] = (double)wf.ServiceLnkInfo;
            }
            this._pageTitleLenStat = new StatModel(titleLenArray);
            this._pageSizeStat = new StatModel(pageSizeArray);
            this._pageOutDegreeStat = new StatModel(pageOutsArray);
            this._pageInDegreeStat = new StatModel(pageInsArray);
            this._pageImgRatioStat = new StatModel(imgRatioArray);
            this._pageInSiteLnkNumStat = new StatModel(pageInLnkArray);
            this._pageServiceLnkInfoStat = new StatModel(servLnkInfoArray);
        }

        private StatModel _pageTitleLenStat = null;

        /// <summary>
        /// Result Web Page's Title Length Statistics
        /// </summary>
        public StatModel PageTitleLenStat
        {
            get
            {
                if (_pageTitleLenStat == null)
                    InitializeWebPageFeatureStats();
                return _pageTitleLenStat;
            }
        }

        private StatModel _pageSizeStat = null;

        /// <summary>
        /// 
        /// </summary>
        public StatModel PageSizeStat
        {
            get
            {
                if (_pageSizeStat == null)
                    InitializeWebPageFeatureStats();
                return _pageSizeStat;
            }
        }

        private StatModel _pageOutDegreeStat = null;

        public StatModel PageOutDegreeStat
        {
            get
            {
                if (_pageOutDegreeStat == null)
                    InitializeWebPageFeatureStats();
                return _pageOutDegreeStat;
            }
        }

        private StatModel _pageInDegreeStat = null;

        public StatModel PageInDegreeStat
        {
            get
            {
                if (_pageInDegreeStat == null)
                    InitializeWebPageFeatureStats();
                return _pageInDegreeStat;
            }
        }

        private StatModel _pageImgRatioStat = null;

        public StatModel PageImageRatioStat
        {
            get
            {
                if (_pageImgRatioStat == null)
                    InitializeWebPageFeatureStats();
                return _pageImgRatioStat;
            }
        }

        private StatModel _pageInSiteLnkNumStat = null;

        public StatModel PageInSiteLnkNum
        {
            get
            {
                if (_pageInSiteLnkNumStat == null)
                    InitializeWebPageFeatureStats();
                return _pageInSiteLnkNumStat;
            }
        }

        private StatModel _pageServiceLnkInfoStat = null;

        public StatModel PageServiceLnkInfo
        {
            get
            {
                if (_pageServiceLnkInfoStat == null)
                    InitializeWebPageFeatureStats();
                return _pageServiceLnkInfoStat;
            }
        }

        #endregion

        #region Features from Stat of Url Features

        protected void InitializeUrlFeatureStats()
        {
            List<SearchResultFeature> srFeatures = GetSearchResultFeatures();

            // get all the stat. model features:
            int[] urlLenArray = new int[_searchResults.Count];
            int[] urlDepthArray = new int[_searchResults.Count];
            int srvLnkNum = 0, siteLnkNum = 0;

            for (int i = 0; i < srFeatures.Count; i++)
            {
                Hyperlink lf = srFeatures[i].GetUrlFeatures();

                urlLenArray[i] = lf.Length;
                urlDepthArray[i] = lf.Depth;
                HyperlinkType type = lf.GetHyperlinkType();
                if (Hyperlink.IsServiceLink(type))
                    srvLnkNum++;
                if (Hyperlink.IsSiteLink(type))
                    siteLnkNum++;
            }
            this._urlLenStat = new StatModel(urlLenArray);
            this._urlDepthStat = new StatModel(urlDepthArray);
            this._serviceLnkRatio = ((double)srvLnkNum /
                (double)_searchResults.Count);
            this._siteLnkRatio = ((double)siteLnkNum /
                (double)_searchResults.Count);
        }

        private StatModel _urlLenStat = null;

        /// <summary>
        /// Url Length Statistics
        /// </summary>
        public StatModel UrlLenStat
        {
            get
            {
                if (_urlLenStat == null)
                    InitializeUrlFeatureStats();
                return _urlLenStat;
            }
        }

        private StatModel _urlDepthStat = null;

        /// <summary>
        /// Url Depth Statistics
        /// </summary>
        public StatModel UrlDepthStat
        {
            get
            {
                if (_urlDepthStat == null)
                    InitializeUrlFeatureStats();
                return _urlDepthStat;
            }
        }

        private double _serviceLnkRatio = (double)Const.Invalid;

        public double ServiceLnkRatio
        {
            get
            {
                if (_serviceLnkRatio == (double)Const.Invalid)
                    InitializeUrlFeatureStats();
                return _serviceLnkRatio;
            }
        }

        private double _siteLnkRatio = (double)Const.Invalid;

        public double SiteLnkRatio
        {
            get
            {
                if (_siteLnkRatio == (double)Const.Invalid)
                    InitializeUrlFeatureStats();
                return _siteLnkRatio;
            }
        }

        #endregion

        #endregion

        #region User Query Features

        public UserQueryFeature GetUserQueryFeatures()
        {
            return UserQueryFeature.Get(_userQuery);
        }

        #endregion
    }
}
