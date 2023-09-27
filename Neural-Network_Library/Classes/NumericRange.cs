using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NumericRange
    {

        private readonly double _high;


        private readonly double _low;


        private readonly double _mean;


        private readonly double _rms;

        int _samples;


        private readonly double _standardDeviation;


        public NumericRange(IList<Double> values)
        {
            double assignedHigh = 0;
            double assignedLow = 0;
            double total = 0;
            double rmsTotal = 0;

            // get the mean and other 1-pass values.

            foreach (double d in values)
            {
                assignedHigh = Math.Max(assignedHigh, d);
                assignedLow = Math.Min(assignedLow, d);
                total += d;
                rmsTotal += d * d;
            }

            _samples = values.Count;
            _high = assignedHigh;
            _low = assignedLow;
            _mean = total / _samples;
            _rms = Math.Sqrt(rmsTotal / _samples);

            // now get the standard deviation
            double devTotal = values.Sum(d => Math.Pow(d - _mean, 2));

            _standardDeviation = Math.Sqrt(devTotal / _samples);
        }


        public double High
        {
            get { return _high; }
        }


        public double Low
        {
            get { return _low; }
        }


        public double Mean
        {
            get { return _mean; }
        }

        public double RMS
        {
            get { return _rms; }
        }

        public double StandardDeviation
        {
            get { return _standardDeviation; }
        }


        public int Samples
        {
            get { return _samples; }
        }


        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("Range: ");
            result.Append(Format.FormatDouble(_low, 5));
            result.Append(" to ");
            result.Append(Format.FormatDouble(_high, 5));
            result.Append(",samples: ");
            result.Append(Format.FormatInteger(_samples));
            result.Append(",mean: ");
            result.Append(Format.FormatDouble(_mean, 5));
            result.Append(",rms: ");
            result.Append(Format.FormatDouble(_rms, 5));
            result.Append(",s.deviation: ");
            result.Append(Format.FormatDouble(_standardDeviation, 5));

            return result.ToString();
        }
    }
}
