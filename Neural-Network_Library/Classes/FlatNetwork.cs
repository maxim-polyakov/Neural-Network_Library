using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class FlatNetwork
    {

        public const double DefaultBiasActivation = 1.0d;


        public const double NoBiasActivation = 0.0d;


        private IActivationFunction[] _activationFunctions;


        private int _beginTraining;


        private double[] _biasActivation;


        private double _connectionLimit;


        private int[] _contextTargetOffset;


        private int[] _contextTargetSize;


        private int _endTraining;


        private bool _hasContext;

        private int _inputCount;


        private bool _isLimited;


        private int[] _layerContextCount;


        private int[] _layerCounts;

        private int[] _layerFeedCounts;


        private int[] _layerIndex;


        private double[] _layerOutput;


        private double[] _layerSums;


        private int _outputCount;


        private int[] _weightIndex;


        private double[] _weights;
        internal double ConnectionLimit;

        public FlatNetwork()
        {
        }


        public FlatNetwork(FlatLayer[] layers)
        {
            Init(layers);
        }


        public FlatNetwork(int input, int hidden1, int hidden2,
                           int output, bool tanh)
        {
            IActivationFunction linearAct = new ActivationLinear();
            FlatLayer[] layers;
            IActivationFunction act = (tanh)
                                          ? (new ActivationTANH())
                                          : (IActivationFunction)(new ActivationSigmoid());

            if ((hidden1 == 0) && (hidden2 == 0))
            {
                layers = new FlatLayer[2];
                layers[0] = new FlatLayer(linearAct, input,
                                          DefaultBiasActivation);
                layers[1] = new FlatLayer(act, output,
                                          NoBiasActivation);
            }
            else if ((hidden1 == 0) || (hidden2 == 0))
            {
                int count = Math.Max(hidden1, hidden2);
                layers = new FlatLayer[3];
                layers[0] = new FlatLayer(linearAct, input,
                                          DefaultBiasActivation);
                layers[1] = new FlatLayer(act, count,
                                          DefaultBiasActivation);
                layers[2] = new FlatLayer(act, output,
                                          NoBiasActivation);
            }
            else
            {
                layers = new FlatLayer[4];
                layers[0] = new FlatLayer(linearAct, input,
                                          DefaultBiasActivation);
                layers[1] = new FlatLayer(act, hidden1,
                                          DefaultBiasActivation);
                layers[2] = new FlatLayer(act, hidden2,
                                          DefaultBiasActivation);
                layers[3] = new FlatLayer(act, output,
                                          NoBiasActivation);
            }

            _isLimited = false;
            _connectionLimit = 0.0d;

            Init(layers);
        }


        public IActivationFunction[] ActivationFunctions
        {
            get { return _activationFunctions; }
            set { _activationFunctions = value; }
        }



        public int BeginTraining
        {
            get { return _beginTraining; }
            set { _beginTraining = value; }
        }



        public double[] BiasActivation
        {
            get { return _biasActivation; }
            set { _biasActivation = value; }
        }



        //public double ConnectionLimit
        //{
        //    //get { return _connectionLimit; }
        //    //set
        //    //{
        //    //    _connectionLimit = value;
        //    //    if (Math.Abs(_connectionLimit
        //    //                 - BasicNetwork.DefaultConnectionLimit) < SyntFramework.DefaultDoubleEqual)
        //    //    {
        //    //        _isLimited = true;
        //    //    }
        //    //}
        //}



        public int[] ContextTargetOffset
        {
            get { return _contextTargetOffset; }
            set { _contextTargetOffset = value; }
        }



        public int[] ContextTargetSize
        {
            get { return _contextTargetSize; }
            set { _contextTargetSize = value; }
        }



        public int SyntesisLength
        {
            get { return _weights.Length; }
        }



        public int EndTraining
        {
            get { return _endTraining; }
            set { _endTraining = value; }
        }



        public bool HasContext
        {
            get { return _hasContext; }
            set { _hasContext = value; }
        }



        public int InputCount
        {
            get { return _inputCount; }
            set { _inputCount = value; }
        }



        public int[] LayerContextCount
        {
            get { return _layerContextCount; }
            set { _layerContextCount = value; }
        }



        public int[] LayerCounts
        {
            get { return _layerCounts; }
            set { _layerCounts = value; }
        }


        public int[] LayerFeedCounts
        {
            get { return _layerFeedCounts; }
            set { _layerFeedCounts = value; }
        }



        public int[] LayerIndex
        {
            get { return _layerIndex; }
            set { _layerIndex = value; }
        }



        public double[] LayerOutput
        {
            get { return _layerOutput; }
            set { _layerOutput = value; }
        }



        //public int NeuronCount
        //{
        //    get
        //    {
        //        return _layerCounts.Sum();
        //    }
        //}



        public int OutputCount
        {
            get { return _outputCount; }
            set { _outputCount = value; }
        }



        public int[] WeightIndex
        {
            get { return _weightIndex; }
            set { _weightIndex = value; }
        }



        public double[] Weights
        {
            get { return _weights; }
            set { _weights = value; }
        }

        /// <value>the isLimited</value>
        public bool Limited
        {
            get { return _isLimited; }
        }


        //public double CalculateError(IMLDataSet data)
        //{
        //    //////     var errorCalculation = new ErrorCalculation();

        //    ////     var actual = new double[_outputCount];
        //    ////     IMLDataPair pair = BasicMLDataPair.CreatePair(data.InputSize,
        //    ////                                                  data.IdealSize);

        //    ////     for (int i = 0; i < data.Count; i++)
        //    ////     {
        //    ////         data.GetRecord(i, pair);
        //    ////         Compute(pair.InputArray, actual);
        //    ////         errorCalculation.UpdateError(actual, pair.IdealArray, pair.Significance);
        //    ////     }
        //    //   return errorCalculation.Calculate();
        //}


        public void ClearConnectionLimit()
        {
            _connectionLimit = 0.0d;
            _isLimited = false;
        }


        public void ClearContext()
        {
            int index = 0;

            for (int i = 0; i < _layerIndex.Length; i++)
            {
                bool hasBias = (_layerContextCount[i] + _layerFeedCounts[i]) != _layerCounts[i];

                // fill in regular neurons
                for (int j = 0; j < _layerFeedCounts[i]; j++)
                {
                    _layerOutput[index++] = 0;
                }

                // fill in the bias
                if (hasBias)
                {
                    _layerOutput[index++] = _biasActivation[i];
                }

                // fill in context
                for (int j = 0; j < _layerContextCount[i]; j++)
                {
                    _layerOutput[index++] = 0;
                }
            }
        }


        public virtual Object Clone()
        {
            var result = new FlatNetwork();
            CloneFlatNetwork(result);
            return result;
        }


        public void CloneFlatNetwork(FlatNetwork result)
        {
            result._inputCount = _inputCount;
            result._layerCounts = EngineArray.ArrayCopy(_layerCounts);
            result._layerIndex = EngineArray.ArrayCopy(_layerIndex);
            result._layerOutput = EngineArray.ArrayCopy(_layerOutput);
            result._layerSums = EngineArray.ArrayCopy(_layerSums);
            result._layerFeedCounts = EngineArray.ArrayCopy(_layerFeedCounts);
            result._contextTargetOffset = EngineArray
                .ArrayCopy(_contextTargetOffset);
            result._contextTargetSize = EngineArray
                .ArrayCopy(_contextTargetSize);
            result._layerContextCount = EngineArray
                .ArrayCopy(_layerContextCount);
            result._biasActivation = EngineArray.ArrayCopy(_biasActivation);
            result._outputCount = _outputCount;
            result._weightIndex = _weightIndex;
            result._weights = _weights;

            result._activationFunctions = new IActivationFunction[_activationFunctions.Length];
            for (int i = 0; i < result._activationFunctions.Length; i++)
            {
                result._activationFunctions[i] = (IActivationFunction)_activationFunctions[i].Clone();
            }

            result._beginTraining = _beginTraining;
            result._endTraining = _endTraining;
        }


        public virtual void Compute(double[] input, double[] output)
        {
            int sourceIndex = _layerOutput.Length
                              - _layerCounts[_layerCounts.Length - 1];

            EngineArray.ArrayCopy(input, 0, _layerOutput, sourceIndex,
                                  _inputCount);

            for (int i = _layerIndex.Length - 1; i > 0; i--)
            {
                ComputeLayer(i);
            }

            // update context values
            int offset = _contextTargetOffset[0];

            for (int x = 0; x < _contextTargetSize[0]; x++)
            {
                _layerOutput[offset + x] = _layerOutput[x];
            }

            EngineArray.ArrayCopy(_layerOutput, 0, output, 0, _outputCount);
        }


        protected internal void ComputeLayer(int currentLayer)
        {
            int inputIndex = _layerIndex[currentLayer];
            int outputIndex = _layerIndex[currentLayer - 1];
            int inputSize = _layerCounts[currentLayer];
            int outputSize = _layerFeedCounts[currentLayer - 1];

            int index = _weightIndex[currentLayer - 1];

            int limitX = outputIndex + outputSize;
            int limitY = inputIndex + inputSize;

            // weight values
            for (int x = outputIndex; x < limitX; x++)
            {
                double sum = 0;
                for (int y = inputIndex; y < limitY; y++)
                {
                    sum += _weights[index++] * _layerOutput[y];
                }
                _layerOutput[x] = sum;
                _layerSums[x] = sum;
            }

            _activationFunctions[currentLayer - 1].ActivationFunction(
                _layerOutput, outputIndex, outputSize);

            // update context values
            int offset = _contextTargetOffset[currentLayer];

            for (int x = 0; x < _contextTargetSize[currentLayer]; x++)
            {
                _layerOutput[offset + x] = _layerOutput[outputIndex + x];
            }
        }


        public void DecodeNetwork(double[] data)
        {
            if (data.Length != _weights.Length)
            {

            }
            _weights = data;
        }


        public double[] SyntesisNetwork()
        {
            return _weights;
        }



        //public Type HasSameActivationFunction()
        //{
        //    List<Type> map = new List<Type>();


        //    foreach (IActivationFunction activation in _activationFunctions)
        //    {
        //        if (!map.Contains(activation.GetType()))
        //        {
        //            map.Add(activation.GetType());
        //        }
        //    }

        //    if (map.Count != 1)
        //    {
        //        return null;
        //    }
        //    return map[0];
        //}

        /// <summary>
        /// Construct a flat network.
        /// </summary>
        ///
        /// <param name="layers">The layers of the network to create.</param>
        public void Init(FlatLayer[] layers)
        {
            int layerCount = layers.Length;

            _inputCount = layers[0].Count;
            _outputCount = layers[layerCount - 1].Count;

            _layerCounts = new int[layerCount];
            _layerContextCount = new int[layerCount];
            _weightIndex = new int[layerCount];
            _layerIndex = new int[layerCount];
            _activationFunctions = new IActivationFunction[layerCount];
            _layerFeedCounts = new int[layerCount];
            _contextTargetOffset = new int[layerCount];
            _contextTargetSize = new int[layerCount];
            _biasActivation = new double[layerCount];

            int index = 0;
            int neuronCount = 0;
            int weightCount = 0;

            for (int i = layers.Length - 1; i >= 0; i--)
            {
                FlatLayer layer = layers[i];
                FlatLayer nextLayer = null;

                if (i > 0)
                {
                    nextLayer = layers[i - 1];
                }

                _biasActivation[index] = layer.BiasActivation;
                _layerCounts[index] = layer.TotalCount;
                _layerFeedCounts[index] = layer.Count;
                _layerContextCount[index] = layer.ContextCount;
                _activationFunctions[index] = layer.Activation;

                neuronCount += layer.TotalCount;

                if (nextLayer != null)
                {
                    weightCount += layer.Count * nextLayer.TotalCount;
                }

                if (index == 0)
                {
                    _weightIndex[index] = 0;
                    _layerIndex[index] = 0;
                }
                else
                {
                    _weightIndex[index] = _weightIndex[index - 1]
                                         + (_layerCounts[index] * _layerFeedCounts[index - 1]);
                    _layerIndex[index] = _layerIndex[index - 1]
                                        + _layerCounts[index - 1];
                }

                int neuronIndex = 0;
                for (int j = layers.Length - 1; j >= 0; j--)
                {
                    if (layers[j].ContextFedBy == layer)
                    {
                        _hasContext = true;
                        _contextTargetSize[index] = layers[j].ContextCount;
                        _contextTargetOffset[index] = neuronIndex
                                                     + (layers[j].TotalCount - layers[j].ContextCount);
                    }
                    neuronIndex += layers[j].TotalCount;
                }

                index++;
            }

            _beginTraining = 0;
            _endTraining = _layerCounts.Length - 1;

            _weights = new double[weightCount];
            _layerOutput = new double[neuronCount];
            _layerSums = new double[neuronCount];

            ClearContext();
        }


        /// <summary>
        /// Perform a simple randomization of the weights of the neural network
        /// between -1 and 1.
        /// </summary>
        ///
        public void Randomize()
        {
            Randomize(1, -1);
        }

        /// <summary>
        /// Perform a simple randomization of the weights of the neural network
        /// between the specified hi and lo.
        /// </summary>
        ///
        /// <param name="hi">The network high.</param>
        /// <param name="lo">The network low.</param>
        public void Randomize(double hi, double lo)
        {
            for (int i = 0; i < _weights.Length; i++)
            {
                _weights[i] = (ThreadSafeRandom.NextDouble() * (hi - lo)) + lo;
            }
        }

        /// <summary>
        /// The layer sums, before the activation is applied.
        /// </summary>
        public double[] LayerSums
        {
            get { return _layerSums; }
            set { _layerSums = value; }
        }
    }
}
