using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IErrorFunction
    {
        /// <summary>
        /// Calculate the error.
        /// </summary>
        /// <param name="ideal">The ideal values.</param>
        /// <param name="actual">The actual values.</param>
        /// <param name="error">The rror output.</param>
        void CalculateError(double[] ideal, double[] actual, double[] error);
    }
}
