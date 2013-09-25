using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WebSearch.Linguistics.Net;
using WebSearch.Common.Net;

namespace TestApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            string before = Console.ReadLine();
            string behind = Console.ReadLine();
            string tagname;
            while (before != "exit")
            {
                HtmlTagType type = HtmlParser.GetEnclosedHtmlTag(before, behind, out tagname);
                Console.WriteLine(type.ToString() + " " + tagname);
                before = Console.ReadLine();
                behind = Console.ReadLine();
            }
            return;

            // test the chinese speller
            Console.WriteLine(ChineseSpeller.Spell("中国人"));
            return;

            string text = Console.ReadLine();
            while (text != "exit")
            {
                Console.WriteLine(Regex.Replace(text, "\\.", " . "));
                Console.WriteLine(NictclasFilterBugs(text));
                text = Console.ReadLine();
            }
        }

        private static string NictclasFilterBugs(string text)
        {
            text = Regex.Replace(text, "(?<digit>\\d)/",
                new MatchEvaluator(Program.PreprocessDigits));
            text = Regex.Replace(text, "(?<date>\\d日|月)(?<ct>-\\d)",
                new MatchEvaluator(Program.PreprocessDates));
            // filter the sequent symbols
            return Regex.Replace(text, "\\W{10,}|//|'", " ");
        }

        private static string PreprocessDigits(Match m)
        {
            return m.Groups["digit"].Value + " " + "/";
        }

        private static string PreprocessDates(Match m)
        {
            return m.Groups["date"].Value + " " + m.Groups["ct"].Value;
        }
    }
}
