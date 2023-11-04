using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class Evaluate
    {
        /// <summary>
        /// Mili-seconds in a second.
        /// </summary>
        public const int Milis = 1000;

        /// <summary>
        /// Evaluate training.
        /// </summary>
        /// <param name="input">Input neurons.</param>
        /// <param name="hidden1">Hidden 1 neurons.</param>
        /// <param name="hidden2">Hidden 2 neurons.</param>
        /// <param name="output">Output neurons.</param>
        /// <returns>The result of the evaluation.</returns>
        public static int EvaluateTrain(int input, int hidden1, int hidden2,
                                        int output)
        {
            BasicNetwork network = SyntUtility.SimpleFeedForward(input,
                                                                  hidden1, hidden2, output, true);
            IMLDataSet training = RandomTrainingFactory.Generate(1000,
                                                                10000, input, output, -1, 1);

            return EvaluateTrain(network, training);
        }


        /// <summary>
        /// Evaluate how long it takes to calculate the error for the network. This
        /// causes each of the training pairs to be run through the network. The
        /// network is evaluated 10 times and the lowest time is reported. 
        /// </summary>
        /// <param name="network">The training data to use.</param>
        /// <param name="training">The number of seconds that it took.</param>
        /// <returns></returns>
        public static int EvaluateTrain(BasicNetwork network, IMLDataSet training)
        {
            // train the neural network
            IMLTrain train = new ResilientPropagation(network, training);

            int iterations = 0;
            var watch = new Stopwatch();
            watch.Start();
            while (watch.ElapsedMilliseconds < (10 * Milis))
            {
                iterations++;
                train.Iteration();
            }

            return iterations;
        }
    }
}
