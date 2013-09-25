using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Common.Net;
using WebSearch.Model.Net;
using Lucene.Net.Documents;
using WebSearch.Crawler.Net;
using System.IO;
using WebSearch.DataCenter.Net.Lucene;

namespace SearcherDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Index Directory: ");
            string indexDir = Console.ReadLine();
            if (!Directory.Exists(indexDir))
            {
                Console.WriteLine("Invalid index directory");
                return;
            }

            LuceneSearcher _searcher = new LuceneSearcher(indexDir, SupportedLanguage.Chinese);

            Console.Write("Query: ");
            string query = Console.ReadLine();
            while (query != "exit")
            {
                SearchResultList results = _searcher.Search(query, "html", 10);
                foreach (SearchResult result in results)
                {
                    Console.WriteLine("==========================================\n" + result.Url);
                    foreach (string snip in result.Snippets)
                        Console.WriteLine("[--------------------snippet]: \n" + snip);
                }
                Console.Write("Query: ");
                query = Console.ReadLine();
            }
            //Console.WriteLine(results.Count.ToString());
            _searcher.Close();
        }
    }
}
