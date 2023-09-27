using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface INeuralNetworkPattern
    {
        /// <summary>
        /// Set the activation function to be used for all created layers that allow
        /// an activation function to be specified. Not all patterns allow the
        /// activation function to be specified.
        /// </summary>
        ///
        /// <value>The activation function.</value>
        IActivationFunction ActivationFunction
        {
            set;
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        ///
        /// <value>The number of input neurons.</value>
        int InputNeurons { set; }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        ///
        /// <value>The output neuron count.</value>
        int OutputNeurons { set; }

        /// <summary>
        /// Add the specified hidden layer.
        /// </summary>
        ///
        /// <param name="count">The number of neurons in the hidden layer.</param>
        void AddHiddenLayer(int count);

        /// <summary>
        /// Clear the hidden layers so that they can be redefined.
        /// </summary>
        ///
        void Clear();

        /// <summary>
        /// Generate the specified neural network.
        /// </summary>
        ///
        /// <returns>The resulting neural network.</returns>
        IMLMethod Generate();
    }
}
