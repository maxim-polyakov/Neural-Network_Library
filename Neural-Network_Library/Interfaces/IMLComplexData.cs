using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLComplexData : IMLData
    {
        /// <summary>
        /// The complex numbers.
        /// </summary>
        ComplexNumber[] ComplexData { get; }

        /// <summary>
        /// Get the complex data at the specified index. 
        /// </summary>
        /// <param name="index">The index to get the complex data at.</param>
        /// <returns>The complex data.</returns>
        ComplexNumber GetComplexData(int index);

        /// <summary>
        /// Set the complex number array.
        /// </summary>
        /// <param name="theData">The new array.</param>
        void SetComplexData(ComplexNumber[] theData);

        /// <summary>
        /// Set a data element to a complex number.
        /// </summary>
        /// <param name="index">The index to set.</param>
        /// <param name="d">The complex number.</param>
        void SetComplexData(int index, ComplexNumber d);
    }
}
