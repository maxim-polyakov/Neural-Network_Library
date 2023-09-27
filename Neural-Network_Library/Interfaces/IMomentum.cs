using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMomentum
    {
        /// <summary>
        /// Set the momentum.
        /// </summary>
        double Momentum
        {
            get;
            set;
        }
    }
}
