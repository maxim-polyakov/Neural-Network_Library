using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class ActivationSIN : IActivationFunction
    {
        /// <summary>
        /// Construct the sin activation function.
        /// </summary>
        ///
        public ActivationSIN()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationSIN();
        }


        /// <returns>Return true, sin has a derivative.</returns>
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
                x[i] = BoundMath.Sin(x[i]);
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            return BoundMath.Cos(b);
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { };
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
