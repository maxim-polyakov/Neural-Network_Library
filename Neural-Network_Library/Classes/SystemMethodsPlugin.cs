﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SystemMethodsPlugin : ISyntPluginService1
    {
        /// <summary>
        /// A factory used to create Bayesian networks
        /// </summary>
        private readonly BayesianFactory _bayesianFactory = new BayesianFactory();

        /// <summary>
        /// A factory used to create feedforward neural networks.
        /// </summary>
        private readonly FeedforwardFactory _feedforwardFactory
            = new FeedforwardFactory();

        /// <summary>
        /// The factory for PNN's.
        /// </summary>
        private readonly PNNFactory _pnnFactory = new PNNFactory();

        /// <summary>
        /// A factory used to create RBF networks.
        /// </summary>
        private readonly RBFNetworkFactory _rbfFactory = new RBFNetworkFactory();

        /// <summary>
        /// A factory used to create SOM's.
        /// </summary>
        private readonly SOMFactory _somFactory = new SOMFactory();

        /// <summary>
        /// A factory used to create support vector machines.
        /// </summary>
        private readonly SVMFactory _svmFactory = new SVMFactory();

        #region ISyntPluginService1 Members

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
            get { return "HRI-System-Methods"; }
        }

        /// <summary>
        /// This is a type-1 plugin.
        /// </summary>
        public int PluginType
        {
            get { return 1; }
        }


        /// <inheritdoc/>
        public IActivationFunction CreateActivationFunction(String name)
        {
            return null;
        }

        /// <inheritdoc/>
        public IMLMethod CreateMethod(String methodType, String architecture,
                                      int input, int output)
        {
            if (MLMethodFactory.TypeFeedforward.Equals(methodType))
            {
                return _feedforwardFactory.Create(architecture, input, output);
            }
            if (MLMethodFactory.TypeRbfnetwork.Equals(methodType))
            {
                return _rbfFactory.Create(architecture, input, output);
            }
            if (MLMethodFactory.TypeSVM.Equals(methodType))
            {
                return _svmFactory.Create(architecture, input, output);
            }
            if (MLMethodFactory.TypeSOM.Equals(methodType))
            {
                return _somFactory.Create(architecture, input, output);
            }
            if (MLMethodFactory.TypePNN.Equals(methodType))
            {
                return _pnnFactory.Create(architecture, input, output);
            }
            if (MLMethodFactory.TypeBayesian.Equals(methodType))
            {
                return _bayesianFactory.Create(architecture, input, output);
            }

            throw new SyntError("Unknown method type: " + methodType);
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
            get { return SyntPluginBaseConst.SERVICE_TYPE_GENERAL; }
        }

        #endregion
    }
}
