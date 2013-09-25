using System;
using System.Collections.Generic;

namespace SJTU.CS.Apex.WebSearch.DM.BayesClassifier
{
    #region Interfaces
	/// <summary>
	/// Classifier methods.
	/// </summary>
	public interface IClassifier
	{
		/// <summary>
		/// Trains this Category from a word or phrase<\summary>
		void TeachPhrases(string cat, string[] phrases);

		/// <summary>
		/// Trains this Category from a word or phrase<\summary>
		void TeachCategory(string cat, System.IO.TextReader tr);

		/// <summary>
		/// Classifies a text<\summary>
		/// <returns>
		/// returns classification values for the text, the higher, the better is the match.</returns>
		Dictionary<string, double> Classify(System.IO.StreamReader tr);
	}
	/// <summary>
	/// Category methods.
	/// </summary>
	interface ICategory
	{
		string Name { get; }
		/// <summary>
		/// Reset all trained data<\summary>
		void Reset();
		/// <summary>
		/// Gets a PhraseCount for Phrase or 0 if phrase not present<\summary>
		int GetPhraseCount(string phrase);

		/// <summary>
		/// Trains this Category from a file<\summary>
		void TeachCategory(System.IO.TextReader reader);
		/// <summary>
		/// Trains this Category from a word or phrase<\summary>
		void TeachPhrase(string rawPhrase);
		/// <summary>
		/// Trains this Category from a word or phrase array<\summary>
		void TeachPhrases(string[] words);
		/// <value>
		/// Gets total number of word occurences in this category</value>
		int TotalWords { get; }
	}
    #endregion
}
