using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class SOMNetwork : BasicML, IMLClassification, IMLResettable,
                                 IMLError
    {
        /// <summary>
        /// The weights of the output neurons base on the input from the input
	    /// neurons.
        /// </summary>
        private Matrix _weights;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SOMNetwork()
        {
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="inputCount">Number of input neurons</param>
        /// <param name="outputCount">Number of output neurons</param>
        public SOMNetwork(int inputCount, int outputCount)
        {
            _weights = new Matrix(outputCount, inputCount);
        }

        /// <summary>
        /// The weights.
        /// </summary>
        public Matrix Weights
        {
            get { return _weights; }
            set { _weights = value; }
        }

        /// <summary>
        /// Classify the input into one of the output clusters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The cluster it was clasified into.</returns>
        public int Classify(IMLData input)
        {
            if (input.Count > InputCount)
            {
                throw new NeuralNetworkError(
                    "Can't classify SOM with input size of " + InputCount
                    + " with input data of count " + input.Count);
            }

            double[][] m = _weights.Data;
            double[] inputData = input.Data;
            double minDist = Double.PositiveInfinity;
            int result = -1;

            for (int i = 0; i < OutputCount; i++)
            {
                double dist = EngineArray.EuclideanDistance(inputData, m[i]);
                if (dist < minDist)
                {
                    minDist = dist;
                    result = i;
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public int InputCount
        {
            get { return _weights.Cols; }
        }

        /// <inheritdoc/>
        public int OutputCount
        {
            get { return _weights.Rows; }
        }

        /// <summary>
        /// Calculate the error for the specified data set. The error is the largest distance.
        /// </summary>
        /// <param name="data">The data set to check.</param>
        /// <returns>The error.</returns>
        public double CalculateError(IMLDataSet data)
        {
            var bmu = new BestMatchingUnit(this);

            bmu.Reset();

            // Determine the BMU for each training element.
            foreach (IMLDataPair pair in data)
            {
                IMLData input = pair.Input;
                bmu.CalculateBMU(input);
            }

            // update the error
            return bmu.WorstDistance / 100.0;
        }

        /// <summary>
        /// Randomize the network.
        /// </summary>
        public void Reset()
        {
            _weights.Randomize(-1, 1);
        }

        /// <summary>
        /// Randomize the network.
        /// </summary>
        /// <param name="seed">Not used.</param>
        public void Reset(int seed)
        {
            Reset();
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public override void UpdateProperties()
        {
            // unneeded
        }

        /// <summary>
        /// An alias for the classify method, kept for compatibility 
	    /// with earlier versions of Synt.
        /// </summary>
        /// <param name="input">The input pattern.</param>
        /// <returns>The winning neuron.</returns>
        public int Winner(IMLData input)
        {
            return Classify(input);
        }
    }
}
