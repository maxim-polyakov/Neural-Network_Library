using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ADALINEPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of neurons in the input layer.
        /// </summary>
        ///
        private int _inputNeurons;

        /// <summary>
        /// The number of neurons in the output layer.
        /// </summary>
        ///
        private int _outputNeurons;

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Not used, the ADALINE has no hidden layers, this will throw an error.
        /// </summary>
        ///
        /// <param name="count">The neuron count.</param>
        public void AddHiddenLayer(int count)
        {

        }

        /// <summary>
        /// Clear out any parameters.
        /// </summary>
        ///
        public void Clear()
        {
            _inputNeurons = 0;
            _outputNeurons = 0;
        }

        /// <summary>
        /// Generate the network.
        /// </summary>
        public IMLMethod Generate()
        {
            var network = new BasicNetwork();

            ILayer inputLayer = new BasicLayer(new ActivationLinear(), true,
                                              _inputNeurons);
            ILayer outputLayer = new BasicLayer(new ActivationLinear(), false,
                                               _outputNeurons);

            network.AddLayer(inputLayer);
            network.AddLayer(outputLayer);
            network.Structure.FinalizeStructure();

            (new RangeRandomizer(-0.5d, 0.5d)).Randomize(network);

            return network;
        }

        /// <summary>
        /// Not used, ADALINE does not use custom activation functions.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
            }
        }

        /// <summary>
        /// Set the input neurons.
        /// </summary>
        public int InputNeurons
        {
            set { _inputNeurons = value; }
        }

        /// <summary>
        /// Set the output neurons.
        /// </summary>
        public int OutputNeurons
        {
            set { _outputNeurons = value; }
        }

        #endregion
    }
}
