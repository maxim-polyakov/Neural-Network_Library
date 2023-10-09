using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NeuralDataMapping
    {
        /// <summary>
        /// Construct the neural data mapping class, with null values.
        /// </summary>
        ///
        public NeuralDataMapping()
        {
            From = null;
            To = null;
        }

        /// <summary>
        /// Construct the neural data mapping class with the specified values.
        /// </summary>
        ///
        /// <param name="f">The source data.</param>
        /// <param name="t">The target data.</param>
        public NeuralDataMapping(IMLData f, IMLData t)
        {
            From = f;
            To = t;
        }

        /// <summary>
        /// Set the from data.
        /// </summary>
        ///
        /// <value>The from data.</value>
        public IMLData From
        {
            get;
            set;
        }


        /// <summary>
        /// Set the target data.
        /// </summary>
        ///
        /// <value>The target data.</value>
        public IMLData To
        {
            get;
            set;
        }

        /// <summary>
        /// Copy from one object to the other.
        /// </summary>
        ///
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        public static void Copy(NeuralDataMapping source,
                                NeuralDataMapping target)
        {
            for (int i = 0; i < source.From.Count; i++)
            {
                target.From[i] = source.From[i];
            }

            for (int i_0 = 0; i_0 < source.To.Count; i_0++)
            {
                target.To[i_0] = source.To[i_0];
            }
        }
    }
}
