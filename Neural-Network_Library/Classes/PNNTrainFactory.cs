using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PNNTrainFactory
    {
        /// <summary>
        /// Create a PNN trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String args)
        {
            if (!(method is BasicPNN))
            {
                throw new SyntError(
                    "PNN training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new TrainBasicPNN((BasicPNN)method, training);
        }
    }
}
