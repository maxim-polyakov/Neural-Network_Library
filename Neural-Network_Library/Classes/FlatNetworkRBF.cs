using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class FlatNetworkRBF : FlatNetwork
    {


        private IRadialBasisFunction[] _rbf;


        public FlatNetworkRBF()
        {
        }


        public FlatNetworkRBF(int inputCount, int hiddenCount,
                              int outputCount, IRadialBasisFunction[] rbf)
        {
            var layers = new FlatLayer[3];
            _rbf = rbf;

            layers[0] = new FlatLayer(new ActivationLinear(), inputCount, 0.0d);
            layers[1] = new FlatLayer(new ActivationLinear(), hiddenCount, 0.0d);
            layers[2] = new FlatLayer(new ActivationLinear(), outputCount, 0.0d);

            Init(layers);
        }


        public IRadialBasisFunction[] RBF
        {
            get { return _rbf; }
            set { _rbf = value; }
        }


        public override sealed Object Clone()
        {
            var result = new FlatNetworkRBF();
            CloneFlatNetwork(result);
            result._rbf = _rbf;
            return result;
        }


        public override sealed void Compute(double[] x, double[] output)
        {
            int outputIndex = LayerIndex[1];

            for (int i = 0; i < _rbf.Length; i++)
            {
                double o = _rbf[i].Calculate(x);
                LayerOutput[outputIndex + i] = o;
            }

            // now compute the output
            ComputeLayer(1);
            EngineArray.ArrayCopy(LayerOutput, 0, output, 0,
                                  OutputCount);
        }
    }
}
