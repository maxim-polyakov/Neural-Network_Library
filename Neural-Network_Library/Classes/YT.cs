using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class YT : BasicT, ICloneable
    {
        /// <summary>
        /// The neurons property.
        /// </summary>
        public const String PROPERTY_NEURONS = "neurons";

        /// <summary>
        /// The links property.
        /// </summary>
        public const String PROPERTY_LINKS = "links";

        /// <summary>
        /// The adjustment factor for disjoint genes.
        /// </summary>
        ///
        public const double TWEAK_DISJOINT = 1;

        /// <summary>
        /// The adjustment factor for excess genes.
        /// </summary>
        ///
        public const double TWEAK_EXCESS = 1;

        /// <summary>
        /// The adjustment factor for matched genes.
        /// </summary>
        ///
        public const double TWEAK_MATCHED = 0.4d;

        /// <summary>
        /// The number of inputs.
        /// </summary>
        ///
        private int inputCount;

        /// <summary>
        /// The chromsome that holds the links.
        /// </summary>
        ///
        private Q linksQ;

        /// <summary>
        /// THe network depth.
        /// </summary>
        ///
        private int networkDepth;

        /// <summary>
        /// The Q that holds the neurons.
        /// </summary>
        ///
        private Q neuronsQ;

        /// <summary>
        /// The number of outputs.
        /// </summary>
        ///
        private int outputCount;

        /// <summary>
        /// The species id.
        /// </summary>
        ///
        private long speciesID;

        /// <summary>
        /// Construct a T by copying another.
        /// </summary>
        ///
        /// <param name="other">The other T.</param>
        public YT(YT other)
        {
            neuronsQ = new Q();
            linksQ = new Q();
            GA = other.GA;

            Qs.Add(neuronsQ);
            Qs.Add(linksQ);

            TID = other.TID;
            networkDepth = other.networkDepth;
            Population = other.Population;
            Score = other.Score;
            AdjustedScore = other.AdjustedScore;
            AmountToSpawn = other.AmountToSpawn;
            inputCount = other.inputCount;
            outputCount = other.outputCount;
            speciesID = other.speciesID;


            // copy neurons
            foreach (IGene gene in other.Neurons.Genes)
            {
                var oldGene = (YNeuronGene)gene;
                var newGene = new YNeuronGene(
                    oldGene.NeuronType, oldGene.Id,
                    oldGene.SplitY, oldGene.SplitX,
                    oldGene.Recurrent, oldGene.ActivationResponse);
                Neurons.Add(newGene);
            }


            // copy links
            foreach (IGene gene_0 in other.Links.Genes)
            {
                var oldGene_1 = (YLinkGene)gene_0;
                var newGene_2 = new YLinkGene(
                    oldGene_1.FromNeuronID, oldGene_1.ToNeuronID,
                    oldGene_1.Enabled, oldGene_1.InnovationId,
                    oldGene_1.Weight, oldGene_1.Recurrent);
                Links.Add(newGene_2);
            }
        }

        /// <summary>
        /// Create a Y gnome.
        /// </summary>
        ///
        /// <param name="TID">The T id.</param>
        /// <param name="neurons">The neurons.</param>
        /// <param name="links">The links.</param>
        /// <param name="inputCount_0">The input count.</param>
        /// <param name="outputCount_1">The output count.</param>
        public YT(long TID, Q neurons,
                          Q links, int inputCount_0, int outputCount_1)
        {
            TID = TID;
            linksQ = links;
            neuronsQ = neurons;
            AmountToSpawn = 0;
            AdjustedScore = 0;
            inputCount = inputCount_0;
            outputCount = outputCount_1;

            Qs.Add(neuronsQ);
            Qs.Add(linksQ);
        }

        /// <summary>
        /// Construct a T, do not provide links and neurons.
        /// </summary>
        ///
        /// <param name="id">The T id.</param>
        /// <param name="inputCount_0">The input count.</param>
        /// <param name="outputCount_1">The output count.</param>
        public YT(long id, int inputCount_0, int outputCount_1)
        {
            TID = id;
            AdjustedScore = 0;
            inputCount = inputCount_0;
            outputCount = outputCount_1;
            AmountToSpawn = 0;
            speciesID = 0;

            double inputRowSlice = 0.8d / (inputCount_0);
            neuronsQ = new Q();
            linksQ = new Q();

            Qs.Add(neuronsQ);
            Qs.Add(linksQ);

            for (int i = 0; i < inputCount_0; i++)
            {
                neuronsQ.Add(new YNeuronGene(YNeuronType.Input,
                                                         i, 0, 0.1d + i * inputRowSlice));
            }

            neuronsQ.Add(new YNeuronGene(YNeuronType.Bias,
                                                     inputCount_0, 0, 0.9d));

            double outputRowSlice = 1 / (double)(outputCount_1 + 1);

            for (int i_2 = 0; i_2 < outputCount_1; i_2++)
            {
                neuronsQ.Add(new YNeuronGene(
                                          YNeuronType.Output, i_2 + inputCount_0 + 1, 1, (i_2 + 1)
                                                                                            * outputRowSlice));
            }

            for (int i_3 = 0; i_3 < inputCount_0 + 1; i_3++)
            {
                for (int j = 0; j < outputCount_1; j++)
                {
                    linksQ.Add(new YLinkGene(
                                            ((YNeuronGene)neuronsQ.Get(i_3)).Id,
                                            ((YNeuronGene)Neurons.Get(
                                                inputCount_0 + j + 1)).Id, true, inputCount_0
                                                                                 + outputCount_1 + 1 + NumGenes,
                                            RangeRandomizer.Randomize(-1, 1), false));
                }
            }
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        public YT()
        {
        }

        /// <value>the inputCount to set</value>
        public int InputCount
        {
            get { return inputCount; }
            set { inputCount = value; }
        }


        /// <value>THe links Q.</value>
        public Q Links
        {
            get { return linksQ; }
        }


        /// <value>the networkDepth to set</value>
        public int NetworkDepth
        {
            get { return networkDepth; }
            set { networkDepth = value; }
        }


        /// <value>The neurons Q.</value>
        public Q Neurons
        {
            get { return neuronsQ; }
        }


        /// <value>The number of genes in the links Q.</value>
        public int NumGenes
        {
            get { return linksQ.Size(); }
        }


        /// <value>the outputCount to set</value>
        public int OutputCount
        {
            get { return outputCount; }
            set { outputCount = value; }
        }


        /// <summary>
        /// Set the species id.
        /// </summary>
        ///
        /// <value>The species id.</value>
        public long SpeciesID
        {
            get { return speciesID; }
            set { speciesID = value; }
        }

        /// <value>the linksQ to set</value>
        public Q LinksQ
        {
            get { return linksQ; }
            set { linksQ = value; }
        }


        /// <value>the neuronsQ to set</value>
        public Q NeuronsQ
        {
            get { return neuronsQ; }
            set { neuronsQ = value; }
        }

        #region ICloneable Members

        /// <summary>
        /// Clone the object. Not currently supported.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Mutate the T by adding a link to this T.
        /// </summary>
        ///
        /// <param name="mutationRate">The mutation rate.</param>
        /// <param name="chanceOfLooped">The chance of a self-connected neuron.</param>
        /// <param name="numTrysToFindLoop">The number of tries to find a loop.</param>
        /// <param name="numTrysToAddLink">The number of tries to add a link.</param>
        internal void AddLink(double mutationRate, double chanceOfLooped,
                              int numTrysToFindLoop, int numTrysToAddLink)
        {
            // should we even add the link
            if (ThreadSafeRandom.NextDouble() > mutationRate)
            {
                return;
            }

            int countTrysToFindLoop = numTrysToFindLoop;
            int countTrysToAddLink = numTrysToFindLoop;

            // the link will be between these two neurons
            long neuron1ID = -1;
            long neuron2ID = -1;

            bool recurrent = false;

            // a self-connected loop?
            if (ThreadSafeRandom.NextDouble() < chanceOfLooped)
            {
                // try to find(randomly) a neuron to add a self-connected link to
                while ((countTrysToFindLoop--) > 0)
                {
                    YNeuronGene neuronGene = ChooseRandomNeuron(false);

                    // no self-links on input or bias neurons
                    if (!neuronGene.Recurrent
                        && (neuronGene.NeuronType != YNeuronType.Bias)
                        && (neuronGene.NeuronType != YNeuronType.Input))
                    {
                        neuron1ID = neuronGene.Id;
                        neuron2ID = neuronGene.Id;

                        neuronGene.Recurrent = true;
                        recurrent = true;

                        countTrysToFindLoop = 0;
                    }
                }
            }
            else
            {
                // try to add a regular link
                while ((countTrysToAddLink--) > 0)
                {
                    YNeuronGene neuron1 = ChooseRandomNeuron(true);
                    YNeuronGene neuron2 = ChooseRandomNeuron(false);

                    if (!IsDuplicateLink(neuron1ID, neuron2ID)
                        && (neuron1.Id != neuron2.Id)
                        && (neuron2.NeuronType != YNeuronType.Bias))
                    {
                        neuron1ID = neuron1.Id;
                        neuron2ID = neuron2.Id;
                        break;
                    }
                }
            }

            // did we fail to find a link
            if ((neuron1ID < 0) || (neuron2ID < 0))
            {
                return;
            }

            // check to see if this innovation has already been tried
            YInnovation innovation = ((YTraining)GA).Innovations.CheckInnovation(neuron1ID,
                                                                                                      neuron1ID,
                                                                                                      YInnovationType
                                                                                                          .NewLink);

            // see if this is a recurrent(backwards) link
            var neuronGene_0 = (YNeuronGene)neuronsQ
                                                    .Get(GetElementPos(neuron1ID));
            if (neuronGene_0.SplitY > neuronGene_0.SplitY)
            {
                recurrent = true;
            }

            // is this a new innovation?
            if (innovation == null)
            {
                // new innovation
                ((YTraining)GA).Innovations
                    .CreateNewInnovation(neuron1ID, neuron2ID,
                                         YInnovationType.NewLink);

                long id2 = GA.Population.AssignInnovationID();

                var linkGene = new YLinkGene(neuron1ID,
                                                neuron2ID, true, id2, RangeRandomizer.Randomize(-1, 1),
                                                recurrent);
                linksQ.Add(linkGene);
            }
            else
            {
                // existing innovation
                var linkGene_1 = new YLinkGene(neuron1ID,
                                                  neuron2ID, true, innovation.InnovationID,
                                                  RangeRandomizer.Randomize(-1, 1), recurrent);
                linksQ.Add(linkGene_1);
            }
        }

        /// <summary>
        /// Mutate the T by adding a neuron.
        /// </summary>
        ///
        /// <param name="mutationRate">The mutation rate.</param>
        /// <param name="numTrysToFindOldLink">The number of tries to find a link to split.</param>
        internal void AddNeuron(double mutationRate, int numTrysToFindOldLink)
        {
            // should we add a neuron?
            if (ThreadSafeRandom.NextDouble() > mutationRate)
            {
                return;
            }

            int countTrysToFindOldLink = numTrysToFindOldLink;

            // the link to split
            YLinkGene splitLink = null;

            int sizeBias = inputCount + outputCount + 10;

            // if there are not at least
            int upperLimit;
            if (linksQ.Size() < sizeBias)
            {
                upperLimit = NumGenes - 1 - (int)Math.Sqrt(NumGenes);
            }
            else
            {
                upperLimit = NumGenes - 1;
            }

            while ((countTrysToFindOldLink--) > 0)
            {
                // choose a link, use the square root to prefer the older links
                int i = RangeRandomizer.RandomInt(0, upperLimit);
                var link = (YLinkGene)linksQ
                                              .Get(i);

                // get the from neuron
                long fromNeuron = link.FromNeuronID;

                if ((link.Enabled)
                    && (!link.Recurrent)
                    && (((YNeuronGene)Neurons.Get(
                        GetElementPos(fromNeuron))).NeuronType != YNeuronType.Bias))
                {
                    splitLink = link;
                    break;
                }
            }

            if (splitLink == null)
            {
                return;
            }

            splitLink.Enabled = false;

            double originalWeight = splitLink.Weight;

            long from = splitLink.FromNeuronID;
            long to = splitLink.ToNeuronID;

            var fromGene = (YNeuronGene)Neurons.Get(
                GetElementPos(from));
            var toGene = (YNeuronGene)Neurons.Get(
                GetElementPos(to));

            double newDepth = (fromGene.SplitY + toGene.SplitY) / 2;
            double newWidth = (fromGene.SplitX + toGene.SplitX) / 2;

            // has this innovation already been tried?
            YInnovation innovation = ((YTraining)GA).Innovations.CheckInnovation(from, to,
                                                                                                      YInnovationType
                                                                                                          .NewNeuron);

            // prevent chaining
            if (innovation != null)
            {
                long neuronID = innovation.NeuronID;

                if (AlreadyHaveThisNeuronID(neuronID))
                {
                    innovation = null;
                }
            }

            if (innovation == null)
            {
                // this innovation has not been tried, create it
                long newNeuronID = ((YTraining)GA).Innovations.CreateNewInnovation(from, to,
                                                                                                     YInnovationType.
                                                                                                         NewNeuron,
                                                                                                     YNeuronType.
                                                                                                         Hidden,
                                                                                                     newWidth, newDepth);

                neuronsQ.Add(new YNeuronGene(
                                          YNeuronType.Hidden, newNeuronID, newDepth, newWidth));

                // add the first link
                long link1ID = (GA).Population.AssignInnovationID();

                ((YTraining)GA).Innovations
                    .CreateNewInnovation(from, newNeuronID,
                                         YInnovationType.NewLink);

                var link1 = new YLinkGene(from, newNeuronID,
                                             true, link1ID, 1.0d, false);

                linksQ.Add(link1);

                // add the second link
                long link2ID = (GA).Population.AssignInnovationID();

                ((YTraining)GA).Innovations
                    .CreateNewInnovation(newNeuronID, to,
                                         YInnovationType.NewLink);

                var link2 = new YLinkGene(newNeuronID, to, true,
                                             link2ID, originalWeight, false);

                linksQ.Add(link2);
            }

            else
            {
                // existing innovation
                long newNeuronID_0 = innovation.NeuronID;

                YInnovation innovationLink1 = ((YTraining)GA).Innovations.CheckInnovation(from,
                                                                                                               newNeuronID_0,
                                                                                                               YInnovationType
                                                                                                                   .
                                                                                                                   NewLink);
                YInnovation innovationLink2 =
                    ((YTraining)GA).Innovations.CheckInnovation(newNeuronID_0, to,
                                                                                  YInnovationType.NewLink);

                if ((innovationLink1 == null) || (innovationLink2 == null))
                {
                    throw new NeuralNetworkError("Y Error");
                }

                var link1_1 = new YLinkGene(from, newNeuronID_0,
                                               true, innovationLink1.InnovationID, 1.0d, false);
                var link2_2 = new YLinkGene(newNeuronID_0, to, true,
                                               innovationLink2.InnovationID, originalWeight, false);

                linksQ.Add(link1_1);
                linksQ.Add(link2_2);

                var newNeuron = new YNeuronGene(
                    YNeuronType.Hidden, newNeuronID_0, newDepth, newWidth);

                neuronsQ.Add(newNeuron);
            }

            return;
        }

        /// <summary>
        /// Do we already have this neuron id?
        /// </summary>
        ///
        /// <param name="id">The id to check for.</param>
        /// <returns>True if we already have this neuron id.</returns>
        public bool AlreadyHaveThisNeuronID(long id)
        {
            foreach (IGene gene in neuronsQ.Genes)
            {
                var neuronGene = (YNeuronGene)gene;

                if (neuronGene.Id == id)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Choose a random neuron.
        /// </summary>
        ///
        /// <param name="includeInput">Should the input neurons be included.</param>
        /// <returns>The random neuron.</returns>
        private YNeuronGene ChooseRandomNeuron(bool includeInput)
        {
            int start;

            if (includeInput)
            {
                start = 0;
            }
            else
            {
                start = inputCount + 1;
            }

            int neuronPos = RangeRandomizer.RandomInt(start, Neurons
                                                                 .Size() - 1);
            var neuronGene = (YNeuronGene)neuronsQ
                                                  .Get(neuronPos);
            return neuronGene;
        }

        /// <summary>
        /// Convert the genes to an actual network.
        /// </summary>
        ///
        public override void Decode()
        {
            var pop = (YPopulation)Population;

            IList<YNeuron> neurons = new List<YNeuron>();


            foreach (IGene gene in Neurons.Genes)
            {
                var neuronGene = (YNeuronGene)gene;
                var neuron = new YNeuron(
                    neuronGene.NeuronType, neuronGene.Id,
                    neuronGene.SplitY, neuronGene.SplitX,
                    neuronGene.ActivationResponse);

                neurons.Add(neuron);
            }


            // now to create the links.
            foreach (IGene gene_0 in Links.Genes)
            {
                var linkGene = (YLinkGene)gene_0;
                if (linkGene.Enabled)
                {
                    int element = GetElementPos(linkGene.FromNeuronID);
                    YNeuron fromNeuron = neurons[element];

                    element = GetElementPos(linkGene.ToNeuronID);
                    if (element == -1)
                    {
                        Console.Out.WriteLine("test");
                    }
                    YNeuron toNeuron = neurons[element];

                    var link = new YLink(linkGene.Weight,
                                            fromNeuron, toNeuron, linkGene.Recurrent);

                    fromNeuron.OutputboundLinks.Add(link);
                    toNeuron.InboundLinks.Add(link);
                }
            }

            var network = new YNetwork(inputCount, outputCount, neurons,
                                          pop.YActivationFunction,
                                          pop.OutputActivationFunction, 0);

            network.Snapshot = pop.Snapshot;
            Organism = network;
        }

        /// <summary>
        /// Convert the network to genes. Not currently supported.
        /// </summary>
        ///
        public override void Syntesis()
        {
        }

        /// <summary>
        /// Get the compatibility score with another T. Used to determine
        /// species.
        /// </summary>
        ///
        /// <param name="T">The other T.</param>
        /// <returns>The score.</returns>
        public double GetCompatibilityScore(YT T)
        {
            double numDisjoint = 0;
            double numExcess = 0;
            double numMatched = 0;
            double weightDifference = 0;

            int g1 = 0;
            int g2 = 0;

            while ((g1 < linksQ.Size() - 1)
                   || (g2 < linksQ.Size() - 1))
            {
                if (g1 == linksQ.Size() - 1)
                {
                    g2++;
                    numExcess++;

                    continue;
                }

                if (g2 == T.Links.Size() - 1)
                {
                    g1++;
                    numExcess++;

                    continue;
                }

                // get innovation numbers for each gene at this point
                long id1 = ((YLinkGene)linksQ.Get(g1)).InnovationId;
                long id2 = ((YLinkGene)T.Links.Get(g2)).InnovationId;

                // innovation numbers are identical so increase the matched score
                if (id1 == id2)
                {
                    g1++;
                    g2++;
                    numMatched++;

                    // get the weight difference between these two genes
                    weightDifference += Math.Abs(((YLinkGene)linksQ.Get(g1)).Weight
                                                 - ((YLinkGene)T.Links.Get(g2)).Weight);
                }

                // innovation numbers are different so increment the disjoint score
                if (id1 < id2)
                {
                    numDisjoint++;
                    g1++;
                }

                if (id1 > id2)
                {
                    ++numDisjoint;
                    ++g2;
                }
            }

            int longest = T.NumGenes;

            if (NumGenes > longest)
            {
                longest = NumGenes;
            }

            double score = (TWEAK_EXCESS * numExcess / longest)
                           + (TWEAK_DISJOINT * numDisjoint / longest)
                           + (TWEAK_MATCHED * weightDifference / numMatched);

            return score;
        }

        /// <summary>
        /// Get the specified neuron's index.
        /// </summary>
        ///
        /// <param name="neuronID">The neuron id to check for.</param>
        /// <returns>The index.</returns>
        private int GetElementPos(long neuronID)
        {
            for (int i = 0; i < Neurons.Size(); i++)
            {
                var neuronGene = (YNeuronGene)neuronsQ
                                                      .GetGene(i);
                if (neuronGene.Id == neuronID)
                {
                    return i;
                }
            }

            return -1;
        }


        /// <summary>
        /// Get the specified split y.
        /// </summary>
        ///
        /// <param name="nd">The neuron.</param>
        /// <returns>The split y.</returns>
        public double GetSplitY(int nd)
        {
            return ((YNeuronGene)neuronsQ.Get(nd)).SplitY;
        }

        /// <summary>
        /// Determine if this is a duplicate link.
        /// </summary>
        ///
        /// <param name="fromNeuronID">The from neuron id.</param>
        /// <param name="toNeuronID">The to neuron id.</param>
        /// <returns>True if this is a duplicate link.</returns>
        public bool IsDuplicateLink(long fromNeuronID,
                                    long toNeuronID)
        {
            foreach (IGene gene in Links.Genes)
            {
                var linkGene = (YLinkGene)gene;
                if ((linkGene.FromNeuronID == fromNeuronID)
                    && (linkGene.ToNeuronID == toNeuronID))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Mutate the activation response.
        /// </summary>
        ///
        /// <param name="mutateRate">The mutation rate.</param>
        /// <param name="maxPertubation">The maximum to perturb it by.</param>
        public void MutateActivationResponse(double mutateRate,
                                             double maxPertubation)
        {
            foreach (IGene gene in neuronsQ.Genes)
            {
                if (ThreadSafeRandom.NextDouble() < mutateRate)
                {
                    var neuronGene = (YNeuronGene)gene;
                    neuronGene.ActivationResponse = neuronGene.ActivationResponse
                                                    + RangeRandomizer.Randomize(-1, 1) * maxPertubation;
                }
            }
        }

        /// <summary>
        /// Mutate the weights.
        /// </summary>
        ///
        /// <param name="mutateRate">The mutation rate.</param>
        /// <param name="probNewMutate">The probability of a whole new weight.</param>
        /// <param name="maxPertubation">The max perturbation.</param>
        public void MutateWeights(double mutateRate,
                                  double probNewMutate, double maxPertubation)
        {
            foreach (IGene gene in linksQ.Genes)
            {
                var linkGene = (YLinkGene)gene;
                if (ThreadSafeRandom.NextDouble() < mutateRate)
                {
                    if (ThreadSafeRandom.NextDouble() < probNewMutate)
                    {
                        linkGene.Weight = RangeRandomizer.Randomize(-1, 1);
                    }
                    else
                    {
                        linkGene.Weight = linkGene.Weight
                                          + RangeRandomizer.Randomize(-1, 1) * maxPertubation;
                    }
                }
            }
        }

        /// <summary>
        /// Sort the genes.
        /// </summary>
        ///
        public void SortGenes()
        {
            linksQ.Genes.Sort();
        }
    }
}
