using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class Tag
    {
        /// <summary>
        /// Tag types.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// A beginning tag.
            /// </summary>
            Begin,
            /// <summary>
            /// An ending tag.
            /// </summary>
            End,
            /// <summary>
            /// A comment.
            /// </summary>
            Comment,
            /// <summary>
            /// A CDATA section.
            /// </summary>
            CDATA
        };


        /// <summary>
        /// The tag's attributes.
        /// </summary>
        private readonly IDictionary<String, String> _attributes =
            new Dictionary<String, String>();

        /// <summary>
        /// The tag name.
        /// </summary>
        private String _name = "";

        /// <summary>
        /// The tag type.
        /// </summary>
        private Type _type;

        /// <summary>
        /// Clear the name, type and attributes.
        /// </summary>
        public void Clear()
        {
            _attributes.Clear();
            _name = "";
            _type = Type.Begin;
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A cloned copy of the object.</returns>
        public virtual object Clone()
        {
            var result = new Tag { Name = Name, TagType = TagType };
            foreach (String key in _attributes.Keys)
            {
                String value = _attributes[key];
                result.Attributes[key] = value;
            }
            return result;
        }

        /// <summary>
        /// Get the specified attribute as an integer.
        /// </summary>
        /// <param name="attributeId">The attribute name.</param>
        /// <returns>The attribute value.</returns>
        public int GetAttributeInt(String attributeId)
        {
            try
            {
                String str = GetAttributeValue(attributeId);
                return int.Parse(str);
            }
            catch (Exception e)
            {
#if logging
                if (logger.IsErrorEnabled)
                {
                    logger.Error("Exception", e);
                }
#endif
                throw new ParseError(e);
            }
        }


        /// <summary>
        /// The attributes for this tag as a dictionary.
        /// </summary>
        public IDictionary<String, String> Attributes
        {
            get { return _attributes; }
        }

        /// <summary>
        /// Get the value of the specified attribute.
        /// </summary>
        /// <param name="name">The name of an attribute.</param>
        /// <returns>The value of the specified attribute.</returns>
        public String GetAttributeValue(String name)
        {
            if (!_attributes.ContainsKey(name))
                return null;

            return _attributes[name];
        }


        /// <summary>
        /// The tag name.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The tag type.
        /// </summary>
        public Type TagType
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Set a HTML attribute.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="valueRen">The value of the attribute.</param>
        public void SetAttribute(String name, String valueRen)
        {
            _attributes[name] = valueRen;
        }

        /// <summary>
        /// Convert this tag back into string form, with the 
        /// beginning &lt; and ending &gt;.
        /// </summary>
        /// <returns>The Attribute object that was found.</returns>
        public override String ToString()
        {
            var buffer = new StringBuilder("<");

            if (_type == Type.End)
            {
                buffer.Append("/");
            }

            buffer.Append(_name);

            ICollection<String> set = _attributes.Keys;
            foreach (String key in set)
            {
                String value = _attributes[key];
                buffer.Append(' ');

                if (value == null)
                {
                    buffer.Append("\"");
                    buffer.Append(key);
                    buffer.Append("\"");
                }
                else
                {
                    buffer.Append(key);
                    buffer.Append("=\"");
                    buffer.Append(value);
                    buffer.Append("\"");
                }
            }

            buffer.Append(">");
            return buffer.ToString();
        }
    }
}
