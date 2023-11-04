using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SyntLogging
    {
        /// <summary>
        /// The lowest level log type. Debug logging provides low-level Synt
        /// diagnostics that may slow performance, but allow you to peer into the
        /// inner workings.
        /// </summary>
        ///
        public const int LevelDebug = 0;

        /// <summary>
        /// Info logging tells you when major processes start and stop.
        /// </summary>
        ///
        public const int LevelInfo = 1;

        /// <summary>
        /// Error level tells you about errors, less important to critical.
        /// </summary>
        ///
        public const int LevelError = 2;

        /// <summary>
        /// Critical logging logs errors that cannot be recovered from.
        /// </summary>
        ///
        public const int LevelCritical = 3;

        /// <summary>
        /// Logging is disabled at this level.
        /// </summary>
        ///
        public const int LevelDisable = 4;

        /// <value>The current logging level.</value>
        public int CurrentLevel
        {
            get { return SyntFramework.Instance.LoggingPlugin.LogLevel; }
        }

        /// <summary>
        /// Log the message.
        /// </summary>
        ///
        /// <param name="level">The level to log at.</param>
        /// <param name="message">The message to log.</param>
        public static void Log(int level, String message)
        {
            SyntFramework.Instance.LoggingPlugin.Log(level, message);
        }

        /// <summary>
        /// Log the error.
        /// </summary>
        ///
        /// <param name="level">The level to log at.</param>
        /// <param name="t">The exception to log.</param>
        public static void Log(int level, Exception t)
        {
            SyntFramework.Instance.LoggingPlugin.Log(level, t);
        }

        /// <summary>
        /// Log the error at ERROR level.
        /// </summary>
        ///
        /// <param name="t">The exception to log.</param>
        public static void Log(Exception t)
        {
            Log(LevelError, t);
        }
    }
}
