using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class NormalizationStorageMLDataSet : INormalizationStorage
    {
        /// <summary>
        /// The data set to add to.
        /// </summary>
        [NonSerialized]
        private readonly IMLDataSet _dataset;

        /// <summary>
        /// The ideal count.
        /// </summary>
        private readonly int _idealCount;

        /// <summary>
        /// The input count.
        /// </summary>
        private readonly int _inputCount;

        /// <summary>
        /// Construct a new NeuralDataSet based on the parameters specified.
        /// </summary>
        /// <param name="inputCount">The input count.</param>
        /// <param name="idealCount">The output count.</param>
        public NormalizationStorageMLDataSet(int inputCount,
                                                 int idealCount)
        {
            _inputCount = inputCount;
            _idealCount = idealCount;
            _dataset = new BasicMLDataSet();
        }

        /// <summary>
        /// Construct a normalized neural storage class to hold data.
        /// </summary>
        /// <param name="dataset">The data set to store to. This uses an existing data set.</param>
        public NormalizationStorageMLDataSet(IMLDataSet dataset)
        {
            _dataset = dataset;
            _inputCount = _dataset.InputSize;
            _idealCount = _dataset.IdealSize;
        }

        /// <summary>
        /// The data set being used.
        /// </summary>
        public IMLDataSet DataSet
        {
            get { return _dataset; }
        }

        #region INormalizationStorage Members

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Open()
        {
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount">How much of the data is input.</param>
        public void Write(double[] data, int inputCount)
        {
            if (_idealCount == 0)
            {
                var inputData = new BasicMLData(data);
                _dataset.Add(inputData);
            }
            else
            {
                var inputData = new BasicMLData(
                    _inputCount);
                var idealData = new BasicMLData(
                    _idealCount);

                int index = 0;
                for (int i = 0; i < _inputCount; i++)
                {
                    inputData[i] = data[index++];
                }

                for (int i = 0; i < _idealCount; i++)
                {
                    idealData[i] = data[index++];
                }

                _dataset.Add(inputData, idealData);
            }
        }

        #endregion
    }
}
