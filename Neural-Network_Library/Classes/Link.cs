using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class Link : DocumentRange
    {
        /// <summary>
        /// The target address for this link.
        /// </summary>
        private Address _target;

        /// <summary>
        /// Construct a link from the specified web page.
        /// </summary>
        /// <param name="source">The web page this link is from.</param>
        public Link(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// The target of this link.
        /// </summary>
        public Address Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[Link:");
            result.Append(_target);
            result.Append("|");
            result.Append(GetTextOnly());
            result.Append("]");
            return result.ToString();
        }
    }
}
