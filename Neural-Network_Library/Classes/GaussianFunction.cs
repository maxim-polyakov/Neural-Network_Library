﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class GaussianFunction : BasicRBF
    {

        public GaussianFunction()
        {
            Centers = new double[1];
            Peak = 1;
            Width = 1;
        }


        public GaussianFunction(int dimensions)
        {
            Centers = new double[dimensions];
            Peak = 1.0;
            Width = 1.0;
        }

        public GaussianFunction(double peak, double[] center,
                                double width)
        {
            Centers = center;
            Peak = peak;
            Width = width;
        }

        public GaussianFunction(double center, double peak,
                                double width)
        {
            Centers = new double[1];
            Centers[0] = center;
            Peak = peak;
            Width = width;
        }


        public override double Calculate(double[] x)
        {
            double value = 0;
            double[] center = Centers;
            double width = Width;

            for (int i = 0; i < center.Length - 1; i++)
            {
                value += Math.Pow(x[i] - center[i], 2)
                         / (2.0 * width * width);
            }
            return Peak * Math.Exp(-value);
        }
    }
}
