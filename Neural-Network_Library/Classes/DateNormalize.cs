using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class DateNormalize
    {

        #region normalizes days and month in a date.

        public double[] NormalizeMonth(int month)
        {
            var eq = new Equilateral(12, -1, 1);
            return eq.Syntesis(month);
        }

        public int DenormalizeMonth(double[] montharry)
        {
            var eq = new Equilateral(12, -1, 1);
            return eq.Decode(montharry);
        }

        public double[] NormalizeDays(int days)
        {
            var eq = new Equilateral(31, -1, 1);
            return eq.Syntesis(days);
        }
        public int DenormalizeDays(double[] days)
        {
            var eq = new Equilateral(31, -1, 1);
            return eq.Decode(days);
        }
        public double[] NormalizeHour(int hour)
        {
            var eq = new Equilateral(24, -1, 1);
            return eq.Syntesis(hour);
        }
        public int DenormalizeHour(double[] hour)
        {
            var eq = new Equilateral(24, -1, 1);
            return eq.Decode(hour);
        }

        public double[] NormalizeSeconds(int Seconds)
        {
            var eq = new Equilateral(60, -1, 1);
            return eq.Syntesis(Seconds);
        }
        public int DenormalizeSeconds(double[] seconds)
        {
            var eq = new Equilateral(60, -1, 1);
            return eq.Decode(seconds);
        }


        public double[] NormalizeYear(int year)
        {
            var eq = new Equilateral(2011, -1, 1);
            return eq.Syntesis(year);
        }
        public static int DenormalizeYear(double[] year)
        {
            var eq = new Equilateral(2011, -1, 1);
            return eq.Decode(year);
        }
        public double[] NormalizeYear(int hour, int MaxYear)
        {
            var eq = new Equilateral(MaxYear, -1, 1);
            return eq.Syntesis(hour);
        }
        public int DenormalizeYear(double[] days, int MaxYear)
        {
            var eq = new Equilateral(MaxYear, -1, 1);
            return eq.Decode(days);
        }

        #endregion
    }
}
