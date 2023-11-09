using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class ActivationSigmoid : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct a basic sigmoid function, with a slope of 1.
        /// </summary>
        ///
        public ActivationSigmoid()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationSigmoid();
        }

        /// <returns>True, sigmoid has a derivative.</returns>
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
                x[i] = 1.0d / (1.0d + BoundMath.Exp(-1 * x[i]));
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            return a * (1.0d - a);
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] results = { };
                return results;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }


    }
}
