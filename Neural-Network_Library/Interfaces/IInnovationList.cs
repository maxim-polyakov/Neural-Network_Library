using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IInnovationList
    {
        /// <value>A list of innovations.</value>
        IList<IInnovation> Innovations { get; }

        /// <summary>
        /// Add an innovation.
        /// </summary>
        ///
        /// <param name="innovation">The innovation added.</param>
        void Add(IInnovation innovation);

        /// <summary>
        /// Get the innovation specified by index.
        /// </summary>
        ///
        /// <param name="id">The index.</param>
        /// <returns>The innovation.</returns>
        IInnovation Get(int id);
    }
}
