using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Contexts;
using WebSearch.Linguistics.Net;
using System.Text.RegularExpressions;

namespace TestApplication1
{
    class Program
    {
        public enum TestEnum
        {
            Test1, Test2
        };

        static void Main(string[] args)
        {
            // test translation
            string text = Console.ReadLine();
            while (text != "exit")
            {
                string result = GoogleTranslator.Translate(text, GoogleTranslator.EN_to_CN);
                Console.WriteLine(result);
                text = Console.ReadLine();
            }
            return;

            string test = Console.ReadLine();

            while (test != "exit")
            {
                string []result = ChineseHelper.Segment(test);
                foreach (string seg in result)
                    Console.Write(seg + " ");
                Console.WriteLine();
                test = Console.ReadLine();
            }
            return;
        }
    }
}
            
//            SharpWordNet.WordNetEngine engine = new SharpWordNet.DataFileEngine(@"C:\Program Files\WordNet\2.1\dict");

//            string query = Console.ReadLine();

//            while (query != "exit")
//            {
//                SharpWordNet.Synset[] synsets = engine.GetSynsets(query, "noun");
//                foreach (Synset s in synsets)
//                {
//                    int total = s.WordCount;
//                    Console.Write(s.WordCount.ToString() + ": ");
//                    for (int i = 0; i < total; i++)
//                    {
//                        Console.Write(s.GetWord(i) + " ");
//                    }
//                    Console.WriteLine("");
//                }
//                query = Console.ReadLine();
//            }
//            return;
            
//            /*WebProxy proxy = new WebProxy("http://inproxy.sjtu.edu.cn:8000");
//            proxy.Credentials = new NetworkCredential("liangliang_norest", "MSGTSCNewsgroup");*/
//            WebRequest request = null;
//           // request.Proxy = proxy;
//            StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.GetEncoding("gb2312"));
//            string text = reader.ReadToEnd();
            
            
//    //        IndexSearcher searcher = new IndexSearcher(ramDir);
//    //Query query = QueryParser.parse("Kenne*", FIELD_NAME, analyzer);
//    //query = query.rewrite(reader); //required to expand search terms
//    //Hits hits = searcher.search(query);

//    //Highlighter highlighter = new Highlighter(this, new QueryScorer(query));
//    //for (int i = 0; i < hits.length(); i++)
//    //{
//    //    String text = hits.doc(i).get(FIELD_NAME);
//    //    TokenStream tokenStream = analyzer.tokenStream(FIELD_NAME, new StringReader(text));
//    //    // Get 3 best fragments and seperate with a "..."
//    //    String result = highlighter.getBestFragments(tokenStream, text, 3, "...");
//    //    System.out.println(result);
//    //}
//        }
//    }
//}
