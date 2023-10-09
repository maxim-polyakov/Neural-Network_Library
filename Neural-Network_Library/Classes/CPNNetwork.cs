using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class CPNNetwork : BasicML, IMLRegression, IMLResettable, IMLError
    {
        /// <summary>
        /// The number of neurons in the input layer.
        /// </summary>
        ///
        private readonly int _inputCount;

        /// <summary>
        /// The number of neurons in the instar, or hidden, layer.
        /// </summary>
        ///
        private readonly int _instarCount;

        /// <summary>
        /// The number of neurons in the outstar, or output, layer.
        /// </summary>
        ///
        private readonly int _outstarCount;

        /// <summary>
        /// The weights from the input to the instar layer.
        /// </summary>
        ///
        private readonly Matrix _weightsInputToInstar;

        /// <summary>
        /// The weights from the instar to the outstar layer.
        /// </summary>
        ///
        private readonly Matrix _weightsInstarToOutstar;

        /// <summary>
        /// The number of winning neurons.
        /// </summary>
        ///
        private readonly int _winnerCount;

        /// <summary>
        /// Construct the counterpropagation neural network.
        /// </summary>
        ///
        /// <param name="theInputCount">The number of input neurons.</param>
        /// <param name="theInstarCount">The number of instar neurons.</param>
        /// <param name="theOutstarCount">The number of outstar neurons.</param>
        /// <param name="theWinnerCount">The winner count.</param>
        public CPNNetwork(int theInputCount, int theInstarCount,
                          int theOutstarCount, int theWinnerCount)
        {
            _inputCount = theInputCount;
            _instarCount = theInstarCount;
            _outstarCount = theOutstarCount;

            _weightsInputToInstar = new Matrix(_inputCount, _instarCount);
            _weightsInstarToOutstar = new Matrix(_instarCount, _outstarCount);
            _winnerCount = theWinnerCount;
        }


        /// <value>The instar count, same as the input count.</value>
        public int InstarCount
        {
            get { return _instarCount; }
        }


        /// <value>The outstar count, same as the output count.</value>
        public int OutstarCount
        {
            get { return _outstarCount; }
        }


        /// <value>The weights between the input and instar.</value>
        public Matrix WeightsInputToInstar
        {
            get { return _weightsInputToInstar; }
        }


        /// <value>The weights between the instar and outstar.</value>
        public Matrix WeightsInstarToOutstar
        {
            get { return _weightsInstarToOutstar; }
        }


        /// <value>The winner count.</value>
        public int WinnerCount
        {
            get { return _winnerCount; }
        }

        #region MLError Members

        /// <summary>
        /// Calculate the error for this neural network.
        /// </summary>
        ///
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(IMLDataSet data)
        {
            return SyntUtility.CalculateRegressionError(this, data);
        }

        #endregion

        #region MLRegression Members

        /// <inheritdoc/>
        public IMLData Compute(IMLData input)
        {
            IMLData temp = ComputeInstar(input);
            return ComputeOutstar(temp);
        }

        /// <inheritdoc/>
        public int InputCount
        {
            get { return _inputCount; }
        }

        /// <inheritdoc/>
        public int OutputCount
        {
            get { return _outstarCount; }
        }

        #endregion

        #region MLResettable Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Reset()
        {
            Reset(0);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Reset(int seed)
        {
            var randomize = new ConsistentRandomizer(-1, 1,
                                                     seed);
            randomize.Randomize(_weightsInputToInstar);
            randomize.Randomize(_weightsInstarToOutstar);
        }

        #endregion

        /// <summary>
        /// Compute the instar layer.
        /// </summary>
        ///
        /// <param name="input">The input.</param>
        /// <returns>The output.</returns>
        public IMLData ComputeInstar(IMLData input)
        {
            IMLData result = new BasicMLData(_instarCount);
            int w, i;
            int winner = 0;
            var winners = new bool[_instarCount];

            for (i = 0; i < _instarCount; i++)
            {
                double sum = 0;
                int j;
                for (j = 0; j < _inputCount; j++)
                {
                    sum += _weightsInputToInstar[j, i] * input[j];
                }
                result[i] = sum;
                winners[i] = false;
            }
            double sumWinners = 0;
            for (w = 0; w < _winnerCount; w++)
            {
                double maxOut = Double.MinValue;
                for (i = 0; i < _instarCount; i++)
                {
                    if (!winners[i] && (result[i] > maxOut))
                    {
                        winner = i;
                        maxOut = result[winner];
                    }
                }
                winners[winner] = true;
                sumWinners += result[winner];
            }
            for (i = 0; i < _instarCount; i++)
            {
                if (winners[i]
                    && (Math.Abs(sumWinners) > SyntFramework.DefaultDoubleEqual))
                {
                    result.Data[i] /= sumWinners;
                }
                else
                {
                    result.Data[i] = 0;
                }
            }

            return result;
        }

        /// <summary>
        /// Compute the outstar layer.
        /// </summary>
        ///
        /// <param name="input">The input.</param>
        /// <returns>The output.</returns>
        public IMLData ComputeOutstar(IMLData input)
        {
            IMLData result = new BasicMLData(_outstarCount);

            for (int i = 0; i < _outstarCount; i++)
            {
                double sum = 0;
                for (int j = 0; j < _instarCount; j++)
                {
                    sum += _weightsInstarToOutstar[j, i] * input[j];
                }
                result[i] = sum;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void UpdateProperties()
        {
            // unneeded
        }
    }
}
