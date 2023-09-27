using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class Distort : BasicRandomizer
    {

        private readonly double _factor;


        public Distort(double f)
        {
            _factor = f;
        }


        public override double Randomize(double d)
        {
            return d + (_factor - (NextDouble() * _factor * 2));
        }
    }
}
