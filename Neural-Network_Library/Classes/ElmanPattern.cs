using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ElmanPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The activation function.
        /// </summary>
        ///
        private IActivationFunction _activation;

        /// <summary>
        /// The number of hidden neurons.
        /// </summary>
        ///
        private int _hiddenNeurons;

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

        /// <summary>
        /// Create an object to generate Elman neural networks.
        /// </summary>
        ///
        public ElmanPattern()
        {
            _inputNeurons = -1;
            _outputNeurons = -1;
            _hiddenNeurons = -1;
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer with the specified number of neurons.
        /// </summary>
        ///
        /// <param name="count">The number of neurons in this hidden layer.</param>
        public void AddHiddenLayer(int count)
        {
            if (_hiddenNeurons != -1)
            {
                throw new PatternError(
                    "An Elman neural network should have only one hidden layer.");
            }

            _hiddenNeurons = count;
        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        ///
        public void Clear()
        {
            _hiddenNeurons = -1;
        }

        /// <summary>
        /// Generate the Elman neural network.
        /// </summary>
        ///
        /// <returns>The Elman neural network.</returns>
        public IMLMethod Generate()
        {
            BasicLayer hidden, input;

            var network = new BasicNetwork();
            network.AddLayer(input = new BasicLayer(_activation, true,
                                                    _inputNeurons));
            network.AddLayer(hidden = new BasicLayer(_activation, true,
                                                     _hiddenNeurons));
            network.AddLayer(new BasicLayer(null, false, _outputNeurons));
            input.ContextFedBy = hidden;
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        /// <summary>
        /// Set the activation function to use on each of the layers.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set { _activation = value; }
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        public int InputNeurons { set { _inputNeurons = value; } }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        public int OutputNeurons
        {

            set { _outputNeurons = value; }
        }

        #endregion
    }
}
