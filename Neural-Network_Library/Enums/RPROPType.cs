using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public enum RPROPType
    {
        /// <summary>
        /// RPROP+ : The classic RPROP algorithm.  Uses weight back tracking.
        /// </summary>
        RPROPp,

        /// <summary>
        /// RPROP- : No weight back tracking.
        /// </summary>
        RPROPm,

        /// <summary>
        /// iRPROP+ : New weight back tracking method, some consider this to be
        /// the most advanced RPROP.
        /// </summary>
        iRPROPp,

        /// <summary>
        /// iRPROP- : New RPROP without weight back tracking. 
        /// </summary>
        iRPROPm
    }
}
