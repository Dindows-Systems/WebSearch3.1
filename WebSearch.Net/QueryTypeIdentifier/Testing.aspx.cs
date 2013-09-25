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
using SJTU.CS.Apex.WebSearch.DAL;
using SJTU.CS.Apex.WebSearch.Util;
using SJTU.CS.Apex.WebSearch.Test;
using SJTU.CS.Apex.WebSearch.Model;
using System.Drawing;
using Model.Net;

public partial class Testing : System.Web.UI.Page
{
    #region Properties

    private DataTable _testSetDataTable = null;

    private DataTable TestSetDataTable
    {
        get
        {
            // late initialization of the train set data table
            if (this._testSetDataTable == null)
                this._testSetDataTable = DALHelper.GetAnalysisDAO().
                    GetTestSetData(CurrentLoadedTestSetName);
            return this._testSetDataTable;
        }
        set { _testSetDataTable = value; }
    }

    private string CurrentLoadedTestSetName
    {
        get { return (string)ViewState["CurrentLoadedTestSetName"]; }
        set { ViewState["CurrentLoadedTestSetName"] = value; }
    }

    private Hashtable JudgedQueries
    {
        get
        {
            // get the judged logs data from view state
            if (ViewState["judgedtestqueries"] == null)
            {
                Hashtable _judgedLogs = new Hashtable();
                ViewState["judgedtestqueries"] = _judgedLogs;
            }
            return (Hashtable)ViewState["judgedtestqueries"];
        }
        set { ViewState["judgedtestqueries"] = value; }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.LoadQueryLogDBs();         // load all the query log dbs
            this.LoadTestingSets();
            this.LoadSpecificDataSets();    // load all the specific testsets
        }
        if (this.dl_TestingSets.SelectedIndex == -1)
            this.lnk_Charting.Visible = false;
    }

    /* ------------------------ For Query Log DB list ------------------------*/
    /// <summary>
    /// Load the Query Log DB data
    /// </summary>
    protected void LoadQueryLogDBs()
    {
        // one for select
        this.dl_QueryLogDBs.Items.Clear();

        this.dl_QueryLogDBs.DataSource = QueryLogs.GetQueryLogs();
        this.dl_QueryLogDBs.DataValueField = "ID";
        this.dl_QueryLogDBs.DataTextField = "Name";
        this.dl_QueryLogDBs.DataBind();

        this.dl_QueryLogDBs.SelectedValue =
            QueryLogs.Sogou.ID.ToString();

        // another for new
        this.dl_ChooseQueryLogDBs.Items.Clear();

        this.dl_ChooseQueryLogDBs.DataSource = QueryLogs.GetQueryLogs();
        this.dl_ChooseQueryLogDBs.DataValueField = "ID";
        this.dl_ChooseQueryLogDBs.DataTextField = "Name";
        this.dl_ChooseQueryLogDBs.DataBind();

        this.dl_ChooseQueryLogDBs.SelectedValue =
            QueryLogs.Sogou.ID.ToString();
    }
    protected void dl_QueryLogDBs_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.LoadTestingSets();
    }

    protected void LoadSpecificDataSets()
    {
        this.dl_SpecificTestsets.Items.Clear();

        this.dl_SpecificTestsets.DataSource = DALHelper.GetAnalysisDAO().GetTestSets(null);
        this.dl_SpecificTestsets.DataValueField = "ID";
        this.dl_SpecificTestsets.DataTextField = "tableName";
        this.dl_SpecificTestsets.DataBind();
    }

    /* ------------------------ For Training Set list ------------------------*/
    protected void LoadTestingSets()
    {
        this.dl_TestingSets.Items.Clear();

        this.dl_TestingSets.DataSource = DALHelper.GetAnalysisDAO().
            GetTestSets((QueryLog)int.Parse(dl_QueryLogDBs.SelectedValue));
        this.dl_TestingSets.DataValueField = "ID";
        this.dl_TestingSets.DataTextField = "tableName";
        this.dl_TestingSets.DataBind();

    //    if (dl_TestingSets.Items.Count > 0)
    //        this.dl_TestingSets.SelectedIndex = 0;
    }

    /* ------------------------ For Training Set Data ------------------------*/
    protected void LoadTestingSetData()
    {
        if (dl_TestingSets.SelectedIndex == -1 || dl_TestingSets.Items.Count == 0)
            return;

        this.gdv_TestItems.DataSource = this.TestSetDataTable;
        this.gdv_TestItems.DataKeyNames = new String[] { "queryID" };

        if (ViewState["sortexpression"] != null && ViewState["sortexpression"].ToString() != "")
            this.TestSetDataTable.DefaultView.Sort = 
                ViewState["sortexpression"].ToString() + " "
                + ViewState["sortdirection"].ToString();

        this.gdv_TestItems.DataBind();
    }
    protected void gdv_TestItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // when sorting, selected index set as null
        this.gdv_TestItems.SelectedIndex = -1;

        // reset the current grid view page idex
        this.gdv_TestItems.PageIndex = e.NewPageIndex;
        this.FollowUpGridView();
    }
    protected void gdv_TestItems_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        this.gdv_TestItems.EditIndex = -1;
        this.LoadTestingSetData();
    }
    protected void gdv_TestItems_RowDataBound(object sender, GridViewRowEventArgs e)
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
    protected void gdv_TestItems_DataBound(object sender, EventArgs e)
    {
        if (this.gdv_TestItems.Rows.Count == 0)
        {
            this.lnk_Charting.Visible = false;
        }
    }
    protected void gdv_TestItems_RowEditing(object sender, GridViewEditEventArgs e)
    {
        this.gdv_TestItems.EditIndex = e.NewEditIndex;
        this.LoadTestingSetData();
    }
    protected void gdv_TestItems_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        // add the manual judgement result into db
        RadioButtonList radiolist = (RadioButtonList)gdv_TestItems.
            Rows[e.RowIndex].FindControl("RadioButtonListEdit");

        // if there's something chosen in the radio button list
        if (radiolist.SelectedIndex >= 0)
        {
            int queryID = int.Parse(this.gdv_TestItems.DataKeys
                [e.RowIndex].Value.ToString());
            int type = int.Parse(radiolist.SelectedValue);
            // insert it into db.Survey (it will automatically update the i(q)
            DALHelper.GetAnalysisDAO().AddSurvey(CurrentLoadedTestSetName,
                queryID, (QueryType)type);
            // update the judge record on client side
            this.JudgedQueries.Add(queryID.ToString(), type.ToString());
        }
        // rebind the training set data table
        this.gdv_TestItems.EditIndex = -1;
        this.LoadTestingSetData();
    }
    protected void gdv_TestItems_Sorting(object sender, GridViewSortEventArgs e)
    {
        // when sorting, selected index set as null
        this.gdv_TestItems.SelectedIndex = -1;

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
    protected void FollowUpGridView()
    {
        DataView _dataView = new DataView(this.TestSetDataTable);

        // set the sort expression
        if (ViewState["sortexpression"] != null &&
            ViewState["sortexpression"].ToString() != "")
            _dataView.Sort = ViewState["sortexpression"].ToString() +
                " " + ViewState["sortdirection"].ToString();

        // set the filter

        this.gdv_TestItems.DataSource = _dataView;
        this.gdv_TestItems.DataBind();
    }

    protected void btn_Load_Click(object sender, ImageClickEventArgs e)
    {
        this.panel_QueryLogs.BackColor = Color.WhiteSmoke;
        this.panel_Specific.BackColor = Color.Transparent;

        if (CurrentLoadedTestSetName != dl_TestingSets.SelectedItem.Text)
        {
            CurrentLoadedTestSetName = dl_TestingSets.SelectedItem.Text;
            this.TestSetDataTable = null;
            this.JudgedQueries = null;
        }

        this.LoadTestingSetData();
        // update the url of charting
        if (this.dl_TestingSets.SelectedIndex != -1)
        {
            this.lnk_Charting.Visible = true;
            this.lnk_Charting.NavigateUrl = "javascript:charting('" +
                this.dl_TestingSets.SelectedItem.Text + "')";
        }
    }
    protected void btn_Load2_Click(object sender, ImageClickEventArgs e)
    {
        this.panel_Specific.BackColor = Color.WhiteSmoke;
        this.panel_QueryLogs.BackColor = Color.Transparent;

        if (CurrentLoadedTestSetName != dl_SpecificTestsets.SelectedItem.Text)
        {
            CurrentLoadedTestSetName = dl_SpecificTestsets.SelectedItem.Text;
            this.TestSetDataTable = null;
            this.JudgedQueries = null;
        }

        this.LoadTestingSetData();
        // update the url of charting
        if (this.dl_SpecificTestsets.SelectedIndex != -1)
        {
            this.lnk_Charting.Visible = true;
            this.lnk_Charting.NavigateUrl = "javascript:charting('" +
                this.dl_SpecificTestsets.SelectedItem.Text + "')";
        }
    }
    protected void btn_New_Click(object sender, EventArgs e)
    {
        // try to create the test set according to the user setting
        if (!DALHelper.GetAnalysisDAO().CreateTestSet(
            this.tb_TestSetName.Text,                   // the test name
            int.Parse(this.tb_QueryLogCount.Text),      // the count of items to be in the test set
            (QueryLog)int.Parse(this.dl_ChooseQueryLogDBs.SelectedValue)))
        {
            // if it failed in creating the test set
            Response.Write("<script>alert('Sorry, the operation to new a test set failed.');</script>");
        }
        // temporarily store the current select of 
        string past = this.dl_TestingSets.SelectedValue;
        this.LoadTestingSets();
        this.dl_TestingSets.SelectedValue = past;
    }
    protected void btn_Test_Click(object sender, ImageClickEventArgs e)
    {
        float[] rates = Tester.GetRates(this.TestSetDataTable);

        this.lbl_TestResult.Text = "CD:" + rates[0].ToString() + 
            ", AVGnC:" + rates[1].ToString() + 
            ", nRS:" + rates[2].ToString() +
            ", nCS:" + rates[3].ToString() +
            ", DTree:" + rates[4].ToString();
    }
}
