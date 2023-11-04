using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class InputFieldArray1D : BasicInputField, IHasFixedLength
    {
        /// <summary>
        /// A reference to the array.
        /// </summary>
        private readonly double[] _array;

        /// <summary>
        /// Construct the 1D array field.
        /// </summary>
        /// <param name="usedForNetworkInput">True if this field is used for the actual
        /// input to the neural network.  See getUsedForNetworkInput for more info.</param>
        /// <param name="array">The array to use.</param>
        public InputFieldArray1D(bool usedForNetworkInput,
                                 double[] array)
        {
            _array = array;
            UsedForNetworkInput = usedForNetworkInput;
        }

        #region IHasFixedLength Members

        /// <summary>
        /// The length of the array.
        /// </summary>
        public int Length
        {
            get { return _array.Length; }
        }

        #endregion

        /// <summary>
        /// Get the value from the specified index.
        /// </summary>
        /// <param name="i">The index to retrieve.</param>
        /// <returns>The value at the specified index.</returns>
        public override double GetValue(int i)
        {
            return _array[i];
        }
    }
}
