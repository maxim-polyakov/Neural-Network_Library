using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NeuralGAlgorithm : BasicTraining, IMultiThreadable
    {
        /// <summary>
        /// Construct a neural G algorithm.
        /// </summary>
        ///
        /// <param name="network">The network to base this on.</param>
        /// <param name="randomizer">The randomizer used to create this initial population.</param>
        /// <param name="calculateScore">The score calculation object.</param>
        /// <param name="populationSize">The population size.</param>
        /// <param name="mutationPercent">The percent of offspring to mutate.</param>
        /// <param name="percentToMate">The percent of the population allowed to mate.</param>
        public NeuralGAlgorithm(BasicNetwork network,
                                      IRandomizer randomizer, ICalculateScore calculateScore,
                                      int populationSize, double mutationPercent,
                                      double percentToMate) : base(TrainingImplementationType.Iterative)
        {
            G = new NeuralGAlgorithmHelper
            {
                CalculateScore = new GScoreAdapter(calculateScore)
            };
            IPopulation population = new BasicPopulation(populationSize);
            G.MutationPercent = mutationPercent;
            G.MatingPopulation = percentToMate * 2;
            G.PercentToMate = percentToMate;
            G.Crossover = new Splice(network.Structure.CalculateSize() / 3);
            G.Mutate = new MutatePerturb(4.0d);
            G.Population = population;
            for (int i = 0; i < population.PopulationSize; i++)
            {
                var QNetwork = (BasicNetwork)(network
                                                           .Clone());
                randomizer.Randomize(QNetwork);

                var T = new NeuralT(QNetwork) { GA = G };
                G.PerformCalculateScore(T);
                G.Population.Add(T);
            }
            population.Sort();
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Set the G helper class.
        /// </summary>
        public NeuralGAlgorithmHelper G { get; set; }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return G.Method; }
        }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            SyntLogging.Log(SyntLogging.LevelInfo,
                             "Performing G iteration.");
            PreIteration();
            G.Iteration();
            Error = G.Error;
            PostIteration();
        }

        /// <inheritdoc/>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override sealed void Resume(TrainingContinuation state)
        {
        }



        #region Nested type: NeuralGAlgorithmHelper

        /// <summary>
        /// Very simple class that implements a G algorithm.
        /// </summary>
        ///
        public class NeuralGAlgorithmHelper : BasicGAlgorithm
        {
            /// <value>The error from the last iteration.</value>
            public double Error
            {
                get
                {
                    IT T = Population.Best;
                    return T.Score;
                }
            }


            /// <summary>
            /// Get the current best neural network.
            /// </summary>
            public IMLMethod Method
            {
                get
                {
                    IT T = Population.Best;
                    return (BasicNetwork)T.Organism;
                }
            }
        }

        #endregion

        /// <inheritdoc/>
        public int ThreadCount
        {
            get
            {
                return this.G.ThreadCount;
            }
            set
            {
                this.G.ThreadCount = value;
            }
        }
    }
}
