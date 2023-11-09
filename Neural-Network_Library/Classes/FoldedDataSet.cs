using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class FoldedDataSet : IMLDataSet
    {
        /// <summary>
        /// Error message: adds are not supported.
        /// </summary>
        public const String AddNotSupported = "Direct adds to the folded dataset are not supported.";

        /// <summary>
        /// The underlying dataset.
        /// </summary>
        private readonly IMLDataSet _underlying;

        /// <summary>
        /// The fold that we are currently on.
        /// </summary>
        private int _currentFold;

        /// <summary>
        /// The offset to the current fold.
        /// </summary>
        private int _currentFoldOffset;

        /// <summary>
        /// The size of the current fold.
        /// </summary>
        private int _currentFoldSize;

        /// <summary>
        /// The size of all folds, except the last fold, the last fold may have a
        /// different number.
        /// </summary>
        private int _foldSize;

        /// <summary>
        /// The size of the last fold.
        /// </summary>
        private int _lastFoldSize;

        /// <summary>
        /// The total number of folds. Or 0 if the data has not been folded yet.
        /// </summary>
        private int _numFolds;

        /// <summary>
        /// Create a folded dataset. 
        /// </summary>
        /// <param name="underlying">The underlying folded dataset.</param>
        public FoldedDataSet(IMLDataSet underlying)
        {
            _underlying = underlying;
            Fold(1);
        }

        /// <summary>
        /// The owner object(from openAdditional)
        /// </summary>
        public FoldedDataSet Owner { get; set; }

        /// <summary>
        /// The current fold.
        /// </summary>
        public int CurrentFold
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.CurrentFold;
                }
                return _currentFold;
            }
            set
            {
                if (Owner != null)
                {
                    throw new TrainingError("Can't set the fold on a non-top-level set.");
                }

                if (value >= _numFolds)
                {
                    throw new TrainingError(
                        "Can't set the current fold to be greater than the number of folds.");
                }
                _currentFold = value;
                _currentFoldOffset = _foldSize * _currentFold;

                _currentFoldSize = _currentFold == (_numFolds - 1) ? _lastFoldSize : _foldSize;
            }
        }

        /// <summary>
        /// The current fold offset.
        /// </summary>
        public int CurrentFoldOffset
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.CurrentFoldOffset;
                }
                return _currentFoldOffset;
            }
        }

        /// <summary>
        /// The current fold size.
        /// </summary>
        public int CurrentFoldSize
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.CurrentFoldSize;
                }
                return _currentFoldSize;
            }
        }

        /// <summary>
        /// The number of folds.
        /// </summary>
        public int NumFolds
        {
            get { return _numFolds; }
        }

        /// <summary>
        /// The underlying dataset.
        /// </summary>
        public IMLDataSet Underlying
        {
            get { return _underlying; }
        }

        #region MLDataSet Members

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="data1">Not used.</param>
        public void Add(IMLData data1)
        {
            throw new TrainingError(AddNotSupported);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        /// <param name="idealData">Not used.</param>
        public void Add(IMLData inputData, IMLData idealData)
        {
            throw new TrainingError(AddNotSupported);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        public void Add(IMLDataPair inputData)
        {
            throw new TrainingError(AddNotSupported);
        }

        /// <summary>
        /// Close the dataset.
        /// </summary>
        public void Close()
        {
            _underlying.Close();
        }


        /// <summary>
        /// The ideal size.
        /// </summary>
        public int IdealSize
        {
            get { return _underlying.IdealSize; }
        }

        /// <summary>
        /// The input size.
        /// </summary>
        public int InputSize
        {
            get { return _underlying.InputSize; }
        }

        /// <summary>
        /// Get a record.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="pair">The record.</param>
        public void GetRecord(int index, IMLDataPair pair)
        {
            _underlying.GetRecord(CurrentFoldOffset + index, pair);
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int Count
        {
            get { return CurrentFoldSize; }
        }

        /// <summary>
        /// True if this is a supervised set.
        /// </summary>
        public bool Supervised
        {
            get { return _underlying.Supervised; }
        }


        /// <summary>
        /// Open an additional dataset.
        /// </summary>
        /// <returns>The dataset.</returns>
        public IMLDataSet OpenAdditional()
        {
            var folded = new FoldedDataSet(_underlying.OpenAdditional()) { Owner = this };
            return folded;
        }


        /// <summary>
        /// Get an enumberator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            return new FoldedEnumerator(this);
        }

        #endregion

        /// <summary>
        /// Fold the dataset. Must be done before the dataset is used. 
        /// </summary>
        /// <param name="numFolds">The number of folds.</param>
        public void Fold(int numFolds)
        {
            _numFolds = Math.Min(numFolds, _underlying
                                               .Count);
            _foldSize = _underlying.Count / _numFolds;
            _lastFoldSize = _underlying.Count - (_foldSize * _numFolds);
            CurrentFold = 0;
        }

        /// <inheritdoc/>
        public IMLDataPair this[int x]
        {
            get
            {
                IMLDataPair result = BasicMLDataPair.CreatePair(InputSize, IdealSize);
                this.GetRecord(x, result);
                return result;
            }
        }
    }
}
