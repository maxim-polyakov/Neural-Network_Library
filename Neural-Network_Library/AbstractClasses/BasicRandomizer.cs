using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public abstract class BasicRandomizer
    {

        private Random _random;


        protected BasicRandomizer()
        {
            _random = new Random((int)(DateTime.Now.Ticks * 100));
        }


        /// <value>the random to set</value>
        public Random Random
        {
            get { return _random; }
            set { _random = value; }
        }

        #region IRandomizer Members


        public virtual void Randomize(double[] d)
        {
            Randomize(d, 0, d.Length);
        }


        public virtual void Randomize(double[] d, int begin, int size)
        {
            for (int i = 0; i < size; i++)
            {
                d[begin + i] = Randomize(d[begin + i]);
            }
        }


        public virtual void Randomize(double[][] d)
        {
            foreach (double[] t in d)
            {
                for (var c = 0; c < d[0].Length; c++)
                {
                    t[c] = Randomize(t[c]);
                }
            }
        }


        public virtual void Randomize(Matrix m)
        {
            double[][] d = m.Data;
            for (int r = 0; r < m.Rows; r++)
            {
                for (int c = 0; c < m.Cols; c++)
                {
                    d[r][c] = Randomize(d[r][c]);
                }
            }
        }


        public virtual void Randomize(IMLMethod method)
        {
            if (method is BasicNetwork)
            {
                var network = (BasicNetwork)method;
                for (int i = 0; i < network.LayerCount - 1; i++)
                {
                    Randomize(network, i);
                }
            }
            else if (method is IMLEncodable)
            {
                var Syntesis = (IMLEncodable)method;
                var Syntesisd = new double[Syntesis.SyntesisdArrayLength()];
                Syntesis.SyntesisToArray(Syntesisd);
                Randomize(Syntesisd);
                Syntesis.DecodeFromArray(Syntesisd);
            }
        }


        public abstract double Randomize(double d);

        #endregion


        public double NextDouble()
        {
            return _random.NextDouble();
        }


        public double NextDouble(double min, double max)
        {
            double range = max - min;
            return (range * _random.NextDouble()) + min;
        }

        public virtual void Randomize(BasicNetwork network, int fromLayer)
        {
            int fromCount = network.GetLayerTotalNeuronCount(fromLayer);
            int toCount = network.GetLayerNeuronCount(fromLayer + 1);

            for (int fromNeuron = 0; fromNeuron < fromCount; fromNeuron++)
            {
                for (int toNeuron = 0; toNeuron < toCount; toNeuron++)
                {
                    double v = network.GetWeight(fromLayer, fromNeuron, toNeuron);
                    v = Randomize(v);
                    network.SetWeight(fromLayer, fromNeuron, toNeuron, v);
                }
            }
        }
    }
}
