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

public partial class _Default : System.Web.UI.Page 
{
    protected string WebCollectionName
    {
        get { return (string)ViewState["WebCollection"]; }
        set { ViewState["WebCollection"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
        }
    }

    protected void btn_Search_Click(object sender, ImageClickEventArgs e)
    {
        string query = this.tb_UserQuery.Text;
        int size = int.Parse(this.dl_ResultNum.SelectedValue);
        string webcoll = this.tb_WebCollection.Text;
        // ensure the foot has the same values
        this.tb_UserQuery2.Text = query;
        this.dl_ResultNum2.SelectedValue = size.ToString();
        this.tb_WebCollection2.Text = webcoll;
        // call the search function
        this.Search(query, size, webcoll);
        // update the compare link
        this.UpdateCompareLink(query);
    }
    protected void btn_Search2_Click(object sender, ImageClickEventArgs e)
    {
        string query = this.tb_UserQuery2.Text;
        int size = int.Parse(this.dl_ResultNum2.SelectedValue);
        string webcoll = this.tb_WebCollection2.Text;
        // ensure the head has the same values
        this.tb_UserQuery.Text = query;
        this.dl_ResultNum.SelectedValue = size.ToString();
        this.tb_WebCollection.Text = webcoll;
        // call the search function
        this.Search(query, size, webcoll);
        // update the compare link
        this.UpdateCompareLink(query);
    }
    protected void UpdateCompareLink(string query)
    {
        this.lnk_Compare.NavigateUrl = "Compare.aspx";
        this.lnk_Compare2.NavigateUrl = "Compare.aspx";
        if (query != null && query.Trim().Length != 0)
        {
            string s = "?search=" + HttpUtility.UrlEncode(query);
            this.lnk_Compare.NavigateUrl += s;
            this.lnk_Compare2.NavigateUrl += s;
        }
    }

    #region Call Search Function

    protected void Search(string query, int size, string webcollection)
    {
        // 1. update the web collection name and searcher
        if (this.WebCollectionName != webcollection ||
            Helper.Searcher == null)
        {
            this.WebCollectionName = webcollection;
            // get the data source according to web collection name
            Helper.Searcher = DataRetriever.GetWebCollection(webcollection);
        }
        if (Helper.SogouResultXml == null)
        {
            XmlDocument _sogouResultXml = new XmlDocument();
            _sogouResultXml.Load(Server.MapPath("searchResults.xml"));
            Helper.SogouResultXml = _sogouResultXml;
        }
        
        // 2. search the query for result list
        SearchResultList results = Helper.Searcher.Search(query, size);
        this.BindSearchResult(results);
    }

    protected void BindSearchResult(SearchResultList searchResults)
    {
        this.dtl_SearchResults.DataSource = searchResults;
        this.dtl_SearchResults.DataBind();
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
}
