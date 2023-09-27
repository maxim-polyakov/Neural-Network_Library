using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ConstRandomizer : BasicRandomizer
    {

        private readonly double value;


        public ConstRandomizer(double v)
        {
            this.value = v;
        }

        public override double Randomize(double d)
        {
            return value;
        }
    }
}
