using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class FoldedEnumerator : IEnumerator<IMLDataPair>
    {
        /// <summary>
        /// The owner.
        /// </summary>
        private readonly FoldedDataSet _owner;

        /// <summary>
        /// The current index.
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// The current data item.
        /// </summary>
        private IMLDataPair _currentPair;

        /// <summary>
        /// Construct an enumerator.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public FoldedEnumerator(FoldedDataSet owner)
        {
            _owner = owner;
            _currentIndex = -1;
        }

        #region IEnumerator<MLDataPair> Members

        /// <summary>
        /// The current object.
        /// </summary>
        public IMLDataPair Current
        {
            get
            {
                if (_currentIndex < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return _currentPair;
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Move to the next record.
        /// </summary>
        /// <returns>True, if we were able to move to the next record.</returns>
        public bool MoveNext()
        {
            if (HasNext())
            {
                IMLDataPair pair = BasicMLDataPair.CreatePair(
                    _owner.InputSize, _owner.IdealSize);
                _owner.GetRecord(_currentIndex++, pair);
                _currentPair = pair;
                return true;
            }
            _currentPair = null;
            return false;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The current object.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                if (_currentIndex < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return _currentPair;
            }
        }

        #endregion

        /// <summary>
        /// Determine if there is a next record.
        /// </summary>
        /// <returns>True, if there is a next record.</returns>
        public bool HasNext()
        {
            return _currentIndex < _owner.CurrentFoldSize;
        }
    }
}
