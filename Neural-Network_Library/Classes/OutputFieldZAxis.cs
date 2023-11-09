using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class OutputFieldZAxis : OutputFieldGrouped
    {
        /// <summary>
        /// Construct a ZAxis output field.
        /// </summary>
        /// <param name="group">The group this field belongs to.</param>
        /// <param name="field">The input field this is based on.</param>
        public OutputFieldZAxis(IOutputFieldGroup group,
                                IInputField field)
            : base(group, field)
        {
            if (!(group is ZAxisGroup))
            {
                throw new NormalizationError(
                    "Must use ZAxisGroup with OutputFieldZAxis.");
            }
        }

        /// <summary>
        /// The subfield count, which is one, as this field type does not
        /// have subfields.
        /// </summary>
        public override int SubfieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Calculate the current value for this field. 
        /// </summary>
        /// <param name="subfield">Ignored, this field type does not have subfields.</param>
        /// <returns>The current value for this field.</returns>
        public override double Calculate(int subfield)
        {
            return (SourceField.CurrentValue * ((ZAxisGroup)Group)
                                                 .Multiplier);
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }
}
