using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PersistBoltzmann : ISyntPersistor
    {
        #region SyntPersistor Members

        /// <inheritdoc/>
        public virtual int FileVersion
        {
            get { return 1; }
        }


        /// <inheritdoc/>
        public virtual String PersistClassString
        {
            get { return "BoltzmannMachine"; }
        }


        /// <inheritdoc/>
        public Object Read(Stream mask0)
        {
            var result = new BoltzmannMachine();
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("BOLTZMANN")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("BOLTZMANN")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();
                    result.Weights = NumberList.FromList(CSVFormat.EgFormat,
                                                         (p[PersistConst.Weights]));
                    result.SetCurrentState(NumberList.FromList(CSVFormat.EgFormat,
                                                               (p[PersistConst.Output])));
                    result.NeuronCount = SyntFileSection.ParseInt(p,
                                                                   PersistConst.NeuronCount);

                    result.Threshold = NumberList.FromList(CSVFormat.EgFormat,
                                                           (p[PersistConst.Thresholds]));
                    result.AnnealCycles = SyntFileSection.ParseInt(p,
                                                                    BoltzmannMachine.ParamAnnealCycles);
                    result.RunCycles = SyntFileSection.ParseInt(p,
                                                                 BoltzmannMachine.ParamRunCycles);
                    result.Temperature = SyntFileSection.ParseDouble(p,
                                                                      PersistConst.Temperature);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var boltz = (BoltzmannMachine)obj;
            xout.AddSection("BOLTZMANN");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(boltz.Properties);
            xout.AddSubSection("NETWORK");
            xout.WriteProperty(PersistConst.Weights, boltz.Weights);
            xout.WriteProperty(PersistConst.Output, boltz.CurrentState.Data);
            xout.WriteProperty(PersistConst.NeuronCount, boltz.NeuronCount);

            xout.WriteProperty(PersistConst.Thresholds, boltz.Threshold);
            xout.WriteProperty(BoltzmannMachine.ParamAnnealCycles,
                               boltz.AnnealCycles);
            xout.WriteProperty(BoltzmannMachine.ParamRunCycles, boltz.RunCycles);
            xout.WriteProperty(PersistConst.Temperature, boltz.Temperature);

            xout.Flush();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(BoltzmannMachine); }
        }

        #endregion
    }
}
