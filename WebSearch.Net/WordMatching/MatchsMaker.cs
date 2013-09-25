/*
Matching two strings
Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
Copyright (c) 2005 by Thanh Ngoc Dao, All rights reserved.
---------
Thanks to Malcolm Crowe,Troy Simpson, Jeff Martin for the .NET WordNet port
Thanks to Carl Mercier for his french characters conversion function.

Please test carefully before using.
http://www.codeproject.com/cs/algorithms/improvestringsimilarity.asp
 * 
Introduction
This article describes a way of capturing the similarity between two strings (or words). String similarity is a confidence score that reflects the relation between the meanings of two strings, which usually consists of multiple words or acronyms. Currently, in this approach I am more concerned on the measurement which reflects the relation between the patterns of the two strings, rather than the meaning of the words.

I implemented this algorithm when I was developing a tool to make the matching between XML schemas semi-automatic.

Preparing the ground
In this paper, I have implemented two algorithms:

Levenshtein algorithm[1] 
The Kuhn-Munkres algorithm (also known as the Hungarian method)[2] 
Without going "deep into theory", if you want to understand these algorithms, please read about them in the algorithm books. Other information about them can be reached at:

Levenshtein 
The optimal assignment problem 
Problem
The string similarity algorithm was developed to satisfy the following requirements:

A true reflection of lexical similarity - strings with small differences should be recognized as being similar. In particular, a significant sub-string overlap should point to a high level of similarity between the strings. 
A robustness to changes of word order- two strings which contain the same words, but in a different order, should be recognized as being similar. On the other hand, if one string is just a random anagram of the characters contained in the other, then it should (usually) be recognized as dissimilar. 
Language independence - the algorithm should work not only in English, but also in many different languages. 
Solution
The similarity is calculated in three steps:

Partition each string into a list of tokens. 
Computing the similarity between tokens by using a string edit-distance algorithm (extension feature: semantic similarity measurement using the WordNet library). 
Computing the similarity between two token lists. 
*/


using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WordsMatching
{
    /// <summary>
    /// Summary description for StringMatcher.
    /// </summary>
    /// 
    public delegate float Similarity(string s1, string s2);

    public class MatchsMaker
    {
        private string _lString, _rString;
        private string[] _leftTokens, _rightTokens;
        private int leftLen, rightLen;
        private float[,] cost;
        Similarity getSimilarity;

        private bool _accentInsensitive;

        public MatchsMaker(string left, string right) : this(left, right, false) { }
        public MatchsMaker(string left, string right, bool accentInsensitive)
        {
            _accentInsensitive = accentInsensitive;

            _lString = left;
            _rString = right;

            if (_accentInsensitive)
            {
                _lString = StripAccents(_lString);
                _rString = StripAccents(_rString);
            }

            MyInit();
        }

        private string StripAccents(string input)
        {
            string beforeConversion = "‡¿‚¬‰ƒ·¡È…Ë»Í ÎÀÏÃÓŒÔœÚ“Ù‘ˆ÷˘Ÿ˚€¸‹Á«íÒ";
            string afterConversion = "aAaAaAaAeEeEeEeEiIiIiIoOoOoOuUuUuUcC'n";

            System.Text.StringBuilder sb = new System.Text.StringBuilder(input);

            for (int i = 0; i < beforeConversion.Length; i++)
            {
                char beforeChar = beforeConversion[i];
                char afterChar = afterConversion[i];

                sb.Replace(beforeChar, afterChar);
            }

            sb.Replace("?", "oe");
            sb.Replace("?", "ae");

            return sb.ToString();
        }

        private void MyInit()
        {
            ISimilarity editdistance = new Leven();
            getSimilarity = new Similarity(editdistance.GetSimilarity);

            //ISimilarity lexical=new LexicalSimilarity() ;
            //getSimilarity=new Similarity(lexical.GetSimilarity) ;

            Tokeniser tokeniser = new Tokeniser();
            _leftTokens = tokeniser.Partition(_lString);
            _rightTokens = tokeniser.Partition(_rString);
            if (_leftTokens.Length > _rightTokens.Length)
            {
                string[] tmp = _leftTokens;
                _leftTokens = _rightTokens;
                _rightTokens = tmp;
                string s = _lString; _lString = _rString; _rString = s;
            }

            leftLen = _leftTokens.Length - 1;
            rightLen = _rightTokens.Length - 1;
            Initialize();

        }

        private void Initialize()
        {
            cost = new float[leftLen + 1, rightLen + 1];
            for (int i = 0; i <= leftLen; i++)
                for (int j = 0; j <= rightLen; j++)
                    cost[i, j] = getSimilarity(_leftTokens[i], _rightTokens[j]);
        }


        public float GetScore()
        {
            BipartiteMatcher match = new BipartiteMatcher(_leftTokens, _rightTokens, cost);
            return match.Score;
        }


        public float Score
        {
            get
            {
                return GetScore();
            }
        }

    }
}
