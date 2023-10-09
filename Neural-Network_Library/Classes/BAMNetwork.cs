using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class BAMNetwork : BasicML
    {
        /// <summary>
        /// Neurons in the F1 layer.
        /// </summary>
        ///
        private int _f1Count;

        /// <summary>
        /// Neurons in the F2 layer.
        /// </summary>
        ///
        private int _f2Count;

        /// <summary>
        /// The weights between the F1 and F2 layers.
        /// </summary>
        ///
        private Matrix _weightsF1ToF2;

        /// <summary>
        /// The weights between the F1 and F2 layers.
        /// </summary>
        ///
        private Matrix _weightsF2ToF1;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public BAMNetwork()
        {
        }

        /// <summary>
        /// Construct the BAM network.
        /// </summary>
        ///
        /// <param name="theF1Count">The F1 count.</param>
        /// <param name="theF2Count">The F2 count.</param>
        public BAMNetwork(int theF1Count, int theF2Count)
        {
            _f1Count = theF1Count;
            _f2Count = theF2Count;

            _weightsF1ToF2 = new Matrix(_f1Count, _f2Count);
            _weightsF2ToF1 = new Matrix(_f2Count, _f1Count);
        }

        /// <summary>
        /// Set the F1 neuron count.
        /// </summary>
        public int F1Count
        {
            get { return _f1Count; }
            set { _f1Count = value; }
        }


        /// <summary>
        /// Set the F2 neuron count.
        /// </summary>
        public int F2Count
        {
            get { return _f2Count; }
            set { _f2Count = value; }
        }

        /// <summary>
        /// Set the weights for F1 to F2.
        /// </summary>
        public Matrix WeightsF1ToF2
        {
            get { return _weightsF1ToF2; }
            set { _weightsF1ToF2 = value; }
        }


        /// <summary>
        /// Set the weights for F2 to F1.
        /// </summary>
        public Matrix WeightsF2ToF1
        {
            get { return _weightsF2ToF1; }
            set { _weightsF2ToF1 = value; }
        }

        /// <summary>
        /// Add a pattern to the neural network.
        /// </summary>
        ///
        /// <param name="inputPattern">The input pattern.</param>
        /// <param name="outputPattern">The output pattern(for this input).</param>
        public void AddPattern(IMLData inputPattern,
                               IMLData outputPattern)
        {
            for (int i = 0; i < _f1Count; i++)
            {
                for (int j = 0; j < _f2Count; j++)
                {
                    var weight = (int)(inputPattern[i] * outputPattern[j]);
                    _weightsF1ToF2.Add(i, j, weight);
                    _weightsF2ToF1.Add(j, i, weight);
                }
            }
        }

        /// <summary>
        /// Clear any connection weights.
        /// </summary>
        ///
        public void Clear()
        {
            _weightsF1ToF2.Clear();
            _weightsF2ToF1.Clear();
        }

        /// <summary>
        /// Setup the network logic, read parameters from the network. NOT USED, call
        /// compute(NeuralInputData).
        /// </summary>
        ///
        /// <param name="input">NOT USED</param>
        /// <returns>NOT USED</returns>
        public IMLData Compute(IMLData input)
        {
            throw new NeuralNetworkError(
                "Compute on BasicNetwork cannot be used, rather call"
                + " the compute(NeuralData) method on the BAMLogic.");
        }

        /// <summary>
        /// Compute the network for the specified input.
        /// </summary>
        ///
        /// <param name="input">The input to the network.</param>
        /// <returns>The output from the network.</returns>
        public NeuralDataMapping Compute(NeuralDataMapping input)
        {
            bool stable1;
            bool stable2;

            do
            {
                stable1 = PropagateLayer(_weightsF1ToF2, input.From,
                                         input.To);
                stable2 = PropagateLayer(_weightsF2ToF1, input.To,
                                         input.From);
            } while (!stable1 && !stable2);
            return null;
        }


        /// <summary>
        /// Get the specified weight.
        /// </summary>
        ///
        /// <param name="matrix">The matrix to use.</param>
        /// <param name="input">The input, to obtain the size from.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The value from the matrix.</returns>
        private static double GetWeight(Matrix matrix, IMLData input,
                                 int x, int y)
        {
            if (matrix.Rows != input.Count)
            {
                return matrix[x, y];
            }
            return matrix[y, x];
        }


        /// <summary>
        /// Propagate the layer.
        /// </summary>
        ///
        /// <param name="matrix">The matrix for this layer.</param>
        /// <param name="input">The input pattern.</param>
        /// <param name="output">The output pattern.</param>
        /// <returns>True if the network has become stable.</returns>
        private static bool PropagateLayer(Matrix matrix, IMLData input,
                                    IMLData output)
        {
            int i;

            bool stable = true;

            for (i = 0; i < output.Count; i++)
            {
                double sum = 0; // **FIX** ?? int ??
                int j;
                for (j = 0; j < input.Count; j++)
                {
                    sum += GetWeight(matrix, input, i, j) * input[j];
                }
                if (sum != 0)
                {
                    int xout;
                    if (sum < 0)
                    {
                        xout = -1;
                    }
                    else
                    {
                        xout = 1;
                    }
                    if (xout != (int)output[i])
                    {
                        stable = false;
                        output[i] = xout;
                    }
                }
            }
            return stable;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void UpdateProperties()
        {
            // TODO Auto-generated method stub
        }
    }
}
