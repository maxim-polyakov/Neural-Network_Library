using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IT : IComparable<IT>
    {
        /// <summary>
        /// Set the adjusted score.
        /// </summary>
        double AdjustedScore { get; set; }


        /// <summary>
        /// Set the amount to spawn.
        /// </summary>
        double AmountToSpawn { get; set; }


        /// <value>The Qs that make up this T.</value>
        IList<Q> Qs
        {
            get;
        }


        /// <summary>
        /// Set the GA used by this T. This is normally a transient field and
        /// only used during training.
        /// </summary>
        GAlgorithm GA
        {
            get;
            set;
        }


        /// <summary>
        /// Set the T ID.
        /// </summary>
        long TID
        {
            get;
            set;
        }


        /// <value>The organism produced by this T.</value>
        Object Organism
        {
            get;
        }


        /// <summary>
        /// Set the population that this T belongs to.
        /// </summary>
        IPopulation Population
        {
            get;
            set;
        }


        /// <summary>
        /// Set the score.
        /// </summary>
        double Score
        {
            get;
            set;
        }

        /// <returns>The number of genes in this T.</returns>
        int CalculateGeneCount();

        /// <summary>
        /// Use the genes to update the organism.
        /// </summary>
        ///
        void Decode();

        /// <summary>
        /// Use the organism to update the genes.
        /// </summary>
        ///
        void Syntesis();


        /// <summary>
        /// Mate with another T and produce two children.
        /// </summary>
        ///
        /// <param name="father">The father T.</param>
        /// <param name="child1">The first child.</param>
        /// <param name="child2">The second child.</param>
        void Mate(IT father, IT child1, IT child2);
    }
}
