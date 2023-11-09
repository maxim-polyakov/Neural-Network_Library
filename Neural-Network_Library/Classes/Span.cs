﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class Span : DocumentRange
    {
        /// <summary>
        /// Construct a span range from the specified web page.
        /// </summary>
        /// <param name="source">The source web page.</param>
        public Span(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// This object as a string. 
        /// </summary>
        /// <returns>This object as a string. </returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[Span:class=");
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
