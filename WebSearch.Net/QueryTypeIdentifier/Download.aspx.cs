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

public partial class Download : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request["type"] == null || Request["name"] == null)
            this.ErrorDownload();

        // get the dir and file name
        string dir = Request["type"].ToString();
        string name = Request["name"].ToString();

        if (dir == "1")
            dir = "SourceCodes";
        else if (dir == "2")
            dir = "ResearchPapers";
        else if (dir == "3")
            dir = "ProjectDocs";
        else
            this.ErrorDownload();

        string fileName = "App_GlobalResources/" + dir + "/" + name;
        if (!File.Exists(Server.MapPath(fileName)))
            this.ErrorDownload();
        else
        {
            Response.Redirect(fileName);
            Response.Write("<script>window.close();</script>");
        }
    }

    private void ErrorDownload()
    {
        Response.Write("<center><p>&nbsp;</p><p>&nbsp;</p><p>&nbsp;</p><p>&nbsp;</p>" +
            "<p>&nbsp;</p><p><b>Sorry, the download " +
            "could not be accessed!</b></p></center>");
    }
}
