using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BiPolarUtil
    {
        /// <summary>
        /// Convert binary to bipolar, true is 1 and false is -1.
        /// </summary>
        /// <param name="b">The binary value.</param>
        /// <returns>The bipolar value.</returns>
        public static double Bipolar2double(bool b)
        {
            if (b)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Convert a boolean array to bipolar, true is 1 and false is -1.
        /// </summary>
        /// <param name="b">The binary array to convert.</param>
        /// <returns></returns>
        public static double[] Bipolar2double(bool[] b)
        {
            var result = new double[b.Length];

            for (int i = 0; i < b.Length; i++)
            {
                result[i] = Bipolar2double(b[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert a 2D boolean array to bipolar, true is 1 and false is -1.
        /// </summary>
        /// <param name="b">The 2D array to convert.</param>
        /// <returns>A bipolar array.</returns>
        public static double[][] Bipolar2double(bool[][] b)
        {
            var result = new double[b.Length][];

            for (int row = 0; row < b.Length; row++)
            {
                result[row] = new double[b[row].Length];
                for (int col = 0; col < b[row].Length; col++)
                {
                    result[row][col] = Bipolar2double(b[row][col]);
                }
            }

            return result;
        }

        /// <summary>
        /// Convert biploar to boolean, true is 1 and false is -1.
        /// </summary>
        /// <param name="d">A bipolar value.</param>
        /// <returns>A boolean value.</returns>
        public static bool Double2bipolar(double d)
        {
            if (d > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Convert a bipolar array to a boolean array, true is 1 and false is -1.
        /// </summary>
        /// <param name="d">A bipolar array.</param>
        /// <returns>A boolean array.</returns>
        public static bool[] Double2bipolar(double[] d)
        {
            var result = new bool[d.Length];

            for (int i = 0; i < d.Length; i++)
            {
                result[i] = Double2bipolar(d[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert a 2D bipolar array to a boolean array, true is 1 and false is -1.
        /// </summary>
        /// <param name="d">A 2D bipolar array.</param>
        /// <returns>A 2D boolean array.</returns>
        public static bool[][] Double2bipolar(double[][] d)
        {
            var result = new bool[d.Length][];

            for (int row = 0; row < d.Length; row++)
            {
                result[row] = new bool[d[row].Length];
                for (int col = 0; col < d[row].Length; col++)
                {
                    result[row][col] = Double2bipolar(d[row][col]);
                }
            }

            return result;
        }

        /// <summary>
        /// Normalize a binary number.  Greater than 0 becomes 1, zero and below are false.
        /// </summary>
        /// <param name="d">A binary number in a double.</param>
        /// <returns>A double that will be 0 or 1.</returns>
        public static double NormalizeBinary(double d)
        {
            if (d > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Convert a single number from bipolar to binary.
        /// </summary>
        /// <param name="d">a bipolar number.</param>
        /// <returns>A binary number.</returns>
        public static double ToBinary(double d)
        {
            return (d + 1) / 2.0;
        }

        /// <summary>
        /// Convert a number to bipolar.
        /// </summary>
        /// <param name="d">A binary number.</param>
        /// <returns></returns>
        public static double ToBiPolar(double d)
        {
            return (2 * NormalizeBinary(d)) - 1;
        }

        /// <summary>
        /// Normalize a number and convert to binary.
        /// </summary>
        /// <param name="d">A bipolar number.</param>
        /// <returns>A binary number stored as a double</returns>
        public static double ToNormalizedBinary(double d)
        {
            return NormalizeBinary(ToBinary(d));
        }
    }
}
