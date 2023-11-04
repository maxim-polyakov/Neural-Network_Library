using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PersistBAM : ISyntPersistor
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
            get { return "BAM"; }
        }


        /// <inheritdoc/>
        public Object Read(Stream mask0)
        {
            var result = new BAMNetwork();
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("BAM")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("BAM")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();

                    result.F1Count = SyntFileSection.ParseInt(p,
                                                               PersistConst.PropertyF1Count);
                    result.F2Count = SyntFileSection.ParseInt(p,
                                                               PersistConst.PropertyF2Count);
                    result.WeightsF1ToF2 = SyntFileSection.ParseMatrix(p, PersistConst.PropertyWeightsF1F2);
                    result.WeightsF2ToF1 = SyntFileSection.ParseMatrix(p, PersistConst.PropertyWeightsF2F1);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var bam = (BAMNetwork)obj;
            xout.AddSection("BAM");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(bam.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.PropertyF1Count, bam.F1Count);
            xout.WriteProperty(PersistConst.PropertyF2Count, bam.F2Count);
            xout.WriteProperty(PersistConst.PropertyWeightsF1F2,
                               bam.WeightsF1ToF2);
            xout.WriteProperty(PersistConst.PropertyWeightsF2F1,
                               bam.WeightsF2ToF1);

            xout.Flush();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(BAMNetwork); }
        }

        #endregion
    }
}
