using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class JobUnitContext
    {
        /// <summary>
        /// The JobUnit that this context will execute.
        /// </summary>
        public Object JobUnit { get; set; }

        /// <summary>
        /// The owner of this job.
        /// </summary>
        public ConcurrentJob Owner { get; set; }


        /// <summary>
        /// The task number.
        /// </summary>
        public int TaskNumber { get; set; }
    }
}
