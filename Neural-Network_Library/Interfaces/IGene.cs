using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IGene : IComparable<IGene>
    {
        /// <summary>
        /// Get the ID of this gene, -1 for undefined.
        /// </summary>
        ///
        /// <value>The ID of this gene.</value>
        long Id
        {
            get;
        }


        /// <value>The innovation ID of this gene.</value>
        long InnovationId
        {
            get;
        }


        /// <summary>
        /// Determine if this gene is enabled.
        /// </summary>
        ///
        /// <value>True if this gene is enabled.</value>
        bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        ///
        /// <param name="gene">The other gene to copy.</param>
        void Copy(IGene gene);
    }
}
