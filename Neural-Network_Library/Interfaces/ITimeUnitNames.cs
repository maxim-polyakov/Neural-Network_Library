using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    internal interface ITimeUnitNames
    {
        String Singular(TimeUnit unit);
        String Plural(TimeUnit unit);
        String Code(TimeUnit unit);
    }
}
