using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class Equilateral
    {

        public const int MinEq = 3;


        private readonly double[][] _matrix;


        public Equilateral(int count, double high, double low)
        {
            _matrix = Equilat(count, high, low);
        }


        public int Decode(double[] activations)
        {
            double minValue = double.PositiveInfinity;
            int minSet = -1;

            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                double dist = GetDistance(activations, i);
                if (dist < minValue)
                {
                    minValue = dist;
                    minSet = i;
                }
            }
            return minSet;
        }


        public double[] Syntesis(int set)
        {
            if (set < 0 || set > _matrix.Length)
            {

            }
            return _matrix[set];
        }


        private static double[][] Equilat(int n,
                                   double high, double low)
        {
            var result = new double[n][]; // n - 1
            for (int i = 0; i < n; i++)
            {
                result[i] = new double[n - 1];
            }

            result[0][0] = -1;
            result[1][0] = 1.0;

            for (int k = 2; k < n; k++)
            {
                // scale the matrix so far
                double r = k;
                double f = Math.Sqrt(r * r - 1.0) / r;
                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < k - 1; j++)
                    {
                        result[i][j] *= f;
                    }
                }

                r = -1.0 / r;
                for (int i = 0; i < k; i++)
                {
                    result[i][k - 1] = r;
                }

                for (int i = 0; i < k - 1; i++)
                {
                    result[k][i] = 0.0;
                }
                result[k][k - 1] = 1.0;
            }

            // scale it
            for (int row = 0; row < result.GetLength(0); row++)
            {
                for (int col = 0; col < result[0].GetLength(0); col++)
                {
                    const double min = -1;
                    const double max = 1;
                    result[row][col] = ((result[row][col] - min) / (max - min))
                                       * (high - low) + low;
                }
            }

            return result;
        }


        public double GetDistance(double[] data, int set)
        {
            double result = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                result += Math.Pow(data[i] - _matrix[set][i], 2);
            }
            return Math.Sqrt(result);
        }


        public int GetSmallestDistance(double[] data)
        {
            int bestSet = -1;
            double bestDistance = double.MaxValue;

            for (int i = 0; i < _matrix.Length; i++)
            {
                double d = GetDistance(data, i);
                if (bestSet == -1 || d < bestDistance)
                {
                    bestSet = i;
                    bestDistance = d;
                }
            }

            return bestSet;
        }
    }
}
