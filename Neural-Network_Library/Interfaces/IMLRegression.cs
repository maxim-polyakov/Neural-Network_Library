using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLRegression : IMLInputOutput
    {
        /// <summary>
        /// Compute regression.
        /// </summary>
        ///
        /// <param name="input">The input data.</param>
        /// <returns>The output data.</returns>
        IMLData Compute(IMLData input);
    }
}
