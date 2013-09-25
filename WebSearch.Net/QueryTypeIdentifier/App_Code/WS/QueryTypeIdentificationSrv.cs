using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using SJTU.CS.Apex.WebSearch.BLL;
using SJTU.CS.Apex.WebSearch.BLL.QueryTypeEngines;

/// <summary>
/// Summary description for QueryTypeIdentification
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class QueryTypeIdentificationSrv : System.Web.Services.WebService
{

    public QueryTypeIdentificationSrv()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public int GetQueryType(string query)
    {
        IQueryTypeEngine engine = new NRSEngine();
        return engine.GetQueryType(query).ID;
    }
}

