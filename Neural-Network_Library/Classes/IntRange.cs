using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class IntRange
    {

        public IntRange(int high, int low)
        {
            High = high;
            Low = low;
        }

        public int High { get; set; }


        public int Low { get; set; }
    }
}
