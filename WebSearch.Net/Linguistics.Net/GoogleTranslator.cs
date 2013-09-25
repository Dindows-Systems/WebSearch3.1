using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace WebSearch.Linguistics.Net
{
    public class GoogleTranslator
    {
        #region Translation Pairs

        /// <summary>
        /// Chinese to English
        /// </summary>
        public const string CN_to_EN = "zh-CN|en";

        /// <summary>
        /// English to Chinese
        /// </summary>
        public const string EN_to_CN = "en|zh-CN";

        /// <summary>
        /// English to Japanese
        /// </summary>
        public const string EN_to_JA = "en|ja";

        #endregion

        #region Regular Expressions

        private static readonly Regex googleResultRegex = 
            new Regex("<div id=result_box dir=ltr>(?<result>.*?)</div>", RegexOptions.Compiled);

        #endregion

        /// <summary>
        /// Get target encoding according to the lang pair
        /// </summary>
        /// <param name="langPair"></param>
        /// <returns></returns>
        private static Encoding GetEncodingFromLangpair(string langPair)
        {
            if (langPair == EN_to_CN)
                return Encoding.UTF8;
            if (langPair == EN_to_JA)
                return Encoding.GetEncoding("Shift_JIS");

            return Encoding.UTF8;
        }

        /// <summary>
        /// Get the translate result from google's response html
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string GetTranslateResult(string html)
        {
            Match m = googleResultRegex.Match(html);
            if (m != null && m.Success)
                return m.Groups["result"].Value;
            else
                return null;
        }

        #region Google Url

        private const string _engineUrl = "http://translate.google.com/translate_t";

        #endregion

        #region Public Methods

        /// <summary>
        /// Translate the text according to the language pair
        /// </summary>
        /// <param name="text"></param>
        /// <param name="langPair"></param>
        /// <returns></returns>
        public static string Translate(string text, string langPair)
        {
            string url = _engineUrl +
                "?text=" + HttpUtility.UrlEncode(text) +
                "&langpair=" + langPair;
            string html = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.01; Windows NT 5.0)";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                    return null;
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, GetEncodingFromLangpair(langPair));
                html = reader.ReadToEnd();
                reader.Close();
                stream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                // request failed
                return null;
            }
            // get the translation result from the html
            return GetTranslateResult(html);
        }

        #endregion
    }
}
