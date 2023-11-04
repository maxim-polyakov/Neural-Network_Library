using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SystemLoggingPlugin : ISyntPluginLogging1
    {
        /// <summary>
        /// The current level.
        /// </summary>
        ///
        private int currentLevel;

        /// <summary>
        /// True if we are logging to the console.
        /// </summary>
        ///
        private bool logConsole;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public SystemLoggingPlugin()
        {
            currentLevel = SyntLogging.LevelDisable;
            logConsole = false;
        }

        #region SyntPluginType1 Members

        /// <summary>
        /// Not used for this type of plugin.
        /// </summary>
        ///
        /// <param name="gradients">Not used.</param>
        /// <param name="layerOutput">Not used.</param>
        /// <param name="weights">Not used.</param>
        /// <param name="layerDelta">Not used.</param>
        /// <param name="af">Not used.</param>
        /// <param name="index">Not used.</param>
        /// <param name="fromLayerIndex">Not used.</param>
        /// <param name="fromLayerSize">Not used.</param>
        /// <param name="toLayerIndex">Not used.</param>
        /// <param name="toLayerSize">Not used.</param>
        public void CalculateGradient(double[] gradients,
                                      double[] layerOutput, double[] weights,
                                      double[] layerDelta, IActivationFunction af,
                                      int index, int fromLayerIndex, int fromLayerSize,
                                      int toLayerIndex, int toLayerSize)
        {
        }

        /// <summary>
        /// Not used for this type of plugin.
        /// </summary>
        ///
        /// <param name="weights">Not used.</param>
        /// <param name="layerOutput">Not used.</param>
        /// <param name="startIndex">Not used.</param>
        /// <param name="outputIndex">Not used.</param>
        /// <param name="outputSize">Not used.</param>
        /// <param name="inputIndex">Not used.</param>
        /// <param name="inputSize">Not used.</param>
        /// <returns>Not used.</returns>
        public int CalculateLayer(double[] weights,
                                  double[] layerOutput, int startIndex,
                                  int outputIndex, int outputSize, int inputIndex,
                                  int inputSize)
        {
            return 0;
        }

        /// <summary>
        /// Set the logging level.
        /// </summary>
        public int LogLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; }
        }


        /// <inheritdoc/>
        public String PluginDescription
        {
            get
            {
                return "This is the built in logging for Synt, it logs "
                       + "to either a file or System.out";
            }
        }


        /// <inheritdoc/>
        public String PluginName
        {
            get { return "HRI-System-Logging"; }
        }


        /// <value>Returns the service type for this plugin. This plugin provides
        /// the system calculation for layers and gradients. Therefore, this
        /// plugin returns SERVICE_TYPE_CALCULATION.</value>
        public int PluginServiceType
        {
            get { return SyntPluginBaseConst.SERVICE_TYPE_LOGGING; }
        }


        /// <value>This is a type-1 plugin.</value>
        public int PluginType
        {
            get { return 1; }
        }


        /// <summary>
        /// Log the message.
        /// </summary>
        ///
        /// <param name="level">The logging level.</param>
        /// <param name="message">The logging message.</param>
        public void Log(int level, String message)
        {
            if (currentLevel <= level)
            {
                DateTime now = DateTime.Now;
                var line = new StringBuilder();
                line.Append(now.ToString());
                line.Append(" [");
                switch (level)
                {
                    case SyntLogging.LevelCritical:
                        line.Append("CRITICAL");
                        break;
                    case SyntLogging.LevelError:
                        line.Append("ERROR");
                        break;
                    case SyntLogging.LevelInfo:
                        line.Append("INFO");
                        break;
                    case SyntLogging.LevelDebug:
                        line.Append("DEBUG");
                        break;
                    default:
                        line.Append("?");
                        break;
                }
                line.Append("][");
                line.Append(Thread.CurrentThread.Name);
                line.Append("]: ");
                line.Append(message);

                if (logConsole)
                {
                    if (currentLevel > SyntLogging.LevelError)
                    {
                        Console.Error.WriteLine(line.ToString());
                    }
                    else
                    {
                        Console.Out.WriteLine(line.ToString());
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Log(int level, Exception t)
        {
            Log(level, t.ToString());
        }

        #endregion

        /// <summary>
        /// Start logging to the console.
        /// </summary>
        ///
        public void StartConsoleLogging()
        {
            StopLogging();
            logConsole = true;
            LogLevel = SyntLogging.LevelDebug;
        }

        /// <summary>
        /// Stop any console or file logging.
        /// </summary>
        ///
        public void StopLogging()
        {
            logConsole = false;
        }
    }
}
