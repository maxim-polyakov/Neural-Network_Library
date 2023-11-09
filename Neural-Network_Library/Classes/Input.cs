using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class Input : FormElement
    {
        /// <summary>
        /// The type of input element that this is.
        /// </summary>
        private String _type;

        /// <summary>
        /// Construct this Input element.
        /// </summary>
        /// <param name="source">The source for this input ent.</param>
        public Input(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// The type of this input.
        /// </summary>
        public String Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// True if this is autosend, which means that the type is NOT
        /// submit. This prevents a form that has multiple submit buttons
        /// from sending ALL of them in a single post.
        /// </summary>
        public override bool AutoSend
        {
            get { return string.Compare(_type, "submit", true) != 0; }
        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[Input:");
            builder.Append("type=");
            builder.Append(Type);
            builder.Append(",name=");
            builder.Append(Name);
            builder.Append(",value=");
            builder.Append(Value);
            builder.Append("]");
            return builder.ToString();
        }
    }
}
