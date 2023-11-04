using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class YNeuronGene : BasicGene
    {
        /// <summary>
        /// The activation response tag.
        /// </summary>
        public const String PROPERTY_ACT_RESPONSE = "aResp";

        /// <summary>
        /// The recurrent tag.
        /// </summary>
        public const String PROPERTY_RECURRENT = "recurrent";

        /// <summary>
        /// The split-x tag.
        /// </summary>
        public const String PROPERTY_SPLIT_X = "splitX";

        /// <summary>
        /// The split-y tag.
        /// </summary>
        public const String PROPERTY_SPLIT_Y = "splitY";

        /// <summary>
        /// The activation response, the slope of the activation function.
        /// </summary>
        ///
        private double activationResponse;

        /// <summary>
        /// The neuron type.
        /// </summary>
        ///
        private YNeuronType neuronType;

        /// <summary>
        /// True if this is recurrent.
        /// </summary>
        ///
        private bool recurrent;

        /// <summary>
        /// The x-split.
        /// </summary>
        ///
        private double splitX;

        /// <summary>
        /// The y-split.
        /// </summary>
        ///
        private double splitY;

        /// <summary>
        /// The default constructor.
        /// </summary>
        ///
        public YNeuronGene()
        {
        }

        /// <summary>
        /// Construct a gene.
        /// </summary>
        ///
        /// <param name="type">The type of neuron.</param>
        /// <param name="id">The id of this gene.</param>
        /// <param name="splitY_0">The split y.</param>
        /// <param name="splitX_1">The split x.</param>
        public YNeuronGene(YNeuronType type, long id,
                              double splitY_0, double splitX_1) : this(type, id, splitY_0, splitX_1, false, 1.0d)
        {
        }

        /// <summary>
        /// Construct a neuron gene.
        /// </summary>
        ///
        /// <param name="type">The type of neuron.</param>
        /// <param name="id">The id of this gene.</param>
        /// <param name="splitY_0">The split y.</param>
        /// <param name="splitX_1">The split x.</param>
        /// <param name="recurrent_2">True if this is a recurrent link.</param>
        /// <param name="act">The activation response.</param>
        public YNeuronGene(YNeuronType type, long id,
                              double splitY_0, double splitX_1, bool recurrent_2,
                              double act)
        {
            neuronType = type;
            Id = id;
            splitX = splitX_1;
            splitY = splitY_0;
            recurrent = recurrent_2;
            activationResponse = act;
        }

        /// <summary>
        /// Set the activation response.
        /// </summary>
        public double ActivationResponse
        {
            get { return activationResponse; }
            set { activationResponse = value; }
        }


        /// <summary>
        /// Set the neuron type.
        /// </summary>
        public YNeuronType NeuronType
        {
            get { return neuronType; }
            set { neuronType = value; }
        }


        /// <summary>
        /// Set the split x.
        /// </summary>
        public double SplitX
        {
            get { return splitX; }
            set { splitX = value; }
        }


        /// <summary>
        /// Set the split y.
        /// </summary>
        public double SplitY
        {
            get { return splitY; }
            set { splitY = value; }
        }


        /// <summary>
        /// Set if this is a recurrent neuron.
        /// </summary>
        public bool Recurrent
        {
            get { return recurrent; }
            set { recurrent = value; }
        }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        ///
        /// <param name="gene">The other gene.</param>
        public override void Copy(IGene gene)
        {
            var other = (YNeuronGene)gene;
            activationResponse = other.activationResponse;
            Id = other.Id;
            neuronType = other.neuronType;
            recurrent = other.recurrent;
            splitX = other.splitX;
            splitY = other.splitY;
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[YNeuronGene: id=");
            result.Append(Id);
            result.Append(", type=");
            result.Append(NeuronType);
            result.Append("]");
            return result.ToString();
        }
    }
}
