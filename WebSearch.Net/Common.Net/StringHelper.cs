using System;
using System.Collections.Generic;
using System.Text;
using WordsMatching;

namespace WebSearch.Common.Net
{
    public static class StringHelper
    {
        /// <summary>
        /// Get the similarity between two strings
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static Degree Similarity(string str1, string str2)
        {
            if (str1 == str2)
                return new Degree(1.0F);

            if (str1 == null)
                return new Degree(0);
            if (str2 == null)
                return new Degree(0);

            float plus = (str1.Contains(str2) || 
                str2.Contains(str1)) ? 0.2F : 0.0F;
            MatchsMaker match = new MatchsMaker(str1, str2);
            return new Degree(match.Score + plus);
        }
    }
}
