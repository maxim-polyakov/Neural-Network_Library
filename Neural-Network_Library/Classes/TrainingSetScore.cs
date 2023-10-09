using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class TrainingSetScore : ICalculateScore
    {
        /// <summary>
        /// The training set.
        /// </summary>
        ///
        private readonly IMLDataSet _training;

        /// <summary>
        /// Construct a training set score calculation.
        /// </summary>
        ///
        /// <param name="training">The training data to use.</param>
        public TrainingSetScore(IMLDataSet training)
        {
            _training = training;
        }

        #region ICalculateScore Members

        /// <summary>
        /// Calculate the score for the network.
        /// </summary>
        ///
        /// <param name="method">The network to calculate for.</param>
        /// <returns>The score.</returns>
        public double CalculateScore(IMLRegression method)
        {
            return CalculateRegressionError.CalculateError(method, _training);
        }

        /// <summary>
        /// A training set based score should always seek to lower the error,
        /// as a result, this method always returns true.
        /// </summary>
        ///
        /// <returns>Returns true.</returns>
        public bool ShouldMinimize
        {
            get { return true; }
        }

        #endregion
    }
}
