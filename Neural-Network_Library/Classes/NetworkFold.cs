using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NetworkFold
    {
        /// <summary>
        /// The output for this fold.
        /// </summary>
        ///
        private readonly double[] _output;

        /// <summary>
        /// The weights for this fold.
        /// </summary>
        ///
        private readonly double[] _weights;

        /// <summary>
        /// Construct a fold from the specified flat network.
        /// </summary>
        ///
        /// <param name="flat">THe flat network.</param>
        public NetworkFold(FlatNetwork flat)
        {
            _weights = EngineArray.ArrayCopy(flat.Weights);
            _output = EngineArray.ArrayCopy(flat.LayerOutput);
        }


        /// <value>The network weights.</value>
        public double[] Weights
        {
            get { return _weights; }
        }


        /// <value>The network output.</value>
        public double[] Output
        {
            get { return _output; }
        }

        /// <summary>
        /// Copy weights and output to the network.
        /// </summary>
        ///
        /// <param name="target">The network to copy to.</param>
        public void CopyToNetwork(FlatNetwork target)
        {
            EngineArray.ArrayCopy(_weights, target.Weights);
            EngineArray.ArrayCopy(_output, target.LayerOutput);
        }

        /// <summary>
        /// Copy the weights and output from the network.
        /// </summary>
        ///
        /// <param name="source">The network to copy from.</param>
        public void CopyFromNetwork(FlatNetwork source)
        {
            EngineArray.ArrayCopy(source.Weights, _weights);
            EngineArray.ArrayCopy(source.LayerOutput, _output);
        }
    }
}
