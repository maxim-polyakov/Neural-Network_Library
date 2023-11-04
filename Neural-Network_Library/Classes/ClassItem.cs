using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ClassItem
    {
        /// <summary>
        /// The index of the class.
        /// </summary>
        ///
        private int _index;

        /// <summary>
        /// The name of the class.
        /// </summary>
        ///
        private String _name;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theName">The name of the class.</param>
        /// <param name="theIndex">The index of the class.</param>
        public ClassItem(String theName, int theIndex)
        {
            _name = theName;
            _index = theIndex;
        }

        /// <summary>
        /// Set the index of the class.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }


        /// <summary>
        /// Set the name of the class.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }


        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(_name);
            result.Append(", index=");
            result.Append(_index);

            result.Append("]");
            return result.ToString();
        }
    }
}
