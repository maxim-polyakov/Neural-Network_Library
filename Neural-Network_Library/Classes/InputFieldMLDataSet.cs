using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class InputFieldMLDataSet : BasicInputField
    {
        /// <summary>
        /// The data set.
        /// </summary>
        private readonly IMLDataSet _data;

        /// <summary>
        /// The input or ideal index.  This treats the input and ideal as one
        /// long array, concatenated together.
        /// </summary>
        private readonly int _offset;

        /// <summary>
        /// Construct a input field based on a NeuralDataSet.
        /// </summary>
        /// <param name="usedForNetworkInput">Is this field used for neural input.</param>
        /// <param name="data">The data set to use.</param>
        /// <param name="offset">The input or ideal index to use. This treats the input 
        /// and ideal as one long array, concatenated together.</param>
        public InputFieldMLDataSet(bool usedForNetworkInput,
                                       IMLDataSet data, int offset)
        {
            _data = data;
            _offset = offset;
            UsedForNetworkInput = usedForNetworkInput;
        }

        /// <summary>
        /// The neural data set to read.
        /// </summary>
        public IMLDataSet NeuralDataSet
        {
            get { return _data; }
        }

        /// <summary>
        /// The field to be accessed. This treats the input and 
        /// ideal as one long array, concatenated together.
        /// </summary>
        public int Offset
        {
            get { return _offset; }
        }
    }
}
