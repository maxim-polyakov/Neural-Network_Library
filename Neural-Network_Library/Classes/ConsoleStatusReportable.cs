using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ConsoleStatusReportable : IStatusReportable
    {
        #region IStatusReportable Members

        /// <summary>
        /// Simply display any status reports.
        /// </summary>
        /// <param name="total">Total amount.</param>
        /// <param name="current">Current item.</param>
        /// <param name="message">Current message.</param>
        public void Report(int total, int current,
                           String message)
        {
            if (total == 0)
            {
                Console.WriteLine(current + " : " + message);
            }
            else
            {
                Console.WriteLine(current + "/" + total + " : " + message);
            }
        }

        #endregion
    }
}
