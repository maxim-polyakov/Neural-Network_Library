using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class OutputOneOf : BasicOutputField
    {
        /// <summary>
        /// What is the true value, often just "0" or "-1".
        /// </summary>
        private readonly double _falseValue;

        /// <summary>
        /// The nominal items to represent.
        /// </summary>
        private readonly IList<NominalItem> _items = new List<NominalItem>();

        /// <summary>
        /// What is the true value, often just "1".
        /// </summary>
        private readonly double _trueValue;

        /// <summary>
        /// Default constructor for reflection.  Use 1 for true, -1 for false.
        /// </summary>
        public OutputOneOf() : this(1, -1)
        {
        }

        /// <summary>
        /// Construct a one-of field and specify the true and false value.
        /// </summary>
        /// <param name="trueValue">The true value.</param>
        /// <param name="falseValue">The false value.</param>
        public OutputOneOf(double trueValue, double falseValue)
        {
            _trueValue = trueValue;
            _falseValue = falseValue;
        }

        /// <summary>
        /// The false value.
        /// </summary>
        public double FalseValue
        {
            get { return _falseValue; }
        }

        /// <summary>
        /// The number of subfields, or nominal classes.
        /// </summary>
        /// <returns></returns>
        public override int SubfieldCount
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Add a nominal value specifying a single value, the high and low values
        /// will be 0.5 below and 0.5 above.
        /// </summary>
        /// <param name="inputField">The input field to use.</param>
        /// <param name="value">The value to calculate the high and low values off of.</param>
        public void AddItem(IInputField inputField, double value)
        {
            AddItem(inputField, value - 0.5, value + 0.5);
        }

        /// <summary>
        /// Add a nominal item, specify the low and high values.
        /// </summary>
        /// <param name="inputField">The input field to base everything from.</param>
        /// <param name="low">The high value for this nominal item.</param>
        /// <param name="high">The low value for this nominal item.</param>
        public void AddItem(IInputField inputField, double low,
                            double high)
        {
            var item = new NominalItem(inputField, low, high);
            _items.Add(item);
        }

        /// <summary>
        /// Calculate the value for the specified subfield.
        /// </summary>
        /// <param name="subfield">The subfield to calculate for.</param>
        /// <returns>The calculated value for this field.</returns>
        public override double Calculate(int subfield)
        {
            NominalItem item = _items[subfield];
            return item.IsInRange() ? _trueValue : _falseValue;
        }

        /// <summary>
        /// The true value.
        /// </summary>
        /// <returns></returns>
        public double TrueValue
        {
            get { return _trueValue; }
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }
}
