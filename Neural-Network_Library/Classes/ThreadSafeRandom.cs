using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ThreadSafeRandom
    {

        private static readonly Random Random = new Random();


        public static double NextDouble()
        {
            lock (Random)
            {
                return Random.NextDouble();
            }
        }
    }
}
