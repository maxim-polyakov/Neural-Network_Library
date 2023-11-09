using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BayesianFactory
    {
        /// <summary>
        /// Create a bayesian network.
        /// </summary>
        /// <param name="architecture">The architecture to use.</param>
        /// <param name="input">The input neuron count.</param>
        /// <param name="output">The output neuron count.</param>
        /// <returns>The new bayesian network.</returns>
        public IMLMethod Create(String architecture, int input,
                                int output)
        {
            var method = new BayesianNetwork { Contents = architecture };
            return method;
        }
    }
}
