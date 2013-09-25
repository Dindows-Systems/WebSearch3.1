using System;
using System.Data;
using System.Configuration;

namespace WebSearch.DataCenter.Net.MSSQL
{
    /// <summary>
    /// Summary description for QueryTextFilter
    /// </summary>
    public class MSSQLQueryFilter
    {
        public static String Filter(String query)
        {
            query = query.Replace("\"", "").
                Replace(" ", "\" & \"").
                Replace(",", "\" & \"").
                Replace("+", "\" & \"");
            return "\"" + query + "\"";
        }
    }
}