using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public sealed class SyntValidate
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        ///
        private SyntValidate()
        {
        }

        /// <summary>
        /// Validate a network for training.
        /// </summary>
        ///
        /// <param name="network">The network to validate.</param>
        /// <param name="training">The training set to validate.</param>
        public static void ValidateNetworkForTraining(IContainsFlat network,
                                                      IMLDataSet training)
        {
            int inputCount = network.Flat.InputCount;
            int outputCount = network.Flat.OutputCount;

            if (inputCount != training.InputSize)
            {
                throw new NeuralNetworkError("The input layer size of "
                                             + inputCount + " must match the training input size of "
                                             + training.InputSize + ".");
            }

            if ((training.IdealSize > 0)
                && (outputCount != training.IdealSize))
            {
                throw new NeuralNetworkError("The output layer size of "
                                             + outputCount + " must match the training input size of "
                                             + training.IdealSize + ".");
            }
        }
    }
}
