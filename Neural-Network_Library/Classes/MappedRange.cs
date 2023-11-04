using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class MappedRange
    {
        /// <summary>
        /// The high value for the range.
        /// </summary>       
        private readonly double _high;

        /// <summary>
        /// The low value for the range.
        /// </summary>
        private readonly double _low;

        /// <summary>
        /// The value that should be returned for this range.
        /// </summary>
        private readonly double _value;

        /// <summary>
        /// Construct the range mapping.
        /// </summary>
        /// <param name="low">The low value for the range.</param>
        /// <param name="high">The high value for the range.</param>
        /// <param name="value">The value that this range represents.</param>
        public MappedRange(double low, double high, double value)
        {
            _low = low;
            _high = high;
            _value = value;
        }

        /// <summary>
        /// The high value for this range.
        /// </summary>
        public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// The low value for this range.
        /// </summary>
        public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// The value that this range represents.
        /// </summary>
        public double Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Determine if the specified value is in the range.
        /// </summary>
        /// <param name="d">The value to check.</param>
        /// <returns>True if this value is within the range.</returns>
        public bool InRange(double d)
        {
            if ((d >= _low) && (d <= _high))
            {
                return true;
            }
            return false;
        }
    }
}
