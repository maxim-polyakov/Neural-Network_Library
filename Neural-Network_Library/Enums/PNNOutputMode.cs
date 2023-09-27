using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public enum PNNOutputMode
    {
        /// <summary>
        /// Unsupervised training will make use of autoassociation. No "ideal" values
        /// should be provided for training. Input and output neuron counts must
        /// match.
        /// </summary>
        ///
        Unsupervised,

        /// <summary>
        /// Regression is where the neural network performs as a function. Input is
        /// supplied, and output is returned. The output is a numeric value.
        /// </summary>
        ///
        Regression,

        /// <summary>
        /// Classification attempts to classify the input into a number of predefined
        /// classes. The class is stored in the ideal as a single "double" value,
        /// though it is really treated as an integer that represents class
        /// membership. The number of output neurons should match the number of
        /// classes. Classes are indexed beginning at 0.
        /// </summary>
        ///
        Classification
    }
}
