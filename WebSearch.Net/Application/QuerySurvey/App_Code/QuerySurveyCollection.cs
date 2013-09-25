using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using WebSearch.Model.Net;
using System.Collections.Generic;
using System.Data.SqlClient;
using WebSearch.Maths.Net;

/// <summary>
/// Summary description for QuerySurvey
/// </summary>
public class QuerySurveyCollection
{
    #region Static Methods

    protected static SqlConnection GetConnection()
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.
            ConnectionStrings["QuerySurveyConnectionString"].ConnectionString);
        conn.Open();
        return conn;
    }

    /// <summary>
    /// Get all query collections for survey
    /// </summary>
    /// <returns></returns>
    public static IList<QuerySurveyCollection> GetQuerySurveyCollections()
    {
        SqlConnection conn = GetConnection();
        SqlCommand cmd = new SqlCommand("SELECT * FROM [TableCatalog]", conn);
        SqlDataReader reader = cmd.ExecuteReader();

        List<QuerySurveyCollection> results = new List<QuerySurveyCollection>();
        while (reader != null && reader.Read())
        {
            results.Add(new QuerySurveyCollection(
                int.Parse(reader["ID"].ToString()),
                reader["tableName"].ToString(), 
                (Type)int.Parse(reader["tableType"].ToString())));
        }
        conn.Close();
        return results;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static QuerySurveyCollection CreateQuerySurveyCollection(string name, Type type)
    {
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool DeleteQuerySurveyCollection(string name)
    {
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool DeleteQuerySurveyCollection(int id)
    {
        return true;
    }

    #endregion

    #region Properties

    private int _id;

    public int ID
    {
        get { return _id; }
    }

    private string _name;

    public string Name
    {
        get { return _name; }
    }

    public enum Type
    {
        Train = 1, Test = 2
    }

    private Type _type;

    public Type CollectionType
    {
        get { return _type; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor for QuerySurveyCollection
    /// </summary>
    /// <param name="id"></param>
    public QuerySurveyCollection(int id)
    {
        SqlConnection conn = GetConnection();
        // search the db's catalog for the collection
        SqlCommand cmd = new SqlCommand("SELECT * FROM [TableCatalog] WHERE ID=@ID", conn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        // get the query survey collection info 
        SqlDataReader reader = cmd.ExecuteReader();
        if (reader != null && reader.Read())
        {
            this._id = id; this._name = reader["tableName"].ToString();
            this._type = (Type)int.Parse(reader["tableType"].ToString());
            reader.Close(); conn.Close();
        }
        else
        {
            conn.Close();
            throw new Exception("Invalid query survey collection id");
        }
    }

    /// <summary>
    /// Constructor for QuerySurveyCollection
    /// </summary>
    /// <param name="name"></param>
    public QuerySurveyCollection(string name)
    {
        SqlConnection conn = GetConnection();
        SqlCommand cmd = new SqlCommand("SELECT * FROM [TableCatalog] " +
            "WHERE tableName=@TableName", conn);
        cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 100).Value = name;
        // get the query survey collection info
        SqlDataReader reader = cmd.ExecuteReader();
        if (reader != null && reader.Read())
        {
            this._id = int.Parse(reader["ID"].ToString());
            this._name = name;
            this._type = (Type)int.Parse(reader["tableType"].ToString());
            reader.Close(); conn.Close();
        }
        else
        {
            conn.Close();
            throw new Exception("Invalid query survey collection name");
        }
    }

    public QuerySurveyCollection(int id, string name, Type type)
    {
        this._id = id;
        this._name = name;
        this._type = type;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Add a survey for the query survey collection
    /// </summary>
    /// <param name="queryID"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool AddSurvey(int queryID, QueryType type)
    {
        // add the survey record into ...Survey
        SqlConnection conn = GetConnection();
        SqlCommand cmd = new SqlCommand("", conn);
        try
        {
            // insert the survey record
            cmd.CommandText = "INSERT INTO [" + _name + ".Survey] (queryID,queryType) " +
                "VALUES (" + queryID + "," + type.ID + ")";
            if (cmd.ExecuteNonQuery() != 1)
                return false;

            // calculate the queryType according to total and info
            cmd.CommandText = "SELECT COUNT(ID) FROM [" + _name + ".Survey] WHERE " +
                "queryID=" + queryID;
            int total = int.Parse(cmd.ExecuteScalar().ToString());
            if (total == 0) return false;

            cmd.CommandText = "SELECT COUNT(ID) FROM [" + _name + ".Survey] WHERE " +
                "queryID=" + queryID + " AND queryType=@queryType";
            cmd.Parameters.Add("@queryType", SqlDbType.SmallInt);

            // calculate informational, navigational, transactional num
            cmd.Parameters["@queryType"].Value = QueryType.Informational.ID;
            int info = int.Parse(cmd.ExecuteScalar().ToString());
            cmd.Parameters["@queryType"].Value = QueryType.Navigatinoal.ID;
            int navi = int.Parse(cmd.ExecuteScalar().ToString());
            int trac = total - info - navi;

            cmd.CommandText = "UPDATE [" + _name + "] SET queryType=@queryType," +
                "dvariance=@dvariance WHERE queryID=" + queryID;
            cmd.Parameters.Add("@dvariance", SqlDbType.Float);

            if (info >= navi && info >= trac) // informational query
            {
                cmd.Parameters["@queryType"].Value = QueryType.Informational.ID;
                cmd.Parameters["@dvariance"].Value = 1F / Statistics.Variance(info,
                    new int[] { navi, trac });
                // update the trainset's queryType value
                cmd.ExecuteNonQuery();
                return true;
            }
            if (navi >= info && navi >= trac) // navigaional query
            {
                cmd.Parameters["@queryType"].Value = QueryType.Navigatinoal.ID;
                cmd.Parameters["@dvariance"].Value = 1F / Statistics.Variance(navi,
                    new int[] { info, trac });
            }
            else // transactioal query
            {
                cmd.Parameters["@queryType"].Value = QueryType.Transactional.ID;
                cmd.Parameters["@dvariance"].Value = 1F / Statistics.Variance(trac,
                    new int[] { info, navi });
            }
            // update the trainset's queryType value
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (SqlException ex)
        {
            conn.Close();
            throw ex;
        }
    }

    public DataTable GetQueryTypeDistribution(QueryType type, float step)
    {
        step = (step <= 0) ? 0.5F : step;

        int rowNum = 10;

        DataTable result = new DataTable();
        result.Columns.Add("dvariance");
        result.Columns.Add("queryNum");
        DataRow row = null;

        SqlConnection conn = GetConnection();
        SqlCommand cmd = new SqlCommand("SELECT COUNT(queryID) FROM [" + _name + "] WHERE " +
            "(deleted=0 OR deleted IS NULL) AND queryType=@queryType AND " +
            "dvariance>=@left AND dvariance<@right", conn);
        cmd.Parameters.Add("@queryType", SqlDbType.SmallInt).Value = type.ID;
        cmd.Parameters.Add("@left", SqlDbType.Float);
        cmd.Parameters.Add("@right", SqlDbType.Float);

        float left = 0, right = step;
        try
        {
            for (int i = 0; i < rowNum; i++, left = right, right += step)
            {
                row = result.NewRow();
                row["dvariance"] = (i == rowNum - 1) ?
                    "[" + left + ",...)" :
                    "[" + left + "," + right + ")";

                cmd.Parameters["@left"].Value = left;
                cmd.Parameters["@right"].Value =
                    (i == rowNum - 1) ? 1000 : right;
                row["queryNum"] = cmd.ExecuteScalar();
                result.Rows.Add(row);
            }
            return result;
        }
        catch (SqlException ex)
        {
            conn.Close();
            throw ex;
        }
    }

    /// <summary>
    /// Delete a certain query in the query survey collection
    /// </summary>
    /// <param name="queryID"></param>
    /// <returns></returns>
    public bool DeleteQuery(int queryID)
    {
        // set the query's state as deleted
        SqlConnection conn = GetConnection();
        SqlCommand cmd = new SqlCommand("UPDATE [" + _name + 
            "] SET deleted=1 WHERE queryID=" + queryID, conn);
        cmd.ExecuteNonQuery();
        conn.Close();
        return true;
    }

    /// <summary>
    /// Delete a certain query in the query survey collection
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public bool DeleteQuery(string query)
    {
        // set the query's state as deleted
        SqlConnection conn = GetConnection();
        SqlCommand cmd = new SqlCommand("UPDATE [" + _name + 
            "] SET deleted=1 WHERE queryText=" + query, conn);
        cmd.ExecuteNonQuery();
        conn.Close();
        return true;
    }

    /// <summary>
    /// Get the entire query collection
    /// </summary>
    /// <returns></returns>
    public DataTable GetQueries()
    {
        SqlConnection conn = GetConnection();

        DataSet set = new DataSet("QueryCollection");
        SqlDataAdapter adapter = new SqlDataAdapter();
        adapter.SelectCommand = new SqlCommand(
            "SELECT * FROM [" + _name + "] WHERE deleted=0 OR deleted IS NULL", conn);
        adapter.Fill(set, "QueryCollection");
        return set.Tables[0];
    }

    #endregion
}
