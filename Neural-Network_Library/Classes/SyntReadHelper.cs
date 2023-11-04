using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SyntReadHelper
    {
        /// <summary>
        /// The lines read from the file.
        /// </summary>
        ///
        private readonly IList<String> lines;

        /// <summary>
        /// The file being read.
        /// </summary>
        ///
        private readonly TextReader reader;

        /// <summary>
        /// The current section name.
        /// </summary>
        ///
        private String currentSectionName;

        /// <summary>
        /// The current subsection name.
        /// </summary>
        ///
        private String currentSubSectionName;

        /// <summary>
        /// The current section name.
        /// </summary>
        ///
        private SyntFileSection section;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="mask0">The input stream.</param>
        public SyntReadHelper(Stream mask0)
        {
            lines = new List<String>();
            currentSectionName = "";
            currentSubSectionName = "";
            reader = new StreamReader(mask0);
        }

        /// <summary>
        /// Close the file.
        /// </summary>
        ///
        public void Close()
        {
            try
            {
                reader.Close();
            }
            catch (IOException e)
            {
                throw new PersistError(e);
            }
        }

        /// <summary>
        /// Read the next section.
        /// </summary>
        ///
        /// <returns>The next section.</returns>
        public SyntFileSection ReadNextSection()
        {
            try
            {
                String line;
                var largeArrays = new List<double[]>();

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    // is it a comment
                    if (line.StartsWith("//"))
                    {
                        continue;
                    }

                    // is it a section or subsection
                    else if (line.StartsWith("["))
                    {
                        // handle previous section
                        section = new SyntFileSection(
                            currentSectionName, currentSubSectionName);

                        foreach (String str in lines)
                        {
                            section.Lines.Add(str);
                        }


                        // now begin the new section
                        lines.Clear();
                        String s = line.Substring(1).Trim();
                        if (!s.EndsWith("]"))
                        {

                        }
                        s = s.Substring(0, (line.Length - 2) - (0));
                        int idx = s.IndexOf(':');
                        if (idx == -1)
                        {
                            currentSectionName = s;
                            currentSubSectionName = "";
                        }
                        else
                        {
                            if (currentSectionName.Length < 1)
                            {

                            }

                            String newSection = s.Substring(0, (idx) - (0));
                            String newSubSection = s.Substring(idx + 1);

                            if (!newSection.Equals(currentSectionName))
                            {

                            }

                            currentSubSectionName = newSubSection;
                        }
                        section.LargeArrays = largeArrays;
                        return section;
                    }
                    else if (line.Length < 1)
                    {
                        continue;
                    }
                    else if (line.StartsWith("##double"))
                    {
                        double[] d = ReadLargeArray(line);
                        largeArrays.Add(d);
                    }
                    else
                    {
                        if (currentSectionName.Length < 1)
                        {

                        }

                        lines.Add(line);
                    }
                }

                if (currentSectionName.Length == 0)
                {
                    return null;
                }

                section = new SyntFileSection(currentSectionName,
                                               currentSubSectionName);

                foreach (String l in lines)
                {
                    section.Lines.Add(l);
                }

                currentSectionName = "";
                currentSubSectionName = "";
                section.LargeArrays = largeArrays;
                return section;
            }
            catch (IOException ex)
            {
                throw new PersistError(ex);
            }
        }

        /// <summary>
        /// Called internally to read a large array.
        /// </summary>
        /// <param name="line">The line containing the beginning of a large array.</param>
        /// <returns>The array read.</returns>
        private double[] ReadLargeArray(String line)
        {
            String str = line.Substring(9);
            int l = int.Parse(str);
            double[] result = new double[l];

            int index = 0;
            while ((line = this.reader.ReadLine()) != null)
            {
                line = line.Trim();

                // is it a comment
                if (line.StartsWith("//"))
                {
                    continue;
                }
                else if (line.StartsWith("##end"))
                {
                    break;
                }

                double[] t = NumberList.FromList(CSVFormat.EgFormat, line);
                EngineArray.ArrayCopy(t, 0, result, index, t.Length);
                index += t.Length;
            }

            return result;
        }
    }
}
