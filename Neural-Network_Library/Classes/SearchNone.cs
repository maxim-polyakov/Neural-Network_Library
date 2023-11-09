using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SearchNone : IBayesSearch
    {
        #region IBayesSearch Members

        /// <inheritdoc/>
        public void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork,
                         IMLDataSet theData)
        {
        }

        /// <inheritdoc/>
        public bool Iteration()
        {
            return false;
        }

        #endregion
    }
}
