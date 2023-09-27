using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SyntMath
    {

        public static double Hypot(double a, double b)
        {
            double r;
            if (Math.Abs(a) > Math.Abs(b))
            {
                r = b / a;
                r = Math.Abs(a) * Math.Sqrt(1 + r * r);
            }
            else if (b != 0)
            {
                r = a / b;
                r = Math.Abs(b) * Math.Sqrt(1 + r * r);
            }
            else
            {
                r = 0.0;
            }
            return r;
        }



        public static double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180.0);
        }



        public static double Rad2Deg(double rad)
        {
            return rad * (180.0 / Math.PI);
        }


        public static double Factorial(int p)
        {
            double result = 1.0;

            for (int i = 1; i <= p; i++)
            {
                result *= (double)i;
            }

            return result;
        }
    }
}
