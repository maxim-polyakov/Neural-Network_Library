using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class IndexRangeSegregator : IndexSegregator
    {
        /// <summary>
        /// The ending index.
        /// </summary>        
        private readonly int _endingIndex;

        /// <summary>
        /// The starting index.
        /// </summary>
        private readonly int _startingIndex;

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public IndexRangeSegregator()
        {
        }

        /// <summary>
        /// Construct an index range segregator.
        /// </summary>
        /// <param name="startingIndex">The starting index to allow.</param>
        /// <param name="endingIndex">The ending index to allow.</param>
        public IndexRangeSegregator(int startingIndex, int endingIndex)
        {
            _startingIndex = startingIndex;
            _endingIndex = endingIndex;
        }

        /// <summary>
        /// The ending index.
        /// </summary>
        public int EndingIndex
        {
            get { return _endingIndex; }
        }

        /// <summary>
        /// The starting index.
        /// </summary>
        public int StartingIndex
        {
            get { return _startingIndex; }
        }

        /// <summary>
        /// Determines if the current row should be included.
        /// </summary>
        /// <returns>True if the current row should be included.</returns>
        public override bool ShouldInclude()
        {
            bool result = ((CurrentIndex >= _startingIndex) && (CurrentIndex <= _endingIndex));
            RollIndex();
            return result;
        }
    }
}
