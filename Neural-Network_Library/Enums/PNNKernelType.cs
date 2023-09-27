using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public enum PNNKernelType
    {
        /// <summary>
        /// A Gaussian curved kernel. The usual choice.
        /// </summary>
        ///
        Gaussian,

        /// <summary>
        /// A steep kernel.
        /// </summary>
        ///
        Reciprocal
    }
}
