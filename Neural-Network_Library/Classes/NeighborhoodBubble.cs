using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NeighborhoodBubble : INeighborhoodFunction
    {
        /// <summary>
        /// The radius of the bubble.
        /// </summary>
        ///
        private double _radius;

        /// <summary>
        /// Create a bubble neighborhood function that will return 1.0 (full update)
        /// for any neuron that is plus or minus the width distance from the winning
        /// neuron.
        /// </summary>
        ///
        /// <param name="radius">bubble, is actually two times this parameter.</param>
        public NeighborhoodBubble(int radius)
        {
            _radius = radius;
        }

        #region INeighborhoodFunction Members

        /// <summary>
        /// Determine how much the current neuron should be affected by training
        /// based on its proximity to the winning neuron.
        /// </summary>
        ///
        /// <param name="currentNeuron">THe current neuron being evaluated.</param>
        /// <param name="bestNeuron">The winning neuron.</param>
        /// <returns>The ratio for this neuron's adjustment.</returns>
        public double Function(int currentNeuron, int bestNeuron)
        {
            int distance = Math.Abs(bestNeuron - currentNeuron);
            if (distance <= _radius)
            {
                return 1.0d;
            }
            return 0.0d;
        }

        /// <summary>
        /// Set the radius.
        /// </summary>
        public virtual double Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        #endregion
    }
}
