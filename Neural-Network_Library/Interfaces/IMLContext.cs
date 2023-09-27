using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLContext : IMLMethod
    {
        /// <summary>
        /// Clear the context.
        /// </summary>
        ///
        void ClearContext();
    }
}
