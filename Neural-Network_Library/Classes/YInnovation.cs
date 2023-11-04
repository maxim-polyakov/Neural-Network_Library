using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class YInnovation : BasicInnovation
    {
        /// <summary>
        /// The from neuron id.
        /// </summary>
        ///
        private long fromNeuronID;

        /// <summary>
        /// The type of innovation.
        /// </summary>
        ///
        private YInnovationType innovationType;

        /// <summary>
        /// The neuron id.
        /// </summary>
        ///
        private long neuronID;

        /// <summary>
        /// The type of neuron, or none, if this is a link innovation.
        /// </summary>
        ///
        private YNeuronType neuronType;

        /// <summary>
        /// The split x property.
        /// </summary>
        ///
        private double splitX;

        /// <summary>
        /// The split y property.
        /// </summary>
        ///
        private double splitY;

        /// <summary>
        /// The to neuron's id.
        /// </summary>
        ///
        private long toNeuronID;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public YInnovation()
        {
        }

        /// <summary>
        /// Construct an innovation.
        /// </summary>
        ///
        /// <param name="fromNeuronID_0">The from neuron.</param>
        /// <param name="toNeuronID_1">The two neuron.</param>
        /// <param name="innovationType_2">The innovation type.</param>
        /// <param name="innovationID">The innovation id.</param>
        public YInnovation(long fromNeuronID_0, long toNeuronID_1,
                              YInnovationType innovationType_2, long innovationID)
        {
            fromNeuronID = fromNeuronID_0;
            toNeuronID = toNeuronID_1;
            innovationType = innovationType_2;
            InnovationID = innovationID;

            neuronID = -1;
            splitX = 0;
            splitY = 0;
            neuronType = YNeuronType.None;
        }

        /// <summary>
        /// Construct an innovation.
        /// </summary>
        ///
        /// <param name="fromNeuronID_0">The from neuron.</param>
        /// <param name="toNeuronID_1">The to neuron.</param>
        /// <param name="innovationType_2">The innovation type.</param>
        /// <param name="innovationID">The innovation id.</param>
        /// <param name="neuronType_3">The neuron type.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">THe y coordinate.</param>
        public YInnovation(long fromNeuronID_0, long toNeuronID_1,
                              YInnovationType innovationType_2, long innovationID,
                              YNeuronType neuronType_3, double x, double y)
        {
            fromNeuronID = fromNeuronID_0;
            toNeuronID = toNeuronID_1;
            innovationType = innovationType_2;
            InnovationID = innovationID;
            neuronType = neuronType_3;
            splitX = x;
            splitY = y;

            neuronID = 0;
        }

        /// <summary>
        /// Construct an innovation.
        /// </summary>
        ///
        /// <param name="neuronGene">The neuron gene.</param>
        /// <param name="innovationID">The innovation id.</param>
        /// <param name="neuronID_0">The neuron id.</param>
        public YInnovation(YNeuronGene neuronGene,
                              long innovationID, long neuronID_0)
        {
            neuronID = neuronID_0;
            InnovationID = innovationID;
            splitX = neuronGene.SplitX;
            splitY = neuronGene.SplitY;

            neuronType = neuronGene.NeuronType;
            innovationType = YInnovationType.NewNeuron;
            fromNeuronID = -1;
            toNeuronID = -1;
        }


        /// <value>the fromNeuronID to set</value>
        public long FromNeuronID
        {
            get { return fromNeuronID; }
            set { fromNeuronID = value; }
        }

        /// <summary>
        /// The innovation type.
        /// </summary>
        public YInnovationType InnovationType
        {
            get { return innovationType; }
            set { innovationType = value; }
        }


        /// <summary>
        /// Set the neuron id.
        /// </summary>
        public long NeuronID
        {
            get { return neuronID; }
            set { neuronID = value; }
        }

        /// <summary>
        /// The neuron type.
        /// </summary>
        public YNeuronType NeuronType
        {
            get { return neuronType; }
            set { neuronType = value; }
        }

        /// <summary>
        /// The split X, useful for display, also used during training, max is 1.0.
        /// </summary>
        public double SplitX
        {
            get { return splitX; }
            set { splitX = value; }
        }

        /// <summary>
        /// The split Y, useful for display, also used during training, max is 1.0.
        /// </summary>
        public double SplitY
        {
            get { return splitY; }
            set { splitY = value; }
        }


        /// <value>the toNeuronID to set</value>
        public long ToNeuronID
        {
            get { return toNeuronID; }
            set { toNeuronID = value; }
        }


        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[YInnovation:type=");
            switch (innovationType)
            {
                case YInnovationType.NewLink:
                    result.Append("link");
                    break;
                case YInnovationType.NewNeuron:
                    result.Append("neuron");
                    break;
            }
            result.Append(",from=");
            result.Append(fromNeuronID);
            result.Append(",to=");
            result.Append(toNeuronID);
            result.Append(",splitX=");
            result.Append(splitX);
            result.Append(",splitY=");
            result.Append(splitY);
            result.Append("]");
            return result.ToString();
        }
    }
}
