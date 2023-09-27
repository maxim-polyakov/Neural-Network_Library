using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface ISyntPluginLogging1 : SyntPluginBase
    {
        /// <summary>
        /// The current log level.
        /// </summary>
        int LogLevel { get; }

        /// <summary>
        /// Log a message at the specified level. 
        /// </summary>
        /// <param name="level">The level to log at.</param>
        /// <param name="message">The message to log.</param>
        void Log(int level, String message);

        /// <summary>
        /// Log a throwable at the specified level.
        /// </summary>
        /// <param name="level">The level to log at.</param>
        /// <param name="t">The error to log.</param>
        void Log(int level, Exception t);
    }
}
