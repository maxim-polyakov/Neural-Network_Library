using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NeuralDataSetCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The dataset.
        /// </summary>
        private readonly IMLDataSet _dataset;

        /// <summary>
        /// The iterator used to read through the dataset.
        /// </summary>
        private IEnumerator<IMLDataPair> _enumerator;

        /// <summary>
        /// The number of ideal elements.
        /// </summary>
        private int _idealSize;

        /// <summary>
        /// The number of input elements.
        /// </summary>
        private int _inputSize;

        /// <summary>
        /// Construct a CODEC. 
        /// </summary>
        /// <param name="dataset">The dataset to use.</param>
        public NeuralDataSetCODEC(IMLDataSet dataset)
        {
            _dataset = dataset;
            _inputSize = dataset.InputSize;
            _idealSize = dataset.IdealSize;
        }

        #region IDataSetCODEC Members

        /// <inheritdoc/>
        public int InputSize
        {
            get { return _inputSize; }
        }

        /// <inheritdoc/>
        public int IdealSize
        {
            get { return _idealSize; }
        }

        /// <inheritdoc/>
        public bool Read(double[] input, double[] ideal, ref double significance)
        {
            if (!_enumerator.MoveNext())
            {
                return false;
            }
            else
            {
                IMLDataPair pair = _enumerator.Current;
                EngineArray.ArrayCopy(pair.Input.Data, input);
                EngineArray.ArrayCopy(pair.Ideal.Data, ideal);
                significance = pair.Significance;
                return true;
            }
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
            IMLDataPair pair = BasicMLDataPair.CreatePair(_inputSize,
                                                         _idealSize);
            EngineArray.ArrayCopy(input, pair.Input.Data);
            EngineArray.ArrayCopy(ideal, pair.Ideal.Data);
            pair.Significance = significance;
        }

        /// <inheritdoc/>
        public void PrepareWrite(int recordCount,
                                 int inputSize, int idealSize)
        {
            _inputSize = inputSize;
            _idealSize = idealSize;
        }

        /// <inheritdoc/>
        public void PrepareRead()
        {
            _enumerator = _dataset.GetEnumerator();
        }

        /// <inheritdoc/>
        public void Close()
        {
        }

        #endregion
    }
}
