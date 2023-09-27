using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLData : ICloneable, ICentroidFactory<IMLData>
    {
        /// <summary>
        /// Get or set the specified index.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        double this[int x] { get; set; }

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        double[] Data { get; set; }

        /// <summary>
        /// How many elements in this data structure.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Clear the data to zero values.
        /// </summary>
        void Clear();

    }
}
