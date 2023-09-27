using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IActivationFunction : ICloneable
    {
        /// <returns>The params for this activation function.</returns>
        double[] Params { get; }

        /// <returns>The names of the parameters.</returns>
        String[] ParamNames { get; }


        void ActivationFunction(double[] d, int start, int size);


        double DerivativeFunction(double b, double a);


        /// <returns>Return true if this function has a derivative.</returns>
        bool HasDerivative();
    }
}
