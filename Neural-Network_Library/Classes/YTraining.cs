using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class YTraining : GAlgorithm, IMLTrain
    {
        /// <summary>
        /// The number of inputs.
        /// </summary>
        private readonly int inputCount;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        private readonly int outputCount;

        /// <summary>
        /// The average fit adjustment.
        /// </summary>
        private double averageFitAdjustment;

        /// <summary>
        /// The best ever network.
        /// </summary>
        private YNetwork bestEverNetwork;

        /// <summary>
        /// The best ever score.
        /// </summary>
        private double bestEverScore;

        /// <summary>
        /// The iteration number.
        /// </summary>
        private int iteration;

        /// <summary>
        /// The activation mutation rate.
        /// </summary>
        private double paramActivationMutationRate = 0.1;

        /// <summary>
        /// The likelyhood of adding a link.
        /// </summary>
        private double paramChanceAddLink = 0.07;

        /// <summary>
        /// The likelyhood of adding a node.
        /// </summary>
        private double paramChanceAddNode = 0.04;

        /// <summary>
        /// The likelyhood of adding a recurrent link.
        /// </summary>
        private double paramChanceAddRecurrentLink = 0.05;

        /// <summary>
        /// The compatibility threshold for a species.
        /// </summary>
        private double paramCompatibilityThreshold = 0.26;

        /// <summary>
        /// The crossover rate.
        /// </summary>
        private double paramCrossoverRate = 0.7;

        /// <summary>
        /// The max activation perturbation.
        /// </summary>
        private double paramMaxActivationPerturbation = 0.1;

        /// <summary>
        /// The maximum number of species.
        /// </summary>
        private int paramMaxNumberOfSpecies;

        /// <summary>
        /// The maximum number of neurons.
        /// </summary>
        private double paramMaxPermittedNeurons = 100;

        /// <summary>
        /// The maximum weight perturbation.
        /// </summary>
        private double paramMaxWeightPerturbation = 0.5;

        /// <summary>
        /// The mutation rate.
        /// </summary>
        private double paramMutationRate = 0.2;

        /// <summary>
        /// The number of link add attempts.
        /// </summary>
        private int paramNumAddLinkAttempts = 5;

        /// <summary>
        /// The number of generations allowed with no improvement.
        /// </summary>
        private int paramNumGensAllowedNoImprovement = 15;

        /// <summary>
        /// The number of tries to find a looped link.
        /// </summary>
        private int paramNumTrysToFindLoopedLink = 5;

        /// <summary>
        /// The number of tries to find an old link.
        /// </summary>
        private int paramNumTrysToFindOldLink = 5;

        /// <summary>
        /// The probability that the weight will be totally replaced.
        /// </summary>
        private double paramProbabilityWeightReplaced = 0.1;

        /// <summary>
        /// Determines if we are using snapshot mode.
        /// </summary>
        private bool snapshot;

        /// <summary>
        /// The total fit adjustment.
        /// </summary>
        private double totalFitAdjustment;

        /// <summary>
        /// Construct a Y trainer with a new population. The new population is
        /// created from the specified parameters.
        /// </summary>
        /// <param name="calculateScore">The score calculation object.</param>
        /// <param name="inputCount">The input neuron count.</param>
        /// <param name="outputCount">The output neuron count.</param>
        /// <param name="populationSize">The population size.</param>
        public YTraining(ICalculateScore calculateScore,
                            int inputCount, int outputCount,
                            int populationSize)
        {
            this.inputCount = inputCount;
            this.outputCount = outputCount;

            CalculateScore = new GScoreAdapter(calculateScore);
            Comparator = new TComparator(CalculateScore);
            Population = new YPopulation(inputCount, outputCount,
                                            populationSize);

            Init();
        }

        /// <summary>
        /// Construct Y training with an existing population.
        /// </summary>
        /// <param name="calculateScore">The score object to use.</param>
        /// <param name="population">The population to use.</param>
        public YTraining(ICalculateScore calculateScore,
                            IPopulation population)
        {
            if (population.Size() < 1)
            {
                throw new TrainingError("Population can not be empty.");
            }

            var T = (YT)population.Ts[0];
            CalculateScore = new GScoreAdapter(calculateScore);
            Comparator = new TComparator(CalculateScore);
            Population = (population);
            inputCount = T.InputCount;
            outputCount = T.OutputCount;

            Init();
        }

        /// <summary>
        /// The innovations.
        /// </summary>
        public YInnovationList Innovations
        {
            get { return (YInnovationList)Population.Innovations; }
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get { return inputCount; }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public int OutputCount
        {
            get { return outputCount; }
        }

        /// <summary>
        /// Set the activation mutation rate.
        /// </summary>
        public double ParamActivationMutationRate
        {
            get { return paramActivationMutationRate; }
            set { paramActivationMutationRate = value; }
        }


        /// <summary>
        /// Set the chance to add a link.
        /// </summary>
        public double ParamChanceAddLink
        {
            get { return paramChanceAddLink; }
            set { paramChanceAddLink = value; }
        }


        /// <summary>
        /// Set the chance to add a node.
        /// </summary>
        public double ParamChanceAddNode
        {
            get { return paramChanceAddNode; }
            set { paramChanceAddNode = value; }
        }

        /// <summary>
        /// Set the chance to add a recurrent link.
        /// </summary>
        public double ParamChanceAddRecurrentLink
        {
            get { return paramChanceAddRecurrentLink; }
            set { paramChanceAddRecurrentLink = value; }
        }


        /// <summary>
        /// Set the compatibility threshold for species.
        /// </summary>
        public double ParamCompatibilityThreshold
        {
            get { return paramCompatibilityThreshold; }
            set { paramCompatibilityThreshold = value; }
        }


        /// <summary>
        /// Set the cross over rate.
        /// </summary>
        public double ParamCrossoverRate
        {
            get { return paramCrossoverRate; }
            set { paramCrossoverRate = value; }
        }


        /// <summary>
        /// Set the max activation perturbation.
        /// </summary>
        public double ParamMaxActivationPerturbation
        {
            get { return paramMaxActivationPerturbation; }
            set { paramMaxActivationPerturbation = value; }
        }

        /// <summary>
        /// Set the maximum number of species.
        /// </summary>
        public int ParamMaxNumberOfSpecies
        {
            get { return paramMaxNumberOfSpecies; }
            set { paramMaxNumberOfSpecies = value; }
        }

        /// <summary>
        /// Set the max permitted neurons.
        /// </summary>
        public double ParamMaxPermittedNeurons
        {
            get { return paramMaxPermittedNeurons; }
            set { paramMaxPermittedNeurons = value; }
        }

        /// <summary>
        /// Set the max weight perturbation.
        /// </summary>
        public double ParamMaxWeightPerturbation
        {
            get { return paramMaxWeightPerturbation; }
            set { paramMaxWeightPerturbation = value; }
        }

        /// <summary>
        /// Set the mutation rate.
        /// </summary>
        public double ParamMutationRate
        {
            get { return paramMutationRate; }
            set { paramMutationRate = value; }
        }

        /// <summary>
        /// Set the number of attempts to add a link.
        /// </summary>
        public int ParamNumAddLinkAttempts
        {
            get { return paramNumAddLinkAttempts; }
            set { paramNumAddLinkAttempts = value; }
        }

        /// <summary>
        /// Set the number of no-improvement generations allowed.
        /// </summary>
        public int ParamNumGensAllowedNoImprovement
        {
            get { return paramNumGensAllowedNoImprovement; }
            set { paramNumGensAllowedNoImprovement = value; }
        }

        /// <summary>
        /// Set the number of tries to create a looped link.
        /// </summary>
        public int ParamNumTrysToFindLoopedLink
        {
            get { return paramNumTrysToFindLoopedLink; }
            set { paramNumTrysToFindLoopedLink = value; }
        }


        /// <summary>
        /// Set the number of tries to try an old link.
        /// </summary>
        public int ParamNumTrysToFindOldLink
        {
            get { return paramNumTrysToFindOldLink; }
            set { paramNumTrysToFindOldLink = value; }
        }


        /// <summary>
        /// Set the probability to replace a weight.
        /// </summary>
        public double ParamProbabilityWeightReplaced
        {
            get { return paramProbabilityWeightReplaced; }
            set { paramProbabilityWeightReplaced = value; }
        }

        /// <summary>
        /// Set if we are using snapshot mode.
        /// </summary>
        public bool Snapshot
        {
            get { return snapshot; }
            set { snapshot = value; }
        }

        #region MLTrain Members

        /// <inheritdoc/>
        public void AddStrategy(IStrategy strategy)
        {
            throw new TrainingError(
                "Strategies are not supported by this training method.");
        }

        /// <inheritdoc/>
        public bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public void FinishTraining()
        {
        }

        /// <summary>
        /// The error for the best T.
        /// </summary>
        public double Error
        {
            get { return bestEverScore; }
            set { bestEverScore = value; }
        }

        /// <inheritdoc/>
        public TrainingImplementationType ImplementationType
        {
            get { return TrainingImplementationType.Iterative; }
        }

        /// <inheritdoc/>
        public int IterationNumber
        {
            get { return iteration; }
            set { iteration = value; }
        }

        /// <summary>
        /// A network created for the best T.
        /// </summary>
        public IMLMethod Method
        {
            get { return bestEverNetwork; }
        }

        /// <inheritdoc/>
        public IList<IStrategy> Strategies
        {
            get { return new List<IStrategy>(); }
        }

        /// <summary>
        /// Returns null, does not use a training set, rather uses a score function.
        /// </summary>
        public IMLDataSet Training
        {
            get { return null; }
        }

        /// <inheritdoc/>
        public bool TrainingDone
        {
            get { return false; }
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {
            iteration++;
            IList<YT> newPop = new List<YT>();

            int numSpawnedSoFar = 0;

            foreach (ISpecies s in Population.Species)
            {
                if (numSpawnedSoFar < Population.Size())
                {
                    var numToSpawn = (int)Math.Round(s.NumToSpawn);

                    bool bChosenBestYet = false;

                    while ((numToSpawn--) > 0)
                    {
                        YT baby = null;

                        if (!bChosenBestYet)
                        {
                            baby = (YT)s.Leader;

                            bChosenBestYet = true;
                        }

                        else
                        {
                            // if the number of individuals in this species is only
                            // one
                            // then we can only perform mutation
                            if (s.Members.Count == 1)
                            {
                                // spawn a child
                                baby = new YT((YT)s.ChooseParent());
                            }
                            else
                            {
                                var g1 = (YT)s.ChooseParent();

                                if (ThreadSafeRandom.NextDouble() < paramCrossoverRate)
                                {
                                    var g2 = (YT)s.ChooseParent();

                                    int numAttempts = 5;

                                    while ((g1.TID == g2.TID)
                                           && ((numAttempts--) > 0))
                                    {
                                        g2 = (YT)s.ChooseParent();
                                    }

                                    if (g1.TID != g2.TID)
                                    {
                                        baby = Crossover(g1, g2);
                                    }
                                }

                                else
                                {
                                    baby = new YT(g1);
                                }
                            }

                            if (baby != null)
                            {
                                baby.TID = Population.AssignTID();

                                if (baby.Neurons.Size() < paramMaxPermittedNeurons)
                                {
                                    baby.AddNeuron(paramChanceAddNode,
                                                   paramNumTrysToFindOldLink);
                                }

                                // now there's the chance a link may be added
                                baby.AddLink(paramChanceAddLink,
                                             paramChanceAddRecurrentLink,
                                             paramNumTrysToFindLoopedLink,
                                             paramNumAddLinkAttempts);

                                // mutate the weights
                                baby.MutateWeights(paramMutationRate,
                                                   paramProbabilityWeightReplaced,
                                                   paramMaxWeightPerturbation);

                                baby.MutateActivationResponse(
                                    paramActivationMutationRate,
                                    paramMaxActivationPerturbation);
                            }
                        }

                        if (baby != null)
                        {
                            // sort the baby's genes by their innovation numbers
                            baby.SortGenes();

                            // add to new pop
                            // if (newPop.contains(baby)) {
                            // throw new SyntError("readd");
                            // }
                            newPop.Add(baby);

                            ++numSpawnedSoFar;

                            if (numSpawnedSoFar == Population.Size())
                            {
                                numToSpawn = 0;
                            }
                        }
                    }
                }
            }

            while (newPop.Count < Population.Size())
            {
                newPop.Add(TournamentSelection(Population.Size() / 5));
            }

            Population.Clear();
            foreach (YT T in newPop)
            {
                Population.Add(T);
            }

            ResetAndKill();
            SortAndRecord();
            SpeciateAndCalculateSpawnLevels();
        }

        /// <inheritdoc/>
        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        /// <inheritdoc/>
        public TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public void Resume(TrainingContinuation state)
        {
        }

        #endregion

        /// <summary>
        /// Add the specified neuron id.
        /// </summary>
        /// <param name="nodeID">The neuron to add.</param>
        /// <param name="vec">The list to add to.</param>
        public void AddNeuronID(long nodeID, IList<long> vec)
        {
            for (int i = 0; i < vec.Count; i++)
            {
                if (vec[i] == nodeID)
                {
                    return;
                }
            }

            vec.Add(nodeID);

            return;
        }

        /// <summary>
        /// Adjust the compatibility threshold.
        /// </summary>
        public void AdjustCompatibilityThreshold()
        {
            // has this been disabled (unlimited species)
            if (paramMaxNumberOfSpecies < 1)
            {
                return;
            }

            double thresholdIncrement = 0.01;

            if (Population.Species.Count > paramMaxNumberOfSpecies)
            {
                paramCompatibilityThreshold += thresholdIncrement;
            }

            else if (Population.Species.Count < 2)
            {
                paramCompatibilityThreshold -= thresholdIncrement;
            }
        }

        /// <summary>
        /// Adjust each species score.
        /// </summary>
        public void AdjustSpeciesScore()
        {
            foreach (ISpecies s in Population.Species)
            {
                // loop over all Ts and adjust scores as needed
                foreach (IT member in s.Members)
                {
                    double score = member.Score;

                    // apply a youth bonus
                    if (s.Age < Population.YoungBonusAgeThreshold)
                    {
                        score = Comparator.ApplyBonus(score,
                                                      Population.YoungScoreBonus);
                    }

                    // apply an old age penalty
                    if (s.Age > Population.OldAgeThreshold)
                    {
                        score = Comparator.ApplyPenalty(score,
                                                        Population.OldAgePenalty);
                    }

                    double adjustedScore = score / s.Members.Count;

                    member.AdjustedScore = adjustedScore;
                }
            }
        }

        /// <summary>
        /// Perform a cross over.  
        /// </summary>
        /// <param name="mom">The mother T.</param>
        /// <param name="dad">The father T.</param>
        /// <returns></returns>
        public new YT Crossover(YT mom, YT dad)
        {
            YParent best;

            // first determine who is more fit, the mother or the father?
            if (mom.Score == dad.Score)
            {
                if (mom.NumGenes == dad.NumGenes)
                {
                    if (ThreadSafeRandom.NextDouble() > 0)
                    {
                        best = YParent.Mom;
                    }
                    else
                    {
                        best = YParent.Dad;
                    }
                }

                else
                {
                    if (mom.NumGenes < dad.NumGenes)
                    {
                        best = YParent.Mom;
                    }
                    else
                    {
                        best = YParent.Dad;
                    }
                }
            }
            else
            {
                if (Comparator.IsBetterThan(mom.Score, dad.Score))
                {
                    best = YParent.Mom;
                }

                else
                {
                    best = YParent.Dad;
                }
            }

            var babyNeurons = new Q();
            var babyGenes = new Q();

            var vecNeurons = new List<long>();

            int curMom = 0;
            int curDad = 0;

            YLinkGene momGene;
            YLinkGene dadGene;

            YLinkGene selectedGene = null;

            while ((curMom < mom.NumGenes) || (curDad < dad.NumGenes))
            {
                if (curMom < mom.NumGenes)
                {
                    momGene = (YLinkGene)mom.Links.Get(curMom);
                }
                else
                {
                    momGene = null;
                }

                if (curDad < dad.NumGenes)
                {
                    dadGene = (YLinkGene)dad.Links.Get(curDad);
                }
                else
                {
                    dadGene = null;
                }

                if ((momGene == null) && (dadGene != null))
                {
                    if (best == YParent.Dad)
                    {
                        selectedGene = dadGene;
                    }
                    curDad++;
                }
                else if ((dadGene == null) && (momGene != null))
                {
                    if (best == YParent.Mom)
                    {
                        selectedGene = momGene;
                    }
                    curMom++;
                }
                else if (momGene.InnovationId < dadGene.InnovationId)
                {
                    if (best == YParent.Mom)
                    {
                        selectedGene = momGene;
                    }
                    curMom++;
                }
                else if (dadGene.InnovationId < momGene.InnovationId)
                {
                    if (best == YParent.Dad)
                    {
                        selectedGene = dadGene;
                    }
                    curDad++;
                }
                else if (dadGene.InnovationId == momGene.InnovationId)
                {
                    if (ThreadSafeRandom.NextDouble() < 0.5f)
                    {
                        selectedGene = momGene;
                    }

                    else
                    {
                        selectedGene = dadGene;
                    }
                    curMom++;
                    curDad++;
                }

                if (babyGenes.Size() == 0)
                {
                    babyGenes.Add(selectedGene);
                }

                else
                {
                    if (((YLinkGene)babyGenes.Get(babyGenes.Size() - 1))
                            .InnovationId != selectedGene.InnovationId)
                    {
                        babyGenes.Add(selectedGene);
                    }
                }

                // Check if we already have the nodes referred to in SelectedGene.
                // If not, they need to be added.
                AddNeuronID(selectedGene.FromNeuronID, vecNeurons);
                AddNeuronID(selectedGene.ToNeuronID, vecNeurons);
            } // end while

            // now create the required nodes. First sort them into order
            vecNeurons.Sort();

            for (int i = 0; i < vecNeurons.Count; i++)
            {
                babyNeurons.Add(Innovations.CreateNeuronFromID(
                    vecNeurons[i]));
            }

            // finally, create the T
            var babyT = new YT(Population
                                                .AssignTID(), babyNeurons, babyGenes, mom.InputCount,
                                            mom.OutputCount);
            babyT.GA = this;
            babyT.Population = Population;

            return babyT;
        }

        /// <summary>
        /// Init the training.
        /// </summary>
        private void Init()
        {
            if (CalculateScore.ShouldMinimize)
            {
                bestEverScore = Double.MaxValue;
            }
            else
            {
                bestEverScore = Double.MinValue;
            }

            // check the population
            foreach (IT obj in Population.Ts)
            {
                if (!(obj is YT))
                {
                    throw new TrainingError(
                        "Population can only contain objects of YT.");
                }

                var Y = (YT)obj;

                if ((Y.InputCount != inputCount)
                    || (Y.OutputCount != outputCount))
                {
                    throw new TrainingError(
                        "All YT's must have the same input and output sizes as the base network.");
                }
                Y.GA = this;
            }

            Population.Claim(this);

            ResetAndKill();
            SortAndRecord();
            SpeciateAndCalculateSpawnLevels();
        }

        /// <summary>
        /// Reset counts and kill Ts with worse scores.
        /// </summary>
        public void ResetAndKill()
        {
            totalFitAdjustment = 0;
            averageFitAdjustment = 0;

            var speciesArray = new ISpecies[Population.Species.Count];

            for (int i = 0; i < Population.Species.Count; i++)
            {
                speciesArray[i] = Population.Species[i];
            }

            foreach (Object element in speciesArray)
            {
                var s = (ISpecies)element;
                s.Purge();

                if ((s.GensNoImprovement > paramNumGensAllowedNoImprovement)
                    && Comparator.IsBetterThan(bestEverScore,
                                               s.BestScore))
                {
                    Population.Species.Remove(s);
                }
            }
        }

        /// <summary>
        /// Sort the Ts.
        /// </summary>
        public void SortAndRecord()
        {
            foreach (IT g in Population.Ts)
            {
                g.Decode();
                PerformCalculateScore(g);
            }

            Population.Sort();

            IT T = Population.Best;
            double currentBest = T.Score;

            if (Comparator.IsBetterThan(currentBest, bestEverScore))
            {
                bestEverScore = currentBest;
                bestEverNetwork = ((YNetwork)T.Organism);
            }

            bestEverScore = Comparator.BestScore(Error,
                                                 bestEverScore);
        }

        /// <summary>
        /// Determine the species.
        /// </summary>
        public void SpeciateAndCalculateSpawnLevels()
        {
            // calculate compatibility between Ts and species
            AdjustCompatibilityThreshold();

            // assign Ts to species (if any exist)
            foreach (IT g in Population.Ts)
            {
                var T = (YT)g;
                bool added = false;

                foreach (ISpecies s in Population.Species)
                {
                    double compatibility = T.GetCompatibilityScore((YT)s.Leader);

                    if (compatibility <= paramCompatibilityThreshold)
                    {
                        AddSpeciesMember(s, T);
                        T.SpeciesID = s.SpeciesID;
                        added = true;
                        break;
                    }
                }

                // if this T did not fall into any existing species, create a
                // new species
                if (!added)
                {
                    Population.Species.Add(
                        new BasicSpecies(Population, T,
                                         Population.AssignSpeciesID()));
                }
            }

            AdjustSpeciesScore();

            foreach (IT g in Population.Ts)
            {
                var T = (YT)g;
                totalFitAdjustment += T.AdjustedScore;
            }

            averageFitAdjustment = totalFitAdjustment
                                   / Population.Size();

            foreach (IT g in Population.Ts)
            {
                var T = (YT)g;
                double toSpawn = T.AdjustedScore
                                 / averageFitAdjustment;
                T.AmountToSpawn = toSpawn;
            }

            foreach (ISpecies species in Population.Species)
            {
                species.CalculateSpawnAmount();
            }
        }

        /// <summary>
        /// Select a gene using a tournament.
        /// </summary>
        /// <param name="numComparisons">The number of compares to do.</param>
        /// <returns>The chosen T.</returns>
        public YT TournamentSelection(int numComparisons)
        {
            double bestScoreSoFar = 0;

            int chosenOne = 0;

            for (int i = 0; i < numComparisons; ++i)
            {
                var thisTry = (int)RangeRandomizer.Randomize(0, Population.Size() - 1);

                if (Population.Get(thisTry).Score > bestScoreSoFar)
                {
                    chosenOne = thisTry;

                    bestScoreSoFar = Population.Get(thisTry).Score;
                }
            }

            return (YT)Population.Get(chosenOne);
        }
    }
}
