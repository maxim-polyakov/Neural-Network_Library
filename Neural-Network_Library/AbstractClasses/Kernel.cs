using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    internal abstract class Kernel
    {
        //UPGRADE_NOTE: Final was removed from the declaration of 'degree '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'
        private readonly double coef0;
        private readonly double degree;
        //UPGRADE_NOTE: Final was removed from the declaration of 'gamma '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'
        private readonly double gamma;
        private readonly int kernel_type;
        private readonly svm_node[][] x;
        //UPGRADE_NOTE: Final was removed from the declaration of 'x_square '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'
        private readonly double[] x_square;

        internal Kernel(int l, svm_node[][] x_, svm_parameter param)
        {
            kernel_type = param.kernel_type;
            degree = param.degree;
            gamma = param.gamma;
            coef0 = param.coef0;

            x = (svm_node[][])x_.Clone();

            if (kernel_type == svm_parameter.RBF)
            {
                x_square = new double[l];
                for (int i = 0; i < l; i++)
                    x_square[i] = dot(x[i], x[i]);
            }
            else
                x_square = null;
        }

        //UPGRADE_NOTE: Final was removed from the declaration of 'coef0 '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'

        internal abstract float[] get_Q(int column, int len);

        internal virtual void swap_index(int i, int j)
        {
            do
            {
                svm_node[] _ = x[i];
                x[i] = x[j];
                x[j] = _;
            } while (false);
            if (x_square != null)
                do
                {
                    double _ = x_square[i];
                    x_square[i] = x_square[j];
                    x_square[j] = _;
                } while (false);
        }

        private static double tanh(double x)
        {
            double e = Math.Exp(x);
            return 1.0 - 2.0 / (e * e + 1);
        }

        internal virtual double kernel_function(int i, int j)
        {
            switch (kernel_type)
            {
                case svm_parameter.LINEAR:
                    return dot(x[i], x[j]);

                case svm_parameter.POLY:
                    return Math.Pow(gamma * dot(x[i], x[j]) + coef0, degree);

                case svm_parameter.RBF:
                    return Math.Exp((-gamma) * (x_square[i] + x_square[j] - 2 * dot(x[i], x[j])));

                case svm_parameter.SIGMOID:
                    return tanh(gamma * dot(x[i], x[j]) + coef0);

                default:
                    return 0; // java
            }
        }

        internal static double dot(svm_node[] x, svm_node[] y)
        {
            double sum = 0;
            int xlen = x.Length;
            int ylen = y.Length;
            int i = 0;
            int j = 0;
            while (i < xlen && j < ylen)
            {
                if (x[i].index == y[j].index)
                    sum += x[i++].value_Renamed * y[j++].value_Renamed;
                else
                {
                    if (x[i].index > y[j].index)
                        ++j;
                    else
                        ++i;
                }
            }
            return sum;
        }

        internal static double k_function(svm_node[] x, svm_node[] y, svm_parameter param)
        {
            switch (param.kernel_type)
            {
                case svm_parameter.LINEAR:
                    return dot(x, y);

                case svm_parameter.POLY:
                    return Math.Pow(param.gamma * dot(x, y) + param.coef0, param.degree);

                case svm_parameter.RBF:
                    {
                        double sum = 0;
                        int xlen = x.Length;
                        int ylen = y.Length;
                        int i = 0;
                        int j = 0;
                        while (i < xlen && j < ylen)
                        {
                            if (x[i].index == y[j].index)
                            {
                                double d = x[i++].value_Renamed - y[j++].value_Renamed;
                                sum += d * d;
                            }
                            else if (x[i].index > y[j].index)
                            {
                                sum += y[j].value_Renamed * y[j].value_Renamed;
                                ++j;
                            }
                            else
                            {
                                sum += x[i].value_Renamed * x[i].value_Renamed;
                                ++i;
                            }
                        }

                        while (i < xlen)
                        {
                            sum += x[i].value_Renamed * x[i].value_Renamed;
                            ++i;
                        }

                        while (j < ylen)
                        {
                            sum += y[j].value_Renamed * y[j].value_Renamed;
                            ++j;
                        }

                        return Math.Exp((-param.gamma) * sum);
                    }

                case svm_parameter.SIGMOID:
                    return tanh(param.gamma * dot(x, y) + param.coef0);

                default:
                    return 0; // java
            }
        }
    }
}
