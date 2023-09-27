using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BasicLayer : FlatLayer, ILayer
    {
        /// <summary>
        /// The network that this layer belongs to.
        /// </summary>
        ///
        private BasicNetwork _network;

        /// <summary>
        /// Construct this layer with a non-default activation function, also
        /// determine if a bias is desired or not.
        /// </summary>
        ///
        /// <param name="activationFunction">The activation function to use.</param>
        /// <param name="neuronCount">How many neurons in this layer.</param>
        /// <param name="hasBias">True if this layer has a bias.</param>
        public BasicLayer(IActivationFunction activationFunction,
                          bool hasBias, int neuronCount)
            : base(activationFunction, neuronCount, (hasBias) ? 1.0d : 0.0d)
        {
        }

        /// <summary>
        /// Construct this layer with a sigmoid activation function.
        /// </summary>
        public BasicLayer(int neuronCount) : this(new ActivationTANH(), true, neuronCount)
        {
        }

        #region Layer Members

        /// <summary>
        /// Set the network for this layer.
        /// </summary>
        public virtual BasicNetwork Network
        {
            get { return _network; }
            set { _network = value; }
        }

        /// <summary>
        /// THe neuron count.
        /// </summary>
        public virtual int NeuronCount
        {
            get { return Count; }
        }

        /// <summary>
        /// The activation function.
        /// </summary>
        public virtual IActivationFunction ActivationFunction
        {
            get { return Activation; }
        }

        #endregion
    }
}
