using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class HiddenLayerParams
    {
        /// <summary>
        /// The maximum number of neurons on this layer.
        /// </summary>
        ///
        private readonly int _max;

        /// <summary>
        /// The minimum number of neurons on this layer.
        /// </summary>
        ///
        private readonly int _min;

        /// <summary>
        /// Construct a hidden layer param object with the specified min and max
        /// values.
        /// </summary>
        ///
        /// <param name="min">The minimum number of neurons.</param>
        /// <param name="max">The maximum number of neurons.</param>
        public HiddenLayerParams(int min, int max)
        {
            _min = min;
            _max = max;
        }


        /// <value>The maximum number of neurons.</value>
        public int Max
        {
            get { return _max; }
        }


        /// <value>The minimum number of neurons.</value>
        public int Min
        {
            get { return _min; }
        }
    }
}
