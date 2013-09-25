using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace WebSearch.Common.Net
{
    #region Html Tag Type

    public enum HtmlTagType
    {
        Empty = 1, TextFormat, Citation, ComputerOutput, 
        List, Table, Title, Meta, Anchor, Image
    }

    #endregion

    public class HtmlParser
    {
        #region Html Tags

        #region Filter Tags

        // height="[^"]*"
        private static string[] filterAttributes = new string[] {
            "width", "height", "target", "align", "valign", "class", 
            "border", "cellspacing", "cellpadding", "bgcolor", 
            "rowspan", "colspan", "border", "leftmargin", "topmargin", 
            "rightmargin", "name", "id", "hspace", "onmouseover", 
            "onmousedown", "onclick", "onmouseout", "onmousemove",
            "style", "onload", "bgsound", "background", "vspace"};

        // (<input[^>]*>)|(</input>)
        private static string[] filterTags = new string[] {
            "tr", "td", "table", "p", "br", "marquee", "thead",
            "div", "input", "form", "span", "tbody", "tfoot", "th", 
            "select", "option", "link", "label", "iframe", "button",
            "frame", "frameset", "frameset", "colgroup", "col",
            "textarea", "fieldset", "optgroup" };

        // (<script[^/>]*/>)|(<script[^>]*>([^\xA4]*?)</script>)
        public static readonly string[] filterContentTags = new string[] {
            "script", "style", "link", "embed", "object" };

        #endregion

        #region Useful Html Tags

        /// <summary>
        /// Tax formatting html tags
        /// </summary>
        public static readonly string[] TextFormattingTags = new string[] {
            "b", "big", "em", "i", "strong", "small", "sub", 
            "sup", "ins", "del", "strike", "s", "u", "h1", 
            "h2", "h3", "h4", "h5", "h6", "font", "caption", "legend" };

        /// <summary>
        /// Citation html tags
        /// </summary>
        public static readonly string[] CitationTags = new string[] {
            "abbr", "acronym", "address", "bdo", "blockquote",
            "q", "cite", "dfn" };

        /// <summary>
        /// Computer output html tags
        /// </summary>
        public static readonly string[] ComputerOutputTags = new string[] {
            "code", "kbd", "samp", "tt", "var", "pre",
            "listing", "plaintext", "xmp" };

        /// <summary>
        /// List html tags
        /// </summary>
        public static readonly string[] ListTags = new string[] {
            "ol", "ul", "li", "dl", "dt", "dd", "dir", "menu" };

        /// <summary>
        /// Table html tags
        /// </summary>
        public static readonly string[] TableTags = new string[] {
            "table", "tr", "td" };

        /// <summary>
        /// Anchor html tags
        /// </summary>
        public static readonly string[] AnchorTags = new string[] {
            "a" };

        /// <summary>
        /// Image html tags
        /// </summary>
        public static readonly string[] ImageTags = new string[] {
            "img" };

        /// <summary>
        /// Html title tag
        /// </summary>
        public static readonly string[] TitleTags = new string[] {
            "title" };

        /// <summary>
        /// Html meta section tag
        /// </summary>
        public static readonly string[] MetaTags = new string[] {
            "meta" };

        private static Hashtable _htmlTags = null;
        public static Hashtable HtmlTags
        {
            get
            {
                if (_htmlTags == null)
                {
                    _htmlTags = new Hashtable();
                    foreach (string tag in TextFormattingTags)
                        _htmlTags.Add(tag, HtmlTagType.TextFormat);
                    foreach (string tag in CitationTags)
                        _htmlTags.Add(tag, HtmlTagType.Citation);
                    foreach (string tag in ComputerOutputTags)
                        _htmlTags.Add(tag, HtmlTagType.ComputerOutput);
                    foreach (string tag in ListTags)
                        _htmlTags.Add(tag, HtmlTagType.List);
                    foreach (string tag in TableTags)
                        _htmlTags.Add(tag, HtmlTagType.Table);
                    foreach (string tag in AnchorTags)
                        _htmlTags.Add(tag, HtmlTagType.Anchor);
                    foreach (string tag in ImageTags)
                        _htmlTags.Add(tag, HtmlTagType.Image);
                    foreach (string tag in TitleTags)
                        _htmlTags.Add(tag, HtmlTagType.Title);
                    foreach (string tag in MetaTags)
                        _htmlTags.Add(tag, HtmlTagType.Meta);
                }
                return _htmlTags;
            }
        }

        #endregion

        #endregion

        #region Regular Expressions

        /// <summary>
        /// General Filter Regex
        /// </summary>
        public static Regex GeneralFilterRegex
        {
            get
            {
                if (_genFilterRegex == null)
                {
                    string regexStr = "(<!--[^-]*-->)|(<!DOCTYPE[^>]*>)"; // comments
                    // deal with filter attributes
                    for (int i = 0; i < filterAttributes.Length; i++)
                        regexStr += "|" + filterAttributes[i] + "=\"[^\"]*\"";
                    // deal with filter tagger
                    for (int i = 0; i < filterTags.Length; i++)
                        regexStr += "|(<" + filterTags[i] + "[^>]*>)|(</" + filterTags[i] + ">)";
                    // deal with filter tag content
                    for (int i = 0; i < filterContentTags.Length; i++)
                        regexStr += "|(<" + filterContentTags[i] + "[^/>]*/>)|(<" +
                            filterContentTags[i] + "[^>]*>([^\xA4]*?)</" + filterContentTags[i] + ">)";

                    _genFilterRegex = new Regex(regexStr,
                        RegexOptions.IgnoreCase | RegexOptions.Compiled);
                }
                return _genFilterRegex;
            }
        }
        private static Regex _genFilterRegex = null;

        /// <summary>
        /// Plain Text Filter Regex
        /// </summary>
        public static Regex PlainFilterRegex
        {
            get
            {
                if (_plainFilterRegex == null)
                {
                    // filter comments and html tags
                    string regexStr = "(</?[a-z][a-z0-9]*[^<>]*>)|" +
                        "(<!--+[^-]*--+>)|(<!DOCTYPE[^>]*>)"; 
                    
                    _plainFilterRegex = new Regex(regexStr, RegexOptions.IgnoreCase);
                }
                return _plainFilterRegex;
            }
        }
        private static Regex _plainFilterRegex = null;

        /// <summary>
        /// White-space Regex
        /// </summary>
        public static Regex WhiteSpaceRegex
        {
            get { return _whiteSpaceRegex; }
        }
        private static Regex _whiteSpaceRegex = new Regex("\\s{3,}",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        /// <summary>
        /// Sign Regex
        /// </summary>
        public static Regex SignRegex
        {
            get { return _signRegex; }
        }
        private static Regex _signRegex = new Regex(
            "(&nbsp;)|(&quot;)|(&apos;)|(&gt;)|(&lt;)|(&amp;)",
            RegexOptions.Compiled);

        /// <summary>
        /// Description Meta-data Regex
        /// </summary>
        public static Regex DescriptionRegex
        {
            get { return _descRegex; }
        }
        private static Regex _descRegex = new Regex("<meta\\s+name\\s*=\\s*\"?description\"" +
            "?\\s+content\\s*=\\s*\"?(?<description>.*?)\"?\\s*/?>", RegexOptions.IgnoreCase);

        /// <summary>
        /// Keywords Meta-data Regex
        /// </summary>
        public static Regex KeywordsRegex
        {
            get { return _kwdRegex; }
        }
        private static Regex _kwdRegex = new Regex("<meta\\s+name\\s*=\\s*\"?keywords\"" +
            "?\\s+content\\s*=\\s*\"?(?<keywords>.*?)\"?\\s*/?>", RegexOptions.IgnoreCase);

        /// <summary>
        /// Html Tagged Content Regex
        /// </summary>
        public static Regex HtmlTaggedContentRegex
        {
            get { return _taggedContRegex; }
        }
        public static Regex GetHtmlTaggedContentRegex(string tagName)
        {
            return new Regex("<" + tagName + "(\\s[^>]*)?>(.*?)</" + tagName +
                ">|<" + tagName + "(\\s[^>]*)?/>", RegexOptions.IgnoreCase);
        }
        private static Regex _taggedContRegex = new Regex(
            "<([A-Z][A-Z0-9]*)[^>]*>(.*?)</\\1>|<([A-Z][A-Z0-9]*)[^>]*/>", RegexOptions.IgnoreCase);
        

        /// <summary>
        /// Html Tag Regex
        /// </summary>
        public static Regex HtmlTagRegex
        {
            get { return _htmlTagRegex; }
        }
        public static Regex GetHtmlTagRegex(string tagName)
        {
            return new Regex("</?" + tagName + "(\\s[^<>]*)?>", RegexOptions.IgnoreCase);
        }
        private static Regex _htmlTagRegex = new Regex("</?[a-z][a-z0-9]*[^<>]*>", 
            RegexOptions.IgnoreCase);

        /// <summary>
        /// Html Head Tag Regex
        /// </summary>
        public static Regex HtmlHeadTagRegex = new Regex("<(?<headtag>[a-z][a-z0-9]*)[^<>]*",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// Html Tail Tag Regex
        /// </summary>
        public static Regex HtmlTailTagRegex = new Regex("</(?<tailtag>[a-z][a-z0-9]*)[^<>]*", 
            RegexOptions.IgnoreCase);

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Filter(string html, bool plainText)
        {
            string temp = "";
            if (!plainText)
                temp = GeneralFilterRegex.Replace(html, " ");
            else
                temp = PlainFilterRegex.Replace(html, " ");

            temp = SignRegex.Replace(temp, " ");
            // using whitespace regex to filter the blank spaces
            temp = WhiteSpaceRegex.Replace(temp, " ");
            return WhiteSpaceRegex.Replace(temp, " ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static ArrayList FindTag(string html, string tag)
        {
            Regex tagRegex = new Regex("<" + tag + "[^>]*>", RegexOptions.IgnoreCase);
            MatchCollection matches = tagRegex.Matches(html);

            ArrayList results = new ArrayList();
            foreach (Match match in matches)
                results.Add(match.Value);
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string FindTagContent(string html, string tag)
        {
            Regex tagRegex = new Regex("<" + tag + "[^>]*>(.*?)</" +
                tag + ">", RegexOptions.IgnoreCase);
            Match match = tagRegex.Match(html);
            if (match == null || !match.Success)
                return "";
            int start = match.Value.IndexOf('>') + 1;
            int end = match.Value.LastIndexOf("</");
            return match.Value.Substring(start, end - start);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static int FindTagNum(string html, string tag)
        {
            Regex tagRegex = new Regex("<" + tag + "[^>]*>", RegexOptions.IgnoreCase);
            return tagRegex.Matches(html).Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagStr"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static string FindAttribute(string tagStr, string attrName)
        {
            int start = tagStr.IndexOf(attrName);
            bool invalue = false;
            string result = "";
            for (int i = start + attrName.Length; i < tagStr.Length; i++)
            {
                if (tagStr[i] == '\"')
                {
                    if (invalue)
                        return result;
                    else
                    {
                        invalue = true;
                        continue;
                    }
                }
                if (invalue)
                    result += tagStr[i];
            }
            return result;
        }

        /// <summary>
        /// Find Keywords in html's meta data section
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string FindKeywords(string html)
        {
            return KeywordsRegex.Match(html).Groups["keywords"].Value;
        }

        /// <summary>
        /// Find Description in html's meta data section
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string FindDescription(string html)
        {
            return DescriptionRegex.Match(html).Groups["description"].Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="beforeHtml">html snippet before the term</param>
        /// <param name="behindHtml">html snippet behind the term</param>
        /// <param name="tagName">the tag detected for the term</param>
        /// <returns>the html tag type</returns>
        public static HtmlTagType GetEnclosedHtmlTag(string beforeHtml, 
            string behindHtml, out string tagName)
        {
            HtmlTagType result;
            // detect tag from before html first
            if (!string.IsNullOrEmpty(beforeHtml.Trim()))
            {
                // filter all tag enclosures
                beforeHtml = HtmlTaggedContentRegex.Replace(beforeHtml, "");
                // find the head tags
                MatchCollection ms = HtmlHeadTagRegex.Matches(beforeHtml);
                if (ms != null && ms.Count > 0)
                {
                    // from the last to the first:
                    for (int i = ms.Count - 1; i >= 0; i--)
                    {
                        tagName = ms[i].Groups["headtag"].Value.ToLower();
                        result = GetHtmlTagType(tagName);
                        if (result != HtmlTagType.Empty)
                            return result;
                    }
                }
            }

            // then, detect tag from behind html
            if (!string.IsNullOrEmpty(behindHtml.Trim()))
            {
                // filter all tag enclosures
                behindHtml = HtmlTaggedContentRegex.Replace(behindHtml, "");
                // find the tail tags
                MatchCollection ms = HtmlTailTagRegex.Matches(behindHtml);
                if (ms != null && ms.Count > 0)
                {
                    // from the first to the last:
                    for (int i = 0; i < ms.Count; i++)
                    {
                        tagName = ms[i].Groups["tailtag"].Value.ToLower();
                        result = GetHtmlTagType(tagName);
                        if (result != HtmlTagType.Empty)
                            return result;
                    }
                }
            }
            tagName = "";
            return HtmlTagType.Empty;
        }

        public static HtmlTagType GetHtmlTagType(string tagName)
        {
            tagName = tagName.Trim().ToLower();

            if (HtmlTags.Contains(tagName))
                return (HtmlTagType)HtmlTags[tagName];
            return HtmlTagType.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string EncodeHtml(string html)
        {
            StringBuilder sb = new StringBuilder(html);
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace(" ", "&nbsp;");
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string DecodeHtml(string html)
        {
            StringBuilder sb = new StringBuilder(html);
            sb.Replace("&lt;", "<");
            sb.Replace("&gt;", ">");
            sb.Replace("&nbsp;", " ");
            return sb.ToString();
        }
       
        #endregion
    }
}
