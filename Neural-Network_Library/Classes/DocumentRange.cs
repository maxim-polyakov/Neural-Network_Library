using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class DocumentRange
    {
        /// <summary>
        /// Sub elements of this range.
        /// </summary>
        private readonly IList<DocumentRange> _elements = new List<DocumentRange>();

        /// <summary>
        /// The source page for this range.
        /// </summary>
        private WebPage _source;

        /// <summary>
        /// Construct a document range from the specified WebPage.
        /// </summary>
        /// <param name="source">The web page that this range belongs to.</param>
        public DocumentRange(WebPage source)
        {
            _source = source;
        }

        /// <summary>
        /// The beginning of this attribute.
        /// </summary>
        public int Begin { get; set; }

        /// <summary>
        /// The HTML class attribiute for this element.
        /// </summary>
        public String ClassAttribute { get; set; }

        /// <summary>
        /// The elements of this document range. 
        /// </summary>
        public IList<DocumentRange> Elements
        {
            get { return _elements; }
        }

        /// <summary>
        /// The ending index.
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// The HTML id for this element.
        /// </summary>
        public String IdAttribute { get; set; }

        /// <summary>
        /// The web page that owns this class.
        /// </summary>
        public DocumentRange Parent { get; set; }

        /// <summary>
        /// The web page that this range is owned by.
        /// </summary>
        public WebPage Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>
        /// Add an element.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void AddElement(DocumentRange element)
        {
            Elements.Add(element);
            element.Parent = this;
        }

        /// <summary>
        /// Get the text from this range.
        /// </summary>
        /// <returns>The text from this range.</returns>
        public String GetTextOnly()
        {
            var result = new StringBuilder();

            for (int i = Begin; i < End; i++)
            {
                DataUnit du = _source.Data[i];
                if (du is TextDataUnit)
                {
                    result.Append(du.ToString());
                    result.Append("\n");
                }
            }

            return result.ToString();
        }


        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            return GetTextOnly();
        }
    }
}
