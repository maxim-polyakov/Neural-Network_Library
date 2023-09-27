using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface ICalculateScore
    {
        /// <returns>True if the goal is to minimize the score.</returns>
        bool ShouldMinimize { get; }

        /// <summary>
        /// Calculate this network's score.
        /// </summary>
        ///
        /// <param name="network">The network.</param>
        /// <returns>The score.</returns>
        double CalculateScore(IMLRegression network);
    }
}
