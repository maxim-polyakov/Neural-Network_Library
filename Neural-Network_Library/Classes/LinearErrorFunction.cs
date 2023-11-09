using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class LinearErrorFunction : IErrorFunction
    {
        /// <inheritdoc/>
        public void CalculateError(double[] ideal, double[] actual, double[] error)
        {
            for (int i = 0; i < actual.Length; i++)
            {
                error[i] = ideal[i] - actual[i];
            }

        }
    }
}
