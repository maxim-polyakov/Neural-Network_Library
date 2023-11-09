using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SVMFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MAX_LAYERS = 3;

        /// <summary>
        /// Create the SVM.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SVM.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MAX_LAYERS)
            {
                throw new SyntError(
                    "SVM's must have exactly three elements, separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer paramsLayer = ArchitectureParse.ParseLayer(
                layers[1], input);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            String name = paramsLayer.Name;
            String kernelStr = paramsLayer.Params.ContainsKey("KERNEL") ? paramsLayer.Params["KERNEL"] : null;
            String svmTypeStr = paramsLayer.Params.ContainsKey("TYPE") ? paramsLayer.Params["TYPE"] : null;

            SVMType svmType = SVMType.NewSupportVectorClassification;
            KernelType kernelType = KernelType.RadialBasisFunction;

            bool useNew = true;

            if (svmTypeStr == null)
            {
                useNew = true;
            }
            else if (svmTypeStr.Equals("NEW", StringComparison.InvariantCultureIgnoreCase))
            {
                useNew = true;
            }
            else if (svmTypeStr.Equals("OLD", StringComparison.InvariantCultureIgnoreCase))
            {
                useNew = false;
            }
            else
            {
                throw new SyntError("Unsupported type: " + svmTypeStr
                                     + ", must be NEW or OLD.");
            }

            if (name.Equals("C", StringComparison.InvariantCultureIgnoreCase))
            {
                if (useNew)
                {
                    svmType = SVMType.NewSupportVectorClassification;
                }
                else
                {
                    svmType = SVMType.SupportVectorClassification;
                }
            }
            else if (name.Equals("R", StringComparison.InvariantCultureIgnoreCase))
            {
                if (useNew)
                {
                    svmType = SVMType.NewSupportVectorRegression;
                }
                else
                {
                    svmType = SVMType.EpsilonSupportVectorRegression;
                }
            }
            else
            {
                throw new SyntError("Unsupported mode: " + name
                                     + ", must be C for classify or R for regression.");
            }

            if (kernelStr == null)
            {
                kernelType = KernelType.RadialBasisFunction;
            }
            else if ("linear".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Linear;
            }
            else if ("poly".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Poly;
            }
            else if ("precomputed".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Precomputed;
            }
            else if ("rbf".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.RadialBasisFunction;
            }
            else if ("sigmoid".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Sigmoid;
            }
            else
            {
                throw new SyntError("Unsupported kernel: " + kernelStr
                                     + ", must be linear,poly,precomputed,rbf or sigmoid.");
            }

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            if (outputCount != 1)
            {
                throw new SyntError("SVM can only have an output size of 1.");
            }

            var result = new SupportVectorMachine(inputCount, svmType, kernelType);

            return result;
        }
    }
}
