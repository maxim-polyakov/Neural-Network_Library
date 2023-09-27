using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface INeighborhoodFunction
    {
        /// <summary>
        /// Set the radius.
        /// </summary>
        double Radius
        {
            get;
            set;
        }

        /// <summary>
        /// Determine how much the current neuron should be affected by training
        /// based on its proximity to the winning neuron.
        /// </summary>
        ///
        /// <param name="currentNeuron">THe current neuron being evaluated.</param>
        /// <param name="bestNeuron">The winning neuron.</param>
        /// <returns>The ratio for this neuron's adjustment.</returns>
        double Function(int currentNeuron, int bestNeuron);
    }
}
