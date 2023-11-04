using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SyntPluginBaseConst
    {
        /// <summary>
        /// A general plugin, you can have multiple plugins installed that provide
        /// general services.
        /// </summary>
        ///
        public const int SERVICE_TYPE_GENERAL = 0;

        /// <summary>
        /// A special plugin that provides logging. You may only have one logging
        /// plugin installed.
        /// </summary>
        ///
        public const int SERVICE_TYPE_LOGGING = 1;
    }
}
