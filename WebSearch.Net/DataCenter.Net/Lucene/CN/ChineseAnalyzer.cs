using System;
using System.IO;
using System.Text;
using System.Collections;
using Lucene.Net.Analysis;

namespace WebSearch.DataCenter.Net.Lucene.CN
{
	/// <summary>
	/// Title: ChineseAnalyzer
	/// Description:
	///   Subclass of org.apache.lucene.analysis.Analyzer
	///   build from a ChineseTokenizer, filtered with ChineseFilter.
	/// Copyright:   Copyright (c) 2007
	/// Company:
	/// @author Jialiang Ge
	/// @version $Id: ChineseAnalyzer, v 1.2 2007/04/22 20:54:47 ehatcher Exp $
	/// </summary>
    public class ChineseAnalyzer : Analyzer
	{
		public ChineseAnalyzer() 
		{
		}

		/// <summary>
		/// Creates a TokenStream which tokenizes all the text in the provided Reader.
		/// </summary>
		/// <returns>A TokenStream build from a ChineseTokenizer filtered with ChineseFilter.</returns>
		public override sealed TokenStream TokenStream(String fieldName, TextReader reader) 
		{
			TokenStream result = new ChineseTokenizer(reader);
			result = new ChineseFilter(result);
			return result;
		}
	}
}
