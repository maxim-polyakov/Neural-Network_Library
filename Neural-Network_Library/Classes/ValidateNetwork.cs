using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ValidateNetwork
    {
        /// <summary>
        /// Validate that the specified data can be used with the method.
        /// </summary>
        /// <param name="method">The method to validate.</param>
        /// <param name="training">The training data.</param>
        public static void ValidateMethodToData(IMLMethod method, IMLDataSet training)
        {
            if (!(method is IMLInput) || !(method is IMLOutput))
            {
                throw new SyntError(
                    "This machine learning method is not compatible with the provided data.");
            }

            int trainingInputCount = training.InputSize;
            int trainingOutputCount = training.IdealSize;
            int methodInputCount = 0;
            int methodOutputCount = 0;

            if (method is IMLInput)
            {
                methodInputCount = ((IMLInput)method).InputCount;
            }

            if (method is IMLOutput)
            {
                methodOutputCount = ((IMLOutput)method).OutputCount;
            }

            if (methodInputCount != trainingInputCount)
            {
                throw new SyntError(
                    "The machine learning method has an input length of "
                    + methodInputCount + ", but the training data has "
                    + trainingInputCount + ". They must be the same.");
            }

            if (!(method is BasicPNN))
            {
                if (trainingOutputCount > 0 && methodOutputCount != trainingOutputCount)
                {
                    throw new SyntError(
                        "The machine learning method has an output length of "
                        + methodOutputCount
                        + ", but the training data has "
                        + trainingOutputCount + ". They must be the same.");
                }
            }
        }
    }
}
