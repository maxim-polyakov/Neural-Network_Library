using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SOMPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of input neurons.
        /// </summary>
        ///
        private int _inputNeurons;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        ///
        private int _outputNeurons;

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer. SOM networks do not have hidden layers, so this will
        /// throw an error.
        /// </summary>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A SOM network does not have hidden layers.");
        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        ///
        public virtual void Clear()
        {
        }

        /// <summary>
        /// Generate the RSOM network.
        /// </summary>
        public IMLMethod Generate()
        {
            var som = new SOMNetwork(_inputNeurons, _outputNeurons);
            som.Reset();
            return som;
        }

        /// <summary>
        /// Set the activation function. A SOM uses a linear activation function, so
        /// this method throws an error.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A SOM network can't define an activation function.");
            }
        }


        /// <summary>
        /// Set the input neuron count.
        /// </summary>
        public int InputNeurons
        {
            set { _inputNeurons = value; }
        }


        /// <summary>
        /// Set the output neuron count.
        /// </summary>
        public int OutputNeurons
        {
            set { _outputNeurons = value; }
        }

        #endregion
    }
}
