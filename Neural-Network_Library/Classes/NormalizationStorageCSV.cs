using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class NormalizationStorageCSV : INormalizationStorage
    {
        /// <summary>
        /// The CSV format to use.
        /// </summary>
        private readonly CSVFormat _format;

        /// <summary>
        /// The output file.
        /// </summary> 
        private readonly String _outputFile;

        /// <summary>
        /// The output writer.
        /// </summary>
        private StreamWriter _output;

        /// <summary>
        /// Construct a CSV storage object from the specified file.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <param name="file">The file to write the CSV to.</param>
        public NormalizationStorageCSV(CSVFormat format, String file)
        {
            _format = format;
            _outputFile = file;
        }

        /// <summary>
        /// Construct a CSV storage object from the specified file.
        /// </summary>
        /// <param name="file">The file to write the CSV to.</param>
        public NormalizationStorageCSV(String file)
        {
            _format = CSVFormat.English;
            _outputFile = file;
        }

        #region INormalizationStorage Members

        /// <summary>
        /// Close the CSV file.
        /// </summary>
        public void Close()
        {
            _output.Close();
        }

        /// <summary>
        /// Open the CSV file.
        /// </summary>
        public void Open()
        {
            _output = new StreamWriter(_outputFile);
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount"> How much of the data is input.</param>
        public void Write(double[] data, int inputCount)
        {
            var result = new StringBuilder();
            NumberList.ToList(_format, result, data);
            _output.WriteLine(result.ToString());
        }

        #endregion
    }
}
