using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class MatrixMath
    {

        private MatrixMath()
        {
        }


        public static Matrix Add(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows)
            {

            }

            if (a.Cols != b.Cols)
            {

            }

            var result = new double[a.Rows][];
            double[][] aData = a.Data;
            double[][] bData = b.Data;

            for (int resultRow = 0; resultRow < a.Rows; resultRow++)
            {
                result[resultRow] = new double[a.Cols];
                for (int resultCol = 0; resultCol < a.Cols; resultCol++)
                {
                    result[resultRow][resultCol] = aData[resultRow][resultCol]
                                                   + bData[resultRow][resultCol];
                }
            }

            return new Matrix(result);
        }

        public static void Copy(Matrix source, Matrix target)
        {
            double[][] sourceData = source.Data;
            double[][] targetData = target.Data;

            for (int row = 0; row < source.Rows; row++)
            {
                for (int col = 0; col < source.Cols; col++)
                {
                    targetData[row][col] = sourceData[row][col];
                }
            }
        }


        public static Matrix DeleteCol(Matrix matrix, int deleted)
        {
            if (deleted >= matrix.Cols)
            {

            }
            var newMatrix = new double[matrix.Rows][];
            double[][] matrixData = matrix.Data;

            for (int row = 0; row < matrix.Rows; row++)
            {
                int targetCol = 0;

                newMatrix[row] = new double[matrix.Cols - 1];

                for (int col = 0; col < matrix.Cols; col++)
                {
                    if (col != deleted)
                    {
                        newMatrix[row][targetCol] = matrixData[row][col];
                        targetCol++;
                    }
                }
            }
            return new Matrix(newMatrix);
        }

        public static Matrix DeleteRow(Matrix matrix, int deleted)
        {
            if (deleted >= matrix.Rows)
            {

            }
            var newMatrix = new double[matrix.Rows - 1][];
            double[][] matrixData = matrix.Data;

            int targetRow = 0;
            for (int row = 0; row < matrix.Rows; row++)
            {
                if (row != deleted)
                {
                    newMatrix[targetRow] = new double[matrix.Cols];
                    for (int col = 0; col < matrix.Cols; col++)
                    {
                        newMatrix[targetRow][col] = matrixData[row][col];
                    }
                    targetRow++;
                }
            }
            return new Matrix(newMatrix);
        }


        public static Matrix Divide(Matrix a, double b)
        {
            var result = new double[a.Rows][];
            double[][] aData = a.Data;
            for (int row = 0; row < a.Rows; row++)
            {
                result[row] = new double[a.Cols];
                for (int col = 0; col < a.Cols; col++)
                {
                    result[row][col] = aData[row][col] / b;
                }
            }
            return new Matrix(result);
        }


        public static double DotProduct(Matrix a, Matrix b)
        {
            if (!a.IsVector() || !b.IsVector())
            {

            }

            Double[] aArray = a.ToPackedArray();
            Double[] bArray = b.ToPackedArray();

            if (aArray.Length != bArray.Length)
            {

            }

            double result = 0;
            int length = aArray.Length;

            for (int i = 0; i < length; i++)
            {
                result += aArray[i] * bArray[i];
            }

            return result;
        }


        public static Matrix Identity(int size)
        {
            if (size < 1)
            {

            }

            var result = new Matrix(size, size);
            double[][] resultData = result.Data;

            for (int i = 0; i < size; i++)
            {
                resultData[i][i] = 1;
            }

            return result;
        }

        /// <summary>
        /// Multiply every cell in the matrix by the specified value.
        /// </summary>
        /// <param name="a">Multiply every cell in a matrix by the specified value.</param>
        /// <param name="b">The value to multiply by.</param>
        /// <returns>The new multiplied matrix.</returns>
        public static Matrix Multiply(Matrix a, double b)
        {
            var result = new double[a.Rows][];
            double[][] aData = a.Data;

            for (int row = 0; row < a.Rows; row++)
            {
                result[row] = new double[a.Cols];
                for (int col = 0; col < a.Cols; col++)
                {
                    result[row][col] = aData[row][col] * b;
                }
            }
            return new Matrix(result);
        }

        /// <summary>
        /// Multiply two matrixes.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix Multiply(Matrix a, Matrix b)
        {
            if (a.Cols != b.Rows)
            {

            }

            var result = new double[a.Rows][];
            double[][] aData = a.Data;
            double[][] bData = b.Data;

            for (int resultRow = 0; resultRow < a.Rows; resultRow++)
            {
                result[resultRow] = new double[b.Cols];
                for (int resultCol = 0; resultCol < b.Cols; resultCol++)
                {
                    double value = 0;

                    for (int i = 0; i < a.Cols; i++)
                    {
                        value += aData[resultRow][i] * bData[i][resultCol];
                    }
                    result[resultRow][resultCol] = value;
                }
            }

            return new Matrix(result);
        }

        /// <summary>
        /// Subtract one matrix from another.  The two matrixes must have the same number of rows and columns.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The subtracted matrix.</returns>
        public static Matrix Subtract(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows)
            {

            }

            if (a.Cols != b.Cols)
            {

            }

            var result = new double[a.Rows][];
            double[][] aData = a.Data;
            double[][] bData = b.Data;

            for (int resultRow = 0; resultRow < a.Rows; resultRow++)
            {
                result[resultRow] = new double[a.Cols];
                for (int resultCol = 0; resultCol < a.Cols; resultCol++)
                {
                    result[resultRow][resultCol] = aData[resultRow][resultCol]
                                                   - bData[resultRow][resultCol];
                }
            }

            return new Matrix(result);
        }

        /// <summary>
        /// Transpose the specified matrix.
        /// </summary>
        /// <param name="input">The matrix to transpose.</param>
        /// <returns>The transposed matrix.</returns>
        public static Matrix Transpose(Matrix input)
        {
            var inverseMatrix = new double[input.Cols][];
            double[][] inputData = input.Data;

            for (int r = 0; r < input.Cols; r++)
            {
                inverseMatrix[r] = new double[input.Rows];
                for (int c = 0; c < input.Rows; c++)
                {
                    inverseMatrix[r][c] = inputData[c][r];
                }
            }

            return new Matrix(inverseMatrix);
        }

        /// <summary>
        /// Calculate the vector length of the matrix.
        /// </summary>
        /// <param name="input">The vector to calculate for.</param>
        /// <returns>The vector length.</returns>
        public static double VectorLength(Matrix input)
        {
            if (!input.IsVector())
            {

            }
            Double[] v = input.ToPackedArray();
            double rtn = 0.0;
            for (int i = 0; i < v.Length; i++)
            {
                rtn += Math.Pow(v[i], 2);
            }
            return Math.Sqrt(rtn);
        }

        /// <summary>
        /// Multiply the matrix by a vector.
        /// </summary>
        /// <param name="a">The matrix.</param>
        /// <param name="d">The vector.</param>
        /// <returns>The resulting vector.</returns>
        public static double[] Multiply(Matrix a, double[] d)
        {
            double[] p = new double[a.Rows];
            double[][] aData = a.Data;

            for (int r = 0; r < a.Rows; r++)
                for (int i = 0; i < a.Cols; i++)
                    p[r] += aData[r][i] * d[i];

            return p;
        }
    }
}
