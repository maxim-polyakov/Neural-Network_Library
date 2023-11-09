using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ObjectPair<TA, TB>
    {
        /// <summary>
        /// The first object.
        /// </summary>
        private readonly TA _a;

        /// <summary>
        /// The second object.
        /// </summary>
        private readonly TB _b;

        /// <summary>
        /// Construct an object pair. 
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        public ObjectPair(TA a, TB b)
        {
            _a = a;
            _b = b;
        }

        /// <summary>
        /// The first object.
        /// </summary>
        public TA A
        {
            get { return _a; }
        }

        /// <summary>
        /// The second object.
        /// </summary>
        public TB B
        {
            get { return _b; }
        }
    }
}
