using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class IndexSampleSegregator : IndexSegregator
    {
        /// <summary>
        /// The ending index (within a sample).
        /// </summary>
        private readonly int _endingIndex;

        /// <summary>
        /// The sample size.
        /// </summary>
        private readonly int _sampleSize;

        /// <summary>
        /// The starting index (within a sample).
        /// </summary>
        private readonly int _startingIndex;

        /// <summary>
        /// The default constructor, for reflection.
        /// </summary>
        public IndexSampleSegregator()
        {
        }

        /// <summary>
        /// Construct an index sample segregator.
        /// </summary>
        /// <param name="startingIndex">The starting index.</param>
        /// <param name="endingIndex">The ending index.</param>
        /// <param name="sampleSize">The sample size.</param>
        public IndexSampleSegregator(int startingIndex,
                                     int endingIndex, int sampleSize)
        {
            _sampleSize = sampleSize;
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
        /// The sample size.
        /// </summary>
        public int SampleSize
        {
            get { return _sampleSize; }
        }

        /// <summary>
        /// The starting index.
        /// </summary>
        public int StartingIndex
        {
            get { return _startingIndex; }
        }

        /// <summary>
        /// Should this row be included.
        /// </summary>
        /// <returns>True if this row should be included.</returns>
        public override bool ShouldInclude()
        {
            int sampleIndex = CurrentIndex % _sampleSize;
            RollIndex();
            return ((sampleIndex >= _startingIndex) && (sampleIndex <= _endingIndex));
        }
    }
}
