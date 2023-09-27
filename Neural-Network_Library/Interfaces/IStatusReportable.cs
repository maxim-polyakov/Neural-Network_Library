using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IStatusReportable
    {
        /// <summary>
        /// Report on current status.
        /// </summary>
        ///
        /// <param name="total">The total amount of units to process.</param>
        /// <param name="current">The current unit being processed.</param>
        /// <param name="message">The message to currently display.</param>
        void Report(int total, int current, String message);
    }
}
