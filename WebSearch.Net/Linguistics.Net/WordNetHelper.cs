using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using SharpWordNet;
using WebSearch.Common.Net;

namespace WebSearch.Linguistics.Net
{
    public class WordNetHelper
    {
        #region WordNet Members

        private static string _wordNetPath = XmlHelper.ReadNode(
            Config.SettingPath + "Dictionary.xml", "Wordnet").
            Attributes["_Path"].Value;

        private static WordNetEngine _engine = null;

        #endregion

        public enum POS  // part-of-speech
        {
            noun, verb, adjective, adverb
        }

        #region WordNet Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nounLemma"></param>
        /// <returns></returns>
        public static int GetSynsetNum(string lemma, POS pos)
        {
            if (_engine == null)
                _engine = new DataFileEngine(_wordNetPath);

            Synset[] synsets = _engine.GetSynsets(lemma, pos.ToString());
            return synsets.Length;
        }

        #endregion
    }
}
