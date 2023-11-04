using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public sealed class ResourceLoader
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private ResourceLoader()
        {
        }

        /// <summary>
        /// Create a stream to read the resource.
        /// </summary>
        /// <param name="resource">The resource to load.  This should be in the form Synt.Resources.classes.txt</param>
        /// <returns>A stream.</returns>
        public static Stream CreateStream(String resource)
        {
            Stream result = null;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly a in assemblies)
            {
                result = a.GetManifestResourceStream(resource);
                if (result != null)
                    break;
            }

            return result;
        }

        /// <summary>
        /// Load a string.
        /// </summary>
        /// <param name="resource">The resource to load.</param>
        /// <returns>The loaded string.</returns>
        public static String LoadString(String resource)
        {
            var result = new StringBuilder();
            Stream istream = CreateStream(resource);
            var sr = new StreamReader(istream);

            String line;
            while ((line = sr.ReadLine()) != null)
            {
                result.Append(line);
                result.Append("\r\n");
            }
            sr.Close();
            istream.Close();

            return result.ToString();
        }
    }
}
