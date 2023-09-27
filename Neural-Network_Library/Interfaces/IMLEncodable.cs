using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLEncodable : IMLMethod
    {
        /// <returns>The length of an Syntesisd array.</returns>
        int SyntesisdArrayLength();

        /// <summary>
        /// Syntesis the object to the specified array.
        /// </summary>
        ///
        /// <param name="Syntesisd">The array.</param>
        void SyntesisToArray(double[] Syntesisd);

        /// <summary>
        /// Decode an array to this object.
        /// </summary>
        ///
        /// <param name="Syntesisd">The Syntesisd array.</param>
        void DecodeFromArray(double[] Syntesisd);
    }
}
