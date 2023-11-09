using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ListExtractListener : IExtractListener
    {
        /// <summary>
        /// The list to extract into.
        /// </summary>
        private readonly IList<Object> _list = new List<Object>();

        /// <summary>
        /// The list of words extracted.
        /// </summary>
        public IList<Object> List
        {
            get { return _list; }
        }

        #region IExtractListener Members

        /// <summary>
        /// Called when a word is found, add it to the list.
        /// </summary>
        /// <param name="obj">The word found.</param>
        public void FoundData(Object obj)
        {
            _list.Add(obj);
        }

        #endregion
    }
}
