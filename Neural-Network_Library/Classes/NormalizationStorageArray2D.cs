using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class NormalizationStorageArray2D : INormalizationStorage
    {
        /// <summary>
        /// The array to output to.
        /// </summary>
        private readonly double[][] _array;

        /// <summary>
        /// The current data.
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// Construct an object to store to a 2D array.
        /// </summary>
        /// <param name="array">The array to store to.</param>
        public NormalizationStorageArray2D(double[][] array)
        {
            _array = array;
            _currentIndex = 0;
        }

        #region INormalizationStorage Members

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Open()
        {
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount">How much of the data is input.</param>
        public void Write(double[] data, int inputCount)
        {
            for (int i = 0; i < data.Length; i++)
            {
                _array[_currentIndex][i] = data[i];
            }
            _currentIndex++;
        }

        #endregion

        /// <summary>
        /// Get the underlying array.
        /// </summary>
        /// <returns>The underlying array.</returns>
        public double[][] GetArray()
        {
            return this._array;
        }
    }
}
