using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ReadHTML : ReadTags
    {
        /// <summary>
        /// Construct a HTML reader.
        /// </summary>
        /// <param name="istream">The input stream to read from.</param>
        public ReadHTML(Stream istream) : base(istream)
        {
        }

        /// <summary>
        /// Parse the attribute name.
        /// </summary>
        /// <returns>The attribute name.</returns>
        protected String ParseAttributeName()
        {
            String result = base.ParseAttributeName();
            return result.ToLower();
        }
    }
}
