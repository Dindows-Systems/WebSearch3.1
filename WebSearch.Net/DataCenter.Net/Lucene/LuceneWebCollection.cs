using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.DataCenter.Net.DS;
using WebSearch.Model.Net;
using Lucene.Net.Documents;

namespace WebSearch.DataCenter.Net.Lucene
{
    public class LuceneWebCollection : LuceneHelper, IWebCollection
    {
        #region Data Source

        protected WebCollection _dataSource;

        public WebCollection DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        #endregion

        #region LuceneWebCollection Constructors

        public LuceneWebCollection(WebCollection ds)
            : base(ds)
        {
            this._dataSource = ds;
        }

        #endregion

        #region IWebCollection Members

        public SearchResultList Search(string query, int count)
        {
            SearchResultList results = _searcher.Search(
                query, WebCollection.Html, count);
            results.SourceSize = this._dataSource.Size;
            return results;
        }

        public string[] SearchUrls(string query, int count)
        {
            SearchResultList searchResults = this._searcher.Search(
                query, WebCollection.Html, count);
            // if not get any search results
            if (searchResults == null || searchResults.Count == 0)
                return new string[0];

            string[] urls = new string[searchResults.Count];
            int index = 0;
            foreach (SearchResult result in searchResults)
            {
                // extract the url in search results
                urls[index] = result.Url;
                index++;
            }
            return urls;
        }

        #endregion

        #region IDocument Members

        public static Document ToDocument(WebPage webPage)
        {
            Document doc = new Document();

            doc.Add(Field.Keyword(WebCollection.ID, webPage.ID.ToString()));
            doc.Add(Field.Keyword(WebCollection.Url, webPage.Url));
            doc.Add(new Field(WebCollection.Title, webPage.Title, Field.Store.YES,
                Field.Index.TOKENIZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            doc.Add(Field.Text(WebCollection.Description, webPage.Description));
            doc.Add(Field.Text(WebCollection.Keywords, webPage.Keywords));
            doc.Add(new Field(WebCollection.Html, webPage.Html, Field.Store.YES,
                Field.Index.TOKENIZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            doc.Add(Field.UnIndexed(WebCollection.Encoding, webPage.Encoding.WebName));

            return doc;
        }

        public static WebPage ToWebPage(Document document)
        {
            WebPage page = new WebPage();

            page.ID = int.Parse(document.Get(WebCollection.ID));
            page.Url = document.Get(WebCollection.Url);
            page.Html = document.Get(WebCollection.Html);
            page.Title = document.Get(WebCollection.Title);
            page.Description = document.Get(WebCollection.Description);
            page.Keywords = document.Get(WebCollection.Keywords);
            page.Encoding = Encoding.GetEncoding(document.Get(WebCollection.Encoding));

            return page;
        }

        #endregion
    }
}
