using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class InputFieldArray2D : BasicInputField, IHasFixedLength
    {
        /// <summary>
        /// The 2D array to use.
        /// </summary>
        private readonly double[][] _array;

        /// <summary>
        /// The 2nd dimension index to read the field from.
        /// </summary>
        private readonly int _index2;

        /// <summary>
        /// Construct a 2D array input.
        /// </summary>
        /// <param name="usedForNetworkInput">Construct a 2D array input field.</param>
        /// <param name="array">The array to use.</param>
        /// <param name="index2">index2 The secondary index to read the field from.</param>
        public InputFieldArray2D(bool usedForNetworkInput,
                                 double[][] array, int index2)
        {
            _array = array;
            _index2 = index2;
            UsedForNetworkInput = usedForNetworkInput;
        }

        #region IHasFixedLength Members

        /// <summary>
        /// The number of rows in the array.
        /// </summary>
        public int Length
        {
            get { return _array.Length; }
        }

        #endregion

        /// <summary>
        /// Gen index.
        /// </summary>
        /// <param name="i">Read a value from the specified index.</param>
        /// <returns>The value read.</returns>
        public
            override double GetValue(int i)
        {
            return _array[i][_index2];
        }
    }
}
