using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Model.Net;
using WebSearch.DataCenter.Net.DS;
using WebSearch.DataCenter.Net;
using System.Collections;
using System.Text.RegularExpressions;
using WebSearch.Common.Net;
using WebSearch.Maths.Net;

namespace WebSearch.Feature.Net
{
    public class SearchResultFeature
    {
        #region Constructors

        private SearchResult _searchResult = null;

        public static SearchResultFeature Get(SearchResult searchResult)
        {
            return new SearchResultFeature(searchResult);
        }

        protected SearchResultFeature(SearchResult searchResult)
        {
            this._searchResult = searchResult;
        }

        #endregion

        #region Search Result Features

        protected float _tf;

        /// <summary>
        /// Term Frequency
        /// A measure of how often a term is found in a collection of documents. 
        /// TF is combined with inverse document frequency (IDF) as a means 
        /// of determining which documents are most relevant to a query. 
        /// TF is sometimes also used to measure how often a word appears
        /// in a specific document.
        /// </summary>
        public Dictionary<string, double> TFs
        {
            get 
            {
                if (_searchResult.TFs == null)
                {
                    // calculate tf
                    // in lucene, it's been calculated
                }
                return _searchResult.TFs;
            }
        }

        private double _overallTF = (double)Const.Invalid;

        public double OverallTF
        {
            get
            {
                if (_overallTF == (double)Const.Invalid)
                {
                    _overallTF = 0;
                    // the sum of all tfs
                    foreach (double v in TFs.Values)
                        _overallTF += v;
                }
                return _overallTF;
            }
        }

        #region Features between Query and Snippets

        protected void InitializeSnippetFeatures()
        {
            // initialize the _hitHtmlTagTypes
            _hitHtmlTagTypes = new Dictionary<HtmlTagType, int>();
            _hitHtmlTagTypes.Add(HtmlTagType.Anchor, 0);
            _hitHtmlTagTypes.Add(HtmlTagType.Citation, 0);
            _hitHtmlTagTypes.Add(HtmlTagType.ComputerOutput, 0);
            _hitHtmlTagTypes.Add(HtmlTagType.Empty, 0);
            _hitHtmlTagTypes.Add(HtmlTagType.Image, 0);
            _hitHtmlTagTypes.Add(HtmlTagType.List, 0);
            _hitHtmlTagTypes.Add(HtmlTagType.Meta, 0);
            _hitHtmlTagTypes.Add(HtmlTagType.Table, 0);
            _hitHtmlTagTypes.Add(HtmlTagType.TextFormat, 0);
            _hitHtmlTagTypes.Add(HtmlTagType.Title, 0);

            // initialize the _hitHtmlTags
            _hitHtmlTags = new Dictionary<string, int>();

            // get hit html tag for each snippets
            Regex tag = HtmlParser.GetHtmlTaggedContentRegex("h");
            foreach (string snippet in _searchResult.Snippets)
            {
                // one snippet may contain 1..* hits, split these hits:
                string[] parts = tag.Split(snippet);
                string before = "", behind = "", tagResult;
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    // get the before and behind html
                    for (int j = 0; j <= i; j++)
                        before += parts[j];
                    for (int j = i + 1; j < parts.Length; j++)
                        behind += parts[j];

                    // get the enclosed html tag
                    HtmlTagType type = HtmlParser.GetEnclosedHtmlTag(
                        before, behind, out tagResult);

                    // update hit html tag types
                    if (_hitHtmlTagTypes.ContainsKey(type))
                        _hitHtmlTagTypes[type]++;
                    else
                        _hitHtmlTagTypes.Add(type, 1);

                    // update hit html tags
                    if (_hitHtmlTags.ContainsKey(tagResult))
                        _hitHtmlTags[tagResult]++;
                    else
                        _hitHtmlTags.Add(tagResult, 1);
                }
            }
        }

        private Dictionary<HtmlTagType, int> _hitHtmlTagTypes = null;

        /// <summary>
        /// Hit Html Tag Types
        /// Key: html tag type
        /// Value: term count (int)
        /// </summary>
        public Dictionary<HtmlTagType, int> HitHtmlTagTypes
        {
            get
            {
                if (_hitHtmlTagTypes == null)
                {
                    InitializeSnippetFeatures();
                }
                return _hitHtmlTagTypes;
            }
            set { _hitHtmlTagTypes = value; }
        }

        private Dictionary<string, int> _hitHtmlTags = null;

        /// <summary>
        /// Hit Html Tags
        /// Key: html tag
        /// Value: term count (int)
        /// </summary>
        public Dictionary<string, int> HitHtmlTags
        {
            get
            {
                if (_hitHtmlTags == null)
                {
                    InitializeSnippetFeatures();
                }
                return _hitHtmlTags;
            }
            set { _hitHtmlTags = value; }
        }

        #endregion

        #region Features between Url and Query

        protected void InitializeUrlQueryFeatures()
        {
            Hyperlink link = GetUrlFeatures();
            string[] segments = link.Segments;
            List<string> acronyms = LinguisticFeature.Get(
                _searchResult.Query.Value).GetAcronyms();

            // for each <segment, query>
            // for the first segment: domain part
            // phrases are divided by '.'
            string[] phrases = segments[0].Split('.');
            Degree refDegree = null;

            foreach (string query in acronyms) // for each acronym
            {
                foreach (string phrase in phrases)
                {
                    refDegree = StringHelper.Similarity(phrase, query);
                    if (_urlQueryProximity == null ||
                        refDegree.Value > _urlQueryProximity.Value)
                    {
                        _urlQueryProximity = refDegree;
                        _urlQueryProxiPos = 0;
                    }
                }
                // for the sub segments
                for (int i = 1; i < segments.Length; i++)
                {
                    refDegree = StringHelper.Similarity(segments[i], query);
                    if (_urlQueryProximity == null ||
                        refDegree.Value > _urlQueryProximity.Value)
                    {
                        _urlQueryProximity = refDegree;
                        _urlQueryProxiPos = i;
                    }
                }
            }
        }

        protected Degree _urlQueryProximity = null;

        /// <summary>
        /// The proximity between url and query
        /// </summary>
        public Degree UrlQueryProximity
        {
            get
            {
                if (_urlQueryProximity == null)
                    InitializeUrlQueryFeatures();
                return _urlQueryProximity;
            }
            set { _urlQueryProximity = value; }
        }

        protected int _urlQueryProxiPos = Const.Invalid;

        /// <summary>
        /// The segment pos with the largest proximity between url and query
        /// </summary>
        public int UrlQueryProxiPos
        {
            get
            {
                if (_urlQueryProxiPos == Const.Invalid)
                    InitializeUrlQueryFeatures();
                return _urlQueryProxiPos;
            }
            set { _urlQueryProxiPos = value; }
        }

        #endregion

        #region Features between Query and Title

        protected void InitializeTitleQueryFeatures()
        {
            _titleQueryProximity = StringHelper.Similarity(
                _searchResult.Query.Value, _searchResult.Page.Title);
        }

        protected Degree _titleQueryProximity = null;

        public Degree TitleQueryProximity
        {
            get
            {
                if (_titleQueryProximity == null)
                    InitializeTitleQueryFeatures();
                return _titleQueryProximity;
            }
            set { _titleQueryProximity = value; }
        }

        #endregion

        #endregion

        #region User Query Features

        public UserQueryFeature GetUserQueryFeatures()
        {
            return UserQueryFeature.Get(_searchResult.Query);
        }

        #endregion

        #region Web Page Features

        public WebPageFeature GetWebPageFeatures()
        {
            return WebPageFeature.Get(_searchResult.Page);
        }

        #endregion

        #region Url Features

        public Hyperlink GetUrlFeatures()
        {
            string url = this._searchResult.Url.Trim().ToLower();;
            if (!url.StartsWith("http") && !url.StartsWith("ftp"))
                // complete the url
                url = "http://" + url;

            return new Hyperlink(url);
        }

        #endregion
    }
}
