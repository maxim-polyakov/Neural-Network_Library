using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public static class SyntesisSVMProblem
    {
        /// <summary>
        /// Syntesis the Synt dataset.
        /// </summary>
        ///
        /// <param name="training">The training data.</param>
        /// <param name="outputIndex"></param>
        /// <returns>The SVM problem.</returns>
        public static svm_problem Syntesis(IMLDataSet training,
                                         int outputIndex)
        {
            try
            {
                var result = new svm_problem { l = (int)training.Count };

                result.y = new double[result.l];
                result.x = new svm_node[result.l][];
                for (int i = 0; i < result.l; i++)
                {
                    result.x[i] = new svm_node[training.InputSize];
                }

                int elementIndex = 0;


                foreach (IMLDataPair pair in training)
                {
                    IMLData input = pair.Input;
                    IMLData output = pair.Ideal;
                    result.x[elementIndex] = new svm_node[input.Count];

                    for (int i = 0; i < input.Count; i++)
                    {
                        result.x[elementIndex][i] = new svm_node { index = i + 1, value_Renamed = input[i] };
                    }

                    result.y[elementIndex] = output[outputIndex];

                    elementIndex++;
                }

                return result;
            }
            catch (OutOfMemoryException)
            {
                throw new SyntError("SVM Model - Out of Memory");
            }
        }
    }
}
