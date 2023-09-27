using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IGenerateID
    {
        /// <summary>
        /// Get the value for the current id.
        /// </summary>
        long CurrentID { get; set; }

        /// <summary>
        /// Generate an ID number.
        /// </summary>
        /// <returns>The ID number generated.</returns>
        long Generate();
    }
}
