using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PersistSOM : ISyntPersistor
    {
        /**
	 * {@inheritDoc}
	 */

        #region ISyntPersistor Members

        public int FileVersion
        {
            get { return 1; }
        }

        /**
	 * {@inheritDoc}
	 */

        public String PersistClassString
        {
            get { return "SOMNetwork"; }
        }


        /**
	 * {@inheritDoc}
	 */

        public Object Read(Stream istream)
        {
            var result = new SOMNetwork();
            var reader = new SyntReadHelper(istream);
            SyntFileSection section;

            while ((section = reader.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("SOM")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> p = section.ParseParams();
                    EngineArray.PutAll(p, result.Properties);
                }
                if (section.SectionName.Equals("SOM")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();
                    result.Weights = SyntFileSection.ParseMatrix(p,
                                                                  PersistConst.Weights)
                        ;
                }
            }

            return result;
        }

        /**
	 * {@inheritDoc}
	 */

        public void Save(Stream os, Object obj)
        {
            var writer = new SyntWriteHelper(os);
            var som = (SOMNetwork)obj;
            writer.AddSection("SOM");
            writer.AddSubSection("PARAMS");
            writer.AddProperties(som.Properties);
            writer.AddSubSection("NETWORK");
            writer.WriteProperty(PersistConst.Weights, som.Weights);
            writer.WriteProperty(PersistConst.InputCount, som.InputCount);
            writer.WriteProperty(PersistConst.OutputCount, som.OutputCount);
            writer.Flush();
        }


        public Type NativeType
        {
            get { return typeof(SOMNetwork); }
        }

        #endregion
    }
}
