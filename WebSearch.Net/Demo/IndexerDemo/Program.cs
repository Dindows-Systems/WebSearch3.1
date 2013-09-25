using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using WebSearch.Common.Net;
using WebSearch.DataCenter.Net.Lucene;

namespace IndexerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // demo index merger
            MergeIndex();
        }

        static void MergeIndex()
        {
        //    // get the result dir for indexes
        //    Console.Write("Result Dir: ");
        //    string resultDir = Console.ReadLine();
        //    if (!Directory.Exists(resultDir))
        //        Directory.CreateDirectory(resultDir);

            Console.Write("Index1: ");
            string dir1 = Console.ReadLine();
            Console.Write("Index2: ");
            string dir2 = Console.ReadLine();
            //Console.Write("Index3: ");
            //string dir3 = Console.ReadLine();
            //Console.Write("Index4: ");
            //string dir4 = Console.ReadLine();
            //Console.Write("Index5: ");
            //string dir5 = Console.ReadLine();

            List<string> dirs = new List<string>();
            dirs.Add(dir2);
            //dirs.Add(dir3);
            //dirs.Add(dir4);
            //dirs.Add(dir5);
            // start to merge the indexes
            LuceneMerger.Merge(dir1, dirs, LuceneAnalyzer.Get(SupportedLanguage.Chinese));

            Console.WriteLine("Done");
        }
    }
}
