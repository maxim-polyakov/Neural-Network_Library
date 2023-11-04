using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class OutputFieldMultiplicative : OutputFieldGrouped
    {
        /// <summary>
        /// The default constructor.  Used for reflection.
        /// </summary>
        public OutputFieldMultiplicative()
        {
        }

        /// <summary>
        /// Construct a multiplicative output field.
        /// </summary>
        /// <param name="group">The group this field belongs to.</param>
        /// <param name="field">The input field that this field is based on.</param>
        public OutputFieldMultiplicative(IOutputFieldGroup group,
                                         IInputField field)
            : base(group, field)
        {
            if (!(group is MultiplicativeGroup))
            {
                throw new NormalizationError(
                    "Must use MultiplicativeGroup with OutputFieldMultiplicative.");
            }
        }

        /// <summary>
        /// Always returns 1, subfields are not used for this field.
        /// </summary>
        public override int SubfieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Calculate the value for this output field.
        /// </summary>
        /// <param name="subfield">The subfield is not used.</param>
        /// <returns>The value for this field.</returns>
        public override double Calculate(int subfield)
        {
            return SourceField.CurrentValue
                   / ((MultiplicativeGroup)Group).Length;
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }
}
