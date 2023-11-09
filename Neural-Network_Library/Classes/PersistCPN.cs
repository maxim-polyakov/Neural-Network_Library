using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PersistCPN : ISyntPersistor
    {
        /// <summary>
        /// The input to instar property.
        /// </summary>
        ///
        internal const String PropertyInputToInstar = "inputToInstar";

        /// <summary>
        /// The instar to input property.
        /// </summary>
        ///
        internal const String PropertyInstarToInput = "instarToInput";

        /// <summary>
        /// The winner count property.
        /// </summary>
        ///
        internal const String PropertyWinnerCount = "winnerCount";

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(CPNNetwork); }
        }

        #region SyntPersistor Members

        /// <inheritdoc/>
        public int FileVersion
        {
            get { return 1; }
        }


        /// <inheritdoc/>
        public String PersistClassString
        {
            get { return "CPN"; }
        }


        /// <inheritdoc/>
        public Object Read(Stream mask0)
        {
            IDictionary<String, String> networkParams = null;
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;
            int inputCount = 0;
            int instarCount = 0;
            int outputCount = 0;
            int winnerCount = 0;
            Matrix m1 = null;
            Matrix m2 = null;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("CPN")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    networkParams = section.ParseParams();
                }
                if (section.SectionName.Equals("CPN")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    inputCount = SyntFileSection.ParseInt(paras,
                                                           PersistConst.InputCount);
                    instarCount = SyntFileSection.ParseInt(paras,
                                                            PersistConst.Instar);
                    outputCount = SyntFileSection.ParseInt(paras,
                                                            PersistConst.OutputCount);
                    winnerCount = SyntFileSection.ParseInt(paras,
                                                            PropertyWinnerCount);
                    m1 = SyntFileSection.ParseMatrix(paras,
                                                      PropertyInputToInstar);
                    m2 = SyntFileSection.ParseMatrix(paras,
                                                      PropertyInstarToInput);
                }
            }

            var result = new CPNNetwork(inputCount, instarCount, outputCount,
                                        winnerCount);
            EngineArray.PutAll(networkParams, result.Properties);
            result.WeightsInputToInstar.Set(m1);
            result.WeightsInstarToOutstar.Set(m2);
            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var cpn = (CPNNetwork)obj;
            xout.AddSection("CPN");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(cpn.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.InputCount, cpn.InputCount);
            xout.WriteProperty(PersistConst.Instar, cpn.InstarCount);
            xout.WriteProperty(PersistConst.OutputCount, cpn.OutputCount);
            xout.WriteProperty(PropertyInputToInstar,
                               cpn.WeightsInputToInstar);
            xout.WriteProperty(PropertyInstarToInput,
                               cpn.WeightsInstarToOutstar);
            xout.WriteProperty(PropertyWinnerCount, cpn.WinnerCount);

            xout.Flush();
        }

        #endregion
    }
}
