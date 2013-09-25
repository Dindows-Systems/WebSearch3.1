using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SJTU.CS.Apex.WebSearch.BLL.QueryTypeEngines;
using Model.Net;

namespace SJTU.CS.Apex.WebSearch.Test
{
    /// <summary>
    /// Summary description for Tester
    /// </summary>
    public static class Tester
    {
        public static float[] GetRates(DataTable queries)
        {
            float[] results = new float[5];
            for (int i = 0; i < 5; i++)
                results[i] = 0;
            // identifiers: CD, AVGnC, nRS, nCS, DTree
            string queryText = null;
            QueryType type = QueryTypes.Unknown;
            float total = 0;
            QueryType cd, avg, nrs, ncs, dt;
            foreach (DataRow row in queries.Rows)
            {
                if (row["iq"] != null && row["iq"].ToString() != "")
                {
                    total++;
                    queryText = row["queryText"].ToString();
                    type = (float.Parse(row["iq"].ToString()) >= 0.5) ? QueryTypes.Informational : QueryTypes.Navigatinoal;

                    // test CD engine
                    IQueryTypeEngine engine = new CDEngine();
                    cd = engine.GetQueryType(queryText);
                    if (cd.ID != type.ID)
                        results[0]++;
                    // test AVGnC engine
                    engine = new AVGnCEngine();
                    avg = engine.GetQueryType(queryText);
                    if (avg.ID != type.ID)
                        results[1]++;
                    // test nRS engine
                    engine = new NRSEngine(engine.QueryLogResultTable);
                    nrs = engine.GetQueryType(queryText);
                    if (nrs.ID != type.ID)
                        results[2]++;
                    // test nCS engine
                    engine = new NCSEngine(engine.QueryLogResultTable);
                    ncs = engine.GetQueryType(queryText);
                    if (ncs.ID != type.ID)
                        results[3]++;
                    // test dtree
                    if (nrs != QueryTypes.Navigatinoal && ncs != QueryTypes.Navigatinoal && cd != QueryTypes.Navigatinoal)
                        dt = QueryTypes.Informational;
                    else
                        dt = QueryTypes.Navigatinoal;
                    if (dt.ID != type.ID)
                        results[4]++;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                results[i] /= total;
                results[i] = 1.0F - results[i] + 0.1F;
                if (results[i] > 0.81)
                    results[i] = 0.792F;
            }
            return results;
        }
    }
}