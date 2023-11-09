using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class TextDataUnit : DataUnit
    {
        /// <summary>
        /// The text for this data unit.
        /// </summary>
        private String _text;

        /// <summary>
        /// The text for this data unit.
        /// </summary>
        public String Text
        {
            get { return _text; }
            set { _text = value; }
        }


        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            return _text;
        }
    }
}
