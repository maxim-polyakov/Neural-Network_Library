using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class YPopulation : BasicPopulation
    {
        /// <summary>
        /// Y activation function tag.
        /// </summary>
        public const String PropertyYActivation = "YAct";

        /// <summary>
        /// Y output activation function.
        /// </summary>
        public const String PropertyOutputActivation = "outAct";

        /// <summary>
        /// The activation function for Y to use.
        /// </summary>
        ///
        private IActivationFunction _YActivationFunction;

        /// <summary>
        /// The activation function to use on the output layer of Synt.
        /// </summary>
        ///
        private IActivationFunction _outputActivationFunction;

        /// <summary>
        /// Are we using snapshot?
        /// </summary>
        private bool _snapshot;

        /// <summary>
        /// Construct a starting Y population.
        /// </summary>
        ///
        /// <param name="inputCount">The input neuron count.</param>
        /// <param name="outputCount">The output neuron count.</param>
        /// <param name="populationSize">The population size.</param>
        public YPopulation(int inputCount, int outputCount,
                              int populationSize) : base(populationSize)
        {
            _YActivationFunction = new ActivationSigmoid();
            _outputActivationFunction = new ActivationLinear();
            InputCount = inputCount;
            OutputCount = outputCount;

            if (populationSize == 0)
            {
                throw new NeuralNetworkError(
                    "Population must have more than zero Ts.");
            }

            // create the initial population
            for (int i = 0; i < populationSize; i++)
            {
                var T = new YT(AssignTID(), inputCount,
                                            outputCount);
                Add(T);
            }

            // create initial innovations
            var T2 = (YT)Ts[0];
            //  Innovations = new YInnovationList(this, T2.Links,                                           T2.Neurons);
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        public YPopulation()
        {
            _YActivationFunction = new ActivationSigmoid();
            _outputActivationFunction = new ActivationLinear();
        }


        /// <value>the inputCount to set</value>
        public int InputCount { get; set; }


        /// <value>the outputCount to set</value>
        public int OutputCount { get; set; }


        /// <value>the YActivationFunction to set</value>
        public IActivationFunction YActivationFunction
        {
            get { return _YActivationFunction; }
            set { _YActivationFunction = value; }
        }


        /// <value>the outputActivationFunction to set</value>
        public IActivationFunction OutputActivationFunction
        {
            get { return _outputActivationFunction; }
            set { _outputActivationFunction = value; }
        }


        /// <value>the snapshot to set</value>
        public bool Snapshot
        {
            get { return _snapshot; }
            set { _snapshot = value; }
        }
    }
}
