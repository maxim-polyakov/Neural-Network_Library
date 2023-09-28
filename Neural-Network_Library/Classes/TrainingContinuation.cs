using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class TrainingContinuation
    {
        /// <summary>
        /// The contents of this object.
        /// </summary>
        ///
        private readonly IDictionary<String, Object> _contents;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public TrainingContinuation()
        {
            _contents = new Dictionary<String, Object>();
        }


        /// <value>The contents.</value>
        public IDictionary<String, Object> Contents
        {
            get { return _contents; }
        }

        /// <value>the trainingType to set</value>
        public String TrainingType
        {
            get;
            set;
        }

        /// <summary>
        /// Get an object by name.
        /// </summary>
        ///
        /// <param name="name">The name of the object.</param>
        /// <returns>The object requested.</returns>
        public Object Get(String name)
        {
            return _contents[name];
        }


        /// <summary>
        /// Save a list of doubles.
        /// </summary>
        ///
        /// <param name="key">The key to save them under.</param>
        /// <param name="list">The list of doubles.</param>
        public void Put(String key, double[] list)
        {
            _contents[key] = list;
        }

        /// <summary>
        /// Set a value to a string.
        /// </summary>
        ///
        /// <param name="name">The value to set.</param>
        /// <param name="v">The value.</param>
        public void Set(String name, Object v)
        {
            _contents[name] = v;
        }
    }
}
