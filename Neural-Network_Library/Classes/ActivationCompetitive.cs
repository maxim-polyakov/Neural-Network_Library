using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class ActivationCompetitive : IActivationFunction
    {
        /// <summary>
        /// The offset to the parameter that holds the max winners.
        /// </summary>
        ///
        public const int ParamCompetitiveMaxWinners = 0;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Create a competitive activation function with one winner allowed.
        /// </summary>
        ///
        public ActivationCompetitive()
            : this(1)
        {
        }

        /// <summary>
        /// Create a competitive activation function with the specified maximum
        /// number of winners.
        /// </summary>
        ///
        /// <param name="winners">The maximum number of winners that this function supports.</param>
        public ActivationCompetitive(int winners)
        {
            _paras = new double[1];
            _paras[ParamCompetitiveMaxWinners] = winners;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            var winners = new bool[x.Length];
            double sumWinners = 0;

            // find the desired number of winners
            for (int i = 0; i < _paras[0]; i++)
            {
                double maxFound = Double.NegativeInfinity;
                int winner = -1;

                // find one winner
                for (int j = start; j < start + size; j++)
                {
                    if (!winners[j] && (x[j] > maxFound))
                    {
                        winner = j;
                        maxFound = x[j];
                    }
                }
                sumWinners += maxFound;
                winners[winner] = true;
            }

            // adjust weights for winners and non-winners
            for (int i = start; i < start + size; i++)
            {
                if (winners[i])
                {
                    x[i] = x[i] / sumWinners;
                }
                else
                {
                    x[i] = 0.0d;
                }
            }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        object ICloneable.Clone()
        {
            return new ActivationCompetitive(
                (int)_paras[ParamCompetitiveMaxWinners]);
        }

        /// <inheritdoc/>
        public virtual double DerivativeFunction(double b, double a)
        {
            throw new NeuralNetworkError(
                "Can't use the competitive activation function "
                + "where a derivative is required.");
        }


        /// <summary>
        /// The maximum number of winners this function supports.
        /// </summary>
        public int MaxWinners
        {
            get { return (int)_paras[ParamCompetitiveMaxWinners]; }
        }


        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { "maxWinners" };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }


        /// <returns>False, indication that no derivative is available for thisfunction.</returns>
        public virtual bool HasDerivative()
        {
            return false;
        }
    }
}
