using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface ICentroidFactory<in TO>
    {
        /// <summary>
        /// The centroid.
        /// </summary>
        /// <returns>The centroid.</returns>
        ICentroid<TO> CreateCentroid();
    }
}
