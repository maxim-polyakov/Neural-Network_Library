using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PersistART1 : ISyntPersistor
    {
        #region SyntPersistor Members

        /// <inheritdoc/>
        public int FileVersion
        {
            get { return 1; }
        }


        /// <inheritdoc/>
        public String PersistClassString
        {
            get { return "ART1"; }
        }


        /// <inheritdoc/>
        public Object Read(Stream mask0)
        {
            var result = new ART1();
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("ART1")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("ART1")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();

                    result.A1 = SyntFileSection.ParseDouble(p,
                                                             BasicART.PropertyA1);
                    result.B1 = SyntFileSection.ParseDouble(p,
                                                             BasicART.PropertyB1);
                    result.C1 = SyntFileSection.ParseDouble(p,
                                                             BasicART.PropertyC1);
                    result.D1 = SyntFileSection.ParseDouble(p,
                                                             BasicART.PropertyD1);
                    result.F1Count = SyntFileSection.ParseInt(p,
                                                               PersistConst.PropertyF1Count);
                    result.F2Count = SyntFileSection.ParseInt(p,
                                                               PersistConst.PropertyF2Count);
                    result.NoWinner = SyntFileSection.ParseInt(p,
                                                                BasicART.PropertyNoWinner);
                    result.L = SyntFileSection
                        .ParseDouble(p, BasicART.PropertyL);
                    result.Vigilance = SyntFileSection.ParseDouble(p,
                                                                    BasicART.PropertyVigilance);
                    result.WeightsF1ToF2 = SyntFileSection.ParseMatrix(p,
                                                                        PersistConst.PropertyWeightsF1F2);
                    result.WeightsF2ToF1 = SyntFileSection.ParseMatrix(p,
                                                                        PersistConst.PropertyWeightsF2F1);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var art1 = (ART1)obj;
            xout.AddSection("ART1");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(art1.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(BasicART.PropertyA1, art1.A1);
            xout.WriteProperty(BasicART.PropertyB1, art1.B1);
            xout.WriteProperty(BasicART.PropertyC1, art1.C1);
            xout.WriteProperty(BasicART.PropertyD1, art1.D1);
            xout.WriteProperty(PersistConst.PropertyF1Count, art1.F1Count);
            xout.WriteProperty(PersistConst.PropertyF2Count, art1.F2Count);
            xout.WriteProperty(BasicART.PropertyNoWinner, art1.NoWinner);
            xout.WriteProperty(BasicART.PropertyL, art1.L);
            xout.WriteProperty(BasicART.PropertyVigilance, art1.Vigilance);
            xout.WriteProperty(PersistConst.PropertyWeightsF1F2,
                               art1.WeightsF1ToF2);
            xout.WriteProperty(PersistConst.PropertyWeightsF2F1,
                               art1.WeightsF2ToF1);

            xout.Flush();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(ART1); }
        }

        #endregion
    }
}
