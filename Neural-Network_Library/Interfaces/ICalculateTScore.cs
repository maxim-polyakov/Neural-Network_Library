using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface ICalculateTScore
    {
        /// <returns>True if the goal is to minimize the score.</returns>
        bool ShouldMinimize { get; }

        /// <summary>
        /// Calculate this T's score.
        /// </summary>
        ///
        /// <param name="T">The T.</param>
        /// <returns>The score.</returns>
        double CalculateScore(IT T);
    }
}
