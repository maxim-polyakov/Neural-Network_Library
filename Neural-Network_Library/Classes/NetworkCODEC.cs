using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public static class NetworkCODEC
    {
        /// <summary>
        /// Error message.
        /// </summary>
        ///
        private const String Error = "This machine learning method cannot be Syntesisd:";

        /// <summary>
        /// Use an array to populate the memory of the neural network.
        /// </summary>
        ///
        /// <param name="array">An array of doubles.</param>
        /// <param name="network">The network to Syntesis.</param>
        public static void ArrayToNetwork(double[] array,
                                          IMLMethod network)
        {
            if (network is IMLEncodable)
            {
                ((IMLEncodable)network).DecodeFromArray(array);
                return;
            }

        }

        /// <summary>
        /// Determine if the two neural networks are equal. Uses exact precision
        /// required by Arrays.equals.
        /// </summary>
        ///
        /// <param name="network1">The first network.</param>
        /// <param name="network2">The second network.</param>
        /// <returns>True if the two networks are equal.</returns>
        public static bool Equals(BasicNetwork network1,
                                  BasicNetwork network2)
        {
            return Equals(network1, network2, SyntFramework.DefaultPrecision);
        }

        /// <summary>
        /// Determine if the two neural networks are equal.
        /// </summary>
        ///
        /// <param name="network1">The first network.</param>
        /// <param name="network2">The second network.</param>
        /// <param name="precision">How many decimal places to check.</param>
        /// <returns>True if the two networks are equal.</returns>
        public static bool Equals(BasicNetwork network1,
                                  BasicNetwork network2, int precision)
        {
            double[] array1 = NetworkToArray(network1);
            double[] array2 = NetworkToArray(network2);

            if (array1.Length != array2.Length)
            {
                return false;
            }

            double test = Math.Pow(10.0d, precision);
            if (Double.IsInfinity(test) || (test > Int64.MaxValue))
            {
            }

            for (int i = 0; i < array1.Length; i++)
            {
                var l1 = (long)(array1[i] * test);
                var l2 = (long)(array2[i] * test);
                if (l1 != l2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determine the network size.
        /// </summary>
        ///
        /// <param name="network">The network.</param>
        /// <returns>The size.</returns>
        public static int NetworkSize(IMLMethod network)
        {
            if (network is IMLEncodable)
            {
                return ((IMLEncodable)network).SyntesisdArrayLength();
            }
            else return -1;

        }

        /// <summary>
        /// Convert to an array. This is used with some training algorithms that
        /// require that the "memory" of the neuron(the weight and bias values) be
        /// expressed as a linear array.
        /// </summary>
        ///
        /// <param name="network">The network to Syntesis.</param>
        /// <returns>The memory of the neuron.</returns>
        public static double[] NetworkToArray(IMLMethod network)
        {
            int size = NetworkSize(network);

            if (network is IMLEncodable)
            {
                var Syntesisd = new double[size];
                ((IMLEncodable)network).SyntesisToArray(Syntesisd);
                return Syntesisd;
            }
            else return null;
        }
    }
}
