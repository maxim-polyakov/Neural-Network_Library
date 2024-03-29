﻿using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SystemActivationPlugin : ISyntPluginService1
    {
        /// <inheritdoc/>
        public String PluginDescription
        {
            get
            {
                return "This plugin provides the built in machine " +
                        "learning methods for Synt.";
            }
        }

        /// <inheritdoc/>
        public String PluginName
        {
            get
            {
                return "HRI-System-Methods";
            }
        }

        /// <summary>
        /// This is a type-1 plugin.
        /// </summary>
        public int PluginType
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Allocate an activation function.
        /// </summary>
        /// <param name="name">The name of the activation function.</param>
        /// <returns>The activation function.</returns>
        private IActivationFunction AllocateAF(String name)
        {
            if (String.Compare(name, MLActivationFactory.AF_BIPOLAR) == 0)
            {
                return new ActivationBiPolar();
            }

            if (String.Compare(name, MLActivationFactory.AF_COMPETITIVE) == 0)
            {
                return new ActivationCompetitive();
            }

            if (String.Compare(name, MLActivationFactory.AF_GAUSSIAN) == 0)
            {
                return new ActivationGaussian();
            }

            if (String.Compare(name, MLActivationFactory.AF_LINEAR) == 0)
            {
                return new ActivationLinear();
            }

            if (String.Compare(name, MLActivationFactory.AF_LOG) == 0)
            {
                return new ActivationLOG();
            }

            if (String.Compare(name, MLActivationFactory.AF_RAMP) == 0)
            {
                return new ActivationRamp();
            }

            if (String.Compare(name, MLActivationFactory.AF_SIGMOID) == 0)
            {
                return new ActivationSigmoid();
            }

            if (String.Compare(name, MLActivationFactory.AF_SIN) == 0)
            {
                return new ActivationSIN();
            }

            if (String.Compare(name, MLActivationFactory.AF_SOFTMAX) == 0)
            {
                return new ActivationSoftMax();
            }

            if (String.Compare(name, MLActivationFactory.AF_STEP) == 0)
            {
                return new ActivationStep();
            }

            if (String.Compare(name, MLActivationFactory.AF_TANH) == 0)
            {
                return new ActivationTANH();
            }

            return null;
        }


        /// <inheritdoc/>
        public IActivationFunction CreateActivationFunction(String fn)
        {
            String name;
            double[] p;

            int index = fn.IndexOf('[');
            if (index != -1)
            {
                name = fn.Substring(0, index).ToLower();
                int index2 = fn.IndexOf(']');
                if (index2 == -1)
                {
                    throw new SyntError(
                            "Unbounded [ while parsing activation function.");
                }
                String a = fn.Substring(index + 1, index2);
                p = NumberList.FromList(CSVFormat.EgFormat, a);

            }
            else
            {
                name = fn.ToLower();
                p = new double[0];
            }

            IActivationFunction af = AllocateAF(name);

            if (af == null)
            {
                return null;
            }

            if (af.ParamNames.Length != p.Length)
            {
                throw new SyntError(name + " expected "
                        + af.ParamNames.Length + ", but " + p.Length
                        + " were provided.");
            }

            for (int i = 0; i < af.ParamNames.Length; i++)
            {
                af.Params[i] = p[i];
            }

            return af;
        }

        /// <inheritdoc/>
        public IMLMethod CreateMethod(String methodType, String architecture,
                int input, int output)
        {
            return null;
        }

        /// <inheritdoc/>
        public IMLTrain CreateTraining(IMLMethod method, IMLDataSet training,
                String type, String args)
        {
            return null;
        }

        /// <inheritdoc/>
        public int PluginServiceType
        {
            get
            {
                return SyntPluginBaseConst.SERVICE_TYPE_GENERAL;
            }
        }
    }
}
