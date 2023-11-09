using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class TagDataUnit : DataUnit
    {
        /// <summary>
        /// The tag that this data unit is based on.
        /// </summary>
        public Tag Tag { get; set; }
    }
}
