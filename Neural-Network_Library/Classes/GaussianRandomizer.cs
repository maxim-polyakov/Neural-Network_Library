using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class GaussianRandomizer : BasicRandomizer
    {

        private readonly double _mean;


        private readonly double _standardDeviation;


        private bool _useLast;


        private double _y2;


        public GaussianRandomizer(double mean, double standardDeviation)
        {
            _useLast = false;
            _mean = mean;
            _standardDeviation = standardDeviation;
        }


        public double BoxMuller(double m, double s)
        {
            double y1;

            // use value from previous call
            if (_useLast)
            {
                y1 = _y2;
                _useLast = false;
            }
            else
            {
                double x1;
                double x2;
                double w;
                do
                {
                    x1 = 2.0d * NextDouble() - 1.0d;
                    x2 = 2.0d * NextDouble() - 1.0d;
                    w = x1 * x1 + x2 * x2;
                } while (w >= 1.0d);

                w = Math.Sqrt((-2.0d * Math.Log(w)) / w);
                y1 = x1 * w;
                _y2 = x2 * w;
                _useLast = true;
            }

            return (m + y1 * s);
        }


        public override double Randomize(double d)
        {
            return BoxMuller(_mean, _standardDeviation);
        }
    }
}
