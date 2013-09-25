using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using SJTU.CS.Apex.WebSearch.BLL.AutoCompleteEngines;


/// <summary>
/// Summary description for QueryAutoComplete
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class QueryAutoCompleteSrv : System.Web.Services.WebService
{
    public QueryAutoCompleteSrv()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string[] GetCompletionList(string prefixText, int count)
    {
        IAutoCompleteEngine engine = new TopRankEngine();
        return engine.GetCompleteQueries(prefixText, count);
    }
}

