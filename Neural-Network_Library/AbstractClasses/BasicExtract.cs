using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public abstract class BasicExtract : IExtract
    {
        /// <summary>
        /// The classes registered as listeners for the extraction.
        /// </summary>
        private readonly ICollection<IExtractListener> _listeners =
            new List<IExtractListener>();

        #region IExtract Members

        /// <summary>
        /// Add a listener for the extraction.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        public void AddListener(IExtractListener listener)
        {
            _listeners.Add(listener);
        }

        /// <summary>
        /// Extract from the web page and return the results as a list.
        /// </summary>
        /// <param name="page">The web page to extract from.</param>
        /// <returns>The results of the extraction as a List.</returns>
        public IList<Object> ExtractList(WebPage page)
        {
            Listeners.Clear();
            var listener = new ListExtractListener();
            AddListener(listener);
            Extract(page);
            return listener.List;
        }

        /// <summary>
        /// A list of listeners registered with this object.
        /// </summary>
        public ICollection<IExtractListener> Listeners
        {
            get { return _listeners; }
        }

        /// <summary>
        /// Remove the specified listener.
        /// </summary>
        /// <param name="listener">The listener to rmove.</param>
        public void RemoveListener(IExtractListener listener)
        {
            _listeners.Remove(listener);
        }

        /// <summary>
        /// Extract data from the web page.
        /// </summary>
        /// <param name="page">The page to extract from.</param>
        public abstract void Extract(WebPage page);

        #endregion

        /// <summary>
        /// Distribute an object to the listeners.
        /// </summary>
        /// <param name="obj">The object to be distributed.</param>
        public void Distribute(Object obj)
        {
            foreach (IExtractListener listener in _listeners)
            {
                listener.FoundData(obj);
            }
        }
    }
}
