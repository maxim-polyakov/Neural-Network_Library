using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class ActivationBiPolar : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct the bipolar activation function.
        /// </summary>
        ///
        public ActivationBiPolar()
        {
            _paras = new double[0];
        }

        /// <inheritdoc/>
        public virtual double DerivativeFunction(double b, double a)
        {
            return 1;
        }


        /// <returns>Return true, bipolar has a 1 for derivative.</returns>
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
                if (x[i] > 0)
                {
                    x[i] = 1;
                }
                else
                {
                    x[i] = -1;
                }
            }
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { "slope" };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationBiPolar();
        }
    }
}
