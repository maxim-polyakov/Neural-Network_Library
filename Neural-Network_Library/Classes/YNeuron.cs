using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class YNeuron
    {
        /// <summary>
        /// The activation response. This is evolved to allow Y to scale the slope
        /// of the activation function.
        /// </summary>
        ///
        private readonly double _activationResponse;

        /// <summary>
        /// Inbound links to this neuron.
        /// </summary>
        ///
        private readonly IList<YLink> _inboundLinks;

        /// <summary>
        /// The neuron id.
        /// </summary>
        ///
        private readonly long _neuronID;

        /// <summary>
        /// The type of neuron this is.
        /// </summary>
        ///
        private readonly YNeuronType _neuronType;

        /// <summary>
        /// The outbound links for this neuron.
        /// </summary>
        ///
        private readonly IList<YLink> _outputboundLinks;

        /// <summary>
        /// The x-position of this neuron. Used to split links, as well as display.
        /// </summary>
        ///
        private readonly int _posX;

        /// <summary>
        /// The y-position of this neuron. Used to split links, as well as display.
        /// </summary>
        ///
        private readonly int _posY;

        /// <summary>
        /// The split value for X. Used to track splits.
        /// </summary>
        ///
        private readonly double _splitX;

        /// <summary>
        /// The split value for Y. Used to track splits.
        /// </summary>
        ///
        private readonly double _splitY;

        /// <summary>
        /// The sum activation.
        /// </summary>
        ///
        private readonly double _sumActivation;

        /// <summary>
        /// The output from the neuron.
        /// </summary>
        ///
        private double _output;

        /// <summary>
        /// Default constructor, used for persistance.
        /// </summary>
        ///
        public YNeuron()
        {
            _inboundLinks = new List<YLink>();
            _outputboundLinks = new List<YLink>();
        }

        /// <summary>
        /// Construct a Y neuron.
        /// </summary>
        ///
        /// <param name="neuronType_0">The type of neuron.</param>
        /// <param name="neuronID_1">The id of the neuron.</param>
        /// <param name="splitY_2">The split for y.</param>
        /// <param name="splitX_3">THe split for x.</param>
        /// <param name="activationResponse_4">The activation response.</param>
        public YNeuron(YNeuronType neuronType_0, long neuronID_1,
                          double splitY_2, double splitX_3,
                          double activationResponse_4)
        {
            _inboundLinks = new List<YLink>();
            _outputboundLinks = new List<YLink>();
            _neuronType = neuronType_0;
            _neuronID = neuronID_1;
            _splitY = splitY_2;
            _splitX = splitX_3;
            _activationResponse = activationResponse_4;
            _posX = 0;
            _posY = 0;
            _output = 0;
            _sumActivation = 0;
        }


        /// <value>the activation response.</value>
        public double ActivationResponse
        {
            get { return _activationResponse; }
        }


        /// <value>the inbound links.</value>
        public IList<YLink> InboundLinks
        {
            get { return _inboundLinks; }
        }


        /// <value>The neuron id.</value>
        public long NeuronID
        {
            get { return _neuronID; }
        }


        /// <value>the neuron type.</value>
        public YNeuronType NeuronType
        {
            get { return _neuronType; }
        }


        /// <value>The output of the neuron.</value>
        public double Output
        {
            get { return _output; }
            set { _output = value; }
        }


        /// <value>The outbound links.</value>
        public IList<YLink> OutputboundLinks
        {
            get { return _outputboundLinks; }
        }


        /// <value>The x position.</value>
        public int PosX
        {
            get { return _posX; }
        }


        /// <value>The y position.</value>
        public int PosY
        {
            get { return _posY; }
        }


        /// <value>The split x.</value>
        public double SplitX
        {
            get { return _splitX; }
        }


        /// <value>The split y.</value>
        public double SplitY
        {
            get { return _splitY; }
        }


        /// <value>The sum activation.</value>
        public double SumActivation
        {
            get { return _sumActivation; }
        }


        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[YNeuron:id=");
            result.Append(_neuronID);
            result.Append(",type=");
            switch (_neuronType)
            {
                case YNeuronType.Input:
                    result.Append("I");
                    break;
                case YNeuronType.Output:
                    result.Append("O");
                    break;
                case YNeuronType.Bias:
                    result.Append("B");
                    break;
                case YNeuronType.Hidden:
                    result.Append("H");
                    break;
                default:
                    result.Append("Unknown");
                    break;
            }
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// Convert a string to a Y neuron type.
        /// </summary>
        /// <param name="t">The string.</param>
        /// <returns>The Y neuron type.</returns>
        public static YNeuronType String2NeuronType(String t)
        {
            String type = t.ToLower().Trim();

            if (type.Length > 0)
            {
                switch ((int)type[0])
                {
                    case 'i':
                        return YNeuronType.Input;
                    case 'o':
                        return YNeuronType.Output;
                    case 'h':
                        return YNeuronType.Hidden;
                    case 'b':
                        return YNeuronType.Bias;
                    case 'n':
                        return YNeuronType.None;
                }
            }

            return default(YNeuronType) /* was: null */;
        }

        /// <summary>
        /// Convert Y neuron type to string.
        /// </summary>
        /// <param name="t">The neuron type.</param>
        /// <returns>The string of the specified neuron type.</returns>
        public static String NeuronType2String(YNeuronType t)
        {
            switch (t)
            {
                case YNeuronType.Input:
                    return "I";
                case YNeuronType.Bias:
                    return "B";
                case YNeuronType.Hidden:
                    return "H";
                case YNeuronType.Output:
                    return "O";
                case YNeuronType.None:
                    return "N";
                default:
                    return null;
            }
        }
    }
}
