﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public abstract class ThermalNetwork : BasicML,
                                            IMLAutoAssocation, IMLResettable
    {
        /// <summary>
        /// The current state of the thermal network.
        /// </summary>
        ///
        private BiPolarMLData _currentState;

        /// <summary>
        /// The neuron count.
        /// </summary>
        ///
        private int _neuronCount;

        /// <summary>
        /// The weights.
        /// </summary>
        ///
        private double[] _weights;

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        protected ThermalNetwork()
        {
        }

        /// <summary>
        /// Construct the network with the specicified neuron count.
        /// </summary>
        ///
        /// <param name="neuronCount">The number of neurons.</param>
        protected ThermalNetwork(int neuronCount)
        {
            _neuronCount = neuronCount;
            _weights = new double[neuronCount * neuronCount];
            _currentState = new BiPolarMLData(neuronCount);
        }

        /// <summary>
        /// Set the neuron count.
        /// </summary>
        public int NeuronCount
        {
            get { return _neuronCount; }
            set { _neuronCount = value; }
        }

        /// <summary>
        /// Set the weight array.
        /// </summary>
        ///
        /// <value>The weight array.</value>
        public double[] Weights
        {
            get { return _weights; }
            set { _weights = value; }
        }

        /// <summary>
        /// Set the current state.
        /// </summary>
        public BiPolarMLData CurrentState
        {
            get { return _currentState; }
            set
            {
                for (int i = 0; i < value.Count; i++)
                {
                    _currentState[i] = value[i];
                }
            }
        }

        #region MLAutoAssocation Members

        /// <summary>
        /// from Synt.ml.MLInput
        /// </summary>
        public abstract int InputCount { get; }


        /// <summary>
        /// from Synt.ml.MLOutput
        /// </summary>
        ///
        public abstract int OutputCount { get; }


        /// <summary>
        /// from Synt.ml.MLRegression
        /// </summary>
        ///
        public abstract IMLData Compute(
            IMLData input);

        #endregion

        #region MLResettable Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Reset()
        {
            Reset(0);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Reset(int seed)
        {
            CurrentState.Clear();
            EngineArray.Fill(_weights, 0.0d);
        }

        #endregion

        /// <summary>
        /// Add to the specified weight.
        /// </summary>
        ///
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <param name="v">The value to add.</param>
        public void AddWeight(int fromNeuron, int toNeuron,
                              double v)
        {
            int index = (toNeuron * _neuronCount) + fromNeuron;
            if (index >= _weights.Length)
            {
                throw new NeuralNetworkError("Out of range: fromNeuron:"
                                             + fromNeuron + ", toNeuron: " + toNeuron);
            }
            _weights[index] += v;
        }


        /// <returns>Calculate the current energy for the network. The network will
        /// seek to lower this value.</returns>
        public double CalculateEnergy()
        {
            double tempE = 0;
            int neuronCount = NeuronCount;

            for (int i = 0; i < neuronCount; i++)
            {
                for (int j = 0; j < neuronCount; j++)
                {
                    if (i != j)
                    {
                        tempE += GetWeight(i, j) * _currentState[i]
                                 * _currentState[j];
                    }
                }
            }
            return -1 * tempE / 2;
        }

        /// <summary>
        /// Clear any connection weights.
        /// </summary>
        ///
        public void Clear()
        {
            EngineArray.Fill(_weights, 0);
        }


        /// <summary>
        /// Get a weight.
        /// </summary>
        ///
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <returns>The weight.</returns>
        public double GetWeight(int fromNeuron, int toNeuron)
        {
            int index = (toNeuron * _neuronCount) + fromNeuron;
            return _weights[index];
        }


        /// <summary>
        /// Init the network.
        /// </summary>
        ///
        /// <param name="neuronCount">The neuron count.</param>
        /// <param name="weights">The weights.</param>
        /// <param name="output">The toutpu</param>
        public void Init(int neuronCount, double[] weights,
                         double[] output)
        {
            if (neuronCount != output.Length)
            {
                throw new NeuralNetworkError("Neuron count(" + neuronCount
                                             + ") must match output count(" + output.Length + ").");
            }

            if ((neuronCount * neuronCount) != weights.Length)
            {
                throw new NeuralNetworkError("Weight count(" + weights.Length
                                             + ") must be the square of the neuron count(" + neuronCount
                                             + ").");
            }

            _neuronCount = neuronCount;
            _weights = weights;
            _currentState = new BiPolarMLData(neuronCount) { Data = output };
        }

        /// <summary>
        /// Set the current state.
        /// </summary>
        /// <param name="s">The new current state.</param>
        public void SetCurrentState(double[] s)
        {
            _currentState = new BiPolarMLData(s.Length);
            EngineArray.ArrayCopy(s, _currentState.Data);
        }

        /// <summary>
        /// Set the weight.
        /// </summary>
        ///
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <param name="v">The value.</param>
        public void SetWeight(int fromNeuron, int toNeuron,
                              double v)
        {
            int index = (toNeuron * _neuronCount) + fromNeuron;
            _weights[index] = v;
        }
    }
}
