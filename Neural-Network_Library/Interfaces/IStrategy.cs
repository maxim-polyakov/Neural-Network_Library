using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IStrategy
    {
        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train">The training algorithm.</param>
        void Init(IMLTrain train);

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        ///
        void PreIteration();

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        void PostIteration();
    }
}
