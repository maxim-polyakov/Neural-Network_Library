using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface ICrossover
    {
        /// <summary>
        /// Mate two Qs.
        /// </summary>
        ///
        /// <param name="mother">The mother.</param>
        /// <param name="father">The father.</param>
        /// <param name="offspring1">The first offspring.</param>
        /// <param name="offspring2">The second offspring.</param>
        void Mate(Q mother, Q father,
                  Q offspring1, Q offspring2);
    }
}
