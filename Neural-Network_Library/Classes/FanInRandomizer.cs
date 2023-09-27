using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class FanInRandomizer : BasicRandomizer
    {
        internal const String Error = "To use FanInRandomizer you must "
                                      + "present a Matrix or 2D array type value.";


        private const double DefaultBoundary = 2.4d;


        private readonly double _lowerBound;


        private readonly bool _sqrt;


        private readonly double _upperBound;

        public FanInRandomizer() : this(-DefaultBoundary, DefaultBoundary, false)
        {
        }


        public FanInRandomizer(double boundary, bool sqrt) : this(-boundary, boundary, sqrt)
        {
        }


        public FanInRandomizer(double aLowerBound, double anUpperBound,
                               bool sqrt)
        {
            _lowerBound = aLowerBound;
            _upperBound = anUpperBound;
            _sqrt = sqrt;
        }


        private double CalculateValue(int rows)
        {
            double rowValue = _sqrt ? Math.Sqrt(rows) : rows;

            return (_lowerBound / rowValue) + NextDouble()
                   * ((_upperBound - _lowerBound) / rowValue);
        }

        /// <summary>
        /// Throw an error if this class is used improperly.
        /// </summary>
        ///
        private static void CauseError()
        {

        }


        public override double Randomize(double d)
        {
            CauseError();
            return 0;
        }


        public override void Randomize(double[] d)
        {
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = CalculateValue(1);
            }
        }


        public override void Randomize(double[][] d)
        {
            foreach (double[] t in d)
            {
                for (var col = 0; col < d[0].Length; col++)
                {
                    t[col] = CalculateValue(d.Length);
                }
            }
        }


        public override void Randomize(Matrix m)
        {
            for (int row = 0; row < m.Rows; row++)
            {
                for (int col = 0; col < m.Cols; col++)
                {
                    m[row, col] = CalculateValue(m.Rows);
                }
            }
        }


        public override void Randomize(BasicNetwork network, int fromLayer)
        {
            int fromCount = network.GetLayerTotalNeuronCount(fromLayer);
            int toCount = network.GetLayerNeuronCount(fromLayer + 1);

            for (int fromNeuron = 0; fromNeuron < fromCount; fromNeuron++)
            {
                for (int toNeuron = 0; toNeuron < toCount; toNeuron++)
                {
                    double v = CalculateValue(toCount);
                    network.SetWeight(fromLayer, fromNeuron, toNeuron, v);
                }
            }
        }
    }
}
