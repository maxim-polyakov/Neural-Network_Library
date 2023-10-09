using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BAMPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of neurons in the first layer.
        /// </summary>
        ///
        private int _f1Neurons;

        /// <summary>
        /// The number of neurons in the second layer.
        /// </summary>
        ///
        private int _f2Neurons;

        /// <summary>
        /// Set the F1 neurons. The BAM really does not have an input and output
        /// layer, so this is simply setting the number of neurons that are in the
        /// first layer.
        /// </summary>
        ///
        /// <value>The number of neurons in the first layer.</value>
        public int F1Neurons
        {
            set { _f1Neurons = value; }
        }


        /// <summary>
        /// Set the output neurons. The BAM really does not have an input and output
        /// layer, so this is simply setting the number of neurons that are in the
        /// second layer.
        /// </summary>
        public int F2Neurons
        {
            set { _f2Neurons = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Unused, a BAM has no hidden layers.
        /// </summary>
        ///
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A BAM network has no hidden layers.");
        }

        /// <summary>
        /// Clear any settings on the pattern.
        /// </summary>
        public void Clear()
        {
            _f1Neurons = 0;
            _f2Neurons = 0;
        }


        /// <returns>The generated network.</returns>
        public IMLMethod Generate()
        {
            var bam = new BAMNetwork(_f1Neurons, _f2Neurons);
            return bam;
        }

        /// <summary>
        /// Not used, the BAM uses a bipoloar activation function.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A BAM network can't specify a custom activation function.");
            }
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            set
            {
                throw new PatternError(
                    "A BAM network has no input layer, consider setting F1 layer.");
            }
        }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        public int OutputNeurons
        {
            set
            {
                throw new PatternError(
                    "A BAM network has no output layer, consider setting F2 layer.");
            }
        }

        #endregion
    }
}
