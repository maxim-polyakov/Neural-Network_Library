using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class CSVMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// The CSV filename to read from.
        /// </summary>
        private readonly String _filename;

        /// <summary>
        /// The format that separates the columns, defaults to a comma.
        /// </summary>
        private readonly CSVFormat _format;

        /// <summary>
        /// Specifies if headers are present on the first row.
        /// </summary>
        private readonly bool _headers;

        /// <summary>
        /// The number of columns of ideal data.
        /// </summary>
        private readonly int _idealSize;

        /// <summary>
        /// The number of columns of input data.
        /// </summary>
        private readonly int _inputSize;

        /// <summary>
        /// Construct this data set using a comma as a delimiter.
        /// </summary>
        /// <param name="filename">The CSV filename to read.</param>
        /// <param name="inputSize">The number of columns that make up the input set.</param>
        /// <param name="idealSize">The number of columns that make up the ideal set.</param>
        /// <param name="headers">True if headers are present on the first line.</param>
        public CSVMLDataSet(String filename, int inputSize,
                            int idealSize, bool headers)
            : this(filename, inputSize, idealSize, headers, CSVFormat.English, false)
        {
        }

        /// <summary>
        /// Construct this data set using a comma as a delimiter.
        /// </summary>
        /// <param name="filename">The CSV filename to read.</param>
        /// <param name="inputSize">The number of columns that make up the input set.</param>
        /// <param name="idealSize">The number of columns that make up the ideal set.</param>
        /// <param name="headers">True if headers are present on the first line.</param>
        /// <param name="format">The format to use.</param>
        public CSVMLDataSet(String filename, int inputSize,
                            int idealSize, bool headers, CSVFormat format, bool expectSignificance)
        {
            _filename = filename;
            _inputSize = inputSize;
            _idealSize = idealSize;
            _format = format;
            _headers = headers;

            IDataSetCODEC codec = new CSVDataCODEC(filename, format, headers, inputSize, idealSize, expectSignificance);
            var load = new MemoryDataLoader(codec) { Result = this };
            load.External2Memory();
        }

        /// <summary>
        /// Get the filename for the CSV file.
        /// </summary>
        public String Filename
        {
            get { return _filename; }
        }

        /// <summary>
        /// The delimiter.
        /// </summary>
        public CSVFormat Format
        {
            get { return _format; }
        }

        /// <summary>
        /// True if the first row specifies field names.
        /// </summary>
        public bool Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// The amount of ideal data.
        /// </summary>
        public override int IdealSize
        {
            get { return _idealSize; }
        }

        /// <summary>
        /// The amount of input data.
        /// </summary>
        public override int InputSize
        {
            get { return _inputSize; }
        }
    }
}
