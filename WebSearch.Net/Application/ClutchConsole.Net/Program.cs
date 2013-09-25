using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using WebSearch.Common.Net;
using WebSearch.Crawler.Net;
using WebSearch.Model.Net;
using Lucene.Net.Documents;
using WebSearch.DataCenter.Net.Lucene;
using WebSearch.DataCenter.Net.DS;
using Lucene.Net.Search;
using WebSearch.Linguistics.Net;

namespace ClutchConsole.Net
{
    class Program
    {
        /// <summary>
        /// index the files in the directory to the targetDir
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="targetDir"></param>
        static void IndexDirectory(string directory, string targetDir)
        {
            // set the indexer
            LuceneIndexer.INDEX_STORE_PATH = targetDir;
            LuceneIndexer.MAX_DOCS_IN_RAM = 10;
            LuceneIndexer.LANGUAGE = SupportedLanguage.Chinese;
            LuceneIndexer.Initialize();
            LuceneSearcher _searcher = new LuceneSearcher(targetDir, SupportedLanguage.Chinese);

            Console.Write("Start ID: ");
            int startID = int.Parse(Console.ReadLine());
            Console.Write("End ID: ");
            int endID = int.Parse(Console.ReadLine());

            if (!directory.EndsWith("\\"))
                directory += "\\";

            StreamWriter log = new StreamWriter("log.txt", true, Encoding.GetEncoding("gb2312"));

            //string[] files = Directory.GetFiles(directory);
            //Console.WriteLine("Total: " + files.Length.ToString() + " files");
            //foreach (string file in files)
            for (int fid = startID; fid <= endID; fid++)
            {
                string file = directory + fid.ToString() + ".htm";
                if (!File.Exists(file))
                    continue;

                // if the file has already been indexed, skip it.
                Hits hits = _searcher.SearchKeyword(fid.ToString(), WebCollection.ID);
                if (!(hits == null || hits.Length() <= 0))
                    continue;

                // read the file
                StreamReader reader = new StreamReader(file, Encoding.GetEncoding("gb2312"));

                Console.Write(fid.ToString());
                string url = reader.ReadLine(); // the first line is url
                Console.WriteLine(" " + url);
                string html = reader.ReadToEnd();
                WebPage page = new WebPage(fid, url, html, Encoding.GetEncoding("gb2312"));

                Document doc = LuceneWebCollection.ToDocument(page);

                try
                {
                    LuceneIndexer.Instance.AddDocument(doc);
                }
                catch (Exception ex)
                {
                    ChineseHelper.Reset();   
                    Console.WriteLine(fid.ToString() + "\t" + ex.Message);
                    log.WriteLine(fid.ToString() + "\t" + ex.Message);
                    log.Flush();
                }
                reader.Close();
            }
            log.Close();
            LuceneIndexer.Close();
            _searcher.Close();
        }

        static void Main(string[] args)
        {
            Console.Write("The source directory: ");
            string srcDir = Console.ReadLine();
            if (!Directory.Exists(srcDir))
                throw new Exception("unexisting directory");
            Console.Write("The target directory: ");
            string tarDir = Console.ReadLine();
            if (!Directory.Exists(tarDir))
                throw new Exception("unexisting directory");

            IndexDirectory(srcDir, tarDir);

            return;

            // for parameters
            Console.Write("Url-list File Name: ");
            string urlFileName = Console.ReadLine();
            Console.Write("Start ID: ");
            int startID = int.Parse(Console.ReadLine());
            Console.Write("End ID: ");
            int endID = int.Parse(Console.ReadLine());
            Console.Write("Data Dir: ");
            string dataDir = Console.ReadLine();
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);
            if (!dataDir.EndsWith("\\"))
                dataDir += "\\";
            //Console.Write("Index Dir: ");
            //string indexDir = Console.ReadLine();
            //if (!Directory.Exists(indexDir))
            //    Directory.CreateDirectory(indexDir);
            //if (!indexDir.EndsWith("\\"))
            //    indexDir += "\\";
            Console.Write("Language: ");
            SupportedLanguage lang = SupportedLanguage.English;
            if (Console.ReadLine().ToLower().StartsWith("ch"))
                lang = SupportedLanguage.Chinese;

            // set the indexer
            //LuceneIndexer.INDEX_STORE_PATH = indexDir;
            //LuceneIndexer.MAX_DOCS_IN_RAM = 10;
            //LuceneIndexer.LANGUAGE = lang;
            //LuceneIndexer.Initialize();

            // set the web crawler
            WebCrawler crawler = new WebCrawler(WebProxies.Get("SJTU"));
            crawler.StartPageID = startID;

            // read the url file
            StreamReader reader = new StreamReader(urlFileName);
            StreamWriter writer = new StreamWriter("Url" +
                startID.ToString() + "-" + endID.ToString() + ".log.txt");
            string url = reader.ReadLine();
            int currentID = 0;
            while (url != null)
            {
                currentID++; // from number 1
                if (currentID >= startID)
                {
                    if (currentID > endID)
                        break;

                    Console.Write(currentID.ToString() + ": " + url);

                    // crawl the page
                    WebPage page = crawler.Retrieve(url);
                    if (page != null)
                    {
                        try
                        {
                            Console.WriteLine(" crawled");
                            // save the page content
                            page.ID = currentID;
                            File.WriteAllText(dataDir + page.ID + ".htm",
                                page.Url + "\n" + page.Html, Encoding.GetEncoding("gb2312"));
                            // index the page
                            //Document doc = LuceneWebCollection.ToDocument(page);
                            //LuceneIndexer.Instance.AddDocument(doc);
                        }
                        catch (Exception ex)
                        {
                            writer.WriteLine(currentID.ToString() + "\t" + url + "\tProcess error: ");
                            writer.Flush();
                            Console.WriteLine(" Process error" + ex.Message);
                        }
                    }
                    else
                    {
                        // log the url
                        writer.WriteLine(currentID.ToString() + "\t" + url);
                        writer.Flush();
                        Console.WriteLine(" error");
                    }
                }
                url = reader.ReadLine();
            }
            //LuceneIndexer.Close();
            reader.Close();
            writer.Close();
        }
    }
}
