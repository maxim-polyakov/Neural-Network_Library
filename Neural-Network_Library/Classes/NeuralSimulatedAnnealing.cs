using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NeuralSimulatedAnnealing : BasicTraining
    {
        /// <summary>
        /// The cutoff for random data.
        /// </summary>
        ///
        public const double Cut = 0.5d;

        /// <summary>
        /// This class actually performs the training.
        /// </summary>
        ///
        private readonly NeuralSimulatedAnnealingHelper _anneal;

        /// <summary>
        /// Used to calculate the score.
        /// </summary>
        ///
        private readonly ICalculateScore _calculateScore;

        /// <summary>
        /// The neural network that is to be trained.
        /// </summary>
        ///
        private readonly BasicNetwork _network;

        /// <summary>
        /// Construct a simulated annleaing trainer for a feedforward neural network.
        /// </summary>
        ///
        /// <param name="network">The neural network to be trained.</param>
        /// <param name="calculateScore">Used to calculate the score for a neural network.</param>
        /// <param name="startTemp">The starting temperature.</param>
        /// <param name="stopTemp">The ending temperature.</param>
        /// <param name="cycles">The number of cycles in a training iteration.</param>
        public NeuralSimulatedAnnealing(BasicNetwork network,
                                        ICalculateScore calculateScore, double startTemp,
                                        double stopTemp, int cycles) : base(TrainingImplementationType.Iterative)
        {
            _network = network;
            _calculateScore = calculateScore;
            _anneal = new NeuralSimulatedAnnealingHelper(this)
            {
                Temperature = startTemp,
                StartTemperature = startTemp,
                StopTemperature = stopTemp,
                Cycles = cycles
            };
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Get the network as an array of doubles.
        /// </summary>
        public double[] Array
        {
            get
            {
                return NetworkCODEC
                    .NetworkToArray(_network);
            }
        }


        /// <value>A copy of the annealing array.</value>
        public double[] ArrayCopy
        {
            get { return Array; }
        }


        /// <value>The object used to calculate the score.</value>
        public ICalculateScore CalculateScore
        {
            get { return _calculateScore; }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }


        /// <summary>
        /// Perform one iteration of simulated annealing.
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            SyntLogging.Log(SyntLogging.LevelInfo,
                             "Performing Simulated Annealing iteration.");
            PreIteration();
            _anneal.Iteration();
            Error = _anneal.PerformCalculateScore();
            PostIteration();
        }

        /// <inheritdoc/>
        public override TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// Convert an array of doubles to the current best network.
        /// </summary>
        ///
        /// <param name="array">An array.</param>
        public void PutArray(double[] array)
        {
            NetworkCODEC.ArrayToNetwork(array,
                                        _network);
        }

        /// <summary>
        /// Randomize the weights and bias values. This function does most of the
        /// work of the class. Each call to this class will randomize the data
        /// according to the current temperature. The higher the temperature the more
        /// randomness.
        /// </summary>
        ///
        public void Randomize()
        {
            double[] array = NetworkCODEC
                .NetworkToArray(_network);

            for (int i = 0; i < array.Length; i++)
            {
                double add = Cut - ThreadSafeRandom.NextDouble();
                add /= _anneal.StartTemperature;
                add *= _anneal.Temperature;
                array[i] = array[i] + add;
            }

            NetworkCODEC.ArrayToNetwork(array,
                                        _network);
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}
