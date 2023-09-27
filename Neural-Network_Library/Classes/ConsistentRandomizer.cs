using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ConsistentRandomizer : BasicRandomizer
    {

        private readonly double _max;


        private readonly double _min;

        private readonly LinearCongruentialGenerator _rand;


        private readonly int _seed;


        public ConsistentRandomizer(double min, double max) : this(min, max, 1000)
        {
        }


        public ConsistentRandomizer(double min, double max,
                                    int seed)
        {
            _max = max;
            _min = min;
            _seed = seed;
            _rand = new LinearCongruentialGenerator(seed);
        }


        public override double Randomize(double d)
        {
            return _rand.Range(_min, _max);
        }


        public void Randomize(BasicNetwork network)
        {
            _rand.Seed = _seed;
            base.Randomize(network);
        }
    }
}
