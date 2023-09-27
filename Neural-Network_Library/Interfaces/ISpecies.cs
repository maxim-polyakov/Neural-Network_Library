using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface ISpecies
    {
        /// <summary>
        /// Set the age of this species.
        /// </summary>
        int Age { get; set; }


        /// <summary>
        /// Set the best score.
        /// </summary>
        double BestScore { get; set; }


        /// <summary>
        /// Set the number of generations with no improvement.
        /// </summary>
        int GensNoImprovement { get; set; }


        /// <summary>
        /// Set the leader of this species.
        /// </summary>
        IT Leader { get; set; }


        /// <value>The numbers of this species.</value>
        IList<IT> Members { get; }


        /// <value>The number of Ts this species will try to spawn into the
        /// next generation.</value>
        double NumToSpawn { get; }


        /// <summary>
        /// Set the number of spawns required.
        /// </summary>
        double SpawnsRequired { get; set; }


        /// <value>The species ID.</value>
        long SpeciesID { get; }

        /// <summary>
        /// Calculate the amount that a species will spawn.
        /// </summary>
        ///
        void CalculateSpawnAmount();

        /// <summary>
        /// Choose a worthy parent for mating.
        /// </summary>
        ///
        /// <returns>The parent T.</returns>
        IT ChooseParent();


        /// <summary>
        /// Purge old unsuccessful Ts.
        /// </summary>
        ///
        void Purge();
    }
}
