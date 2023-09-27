using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public enum YNeuronType
    {
        /// <summary>
        /// Each Y network has one bias neuron.
        /// </summary>
        ///
        Bias,

        /// <summary>
        /// Hidden neurons are between the input and output.
        /// </summary>
        ///
        Hidden,

        /// <summary>
        /// Input neurons receive input, they are never altered during evolution.
        /// </summary>
        ///
        Input,

        /// <summary>
        /// Not really a neuron type, as you will never see one of these in the
        /// network. However, it is used to mark an innovation as not affecting a
        /// neuron type, but rather a link.
        /// </summary>
        ///
        None,

        /// <summary>
        /// Output neurons provide output, they are never altered during evolution.
        /// </summary>
        ///
        Output
    }
}
