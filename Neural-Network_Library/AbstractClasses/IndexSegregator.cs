using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public abstract class IndexSegregator : ISegregator
    {
        /// <summary>
        /// The current index.  Updated rows are processed.
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// THe normalization object this belongs to.
        /// </summary>
        private DataNormalization _normalization;

        /// <summary>
        /// The current index.
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
        }

        #region ISegregator Members

        /// <summary>
        /// The normalization object this object will use.
        /// </summary>
        public DataNormalization Owner
        {
            get { return _normalization; }
        }

        /// <summary>
        /// Setup this class with the specified normalization object.
        /// </summary>
        /// <param name="normalization">Normalization object.</param>
        public void Init(DataNormalization normalization)
        {
            _normalization = normalization;
        }

        /// <summary>
        /// Should this row be included, according to this segregator.
        /// </summary>
        /// <returns>True if this row should be included.</returns>
        public abstract bool ShouldInclude();

        /// <summary>
        /// Init for pass... nothing to do fo this class.
        /// </summary>
        public void PassInit()
        {
            _currentIndex = 0;
        }

        #endregion

        /// <summary>
        /// Used to increase the current index as data is processed.
        /// </summary>
        public void RollIndex()
        {
            _currentIndex++;
        }
    }
}
