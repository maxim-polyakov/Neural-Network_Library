﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IBayesSearch
    {
        /// <summary>
        /// Init the search object.
        /// </summary>
        /// <param name="theTrainer">The trainer to use.</param>
        /// <param name="theNetwork">The network to use.</param>
        /// <param name="theData">The data to use.</param>
        void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork, IMLDataSet theData);

        /// <summary>
        /// Perform an iteration. 
        /// </summary>
        /// <returns>True to continue.</returns>
        bool Iteration();
    }
}
