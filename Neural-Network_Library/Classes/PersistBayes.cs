﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PersistBayes : ISyntPersistor
    {
        /// <summary>
        /// The file version.
        /// </summary>
        public int FileVersion
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public Object Read(Stream istream)
        {
            BayesianNetwork result = new BayesianNetwork();
            SyntReadHelper input = new SyntReadHelper(istream);
            SyntFileSection section;
            String queryType = "";
            String queryStr = "";
            String contentsStr = "";

            while ((section = input.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("BAYES-NETWORK")
                        && section.SubSectionName.Equals("BAYES-PARAM"))
                {
                    IDictionary<String, String> p = section.ParseParams();
                    queryType = p["queryType"];
                    queryStr = p["query"];
                    contentsStr = p["contents"];
                }
                if (section.SectionName.Equals("BAYES-NETWORK")
                        && section.SubSectionName.Equals("BAYES-TABLE"))
                {

                    result.Contents = contentsStr;

                    // first, define relationships (1st pass)
                    foreach (String line in section.Lines)
                    {
                        result.DefineRelationship(line);
                    }

                    result.FinalizeStructure();

                    // now define the probabilities (2nd pass)
                    foreach (String line in section.Lines)
                    {
                        result.DefineProbability(line);
                    }
                }
                if (section.SectionName.Equals("BAYES-NETWORK")
                        && section.SubSectionName.Equals("BAYES-PROPERTIES"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
            }

            // define query, if it exists
            if (queryType.Length > 0)
            {
                IBayesianQuery query = null;
                if (queryType.Equals("EnumerationQuery"))
                {
                    query = new EnumerationQuery(result);
                }
                else
                {
                    query = new SamplingQuery(result);
                }

                if (query != null && queryStr.Length > 0)
                {
                    result.Query = query;
                    result.DefineClassificationStructure(queryStr);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            SyntWriteHelper o = new SyntWriteHelper(os);
            BayesianNetwork b = (BayesianNetwork)obj;
            o.AddSection("BAYES-NETWORK");
            o.AddSubSection("BAYES-PARAM");
            String queryType = "";
            String queryStr = b.ClassificationStructure;

            if (b.Query != null)
            {
                queryType = b.Query.GetType().Name;
            }

            o.WriteProperty("queryType", queryType);
            o.WriteProperty("query", queryStr);
            o.WriteProperty("contents", b.Contents);
            o.AddSubSection("BAYES-PROPERTIES");
            o.AddProperties(b.Properties);

            o.AddSubSection("BAYES-TABLE");
            foreach (BayesianEvent e in b.Events)
            {
                foreach (TableLine line in e.Table.Lines)
                {
                    if (line == null)
                        continue;
                    StringBuilder str = new StringBuilder();
                    str.Append("P(");

                    str.Append(BayesianEvent.FormatEventName(e, line.Result));

                    if (e.Parents.Count > 0)
                    {
                        str.Append("|");
                    }

                    int index = 0;
                    bool first = true;
                    foreach (BayesianEvent parentEvent in e.Parents)
                    {
                        if (!first)
                        {
                            str.Append(",");
                        }
                        first = false;
                        int arg = line.Arguments[index++];
                        if (parentEvent.IsBoolean)
                        {
                            if (arg == 0)
                            {
                                str.Append("+");
                            }
                            else
                            {
                                str.Append("-");
                            }
                        }
                        str.Append(parentEvent.Label);
                        if (!parentEvent.IsBoolean)
                        {
                            str.Append("=");
                            if (arg >= parentEvent.Choices.Count)
                            {
                                throw new BayesianError("Argument value " + arg + " is out of range for event " + parentEvent.ToString());
                            }
                            str.Append(parentEvent.GetChoice(arg));
                        }
                    }
                    str.Append(")=");
                    str.Append(line.Probability);
                    str.Append("\n");
                    o.Write(str.ToString());
                }
            }

            o.Flush();
        }

        /// <inheritdoc/>
        public String PersistClassString
        {
            get
            {
                return "BayesianNetwork";
            }
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(BayesianNetwork); }
        }

    }
}
