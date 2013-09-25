using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Common.Net;
using System.Collections;
using Lucene.Net.Analysis.Standard;
using WebSearch.DataCenter.Net.Lucene.CN;
using Lucene.Net.Analysis;

namespace WebSearch.DataCenter.Net.Lucene
{
    public class LuceneAnalyzer
    {
        private static Hashtable _analyzers = new Hashtable();

        public static Analyzer Get(SupportedLanguage language)
        {
            Analyzer analyzer = null;

            if (_analyzers[language] == null)
            {
                switch (language) // according to the language
                {
                    case SupportedLanguage.Chinese:
                        analyzer = new ChineseAnalyzer();
                        break;
                    case SupportedLanguage.English:
                        analyzer = new StandardAnalyzer();
                        break;
                    default:
                        analyzer = new StandardAnalyzer();
                        break;
                }
                _analyzers.Add(language, analyzer);
            }
            else
                analyzer = (Analyzer)_analyzers[language];

            return analyzer;
        }
    }
}
