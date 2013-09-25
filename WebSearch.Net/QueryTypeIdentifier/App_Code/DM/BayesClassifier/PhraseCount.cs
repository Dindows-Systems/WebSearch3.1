using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SJTU.CS.Apex.WebSearch.DM.BayesClassifier
{
	/// <summary>
	/// stores occurence counter for words or phrases</summary>
	class PhraseCount
	{
		string m_RawPhrase;

		public PhraseCount(string rawPhrase)
		{
			m_RawPhrase = rawPhrase;
			m_Count = 0;
		}

		/// <value>
		/// Stores the raw Phrase, the real matching phrase is stored as key to this element</value>
		/// <seealso cref="DePhrase(string)">
		/// See DePhrase </seealso>
		public string RawPhrase
		{
			get { return m_RawPhrase; }
			//set { m_RawPhrase = value; }
		}

		int m_Count;

		/// <value>
		/// Count Accessor</value>
		public int Count
		{
			get { return m_Count; }
			set { m_Count = value; }
		}
	}
}
