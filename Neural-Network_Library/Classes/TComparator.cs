using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class TComparator : IComparer<IT>
    {
        /// <summary>
        /// The method to calculate the score.
        /// </summary>
        ///
        private readonly ICalculateTScore _calculateScore;

        /// <summary>
        /// Construct the T comparator.
        /// </summary>
        ///
        /// <param name="theCalculateScore">The score calculation object to use.</param>
        public TComparator(ICalculateTScore theCalculateScore)
        {
            _calculateScore = theCalculateScore;
        }

        /// <value>The score calculation object.</value>
        public ICalculateTScore CalculateScore
        {
            get { return _calculateScore; }
        }

        #region IComparer<IT> Members

        /// <summary>
        /// Compare two Ts.
        /// </summary>
        ///
        /// <param name="T1">The first T.</param>
        /// <param name="T2">The second T.</param>
        /// <returns>Zero if equal, or less than or greater than zero to indicate
        /// order.</returns>
        public int Compare(IT T1, IT T2)
        {
            return T1.Score.CompareTo(T2.Score);
        }

        #endregion

        /// <summary>
        /// Apply a bonus, this is a simple percent that is applied in the direction
        /// specified by the "should minimize" property of the score function.
        /// </summary>
        ///
        /// <param name="v">The current value.</param>
        /// <param name="bonus">The bonus.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyBonus(double v, double bonus)
        {
            double amount = v * bonus;
            if (_calculateScore.ShouldMinimize)
            {
                return v - amount;
            }
            return v + amount;
        }

        /// <summary>
        /// Apply a penalty, this is a simple percent that is applied in the
        /// direction specified by the "should minimize" property of the score
        /// function.
        /// </summary>
        ///
        /// <param name="v">The current value.</param>
        /// <param name="bonus">The penalty.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyPenalty(double v, double bonus)
        {
            double amount = v * bonus;
            return _calculateScore.ShouldMinimize ? v - amount : v + amount;
        }

        /// <summary>
        /// Determine the best score from two scores, uses the "should minimize"
        /// property of the score function.
        /// </summary>
        ///
        /// <param name="d1">The first score.</param>
        /// <param name="d2">The second score.</param>
        /// <returns>The best score.</returns>
        public double BestScore(double d1, double d2)
        {
            return _calculateScore.ShouldMinimize ? Math.Min(d1, d2) : Math.Max(d1, d2);
        }


        /// <summary>
        /// Determine if one score is better than the other.
        /// </summary>
        ///
        /// <param name="d1">The first score to compare.</param>
        /// <param name="d2">The second score to compare.</param>
        /// <returns>True if d1 is better than d2.</returns>
        public bool IsBetterThan(double d1, double d2)
        {
            return _calculateScore.ShouldMinimize ? d1 < d2 : d1 > d2;
        }
    }
}
