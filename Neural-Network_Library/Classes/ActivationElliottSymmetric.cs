using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ActivationElliottSymmetric : IActivationFunction
    {
        /// <summary>
        /// The params.
        /// </summary>
        private readonly double[] _p;

        /// <summary>
        /// Construct a basic Elliott activation function, with a slope of 1.
        /// </summary>
        public ActivationElliottSymmetric()
        {
            _p = new double[1];
            _p[0] = 1.0;
        }

        #region IActivationFunction Members

        /// <inheritdoc />
        public void ActivationFunction(double[] x, int start,
                                       int size)
        {
            for (int i = start; i < start + size; i++)
            {
                double s = _p[0];
                x[i] = (x[i] * s) / (1 + Math.Abs(x[i] * s));
            }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The object to be cloned.</returns>
        public object Clone()
        {
            return new ActivationElliottSymmetric();
        }

        /// <inheritdoc />
        public double DerivativeFunction(double b, double a)
        {
            double s = _p[0];
            double d = (1.0 + Math.Abs(b * s));
            return (s * 1.0) / (d * d);
        }

        /// <inheritdoc />
        public String[] ParamNames
        {
            get
            {
                String[] result = { "Slope" };
                return result;
            }
        }

        /// <inheritdoc />
        public double[] Params
        {
            get { return _p; }
        }

        /// <summary>
        /// Return true, Elliott activation has a derivative.
        /// </summary>
        /// <returns>Return true, Elliott activation has a derivative.</returns>
        public bool HasDerivative()
        {
            return true;
        }

        #endregion

        /// <inheritdoc />
        public void SetParam(int index, double value)
        {
            _p[index] = value;
        }
    }
}
