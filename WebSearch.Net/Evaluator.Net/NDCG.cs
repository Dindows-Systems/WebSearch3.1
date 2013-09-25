using System;
using System.Collections.Generic;
using System.Text;

namespace Evaluator.Net
{
    /// <summary>
    /// normalized discounted cumulative gain.
    /// taking both the degree of relevance and the rank position 
    /// (determined by the probability of relevance) of a document
    /// into account
    /// </summary>
    /// <see cref="IR evaluation methods for retrieving highly relevant documents"/>
    public class NDCG
    {
        protected uint[] _relevanceLevels = null;

        #region Relevance Levels

        /// <summary>
        /// Binary Relevance Levels
        /// </summary>
        public static readonly uint[] BinaryRelevanceLevels = new uint[] { 0, 1 };

        /// <summary>
        /// Five Grades Relevance Levels
        /// </summary>
        public static readonly uint[] GradedRelevanceLevels = new uint[] { 0, 1, 2, 3, 4 };

        #endregion

        public NDCG(uint[] relevLevels)
        {
            this._relevanceLevels = relevLevels;
        }

        /// <summary>
        /// Evaluate the relevances
        /// </summary>
        /// <param name="relevances">
        /// 'int' is for rank, uint is for relevance</param>
        /// <returns></returns>
        public float Evaluate(Dictionary<int, uint> relevances)
        {
            // first get the specially calculated normalization constant 
            // for making a perfect ordering obtain an NDCG value of 1;
            return 0;   
        }

        /// <summary>
        /// Evaluate the relevances
        /// Taking default rank as 1,2,3,... for the relevances
        /// </summary>
        /// <param name="relevances"></param>
        /// <returns></returns>
        public double Evaluate(uint[] relevances)
        {
            int size = relevances.Length;
            // first get the specially calculated normalization constant N
            // for making a perfect ordering obtain an NDCG value of 1;
            double N = 0, maxDCG = 0;
            uint maxRelev = _relevanceLevels[_relevanceLevels.Length - 1];
            for (int rank = 1; rank <= size; rank++)
                maxDCG += (Math.Pow(2, maxRelev) - 1)
                    / Math.Log(1 + rank);
            N = 1 / maxDCG;

            // calculate the result
            double DCG = 0;
            for (int rank = 1; rank <= size; rank++)
                DCG += (Math.Pow(2, relevances[rank - 1]) - 1)
                    / Math.Log(1 + rank);
            return DCG * N;
        }
    }
}
