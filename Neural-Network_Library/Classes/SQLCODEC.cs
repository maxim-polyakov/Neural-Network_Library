using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SQLCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The database connection.
        /// </summary>
        private readonly DbConnection _connection;

        /// <summary>
        /// What is the size of the ideal data?
        /// </summary>
        private readonly int _idealSize;

        /// <summary>
        /// What is the size of the input data?
        /// </summary>
        private readonly int _inputSize;

        /// <summary>
        /// The SQL statement being used.
        /// </summary>
        private readonly DbCommand _statement;

        /// <summary>
        /// Holds results from the SQL query.
        /// </summary>
        private DbDataReader _results;

        /// <summary>
        /// Create a SQL neural data set.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="inputSize">The size of the input data being read.</param>
        /// <param name="idealSize">The size of the ideal output data being read.</param>
        /// <param name="connectString">The connection string.</param>
        public SQLCODEC(String sql, int inputSize,
                        int idealSize, String connectString)
        {
            _inputSize = inputSize;
            _idealSize = idealSize;
            _connection = new OleDbConnection(connectString);
            _connection.Open();
            _statement = _connection.CreateCommand();
            _statement.CommandText = sql;
            _statement.Prepare();
            _statement.Connection = _connection;
        }

        #region IDataSetCODEC Members

        /// <summary>
        /// Read a record.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="ideal">The ideal data.</param>
        /// <returns></returns>
        public bool Read(double[] input, double[] ideal, ref double significance)
        {
            if (!_results.NextResult())
                return false;

            for (int i = 1; i <= _inputSize; i++)
            {
                input[i - 1] = _results.GetDouble(i);
            }

            if (_idealSize > 0)
            {
                for (int i = 1; i <= _idealSize; i++)
                {
                    ideal[i - 1] =
                        _results.GetDouble(i + _inputSize);
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prepare to write.
        /// </summary>
        /// <param name="recordCount">The record count.</param>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size.</param>
        public void PrepareWrite(int recordCount, int inputSize, int idealSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prepare to read.
        /// </summary>
        public void PrepareRead()
        {
            _results = _statement.ExecuteReader();
        }

        /// <summary>
        /// The input size.
        /// </summary>
        public int InputSize
        {
            get { return _inputSize; }
        }

        /// <summary>
        /// The ideal size.
        /// </summary>
        public int IdealSize
        {
            get { return _idealSize; }
        }

        /// <summary>
        /// Close the codec.
        /// </summary>
        public void Close()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
            if (_results != null)
            {
                _results.Close();
            }
        }

        #endregion
    }
}
