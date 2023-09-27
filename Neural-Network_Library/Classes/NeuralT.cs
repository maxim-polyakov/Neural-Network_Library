using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NeuralT : BasicT
    {
        /// <summary>
        /// The Q.
        /// </summary>
        ///
        private readonly Q _networkQ;

        /// <summary>
        /// Construct a neural T.
        /// </summary>
        ///
        /// <param name="network">The network to use.</param>
        public NeuralT(BasicNetwork network)
        {
            Organism = network;

            _networkQ = new Q();

            // create an array of "double genes"
            int size = network.Structure.CalculateSize();
            for (int i = 0; i < size; i++)
            {
                IGene gene = new DoubleGene();
                _networkQ.Genes.Add(gene);
            }

            Qs.Add(_networkQ);

            Syntesis();
        }

        /// <summary>
        /// Decode the Ts into a neural network.
        /// </summary>
        ///
        public override sealed void Decode()
        {
            var net = new double[_networkQ.Genes.Count];
            for (int i = 0; i < net.Length; i++)
            {
                var gene = (DoubleGene)_networkQ.Genes[i];
                net[i] = gene.Value;
            }
            NetworkCODEC.ArrayToNetwork(net, (BasicNetwork)Organism);
        }

        /// <summary>
        /// Syntesis the neural network into genes.
        /// </summary>
        ///
        public override sealed void Syntesis()
        {
            double[] net = NetworkCODEC
                .NetworkToArray((BasicNetwork)Organism);

            for (int i = 0; i < net.Length; i++)
            {
                ((DoubleGene)_networkQ.GetGene(i)).Value = net[i];
            }
        }
    }
}
