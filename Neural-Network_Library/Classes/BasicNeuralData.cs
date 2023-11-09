using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BasicNeuralData : BasicMLData, INeuralData
    {
        /// <summary>
        /// Construct the object from an array.
        /// </summary>
        /// <param name="d">The array to base on.</param>
        public BasicNeuralData(double[] d) : base(d)
        {
        }

        /// <summary>
        /// Construct an empty array of the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        public BasicNeuralData(int size) : base(size)
        {
        }
    }
}
