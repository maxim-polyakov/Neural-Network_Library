using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class NeuralStructure
    {
        /// <summary>
        /// The layers in this neural network.
        /// </summary>
        ///
        private readonly IList<ILayer> _layers;

        /// <summary>
        /// The neural network this class belongs to.
        /// </summary>
        ///
        private readonly BasicNetwork _network;

        /// <summary>
        /// The limit, below which a connection is treated as zero.
        /// </summary>
        ///
        private double _connectionLimit;

        /// <summary>
        /// Are connections limited?
        /// </summary>
        ///
        private bool _connectionLimited;

        /// <summary>
        /// The flattened form of the network.
        /// </summary>
        ///
        private FlatNetwork _flat;

        /// <summary>
        /// Construct a structure object for the specified network.
        /// </summary>
        public NeuralStructure(BasicNetwork network)
        {
            _layers = new List<ILayer>();
            _network = network;
        }


        /// <value>The connection limit.</value>
        public double ConnectionLimit
        {
            get { return _connectionLimit; }
        }


        /// <summary>
        /// Set the flat network.
        /// </summary>
        public FlatNetwork Flat
        {
            get
            {
                RequireFlat();
                return _flat;
            }
            set { _flat = value; }
        }


        /// <value>The layers in this neural network.</value>
        public IList<ILayer> Layers
        {
            get { return _layers; }
        }


        /// <value>The network this structure belongs to.</value>
        public BasicNetwork Network
        {
            get { return _network; }
        }


        /// <value>True if this is not a fully connected feedforward network.</value>
        public bool ConnectionLimited
        {
            get { return _connectionLimited; }
        }

        /// <summary>
        /// Calculate the size that an array should be to hold all of the weights and
        /// bias values.
        /// </summary>
        ///
        /// <returns>The size of the calculated array.</returns>
        public int CalculateSize()
        {
            return NetworkCODEC.NetworkSize(_network);
        }

        /// <summary>
        /// Enforce that all connections are above the connection limit. Any
        /// connections below this limit will be severed.
        /// </summary>
        ///
        public void EnforceLimit()
        {
            if (!_connectionLimited)
            {
                return;
            }

            double[] weights = _flat.Weights;

            for (int i = 0; i < weights.Length; i++)
            {
                if (Math.Abs(weights[i]) < _connectionLimit)
                {
                    weights[i] = 0;
                }
            }
        }

        /// <summary>
        /// Parse/finalize the limit value for connections.
        /// </summary>
        ///
        public void FinalizeLimit()
        {
            // see if there is a connection limit imposed
            String limit = _network
                .GetPropertyString(BasicNetwork.TagLimit);
            if (limit != null)
            {
                try
                {
                    _connectionLimited = true;
                    _connectionLimit = CSVFormat.EgFormat.Parse(limit);
                    EnforceLimit();
                }
                catch (FormatException)
                {

                }
            }
            else
            {
                _connectionLimited = false;
                _connectionLimit = 0;
            }
        }

        /// <summary>
        /// Build the synapse and layer structure. This method should be called after
        /// you are done adding layers to a network, or change the network's logic
        /// property.
        /// </summary>
        ///
        public void FinalizeStructure()
        {
            if (_layers.Count < 2)
            {
            }

            var flatLayers = new FlatLayer[_layers.Count];

            for (int i = 0; i < _layers.Count; i++)
            {
                var layer = (BasicLayer)_layers[i];
                if (layer.Activation == null)
                {
                    layer.Activation = new ActivationLinear();
                }

                flatLayers[i] = layer;
            }

            _flat = new FlatNetwork(flatLayers);

            FinalizeLimit();
            _layers.Clear();
            EnforceLimit();
        }


        /// <summary>
        /// Throw an error if there is no flat network.
        /// </summary>
        ///
        public void RequireFlat()
        {
            if (_flat == null)
            {

            }
        }

        /// <summary>
        /// Update any properties from the property map.
        /// </summary>
        ///
        public void UpdateProperties()
        {
            if (_network.Properties.ContainsKey(BasicNetwork.TagLimit))
            {
                _connectionLimit = _network
                    .GetPropertyDouble(BasicNetwork.TagLimit);
                _connectionLimited = true;
            }
            else
            {
                _connectionLimited = false;
                _connectionLimit = 0;
            }

            if (_flat != null)
            {
                _flat.ConnectionLimit = _connectionLimit;
            }
        }
    }
}
