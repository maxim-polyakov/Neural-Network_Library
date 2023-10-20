using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BasicNeuralDataSet : BasicMLDataSet, INeuralDataSet
    {
        /// <summary>
        /// Construct a data set from input and ideal.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="ideal">The ideal data.</param>
        public BasicNeuralDataSet(double[][] input, double[][] ideal)
            : base(input, ideal)
        {
        }
    }
}
