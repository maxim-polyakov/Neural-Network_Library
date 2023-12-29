using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class ActivationLOG : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct the activation function.
        /// </summary>
        ///
        public ActivationLOG()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationLOG();
        }

        /// <returns>Return true, log has a derivative.</returns>
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
                if (x[i] >= 0)
                {
                    x[i] = BoundMath.Log(1 + x[i]);
                }
                else
                {
                    x[i] = -BoundMath.Log(1 - x[i]);
                }
            }
        }
        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            if (b >= 0)
            {
                return 1 / (1 + b);
            }
            return 1 / (1 - b);
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
