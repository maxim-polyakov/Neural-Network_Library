using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class RangeRandomizer : BasicRandomizer
    {

        private readonly double _max;



        private readonly double _min;


        public RangeRandomizer(double min, double max)
        {
            _max = max;
            _min = min;
        }


        /// <value>the min</value>
        public double Min
        {
            get { return _min; }
        }


        /// <value>the max</value>
        public double Max
        {
            get { return _max; }
        }


        public static int RandomInt(int min, int max)
        {
            return (int)Randomize(min, max + 1);
        }


        public static double Randomize(double min, double max)
        {
            double range = max - min;
            return (range * ThreadSafeRandom.NextDouble()) + min;
        }


        public override double Randomize(double d)
        {
            return NextDouble(_min, _max);
        }
    }
}
