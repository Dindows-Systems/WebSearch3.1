using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WebSearch.Common.Net;
using System.IO;
using WebSearch.Model.Net;
using OpenNLP.Tools.Parser;
using System.Collections;
using WebSearch.Maths.Net;
using WebSearch.Linguistics.Net;

namespace WebSearch.Feature.Net
{
    public class LinguisticFeature
    {
        #region Regular Expressions

        public static readonly Regex englishWordRegex = new Regex(
            @"\b([\u0041-\u007A\x41-\x5A\x61-\x7A\x30-\x39])+\b", RegexOptions.None);

        public static readonly Regex acronymRegex = new Regex(
            @"[A-Z][A-Z\-]+[a-zA-Z0-9\.]{0,9}", RegexOptions.None);

        public static readonly Regex fullCapitalRegex = new Regex(
            @"\b[A-Z][A-Z0-9]+\b", RegexOptions.None);

        public static readonly Regex firstCapitalRegex = new Regex(
            @"\b[A-Z]", RegexOptions.None);

        #endregion

        #region Constructors

        protected string _text = null;

        public string Text
        {
            get { return _text; }
        }

        protected SupportedLanguage _lang;

        public SupportedLanguage Language
        {
            get { return _lang; }
        }

        protected bool _isPhrase = false;

        /// <summary>
        /// Whether the text is a phrase or a (group of) complete sentence
        /// </summary>
        public bool IsPhrase
        {
            get { return _isPhrase; }
        }

        public static LinguisticFeature Get(string text, 
            SupportedLanguage lang, bool isPhrase)
        {
            LinguisticFeature feature = Get(text, lang);
            feature._isPhrase = isPhrase;
            return feature;
        }

        public static LinguisticFeature Get(string text, SupportedLanguage lang)
        {
            switch (lang) // according to language
            {
                case SupportedLanguage.Chinese:
                    return new CNLinguisticFeature(text);
                case SupportedLanguage.English:
                    return new LinguisticFeature(text);
                default:
                    return new LinguisticFeature(text);
            }
        }

        public static LinguisticFeature Get(string text, bool isPhrase)
        {
            LinguisticFeature feature = Get(text);
            feature._isPhrase = isPhrase;
            return feature;
        }

        public static LinguisticFeature Get(string text)
        {
            // identify the text's language
            SupportedLanguage language = SupportedLanguage.Chinese;
            if (Encoding.Default.GetByteCount(text) == text.Length)
                language = SupportedLanguage.English;

            return Get(text, language);
        }

        internal LinguisticFeature(string text)
        {
            this._text = text;
            this._lang = SupportedLanguage.English;
        }

        internal LinguisticFeature() { }

        #endregion

        #region NLP Parse of the Text

        protected virtual void InitializePosTagFeatures()
        {
            // split the whole text into sentences:
            string[] sentences = SharpNLPHelper.SplitSentences(_text);
            this._sentNum = sentences.Length;
            // 
            // initialize the tag-related features:
            this._prepNum = this._ppNum = this._propNounNum = 0;
            this._numeralNum = this._conjNum = this._verbNum = 0;
            this._endWithVerb = false; this._avgPolysemy = 0.0F;

            string posTaggedSentence = "", tag, token;

            foreach (string sentence in sentences)
            {
                // pos tagger the sentence.
                string[] tokens = SharpNLPHelper.TokenizeSentence(sentence);
                string[] posTags = SharpNLPHelper.PosTagTokens(tokens);
                // statistics the tags
                for (int i = 0; i < posTags.Length; i++)
                {
                    tag = posTags[i]; token = tokens[i];
                    posTaggedSentence += token + @"/" + tag + " ";

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
                        // calculate polysemy
                        _avgPolysemy += WordNetHelper.GetSynsetNum(
                            token, WordNetHelper.POS.noun);
                        // proper noun
                        if (POSTag.IsProperNoun(tag))
                            _propNounNum++;
                        continue;
                    }
                    #endregion
                    #region tag: verb detection
                    if (POSTag.IsVerb(tag))
                    {
                        this._verbNum++;
                        // test whether the verb occurs at the end of the text
                        if (_endWithVerb == false && (i == 0 || i == posTags.Length - 1
                            || (i == posTags.Length - 2 &&
                            POSTag.IsSymbol(posTags[posTags.Length - 1]))))
                            this._endWithVerb = true;
                        continue;
                    }
                    #endregion
                }
                //string parse = NLPHelper.IdentifyCoreferents
                // calculate syntactic depth
                // calculate syntactic links span
            }
            this._avgPolysemy = (_nounNum == 0) ?
                0 : _avgPolysemy / (float)_nounNum;
            
            // estimate the gender info of text
            this._gender = SharpNLPHelper.GetGender(posTaggedSentence);
        }

        #endregion

        #region morphological features

        protected virtual void InitializeMorphologicalFeatures()
        {
            // get the sentence num:
            if (_sentNum == Const.Invalid)
                InitializePosTagFeatures();
            // get the english word num:
            MatchCollection enMatches = englishWordRegex.Matches(_text);
            this._wordNum = enMatches.Count;
            int total = 0;
            foreach (Match word in enMatches)
                total += word.Value.Length;
            this._avgWordLen = (float)total / (float)_wordNum;
            this._avgSentLen = (float)_wordNum / (float)_sentNum;
        }

        protected int _wordNum = Const.Invalid;

        /// <summary>
        /// Number (#) of Words
        /// </summary>
        public virtual int WordNum
        {
            get
            {
                if (_wordNum == Const.Invalid)
                    InitializeMorphologicalFeatures();
                return _wordNum;
            }
            set { _wordNum = value; }
        }

        protected float _avgWordLen = Const.Invalid;

        /// <summary>
        /// Average Word Length
        /// average length of words in text, measured in numbers of characters.
        /// </summary>
        /// <remarks>not for Chinese</remarks>
        public virtual float AVGWordLen
        {
            get
            {
                if (_avgWordLen == Const.Invalid)
                    InitializeMorphologicalFeatures();
                return _avgWordLen;
            }
            set { _avgWordLen = value; }
        }

        protected int _sentNum = Const.Invalid;

        /// <summary>
        /// Sentence Number
        /// </summary>
        public virtual int SentenceNum
        {
            get
            {
                if (_sentNum == Const.Invalid)
                    InitializePosTagFeatures();
                return _sentNum;
            }
            set { _sentNum = value; }
        }

        protected float _avgSentLen = Const.Invalid;

        /// <summary>
        /// Average Sentence Length
        /// </summary>
        public virtual float AVGSentLen
        {
            get 
            {
                if (_avgSentLen == Const.Invalid)
                    InitializeMorphologicalFeatures();
                return _avgSentLen; 
            }
            set { _avgSentLen = value; }
        }

        protected int _avgMorphemeNum = Const.Invalid;

        /// <summary>
        /// Average # of Morphemes per Word 每个单词的平均词素数
        /// average number of morphemes per word in the text,
        /// using the CELEX morphological database.
        /// </summary>
        /// <remarks>Only for English</remarks>
        public virtual int AVGMorphemeNum
        {
            get
            {
                if (_avgMorphemeNum == Const.Invalid)
                    InitializeMorphologicalFeatures();
                return _avgMorphemeNum;
            }
            set { _avgMorphemeNum = value; }
        }

        protected int _suffixNum = Const.Invalid;

        /// <summary>
        /// # of Suffixed Tokens Word 带后缀的单词数
        /// the number of suffixed tokens is a more general method,
        /// which can lead to consistent results with any word form,
        /// using a bootstrapping method in order to extract the
        /// most frequent suffixes from the CELEX database.
        /// </summary>
        /// <remarks>Only for English</remarks>
        public virtual int SuffixNum
        {
            get
            {
                if (_suffixNum == Const.Invalid)
                    InitializeMorphologicalFeatures();
                return _suffixNum;
            }
            set { _suffixNum = value; }
        }

        protected int _propNounNum = Const.Invalid;

        /// <summary>
        /// # of Proper Nouns 专有名词数
        /// obtained through the POS tagger's analysis and 
        /// with a more robust method based on upper-case word form.
        /// </summary>
        /// <remarks>morphological feature</remarks>
        /// <returns></returns>
        public virtual int ProperNounNum
        {
            get
            {
                if (_propNounNum == Const.Invalid)
                {
                    InitializePosTagFeatures();
                }
                return _propNounNum;
            }
            set { _propNounNum = value; }
        }

        protected int _acronymNum = Const.Invalid;

        /// <summary>
        /// # of Acronyms 只取首字母的缩写词数
        /// detected using a simple pattern-matching technique.
        /// </summary>
        /// <remarks>Only for English</remarks>
        public virtual int AcronymNum
        {
            get
            {
                if (_acronymNum == Const.Invalid)
                    // acronym regex match:
                    _acronymNum = acronymRegex.Matches(_text).Count;
                return _acronymNum;
            }
            set { _acronymNum = value; }
        }

        protected Degree _capital = null;

        /// <summary>
        /// Capital degree of the text
        /// </summary>
        /// <remarks>Only for English</remarks>
        public virtual Degree Capital
        {
            get
            {
                if (_capital == null)
                {
                    // 1. # of words fully capital
                    int full = fullCapitalRegex.Matches(_text).Count;
                    // 2. # of words first capital
                    int first = firstCapitalRegex.Matches(_text).Count;
                    // for full capitals, they will be counted twice
                    // thus, they have power: 2
                    _capital = new Degree((float)(full + first) / (float)WordNum);
                }
                return _capital;
            }
            set { _capital = value; }
        }

        protected int _numeralNum = Const.Invalid;

        /// <summary>
        /// # of Numeral Values 数字的数量
        /// detected using a simple pattern-matching technique.
        /// </summary>
        public virtual int NumeralNum
        {
            get
            {
                if (_numeralNum == Const.Invalid)
                    InitializePosTagFeatures();
                return _numeralNum;
            }
            set { _numeralNum = value; }
        }

        protected int _unknownTokenNum = Const.Invalid;

        /// <summary>
        /// # of Unknown Tokens
        /// words marked up as such by the POS tagger, most are constructed words
        /// </summary>
        public virtual int UnknownTokenNum
        {
            get
            {
                if (_unknownTokenNum == Const.Invalid)
                    InitializePosTagFeatures();
                return _unknownTokenNum;
            }
            set { _unknownTokenNum = value; }
        }

        #endregion

        #region syntactical features

        protected virtual void InitializeSyntacticalFeatures()
        {
            // nothing to do
        }

        protected int _conjNum = Const.Invalid;

        /// <summary>
        /// # of Conjunctions
        /// detected through POS tagging
        /// </summary>
        public virtual int ConjunctionNum
        {
            get
            {
                if (_conjNum == Const.Invalid)
                {
                    InitializeSyntacticalFeatures();
                    InitializePosTagFeatures();
                }
                return _conjNum;
            }
            set { _conjNum = value; }
        }

        protected int _prepNum = Const.Invalid;

        /// <summary>
        /// # of Prepositions
        /// detected through POS tagging
        /// </summary>
        public virtual int PrepositionNum
        {
            get
            {
                if (_prepNum == Const.Invalid)
                {
                    InitializeSyntacticalFeatures();
                    InitializePosTagFeatures();
                }
                return _prepNum;
            }
            set { _prepNum = value; }
        }

        protected int _ppNum = Const.Invalid;

        /// <summary>
        /// # of Personal Pronouns
        /// detected through POS tagging
        /// </summary>
        public virtual int PersonalPronounNum
        {
            get
            {
                if (_ppNum == Const.Invalid)
                {
                    InitializeSyntacticalFeatures();
                    InitializePosTagFeatures();
                }
                return _ppNum;
            }
            set { _ppNum = value; }
        }

        protected int _nounNum = Const.Invalid;

        /// <summary>
        /// # of Nouns
        /// detected through POS tagging
        /// </summary>
        public virtual int NounNum
        {
            get
            {
                if (_nounNum == Const.Invalid)
                {
                    InitializeSyntacticalFeatures();
                    InitializePosTagFeatures();
                }
                return _nounNum;
            }
            set { _nounNum = value; }
        }

        protected int _verbNum = Const.Invalid;

        /// <summary>
        /// # of Verbs
        /// detected through POS tagging
        /// </summary>
        public virtual int VerbNum
        {
            get
            {
                if (_verbNum == Const.Invalid)
                {
                    InitializeSyntacticalFeatures();
                    InitializePosTagFeatures();
                }
                return _verbNum;
            }
            set { _verbNum = value; }
        }

        protected bool? _endWithVerb = null;

        public bool? EndWithVerb
        {
            get
            {
                if (_endWithVerb == null)
                    InitializePosTagFeatures();
                return _endWithVerb;
            }
            set { _endWithVerb = value; }
        }

        protected int _avgSyntacticDepth = Const.Invalid;

        /// <summary>
        /// Average Syntactic Depth (SYNTDEPTH)
        /// syntactic depth is a straightforward measure of syntactic 
        /// complexity in terms of hierarchy. It simply corresponds 
        /// to the maximum number of nested syntactic constituents in
        /// the query. It can be computed from the result of the 
        /// syntactic analyzer.
        /// </summary>
        public virtual int AVGSyntacticDepth
        {
            get
            {
                if (_avgSyntacticDepth == Const.Invalid)
                    InitializeSyntacticalFeatures();
                return _avgSyntacticDepth;
            }
            set { _avgSyntacticDepth = value; }
        }

        protected int _syntacticLinksSpan = Const.Invalid;

        /// <summary>
        /// Average Syntactic Links Span (SYNTDIST)
        /// total distance in terms of number of words divided by 
        /// the number of identified links. It can be computed 
        /// from the result of syntactic analyzer.
        /// </summary>
        public virtual int SyntacticLinksSpan
        {
            get
            {
                if (_syntacticLinksSpan == Const.Invalid)
                    InitializeSyntacticalFeatures();
                return _syntacticLinksSpan;
            }
            set { _syntacticLinksSpan = value; }
        }

        #endregion

        #region semantic features

        protected virtual void InitializeSemanticFeatures()
        {
            // nothing to do
        }

        protected float _avgPolysemy = (float)Const.Invalid;

        /// <summary>
        /// Average Polysemy Value (SYNSETS)
        /// corresponds to the number of synsets in the WordNet
        /// database each word belongs to. It is directly available
        /// in WordNet and roughly corresponds to the different
        /// meanings a given word can have.
        /// </summary>
        public virtual float AVGPolysemy
        {
            get
            {
                if (_avgPolysemy == (float)Const.Invalid)
                {
                    InitializeSemanticFeatures();
                    InitializePosTagFeatures();
                }
                return _avgPolysemy;
            }
            set { _avgPolysemy = value; }
        }

        protected Gender _gender = Gender.Invalid;

        /// <summary>
        /// The possible gender of the text's writer
        /// </summary>
        public virtual Gender Gender
        {
            get
            {
                if (!_gender.IsValid)
                {
                    InitializeSemanticFeatures();
                    InitializePosTagFeatures();
                }
                return _gender;
            }
        }

        #endregion

        #region Other Features

        /// <summary>
        /// get all the possible acronyms for the given text
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetAcronyms()
        {
            List<string> result = new List<string>();
            // for english text, acronym may be:
            // 1. comibination of first letters
            string acronym1 = "";
            // tokenize words
            string[] tokens = SharpNLPHelper.TokenizeSentence(_text);
            foreach (string token in tokens)
                if (token != null && token.Length > 0)
                    acronym1 += token[0];
            acronym1 = acronym1.ToUpper();
            result.Add(acronym1);
            // 2. _text itself
            result.Add(_text);
            return result;
        }

        #endregion
    }
}
