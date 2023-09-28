using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class TrainAdaline : BasicTraining, ILearningRate
    {
        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly BasicNetwork _network;

        /// <summary>
        /// The training data to use.
        /// </summary>
        ///
        private readonly IMLDataSet _training;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double _learningRate;

        /// <summary>
        /// Construct an ADALINE trainer.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        /// <param name="learningRate">The learning rate.</param>
        public TrainAdaline(BasicNetwork network, IMLDataSet training,
                            double learningRate) : base(TrainingImplementationType.Iterative)
        {
            if (network.LayerCount > 2)
            {
                throw new NeuralNetworkError(
                    "An ADALINE network only has two layers.");
            }
            _network = network;

            _training = training;
            _learningRate = learningRate;
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

        #region ILearningRate Members

        /// <summary>
        /// Set the learning rate.
        /// </summary>
        public double LearningRate
        {
            get { return _learningRate; }
            set { _learningRate = value; }
        }

        #endregion

        /// <inheritdoc/>
        public override sealed void Iteration()
        {
            var errorCalculation = new ErrorCalculation();


            foreach (IMLDataPair pair in _training)
            {
                // calculate the error
                IMLData output = _network.Compute(pair.Input);

                for (int currentAdaline = 0; currentAdaline < output.Count; currentAdaline++)
                {
                    double diff = pair.Ideal[currentAdaline]
                                  - output[currentAdaline];

                    // weights
                    for (int i = 0; i <= _network.InputCount; i++)
                    {
                        double input;

                        if (i == _network.InputCount)
                        {
                            input = 1.0d;
                        }
                        else
                        {
                            input = pair.Input[i];
                        }

                        _network.AddWeight(0, i, currentAdaline,
                                          _learningRate * diff * input);
                    }
                }

                errorCalculation.UpdateError(output.Data, pair.Ideal.Data, pair.Significance);
            }

            // set the global error
            Error = errorCalculation.Calculate();
        }

        /// <inheritdoc/>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}
