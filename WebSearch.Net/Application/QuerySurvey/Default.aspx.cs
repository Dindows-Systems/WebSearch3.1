using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Drawing;
using WebSearch.Model.Net;

public partial class Default : System.Web.UI.Page
{
    #region Properties

    private QuerySurveyCollection _querySurveyColl = null;

    public QuerySurveyCollection QuerySurveyColl
    {
        get
        {
            if (this._querySurveyColl == null)
            {
                _querySurveyColl = new QuerySurveyCollection(
                    CurrentLoadedQueryCollectionName);
            }
            return _querySurveyColl;
        }
        set { _querySurveyColl = value; }
    }

    private DataTable _queryCollectionDataTable = null;

    private DataTable QueryCollectionDataTable
    {
        get
        {
            // late initialization of the train set data table
            if (this._queryCollectionDataTable == null)
            {
                QuerySurveyCollection col = new QuerySurveyCollection(
                    CurrentLoadedQueryCollectionName);
                this._queryCollectionDataTable = col.GetQueries();
            }
            return this._queryCollectionDataTable;
        }
        set { _queryCollectionDataTable = value; }
    }

    private string CurrentLoadedQueryCollectionName
    {
        get { return (string)ViewState["CurrentLoadedQCName"]; }
        set { ViewState["CurrentLoadedQCName"] = value; }
    }

    private Hashtable JudgedQueries
    {
        get
        {
            // get the judged logs data from view state
            if (ViewState["judgedtrainqueries"] == null)
            {
                Hashtable _judgedLogs = new Hashtable();
                ViewState["judgedtrainqueries"] = _judgedLogs;
            }
            return (Hashtable)ViewState["judgedtrainqueries"];
        }
        set { ViewState["judgedtrainqueries"] = value; }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.LoadQueryCollections();    // load all the specific testsets
        }
        if (this.dl_QueryCollections.SelectedIndex == -1)
            this.lnk_Charting.Visible = false;
    }

    /* ------------------------ For Query Collection ------------------------*/

    protected void LoadQueryCollections()
    {
        this.dl_QueryCollections.Items.Clear();

        this.dl_QueryCollections.DataSource = QuerySurveyCollection.GetQuerySurveyCollections();
        this.dl_QueryCollections.DataValueField = "ID";
        this.dl_QueryCollections.DataTextField = "Name";
        this.dl_QueryCollections.DataBind();
    }

    /* ------------------------ For Query Collection Data ------------------------*/
    protected void LoadQueryCollectionData()
    {
        if (dl_QueryCollections.SelectedIndex == -1 || dl_QueryCollections.Items.Count == 0)
            return;

        this.gdv_QueryCollection.DataSource = this.QueryCollectionDataTable;
        this.gdv_QueryCollection.DataKeyNames = new String[] { "queryID" };

        if (ViewState["sortexpression"] != null && ViewState["sortexpression"].ToString() != "")
            this.QueryCollectionDataTable.DefaultView.Sort = 
                ViewState["sortexpression"].ToString() + " "
                + ViewState["sortdirection"].ToString();

        this.gdv_QueryCollection.DataBind();
    }
    protected void gdv_QueryCollecion_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string type = e.Row.Cells[2].Text;
            e.Row.Cells[2].Text = (type != "" && type != "N/A") ?
                ((QueryType)int.Parse(type)).Name : "<font color='red'>N/A</font>";

            if (this.JudgedQueries[e.Row.Cells[0].Text] != null)
            {
                RadioButtonList radios = (RadioButtonList)e.Row.Cells[3].
                    FindControl("RadioButtonListView");
                radios.SelectedValue = this.JudgedQueries[e.Row.Cells[0].Text].ToString();
                e.Row.Cells[5].Enabled = false;
            }
        }
    }
    protected void gdv_QueryCollecion_DataBound(object sender, EventArgs e)
    {
        if (this.gdv_QueryCollection.Rows.Count == 0)
        {
            this.lnk_Charting.Visible = false;
        }
        else
        {
            // show the query collection size
            this.gdv_QueryCollection.FooterRow.Cells[1].Text = "Total: " + 
                QueryCollectionDataTable.Rows.Count.ToString() + " querie(s)";
        }
    }
    protected void gdv_QueryCollecion_RowEditing(object sender, GridViewEditEventArgs e)
    {
        this.gdv_QueryCollection.EditIndex = e.NewEditIndex;
        this.LoadQueryCollectionData();
    }
    protected void gdv_QueryCollection_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int queryID = (int)gdv_QueryCollection.DataKeys[e.RowIndex].Value;
        QuerySurveyColl.DeleteQuery(queryID);
        QueryCollectionDataTable = null; // need to update the query collection dt
        this.LoadQueryCollectionData();
    }
    protected void gdv_QueryCollecion_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        // add the manual judgement result into db
        RadioButtonList radiolist = (RadioButtonList)gdv_QueryCollection.
            Rows[e.RowIndex].FindControl("RadioButtonListEdit");

        // if there's something chosen in the radio button list
        if (radiolist.SelectedIndex >= 0)
        {
            int queryID = int.Parse(this.gdv_QueryCollection.DataKeys[e.RowIndex].Value.ToString());
            int type = int.Parse(radiolist.SelectedValue);
            // insert it into db.Survey (it will automatically update the i(q)
            QuerySurveyColl.AddSurvey(queryID, (QueryType)type);
            // update the judge record on client side
            this.JudgedQueries.Add(queryID.ToString(), type.ToString());
        }
        // rebind the training set data table
        this.gdv_QueryCollection.EditIndex = -1;
        this.LoadQueryCollectionData();
    }
    protected void gdv_QueryCollecion_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        this.gdv_QueryCollection.EditIndex = -1;
        this.LoadQueryCollectionData();
    }
    protected void gdv_QueryCollecion_Sorting(object sender, GridViewSortEventArgs e)
    {
        // when sorting, selected index set as null
        this.gdv_QueryCollection.SelectedIndex = -1;

        ViewState["sortexpression"] = e.SortExpression;

        if (ViewState["sortdirection"] == null)
        {
            ViewState["sortdirection"] = "ASC";
        }
        else
        {
            if (ViewState["sortdirection"].ToString() == "ASC")
                ViewState["sortdirection"] = "DESC";
            else
                ViewState["sortdirection"] = "ASC";
        }
        this.FollowUpGridView();
    }
    protected void gdv_QueryCollecion_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // when sorting, selected index set as null
        this.gdv_QueryCollection.SelectedIndex = -1;

        // reset the current grid view page idex
        this.gdv_QueryCollection.PageIndex = e.NewPageIndex;
        this.FollowUpGridView();
    }
    protected void FollowUpGridView()
    {
        DataView _dataView = new DataView(this.QueryCollectionDataTable);

        // set the sort expression
        if (ViewState["sortexpression"] != null && 
            ViewState["sortexpression"].ToString() != "")
            _dataView.Sort = ViewState["sortexpression"].ToString() + 
                " " + ViewState["sortdirection"].ToString();
        
        // set the filter

        this.gdv_QueryCollection.DataSource = _dataView;
        this.gdv_QueryCollection.DataBind();
    }

    protected void btn_Load_Click(object sender, ImageClickEventArgs e)
    {
        if (CurrentLoadedQueryCollectionName != dl_QueryCollections.SelectedItem.Text)
        {
            CurrentLoadedQueryCollectionName = dl_QueryCollections.SelectedItem.Text;
            this.QueryCollectionDataTable = null;
            this.JudgedQueries = null;
        }

        this.LoadQueryCollectionData();
        // update the url of charting
        if (this.dl_QueryCollections.SelectedIndex != -1)
        {
            this.lnk_Charting.Visible = true;
            this.lnk_Charting.NavigateUrl = "javascript:charting('" +
                this.dl_QueryCollections.SelectedItem.Text + "')";
        }
    }
    //protected void btn_New_Click(object sender, EventArgs e)
    //{
    //    // try to create the test set according to the user setting
    //    if (!DALHelper.GetAnalysisDAO().CreateTrainSet( 
    //        this.tb_TrainSetName.Text,                  // the training set name
    //        int.Parse(this.tb_QueryLogCount.Text),      // the count of items in the train set
    //        (QueryLog)int.Parse(this.dl_ChooseQueryLogDBs.SelectedValue)))
    //    {
    //        // if it failed in creating the test set
    //        Response.Write("<script>alert('Sorry, the operation to new a training set failed.');</script>");
    //    }
    //    // temporarily store the current select of 
    //    string past = this.dl_TrainingSets.SelectedValue;
    //    this.LoadTrainingSets();
    //    this.dl_TrainingSets.SelectedValue = past;
    //}
}
