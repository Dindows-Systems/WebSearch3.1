using System;
using System.Collections.Generic;
using System.Text;

namespace SJTU.CS.Apex.WebSearch.DM.BayesClassifier
{
	/// <summary>
	/// serves to exclude certain words from the bayesian classification</summary>
	class ExcludedWords  
	{
		/// <summary>
		/// List of english words i'm not interested in</summary>
		/// <remarks>
		/// You might use frequently used words for this list
		/// </remarks>
		static string[] enu_most_common =
		{
			 "the", 
			 "to", 
			 "and", 
			 "a", 
			 "an", 
			 "in", 
			 "is", 
			 "it", 
			 "you", 
			 "that", 
			 "was", 
			 "for", 
			 "on", 
			 "are", 
			 "with", 
			 "as", 
			 "be", 
			 "been", 
			 "at", 
			 "one", 
			 "have", 
			 "this", 
			 "what", 
			 "which", 
		};

		Dictionary<string, int> m_Dict;

		public ExcludedWords()
		{
			m_Dict = new Dictionary<string, int>();
		}

		/// <summary>
		/// Initializes for english</summary>
		public void InitDefault()
		{
			Init(enu_most_common);
		}
		public void Init(string[] excluded)
		{
			m_Dict.Clear();
			for (int i = 0; i < excluded.Length; i++)
			{
				m_Dict.Add(excluded[i], i);
			}
		}
		/// <summary>
		/// checks to see if a word is to be excluded</summary>
		public bool IsExcluded(string word)
		{
			return m_Dict.ContainsKey(word);
		}

	}
}
