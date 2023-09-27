using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMultiThreadable
    {
        /// <summary>
        /// The thread count.  Set to zero to automatically determine the 
        /// thread count based on cores.  Set to 1 to specify single threaded.
        /// </summary>
        int ThreadCount { get; set; }
    }
}
