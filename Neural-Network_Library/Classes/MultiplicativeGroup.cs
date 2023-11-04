using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class MultiplicativeGroup : BasicOutputFieldGroup
    {
        /// <summary>
        /// The "length" of this field.
        /// </summary>
        private double _length;

        /// <summary>
        /// The length of this field.  This is the sum of the squares of
        /// all of the groupped fields.  The square root of this sum is the 
        /// length. 
        /// </summary>
        public double Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Called to init this group for a new field.  This recalculates the
        /// "length".
        /// </summary>
        public override void RowInit()
        {
            double value = GroupedFields.Sum(field => (field.SourceField.CurrentValue * field.SourceField.CurrentValue));
            _length = Math.Sqrt(value);
        }
    }
}
