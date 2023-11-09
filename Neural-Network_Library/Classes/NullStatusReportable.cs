using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NullStatusReportable : IStatusReportable
    {
        #region IStatusReportable Members

        /// <summary>
        /// Simply ignore any status reports.
        /// </summary>
        /// <param name="total">Not used.</param>
        /// <param name="current">Not used.</param>
        /// <param name="message">Not used.</param>
        public void Report(int total, int current,
                           String message)
        {
        }

        #endregion
    }
}
