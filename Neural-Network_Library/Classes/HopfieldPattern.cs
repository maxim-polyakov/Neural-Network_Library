using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class HopfieldPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// How many neurons in the Hopfield network. Default to -1, which is
        /// invalid. Therefore this value must be set.
        /// </summary>
        ///
        private int _neuronCount;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public HopfieldPattern()
        {
            _neuronCount = -1;
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer. This will throw an error, because the Hopfield neural
        /// network has no hidden layers.
        /// </summary>
        ///
        /// <param name="count">The number of neurons.</param>
        public void AddHiddenLayer(int count)
        {

        }

        /// <summary>
        /// Nothing to clear.
        /// </summary>
        ///
        public virtual void Clear()
        {
        }

        /// <summary>
        /// Generate the Hopfield neural network.
        /// </summary>
        ///
        /// <returns>The generated network.</returns>
        public IMLMethod Generate()
        {
            var logic = new HopfieldNetwork(_neuronCount);
            return logic;
        }

        /// <summary>
        /// Set the activation function to use. This function will throw an error,
        /// because the Hopfield network must use the BiPolar activation function.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {

            }
        }


        /// <summary>
        /// Set the number of input neurons, this must match the output neurons.
        /// </summary>
        public int InputNeurons
        {
            set { _neuronCount = value; }
        }


        /// <summary>
        /// Set the number of output neurons, should not be used with a hopfield
        /// neural network, because the number of input neurons defines the number of
        /// output neurons.
        /// </summary>
        public int OutputNeurons
        {
            set
            {

            }
        }

        #endregion
    }
}
