using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IContainsFlat : IMLMethod
    {
        /// <value>The flat network associated with this neural network.</value>
        FlatNetwork Flat { get; }
    }

}
