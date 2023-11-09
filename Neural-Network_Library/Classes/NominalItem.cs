using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class NominalItem
    {
        /// <summary>
        /// The high value for the range.
        /// </summary>
        private readonly double _high;

        /// <summary>
        /// The input field used to verify against the range.
        /// </summary>
        private readonly IInputField _inputField;

        /// <summary>
        /// The low value for the range.
        /// </summary>
        private readonly double _low;

        /// <summary>
        /// Construct a empty range item.  Used mainly for reflection.
        /// </summary>
        public NominalItem()
        {
        }

        /// <summary>
        /// Create a nominal item.
        /// </summary>
        /// <param name="inputField">The field that this item is based on.</param>
        /// <param name="high">The high value.</param>
        /// <param name="low">The low value.</param>
        public NominalItem(IInputField inputField, double low,
                           double high)
        {
            _high = high;
            _low = low;
            _inputField = inputField;
        }

        /// <summary>
        /// The high value.
        /// </summary>
        public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// The input field value.
        /// </summary>
        public IInputField InputField
        {
            get { return _inputField; }
        }

        /// <summary>
        /// The low value.
        /// </summary>
        public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// Begin a row.
        /// </summary>
        public void BeginRow()
        {
        }

        /// <summary>
        /// Determine if the specified value is in range.
        /// </summary>
        /// <returns>True if this item is within range.</returns>
        public bool IsInRange()
        {
            double currentValue = _inputField.CurrentValue;
            return ((currentValue >= _low) && (currentValue <= _high));
        }
    }
}
