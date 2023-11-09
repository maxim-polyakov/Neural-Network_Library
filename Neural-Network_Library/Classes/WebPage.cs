using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class WebPage
    {
        /// <summary>
        /// The contents of this page, builds upon the list of DataUnits.
        /// </summary>
        private readonly IList<DocumentRange> _contents = new List<DocumentRange>();

        /// <summary>
        /// The data units that make up this page.
        /// </summary>
        private readonly IList<DataUnit> _data = new List<DataUnit>();

        /// <summary>
        /// The title of this HTML page.
        /// </summary>
        private DocumentRange _title;

        /// <summary>
        /// The contents in a list collection.
        /// </summary>
        public IList<DocumentRange> Contents
        {
            get { return _contents; }
        }

        /// <summary>
        /// The data units in a list collection.
        /// </summary>
        public IList<DataUnit> Data
        {
            get { return _data; }
        }

        /// <summary>
        /// The title of this document.
        /// </summary>
        public DocumentRange Title
        {
            get { return _title; }
            set
            {
                _title = value;
                _title.Source = this;
            }
        }

        /// <summary>
        /// Add to the content collection.
        /// </summary>
        /// <param name="span">The range to add to the collection.</param>
        public void AddContent(DocumentRange span)
        {
            span.Source = this;
            _contents.Add(span);
        }

        /// <summary>
        /// Add a data unit to the collection.
        /// </summary>
        /// <param name="unit">The data unit to load.</param>
        public void AddDataUnit(DataUnit unit)
        {
            _data.Add(unit);
        }

        /// <summary>
        /// Find the specified DocumentRange subclass in the contents list.
        /// </summary>
        /// <param name="c">The class type to search for.</param>
        /// <param name="index">The index to search from.</param>
        /// <returns>The document range that was found.</returns>
        public DocumentRange Find(Type c, int index)
        {
            int i = index;
            foreach (DocumentRange span in Contents)
            {
                if (span.GetType() == c)
                {
                    if (i <= 0)
                    {
                        return span;
                    }
                    i--;
                }
            }
            return null;
        }

        /// <summary>
        /// Find the link that contains the specified string.
        /// </summary>
        /// <param name="str">The string to search for.</param>
        /// <returns>The link found.</returns>
        public Link FindLink(String str)
        {
            return Contents.OfType<Link>().FirstOrDefault(link => link.GetTextOnly().Equals(str));
        }

        /// <summary>
        /// Get the number of data items in this collection.
        /// </summary>
        /// <returns>The size of the data unit.</returns>
        public int getDataSize()
        {
            return _data.Count;
        }

        /// <summary>
        /// Get the DataUnit unit at the specified index.
        /// </summary>
        /// <param name="i">The index to use.</param>
        /// <returns>The DataUnit found at the specified index.</returns>
        public DataUnit GetDataUnit(int i)
        {
            return _data[i];
        }


        /// <summary>
        /// The object as a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            foreach (DocumentRange span in Contents)
            {
                result.Append(span.ToString());
                result.Append("\n");
            }
            return result.ToString();
        }
    }
}
