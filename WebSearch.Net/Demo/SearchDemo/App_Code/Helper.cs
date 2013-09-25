using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using WebSearch.DataCenter.Net;
using System.Xml;

/// <summary>
/// Summary description for SearchHelper
/// </summary>
public class Helper
{
    private static IWebCollection _searcher = null;
    private static IQueryLog _sogouQueryLog = null;
    private static XmlDocument _sogouResultXml = null;

    public static IWebCollection Searcher
    {
        get { return _searcher; }
        set { _searcher = value; }
    }

    public static IQueryLog SogouQueryLog
    {
        get { return _sogouQueryLog; }
        set { _sogouQueryLog = value; }
    }

    public static XmlDocument SogouResultXml
    {
        get { return _sogouResultXml; }
        set { _sogouResultXml = value; }
    }
}
