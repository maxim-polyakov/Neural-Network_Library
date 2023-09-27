using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IPopulation
    {
        /// <value>The best T in the population.</value>
        IT Best { get; }


        /// <value>The Ts in the population.</value>
        IList<IT> Ts { get; }


        /// <summary>
        /// Set the innovations collection.
        /// </summary>
        IInnovationList Innovations { get; set; }


        /// <summary>
        /// Set the old age penalty.
        /// </summary>
        double OldAgePenalty { get; set; }


        /// <summary>
        /// Set the age at which a T is considered "old".
        /// </summary>
        int OldAgeThreshold { get; set; }


        /// <summary>
        /// Set the max population size.
        /// </summary>
        int PopulationSize { get; set; }


        /// <value>A list of species.</value>
        IList<ISpecies> Species { get; }


        /// <summary>
        /// Set the survival rate.
        /// </summary>
        ///
        /// <value>The survival rate.</value>
        double SurvivalRate { get; set; }


        /// <value>The age, below which, a T is considered "young".</value>
        int YoungBonusAgeThreshold { get; }


        /// <summary>
        /// Set the youth score bonus.
        /// </summary>
        double YoungScoreBonus { get; set; }


        /// <summary>
        /// Set the age at which genoms are considered young.
        /// </summary>
        ///
        /// <value>The age.</value>
        int YoungBonusAgeThreshhold { set; }

        /// <summary>
        /// Add a T to the population.
        /// </summary>
        ///
        /// <param name="T">The T to add.</param>
        void Add(IT T);

        /// <returns>Assign a gene id.</returns>
        long AssignGeneID();


        /// <returns>Assign a T id.</returns>
        long AssignTID();


        /// <returns>Assign an innovation id.</returns>
        long AssignInnovationID();


        /// <returns>Assign a species id.</returns>
        long AssignSpeciesID();

        /// <summary>
        /// Clear all Ts from this population.
        /// </summary>
        ///
        void Clear();

        /// <summary>
        /// Get a T by index.  Index 0 is the best T.
        /// </summary>
        ///
        /// <param name="i">The T to get.</param>
        /// <returns>The T at the specified index.</returns>
        IT Get(int i);


        /// <returns>The size of the population.</returns>
        int Size();

        /// <summary>
        /// Sort the population by best score.
        /// </summary>
        ///
        void Sort();

        /// <summary>
        /// Claim the population, before training.
        /// </summary>
        ///
        /// <param name="ga">The GA that is claiming.</param>
        void Claim(GAlgorithm ga);
    }
}
