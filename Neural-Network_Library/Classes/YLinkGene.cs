﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class YLinkGene : BasicGene
    {
        /// <summary>
        /// The from neuron id.
        /// </summary>
        ///
        private long fromNeuronID;

        /// <summary>
        /// Is this a recurrent connection.
        /// </summary>
        ///
        private bool recurrent;

        /// <summary>
        /// The to neuron id.
        /// </summary>
        ///
        private long toNeuronID;

        /// <summary>
        /// The weight of this link.
        /// </summary>
        ///
        private double weight;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public YLinkGene()
        {
        }

        /// <summary>
        /// Construct a Y link gene.
        /// </summary>
        ///
        /// <param name="fromNeuronID_0">The source neuron.</param>
        /// <param name="toNeuronID_1">The target neuron.</param>
        /// <param name="enabled">Is this link enabled.</param>
        /// <param name="innovationID">The innovation id.</param>
        /// <param name="weight_2">The weight.</param>
        /// <param name="recurrent_3">Is this a recurrent link?</param>
        public YLinkGene(long fromNeuronID_0, long toNeuronID_1,
                            bool enabled, long innovationID,
                            double weight_2, bool recurrent_3)
        {
            fromNeuronID = fromNeuronID_0;
            toNeuronID = toNeuronID_1;
            Enabled = enabled;
            InnovationId = innovationID;
            weight = weight_2;
            recurrent = recurrent_3;
        }

        /// <summary>
        /// Set the weight of this connection.
        /// </summary>
        public double Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        /// <summary>
        /// True if this is a recurrent link.
        /// </summary>
        public bool Recurrent
        {
            get { return recurrent; }
            set { recurrent = value; }
        }

        /// <summary>
        /// The from neuron id.
        /// </summary>
        public int FromNeuronID
        {
            get { return (int)fromNeuronID; }
            set { fromNeuronID = value; }
        }

        /// <summary>
        /// The to neuron id.
        /// </summary>
        public int ToNeuronID
        {
            get { return (int)toNeuronID; }
            set { toNeuronID = value; }
        }

        /// <summary>
        /// Copy from another gene.
        /// </summary>
        /// <param name="gene">The other gene.</param>
        public override void Copy(IGene gene)
        {
            var other = (YLinkGene)gene;
            Enabled = other.Enabled;
            fromNeuronID = other.fromNeuronID;
            toNeuronID = other.toNeuronID;
            InnovationId = other.InnovationId;
            recurrent = other.recurrent;
            weight = other.weight;
        }


        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[YLinkGene:innov=");
            result.Append(InnovationId);
            result.Append(",enabled=");
            result.Append(Enabled);
            result.Append(",from=");
            result.Append(fromNeuronID);
            result.Append(",to=");
            result.Append(toNeuronID);
            result.Append("]");
            return result.ToString();
        }
    }
}
