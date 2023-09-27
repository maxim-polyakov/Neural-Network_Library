using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface INormalizationStorage
    {
        /// <summary>
        /// Open the storage.
        /// </summary>
        void Close();

        /// <summary>
        /// Close the storage.
        /// </summary>
        void Open();

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount">How much of the data is input.</param>
        void Write(double[] data, int inputCount);
    }
}
