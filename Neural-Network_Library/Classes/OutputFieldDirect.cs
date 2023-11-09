using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class OutputFieldDirect : BasicOutputField
    {
        /// <summary>
        /// The source field.
        /// </summary>
        private readonly IInputField _sourceField;


        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public OutputFieldDirect()
        {
        }

        /// <summary>
        /// Construct a direct output field.
        /// </summary>
        /// <param name="sourceField">The source field to pass directly on.</param>
        public OutputFieldDirect(IInputField sourceField)
        {
            _sourceField = sourceField;
        }

        /// <summary>
        /// Always returns 1, as subfields are not used.
        /// </summary>
        public override int SubfieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Calculate the value for this field. This will simply be the
        /// value from the input field. 
        /// </summary>
        /// <param name="subfield">Not used, as this output field type does not
        /// support subfields.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            return _sourceField.CurrentValue;
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }
}
