using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class MLDataFieldHolder
    {
        /// <summary>
        /// A field.
        /// </summary>
        private readonly InputFieldMLDataSet _field;

        /// <summary>
        /// An iterator.
        /// </summary>
        private readonly IEnumerator<IMLDataPair> _iterator;

        /// <summary>
        /// A neural data pair.
        /// </summary>
        private IMLDataPair _pair;

        /// <summary>
        /// Construct the class.
        /// </summary>
        /// <param name="iterator">An iterator.</param>
        /// <param name="field">A field.</param>
        public MLDataFieldHolder(IEnumerator<IMLDataPair> iterator,
                                     InputFieldMLDataSet field)
        {
            _iterator = iterator;
            _field = field;
        }

        /// <summary>
        /// The field.
        /// </summary>
        public InputFieldMLDataSet Field
        {
            get { return _field; }
        }

        /// <summary>
        /// The pair.
        /// </summary>
        public IMLDataPair Pair
        {
            get { return _pair; }
            set { _pair = value; }
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            return _iterator;
        }

        /// <summary>
        /// Obtain the next pair.
        /// </summary>
        public void ObtainPair()
        {
            _iterator.MoveNext();
            _pair = _iterator.Current;
        }
    }
}
