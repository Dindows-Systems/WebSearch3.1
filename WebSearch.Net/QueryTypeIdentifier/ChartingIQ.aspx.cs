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
using WebChart;
using SJTU.CS.Apex.WebSearch.DAL;
using System.Drawing;
using SJTU.CS.Apex.WebSearch.Model;
using Model.Net;

public partial class ChartingIQ : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Request["setName"] == null)
                return;
            String setName = Request["setName"].ToString();
            if (setName == "")
                return;

            this.DrawWebChart(setName);
        }
    }

    private void DrawWebChart(string datasetName)
    {
        this.ChartControl.Charts.Clear();
        
        // for the informational query distribution
        ColumnChart informational = new ColumnChart();
        informational.DataXValueField = "dvariance";
        informational.DataYValueField = "queryNum";
        informational.DataLabels.Visible = true;
        informational.Fill.Color = Color.Blue;
        informational.Legend = "Informational Query Distribution";
        // get the data view to be shown on the chart
        informational.DataSource = DALHelper.GetAnalysisDAO().
            GetDataSetQueryTypeDistribution(datasetName, 
            QueryTypes.Informational, 0.5F).DefaultView;
        informational.DataBind();
        this.ChartControl.Charts.Add(informational);

        // for the navigational query distribution
        ColumnChart navigational = new ColumnChart();
        navigational.DataXValueField = "dvariance";
        navigational.DataYValueField = "queryNum";
        navigational.DataLabels.Visible = true;
        navigational.Fill.Color = Color.Red;
        navigational.Legend = "Navigational Query Distribution";
        // get the data view to be shown on the chart
        navigational.DataSource = DALHelper.GetAnalysisDAO().
            GetDataSetQueryTypeDistribution(datasetName,
            QueryTypes.Navigatinoal, 0.5F).DefaultView;
        navigational.DataBind();
        this.ChartControl.Charts.Add(navigational);

        // for the transactional query distribution
        ColumnChart transactional = new ColumnChart();
        transactional.DataXValueField = "dvariance";
        transactional.DataYValueField = "queryNum";
        transactional.DataLabels.Visible = true;
        transactional.Fill.Color = Color.Green;
        transactional.Legend = "Transactional Query Distribution";
        // get the data view to be shown on the chart
        transactional.DataSource = DALHelper.GetAnalysisDAO().
            GetDataSetQueryTypeDistribution(datasetName,
            QueryTypes.Transactional, 0.5F).DefaultView;
        transactional.DataBind();
        this.ChartControl.Charts.Add(transactional);

        this.ChartControl.RedrawChart();
    }
}
