using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Collections;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace Neural_Network_Library
{    
    //___________
    [Serializable]
    public class BasicSpecies : ISpecies
    {
        /// <summary>
        /// The list of Ts.
        /// </summary>
        ///
        private readonly IList<IT> _members;

        /// <summary>
        /// The age of this species.
        /// </summary>
        ///
        private int _age;

        /// <summary>
        /// The best score.
        /// </summary>
        ///
        private double _bestScore;

        /// <summary>
        /// The number of generations with no improvement.
        /// </summary>
        ///
        private int _gensNoImprovement;

        /// <summary>
        /// The leader.
        /// </summary>
        ///
        private IT _leader;

        /// <summary>
        /// The id of the leader.
        /// </summary>
        [NonSerialized]
        private long _leaderID;

        /// <summary>
        /// The owner class.
        /// </summary>
        ///
        private IPopulation _population;

        /// <summary>
        /// The number of spawns required.
        /// </summary>
        ///
        private double _spawnsRequired;

        /// <summary>
        /// The species id.
        /// </summary>
        ///
        private long _speciesID;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public BasicSpecies()
        {
            _members = new List<IT>();
        }

        /// <summary>
        /// Construct a species.
        /// </summary>
        ///
        /// <param name="thePopulation">The population the species belongs to.</param>
        /// <param name="theFirst">The first T in the species.</param>
        /// <param name="theSpeciesID">The species id.</param>
        public BasicSpecies(IPopulation thePopulation, IT theFirst,
                            long theSpeciesID)
        {
            _members = new List<IT>();
            _population = thePopulation;
            _speciesID = theSpeciesID;
            _bestScore = theFirst.Score;
            _gensNoImprovement = 0;
            _age = 0;
            _leader = theFirst;
            _spawnsRequired = 0;
            _members.Add(theFirst);
        }

        /// <value>the population to set</value>
        public IPopulation Population
        {
            get { return _population; }
            set { _population = value; }
        }

        /// <summary>
        /// Set the leader id. This value is not persisted, it is used only for
        /// loading.
        /// </summary>
        ///
        /// <value>the leaderID to set</value>
        public long TempLeaderID
        {
            get { return _leaderID; }
            set { _leaderID = value; }
        }

        #region ISpecies Members

        /// <summary>
        /// Calculate the amount to spawn.
        /// </summary>
        ///
        public void CalculateSpawnAmount()
        {
            _spawnsRequired = 0;

            foreach (IT T in _members)
            {
                _spawnsRequired += T.AmountToSpawn;
            }
        }

        /// <summary>
        /// Choose a parent to mate. Choose from the population, determined by the
        /// survival rate. From this pool, a random parent is chosen.
        /// </summary>
        ///
        /// <returns>The parent.</returns>
        public IT ChooseParent()
        {
            IT baby;

            // If there is a single member, then choose that one.
            if (_members.Count == 1)
            {
                baby = _members[0];
            }
            else
            {
                // If there are many, then choose the population based on survival
                // rate
                // and select a random T.
                int maxIndexSize = (int)(_population.SurvivalRate * _members.Count) + 1;
                var theOne = (int)RangeRandomizer.Randomize(0, maxIndexSize);
                baby = _members[theOne];
            }

            return baby;
        }

        /// <summary>
        /// Set the age of this species.
        /// </summary>
        ///
        /// <value>The age of this species.</value>
        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }


        /// <summary>
        /// Set the best score.
        /// </summary>
        ///
        /// <value>The best score.</value>
        public double BestScore
        {
            get { return _bestScore; }
            set { _bestScore = value; }
        }


        /// <summary>
        /// Set the number of generations with no improvement.
        /// </summary>
        ///
        /// <value>The number of generations.</value>
        public int GensNoImprovement
        {
            get { return _gensNoImprovement; }
            set { _gensNoImprovement = value; }
        }


        /// <summary>
        /// Set the leader.
        /// </summary>
        ///
        /// <value>The new leader.</value>
        public IT Leader
        {
            get { return _leader; }
            set { _leader = value; }
        }


        /// <value>The members of this species.</value>
        public IList<IT> Members
        {
            get { return _members; }
        }


        /// <value>The number to spawn.</value>
        public double NumToSpawn
        {
            get { return _spawnsRequired; }
        }


        /// <summary>
        /// Set the number of spawns required.
        /// </summary>
        public double SpawnsRequired
        {
            get { return _spawnsRequired; }
            set { _spawnsRequired = value; }
        }


        /// <summary>
        /// Purge all members, increase age by one and count the number of
        /// generations with no improvement.
        /// </summary>
        ///
        public void Purge()
        {
            _members.Clear();
            _age++;
            _gensNoImprovement++;
            _spawnsRequired = 0;
        }

        /// <summary>
        /// Set the species id.
        /// </summary>
        public long SpeciesID
        {
            get { return _speciesID; }
            set { _speciesID = value; }
        }

        #endregion
    }

}