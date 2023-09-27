using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class GScoreAdapter : ICalculateTScore
    {
        /// <summary>
        /// The calculate score object to use.
        /// </summary>
        ///
        private readonly ICalculateScore _calculateScore;

        /// <summary>
        /// Construct the adapter.
        /// </summary>
        ///
        /// <param name="calculateScore">The CalculateScore object to use.</param>
        public GScoreAdapter(ICalculateScore calculateScore)
        {
            _calculateScore = calculateScore;
        }

        #region ICalculateTScore Members

        /// <summary>
        /// Calculate the T's score.
        /// </summary>
        ///
        /// <param name="T">The T to calculate for.</param>
        /// <returns>The calculated score.</returns>
        public double CalculateScore(IT T)
        {
            var network = (IMLRegression)T.Organism;
            return _calculateScore.CalculateScore(network);
        }


        /// <returns>True, if the score should be minimized.</returns>
        public bool ShouldMinimize
        {
            get { return _calculateScore.ShouldMinimize; }
        }

        #endregion
    }
}
