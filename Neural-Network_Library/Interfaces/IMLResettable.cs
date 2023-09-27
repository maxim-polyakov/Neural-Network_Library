using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLResettable : IMLMethod
    {
        /// <summary>
        /// Reset the weights.
        /// </summary>
        ///
        void Reset();

        /// <summary>
        /// Reset the weights with a seed.
        /// </summary>
        ///
        /// <param name="seed">The seed value.</param>
        void Reset(int seed);
    }
}
