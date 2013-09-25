using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.DataCenter.Net;
using WebSearch.Model.Net;
using System.IO;

namespace SearchEngineDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Select Mode [1/2]: ");
            int mode = int.Parse(Console.ReadLine());
            Console.Write("Result Num: ");
            int count = int.Parse(Console.ReadLine());

            switch (mode)
            {
                case 1: SearchUrl(count); break;
                default: Search(count); break;
            }
        }

        public static void SearchUrl(int count)
        {
            StreamWriter writer = new StreamWriter("C:\\Urls.txt", true, Encoding.GetEncoding("gb2312"));
            StreamWriter writer2 = new StreamWriter("C:\\Errors.txt", true, Encoding.GetEncoding("gb2312"));
            StreamReader reader = new StreamReader(@"D:\Studies\WebSearch.Net\Data\1000Queries.txt", Encoding.GetEncoding("gb2312"));

            Console.Write("Query/[exit]: ");
            string query = reader.ReadLine();// Console.ReadLine();
            while (query != null)
            {/*
                Console.WriteLine("Google Search Result: ----------");
                IWebCollection webCollection = DataRetriever.GetWebCollection("google");
                string[] results = webCollection.SearchUrls(query, count);
                for (int i = 0; i < results.Length; i++)
                    Console.WriteLine(i.ToString() + ": " + results[i]);

                Console.WriteLine("Baidu Search Result: ----------");
                webCollection = DataRetriever.GetWebCollection("baidu");
                results = webCollection.SearchUrls(query, count);
                for (int i = 0; i < results.Length; i++)
                    Console.WriteLine(i.ToString() + ": " + results[i]);
                */
                //Console.WriteLine("Sogou Search Result: ----------");
                Console.WriteLine(query);
                writer.WriteLine("[query]:\t" + query);
                IWebCollection webCollection = DataRetriever.GetWebCollection("sogou");
                string[] results = webCollection.SearchUrls(query, count);
                if (results.Length == 0)
                    writer2.WriteLine(query);
                else
                {
                    for (int i = 0; i < results.Length; i++)
                    {
                        writer.WriteLine(results[i]);
                        //Console.WriteLine(i.ToString() + ": " + results[i]);
                    }
                }
                //Console.Write("\nQuery/[exit]: ");
                query = reader.ReadLine();// Console.ReadLine();
                writer.Flush();
                writer2.Flush();
            }
            reader.Close();
            writer.Close();
            writer2.Close();
        }

        public static void Search(int count)
        {
            Console.Write("Query/[exit]: ");
            string query = Console.ReadLine();
            while (query != "exit")
            {
                Console.WriteLine("Google Search Result: ----------");
                IWebCollection webCollection = DataRetriever.GetWebCollection("google");
                SearchResultList results = webCollection.Search(query, count);
                foreach (SearchResult result in results)
                {
                    Console.Write(result.Rank.ToString() + ": ");
                    Console.WriteLine(result.Anchor);
                    Console.WriteLine(result.Url);
                    Console.WriteLine(result.Snippets[0]);
                    Console.WriteLine();
                }

                Console.WriteLine("\nBaidu Search Result: ----------");
                webCollection = DataRetriever.GetWebCollection("baidu");
                results = webCollection.Search(query, count);
                foreach (SearchResult result in results)
                {
                    Console.Write(result.Rank.ToString() + ": ");
                    Console.WriteLine(result.Anchor);
                    Console.WriteLine(result.Url);
                    Console.WriteLine(result.Snippets[0]);
                    Console.WriteLine();
                }

                Console.WriteLine("\nSogou Search Result: ----------");
                webCollection = DataRetriever.GetWebCollection("sogou");
                results = webCollection.Search(query, count);
                foreach (SearchResult result in results)
                {
                    Console.Write(result.Rank.ToString() + ": ");
                    Console.WriteLine(result.Anchor);
                    Console.WriteLine(result.Url);
                    Console.WriteLine(result.Snippets[0]);
                    Console.WriteLine();
                }

                Console.Write("\nQuery/[exit]: ");
                query = Console.ReadLine();
            }
        }
    }
}
