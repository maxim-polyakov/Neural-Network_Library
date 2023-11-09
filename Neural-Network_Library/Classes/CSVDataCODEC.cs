using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class CSVDataCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The external CSV file.
        /// </summary>
        private readonly String _file;

        /// <summary>
        /// The CSV format to use.
        /// </summary>
        private readonly CSVFormat _format;

        /// <summary>
        /// True, if headers are present in the CSV file.
        /// </summary>
        private readonly bool _headers;

        /// <summary>
        /// The size of the ideal data.
        /// </summary>
        private int _idealCount;

        /// <summary>
        /// The size of the input data.
        /// </summary>
        private int _inputCount;

        /// <summary>
        /// A file used to output the CSV file.
        /// </summary>
        private TextWriter _output;

        /// <summary>
        /// The utility to assist in reading the CSV file.
        /// </summary>
        private ReadCSV _readCSV;

        /// <summary>
        /// Should a significance column be expected.
        /// </summary>
        private bool _significance;

        /// <summary>
        /// Create a CODEC to load data from CSV to binary. 
        /// </summary>
        /// <param name="file">The CSV file to load.</param>
        /// <param name="format">The format that the CSV file is in.</param>
        /// <param name="headers">True, if there are headers.</param>
        /// <param name="inputCount">The number of input columns.</param>
        /// <param name="idealCount">The number of ideal columns.</param>
        /// <param name="significance">Is there a signficance column.</param>
        public CSVDataCODEC(
            String file,
            CSVFormat format,
            bool headers,
            int inputCount, int idealCount, bool significance)
        {
            if (_inputCount != 0)
            {
                throw new BufferedDataError(
                    "To export CSV, you must use the CSVDataCODEC constructor that does not specify input or ideal sizes.");
            }
            _file = file;
            _format = format;
            _inputCount = inputCount;
            _idealCount = idealCount;
            _headers = headers;
            _significance = significance;
        }

        /// <summary>
        /// Constructor to create CSV from binary.
        /// </summary>
        /// <param name="file">The CSV file to create.</param>
        /// <param name="format">The format for that CSV file.</param>
        public CSVDataCODEC(String file, CSVFormat format, bool significance)
        {
            _file = file;
            _format = format;
            _significance = significance;
        }

        #region IDataSetCODEC Members

        /// <inheritdoc/>
        public bool Read(double[] input, double[] ideal, ref double significance)
        {
            if (_readCSV.Next())
            {
                int index = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    input[i] = _readCSV.GetDouble(index++);
                }

                for (int i = 0; i < ideal.Length; i++)
                {
                    ideal[i] = _readCSV.GetDouble(index++);
                }

                if (_significance)
                {
                    significance = _readCSV.GetDouble(index++);
                }
                else
                {
                    significance = 1;
                }
                return true;
            }
            return false;
        }


        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
            if (_significance)
            {
                var record = new double[input.Length + ideal.Length + 1];
                EngineArray.ArrayCopy(input, record);
                EngineArray.ArrayCopy(ideal, 0, record, input.Length, ideal.Length);
                record[record.Length - 1] = significance;
                var result = new StringBuilder();
                NumberList.ToList(_format, result, record);
                _output.WriteLine(result.ToString());
            }
            else
            {
                var record = new double[input.Length + ideal.Length];
                EngineArray.ArrayCopy(input, record);
                EngineArray.ArrayCopy(ideal, 0, record, input.Length, ideal.Length);
                var result = new StringBuilder();
                NumberList.ToList(_format, result, record);
                _output.WriteLine(result.ToString());
            }
        }

        /// <summary>
        /// Prepare to write to a CSV file. 
        /// </summary>
        /// <param name="recordCount">The total record count, that will be written.</param>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size.</param>
        public void PrepareWrite(
            int recordCount,
            int inputSize,
            int idealSize)
        {
            try
            {
                _inputCount = inputSize;
                _idealCount = idealSize;
                _output = new StreamWriter(new FileStream(_file, FileMode.Create));
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Prepare to read from the CSV file.
        /// </summary>
        public void PrepareRead()
        {
            if (_inputCount == 0)
            {
                throw new BufferedDataError(
                    "To import CSV, you must use the CSVDataCODEC constructor that specifies input and ideal sizes.");
            }
            _readCSV = new ReadCSV(_file, _headers,
                                  _format);
        }

        /// <inheritDoc/>
        public int InputSize
        {
            get { return _inputCount; }
        }

        /// <inheritDoc/>
        public int IdealSize
        {
            get { return _idealCount; }
        }

        /// <inheritDoc/>
        public void Close()
        {
            if (_readCSV != null)
            {
                _readCSV.Close();
                _readCSV = null;
            }

            if (_output != null)
            {
                _output.Close();
                _output = null;
            }
        }

        #endregion
    }
}
