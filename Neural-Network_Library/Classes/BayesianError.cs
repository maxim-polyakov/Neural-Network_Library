using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BayesianError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public BayesianError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public BayesianError(Exception e)
            : base(e)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="e">The exception.</param>
        public BayesianError(String msg, Exception e)
            : base(msg, e)
        {
        }
    }
}
