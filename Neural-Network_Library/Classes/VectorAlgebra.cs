using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class VectorAlgebra
    {
        static Random rand = new Random();


        public void Add(double[] v1, double[] v2)
        {
            for (int i = 0; i < v1.Length; i++)
            {
                v1[i] += v2[i];
            }
        }


        public void Sub(double[] v1, double[] v2)
        {
            for (int i = 0; i < v1.Length; i++)
            {
                v1[i] -= v2[i];
            }
        }


        public void Neg(double[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = -v[i];
            }
        }


        public void MulRand(double[] v, double k)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] *= k * rand.NextDouble();
            }
        }


        public void Mul(double[] v, double k)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] *= k;
            }
        }

        public void Copy(double[] dst, double[] src)
        {
            EngineArray.ArrayCopy(src, dst);
        }

        public void Randomise(double[] v)
        {
            Randomise(v, 0.1);
        }


        public void Randomise(double[] v, double maxValue)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = (2 * rand.NextDouble() - 1) * maxValue;
            }
        }


        public void ClampComponents(double[] v, double maxValue)
        {
            if (maxValue != -1)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    if (v[i] > maxValue) v[i] = maxValue;
                    if (v[i] < -maxValue) v[i] = -maxValue;
                }
            }
        }

    }
}
