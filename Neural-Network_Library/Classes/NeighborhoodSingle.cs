using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NeighborhoodSingle : INeighborhoodFunction
    {
        #region INeighborhoodFunction Members

        /// <summary>
        /// Determine how much the current neuron should be affected by training
        /// based on its proximity to the winning neuron.
        /// </summary>
        ///
        /// <param name="currentNeuron">THe current neuron being evaluated.</param>
        /// <param name="bestNeuron">The winning neuron.</param>
        /// <returns>The ratio for this neuron's adjustment.</returns>
        public virtual double Function(int currentNeuron, int bestNeuron)
        {
            if (currentNeuron == bestNeuron)
            {
                return 1.0d;
            }
            return 0.0d;
        }

        /// <summary>
        /// Set the radius.  This type does not use a radius, so this has no effect.
        /// </summary>
        ///
        /// <value>The radius.</value>
        public virtual double Radius
        {
            get { return 1; }
            set
            {
                // no effect on this type
            }
        }

        #endregion
    }
}
