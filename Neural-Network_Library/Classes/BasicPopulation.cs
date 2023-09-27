using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class BasicPopulation : IPopulation
    {
        /// <summary>
        /// Thed default old age penalty.
        /// </summary>
        ///
        public const double DefaultOldAgePenalty = 0.3d;

        /// <summary>
        /// The default old age threshold.
        /// </summary>
        ///
        public const int DefaultOldAgeThreshold = 50;

        /// <summary>
        /// The default survival rate.
        /// </summary>
        ///
        public const double DefaultSurvivalRate = 0.2d;

        /// <summary>
        /// The default youth penalty.
        /// </summary>
        ///
        public const double DefaultYouthBonus = 0.3d;

        /// <summary>
        /// The default youth threshold.
        /// </summary>
        ///
        public const int DefaultYouthThreshold = 10;

        /// <summary>
        /// Generate gene id's.
        /// </summary>
        ///
        private readonly IGenerateID _geneIDGenerate;

        /// <summary>
        /// Generate T id's.
        /// </summary>
        ///
        private readonly IGenerateID _TIDGenerate;

        /// <summary>
        /// The population.
        /// </summary>
        ///
        private readonly List<IT> _Ts;

        /// <summary>
        /// Generate innovation id's.
        /// </summary>
        ///
        private readonly IGenerateID _innovationIDGenerate;

        /// <summary>
        /// Generate species id's.
        /// </summary>
        ///
        private readonly IGenerateID _speciesIDGenerate;

        /// <summary>
        /// The young threshold.
        /// </summary>
        ///
        private int _youngBonusAgeThreshold;

        /// <summary>
        /// Construct an empty population.
        /// </summary>
        ///
        public BasicPopulation()
        {
            _geneIDGenerate = new BasicGenerateID();
            _TIDGenerate = new BasicGenerateID();
            _Ts = new List<IT>();
            _innovationIDGenerate = new BasicGenerateID();
            OldAgePenalty = DefaultOldAgePenalty;
            OldAgeThreshold = DefaultOldAgeThreshold;
            Species = new List<ISpecies>();
            _speciesIDGenerate = new BasicGenerateID();
            SurvivalRate = DefaultSurvivalRate;
            _youngBonusAgeThreshold = DefaultYouthThreshold;
            YoungScoreBonus = DefaultYouthBonus;
            PopulationSize = 0;
        }

        /// <summary>
        /// Construct a population.
        /// </summary>
        /// <param name="thePopulationSize">The population size.</param>
        public BasicPopulation(int thePopulationSize)
        {
            _geneIDGenerate = new BasicGenerateID();
            _TIDGenerate = new BasicGenerateID();
            _Ts = new List<IT>();
            _innovationIDGenerate = new BasicGenerateID();
            OldAgePenalty = DefaultOldAgePenalty;
            OldAgeThreshold = DefaultOldAgeThreshold;
            Species = new List<ISpecies>();
            _speciesIDGenerate = new BasicGenerateID();
            SurvivalRate = DefaultSurvivalRate;
            _youngBonusAgeThreshold = DefaultYouthThreshold;
            YoungScoreBonus = DefaultYouthBonus;
            PopulationSize = thePopulationSize;
        }

        /// <value>the geneIDGenerate</value>
        public IGenerateID GeneIDGenerate
        {
            get { return _geneIDGenerate; }
        }


        /// <value>the TIDGenerate</value>
        public IGenerateID TIDGenerate
        {
            get { return _TIDGenerate; }
        }

        /// <value>the innovationIDGenerate</value>
        public IGenerateID InnovationIDGenerate
        {
            get { return _innovationIDGenerate; }
        }

        /// <summary>
        /// Set the name.
        /// </summary>
        public String Name { get; set; }

        /// <value>the speciesIDGenerate</value>
        public IGenerateID SpeciesIDGenerate
        {
            get { return _speciesIDGenerate; }
        }

        #region IPopulation Members


        /// <inheritdoc/>
        public void Add(IT T)
        {
            _Ts.Add(T);
            T.Population = this;
        }

        /// <inheritdoc/>
        public long AssignGeneID()
        {
            return _geneIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public long AssignTID()
        {
            return _TIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public long AssignInnovationID()
        {
            return _innovationIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public long AssignSpeciesID()
        {
            return _speciesIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public void Claim(GAlgorithm ga)
        {
            foreach (IT T in _Ts)
            {
                T.GA = ga;
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _Ts.Clear();
        }

        /// <inheritdoc/>
        public IT Get(int i)
        {
            return _Ts[i];
        }

        /// <inheritdoc/>
        public IT Best
        {
            get { return _Ts.Count == 0 ? null : _Ts[0]; }
        }


        /// <inheritdoc/>
        public IList<IT> Ts
        {
            get { return _Ts; }
        }


        /// <inheritdoc/>
        public IInnovationList Innovations { get; set; }


        /// <inheritdoc/>
        public double OldAgePenalty { get; set; }


        /// <inheritdoc/>
        public int OldAgeThreshold { get; set; }


        /// <inheritdoc/>
        public int PopulationSize { get; set; }


        /// <inheritdoc/>
        public IList<ISpecies> Species { get; set; }


        /// <inheritdoc/>
        public double SurvivalRate { get; set; }


        /// <value>the youngBonusAgeThreshold to set</value>
        public int YoungBonusAgeThreshold
        {
            get { return _youngBonusAgeThreshold; }
            set { _youngBonusAgeThreshold = value; }
        }


        /// <inheritdoc/>
        public double YoungScoreBonus { get; set; }


        /// <inheritdoc/>
        public int YoungBonusAgeThreshhold
        {
            set { _youngBonusAgeThreshold = value; }
        }


        /// <inheritdoc/>
        public int Size()
        {
            return _Ts.Count;
        }

        /// <inheritdoc/>
        public void Sort()
        {
            _Ts.Sort();
        }

        #endregion
    }
}
