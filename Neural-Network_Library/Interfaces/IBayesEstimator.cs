using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IBayesEstimator
    {
        /// <summary>
        /// Init the estimator.
        /// </summary>
        /// <param name="theTrainer">The trainer.</param>
        /// <param name="theNetwork">The network.</param>
        /// <param name="theData">The data.</param>
        void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork, IMLDataSet theData);

        /// <summary>
        /// Perform an iteration.
        /// </summary>
        /// <returns>True, if we should contune.</returns>
        bool Iteration();
    }
}
