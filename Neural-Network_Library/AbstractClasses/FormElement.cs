using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public abstract class FormElement : DocumentRange
    {
        /// <summary>
        /// The name of the element.
        /// </summary>
        private String _name;

        /// <summary>
        /// The owner.
        /// </summary>
        private Form _owner;

        /// <summary>
        /// The value.
        /// </summary>
        private String _value;

        /// <summary>
        /// Construct a form element from the specified web page. 
        /// </summary>
        /// <param name="source">The page that holds this form element.</param>
        protected FormElement(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// The name of this form.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The owner of this form element.
        /// </summary>
        public Form Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        /// <summary>
        /// The value of this form element.
        /// </summary>
        public String Value
        {
            get { return _value; }
            set { this._value = value; }
        }

        /// <summary>
        /// True if this is autosend, which means that the type is 
        /// NOT submit.  This prevents a form that has multiple submit buttons
        /// from sending ALL of them in a single post.
        /// </summary>
        public abstract bool AutoSend { get; }
    }
}
