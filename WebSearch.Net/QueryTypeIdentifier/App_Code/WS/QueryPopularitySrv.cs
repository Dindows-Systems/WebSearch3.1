using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;


/// <summary>
/// Summary description for QueryPopularity
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class QueryPopularitySrv : System.Web.Services.WebService
{

    public QueryPopularitySrv()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public int GetPopularity(string query)
    {
        int length = query.Length;
        return (length < 9) ? length : 9;
    }

}

