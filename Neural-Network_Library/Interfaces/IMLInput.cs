using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLInput : IMLMethod
    {
        /// <value>The input.</value>
        int InputCount { get; }
    }
}
