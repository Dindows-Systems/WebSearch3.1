using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.IO;
using WebSearch.Feature.Net;
using WebSearch.Model.Net;
using WebSearch.DataCenter.Net.DS;

namespace FeatureDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Phrase2();
        }

        /// <summary>
        /// 以用户query的语言学特征以及query的click through信息为主
        /// </summary>
        public static void Phrase1()
        {
            Console.Write("Query File: ");
            string queryFile = @"D:\WebSearch.Net\Data\SogouQueries.txt";//Console.ReadLine();
            if (!File.Exists(queryFile))
                throw new Exception("Query File Not Found");

            Console.Write("Excel File: ");
            string excelFile = @"D:\WebSearch.Net\Data\Features.xls";// Console.ReadLine();
            if (!File.Exists(excelFile))
                throw new Exception("Excel File Not Found");

            // stream reader for the queyr file
            StreamReader reader = new StreamReader(queryFile, Encoding.GetEncoding("gb2312"));
            StreamWriter writer = new StreamWriter("phrase1_log.txt", true, Encoding.GetEncoding("gb2312"));

            // using oledb to connect to the excel
            string connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                excelFile + ";" + "Extended Properties=Excel 8.0;";
            // build the connection to the excel
            OleDbConnection conn = new OleDbConnection(connStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO [sheet1$] " +
                "(Query,WordNum,AVGWordLen,PrepNum,PpNum,PropNounNum,NumeralNum,ConjNum,EndWithVerb," +
                "AVGnC,AVGnR,BestUrlResult,BestUrlAVGRank,BestUrlClickNum,BestUrlAVGOrder,MeanCD,StdDevCD," +
                "MedianCD,MRR,nCS,nRS,UrlResultNum,SuccessAt1,SuccessAt5,SuccessAt10,IsUrl,IsFileName) VALUES" +
                "(@Query,@WordNum,@AVGWordLen,@PrepNum,@PpNum,@PropNounNum,@NumeralNum,@ConjNum,@EndWithVerb," +
                "@AVGnC,@AVGnR,@BestUrlResult,@BestUrlAVGRank,@BestUrlClickNum,@BestUrlAVGOrder,@MeanCD," +
                "@StdDevCD,@MedianCD,@MRR,@nCS,@nRS,@UrlResultNum,@SuccessAt1,@SuccessAt5,@SuccessAt10," +
                "@IsUrl,@IsFileName)";
            cmd.Parameters.Add("@Query", OleDbType.VarChar, 500);
            cmd.Parameters.Add("@IsFileName", OleDbType.Integer);
            cmd.Parameters.Add("@IsUrl", OleDbType.Integer);
            cmd.Parameters.Add("@WordNum", OleDbType.Integer);
            cmd.Parameters.Add("@AVGWordLen", OleDbType.Double);
            cmd.Parameters.Add("@PrepNum", OleDbType.Integer);
            cmd.Parameters.Add("@PpNum", OleDbType.Integer);
            cmd.Parameters.Add("@PropNounNum", OleDbType.Integer);
            cmd.Parameters.Add("@NumeralNum", OleDbType.Integer);
            cmd.Parameters.Add("@ConjNum", OleDbType.Integer);
            cmd.Parameters.Add("@EndWithVerb", OleDbType.Integer);
            cmd.Parameters.Add("@AVGnC", OleDbType.Double);
            cmd.Parameters.Add("@AVGnR", OleDbType.Double);
            cmd.Parameters.Add("@BestUrlResult", OleDbType.VarChar, 500);
            cmd.Parameters.Add("@BestUrlAVGRank", OleDbType.Double);
            cmd.Parameters.Add("@BestUrlClickNum", OleDbType.Double);
            cmd.Parameters.Add("@BestUrlAVGOrder", OleDbType.Double);
            cmd.Parameters.Add("@MeanCD", OleDbType.Double);
            cmd.Parameters.Add("@StdDevCD", OleDbType.Double);
            cmd.Parameters.Add("@MedianCD", OleDbType.Double);
            cmd.Parameters.Add("@MRR", OleDbType.Double);
            cmd.Parameters.Add("@nCS", OleDbType.Double);
            cmd.Parameters.Add("@nRS", OleDbType.Double);
            cmd.Parameters.Add("@UrlResultNum", OleDbType.Integer);
            cmd.Parameters.Add("@SuccessAt1", OleDbType.Double);
            cmd.Parameters.Add("@SuccessAt5", OleDbType.Double);
            cmd.Parameters.Add("@SuccessAt10", OleDbType.Double);

            // for each query in the query file:
            string query = reader.ReadLine();
            while (query != null)
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                try
                {
                    cmd.Parameters["@Query"].Value = query;
                    // get the feature for the query
                    // 1. query's linguistic features
                    UserQueryFeature feature = UserQueryFeature.Get(query);
                    cmd.Parameters["@IsFileName"].Value = (feature.IsFileName == false) ? 0 : 1;
                    cmd.Parameters["@IsUrl"].Value = (feature.IsUrl == false) ? 0 : 1;

                    LinguisticFeature lf = feature.GetLinguisticFeatures();
                    cmd.Parameters["@WordNum"].Value = lf.WordNum;
                    cmd.Parameters["@AVGWordLen"].Value = lf.AVGWordLen;
                    cmd.Parameters["@PrepNum"].Value = lf.PrepositionNum;
                    cmd.Parameters["@PpNum"].Value = lf.PersonalPronounNum;
                    cmd.Parameters["@PropNounNum"].Value = lf.ProperNounNum;
                    cmd.Parameters["@NumeralNum"].Value = lf.NumeralNum;
                    cmd.Parameters["@ConjNum"].Value = lf.ConjunctionNum;
                    cmd.Parameters["@EndWithVerb"].Value = (lf.EndWithVerb == false) ? 0 : 1;

                    // 2. query' click-through features
                    ClickThroughFeature cf = feature.GetClickThroughFeatures("Sogou");
                    cmd.Parameters["@AVGnC"].Value = cf.AVGnC;
                    cmd.Parameters["@AVGnR"].Value = cf.AVGnR;
                    cmd.Parameters["@BestUrlResult"].Value = cf.BestUrlResult.URI.AbsoluteUri;
                    cmd.Parameters["@BestUrlClickNum"].Value = cf.BestUrlClickNum;
                    cmd.Parameters["@BestUrlAVGRank"].Value = cf.BestUrlAVGRank;
                    cmd.Parameters["@BestUrlAVGOrder"].Value = cf.BestUrlAVGOrder;
                    cmd.Parameters["@MeanCD"].Value = cf.MeanCD;
                    cmd.Parameters["@MedianCD"].Value = cf.MedianCD;
                    cmd.Parameters["@StdDevCD"].Value = cf.StdDevCD;
                    cmd.Parameters["@MRR"].Value = cf.MRR;
                    cmd.Parameters["@nCS"].Value = cf.nCS;
                    cmd.Parameters["@nRS"].Value = cf.nRS;
                    cmd.Parameters["@UrlResultNum"].Value = cf.UrlResultNum;
                    cmd.Parameters["@SuccessAt1"].Value = cf.SuccessAt1;
                    cmd.Parameters["@SuccessAt5"].Value = cf.SuccessAt5;
                    cmd.Parameters["@SuccessAt10"].Value = cf.SuccessAt10;

                    // 
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    writer.WriteLine(query + "\t" + ex.Message);
                    writer.Flush();
                }

                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
                // read the next query
                query = reader.ReadLine();
            }
            reader.Close();
            if (conn.State == System.Data.ConnectionState.Open)
                conn.Close();
            writer.Close();
        }

        /// <summary>
        /// 从(query, search result)间抽取大量feature
        /// </summary>
        public static void Phrase2()
        {
            Console.Write("Query File: ");
            string queryFile = @"E:\User\Jialiang\After May 1\Data\SogouQueries.txt";//Console.ReadLine();
            if (!File.Exists(queryFile))
                throw new Exception("Query File Not Found");

            Console.Write("Excel File: ");
            string excelFile = @"E:\User\Jialiang\After May 1\Data\Features.txt";// Console.ReadLine();
            //if (!File.Exists(excelFile))
            //    throw new Exception("Excel File Not Found");

            // stream reader for the queyr file
            StreamReader reader = new StreamReader(queryFile, Encoding.GetEncoding("gb2312"));
            StreamWriter writer = new StreamWriter("phrase2_log.txt", true, Encoding.GetEncoding("gb2312"));
            StreamWriter recorder = new StreamWriter(excelFile, true, Encoding.GetEncoding("gb2312"));
            // using oledb to connect to the excel
            //string connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
            //    excelFile + ";" + "Extended Properties=Excel 8.0;";
            //// build the connection to the excel
            //OleDbConnection conn = new OleDbConnection(connStr);
            //conn.Open();
            //OleDbCommand cmd = new OleDbCommand();
            //cmd.Connection = conn;
            //cmd.CommandText = "INSERT INTO [sheet2$] " +
            //    "(Query,IDF) VALUES" +
            //    "(@Query,@IDF)";
            //cmd.Parameters.Add("@Query", OleDbType.VarChar, 500);
            //cmd.Parameters.Add("@IDF", OleDbType.Double);

            // for each query in the query file:
            string query = reader.ReadLine();
            while (query != null)
            {
                //if (conn.State != System.Data.ConnectionState.Open)
                //    conn.Open();

                try
                {
                    string line = query;
                    Console.WriteLine(query);
                    //cmd.Parameters["@Query"].Value = query;

                    // get search result list features
                    SearchResultListFeature lf = SearchResultListFeature.Get(new UserQuery(query),
                        (WebCollection)DataSource.Get(DataSourceType.WebCollection, "SogouIndex"));

                    line += "\t" + Math.Round(lf.IDF, 8);

                    line += "\t" + lf.PageImageRatioStat.Min;
                    line += "\t" + lf.PageImageRatioStat.Max;
                    line += "\t" + lf.PageImageRatioStat.Mean;
                    line += "\t" + lf.PageImageRatioStat.Median;
                    line += "\t" + lf.PageImageRatioStat.StdDev;
                    line += "\t" + lf.PageImageRatioStat.Variance;
                    line += "\t" + lf.PageImageRatioStat.Entropy;

                    line += "\t" + lf.PageInSiteLnkNum.Min;
                    line += "\t" + lf.PageInSiteLnkNum.Max;
                    line += "\t" + lf.PageInSiteLnkNum.Mean;
                    line += "\t" + lf.PageInSiteLnkNum.Median;
                    line += "\t" + lf.PageInSiteLnkNum.StdDev;
                    line += "\t" + lf.PageInSiteLnkNum.Variance;
                    line += "\t" + lf.PageInSiteLnkNum.Entropy;

                    line += "\t" + lf.PageOutDegreeStat.Min;
                    line += "\t" + lf.PageOutDegreeStat.Max;
                    line += "\t" + lf.PageOutDegreeStat.Mean;
                    line += "\t" + lf.PageOutDegreeStat.Median;
                    line += "\t" + lf.PageOutDegreeStat.StdDev;
                    line += "\t" + lf.PageOutDegreeStat.Variance;
                    line += "\t" + lf.PageOutDegreeStat.Entropy;

                    line += "\t" + lf.PageServiceLnkInfo.Min;
                    line += "\t" + lf.PageServiceLnkInfo.Max;
                    line += "\t" + lf.PageServiceLnkInfo.Mean;
                    line += "\t" + lf.PageServiceLnkInfo.Median;
                    line += "\t" + lf.PageServiceLnkInfo.StdDev;
                    line += "\t" + lf.PageServiceLnkInfo.Variance;
                    line += "\t" + lf.PageServiceLnkInfo.Entropy;

                    line += "\t" + lf.PageSizeStat.Min;
                    line += "\t" + lf.PageSizeStat.Max;
                    line += "\t" + lf.PageSizeStat.Mean;
                    line += "\t" + lf.PageSizeStat.Median;
                    line += "\t" + lf.PageSizeStat.StdDev;
                    line += "\t" + lf.PageSizeStat.Variance;
                    line += "\t" + lf.PageSizeStat.Entropy;

                    line += "\t" + lf.PageTitleLenStat.Min;
                    line += "\t" + lf.PageTitleLenStat.Max;
                    line += "\t" + lf.PageTitleLenStat.Mean;
                    line += "\t" + lf.PageTitleLenStat.Median;
                    line += "\t" + lf.PageTitleLenStat.StdDev;
                    line += "\t" + lf.PageTitleLenStat.Variance;
                    line += "\t" + lf.PageTitleLenStat.Entropy;

                    line += "\t" + lf.ServiceLnkRatio;

                    line += "\t" + lf.SiteLnkRatio;

                    line += "\t" + lf.TermNuminAnchorStat.Min;
                    line += "\t" + lf.TermNuminAnchorStat.Max;
                    line += "\t" + lf.TermNuminAnchorStat.Mean;
                    line += "\t" + lf.TermNuminAnchorStat.Median;
                    line += "\t" + lf.TermNuminAnchorStat.StdDev;
                    line += "\t" + lf.TermNuminAnchorStat.Variance;
                    line += "\t" + lf.TermNuminAnchorStat.Entropy;

                    line += "\t" + lf.TermNuminImageStat.Min;
                    line += "\t" + lf.TermNuminImageStat.Max;
                    line += "\t" + lf.TermNuminImageStat.Mean;
                    line += "\t" + lf.TermNuminImageStat.Median;
                    line += "\t" + lf.TermNuminImageStat.StdDev;
                    line += "\t" + lf.TermNuminImageStat.Variance;
                    line += "\t" + lf.TermNuminImageStat.Entropy;

                    line += "\t" + lf.TermNuminListStat.Min;
                    line += "\t" + lf.TermNuminListStat.Max;
                    line += "\t" + lf.TermNuminListStat.Mean;
                    line += "\t" + lf.TermNuminListStat.Median;
                    line += "\t" + lf.TermNuminListStat.StdDev;
                    line += "\t" + lf.TermNuminListStat.Variance;
                    line += "\t" + lf.TermNuminListStat.Entropy;

                    line += "\t" + lf.TermNuminMetaStat.Min;
                    line += "\t" + lf.TermNuminMetaStat.Max;
                    line += "\t" + lf.TermNuminMetaStat.Mean;
                    line += "\t" + lf.TermNuminMetaStat.Median;
                    line += "\t" + lf.TermNuminMetaStat.StdDev;
                    line += "\t" + lf.TermNuminMetaStat.Variance;
                    line += "\t" + lf.TermNuminMetaStat.Entropy;

                    line += "\t" + lf.TermNuminTextFormatStat.Min;
                    line += "\t" + lf.TermNuminTextFormatStat.Max;
                    line += "\t" + lf.TermNuminTextFormatStat.Mean;
                    line += "\t" + lf.TermNuminTextFormatStat.Median;
                    line += "\t" + lf.TermNuminTextFormatStat.StdDev;
                    line += "\t" + lf.TermNuminTextFormatStat.Variance;
                    line += "\t" + lf.TermNuminTextFormatStat.Entropy;

                    line += "\t" + lf.TermNuminTitleStat.Min;
                    line += "\t" + lf.TermNuminTitleStat.Max;
                    line += "\t" + lf.TermNuminTitleStat.Mean;
                    line += "\t" + lf.TermNuminTitleStat.Median;
                    line += "\t" + lf.TermNuminTitleStat.StdDev;
                    line += "\t" + lf.TermNuminTitleStat.Variance;
                    line += "\t" + lf.TermNuminTitleStat.Entropy;

                    line += "\t" + lf.TFStat.Min;
                    line += "\t" + lf.TFStat.Max;
                    line += "\t" + lf.TFStat.Mean;
                    line += "\t" + lf.TFStat.Median;
                    line += "\t" + lf.TFStat.StdDev;
                    line += "\t" + lf.TFStat.Variance;
                    line += "\t" + lf.TFStat.Entropy;

                    line += "\t" + lf.TitleQueryProximityStat.Min;
                    line += "\t" + lf.TitleQueryProximityStat.Max;
                    line += "\t" + lf.TitleQueryProximityStat.Mean;
                    line += "\t" + lf.TitleQueryProximityStat.Median;
                    line += "\t" + lf.TitleQueryProximityStat.StdDev;
                    line += "\t" + lf.TitleQueryProximityStat.Variance;
                    line += "\t" + lf.TitleQueryProximityStat.Entropy;

                    line += "\t" + lf.UrlDepthStat.Min;
                    line += "\t" + lf.UrlDepthStat.Max;
                    line += "\t" + lf.UrlDepthStat.Mean;
                    line += "\t" + lf.UrlDepthStat.Median;
                    line += "\t" + lf.UrlDepthStat.StdDev;
                    line += "\t" + lf.UrlDepthStat.Variance;
                    line += "\t" + lf.UrlDepthStat.Entropy;

                    line += "\t" + lf.UrlLenStat.Min;
                    line += "\t" + lf.UrlLenStat.Max;
                    line += "\t" + lf.UrlLenStat.Mean;
                    line += "\t" + lf.UrlLenStat.Median;
                    line += "\t" + lf.UrlLenStat.StdDev;
                    line += "\t" + lf.UrlLenStat.Variance;
                    line += "\t" + lf.UrlLenStat.Entropy;

                    line += "\t" + lf.UrlQueryProximityStat.Min;
                    line += "\t" + lf.UrlQueryProximityStat.Max;
                    line += "\t" + lf.UrlQueryProximityStat.Mean;
                    line += "\t" + lf.UrlQueryProximityStat.Median;
                    line += "\t" + lf.UrlQueryProximityStat.StdDev;
                    line += "\t" + lf.UrlQueryProximityStat.Variance;
                    line += "\t" + lf.UrlQueryProximityStat.Entropy;

                    line += "\t" + lf.UrlQueryProxiPosStat.Min;
                    line += "\t" + lf.UrlQueryProxiPosStat.Max;
                    line += "\t" + lf.UrlQueryProxiPosStat.Mean;
                    line += "\t" + lf.UrlQueryProxiPosStat.Median;
                    line += "\t" + lf.UrlQueryProxiPosStat.StdDev;
                    line += "\t" + lf.UrlQueryProxiPosStat.Variance;
                    line += "\t" + lf.UrlQueryProxiPosStat.Entropy;

                    recorder.WriteLine(line);
                    //cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    writer.WriteLine(query + "\t" + ex.Message);
                    Console.Write(ex.Message + "\n" + ex.StackTrace);
                    writer.Flush();
                }
                recorder.Flush();

                //if (conn.State == System.Data.ConnectionState.Open)
                //    conn.Close();
                // read the next query
                query = reader.ReadLine();
            }
            reader.Close();
            //if (conn.State == System.Data.ConnectionState.Open)
            //    conn.Close();
            writer.Close();
            recorder.Close();
        }
    }
}
