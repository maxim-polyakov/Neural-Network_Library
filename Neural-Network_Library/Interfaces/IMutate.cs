using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMutate
    {
        /// <summary>
        /// Perform a mutation on the specified Q.
        /// </summary>
        ///
        /// <param name="Q">The Q to mutate.</param>
        void PerformMutation(Q Q);
    }
}
