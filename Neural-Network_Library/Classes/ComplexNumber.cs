using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ComplexNumber
    {

        private readonly double _x;

        private readonly double _y;


        public ComplexNumber(double u, double v)
        {
            _x = u;
            _y = v;
        }


        public ComplexNumber(ComplexNumber other)
        {
            _x = other.Real;
            _y = other.Imaginary;
        }


        public double Real
        {
            get { return _x; }
        }


        public double Imaginary
        {
            get { return _y; }
        }


        public double Mod()
        {
            if (_x != 0 || _y != 0)
            {
                return Math.Sqrt(_x * _x + _y * _y);
            }

            return 0d;
        }


        public double Arg()
        {
            return Math.Atan2(_y, _x);
        }


        public ComplexNumber Conj()
        {
            return new ComplexNumber(_x, -_y);
        }


        public static ComplexNumber operator +(ComplexNumber c1, ComplexNumber c2)
        {
            return new ComplexNumber(c1.Real + c2.Real, c1.Imaginary + c2.Imaginary);
        }



        public static ComplexNumber operator -(ComplexNumber c1, ComplexNumber c2)
        {
            return new ComplexNumber(c1.Real - c2.Real, c1.Imaginary - c2.Imaginary);
        }


        public static ComplexNumber operator *(ComplexNumber c1, ComplexNumber c2)
        {
            return new ComplexNumber(c1.Real * c2.Real - c1.Imaginary * c2.Imaginary, c1.Real * c2.Imaginary + c1.Imaginary
                                                                                  * c2.Real);
        }

        public static ComplexNumber operator /(ComplexNumber c1, ComplexNumber c2)
        {
            double den = Math.Pow(c2.Mod(), 2);
            return new ComplexNumber((c1.Real * c2.Real + c1.Imaginary
                                      * c2.Imaginary) / den, (c1.Imaginary
                                                           * c2.Real - c1.Real * c2.Imaginary) / den);
        }


        public ComplexNumber Exp()
        {
            return new ComplexNumber(Math.Exp(_x) * Math.Cos(_y), Math.Exp(_x)
                                                              * Math.Sin(_y));
        }


        public ComplexNumber Log()
        {
            return new ComplexNumber(Math.Log(Mod()), Arg());
        }



        public ComplexNumber Sqrt()
        {
            double r = Math.Sqrt(Mod());
            double theta = Arg() / 2;
            return new ComplexNumber(r * Math.Cos(theta), r * Math.Sin(theta));
        }



        private static double Cosh(double theta)
        {
            return (Math.Exp(theta) + Math.Exp(-theta)) / 2;
        }


        private static double Sinh(double theta)
        {
            return (Math.Exp(theta) - Math.Exp(-theta)) / 2;
        }


        public ComplexNumber Sin()
        {
            return new ComplexNumber(Cosh(_y) * Math.Sin(_x), Sinh(_y) * Math.Cos(_x));
        }

        public ComplexNumber Cos()
        {
            return new ComplexNumber(Cosh(_y) * Math.Cos(_x), -Sinh(_y) * Math.Sin(_x));
        }



        public ComplexNumber Sinh()
        {
            return new ComplexNumber(Sinh(_x) * Math.Cos(_y), Cosh(_x) * Math.Sin(_y));
        }


        public ComplexNumber Cosh()
        {
            return new ComplexNumber(Cosh(_x) * Math.Cos(_y), Sinh(_x) * Math.Sin(_y));
        }


        public ComplexNumber Tan()
        {
            return (Sin()) / (Cos());
        }


        public static ComplexNumber operator -(ComplexNumber op)
        {
            return new ComplexNumber(-op.Real, -op.Imaginary);
        }

        /// <inheritdoc/>
        public new String ToString()
        {
            if (_x != 0 && _y > 0)
            {
                return _x + " + " + _y + "i";
            }
            if (_x != 0 && _y < 0)
            {
                return _x + " - " + (-_y) + "i";
            }
            if (_y == 0)
            {
                return Format.FormatDouble(_x, 4);
            }
            if (_x == 0)
            {
                return _y + "i";
            }
            // shouldn't get here (unless Inf or NaN)
            return _x + " + i*" + _y;
        }
    }
}
