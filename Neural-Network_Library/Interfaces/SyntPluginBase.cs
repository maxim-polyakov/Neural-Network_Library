using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface SyntPluginBase
    {
        /// <value>The type number for this plugin.</value>
        int PluginType { get; }


        /// <value>The service type provided by this plugin.</value>
        int PluginServiceType { get; }


        /// <value>The name of the plugin.</value>
        string PluginName { get; }


        /// <value>The plugin description.</value>
        string PluginDescription { get; }
    }
}
