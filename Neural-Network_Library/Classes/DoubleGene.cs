using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class DoubleGene : BasicGene
    {
        /// <summary>
        /// The value of this gene.
        /// </summary>
        ///
        private double _value;

        /// <summary>
        /// Set the value of the gene.
        /// </summary>
        ///
        /// <value>The gene's value.</value>
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        ///
        /// <param name="gene">The other gene to copy.</param>
        public override sealed void Copy(IGene gene)
        {
            _value = ((DoubleGene)gene).Value;
        }


        /// <inheritdoc/>
        public override sealed String ToString()
        {
            return "" + _value;
        }
    }
}
