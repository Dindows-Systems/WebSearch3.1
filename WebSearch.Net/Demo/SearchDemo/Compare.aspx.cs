using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using WebSearch.DataCenter.Net.Lucene;
using WebSearch.DataCenter.Net.DS;
using System.Collections.Generic;
using WebSearch.Model.Net;
using WebSearch.Common.Net;
using WebSearch.DataCenter.Net;
using System.Xml;
using System.Text;
using Evaluator.Net;

public partial class Compare : System.Web.UI.Page
{
    protected string WebCollectionName
    {
        get { return (string)ViewState["WebCollection"]; }
        set { ViewState["WebCollection"] = value; }
    }
    protected string QueryLogName
    {
        get { return (string)ViewState["QueryLog"]; }
        set { ViewState["QueryLog"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            // get the request
            if (Request["search"] != null)
            {
                string query = Request["search"].ToString();
                query = HttpUtility.UrlDecode(query);
                this.tb_UserQuery.Text = query;
                this.btn_Search_Click(null, null);
            }
        }
    }

    #region Search Button Events

    protected void btn_Search_Click(object sender, ImageClickEventArgs e)
    {
        string query = this.tb_UserQuery.Text;
        int size = int.Parse(this.dl_ResultNum.SelectedValue);
        string webcoll = this.tb_WebCollection.Text;
        string querylog = this.tb_QueryLog.Text;
        // ensure the foot has the same values
        this.tb_UserQuery2.Text = query;
        this.dl_ResultNum2.SelectedValue = size.ToString();
        this.tb_WebCollection2.Text = webcoll;
        this.tb_QueryLog2.Text = querylog;
        // call the search function
        this.Search(query, size, webcoll, querylog);
    }
    protected void btn_Search2_Click(object sender, ImageClickEventArgs e)
    {
        string query = this.tb_UserQuery2.Text;
        int size = int.Parse(this.dl_ResultNum2.SelectedValue);
        string webcoll = this.tb_WebCollection2.Text;
        string querylog = this.tb_QueryLog2.Text;
        // ensure the head has the same values
        this.tb_UserQuery.Text = query;
        this.dl_ResultNum.SelectedValue = size.ToString();
        this.tb_WebCollection.Text = webcoll;
        this.tb_QueryLog.Text = querylog;
        // call the search function
        this.Search(query, size, webcoll, querylog);
    }

    #endregion

    #region Call Search Function

    protected void Search(string query, int size, string webcollection, string querylog)
    {
        // 1. update the web collection name and searcher
        if (this.WebCollectionName != webcollection || 
            Helper.Searcher == null)
        {
            this.WebCollectionName = webcollection;
            // get the data source according to web collection name
            Helper.Searcher = DataRetriever.GetWebCollection(webcollection);
        }
        if (this.QueryLogName != querylog ||
            Helper.SogouQueryLog == null)
        {
            this.QueryLogName = querylog;
            // get the data source according to query log name
            Helper.SogouQueryLog = DataRetriever.GetQueryLog(querylog);
        }
        if (Helper.SogouResultXml == null)
        {
            XmlDocument _sogouResultXml = new XmlDocument();
            _sogouResultXml.Load(Server.MapPath("searchResults.xml"));
            Helper.SogouResultXml = _sogouResultXml;
        }

        // 2. search the query for result list
        SearchResultList results = Helper.Searcher.Search(query, 500);
        if (results == null)
        {
            this.BindSogouResult(null);
            this.BindSearchResult(null);
            this.BindQueryLogResult(null);
            return;
        }

        size = (size < results.Count) ? size : results.Count;

        #region a. get the cross of sogou result and our search results
        // get the sogou results
        XmlElement elem = Helper.SogouResultXml.GetElementById(query);
        XmlNodeList nlist = null;
        if (elem != null && elem.Attributes.Count != 0) // if exist the query
        {
            nlist = elem.GetElementsByTagName("Url");

            SearchResultList sogouResults = new SearchResultList(size);
            for (int i = 0; i < nlist.Count; i++)
            {
                SearchResult ourResult = GetSearchResult(
                    results, nlist.Item(i).Attributes["_Link"].Value);
                if (ourResult != null)
                {
                    // construct the sogou search result object
                    SearchResult sr = new SearchResult();
                    XmlElement node = (XmlElement)nlist.Item(i);
                    sr.Url = node.Attributes["_Link"].Value;
                    sr.Rank = int.Parse(node.Attributes["_Rank"].Value);

                    XmlNodeList snippetList = node.GetElementsByTagName("Snippet");
                    if (snippetList != null && snippetList.Count > 0)
                        sr.Snippets.Add(snippetList.Item(0).InnerText);

                    sr.Anchor = node.Attributes["_Title"].Value;
                    sr.OtherInfos.Add("OurRank", ourResult.Rank);
                    sogouResults.Add(sr);

                    if (sogouResults.Count >= size)
                        break; // stop iteration
                }
            }
            this.BindSogouResult(sogouResults);
        }
        else
            this.BindSogouResult(null);

        #endregion

        #region b. get the click through result according in query log
        Helper.SogouQueryLog.SetRetrievalType(QueryLog.RetrievalType.ByLink);
        // get the clickthroughs data group by link, order by click count desc
        IList<ClickThrough> clickThroughs = Helper.SogouQueryLog.Retrieve(
            query, MatchType.Exact, "", size, QueryLog.Count + " DESC");
        if (clickThroughs != null)
            this.BindQueryLogResult(clickThroughs);
        else
            this.BindQueryLogResult(null);
        #endregion

        #region 3. get the top 'size' search results
        SearchResultList ourResults = new SearchResultList(size);
        for (int i = 0; i < size; i++)
            ourResults.Add(results[i]);
        this.BindSearchResult(ourResults);

        #region 4. get nDCG@10 over query log results
        size = (size < 10) ? size : 10;
        uint[] relevances = new uint[size];
        for (int i = 0; i < size; i++)
        {
            string url = results[i].Url;
            // find the ulr in query log result
            bool find = false;
            for (int j = 0; j < clickThroughs.Count; j++)
            {
                if (clickThroughs[j].ResultUrl == url ||
                    url.Contains(clickThroughs[j].ResultUrl))
                {
                    find = true;
                    relevances[i] = (uint)Math.Ceiling((double)(10 - j) / 2.0);
                    break;
                }
            }
            if (!find)
                relevances[i] = 0;
        }
        NDCG evaluator = new NDCG(NDCG.GradedRelevanceLevels);
        this.lbl_nDCG.Text = (evaluator.Evaluate(relevances)).ToString() + "[";
        foreach (uint v in relevances)
            lbl_nDCG.Text += " " + v.ToString() + " ";
        lbl_nDCG.Text += "]";
        #endregion

        #endregion
    }

    protected void BindSearchResult(SearchResultList searchResults)
    {
        this.dtl_SearchResults.DataSource = searchResults;
        this.dtl_SearchResults.DataBind();
    }

    protected void BindSogouResult(SearchResultList sogouResults)
    {
        this.dtl_SogouResults.DataSource = sogouResults;
        this.dtl_SogouResults.DataBind();
    }

    protected void BindQueryLogResult(IList<ClickThrough> clickThroughResults)
    {
        this.dtl_QueryLogResults.DataSource = clickThroughResults;
        this.dtl_QueryLogResults.DataBind();
    }

    #endregion

    protected void dtl_SearchResults_ItemDataBound(object sender, DataListItemEventArgs e)
    {
        Label snippets = (Label)e.Item.FindControl("lb_Snippets");
        SearchResult searchResult = (SearchResult)e.Item.DataItem;
        if (snippets != null && searchResult != null)
        {
            foreach (string snippet in searchResult.Snippets)
            {
                string show = HtmlParser.EncodeHtml(snippet);
                show = show.Replace("&lt;h&gt;", "<b>");
                show = show.Replace("&lt;/h&gt;", "</b>");
                snippets.Text += show + "<b>...</b><br />";
            }
        }

        // try to find the search result in sogou search result record
        int crawledRank;
        int sogouRank = GetSogouRank(this.tb_UserQuery.Text,
            searchResult.Url, out crawledRank);
        Label sogou = (Label)e.Item.FindControl("lb_SogouRank");
        if (sogouRank != Const.Invalid)
        {
            sogou.Visible = true;
            sogou.Text = "[sogou:" + sogouRank.ToString() +
                "/" + crawledRank.ToString() + "]";
        }
        else
            // not listed by sogou:
            sogou.Visible = false;
    }
    protected void dtl_SogouResults_ItemDataBound(object sender, DataListItemEventArgs e)
    {
        Label snippets = (Label)e.Item.FindControl("lb_Snippets");
        SearchResult searchResult = (SearchResult)e.Item.DataItem;
        if (snippets != null && searchResult != null)
        {
            foreach (string snippet in searchResult.Snippets)
                snippets.Text += snippet + "<b>...</b><br />";
        }
        // get our rank
        Label ours = (Label)e.Item.FindControl("lb_OurRank");
        if (searchResult.OtherInfos.ContainsKey("OurRank"))
        {
            ours.Visible = true;
            ours.Text = "[ours:" + searchResult.OtherInfos["OurRank"].ToString() + "]";
        }
        else
            ours.Visible = false;
    }
    protected void dtl_QueryLogResults_ItemDataBound(object sender, DataListItemEventArgs e)
    {
        HyperLink title = (HyperLink)e.Item.FindControl("lnk_Title");
        ClickThrough clt = (ClickThrough)e.Item.DataItem;
        string url = clt.ResultUrl;
        if (url.Length < 50)
            title.Text = url;
        else
            title.Text = url.Substring(0, 48) + "...";
    }

    private int GetSogouRank(string query, string url, out int crawledRank)
    {
        XmlElement elem = Helper.SogouResultXml.GetElementById(query);
        if (elem == null)
        {
            crawledRank = Const.Invalid;
            return Const.Invalid;
        }
        // has find the query
        // try to get the rank for the url
        XmlNodeList nlist = elem.GetElementsByTagName("Url");
        crawledRank = 1;
        for (int i = 0; i < nlist.Count; i++)
        {
            if (nlist.Item(i).Attributes["_Link"].Value == url)
                return i + 1;
            if (bool.Parse(nlist.Item(i).Attributes["_Crawled"].Value))
            crawledRank++; 
        }
        // fially, if not find the url
        crawledRank = Const.Invalid;
        return Const.Invalid;
    }

    private SearchResult GetSearchResult(SearchResultList resultList, string url)
    {
        foreach (SearchResult result in resultList)
        {
            if (result.Url == url)
                return result;
        }
        return null; // unfound
    }
}
