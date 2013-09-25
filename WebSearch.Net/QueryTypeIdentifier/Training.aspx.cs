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
using SJTU.CS.Apex.WebSearch.Util;
using SJTU.CS.Apex.WebSearch.DAL;
using System.IO;
using SJTU.CS.Apex.WebSearch.Model;
using System.Drawing;
using Model.Net;

public partial class Training : System.Web.UI.Page
{
    #region Properties

    private DataTable _trainSetDataTable = null;

    private DataTable TrainSetDataTable
    {
        get
        {
            // late initialization of the train set data table
            if (this._trainSetDataTable == null)
                this._trainSetDataTable = DALHelper.GetAnalysisDAO().
                    GetTrainSetData(CurrentLoadedTrainSetName);
            return this._trainSetDataTable;
        }
        set { _trainSetDataTable = value; }
    }

    private string CurrentLoadedTrainSetName
    {
        get { return (string)ViewState["CurrentLoadedTrainSetName"]; }
        set { ViewState["CurrentLoadedTrainSetName"] = value; }
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
            this.LoadQueryLogDBs();         // load all the query log dbs
            this.LoadTrainingSets();
            this.LoadSpecificDataSets();    // load all the specific testsets
        }
        if (this.dl_TrainingSets.SelectedIndex == -1)
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
        this.LoadTrainingSets();
    }

    protected void LoadSpecificDataSets()
    {
        this.dl_SpecificTrainsets.Items.Clear();

        this.dl_SpecificTrainsets.DataSource = DALHelper.GetAnalysisDAO().GetTrainSets(null);
        this.dl_SpecificTrainsets.DataValueField = "ID";
        this.dl_SpecificTrainsets.DataTextField = "tableName";
        this.dl_SpecificTrainsets.DataBind();
    }

    /* ------------------------ For Training Set list ------------------------*/
    protected void LoadTrainingSets()
    {
        this.dl_TrainingSets.Items.Clear();

        this.dl_TrainingSets.DataSource = DALHelper.GetAnalysisDAO().
            GetTrainSets((QueryLog)int.Parse(dl_QueryLogDBs.SelectedValue));
        this.dl_TrainingSets.DataValueField = "ID";
        this.dl_TrainingSets.DataTextField = "tableName";
        this.dl_TrainingSets.DataBind();

        //if (dl_TrainingSets.Items.Count > 0)
        //    this.dl_TrainingSets.SelectedIndex = 0;
    }

    /* ------------------------ For Training Set Data ------------------------*/
    protected void LoadTrainingSetData()
    {
        if (dl_TrainingSets.SelectedIndex == -1 || dl_TrainingSets.Items.Count == 0)
            return;

        this.gdv_TrainItems.DataSource = this.TrainSetDataTable;
        this.gdv_TrainItems.DataKeyNames = new String[] { "queryID" };

        if (ViewState["sortexpression"] != null && ViewState["sortexpression"].ToString() != "")
            this.TrainSetDataTable.DefaultView.Sort = 
                ViewState["sortexpression"].ToString() + " "
                + ViewState["sortdirection"].ToString();

        this.gdv_TrainItems.DataBind();
    }
    protected void gdv_TrainItems_RowDataBound(object sender, GridViewRowEventArgs e)
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
    protected void gdv_TrainItems_DataBound(object sender, EventArgs e)
    {
        if (this.gdv_TrainItems.Rows.Count == 0)
        {
            this.lnk_Charting.Visible = false;
        }
    }
    protected void gdv_TrainItems_RowEditing(object sender, GridViewEditEventArgs e)
    {
        this.gdv_TrainItems.EditIndex = e.NewEditIndex;
        this.LoadTrainingSetData();
    }
    protected void gdv_TrainItems_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        // add the manual judgement result into db
        RadioButtonList radiolist = (RadioButtonList)gdv_TrainItems.
            Rows[e.RowIndex].FindControl("RadioButtonListEdit");

        // if there's something chosen in the radio button list
        if (radiolist.SelectedIndex >= 0)
        {
            int queryID = int.Parse(this.gdv_TrainItems.DataKeys[e.RowIndex].Value.ToString());
            int type = int.Parse(radiolist.SelectedValue);
            // insert it into db.Survey (it will automatically update the i(q)
            DALHelper.GetAnalysisDAO().AddSurvey(CurrentLoadedTrainSetName,
                queryID, (QueryType)type);
            // update the judge record on client side
            this.JudgedQueries.Add(queryID.ToString(), type.ToString());
        }
        // rebind the training set data table
        this.gdv_TrainItems.EditIndex = -1;
        this.LoadTrainingSetData();
    }
    protected void gdv_TrainItems_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        this.gdv_TrainItems.EditIndex = -1;
        this.LoadTrainingSetData();
    }
    protected void gdv_TrainItems_Sorting(object sender, GridViewSortEventArgs e)
    {
        // when sorting, selected index set as null
        this.gdv_TrainItems.SelectedIndex = -1;

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
    protected void gdv_TrainItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // when sorting, selected index set as null
        this.gdv_TrainItems.SelectedIndex = -1;

        // reset the current grid view page idex
        this.gdv_TrainItems.PageIndex = e.NewPageIndex;
        this.FollowUpGridView();
    }
    protected void FollowUpGridView()
    {
        DataView _dataView = new DataView(this.TrainSetDataTable);

        // set the sort expression
        if (ViewState["sortexpression"] != null && 
            ViewState["sortexpression"].ToString() != "")
            _dataView.Sort = ViewState["sortexpression"].ToString() + 
                " " + ViewState["sortdirection"].ToString();
        
        // set the filter

        this.gdv_TrainItems.DataSource = _dataView;
        this.gdv_TrainItems.DataBind();
    }
    
    protected void btn_Load_Click(object sender, ImageClickEventArgs e)
    {
        this.panel_QueryLogs.BackColor = Color.WhiteSmoke;
        this.panel_Specific.BackColor = Color.Transparent;

        if (CurrentLoadedTrainSetName != dl_TrainingSets.SelectedItem.Text)
        {
            CurrentLoadedTrainSetName = dl_TrainingSets.SelectedItem.Text;
            this.TrainSetDataTable = null;
            this.JudgedQueries = null;
        }

        this.LoadTrainingSetData();
        // update the url of charting
        if (this.dl_TrainingSets.SelectedIndex != -1)
        {
            this.lnk_Charting.Visible = true;
            this.lnk_Charting.NavigateUrl = "javascript:charting('" +
                this.dl_TrainingSets.SelectedItem.Text + "')";
        }
    }
    protected void btn_Load2_Click(object sender, ImageClickEventArgs e)
    {
        this.panel_Specific.BackColor = Color.WhiteSmoke;
        this.panel_QueryLogs.BackColor = Color.Transparent;

        if (CurrentLoadedTrainSetName != dl_SpecificTrainsets.SelectedItem.Text)
        {
            CurrentLoadedTrainSetName = dl_SpecificTrainsets.SelectedItem.Text;
            this.TrainSetDataTable = null;
            this.JudgedQueries = null;
        }

        this.LoadTrainingSetData();
        // update the url of charting
        if (this.dl_SpecificTrainsets.SelectedIndex != -1)
        {
            this.lnk_Charting.Visible = true;
            this.lnk_Charting.NavigateUrl = "javascript:charting('" +
                this.dl_SpecificTrainsets.SelectedItem.Text + "')";
        }
    }
    protected void btn_New_Click(object sender, EventArgs e)
    {
        // try to create the test set according to the user setting
        if (!DALHelper.GetAnalysisDAO().CreateTrainSet( 
            this.tb_TrainSetName.Text,                  // the training set name
            int.Parse(this.tb_QueryLogCount.Text),      // the count of items in the train set
            (QueryLog)int.Parse(this.dl_ChooseQueryLogDBs.SelectedValue)))
        {
            // if it failed in creating the test set
            Response.Write("<script>alert('Sorry, the operation to new a training set failed.');</script>");
        }
        // temporarily store the current select of 
        string past = this.dl_TrainingSets.SelectedValue;
        this.LoadTrainingSets();
        this.dl_TrainingSets.SelectedValue = past;
    }
}
