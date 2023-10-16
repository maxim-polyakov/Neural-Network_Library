using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBF.Classes
{
    public class SVMPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of neurons in the first layer.
        /// </summary>
        ///
        private int _inputNeurons;

        /// <summary>
        /// The kernel type.
        /// </summary>
        ///
        private KernelType _kernelType;

        /// <summary>
        /// The number of neurons in the second layer.
        /// </summary>
        ///
        private int _outputNeurons;

        /// <summary>
        /// The SVM type.
        /// </summary>
        ///
        private SVMType _svmType;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public SVMPattern()
        {
            Regression = true;
            _kernelType = KernelType.RadialBasisFunction;
            _svmType = SVMType.EpsilonSupportVectorRegression;
        }

        /// <summary>
        /// Set if regression is used.
        /// </summary>
        public bool Regression { get; set; }

        /// <summary>
        /// Set the kernel type.
        /// </summary>
        public KernelType KernelType
        {
            set { _kernelType = value; }
        }


        /// <summary>
        /// Set the SVM type.
        /// </summary>
        public SVMType SVMType
        {
            set { _svmType = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Unused, a BAM has no hidden layers.
        /// </summary>
        ///
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A SVM network has no hidden layers.");
        }

        /// <summary>
        /// Clear any settings on the pattern.
        /// </summary>
        ///
        public void Clear()
        {
            _inputNeurons = 0;
            _outputNeurons = 0;
        }


        /// <returns>The generated network.</returns>
        public IMLMethod Generate()
        {
            if (_outputNeurons != 1)
            {
                throw new PatternError("A SVM may only have one output.");
            }
            var network = new SupportVectorMachine(_inputNeurons, _svmType,
                                                   _kernelType);
            return network;
        }

        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            get { return _inputNeurons; }
            set { _inputNeurons = value; }
        }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        public int OutputNeurons
        {
            get { return _outputNeurons; }
            set { _outputNeurons = value; }
        }


        /// <summary>
        /// Not used, the BAM uses a bipoloar activation function.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A SVM network can't specify a custom activation function.");
            }
        }

        #endregion
    }
}
