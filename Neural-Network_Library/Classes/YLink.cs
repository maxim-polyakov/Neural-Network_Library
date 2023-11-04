using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class YLink
    {
        /// <summary>
        /// The source neuron.
        /// </summary>
        ///
        private readonly YNeuron _fromNeuron;

        /// <summary>
        /// Is this link recurrent.
        /// </summary>
        ///
        private readonly bool _recurrent;

        /// <summary>
        /// The target neuron.
        /// </summary>
        ///
        private readonly YNeuron _toNeuron;

        /// <summary>
        /// The weight between the two neurons.
        /// </summary>
        ///
        private readonly double _weight;

        /// <summary>
        /// Default constructor, used mainly for persistance.
        /// </summary>
        ///
        public YLink()
        {
        }

        /// <summary>
        /// Construct a Y link.
        /// </summary>
        ///
        /// <param name="weight">The weight between the two neurons.</param>
        /// <param name="fromNeuron">The source neuron.</param>
        /// <param name="toNeuron">The target neuron.</param>
        /// <param name="recurrent">Is this a recurrent link.</param>
        public YLink(double weight, YNeuron fromNeuron,
                        YNeuron toNeuron, bool recurrent)
        {
            _weight = weight;
            _fromNeuron = fromNeuron;
            _toNeuron = toNeuron;
            _recurrent = recurrent;
        }


        /// <value>The source neuron.</value>
        public YNeuron FromNeuron
        {
            get { return _fromNeuron; }
        }


        /// <value>The target neuron.</value>
        public YNeuron ToNeuron
        {
            get { return _toNeuron; }
        }


        /// <value>The weight of the link.</value>
        public double Weight
        {
            get { return _weight; }
        }


        /// <value>True if this is a recurrent link.</value>
        public bool Recurrent
        {
            get { return _recurrent; }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[YLink: fromNeuron=");
            result.Append(FromNeuron.NeuronID);
            result.Append(", toNeuron=");
            result.Append(ToNeuron.NeuronID);
            result.Append("]");
            return result.ToString();
        }
    }
}
