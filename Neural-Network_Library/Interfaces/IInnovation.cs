using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IInnovation
    {
        /// <summary>
        /// Set the innovation id.
        /// </summary>
        long InnovationID
        {
            get;
            set;
        }
    }
}
