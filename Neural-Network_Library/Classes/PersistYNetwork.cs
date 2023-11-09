using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PersistYNetwork : ISyntPersistor
    {
        #region SyntPersistor Members

        /// <summary>
        /// The file version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }

        /// <summary>
        /// The persist class string.
        /// </summary>
        public virtual String PersistClassString
        {
            get { return "YNetwork"; }
        }

        /// <summary>
        /// Read the object.
        /// </summary>
        /// <param name="mask0">The stream to read from.</param>
        /// <returns>The loaded object.</returns>
        public virtual Object Read(Stream mask0)
        {
            var result = new YNetwork();
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;
            IDictionary<Int32, YNeuron> neuronMap = new Dictionary<Int32, YNeuron>();

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("Y")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    foreach (String key in paras.Keys)
                    {
                        result.Properties.Add(key, paras[key]);
                    }
                }
                if (section.SectionName.Equals("Y")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();

                    result.InputCount = SyntFileSection.ParseInt(p,
                                                                  PersistConst.InputCount);
                    result.OutputCount = SyntFileSection.ParseInt(p,
                                                                   PersistConst.OutputCount);
                    result.ActivationFunction = SyntFileSection
                        .ParseActivationFunction(p,
                                                 PersistConst.ActivationFunction);
                    result.OutputActivationFunction = SyntFileSection
                        .ParseActivationFunction(p,
                                                 YPopulation.PropertyOutputActivation);
                    result.NetworkDepth = SyntFileSection.ParseInt(p,
                                                                    PersistConst.Depth);
                    result.Snapshot = SyntFileSection.ParseBoolean(p,
                                                                    PersistConst.Snapshot);
                }
                else if (section.SectionName.Equals("Y")
                         && section.SubSectionName.Equals("NEURONS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);

                        long neuronID = Int32.Parse(cols[0]);
                        YNeuronType neuronType = PersistYPopulation
                            .StringToNeuronType(cols[1]);
                        double activationResponse = CSVFormat.EgFormat
                            .Parse(cols[2]);
                        double splitY = CSVFormat.EgFormat
                            .Parse(cols[3]);
                        double splitX = CSVFormat.EgFormat
                            .Parse(cols[4]);

                        var YNeuron = new YNeuron(neuronType,
                                                        neuronID, splitY, splitX, activationResponse);
                        result.Neurons.Add(YNeuron);
                        neuronMap[((int)neuronID)] = (YNeuron);
                    }
                }
                else if (section.SectionName.Equals("Y")
                         && section.SubSectionName.Equals("LINKS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);
                        int fromID = Int32.Parse(cols[0]);
                        int toID = Int32.Parse(cols[1]);
                        bool recurrent = Int32.Parse(cols[2]) > 0;
                        double weight = CSVFormat.EgFormat.Parse(cols[3]);
                        YNeuron fromNeuron = (neuronMap[fromID]);
                        YNeuron toNeuron = (neuronMap[toID]);
                        var YLink = new YLink(weight, fromNeuron,
                                                    toNeuron, recurrent);
                        fromNeuron.OutputboundLinks.Add(YLink);
                        toNeuron.InboundLinks.Add(YLink);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="os">The output stream.</param>
        /// <param name="obj">The object to save.</param>
        public virtual void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var Y = (YNetwork)obj;
            xout.AddSection("Y");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(Y.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.InputCount, Y.InputCount);
            xout.WriteProperty(PersistConst.OutputCount, Y.OutputCount);
            xout.WriteProperty(PersistConst.ActivationFunction,
                               Y.ActivationFunction);
            xout.WriteProperty(YPopulation.PropertyOutputActivation,
                               Y.OutputActivationFunction);
            xout.WriteProperty(PersistConst.Depth, Y.NetworkDepth);
            xout.WriteProperty(PersistConst.Snapshot, Y.Snapshot);

            xout.AddSubSection("NEURONS");

            foreach (YNeuron YNeuron in Y.Neurons)
            {
                xout.AddColumn((int)YNeuron.NeuronID);
                xout.AddColumn(PersistYPopulation.NeuronTypeToString(YNeuron.NeuronType));
                xout.AddColumn(YNeuron.ActivationResponse);
                xout.AddColumn(YNeuron.SplitX);
                xout.AddColumn(YNeuron.SplitY);
                xout.WriteLine();
            }

            xout.AddSubSection("LINKS");

            foreach (YNeuron YNeuron in Y.Neurons)
            {
                foreach (YLink link in YNeuron.OutputboundLinks)
                {
                    WriteLink(xout, link);
                }
            }

            xout.Flush();
        }

        #endregion

        /// <summary>
        /// Write a link.
        /// </summary>
        /// <param name="xout">The output file.</param>
        /// <param name="link">The link.</param>
        private static void WriteLink(SyntWriteHelper xout, YLink link)
        {
            xout.AddColumn((int)link.FromNeuron.NeuronID);
            xout.AddColumn((int)link.ToNeuron.NeuronID);
            xout.AddColumn(link.Recurrent);
            xout.AddColumn(link.Weight);
            xout.WriteLine();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(YNetwork); }
        }
    }
}
