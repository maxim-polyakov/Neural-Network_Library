using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class InputFieldCSV : BasicInputField
    {

        /// <summary>
        /// The file to read.
        /// </summary>
        private readonly String _file;

        /// <summary>
        /// The CSV column represented by this field.
        /// </summary>
        private readonly int _offset;


        private readonly string _columnName;
        /// <summary>
        /// Construct an InputFieldCSV with the default constructor.  This is mainly
        /// used for reflection.
        /// </summary>
        public InputFieldCSV()
        {
        }




        /// <summary>
        /// Construct a input field for a CSV file.
        /// </summary>
        /// <param name="usedForNetworkInput">True if this field is used for actual 
        /// input to the neural network, as opposed to segregation only.</param>
        /// <param name="file">The tile to read.</param>
        /// <param name="offset">The CSV file column to read.</param>
        public InputFieldCSV(bool usedForNetworkInput, String file,
                             int offset)
        {
            _file = file;
            _offset = offset;
            UsedForNetworkInput = usedForNetworkInput;
        }

        /// <summary>
        /// Construct a input field for a CSV file.
        /// </summary>
        /// <param name="usedForNetworkInput">True if this field is used for actual
        /// input to the neural network, as opposed to segregation only.</param>
        /// <param name="file">The tile to read.</param>
        /// <param name="columnname">The columnname you wish to read.</param>
        public InputFieldCSV(bool usedForNetworkInput, String file,
                             string columnname)
        {
            _file = file;
            _columnName = columnname;

            UsedForNetworkInput = usedForNetworkInput;
        }
        /// <summary>
        /// The file being read.
        /// </summary>
        public String File
        {
            get { return _file; }
        }

        /// <summary>
        /// The column in this CSV file to read.
        /// </summary>
        public int Offset
        {
            get { return _offset; }
        }

        /// <summary>
        /// Gets the name of the column we want to read.
        /// </summary>
        /// <value>
        /// The name of the column we want to read.
        /// </value>
        public string ColumnName
        {
            get { return _columnName; }
        }
    }
}
