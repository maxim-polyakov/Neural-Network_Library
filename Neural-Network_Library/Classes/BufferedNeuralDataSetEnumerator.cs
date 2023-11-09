using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BufferedNeuralDataSetEnumerator : IEnumerator<IMLDataPair>
    {
        /// <summary>
        /// The dataset being iterated over.
        /// </summary>
        private readonly BufferedMLDataSet _data;

        /// <summary>
        /// The current record.
        /// </summary>
        private int _current;

        /// <summary>
        /// The current record.
        /// </summary>
        private IMLDataPair _currentRecord;

        /// <summary>
        /// Construct the buffered enumerator. This is where the file is actually
        /// opened.
        /// </summary>
        /// <param name="owner">The object that created this enumeration.</param>
        public BufferedNeuralDataSetEnumerator(BufferedMLDataSet owner)
        {
            _data = owner;
            _current = 0;
        }

        #region IEnumerator<MLDataPair> Members

        /// <summary>
        /// Get the current record
        /// </summary>
        public IMLDataPair Current
        {
            get { return _currentRecord; }
        }

        /// <summary>
        /// Dispose of the enumerator.
        /// </summary>
        public void Dispose()
        {
        }


        object IEnumerator.Current
        {
            get
            {
                if (_currentRecord == null)
                {
                    throw new IMLDataError("Can't read current record until MoveNext is called once.");
                }
                return _currentRecord;
            }
        }

        /// <summary>
        /// Move to the next element.
        /// </summary>
        /// <returns>True if there are more elements to read.</returns>
        public bool MoveNext()
        {
            try
            {
                if (_current >= _data.Count)
                    return false;

                _currentRecord = BasicMLDataPair.CreatePair(_data
                                                               .InputSize, _data.IdealSize);
                _data.GetRecord(_current++, _currentRecord);
                return true;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Close the enumerator, and the underlying file.
        /// </summary>
        public void Close()
        {
        }
    }
}
