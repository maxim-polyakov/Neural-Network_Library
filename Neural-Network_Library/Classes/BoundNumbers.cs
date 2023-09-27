using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public static class BoundNumbers
    {

        public const double TooSmall = -1.0E20;

        public const double TooBig = 1.0E20;


        public static double Bound(double d)
        {
            if (d < TooSmall)
            {
                return TooSmall;
            }
            return d > TooBig ? TooBig : d;
        }
    }
}
