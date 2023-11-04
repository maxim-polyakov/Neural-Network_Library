﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SystemTrainingPlugin : ISyntPluginService1
    {
        /// <summary>
        /// The factory for simulated annealing.
        /// </summary>
        private readonly AnnealFactory annealFactory = new AnnealFactory();

        /// <summary>
        /// The factory for backprop.
        /// </summary>
        private readonly BackPropFactory backpropFactory = new BackPropFactory();

        /// <summary>
        /// The factory for K2
        /// </summary>
        private readonly TrainBayesianFactory bayesianFactory = new TrainBayesianFactory();

        /// <summary>
        /// The factory for G.
        /// </summary>
        private readonly GFactory GFactory = new GFactory();

        /// <summary>
        /// The factory for LMA.
        /// </summary>
        private readonly LMAFactory lmaFactory = new LMAFactory();

        /// <summary>
        /// The factory for Manhattan networks.
        /// </summary>
        private readonly ManhattanFactory manhattanFactory = new ManhattanFactory();

        /// <summary>
        /// The factory for neighborhood SOM.
        /// </summary>
        private readonly NeighborhoodSOMFactory neighborhoodFactory
            = new NeighborhoodSOMFactory();

        /// <summary>
        /// Nelder Mead Factory.
        /// </summary>
        private readonly NelderMeadFactory nmFactory = new NelderMeadFactory();

        /// <summary>
        /// Factory for PNN.
        /// </summary>
        private readonly PNNTrainFactory pnnFactory = new PNNTrainFactory();

        /// <summary>
        /// PSO training factory.
        /// </summary>
        private readonly PSOFactory psoFactory = new PSOFactory();

        /// <summary>
        /// Factory for quick prop.
        /// </summary>
        private readonly QuickPropFactory qpropFactory = new QuickPropFactory();

        /// <summary>
        /// The factory for RPROP.
        /// </summary>
        private readonly RPROPFactory rpropFactory = new RPROPFactory();

        /// <summary>
        /// The factory for SCG.
        /// </summary>
        private readonly SCGFactory scgFactory = new SCGFactory();

        /// <summary>
        /// The factory for SOM cluster.
        /// </summary>
        private readonly ClusterSOMFactory somClusterFactory = new ClusterSOMFactory();

        /// <summary>
        /// Factory for SVD.
        /// </summary>
        private readonly RBFSVDFactory svdFactory = new RBFSVDFactory();

        /// <summary>
        /// The factory for basic SVM.
        /// </summary>
        private readonly SVMFactory svmFactory = new SVMFactory();

        /// <summary>
        /// The factory for SVM-Search.
        /// </summary>
        private readonly SVMSearchFactory svmSearchFactory = new SVMSearchFactory();

        #region ISyntPluginService1 Members

        /// <inheritdoc/>
        public String PluginDescription
        {
            get
            {
                return "This plugin provides the built in training " +
                       "methods for Synt.";
            }
        }

        /// <inheritdoc/>
        public String PluginName
        {
            get { return "HRI-System-Training"; }
        }

        /// <summary>
        /// This is a type-1 plugin.
        /// </summary>
        public int PluginType
        {
            get { return 1; }
        }


        /// <summary>
        /// This plugin does not support activation functions, so it will 
        /// always return null. 
        /// </summary>
        /// <param name="name">Not used.</param>
        /// <returns>The activation function.</returns>
        public IActivationFunction CreateActivationFunction(String name)
        {
            return null;
        }

        public IMLMethod CreateMethod(String methodType, String architecture,
                                      int input, int output)
        {
            // TODO Auto-generated method stub
            return null;
        }

        public IMLTrain CreateTraining(IMLMethod method, IMLDataSet training,
                                       String type, String args)
        {
            String args2 = args;

            if (args2 == null)
            {
                args2 = "";
            }

            if (String.Compare(MLTrainFactory.TypeRPROP, type) == 0)
            {
                return rpropFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeBackprop, type) == 0)
            {
                return backpropFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSCG, type) == 0)
            {
                return scgFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeLma, type) == 0)
            {
                return lmaFactory.Create(method, training, args2);
            }

            else if (String.Compare(MLTrainFactory.TypeSVMSearch, type) == 0)
            {
                return svmSearchFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSOMNeighborhood, type) == 0)
            {
                return neighborhoodFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeAnneal, type) == 0)
            {
                return annealFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeG, type) == 0)
            {
                return GFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSOMCluster, type) == 0)
            {
                return somClusterFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeManhattan, type) == 0)
            {
                return manhattanFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeSvd, type) == 0)
            {
                return svdFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypePNN, type) == 0)
            {
                return pnnFactory.Create(method, training, args2);
            }
            else if (String.Compare(MLTrainFactory.TypeQPROP, type) == 0)
            {
                return qpropFactory.Create(method, training, args2);
            }
            else if (MLTrainFactory.TypeBayesian.Equals(type))
            {
                return bayesianFactory.Create(method, training, args2);
            }
            else if (MLTrainFactory.TypeNelderMead.Equals(type))
            {
                return nmFactory.Create(method, training, args2);
            }
            else if (MLTrainFactory.TypePSO.Equals(type))
            {
                return psoFactory.Create(method, training, args2);
            }

            else
            {
                throw new SyntError("Unknown training type: " + type);
            }
        }

        /// <inheritdoc/>
        public int PluginServiceType
        {
            get { return SyntPluginBaseConst.SERVICE_TYPE_GENERAL; }
        }

        #endregion
    }
}
