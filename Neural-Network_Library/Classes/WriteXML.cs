using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class WriteXML : WriteTags
    {
        /// <summary>
        /// Construct an object to write an XML file.
        /// </summary>
        /// <param name="os">The output stream.</param>
        public WriteXML(Stream os) : base(os)
        {
        }
    }
}
