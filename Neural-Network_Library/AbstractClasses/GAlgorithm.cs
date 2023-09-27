using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public abstract class GAlgorithm : IMultiThreadable
    {
        /// <summary>
        /// The score calculation object.
        /// </summary>
        ///
        private ICalculateTScore _calculateScore;

        /// <summary>
        /// Set the score calculation object.
        /// </summary>
        public ICalculateTScore CalculateScore
        {
            get { return _calculateScore; }
            set { _calculateScore = value; }
        }

        /// <summary>
        /// The thread count.
        /// </summary>
        public int ThreadCount { get; set; }


        /// <summary>
        /// Set the comparator.
        /// </summary>
        public TComparator Comparator { get; set; }


        /// <summary>
        /// Set the crossover object.
        /// </summary>
        public ICrossover Crossover { get; set; }


        /// <summary>
        /// Set the mating population percent.
        /// </summary>
        public double MatingPopulation { get; set; }


        /// <summary>
        /// Set the mutate object.
        /// </summary>
        public IMutate Mutate { get; set; }


        /// <summary>
        /// Set the mutation percent.
        /// </summary>
        public double MutationPercent { get; set; }


        /// <summary>
        /// Set the percent to mate.
        /// </summary>
        public double PercentToMate { get; set; }


        /// <summary>
        /// Set the population.
        /// </summary>
        public IPopulation Population { get; set; }

        /// <summary>
        /// Add a T.
        /// </summary>
        ///
        /// <param name="species">The species to add.</param>
        /// <param name="T">The T to add.</param>
        public void AddSpeciesMember(ISpecies species,
                                     IT T)
        {
            if (Comparator.IsBetterThan(T.Score,
                                        species.BestScore))
            {
                species.BestScore = T.Score;
                species.GensNoImprovement = 0;
                species.Leader = T;
            }

            species.Members.Add(T);
        }

        /// <summary>
        /// Calculate the score for this T. The T's score will be set.
        /// </summary>
        ///
        /// <param name="g">The T to calculate for.</param>
        public void PerformCalculateScore(IT g)
        {
            if (g.Organism is IMLContext)
            {
                ((IMLContext)g.Organism).ClearContext();
            }
            double score = _calculateScore.CalculateScore(g);
            g.Score = score;
        }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        ///
        public abstract void Iteration();
    }
}
