using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public enum KernelType
    {
        /// <summary>
        /// Linear kernel.
        /// </summary>
        ///
        Linear,

        /// <summary>
        /// Poly kernel.
        /// </summary>
        ///
        Poly,

        /// <summary>
        /// Radial basis function kernel.
        /// </summary>
        ///
        RadialBasisFunction,

        /// <summary>
        /// Sigmoid kernel.
        /// </summary>
        ///
        Sigmoid,

        /// <summary>
        /// Precomputed kernel.
        /// </summary>
        ///
        Precomputed
    }
}
