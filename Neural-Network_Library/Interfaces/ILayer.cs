﻿using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface ILayer
    {
        /// <value>The activation function used for this layer.</value>
        IActivationFunction ActivationFunction { get; }


        /// <summary>
        /// Set the network that this layer belongs to.
        /// </summary>
        BasicNetwork Network { get; set; }


        /// <value>The neuron count.</value>
        int NeuronCount { get; }


        /// <summary>
        /// Most layer types will default this value to one. However, it is possible
        /// to use other values. This is the activation that will be passed over the
        /// bias weights to the inputs of this layer. See the Layer interface
        /// documentation for more information on how Synt handles bias values.
        /// </summary>
        double BiasActivation { get; set; }


        /// <summary>
        /// Set the activation function.
        /// </summary>
        ///
        /// <value>The activation function.</value>
        IActivationFunction Activation { set; }

        /// <returns>True if this layer has a bias.</returns>
        bool HasBias();
    }
}
