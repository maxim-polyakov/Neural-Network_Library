using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class ZAxisGroup : BasicOutputFieldGroup
    {
        /// <summary>
        /// The calculated length.
        /// </summary>
        private double _length;

        /// <summary>
        /// The multiplier, which is the value that all other values will be
        /// multiplied to become normalized.
        /// </summary>
        private double _multiplier;

        /// <summary>
        /// The vector length.
        /// </summary>
        public double Length
        {
            get { return _length; }
        }

        /// <summary>
        /// The value to multiply the other values by to normalize them.
        /// </summary>
        public double Multiplier
        {
            get { return _multiplier; }
        }

        /// <summary>
        /// Initialize this group for a new row.
        /// </summary>
        public override void RowInit()
        {
            double value = (from field in GroupedFields
                            where !(field is OutputFieldZAxisSynthetic)
                            where field.SourceField != null
                            select (field.SourceField.CurrentValue * field.SourceField.CurrentValue)).Sum();

            _length = Math.Sqrt(value);
            _multiplier = 1.0 / Math.Sqrt(GroupedFields.Count);
        }
    }
}
