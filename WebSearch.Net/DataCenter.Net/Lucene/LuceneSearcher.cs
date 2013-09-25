using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Search;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Analysis.Standard;
using WebSearch.Model.Net;
using System.Collections;
using WebSearch.Common.Net;
using Lucene.Net.Highlight;
using WebSearch.Linguistics.Net;
using WebSearch.DataCenter.Net.DS;
using System.Text.RegularExpressions;

namespace WebSearch.DataCenter.Net.Lucene
{
    public class LuceneSearcher : Formatter
    {
        #region Settings for ParallelSearcher

        /// <summary>
        /// the place to hold the index
        /// </summary>
        public string INDEX_STORE_PATH = @"C:\Index";

        /// <summary>
        /// Language used in this indexer
        /// </summary>
        public SupportedLanguage LANGUAGE = SupportedLanguage.Chinese;

        /// <summary>
        /// 
        /// </summary>
        public static int MaxNumFragmentsRequired = 5;

        /// <summary>
        /// 
        /// </summary>
        public static int FragmentSize = 30;

        /// <summary>
        /// 
        /// </summary>
        public static string FragmentSeparator = "...";

        #endregion

        private Analyzer _analyzer = null;
        private IndexSearcher _searcher = null;
        private IndexReader _reader = null;
        private int _numHighlights = 0;

        #region Constructors

        public LuceneSearcher()
        {
            try
            {
                this.InitializeSearchReader();
                this._analyzer = LuceneAnalyzer.Get(LANGUAGE);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public LuceneSearcher(string path,
            SupportedLanguage language)
        {
            try
            {
                this.INDEX_STORE_PATH = path;
                this.LANGUAGE = language;

                this.InitializeSearchReader();
                this._analyzer = LuceneAnalyzer.Get(LANGUAGE);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Initialize Index Searcher and Index Reader
        /// </summary>
        protected void InitializeSearchReader()
        {
            try
            {
                this._reader = IndexReader.Open(INDEX_STORE_PATH);
                this._searcher = new IndexSearcher(_reader);
            }
            catch (Exception exception)
            {
                throw (exception);
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public Hits SearchKeyword(string keyword, string field)
        {
            if (this._searcher == null)
                throw new Exception("Invalid Index Searcher");

            try
            {
                // keyword search cannot use parser!!
                // see: http://mail-archives.apache.org/mod_mbox/lucene-java-user/
                // 200410.mbox/%3CBAED4A5052281C49B0181F191AD263E91573F2@dc1mailbox01.
                // mpls.digitalriver.com%3E
                Query query = new TermQuery(new Term(field, keyword));

                // search the query for the hits
                return this._searcher.Search(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SearchResultList Search(string queryStr, string field, int count)
        {
            if (this._searcher == null)
                throw new Exception("Invalid Index Searcher");

            try
            {
                QueryParser parser = new QueryParser(field, _analyzer);
                parser.SetOperator(QueryParser.DEFAULT_OPERATOR_AND);
                if (this.LANGUAGE == SupportedLanguage.Chinese)
                    ChineseHelper.Segment(queryStr, out queryStr);
                string[] queryTerms = Regex.Split(queryStr, "(\\s|\\r\\n)+");
                Query query = parser.Parse(queryStr);
                
                Hits hits = this._searcher.Search(query);

                Highlighter highlighter = new Highlighter(this, new QueryScorer(query));
                highlighter.SetTextFragmenter(new SimpleFragmenter(FragmentSize));

                count = (count != Const.All && count < hits.Length()) ? count : hits.Length();

                UserQuery userQuery = new UserQuery(queryStr);
                SearchResultList results = new SearchResultList(count);
                results.HitCount = hits.Length();

                // for every result doc, parse it into a search result object
                for (int i = 0; i < count; i++)
                {
                    SearchResult result = new SearchResult();
                    result.Query = userQuery;
                    result.Url = hits.Doc(i).Get(WebCollection.Url);
                    result.Anchor = hits.Doc(i).Get(WebCollection.Title);
                    result.Score = hits.Score(i);
                    result.Rank = i + 1;
                    result.Page = LuceneWebCollection.ToWebPage(hits.Doc(i));
                    
                    // get the snippets
                    string text = hits.Doc(i).Get(field);
                    TermPositionVector tpv = (TermPositionVector)
                        _reader.GetTermFreqVector(hits.Id(i), field);
                    
                    string[] terms = tpv.GetTerms(); // all the terms in doc
                    int[] tfs = tpv.GetTermFrequencies(); // all for doc
                    int totalTermCount = 0; // total term count in doc
                    for (int j = 0; j < tfs.Length; j++)
                        totalTermCount += tfs[j];

                    if (result.TFs == null)
                        result.TFs = new Dictionary<string, double>();

                    // get tern frequencies for the query terms
                    foreach (string queryTerm in queryTerms)
                    {
                        for (int j = 0; j < terms.Length; j++)
                        {
                            if (terms[j].ToLower() == queryTerm.ToLower())
                            {
                                if (result.TFs.ContainsKey(queryTerm))
                                    result.TFs[queryTerm] += (double)
                                        tfs[j] / (double)totalTermCount;
                                else
                                    result.TFs.Add(queryTerm, (double)
                                        tfs[j] / (double)totalTermCount);
                                break;
                            }
                        }
                    }

                    TokenStream tokenStream = TokenSources.GetTokenStream(tpv);
                    String[] snippets = highlighter.GetBestFragments(
                        tokenStream, text, MaxNumFragmentsRequired);
                    for (int j = 0; j < snippets.Length; j++)
                        result.Snippets.Add(snippets[j]);

                    results.Add(result); // add the result
                }
                return results;// this._searcher.Search(query);
            }
            catch (Exception ex)
            {
                // log here
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryStr"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public SearchResultList Search(string queryStr, string field)
        {
            return Search(queryStr, field, Const.All);
        }

        #region Formatter Members

        public string HighlightTerm(string strOriginalText, TokenGroup tokenGroup)
        {
            if (tokenGroup.GetScore(0) <= 0)
                return strOriginalText;
            this._numHighlights++;
            return "<" + SearchResult.HitTermTag + ">" + strOriginalText +
                "</" + SearchResult.HitTermTag + ">";
        }

        #endregion

        public void Close()
        {
            if (this._searcher != null)
                this._searcher.Close();
            if (this._reader != null)
                this._reader.Close();
        }
    }
}
