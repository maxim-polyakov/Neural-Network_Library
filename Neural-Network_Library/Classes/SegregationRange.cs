using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class SegregationRange
    {
        /// <summary>
        /// The high end of this range.
        /// </summary>
        private readonly double _high;

        /// <summary>
        /// Should this range be included.
        /// </summary>
        private readonly bool _include;

        /// <summary>
        /// The low end of this range.
        /// </summary>
        private readonly double _low;

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public SegregationRange()
        {
        }

        /// <summary>
        /// Construct a segregation range.
        /// </summary>
        /// <param name="low">The low end of the range.</param>
        /// <param name="high">The high end of the range.</param>
        /// <param name="include">Specifies if the range should be included.</param>
        public SegregationRange(double low, double high,
                                bool include)
        {
            _low = low;
            _high = high;
            _include = include;
        }

        /// <summary>
        /// The high end of the range.
        /// </summary>
        public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// The low end of the range.
        /// </summary>
        public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// True if this range should be included.
        /// </summary>
        public bool IsIncluded
        {
            get { return _include; }
        }

        /// <summary>
        /// Is this value within the range. 
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is within the range.</returns>
        public bool InRange(double value)
        {
            return ((value >= _low) && (value <= _high));
        }
    }
}
