using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BoltzmannPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of annealing cycles per run.
        /// </summary>
        ///
        private int _annealCycles;

        /// <summary>
        /// The number of neurons in the Boltzmann network.
        /// </summary>
        ///
        private int _neuronCount;

        /// <summary>
        /// The number of cycles per run.
        /// </summary>
        ///
        private int _runCycles;

        /// <summary>
        /// The current temperature.
        /// </summary>
        ///
        private double _temperature;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public BoltzmannPattern()
        {
            _annealCycles = 100;
            _runCycles = 1000;
            _temperature = 0.0d;
        }

        /// <summary>
        /// Set the number of annealing cycles per run.
        /// </summary>
        public int AnnealCycles
        {
            get { return _annealCycles; }
            set { _annealCycles = value; }
        }


        /// <summary>
        /// Set the number of cycles per run.
        /// </summary>
        public int RunCycles
        {
            get { return _runCycles; }
            set { _runCycles = value; }
        }


        /// <summary>
        /// Set the temperature.
        /// </summary>
        public double Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Not supported, will throw an exception, Boltzmann networks have no hidden
        /// layers.
        /// </summary>
        ///
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A Boltzmann network has no hidden layers.");
        }

        /// <summary>
        /// Clear any properties set on this network.
        /// </summary>
        ///
        public void Clear()
        {
            _neuronCount = 0;
        }

        /// <summary>
        /// Generate the network.
        /// </summary>
        public IMLMethod Generate()
        {
            var boltz = new BoltzmannMachine(_neuronCount);
            boltz.Temperature = _temperature;
            boltz.RunCycles = _runCycles;
            boltz.AnnealCycles = _annealCycles;
            return boltz;
        }


        /// <summary>
        /// Not used, will throw an exception.
        /// </summary>
        ///
        /// <value>Not used.</value>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A Boltzmann network will use the BiPolar activation "
                    + "function, no activation function needs to be specified.");
            }
        }


        /// <summary>
        /// Set the number of input neurons. This is the same as the number of output
        /// neurons.
        /// </summary>
        ///
        /// <value>The number of input neurons.</value>
        public int InputNeurons
        {
            set { _neuronCount = value; }
        }


        /// <summary>
        /// Set the number of output neurons. This is the same as the number of input
        /// neurons.
        /// </summary>
        public int OutputNeurons
        {
            set { _neuronCount = value; }
        }

        #endregion
    }
}
