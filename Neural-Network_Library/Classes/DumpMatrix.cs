using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class DumpMatrix
    {
        /// <summary>
        /// Maximum precision.
        /// </summary>
        public const int MaxPrecis = 3;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private DumpMatrix()
        {
        }


        /// <summary>
        /// Dump an array of numbers to a string.
        /// </summary>
        /// <param name="d">The array to dump.</param>
        /// <returns>The array as a string.</returns>
        public static String DumpArray(double[] d)
        {
            var result = new StringBuilder();
            result.Append("[");
            for (int i = 0; i < d.Length; i++)
            {
                if (i != 0)
                {
                    result.Append(",");
                }
                result.Append(d[i]);
            }
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// Dump a matrix to a string.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The matrix as a string.</returns>
        public static String DumpMatrixString(Matrix matrix)
        {
            var result = new StringBuilder();
            result.Append("==");
            result.Append(matrix.ToString());
            result.Append("==\n");
            for (int row = 0; row < matrix.Rows; row++)
            {
                result.Append("  [");
                for (int col = 0; col < matrix.Cols; col++)
                {
                    if (col != 0)
                    {
                        result.Append(",");
                    }
                    result.Append(matrix[row, col]);
                }
                result.Append("]\n");
            }
            return result.ToString();
        }
    }
}
