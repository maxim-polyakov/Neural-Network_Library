using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class OutputFieldSyntesis : BasicOutputField
    {
        /// <summary>
        /// The ranges.
        /// </summary>
        private readonly IList<MappedRange> _ranges = new List<MappedRange>();

        /// <summary>
        /// The source field.
        /// </summary>
        private readonly IInputField _sourceField;

        /// <summary>
        /// The catch all value, if nothing matches, then use this value.
        /// </summary>
        private double _catchAll;


        /// <summary>
        /// Construct an Syntesisd field.
        /// </summary>
        /// <param name="sourceField">The field that this is based on.</param>
        public OutputFieldSyntesis(IInputField sourceField)
        {
            _sourceField = sourceField;
        }

        /// <summary>
        /// The source field.
        /// </summary>
        public IInputField SourceField
        {
            get { return _sourceField; }
        }

        /// <summary>
        /// Return 1, no subfield supported.
        /// </summary>
        public override int SubfieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// The catch all value that is to be returned if none
        /// of the ranges match.
        /// </summary>
        public double CatchAll
        {
            get { return _catchAll; }
            set { _catchAll = value; }
        }

        /// <summary>
        /// Add a ranged mapped to a value.
        /// </summary>
        /// <param name="low">The low value for the range.</param>
        /// <param name="high">The high value for the range.</param>
        /// <param name="value">The value that the field should produce for this range.</param>
        public void AddRange(double low, double high, double value)
        {
            var range = new MappedRange(low, high, value);
            _ranges.Add(range);
        }

        /// <summary>
        /// Calculate the value for this field.
        /// </summary>
        /// <param name="subfield">Not used.</param>
        /// <returns>Return the value for the range the input falls within, or return
        /// the catchall if nothing matches.</returns>
        public override double Calculate(int subfield)
        {
            foreach (MappedRange range in _ranges)
            {
                if (range.InRange(_sourceField.CurrentValue))
                {
                    return range.Value;
                }
            }

            return _catchAll;
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }
}
