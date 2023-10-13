using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class JordanPattern : INeuralNetworkPattern
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
        /// Construct an object to create a Jordan type neural network.
        /// </summary>
        ///
        public JordanPattern()
        {
            _inputNeurons = -1;
            _outputNeurons = -1;
            _hiddenNeurons = -1;
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer, there should be only one.
        /// </summary>
        ///
        /// <param name="count">The number of neurons in this hidden layer.</param>
        public void AddHiddenLayer(int count)
        {
            if (_hiddenNeurons != -1)
            {

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
        /// Generate a Jordan neural network.
        /// </summary>
        ///
        /// <returns>A Jordan neural network.</returns>
        public IMLMethod Generate()
        {
            BasicLayer hidden, output;

            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true,
                                            _inputNeurons));
            network.AddLayer(hidden = new BasicLayer(_activation, true,
                                                     _hiddenNeurons));
            network.AddLayer(output = new BasicLayer(_activation, false,
                                                     _outputNeurons));
            hidden.ContextFedBy = output;
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
        public int InputNeurons
        {
            set { _inputNeurons = value; }
        }


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
