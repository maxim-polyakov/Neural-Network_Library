using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NguyenWidrowRandomizer
    {

        public static String MSG = "This type of randomization is not supported by Nguyen-Widrow";


        public void Randomize(IMLMethod method)
        {
            if (!(method is BasicNetwork))
            {


                BasicNetwork network = (BasicNetwork)method;

                for (int fromLayer = 0; fromLayer < network.LayerCount - 1; fromLayer++)
                {
                    RandomizeSynapse(network, fromLayer);
                }

            }


            double CalculateRange(IActivationFunction af, double r)
            {
                double[] d = { r };
                af.ActivationFunction(d, 0, 1);
                return d[0];
            }

            void RandomizeSynapse(BasicNetwork network, int fromLayer)
            {
                int toLayer = fromLayer + 1;
                int toCount = network.GetLayerNeuronCount(toLayer);
                int fromCount = network.GetLayerNeuronCount(fromLayer);
                int fromCountTotalCount = network.GetLayerTotalNeuronCount(fromLayer);
                IActivationFunction af = network.GetActivation(toLayer);
                double low = CalculateRange(af, Double.NegativeInfinity);
                double high = CalculateRange(af, Double.PositiveInfinity);

                double b = 0.7d * Math.Pow(toCount, (1d / fromCount)) / (high - low);

                for (int toNeuron = 0; toNeuron < toCount; toNeuron++)
                {
                    if (fromCount != fromCountTotalCount)
                    {
                        double w = RangeRandomizer.Randomize(-b, b);
                        network.SetWeight(fromLayer, fromCount, toNeuron, w);
                    }
                    for (int fromNeuron = 0; fromNeuron < fromCount; fromNeuron++)
                    {
                        double w = RangeRandomizer.Randomize(0, b);
                        network.SetWeight(fromLayer, fromNeuron, toNeuron, w);
                    }
                }
            }


        }
    }
}
