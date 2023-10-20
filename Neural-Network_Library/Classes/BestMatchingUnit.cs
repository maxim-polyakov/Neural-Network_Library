using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BestMatchingUnit
    {
        /// <summary>
        /// The owner of this class.
        /// </summary>
        ///
        private readonly SOMNetwork _som;

        /// <summary>
        /// What is the worst BMU distance so far, this becomes the error for the
        /// entire SOM.
        /// </summary>
        ///
        private double _worstDistance;

        /// <summary>
        /// Construct a BestMatchingUnit class.  The training class must be provided.
        /// </summary>
        ///
        /// <param name="som">The SOM to evaluate.</param>
        public BestMatchingUnit(SOMNetwork som)
        {
            _som = som;
        }

        /// <value>What is the worst BMU distance so far, this becomes the error 
        /// for the entire SOM.</value>
        public double WorstDistance
        {
            get { return _worstDistance; }
        }

        /// <summary>
        /// Calculate the best matching unit (BMU). This is the output neuron that
        /// has the lowest Euclidean distance to the input vector.
        /// </summary>
        ///
        /// <param name="input">The input vector.</param>
        /// <returns>The output neuron number that is the BMU.</returns>
        public int CalculateBMU(IMLData input)
        {
            int result = 0;

            // Track the lowest distance so far.
            double lowestDistance = Double.MaxValue;

            for (int i = 0; i < _som.OutputCount; i++)
            {
                double distance = CalculateEuclideanDistance(
                    _som.Weights, input, i);

                // Track the lowest distance, this is the BMU.
                if (distance < lowestDistance)
                {
                    lowestDistance = distance;
                    result = i;
                }
            }

            // Track the worst distance, this is the error for the entire network.
            if (lowestDistance > _worstDistance)
            {
                _worstDistance = lowestDistance;
            }

            return result;
        }

        /// <summary>
        /// Calculate the Euclidean distance for the specified output neuron and the
        /// input vector.  This is the square root of the squares of the differences
        /// between the weight and input vectors.
        /// </summary>
        ///
        /// <param name="matrix">The matrix to get the weights from.</param>
        /// <param name="input">The input vector.</param>
        /// <param name="outputNeuron">The neuron we are calculating the distance for.</param>
        /// <returns>The Euclidean distance.</returns>
        public double CalculateEuclideanDistance(Matrix matrix,
                                                 IMLData input, int outputNeuron)
        {
            double result = 0;

            // Loop over all input data.
            for (int i = 0; i < input.Count; i++)
            {
                double diff = input[i] - matrix[outputNeuron, i];
                result += diff * diff;
            }
            return BoundMath.Sqrt(result);
        }


        /// <summary>
        /// Reset the "worst distance" back to a minimum value.  This should be
        /// called for each training iteration.
        /// </summary>
        ///
        public void Reset()
        {
            _worstDistance = Double.MinValue;
        }
    }
}
