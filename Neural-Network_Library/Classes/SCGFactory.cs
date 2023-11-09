using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SCGFactory
    {
        /// <summary>
        /// Create a SCG trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String args)
        {
            if (!(method is BasicNetwork))
            {
                throw new SyntError(
                    "SCG training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new ScaledConjugateGradient((BasicNetwork)method, training);
        }
    }
}
