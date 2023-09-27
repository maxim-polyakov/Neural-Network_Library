using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class BasicNetwork : BasicML, IContainsFlat, IMLContext,
                                  IMLRegression, IMLEncodable, IMLResettable, IMLClassification, IMLError
    {
        /// <summary>
        /// Tag used for the connection limit.
        /// </summary>
        ///
        public const String TagLimit = "CONNECTION_LIMIT";

        /// <summary>
        /// The default connection limit.
        /// </summary>
        ///
        public const double DefaultConnectionLimit = 0.0000000001d;

        /// <summary>
        /// The property for connection limit.
        /// </summary>
        ///
        public const String TagConnectionLimit = "connectionLimit";

        /// <summary>
        /// The property for begin training.
        /// </summary>
        ///
        public const String TagBeginTraining = "beginTraining";

        /// <summary>
        /// The property for context target offset.
        /// </summary>
        ///
        public const String TagContextTargetOffset = "contextTargetOffset";

        /// <summary>
        /// The property for context target size.
        /// </summary>
        ///
        public const String TagContextTargetSize = "contextTargetSize";

        /// <summary>
        /// The property for end training.
        /// </summary>
        ///
        public const String TagEndTraining = "endTraining";

        /// <summary>
        /// The property for has context.
        /// </summary>
        ///
        public const String TagHasContext = "hasContext";

        /// <summary>
        /// The property for layer counts.
        /// </summary>
        ///
        public const String TagLayerCounts = "layerCounts";

        /// <summary>
        /// The property for layer feed counts.
        /// </summary>
        ///
        public const String TagLayerFeedCounts = "layerFeedCounts";

        /// <summary>
        /// The property for layer index.
        /// </summary>
        ///
        public const String TagLayerIndex = "layerIndex";

        /// <summary>
        /// The property for weight index.
        /// </summary>
        ///
        public const String TagWeightIndex = "weightIndex";

        /// <summary>
        /// The property for bias activation.
        /// </summary>
        ///
        public const String TagBiasActivation = "biasActivation";

        /// <summary>
        /// The property for layer context count.
        /// </summary>
        ///
        public const String TagLayerContextCount = "layerContextCount";

        /// <summary>
        /// Holds the structure of the network. This keeps the network from having to
        /// constantly lookup layers and synapses.
        /// </summary>
        ///
        private readonly NeuralStructure _structure;

        /// <summary>
        /// Construct an empty neural network.
        /// </summary>
        ///
        public BasicNetwork()
        {
            _structure = new NeuralStructure(this);
        }

        /// <value>The layer count.</value>
        public int LayerCount
        {
            get
            {
                _structure.RequireFlat();
                return _structure.Flat.LayerCounts.Length;
            }
        }

        /// <value>Get the structure of the neural network. The structure allows you
        /// to quickly obtain synapses and layers without traversing the
        /// network.</value>
        public NeuralStructure Structure
        {
            get { return _structure; }
        }

        /// <summary>
        /// Sets the bias activation for every layer that supports bias. Make sure
        /// that the network structure has been finalized before calling this method.
        /// </summary>
        public double BiasActivation
        {
            set
            {
                // first, see what mode we are on. If the network has not been
                // finalized, set the layers
                if (_structure.Flat == null)
                {
                    foreach (ILayer layer in _structure.Layers)
                    {
                        if (layer.HasBias())
                        {
                            layer.BiasActivation = value;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < LayerCount; i++)
                    {
                        if (IsLayerBiased(i))
                        {
                            SetLayerBiasActivation(i, value);
                        }
                    }
                }
            }
        }

        #region ContainsFlat Members

        /// <inheritdoc/>
        public FlatNetwork Flat
        {
            get { return Structure.Flat; }
        }

        #endregion

        #region MLClassification Members

        /// <inheritdoc/>
        public int Classify(IMLData input)
        {
            return Winner(input);
        }

        #endregion

        #region MLContext Members

        /// <summary>
        /// Clear any data from any context layers.
        /// </summary>
        ///
        public void ClearContext()
        {
            if (_structure.Flat != null)
            {
                _structure.Flat.ClearContext();
            }
        }

        #endregion

        #region MLEncodable Members

        /// <inheritdoc/>
        public void DecodeFromArray(double[] Syntesisd)
        {
            _structure.RequireFlat();
            double[] weights = _structure.Flat.Weights;
            if (weights.Length != Syntesisd.Length)
            {

            }

            EngineArray.ArrayCopy(Syntesisd, weights);
        }

        /// <inheritdoc/>
        public int SyntesisdArrayLength()
        {
            _structure.RequireFlat();
            return _structure.Flat.SyntesisLength;
        }

        /// <inheritdoc/>
        public void SyntesisToArray(double[] Syntesisd)
        {
            _structure.RequireFlat();
            double[] weights = _structure.Flat.Weights;
            if (weights.Length != Syntesisd.Length)
            {

            }

            EngineArray.ArrayCopy(weights, Syntesisd);
        }

        #endregion

        #region MLError Members

        /// <summary>
        /// Calculate the error for this neural network.
        /// </summary>
        ///
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(IMLDataSet data)
        {
            return SyntUtility.CalculateRegressionError(this, data);
        }

        #endregion

        #region MLRegression Members

        /// <summary>
        /// Compute the output for a given input to the neural network.
        /// </summary>
        ///
        /// <param name="input">The input to the neural network.</param>
        /// <returns>The output from the neural network.</returns>
        public IMLData Compute(IMLData input)
        {

            IMLData result = new BasicMLData(_structure.Flat.OutputCount);
            _structure.Flat.Compute(input.Data, result.Data);
            return result;


        }

        /// <inheritdoc/>
        public virtual int InputCount
        {
            get
            {
                _structure.RequireFlat();
                return Structure.Flat.InputCount;
            }
        }

        /// <inheritdoc/>
        public virtual int OutputCount
        {
            get
            {
                _structure.RequireFlat();
                return Structure.Flat.OutputCount;
            }
        }

        #endregion

        #region MLResettable Members

        /// <summary>
        /// Reset the weight matrix and the bias values. This will use a
        /// Nguyen-Widrow randomizer with a range between -1 and 1. If the network
        /// does not have an input, output or hidden layers, then Nguyen-Widrow
        /// cannot be used and a simple range randomize between -1 and 1 will be
        /// used.
        /// </summary>
        ///
        public void Reset()
        {
            if (LayerCount < 3)
            {
                (new RangeRandomizer(-1, 1)).Randomize(this);
            }
            else
            {
                (new NguyenWidrowRandomizer()).Randomize(this);
            }
        }

        /// <summary>
        /// Reset the weight matrix and the bias values. This will use a
        /// Nguyen-Widrow randomizer with a range between -1 and 1. If the network
        /// does not have an input, output or hidden layers, then Nguyen-Widrow
        /// cannot be used and a simple range randomize between -1 and 1 will be
        /// used.
        /// Use the specified seed.
        /// </summary>
        ///
        public void Reset(int seed)
        {
            Reset();
        }

        #endregion

        /// <summary>
        /// Add a layer to the neural network. If there are no layers added this
        /// layer will become the input layer. This function automatically updates
        /// both the input and output layer references.
        /// </summary>
        ///
        /// <param name="layer">The layer to be added to the network.</param>
        public void AddLayer(ILayer layer)
        {
            layer.Network = this;
            _structure.Layers.Add(layer);
        }

        /// <summary>
        /// Add to a weight.
        /// </summary>
        ///
        /// <param name="fromLayer">The from layer.</param>
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <param name="v">The value to add.</param>
        public void AddWeight(int fromLayer, int fromNeuron,
                              int toNeuron, double v)
        {
            double old = GetWeight(fromLayer, fromNeuron, toNeuron);
            SetWeight(fromLayer, fromNeuron, toNeuron, old + v);
        }

        /// <summary>
        /// Calculate the total number of neurons in the network across all layers.
        /// </summary>
        ///
        /// <returns>The neuron count.</returns>
        public int CalculateNeuronCount()
        {
            int result = 0;

            foreach (ILayer layer in _structure.Layers)
            {
                result += layer.NeuronCount;
            }
            return result;
        }

        /// <summary>
        /// Return a clone of this neural network. Including structure, weights and
        /// bias values. This is a deep copy.
        /// </summary>
        ///
        /// <returns>A cloned copy of the neural network.</returns>
        public Object Clone()
        {
            var result = (BasicNetwork)ObjectCloner.DeepCopy(this);
            return result;
        }

        /// <summary>
        /// Compute the output for this network.
        /// </summary>
        ///
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public void Compute(double[] input, double[] output)
        {
            var input2 = new BasicMLData(input);
            IMLData output2 = Compute(input2);
            EngineArray.ArrayCopy(output2.Data, output);
        }


        /// <returns>The weights as a comma separated list.</returns>
        public String DumpWeights()
        {
            var result = new StringBuilder();
            NumberList.ToList(CSVFormat.EgFormat, result, _structure.Flat.Weights);
            return result.ToString();
        }

        /// <summary>
        /// Enable, or disable, a connection.
        /// </summary>
        ///
        /// <param name="fromLayer">The layer that contains the from neuron.</param>
        /// <param name="fromNeuron">The source neuron.</param>
        /// <param name="toNeuron">The target connection.</param>
        /// <param name="enable">True to enable, false to disable.</param>
        public void EnableConnection(int fromLayer,
                                     int fromNeuron, int toNeuron, bool enable)
        {
            double v = GetWeight(fromLayer, fromNeuron, toNeuron);

            if (enable)
            {
                if (!_structure.ConnectionLimited)
                {
                    return;
                }

                if (Math.Abs(v) < _structure.ConnectionLimit)
                {
                    SetWeight(fromLayer, fromNeuron, toNeuron,
                              RangeRandomizer.Randomize(-1, 1));
                }
            }
            else
            {
                if (!_structure.ConnectionLimited)
                {
                    SetProperty(TagLimit,
                                DefaultConnectionLimit);
                    _structure.UpdateProperties();
                }
                SetWeight(fromLayer, fromNeuron, toNeuron, 0);
            }
        }

        /// <summary>
        /// Compare the two neural networks. For them to be equal they must be of the
        /// same structure, and have the same matrix values.
        /// </summary>
        ///
        /// <param name="other">The other neural network.</param>
        ///// <returns>True if the two networks are equal.</returns>
        //public bool Equals(BasicNetwork other)
        //{
        //    return Equals(other, SyntFramework.DefaultPrecision);
        //}

        /// <summary>
        /// Determine if this neural network is equal to another. Equal neural
        /// networks have the same weight matrix and bias values, within a specified
        /// precision.
        /// </summary>
        ///
        /// <param name="other">The other neural network.</param>
        /// <param name="precision">The number of decimal places to compare to.</param>
        /// <returns>True if the two neural networks are equal.</returns>
        //public bool Equals(BasicNetwork other, int precision)
        //{
        //    return NetworkCODEC.Equals(this, other, precision);
        //}

        /// <summary>
        /// Get the activation function for the specified layer.
        /// </summary>
        ///
        /// <param name="layer">The layer.</param>
        /// <returns>The activation function.</returns>
        public IActivationFunction GetActivation(int layer)
        {
            _structure.RequireFlat();
            int layerNumber = LayerCount - layer - 1;
            return _structure.Flat.ActivationFunctions[layerNumber];
        }


        /// <summary>
        /// Get the bias activation for the specified layer.
        /// </summary>
        ///
        /// <param name="l">The layer.</param>
        /// <returns>The bias activation.</returns>
        public double GetLayerBiasActivation(int l)
        {
            if (!IsLayerBiased(l))
            {

            }

            _structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;

            int layerOutputIndex = _structure.Flat.LayerIndex[layerNumber];
            int count = _structure.Flat.LayerCounts[layerNumber];
            return _structure.Flat.LayerOutput[layerOutputIndex
                                              + count - 1];
        }


        /// <summary>
        /// Get the neuron count.
        /// </summary>
        ///
        /// <param name="l">The layer.</param>
        /// <returns>The neuron count.</returns>
        public int GetLayerNeuronCount(int l)
        {
            _structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;
            return _structure.Flat.LayerFeedCounts[layerNumber];
        }

        /// <summary>
        /// Get the layer output for the specified neuron.
        /// </summary>
        ///
        /// <param name="layer">The layer.</param>
        /// <param name="neuronNumber">The neuron number.</param>
        /// <returns>The output from the last call to compute.</returns>
        public double GetLayerOutput(int layer, int neuronNumber)
        {
            _structure.RequireFlat();
            int layerNumber = LayerCount - layer - 1;
            int index = _structure.Flat.LayerIndex[layerNumber]
                        + neuronNumber;
            double[] output = _structure.Flat.LayerOutput;
            if (index >= output.Length)
            {

            }
            return output[index];
        }

        /// <summary>
        /// Get the total (including bias and context) neuron cont for a layer.
        /// </summary>
        ///
        /// <param name="l">The layer.</param>
        /// <returns>The count.</returns>
        public int GetLayerTotalNeuronCount(int l)
        {
            _structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;
            return _structure.Flat.LayerCounts[layerNumber];
        }


        /// <summary>
        /// Get the weight between the two layers.
        /// </summary>
        ///
        /// <param name="fromLayer">The from layer.</param>
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <returns>The weight value.</returns>
        public double GetWeight(int fromLayer, int fromNeuron,
                                int toNeuron)
        {
            _structure.RequireFlat();
            ValidateNeuron(fromLayer, fromNeuron);
            ValidateNeuron(fromLayer + 1, toNeuron);
            int fromLayerNumber = LayerCount - fromLayer - 1;
            int toLayerNumber = fromLayerNumber - 1;

            if (toLayerNumber < 0)
            {
                throw new NeuralNetworkError(
                    "The specified layer is not connected to another layer: "
                    + fromLayer);
            }

            int weightBaseIndex = _structure.Flat.WeightIndex[toLayerNumber];
            int count = _structure.Flat.LayerCounts[fromLayerNumber];
            int weightIndex = weightBaseIndex + fromNeuron
                              + (toNeuron * count);

            return _structure.Flat.Weights[weightIndex];
        }

        /// <summary>
        /// Generate a hash code.
        /// </summary>
        ///
        /// <returns>THe hash code.</returns>
        public override sealed int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool IsConnected(int layer, int fromNeuron,
                                int toNeuron)
        {
            /*
			 * if (!this.structure.isConnectionLimited()) { return true; } final
			 * double value = synapse.getMatrix().get(fromNeuron, toNeuron);
			 * 
			 * return (Math.abs(value) > this.structure.getConnectionLimit());
			 */
            return false;
        }


        public bool IsLayerBiased(int l)
        {
            _structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;
            return _structure.Flat.LayerCounts[layerNumber] != _structure.Flat.LayerFeedCounts[layerNumber];
        }


        public void SetLayerBiasActivation(int l, double v)
        {
            if (!IsLayerBiased(l))
            {
                throw new NeuralNetworkError(
                    "Error, the specified layer does not have a bias: " + l);
            }

            _structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;

            int layerOutputIndex = _structure.Flat.LayerIndex[layerNumber];
            int count = _structure.Flat.LayerCounts[layerNumber];
            _structure.Flat.LayerOutput[layerOutputIndex + count - 1] = v;
        }


        public void SetWeight(int fromLayer, int fromNeuron,
                              int toNeuron, double v)
        {
            _structure.RequireFlat();
            int fromLayerNumber = LayerCount - fromLayer - 1;
            int toLayerNumber = fromLayerNumber - 1;

            if (toLayerNumber < 0)
            {

            }

            int weightBaseIndex = _structure.Flat.WeightIndex[toLayerNumber];
            int count = _structure.Flat.LayerCounts[fromLayerNumber];
            int weightIndex = weightBaseIndex + fromNeuron
                              + (toNeuron * count);

            _structure.Flat.Weights[weightIndex] = v;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[BasicNetwork: Layers=");

            int layers = _structure.Flat == null ? _structure.Layers.Count : _structure.Flat.LayerCounts.Length;

            builder.Append(layers);
            builder.Append("]");
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void UpdateProperties()
        {
            _structure.UpdateProperties();
        }

        /// <summary>
        /// Validate the the specified targetLayer and neuron are valid.
        /// </summary>
        ///
        /// <param name="targetLayer">The target layer.</param>
        /// <param name="neuron">The target neuron.</param>
        public void ValidateNeuron(int targetLayer, int neuron)
        {
            if ((targetLayer < 0) || (targetLayer >= LayerCount))
            {

            }

            if ((neuron < 0) || (neuron >= GetLayerTotalNeuronCount(targetLayer)))
            {

            }
        }

        /// <summary>
        /// Determine the winner for the specified input. This is the number of the
        /// winning neuron.
        /// </summary>
        ///
        /// <param name="input">The input patter to present to the neural network.</param>
        /// <returns>The winning neuron.</returns>
        public int Winner(IMLData input)
        {
            IMLData output = Compute(input);
            return EngineArray.MaxIndex(output.Data);
        }
    }
}
