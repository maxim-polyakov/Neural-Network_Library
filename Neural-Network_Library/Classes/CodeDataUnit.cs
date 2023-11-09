using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class CodeDataUnit : DataUnit
    {
        /// <summary>
        /// The code for this data unit.
        /// </summary>
        private String _code;

        /// <summary>
        /// The code for this data unit.
        /// </summary>
        public String Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            return _code;
        }
    }
}
