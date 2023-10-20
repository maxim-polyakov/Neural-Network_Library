using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SOMClusterCopyTraining : BasicTraining
    {
        /// <summary>
        /// The SOM to train.
        /// </summary>
        ///
        private readonly SOMNetwork _network;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        public SOMClusterCopyTraining(SOMNetwork network, IMLDataSet training)
            : base(TrainingImplementationType.OnePass)
        {
            _network = network;
            Training = training;
            if (_network.OutputCount < training.Count)
            {
                throw new NeuralNetworkError(
                        "To use cluster copy training you must have at least as many output neurons as training elements.");
            }
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }

        /// <summary>
        /// Copy the specified input pattern to the weight matrix. This causes an
        /// output neuron to learn this pattern "exactly". This is useful when a
        /// winner is to be forced.
        /// </summary>
        ///
        /// <param name="outputNeuron">The output neuron to set.</param>
        /// <param name="input">The input pattern to copy.</param>
        private void CopyInputPattern(int outputNeuron, IMLData input)
        {
            for (int inputNeuron = 0; inputNeuron < _network.InputCount; inputNeuron++)
            {
                _network.Weights[inputNeuron, outputNeuron] = input[inputNeuron];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            int outputNeuron = 0;

            foreach (IMLDataPair pair in Training)
            {
                CopyInputPattern(outputNeuron++, pair.Input);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}
