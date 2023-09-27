using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public enum BayesianInit
    {
        /// <summary>
        /// No init, do not change anything.
        /// </summary>
        InitNoChange,

        /// <summary>
        /// Start with no connections.
        /// </summary>
        InitEmpty,

        /// <summary>
        /// Init as Naive Bayes.
        /// </summary>
        InitNaiveBayes
    }
}
