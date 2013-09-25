using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WebSearch.Common.Net;
using System.Xml;

namespace WebSearch.Linguistics.Net
{
    public class ChineseHelper
    {
        ///////////////////////// Rainsoft WordSeg System ////////////////////

        //#region Rainsoft WordSeg System

        //protected static SegmentWord _wordSeg = null;

        //public static SegmentWord WordSegmentor
        //{
        //    get
        //    {
        //        if (_wordSeg == null)
        //            _wordSeg = new SegmentWord(SegmentMode.Precision);
        //        return _wordSeg;
        //    }
        //}

        //#endregion

        public static string[] Segment(string text)
        {
            // tokenize the text, do the filter and recognize name
            //return WordSegmentor.Segment(text, true, false);

            string result = null;
            Segment(text, out result);
            return Regex.Split(result, "\\s+");
        }

        public static bool Segment(string text, out string result)
        {
            //string[] terms = WordSegmentor.Segment(text, false, false);
            //result = "";
            //foreach (string term in terms)
            //    result += (term + " ");
            //return true;

            // preprocess the text:
            result = "";
            text = NictclasFilterBugs(text);
            Nictclas.OperateType = eOperateType.OnlySegment;
            Nictclas.OutputFormat = eOutputFormat.PKU;
            bool r = Nictclas.ParagraphProcessing(text, ref result);
            result = result.Trim();
            return r;
        }

        ///////////////////////// NICTCLAS System ////////////////////////////

        #region NICTCLAS System

        protected static NICTCLAS _nictclas = null;

        public static NICTCLAS Nictclas
        {
            get
            {
                if (_nictclas == null)
                {
                    XmlElement node = XmlHelper.ReadNode(
                        Config.SettingPath + "Dictionary.xml", "NICTCLAS");
                    _nictclas = new NICTCLAS(node.Attributes["_Path"].Value);
                }
                return _nictclas;
            }
        }

        #endregion

        public static void Reset()
        {
            if (_nictclas != null)
            {
                _nictclas.Dispose();
                _nictclas = null;
            }
        }

        /// <summary>
        /// Get the POS tag tokenized for the text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] PosTagTokens(string text)
        {
            Nictclas.OperateType = eOperateType.SecondTag;
            Nictclas.OutputFormat = eOutputFormat.PKU;

            // preprocess the text, filter some invalid symbols
            // text = NictclasFilterBugs(text);
            // process the text with nictclas
            string result = "";
            Nictclas.ParagraphProcessing(text, ref result);
            MatchCollection coll = Regex.Matches(
                result, @"/(?<tag>\w+)\s");
            if (coll == null)
                return new string[0];

            string[] tags = new string[coll.Count];
            int i = 0;
            foreach (Match match in coll)
            {
                tags[i] = match.Groups["tag"].Value;
                i++;
            }
            return tags;
        }

        #region Private Methods

        private static string NictclasFilterBugs(string text)
        {
            text = Regex.Replace(text, "\\W+", " ");
            //text = Regex.Replace(text, "\\W+", " ");
            text = Regex.Replace(text, "(?<digit>\\d)/",
                new MatchEvaluator(ChineseHelper.PreprocessDigits));
            // filter the sequent symbols and others
            text = Regex.Replace(text, "\\W{10,}|([\\W\\dA-Z]{30,})|//|'|’|‘|\"|“|”|\\(|\\)|Ⅰ", " ");
            text = Regex.Replace(text, "[a-zA-Z0-9_|+_)(*&^%$#@!~?><\":}{\\[\\]/.,\\\\Ⅰ]{50,}", " ");
            text = Regex.Replace(text, "月", "月 ");
            text = Regex.Replace(text, "日", "日 ");
            text = Regex.Replace(text, "(?<words>\\w{40})",
                new MatchEvaluator(ChineseHelper.PreprocessWords));
            // deal with the dots
            return Regex.Replace(text, "(?<sym>\\.|-)",
                new MatchEvaluator(ChineseHelper.PreprocessSymbols));
        }

        private static string PreprocessDigits(Match m)
        {
            return m.Groups["digit"].Value + " " + "/";
        }

        private static string PreprocessSymbols(Match m)
        {
            return m.Groups["sym"].Value + " ";
        }

        private static string PreprocessWords(Match m)
        {
            return m.Groups["words"].Value + " ";
        }

        #endregion
    }
}
