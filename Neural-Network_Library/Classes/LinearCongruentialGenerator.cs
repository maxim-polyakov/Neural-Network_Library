using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class LinearCongruentialGenerator
    {

        public const long MaxRand = 4294967295L;


        public LinearCongruentialGenerator(long seed)
            : this((long)Math.Pow(2L, 32L), 1103515245L, 12345L, seed)
        {
        }


        public LinearCongruentialGenerator(long modulus,
                                           long multiplier, long increment, long seed)
        {
            Modulus = modulus;
            Multiplier = multiplier;
            Increment = increment;
            Seed = seed;
        }


        public long Modulus { get; set; }


        public long Multiplier { get; set; }


        public long Increment { get; set; }

        public long Seed { get; set; }



        public double NextDouble()
        {
            return (double)NextLong() / MaxRand;
        }


        public long NextLong()
        {
            Seed = (Multiplier * Seed + Increment)
                   % Modulus;
            return Seed;
        }

        public double Range(double min, double max)
        {
            double range = max - min;
            return (range * NextDouble()) + min;
        }
    }
}
