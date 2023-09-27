using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IHasFixedLength
    {
        /// <summary>
        /// The number of records in this input field.
        /// </summary>
        int Length { get; }
    }
}
