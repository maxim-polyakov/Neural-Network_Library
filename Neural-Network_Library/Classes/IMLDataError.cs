using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class IMLDataError : SyntError
    {

        public IMLDataError(String str)
            : base(str)
        {
        }


        public IMLDataError(Exception e)
            : base(e)
        {
        }
    }
}
