using System;
using System.Collections.Generic;
using System.Text;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.Tokenize;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.Chunker;
using OpenNLP.Tools.NameFind;
using OpenNLP.Tools.Lang.English;
using System.Collections;
using OpenNLP.Tools.Coreference.Similarity;
using System.Text.RegularExpressions;
using WebSearch.Common.Net;

namespace WebSearch.Linguistics.Net
{
    public static class SharpNLPHelper
    {
        #region OpenNLP Members

        private static string _modelPath = XmlHelper.ReadNode(
            Config.SettingPath + "Dictionary.xml", "SharpNLP").
            Attributes["_Path"].Value;

        private static MaximumEntropySentenceDetector _sentenceDetector;
        private static EnglishMaximumEntropyTokenizer _tokenizer;
        private static EnglishMaximumEntropyPosTagger _posTagger;
        private static EnglishTreebankChunker _chunker;
        private static EnglishTreebankParser _parser;
        private static EnglishNameFinder _nameFinder;
        private static TreebankLinker _coreferenceFinder;

        #endregion

        #region OpenNLP Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        public static string[] SplitSentences(string paragraph)
        {
            if (_sentenceDetector == null)
                _sentenceDetector = new EnglishMaximumEntropySentenceDetector(
                    _modelPath + "EnglishSD.nbin");

            return _sentenceDetector.SentenceDetect(paragraph);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public static string[] TokenizeSentence(string sentence)
        {
            if (_tokenizer == null)
                _tokenizer = new EnglishMaximumEntropyTokenizer(
                    _modelPath + "EnglishTok.nbin");

            return _tokenizer.Tokenize(sentence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static string[] PosTagTokens(string[] tokens)
        {
            if (_posTagger == null)
                _posTagger = new EnglishMaximumEntropyPosTagger(
                    _modelPath + "EnglishPOS.nbin", _modelPath + @"\Parser\tagdict");

            return _posTagger.Tag(tokens);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static string ChunkSentence(string[] tokens, string[] tags)
        {
            if (_chunker == null)
                _chunker = new EnglishTreebankChunker(_modelPath + "EnglishChunk.nbin");

            return _chunker.GetChunks(tokens, tags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public static Parse ParseSentence(string sentence)
        {
            if (_parser == null)
                _parser = new EnglishTreebankParser(_modelPath, true, false);

            return _parser.DoParse(sentence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public static string FindNames(string sentence)
        {
            if (_nameFinder == null)
                _nameFinder = new EnglishNameFinder(_modelPath + "namefind\\");

            string[] models = new string[] { "date", "location", "money",
                "organization", "percentage", "person", "time" };
            return _nameFinder.GetNames(models, sentence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sentenceParse"></param>
        /// <returns></returns>
        public static string FindNames(Parse sentenceParse)
        {
            if (_nameFinder == null)
                _nameFinder = new EnglishNameFinder(_modelPath + "namefind\\");

            string[] models = new string[] { "date", "location", "money", 
                "organization", "percentage", "person", "time" };
            return _nameFinder.GetNames(models, sentenceParse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sentences"></param>
        /// <returns></returns>
        public static string IdentifyCoreferents(string[] sentences)
        {
            if (_coreferenceFinder == null)
                _coreferenceFinder = new TreebankLinker(_modelPath + "coref");

            List<Parse> parsedSentences = new List<Parse>();

            foreach (string sentence in sentences)
            {
                OpenNLP.Tools.Parser.Parse sentenceParse = ParseSentence(sentence);
                //string findNames = FindNames(sentenceParse);
                parsedSentences.Add(sentenceParse);
            }
            return _coreferenceFinder.GetCoreferenceParse(parsedSentences.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="posTaggedSentence"></param>
        /// <returns></returns>
        public static Model.Net.Gender GetGender(string posTaggedSentence)
        {
            //temp:
            return Model.Net.Gender.GetNeuter(1.0F);

            string result = GenderModel.GenderMain(
                _modelPath + "coref\\gen", posTaggedSentence);

            if (result == null)
                Model.Net.Gender.GetNeuter(1.0F);
    
            Match match = Regex.Match(result,
                @"m=(?<male>0.\d*)\sf=(?<female>0.\d*)\sn=(?<neuter>0.\d*)");
            if (match == null) return null;

            // get the male, female and neuter's degree
            float male = float.Parse(match.Groups["male"].Value);
            float female = float.Parse(match.Groups["female"].Value);
            float neuter = float.Parse(match.Groups["neuter"].Value);

            if (neuter > female && neuter > male)
                return Model.Net.Gender.GetNeuter(neuter);
            if (female > neuter && female > male)
                return Model.Net.Gender.GetFemale(female);
            else return Model.Net.Gender.GetMale(male);
        }

        #endregion
    }
}
