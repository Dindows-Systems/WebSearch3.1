using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using WebSearch.DataCenter.Net;
using System.Data.SqlClient;

namespace ToolApplication1
{
    class Program
    {
        public const string QueryFileName = "SogouQueries.txt";
        public static string UrlFileName = "UrlsForQ";

        static void Main(string[] args)
        {
            StreamWriter writer = new StreamWriter("C:\\1000Queries.txt", true, Encoding.GetEncoding("gb2312"));
            SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=QuerySurvey;User ID=sa;Password=111111");
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT queryText FROM [SogouQCollection1] WHERE (deleted=0 OR deleted IS NULL) AND queryType IS NOT NULL";
            //cmd.Parameters.Add("@queryText", System.Data.SqlDbType.NVarChar, 500);
            SqlDataReader reader = cmd.ExecuteReader();
            string query;
            int i = 0;
            while (reader != null && reader.Read())
            {
                query = reader["queryText"].ToString();
                writer.WriteLine(query);
            }
            conn.Close();
            writer.Close();
        }

        private void f()
        {
             int min = int.Parse(Console.ReadLine());
            int max = int.Parse(Console.ReadLine());
            UrlFileName += min.ToString() + "-" + max.ToString() + ".txt";

            List<string> urlTable = new List<string>();

            StreamReader reader = new StreamReader(QueryFileName, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(UrlFileName);
            
            IWebCollection webCollection = null;
            string query = reader.ReadLine();
            string[] tempurls = null;
            int count = 1;
            while (query != null)
            {
                if (count >= min && count <= max)
                {
                    try
                    {
                        // get google urls
                        webCollection = DataRetriever.GetWebCollection("google");
                        tempurls = webCollection.SearchUrls(query, 20);
                        if (tempurls != null)
                        {
                            for (int i = 0; i < tempurls.Length; i++)
                            {
                                if (!urlTable.Contains(tempurls[i]))
                                {
                                    writer.WriteLine(tempurls[i]);
                                    urlTable.Add(tempurls[i]);
                                }
                            }
                        }
                        else
                            Console.WriteLine(query + "google");
                        }
                    catch (Exception ex)
                    {
                        Console.WriteLine(query + "google");
                    }
                    try
                    {
                        // get baidu urls
                        webCollection = DataRetriever.GetWebCollection("baidu");
                        tempurls = webCollection.SearchUrls(query, 20);
                        if (tempurls != null)
                        {
                            for (int i = 0; i < tempurls.Length; i++)
                            {
                                if (!urlTable.Contains(tempurls[i]))
                                {
                                    writer.WriteLine(tempurls[i]);
                                    urlTable.Add(tempurls[i]);
                                }
                            }
                        }
                        else
                            Console.WriteLine(query + "baidu");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(query + "baidu");
                    }
                    try
                    {
                        // get sogou urls
                        webCollection = DataRetriever.GetWebCollection("sogou");
                        tempurls = webCollection.SearchUrls(query, 20);
                        if (tempurls != null)
                        {
                            for (int i = 0; i < tempurls.Length; i++)
                            {
                                if (!urlTable.Contains(tempurls[i]))
                                {
                                    writer.WriteLine(tempurls[i]);
                                    urlTable.Add(tempurls[i]);
                                }
                            }
                        }
                        Console.WriteLine(query + "sogou");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(query + "sogou");
                    }

                    if (count % 100 == 0)
                    {
                        Console.Write(count + "|");
                        writer.Flush();
                    }
                }
                count++;
                if (count > max)
                    break;
                query = reader.ReadLine();
            }
            reader.Close();
            writer.Close();
        
        }
    }

    
}
