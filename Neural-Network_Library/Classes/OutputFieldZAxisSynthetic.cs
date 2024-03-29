﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class OutputFieldZAxisSynthetic : OutputFieldGrouped
    {
        /// <summary>
        /// Construct a synthetic output field for Z-Axis.
        /// </summary>
        /// <param name="group">The Z-Axis group that this belongs to.</param>
        public OutputFieldZAxisSynthetic(IOutputFieldGroup group)
            : base(group, null)
        {
            if (!(group is ZAxisGroup))
            {
                throw new NormalizationError(
                    "Must use ZAxisGroup with OutputFieldZAxisSynthetic.");
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
        /// Calculate the synthetic value for this Z-Axis normalization.
        /// </summary>
        /// <param name="subfield">Not used.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            double l = ((ZAxisGroup)Group).Length;
            double f = ((ZAxisGroup)Group).Multiplier;
            double n = Group.GroupedFields.Count;
            double result = f * Math.Sqrt(n - (l * l));
            return double.IsInfinity(result) || double.IsNaN(result) ? 0 : result;
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }
}
