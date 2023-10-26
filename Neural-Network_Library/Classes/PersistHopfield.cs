using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PersistHopfield : ISyntPersistor
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
        /// The class string.
        /// </summary>
        ///
        public virtual String PersistClassString
        {
            get { return typeof(HopfieldNetwork).Name; }
        }


        /// <summary>
        /// Read a an object.
        /// </summary>
        public Object Read(Stream mask0)
        {
            var result = new HopfieldNetwork();
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("HOPFIELD")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
                if (section.SectionName.Equals("HOPFIELD")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();
                    result.Weights = NumberList.FromList(CSVFormat.EgFormat,
                                                         (p[PersistConst.Weights]));
                    result.SetCurrentState(NumberList.FromList(CSVFormat.EgFormat,
                                                               (p[PersistConst.Output])));
                    result.NeuronCount = SyntFileSection.ParseInt(p,
                                                                   PersistConst.NeuronCount);
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
            var hopfield = (HopfieldNetwork)obj;
            xout.AddSection("HOPFIELD");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(hopfield.Properties);
            xout.AddSubSection("NETWORK");
            xout.WriteProperty(PersistConst.Weights, hopfield.Weights);
            xout.WriteProperty(PersistConst.Output, hopfield.CurrentState.Data);
            xout.WriteProperty(PersistConst.NeuronCount, hopfield.NeuronCount);
            xout.Flush();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(HopfieldNetwork); }
        }

        #endregion
    }
}
