using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class MexicanHatFunction : BasicRBF
    {


        public MexicanHatFunction(int dimensions)
        {
            Centers = new double[dimensions];
            Peak = 1.0;
            Width = 1.0;
        }


        public MexicanHatFunction(double peak, double[] center,
                                  double width)
        {
            Centers = center;
            Peak = peak;
            Width = width;
        }



        public MexicanHatFunction(double center, double peak,
                                  double width)
        {
            Centers = new double[1];
            Centers[0] = center;
            Peak = peak;
            Width = width;
        }



        public override double Calculate(double[] x)
        {
            double[] center = Centers;


            double norm = 0;
            for (int i = 0; i < center.Length; i++)
            {
                norm += Math.Pow(x[i] - center[i], 2);
            }

            // calculate the value

            return Peak * (1 - norm) * Math.Exp(-norm / 2);
        }
    }
}
