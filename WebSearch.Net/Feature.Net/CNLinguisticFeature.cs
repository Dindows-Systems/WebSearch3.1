using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Common.Net;
using System.Text.RegularExpressions;
using WebSearch.Model.Net;
using WebSearch.Linguistics.Net;

namespace WebSearch.Feature.Net
{
    public class CNLinguisticFeature : LinguisticFeature
    {
        #region Regular Expressions

        public static readonly Regex chineseWordRegex = new Regex(
            "[\u4e00-\u9fa5]", RegexOptions.None);

        public static readonly Regex chineseSentRegex = new Regex(
            "[¡££¿£¡\n]", RegexOptions.None);

        #endregion

        internal CNLinguisticFeature(string text)
        {
            this._text = text;
            this._lang = SupportedLanguage.Chinese;
        }

        protected override void InitializePosTagFeatures()
        {
            // initialize the tag-related features:
            this._prepNum = this._ppNum = this._propNounNum = 0;
            this._numeralNum = this._conjNum = 0;
            this._avgPolysemy = 0.0F;

            string[] tags = ChineseHelper.PosTagTokens(_text);
            if (tags.Length == 0)
                tags = new string[] { "ns" };
            foreach (string tag in tags)
            {
                // update the numeral, prep, 
                // pron, conj, proper noun.
                #region tag: num detection
                if (POSTag.IsNumeral(tag))
                { _numeralNum++; continue; }
                #endregion
                #region tag: prep detection
                if (POSTag.IsPreposition(tag))
                { _prepNum++; continue; }
                #endregion
                #region tag: pronoun detection
                if (POSTag.IsPronoun(tag))
                { _ppNum++; continue; }
                #endregion
                #region tag: conj detection
                if (POSTag.IsConjunction(tag))
                { _conjNum++; continue; }
                #endregion
                #region tag: noun detection
                if (POSTag.IsNoun(tag))
                {
                    this._nounNum++;
                    // proper noun
                    if (POSTag.IsProperNoun(tag))
                        _propNounNum++;
                    continue;
                }
                #endregion
                #region tag: verb detection
                if (POSTag.IsVerb(tag))
                { _verbNum++; continue; }
                #endregion
            }
            // get the end with verb info. 1 start with verb. 2. end with verb
            // 3. end with verb + symbol
            this._endWithVerb = (POSTag.IsVerb(tags[0]) || 
                ((tags.Length > 0) && POSTag.IsVerb(tags[tags.Length - 1])) ||
                ((tags.Length > 1) && POSTag.IsSymbol(tags[tags.Length - 1])
                && POSTag.IsVerb(tags[tags.Length - 2]))) ? true : false;
        }

        #region morphological feature

        protected override void InitializeMorphologicalFeatures()
        {
            // english regex match:
            int englishCount = englishWordRegex.Matches(_text).Count;
            // chinese regex match:
            int chineseCount = chineseWordRegex.Matches(_text).Count;
            // --- get the average word number
            this._wordNum = chineseCount + englishCount;
            
            // --- get the sentence number
            this._sentNum = 0;
            string[] sentences = chineseSentRegex.Split(_text);
            int total = 0;
            foreach (string sentence in sentences)
                if (sentence.Trim() != "")
                {
                    total += sentence.Length;
                    _sentNum++;
                }

            this._avgWordLen = (float)total / (float)_wordNum;
            this._avgSentLen = (float)_wordNum / (float)_sentNum;
        }

        #endregion

        #region syntactical feature

        protected override void InitializeSyntacticalFeatures()
        {
            
        }

        #endregion

        #region semantic feature

        protected override void InitializeSemanticFeatures()
        {

        }

        #endregion

        #region Other Features

        public override List<string> GetAcronyms()
        {
            List<string> result = new List<string>();
            // for chinese text, acronyms may be:
            string[] spells = new string[_text.Length];
            for (int i = 0; i < _text.Length; i++)
                spells[i] = ChineseSpeller.Spell(_text[i].ToString());
            // 1. combination of spelling of chars
            string acronym1 = "";
            foreach (string sp in spells)
                acronym1 += sp;
            acronym1 = acronym1.Trim().ToLower();
            result.Add(acronym1);
            // 2. combination of first letters
            string acronym2 = "";
            foreach (string sp in spells)
                if (sp.Length > 0)
                    acronym2 += sp[0].ToString();
            acronym2 = acronym2.Trim().ToUpper();
            result.Add(acronym2);
            // 3. translation of the text
            string acronym3 = GoogleTranslator.Translate(
                _text, GoogleTranslator.CN_to_EN);
            result.Add(acronym3);
            // 4. the text itself
            result.Add(_text);
            return result;
        }

        #endregion
    }
}
