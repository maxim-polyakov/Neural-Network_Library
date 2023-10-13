using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PNNPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of input neurons.
        /// </summary>
        ///
        private int _inputNeurons;

        /// <summary>
        /// The kernel type.
        /// </summary>
        ///
        private PNNKernelType _kernel;

        /// <summary>
        /// The output model.
        /// </summary>
        ///
        private PNNOutputMode _outmodel;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        ///
        private int _outputNeurons;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public PNNPattern()
        {
            _kernel = PNNKernelType.Gaussian;
            _outmodel = PNNOutputMode.Regression;
        }

        /// <summary>
        /// Set the kernel type.
        /// </summary>
        public PNNKernelType Kernel
        {
            get { return _kernel; }
            set { _kernel = value; }
        }


        /// <summary>
        /// Set the output model.
        /// </summary>
        public PNNOutputMode Outmodel
        {
            get { return _outmodel; }
            set { _outmodel = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer. PNN networks do not have hidden layers, so this will
        /// throw an error.
        /// </summary>
        ///
        /// <param name="count">The number of hidden neurons.</param>
        public void AddHiddenLayer(int count)
        {


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
        ///
        /// <returns>The neural network.</returns>
        public IMLMethod Generate()
        {
            var pnn = new BasicPNN(_kernel, _outmodel,
                                   _inputNeurons, _outputNeurons);
            return pnn;
        }

        /// <summary>
        /// Set the input neuron count.
        /// </summary>
        public int InputNeurons
        {
            get { return _inputNeurons; }
            set { _inputNeurons = value; }
        }


        /// <summary>
        /// Set the output neuron count.
        /// </summary>
        ///
        /// <value>The number of neurons.</value>
        public int OutputNeurons
        {
            get { return _outputNeurons; }
            set { _outputNeurons = value; }
        }


        /// <summary>
        /// Set the activation function. A PNN uses a linear activation function, so
        /// this method throws an error.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {

            }
        }

        #endregion
    }
}
