using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class PersistYPopulation : ISyntPersistor
    {
        #region SyntPersistor Members

        /// <summary>
        /// The persistence class string.
        /// </summary>
        public virtual String PersistClassString
        {
            get { return typeof(YPopulation).Name; }
        }


        /// <summary>
        /// Read the object.
        /// </summary>
        /// <param name="mask0">The stream to read the object from.</param>
        /// <returns>The object that was loaded.</returns>
        public virtual Object Read(Stream mask0)
        {
            var result = new YPopulation();
            var innovationList = new YInnovationList { Population = result };
            result.Innovations = innovationList;
            var ins0 = new SyntReadHelper(mask0);
            IDictionary<Int32, ISpecies> speciesMap = new Dictionary<Int32, ISpecies>();
            IDictionary<ISpecies, Int32> leaderMap = new Dictionary<ISpecies, Int32>();
            IDictionary<Int32, IT> TMap = new Dictionary<Int32, IT>();
            SyntFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("Y-POPULATION")
                    && section.SubSectionName.Equals("INNOVATIONS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);
                        var innovation = new YInnovation
                        {
                            InnovationID = Int32.Parse(cols[0]),
                            InnovationType = StringToInnovationType(cols[1]),
                            NeuronType = StringToNeuronType(cols[2]),
                            SplitX = CSVFormat.EgFormat.Parse(cols[3]),
                            SplitY = CSVFormat.EgFormat.Parse(cols[4]),
                            NeuronID = Int32.Parse(cols[5]),
                            FromNeuronID = Int32.Parse(cols[6]),
                            ToNeuronID = Int32.Parse(cols[7])
                        };
                        result.Innovations.Add(innovation);
                    }
                }
                else if (section.SectionName.Equals("Y-POPULATION")
                         && section.SubSectionName.Equals("SPECIES"))
                {
                    foreach (String line in section.Lines)
                    {
                        String[] cols = line.Split(',');
                        var species = new BasicSpecies
                        {
                            SpeciesID = Int32.Parse(cols[0]),
                            Age = Int32.Parse(cols[1]),
                            BestScore = CSVFormat.EgFormat.Parse(cols[2]),
                            GensNoImprovement = Int32.Parse(cols[3]),
                            SpawnsRequired = CSVFormat.EgFormat
                                                  .Parse(cols[4])
                        };

                        species.SpawnsRequired = CSVFormat.EgFormat
                            .Parse(cols[5]);
                        leaderMap[(species)] = (Int32.Parse(cols[6]));
                        result.Species.Add(species);
                        speciesMap[((int)species.SpeciesID)] = (species);
                    }
                }
                else if (section.SectionName.Equals("Y-POPULATION")
                         && section.SubSectionName.Equals("TS"))
                {
                    YT lastT = null;

                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);
                        if (cols[0].Equals("g", StringComparison.InvariantCultureIgnoreCase))
                        {
                            lastT = new YT
                            {
                                NeuronsQ = new Q(),
                                LinksQ = new Q()
                            };
                            lastT.Qs.Add(lastT.NeuronsQ);
                            lastT.Qs.Add(lastT.LinksQ);
                            lastT.TID = Int32.Parse(cols[1]);
                            lastT.SpeciesID = Int32.Parse(cols[2]);
                            lastT.AdjustedScore = CSVFormat.EgFormat
                                .Parse(cols[3]);
                            lastT.AmountToSpawn = CSVFormat.EgFormat
                                .Parse(cols[4]);
                            lastT.NetworkDepth = Int32.Parse(cols[5]);
                            lastT.Score = CSVFormat.EgFormat.Parse(cols[6]);
                            result.Add(lastT);
                            TMap[(int)lastT.TID] = lastT;
                        }
                        else if (cols[0].Equals("n", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var neuronGene = new YNeuronGene
                            {
                                Id = Int32.Parse(cols[1]),
                                NeuronType = StringToNeuronType(cols[2]),
                                Enabled = Int32.Parse(cols[3]) > 0,
                                InnovationId = Int32.Parse(cols[4]),
                                ActivationResponse = CSVFormat.EgFormat
                                                         .Parse(cols[5]),
                                SplitX = CSVFormat.EgFormat.Parse(cols[6]),
                                SplitY = CSVFormat.EgFormat.Parse(cols[7])
                            };
                            lastT.Neurons.Add(neuronGene);
                        }
                        else if (cols[0].Equals("l", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var linkGene = new YLinkGene();
                            linkGene.Id = Int32.Parse(cols[1]);
                            linkGene.Enabled = Int32.Parse(cols[2]) > 0;
                            linkGene.Recurrent = Int32.Parse(cols[3]) > 0;
                            linkGene.FromNeuronID = Int32.Parse(cols[4]);
                            linkGene.ToNeuronID = Int32.Parse(cols[5]);
                            linkGene.Weight = CSVFormat.EgFormat.Parse(cols[6]);
                            linkGene.InnovationId = Int32.Parse(cols[7]);
                            lastT.Links.Add(linkGene);
                        }
                    }
                }
                else if (section.SectionName.Equals("Y-POPULATION")
                         && section.SubSectionName.Equals("CONFIG"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    result.YActivationFunction = SyntFileSection
                        .ParseActivationFunction(paras,
                                                 YPopulation.PropertyYActivation);
                    result.OutputActivationFunction = SyntFileSection
                        .ParseActivationFunction(paras,
                                                 YPopulation.PropertyOutputActivation);
                    result.Snapshot = SyntFileSection.ParseBoolean(paras,
                                                                    PersistConst.Snapshot);
                    result.InputCount = SyntFileSection.ParseInt(paras,
                                                                  PersistConst.InputCount);
                    result.OutputCount = SyntFileSection.ParseInt(paras,
                                                                   PersistConst.OutputCount);
                    result.OldAgePenalty = SyntFileSection.ParseDouble(paras,
                                                                        PopulationConst.PropertyOldAgePenalty);
                    result.OldAgeThreshold = SyntFileSection.ParseInt(paras,
                                                                       PopulationConst.PropertyOldAgeThreshold);
                    result.PopulationSize = SyntFileSection.ParseInt(paras,
                                                                      PopulationConst.PropertyPopulationSize);
                    result.SurvivalRate = SyntFileSection.ParseDouble(paras,
                                                                       PopulationConst.PropertySurvivalRate);
                    result.YoungBonusAgeThreshhold = SyntFileSection.ParseInt(
                        paras, PopulationConst.PropertyYoungAgeThreshold);
                    result.YoungScoreBonus = SyntFileSection.ParseDouble(paras,
                                                                          PopulationConst.PropertyYoungAgeBonus);
                    result.TIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                  PopulationConst.
                                                                                      PropertyNextTID);
                    result.InnovationIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                      PopulationConst.
                                                                                          PropertyNextInnovationID);
                    result.GeneIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                PopulationConst.
                                                                                    PropertyNextGeneID);
                    result.SpeciesIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                   PopulationConst.
                                                                                       PropertyNextSpeciesID);
                }
            }

            // now link everything up


            // first put all the Ts into correct species
            foreach (IT T in result.Ts)
            {
                var YT = (YT)T;
                var speciesId = (int)YT.SpeciesID;
                if (speciesMap.ContainsKey(speciesId))
                {
                    ISpecies s = speciesMap[speciesId];
                    s.Members.Add(YT);
                }

                YT.InputCount = result.InputCount;
                YT.OutputCount = result.OutputCount;
            }


            // set the species leader links
            foreach (ISpecies species in leaderMap.Keys)
            {
                int leaderID = leaderMap[species];
                IT leader = TMap[leaderID];
                species.Leader = leader;
                ((BasicSpecies)species).Population = result;
            }

            return result;
        }

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="os">The stream to write to.</param>
        /// <param name="obj">The object to save.</param>
        public virtual void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var pop = (YPopulation)obj;
            xout.AddSection("Y-POPULATION");
            xout.AddSubSection("CONFIG");
            xout.WriteProperty(PersistConst.Snapshot, pop.Snapshot);
            xout.WriteProperty(YPopulation.PropertyOutputActivation,
                               pop.OutputActivationFunction);
            xout.WriteProperty(YPopulation.PropertyYActivation,
                               pop.YActivationFunction);
            xout.WriteProperty(PersistConst.InputCount, pop.InputCount);
            xout.WriteProperty(PersistConst.OutputCount, pop.OutputCount);
            xout.WriteProperty(PopulationConst.PropertyOldAgePenalty,
                               pop.OldAgePenalty);
            xout.WriteProperty(PopulationConst.PropertyOldAgeThreshold,
                               pop.OldAgeThreshold);
            xout.WriteProperty(PopulationConst.PropertyPopulationSize,
                               pop.PopulationSize);
            xout.WriteProperty(PopulationConst.PropertySurvivalRate,
                               pop.SurvivalRate);
            xout.WriteProperty(PopulationConst.PropertyYoungAgeThreshold,
                               pop.YoungBonusAgeThreshold);
            xout.WriteProperty(PopulationConst.PropertyYoungAgeBonus,
                               pop.YoungScoreBonus);
            xout.WriteProperty(PopulationConst.PropertyNextTID, pop.TIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PropertyNextInnovationID, pop.InnovationIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PropertyNextGeneID, pop.GeneIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PropertyNextSpeciesID, pop.SpeciesIDGenerate.CurrentID);
            xout.AddSubSection("INNOVATIONS");
            if (pop.Innovations != null)
            {
                foreach (IInnovation innovation in pop.Innovations.Innovations)
                {
                    var YInnovation = (YInnovation)innovation;
                    xout.AddColumn(YInnovation.InnovationID);
                    xout.AddColumn(InnovationTypeToString(YInnovation.InnovationType));
                    xout.AddColumn(NeuronTypeToString(YInnovation.NeuronType));
                    xout.AddColumn(YInnovation.SplitX);
                    xout.AddColumn(YInnovation.SplitY);
                    xout.AddColumn(YInnovation.NeuronID);
                    xout.AddColumn(YInnovation.FromNeuronID);
                    xout.AddColumn(YInnovation.ToNeuronID);
                    xout.WriteLine();
                }
            }
            xout.AddSubSection("TS");

            foreach (IT T in pop.Ts)
            {
                var YT = (YT)T;
                xout.AddColumn("g");
                xout.AddColumn(YT.TID);
                xout.AddColumn(YT.SpeciesID);
                xout.AddColumn(YT.AdjustedScore);
                xout.AddColumn(YT.AmountToSpawn);
                xout.AddColumn(YT.NetworkDepth);
                xout.AddColumn(YT.Score);
                xout.WriteLine();


                foreach (IGene neuronGene in YT.Neurons.Genes)
                {
                    var YNeuronGene = (YNeuronGene)neuronGene;
                    xout.AddColumn("n");
                    xout.AddColumn(YNeuronGene.Id);
                    xout.AddColumn(NeuronTypeToString(YNeuronGene.NeuronType));
                    xout.AddColumn(YNeuronGene.Enabled);
                    xout.AddColumn(YNeuronGene.InnovationId);
                    xout.AddColumn(YNeuronGene.ActivationResponse);
                    xout.AddColumn(YNeuronGene.SplitX);
                    xout.AddColumn(YNeuronGene.SplitY);
                    xout.WriteLine();
                }

                foreach (IGene linkGene in YT.Links.Genes)
                {
                    var YLinkGene = (YLinkGene)linkGene;
                    xout.AddColumn("l");
                    xout.AddColumn(YLinkGene.Id);
                    xout.AddColumn(YLinkGene.Enabled);
                    xout.AddColumn(YLinkGene.Recurrent);
                    xout.AddColumn(YLinkGene.FromNeuronID);
                    xout.AddColumn(YLinkGene.ToNeuronID);
                    xout.AddColumn(YLinkGene.Weight);
                    xout.AddColumn(YLinkGene.InnovationId);
                    xout.WriteLine();
                }
            }
            xout.AddSubSection("SPECIES");

            foreach (ISpecies species in pop.Species)
            {
                xout.AddColumn(species.SpeciesID);
                xout.AddColumn(species.Age);
                xout.AddColumn(species.BestScore);
                xout.AddColumn(species.GensNoImprovement);
                xout.AddColumn(species.NumToSpawn);
                xout.AddColumn(species.SpawnsRequired);
                xout.AddColumn(species.Leader.TID);
                xout.WriteLine();
            }
            xout.Flush();
        }

        /// <summary>
        /// The file version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }

        #endregion

        /// <summary>
        /// Convert the neuron type to a string.
        /// </summary>
        /// <param name="t">The neuron type.</param>
        /// <returns>The string.</returns>
        public static String NeuronTypeToString(YNeuronType t)
        {
            switch (t)
            {
                case YNeuronType.Bias:
                    return ("b");
                case YNeuronType.Hidden:
                    return ("h");
                case YNeuronType.Input:
                    return ("i");
                case YNeuronType.None:
                    return ("n");
                case YNeuronType.Output:
                    return ("o");
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert the innovation type to a string.
        /// </summary>
        /// <param name="t">The innovation type.</param>
        /// <returns>The string.</returns>
        public static String InnovationTypeToString(YInnovationType t)
        {
            switch (t)
            {
                case YInnovationType.NewLink:
                    return "l";
                case YInnovationType.NewNeuron:
                    return "n";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert a string to an innovation type.
        /// </summary>
        /// <param name="t">The string to convert.</param>
        /// <returns>The innovation type.</returns>
        public static YInnovationType StringToInnovationType(String t)
        {
            if (t.Equals("l", StringComparison.InvariantCultureIgnoreCase))
            {
                return YInnovationType.NewLink;
            }
            if (t.Equals("n", StringComparison.InvariantCultureIgnoreCase))
            {
                return YInnovationType.NewNeuron;
            }
            return default(YInnovationType) /* was: null */;
        }

        /// <summary>
        /// Convert a string to a neuron type.
        /// </summary>
        /// <param name="t">The string.</param>
        /// <returns>The resulting neuron type.</returns>
        public static YNeuronType StringToNeuronType(String t)
        {
            if (t.Equals("b"))
            {
                return YNeuronType.Bias;
            }
            if (t.Equals("h"))
            {
                return YNeuronType.Hidden;
            }
            if (t.Equals("i"))
            {
                return YNeuronType.Input;
            }
            if (t.Equals("n"))
            {
                return YNeuronType.None;
            }
            if (t.Equals("o"))
            {
                return YNeuronType.Output;
            }
            throw new SyntError("Unknonw neuron type: " + t);
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(YPopulation); }
        }
    }
}
