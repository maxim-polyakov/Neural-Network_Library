using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Neural_Network_Library
{
    public class Div : DocumentRange
    {
        /// <summary>
        /// Construct a range to hold the DIV tag.
        /// </summary>
        /// <param name="source">The web page this range was found on.</param>
        public Div(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[Div:class=");
            result.Append(ClassAttribute);
            result.Append(",id=");
            result.Append(IdAttribute);
            result.Append(",elements=");
            result.Append(Elements.Count);
            result.Append("]");
            return result.ToString();
        }
    }
}
