﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PNNFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MaxLayers = 3;

        /// <summary>
        /// Create a PNN network.
        /// </summary>
        ///
        /// <param name="architecture">THe architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The RBF network.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MaxLayers)
            {
                throw new SyntError(
                    "PNN Networks must have exactly three elements, "
                    + "separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer pnnLayer = ArchitectureParse.ParseLayer(
                layers[1], -1);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            PNNKernelType kernel;
            PNNOutputMode outmodel;

            if (pnnLayer.Name.Equals("c", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Classification;
            }
            else if (pnnLayer.Name.Equals("r", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Regression;
            }
            else if (pnnLayer.Name.Equals("u", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Unsupervised;
            }
            else
            {
                throw new NeuralNetworkError("Unknown model: " + pnnLayer.Name);
            }

            var holder = new ParamsHolder(pnnLayer.Params);

            String kernelStr = holder.GetString("KERNEL", false, "gaussian");

            if (kernelStr.Equals("gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                kernel = PNNKernelType.Gaussian;
            }
            else if (kernelStr.Equals("reciprocal", StringComparison.InvariantCultureIgnoreCase))
            {
                kernel = PNNKernelType.Reciprocal;
            }
            else
            {
                throw new NeuralNetworkError("Unknown kernel: " + kernelStr);
            }

            var result = new BasicPNN(kernel, outmodel, inputCount,
                                      outputCount);

            return result;
        }
    }
}
