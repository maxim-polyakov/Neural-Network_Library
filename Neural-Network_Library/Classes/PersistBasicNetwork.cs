using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PersistBasicNetwork : ISyntPersistor
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
            get { return "BasicNetwork"; }
        }


        /// <summary>
        /// Read an object.
        /// </summary>
        ///
        public Object Read(Stream mask0)
        {
            var result = new BasicNetwork();
            var flat = new FlatNetwork();
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("BASIC")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("BASIC")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();

                    flat.BeginTraining = SyntFileSection.ParseInt(p,
                                                                   BasicNetwork.TagBeginTraining);
                    flat.ConnectionLimit = SyntFileSection.ParseDouble(p,
                                                                        BasicNetwork.TagConnectionLimit);
                    flat.ContextTargetOffset = SyntFileSection.ParseIntArray(
                        p, BasicNetwork.TagContextTargetOffset);
                    flat.ContextTargetSize = SyntFileSection.ParseIntArray(
                        p, BasicNetwork.TagContextTargetSize);
                    flat.EndTraining = SyntFileSection.ParseInt(p,
                                                                 BasicNetwork.TagEndTraining);
                    flat.HasContext = SyntFileSection.ParseBoolean(p,
                                                                    BasicNetwork.TagHasContext);
                    flat.InputCount = SyntFileSection.ParseInt(p,
                                                                PersistConst.InputCount);
                    flat.LayerCounts = SyntFileSection.ParseIntArray(p,
                                                                      BasicNetwork.TagLayerCounts);
                    flat.LayerFeedCounts = SyntFileSection.ParseIntArray(p,
                                                                          BasicNetwork.TagLayerFeedCounts);
                    flat.LayerContextCount = SyntFileSection.ParseIntArray(
                        p, BasicNetwork.TagLayerContextCount);
                    flat.LayerIndex = SyntFileSection.ParseIntArray(p,
                                                                     BasicNetwork.TagLayerIndex);
                    flat.LayerOutput = section.ParseDoubleArray(p, PersistConst.Output);
                    flat.LayerSums = new double[flat.LayerOutput.Length];
                    flat.OutputCount = SyntFileSection.ParseInt(p,
                                                                 PersistConst.OutputCount);
                    flat.WeightIndex = SyntFileSection.ParseIntArray(p,
                                                                      BasicNetwork.TagWeightIndex);
                    flat.Weights = section.ParseDoubleArray(p, PersistConst.Weights);
                    flat.BiasActivation = section.ParseDoubleArray(p, BasicNetwork.TagBiasActivation);
                }
                else if (section.SectionName.Equals("BASIC")
                         && section.SubSectionName.Equals("ACTIVATION"))
                {
                    int index = 0;

                    flat.ActivationFunctions = new IActivationFunction[flat.LayerCounts.Length];


                    foreach (String line in section.Lines)
                    {
                        IActivationFunction af;
                        IList<String> cols = SyntFileSection
                            .SplitColumns(line);
                        String name = ReflectionUtil.AfPath
                                      + cols[0];
                        try
                        {
                            af = (IActivationFunction)ReflectionUtil.LoadObject(name);
                        }
                        catch (TypeLoadException e)
                        {
                            throw new PersistError(e);
                        }
                        catch (TargetException e)
                        {
                            throw new PersistError(e);
                        }
                        catch (MemberAccessException e)
                        {
                            throw new PersistError(e);
                        }

                        for (int i = 0; i < af.ParamNames.Length; i++)
                        {
                            af.Params[i] =
                                        CSVFormat.EgFormat.Parse(cols[i + 1]);
                        }

                        flat.ActivationFunctions[index++] = af;
                    }
                }
            }

            result.Structure.Flat = flat;

            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var net = (BasicNetwork)obj;
            FlatNetwork flat = net.Structure.Flat;
            xout.AddSection("BASIC");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(net.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(BasicNetwork.TagBeginTraining,
                               flat.BeginTraining);
            xout.WriteProperty(BasicNetwork.TagConnectionLimit,
                               flat.ConnectionLimit);
            xout.WriteProperty(BasicNetwork.TagContextTargetOffset,
                               flat.ContextTargetOffset);
            xout.WriteProperty(BasicNetwork.TagContextTargetSize,
                               flat.ContextTargetSize);
            xout.WriteProperty(BasicNetwork.TagEndTraining, flat.EndTraining);
            xout.WriteProperty(BasicNetwork.TagHasContext, flat.HasContext);
            xout.WriteProperty(PersistConst.InputCount, flat.InputCount);
            xout.WriteProperty(BasicNetwork.TagLayerCounts, flat.LayerCounts);
            xout.WriteProperty(BasicNetwork.TagLayerFeedCounts,
                               flat.LayerFeedCounts);
            xout.WriteProperty(BasicNetwork.TagLayerContextCount,
                               flat.LayerContextCount);
            xout.WriteProperty(BasicNetwork.TagLayerIndex, flat.LayerIndex);
            xout.WriteProperty(PersistConst.Output, flat.LayerOutput);
            xout.WriteProperty(PersistConst.OutputCount, flat.OutputCount);
            xout.WriteProperty(BasicNetwork.TagWeightIndex, flat.WeightIndex);
            xout.WriteProperty(PersistConst.Weights, flat.Weights);
            xout.WriteProperty(BasicNetwork.TagBiasActivation,
                               flat.BiasActivation);
            xout.AddSubSection("ACTIVATION");

            foreach (IActivationFunction af in flat.ActivationFunctions)
            {
                xout.AddColumn(af.GetType().Name);
                for (int i = 0; i < af.Params.Length; i++)
                {
                    xout.AddColumn(af.Params[i]);
                }
                xout.WriteLine();
            }

            xout.Flush();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(BasicNetwork); }
        }

        #endregion
    }
}
