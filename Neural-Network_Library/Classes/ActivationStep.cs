using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class ActivationStep : IActivationFunction
    {
        /// <summary>
        /// The step center parameter.
        /// </summary>
        ///
        public const int ParamStepCenter = 0;

        /// <summary>
        /// The step low parameter.
        /// </summary>
        ///
        public const int ParamStepLow = 1;

        /// <summary>
        /// The step high parameter.
        /// </summary>
        ///
        public const int ParamStepHigh = 2;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct a step activation function.
        /// </summary>
        ///
        /// <param name="low">The low of the function.</param>
        /// <param name="center">The center of the function.</param>
        /// <param name="high">The high of the function.</param>
        public ActivationStep(double low, double center, double high)
        {
            _paras = new double[3];
            _paras[ParamStepCenter] = center;
            _paras[ParamStepLow] = low;
            _paras[ParamStepHigh] = high;
        }

        /// <summary>
        /// Create a basic step activation with low=0, center=0, high=1.
        /// </summary>
        ///
        public ActivationStep()
            : this(0.0d, 0.0d, 1.0d)
        {
        }

        /// <summary>
        /// Set the center of this function.
        /// </summary>
        public double Center
        {
            get { return _paras[ParamStepCenter]; }
            set { _paras[ParamStepCenter] = value; }
        }


        /// <summary>
        /// Set the low of this function.
        /// </summary>
        public double Low
        {
            get { return _paras[ParamStepLow]; }
            set { _paras[ParamStepLow] = value; }
        }


        /// <summary>
        /// Set the high of this function.
        /// </summary>
        public double High
        {
            get { return _paras[ParamStepHigh]; }
            set { _paras[ParamStepHigh] = value; }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            var result = new ActivationStep(Low, Center,
                                            High);
            return result;
        }

        /// <returns>Returns true, this activation function has a derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            for (int i = start; i < start + size; i++)
            {
                if (x[i] >= _paras[ParamStepCenter])
                {
                    x[i] = _paras[ParamStepHigh];
                }
                else
                {
                    x[i] = _paras[ParamStepLow];
                }
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            return 1.0d;
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { "center", "low", "high" };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }

    }
}
