using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class NormalizationStorageArray1D : INormalizationStorage
    {
        /// <summary>
        /// The array to store to.
        /// </summary>
        private readonly double[] _array;

        /// <summary>
        /// The current index.
        /// </summary>
        private int _currentIndex;


        /// <summary>
        /// Construct an object to store to a 2D array.
        /// </summary>
        /// <param name="array">The array to store to.</param>
        public NormalizationStorageArray1D(double[] array)
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
            _array[_currentIndex++] = data[0];
        }

        #endregion

        public double[] GetArray()
        {
            return _array;
        }
    }
}
