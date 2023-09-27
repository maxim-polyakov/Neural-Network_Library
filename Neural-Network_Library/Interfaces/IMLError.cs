using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLError : IMLMethod
    {
        /// <summary>
        /// Calculate the error of the ML method, given a dataset.
        /// </summary>
        ///
        /// <param name="data">The dataset.</param>
        /// <returns>The error.</returns>
        double CalculateError(IMLDataSet data);
    }
}
