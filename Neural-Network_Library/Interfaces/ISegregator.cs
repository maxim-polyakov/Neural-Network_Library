using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface ISegregator
    {
        /// <summary>
        /// The normalization object that is being used with this segregator.
        /// </summary>
        DataNormalization Owner { get; }

        /// <summary>
        /// Setup this object to use the specified normalization object.
        /// </summary>
        /// <param name="normalization">The normalization object to use.</param>
        void Init(DataNormalization normalization);

        /// <summary>
        /// Should this row be included, according to this segregator.
        /// </summary>
        /// <returns>True if this row should be included.</returns>
        bool ShouldInclude();

        /// <summary>
        /// Init for a pass.
        /// </summary>
        void PassInit();
    }
}
