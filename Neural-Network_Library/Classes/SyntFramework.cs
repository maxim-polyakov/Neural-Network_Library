using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SyntFramework
    {
        /// <summary>
        /// The current engog version, this should be read from the properties.
        /// </summary>
        public static string Version = "3.1.0";

        /// <summary>
        /// The platform.
        /// </summary>
        public static string PLATFORM = "DotNet";

        /// <summary>
        /// The current engog file version, this should be read from the properties.
        /// </summary>
        private const string FileVersion = "1";


        /// <summary>
        /// The default precision to use for compares.
        /// </summary>
        public const int DefaultPrecision = 10;

        /// <summary>
        /// Default point at which two doubles are equal.
        /// </summary>
        public const double DefaultDoubleEqual = 0.0000001;

        /// <summary>
        /// The version of the Synt JAR we are working with. Given in the form
        /// x.x.x.
        /// </summary>
        public const string SyntVersion = "Synt.version";

        /// <summary>
        /// The Synt file version. This determines of an Synt file can be read.
        /// This is simply an integer, that started with zero and is incramented each
        /// time the format of the Synt data file changes.
        /// </summary>
        public static string SyntFileVersion = "Synt.file.version";

        /// <summary>
        /// The instance.
        /// </summary>
        private static SyntFramework _instance = new SyntFramework();

        /// <summary>
        /// The current logging plugin.
        /// </summary>
        ///
        private ISyntPluginLogging1 _loggingPlugin;

        /// <summary>
        /// The plugins.
        /// </summary>
        ///
        private readonly IList<SyntPluginBase> _plugins;

        /// <summary>
        /// Get the instance to the singleton.
        /// </summary>
        public static SyntFramework Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Get the properties as a Map.
        /// </summary>
        private readonly IDictionary<string, string> _properties =
            new Dictionary<string, string>();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private SyntFramework()
        {
            _properties[SyntVersion] = Version;
            _properties[SyntFileVersion] = FileVersion;

            _plugins = new List<SyntPluginBase>();
            RegisterPlugin(new SystemLoggingPlugin());
            RegisterPlugin(new SystemMethodsPlugin());
            RegisterPlugin(new SystemTrainingPlugin());
            RegisterPlugin(new SystemActivationPlugin());
        }

        /// <summary>
        /// The Synt properties.  Contains version information.
        /// </summary>
        public IDictionary<string, string> Properties
        {
            get { return _properties; }
        }


        /// <summary>
        /// Shutdown Synt.
        /// </summary>
        public void Shutdown()
        {
        }

        /// <value>the loggingPlugin</value>
        public ISyntPluginLogging1 LoggingPlugin
        {
            get { return _loggingPlugin; }
        }

        /// <summary>
        /// Register a plugin. If this plugin provides a core service, such as
        /// calculation or logging, this will remove the old plugin.
        /// </summary>
        ///
        /// <param name="plugin">The plugin to register.</param>
        public void RegisterPlugin(SyntPluginBase plugin)
        {
            // is it not a general plugin?
            if (plugin.PluginServiceType != SyntPluginBaseConst.SERVICE_TYPE_GENERAL)
            {
                if (plugin.PluginServiceType == SyntPluginBaseConst.SERVICE_TYPE_LOGGING)
                {
                    // remove the old logging plugin
                    if (_loggingPlugin != null)
                    {
                        _plugins.Remove(_loggingPlugin);
                    }
                    _loggingPlugin = (ISyntPluginLogging1)plugin;
                }
            }
            // add to the plugins
            _plugins.Add(plugin);
        }

        /// <summary>
        /// Unregister a plugin. If you unregister the current logging or calc
        /// plugin, a new system one will be created. Synt will crash without a
        /// logging or system plugin.
        /// </summary>
        public void UnregisterPlugin(SyntPluginBase plugin)
        {
            // is it a special plugin?
            // if so, replace with the system, Synt will crash without these
            if (plugin == _loggingPlugin)
            {
                _loggingPlugin = new SystemLoggingPlugin();
            }

            // remove it
            _plugins.Remove(plugin);
        }

        /// <summary>
        /// The plugins.
        /// </summary>
        public IList<SyntPluginBase> Plugins
        {
            get { return _plugins; }
        }
    }
}
