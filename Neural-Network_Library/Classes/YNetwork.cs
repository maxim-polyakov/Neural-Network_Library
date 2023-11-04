using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class YNetwork : BasicML, IMLContext, IMLRegression,
                                  IMLError
    {
        /// <summary>
        /// The depth property.
        /// </summary>
        public const String PropertyNetworkDepth = "depth";

        /// <summary>
        /// The links property.
        /// </summary>
        public const String PropertyLinks = "links";

        /// <summary>
        /// The snapshot property.
        /// </summary>
        public const String PropertySnapshot = "snapshot";

        /// <summary>
        /// The neurons that make up this network.
        /// </summary>
        ///
        private readonly IList<YNeuron> _neurons;

        /// <summary>
        /// The activation function.
        /// </summary>
        ///
        private IActivationFunction _activationFunction;

        /// <summary>
        /// The input count.
        /// </summary>
        private int _inputCount;

        /// <summary>
        /// The depth of the network.
        /// </summary>
        ///
        private int _networkDepth;

        /// <summary>
        /// The output activation function.
        /// </summary>
        private IActivationFunction _outputActivationFunction;

        /// <summary>
        /// The output count.
        /// </summary>
        private int _outputCount;

        /// <summary>
        /// Should snapshot be used to calculate the output of the neural network.
        /// </summary>
        ///
        private bool _snapshot;

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public YNetwork()
        {
            _neurons = new List<YNeuron>();
            _snapshot = false;
        }

        /// <summary>
        /// Construct a Y synapse.
        /// </summary>
        ///
        /// <param name="inputCount">The number of input neurons.</param>
        /// <param name="outputCount">The number of output neurons.</param>
        /// <param name="neurons">The neurons in this synapse.</param>
        /// <param name="activationFunction">The activation function to use.</param>
        /// <param name="outputActivationFunction">The output activation function.</param>
        /// <param name="networkDepth">The depth of the network.</param>
        public YNetwork(int inputCount, int outputCount,
                           IEnumerable<YNeuron> neurons,
                           IActivationFunction activationFunction,
                           IActivationFunction outputActivationFunction,
                           int networkDepth)
        {
            _neurons = new List<YNeuron>();
            _snapshot = false;
            _inputCount = inputCount;
            _outputCount = outputCount;
            _outputActivationFunction = outputActivationFunction;

            foreach (YNeuron neuron in neurons)
            {
                _neurons.Add(neuron);
            }

            _networkDepth = networkDepth;
            _activationFunction = activationFunction;
        }

        /// <summary>
        /// Construct a Y network.
        /// </summary>
        ///
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount">The output count.</param>
        public YNetwork(int inputCount, int outputCount)
        {
            _neurons = new List<YNeuron>();
            _snapshot = false;
            _inputCount = inputCount;
            _outputCount = outputCount;
            _networkDepth = 0;
            _activationFunction = new ActivationSigmoid();
        }

        /// <summary>
        /// Set the activation function.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            get { return _activationFunction; }
            set { _activationFunction = value; }
        }

        /// <summary>
        /// The network depth.
        /// </summary>
        public int NetworkDepth
        {
            get { return _networkDepth; }
            set { _networkDepth = value; }
        }


        /// <value>The Y neurons.</value>
        public IList<YNeuron> Neurons
        {
            get { return _neurons; }
        }


        /// <summary>
        /// Sets if snapshot is used.
        /// </summary>
        public bool Snapshot
        {
            get { return _snapshot; }
            set { _snapshot = value; }
        }

        /// <value>the outputActivationFunction to set</value>
        public IActivationFunction OutputActivationFunction
        {
            get { return _outputActivationFunction; }
            set { _outputActivationFunction = value; }
        }

        #region MLContext Members

        /// <summary>
        /// Clear any context from previous runs. This sets the activation of all
        /// neurons to zero.
        /// </summary>
        ///
        public virtual void ClearContext()
        {
            foreach (YNeuron neuron in _neurons)
            {
                neuron.Output = 0;
            }
        }

        #endregion

        #region MLError Members

        /// <summary>
        /// Calculate the error for this neural network. 
        /// </summary>
        ///
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public virtual double CalculateError(IMLDataSet data)
        {
            return SyntUtility.CalculateRegressionError(this, data);
        }

        #endregion

        #region MLRegression Members

        /// <summary>
        /// Compute the output from this synapse.
        /// </summary>
        ///
        /// <param name="input">The input to this synapse.</param>
        /// <returns>The output from this synapse.</returns>
        public virtual IMLData Compute(IMLData input)
        {
            IMLData result = new BasicMLData(_outputCount);

            if (_neurons.Count == 0)
            {
                throw new NeuralNetworkError(
                    "This network has not been evolved yet, it has no neurons in the Y synapse.");
            }

            int flushCount = 1;

            if (_snapshot)
            {
                flushCount = _networkDepth;
            }

            // iterate through the network FlushCount times
            for (int i = 0; i < flushCount; ++i)
            {
                int outputIndex = 0;
                int index = 0;

                result.Clear();

                // populate the input neurons
                while (_neurons[index].NeuronType == YNeuronType.Input)
                {
                    _neurons[index].Output = input[index];

                    index++;
                }

                // set the bias neuron
                _neurons[index++].Output = 1;

                while (index < _neurons.Count)
                {
                    YNeuron currentNeuron = _neurons[index];

                    double sum = 0;


                    foreach (YLink link in currentNeuron.InboundLinks)
                    {
                        double weight = link.Weight;
                        double neuronOutput = link.FromNeuron.Output;
                        sum += weight * neuronOutput;
                    }

                    var d = new double[1];
                    d[0] = sum / currentNeuron.ActivationResponse;
                    _activationFunction.ActivationFunction(d, 0, d.Length);

                    _neurons[index].Output = d[0];

                    if (currentNeuron.NeuronType == YNeuronType.Output)
                    {
                        result[outputIndex++] = currentNeuron.Output;
                    }
                    index++;
                }
            }

            _outputActivationFunction.ActivationFunction(result.Data, 0,
                                                        result.Count);

            return result;
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public virtual int InputCount
        {
            get { return _inputCount; }
            set { _inputCount = value; }
        }

        /// <summary>
        /// The output count.
        /// </summary>
        public virtual int OutputCount
        {
            get { return _outputCount; }
            set { _outputCount = value; }
        }

        #endregion

        /// <summary>
        /// Not needed.
        /// </summary>
        public override void UpdateProperties()
        {
        }
    }
}
