using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class RadialBasisPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of hidden neurons to use. Must be set, default to invalid -1
        /// value.
        /// </summary>
        ///
        private int _hiddenNeurons;

        /// <summary>
        /// The number of input neurons to use. Must be set, default to invalid -1
        /// value.
        /// </summary>
        ///
        private int _inputNeurons;

        /// <summary>
        /// The number of hidden neurons to use. Must be set, default to invalid -1
        /// value.
        /// </summary>
        ///
        private int _outputNeurons;

        /// <summary>
        /// The RBF type.
        /// </summary>
        private RBFEnum _rbfType;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public RadialBasisPattern()
        {
            _rbfType = RBFEnum.Gaussian;
            _inputNeurons = -1;
            _outputNeurons = -1;
            _hiddenNeurons = -1;
        }

        /// <summary>
        /// The RBF type.
        /// </summary>
        public RBFEnum RBF
        {
            set { _rbfType = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add the hidden layer, this should be called once, as a RBF has a single
        /// hidden layer.
        /// </summary>
        ///
        /// <param name="count">The number of neurons in the hidden layer.</param>
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
        /// Generate the RBF network.
        /// </summary>
        ///
        /// <returns>The neural network.</returns>
        public IMLMethod Generate()
        {
            var result = new RBFNetwork(_inputNeurons, _hiddenNeurons,
                                        _outputNeurons, _rbfType);
            return result;
        }

        /// <summary>
        /// Set the activation function, this is an error. The activation function
        /// may not be set on a RBF layer.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {

            }
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
