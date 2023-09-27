using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLClustering : IMLMethod
    {
        /// <value>The clusters.</value>
        IMLCluster[] Clusters { get; }

        /// <summary>
        /// Perform the training iteration.
        /// </summary>
        ///
        void Iteration();

        /// <summary>
        /// Perform the specified number of training iterations.
        /// </summary>
        ///
        /// <param name="count">The number of training iterations.</param>
        void Iteration(int count);


        /// <returns>The number of clusters.</returns>
        int Count { get; }
    }
}
