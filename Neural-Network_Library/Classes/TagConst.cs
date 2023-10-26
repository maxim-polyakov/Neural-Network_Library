using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class TagConst
    {
        /// <summary>
        /// The beginning of a comment.
        /// </summary>
        public const String CommentBegin = "!--";

        /// <summary>
        /// The end of a comment.
        /// </summary>
        public const String CommentEnd = "-->";

        /// <summary>
        /// The beginning of a CDATA section.
        /// </summary>
        public const String CDATABegin = "![CDATA[";

        /// <summary>
        /// The end of a CDATA section.
        /// </summary>
        public const String CDATAEnd = "]]";

        /// <summary>
        /// Private constructor.
        /// </summary>
        private TagConst()
        {
        }
    }
}
