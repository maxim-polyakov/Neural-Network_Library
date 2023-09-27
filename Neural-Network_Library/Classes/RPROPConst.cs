using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public static class RPROPConst
    {
        /// <summary>
        /// The default zero tolerance.
        /// </summary>
        ///
        public const double DefaultZeroTolerance = 0.00000000000000001d;

        /// <summary>
        /// The POSITIVE ETA value. This is specified by the resilient propagation
        /// algorithm. This is the percentage by which the deltas are increased by if
        /// the partial derivative is greater than zero.
        /// </summary>
        ///
        public const double PositiveEta = 1.2d;

        /// <summary>
        /// The NEGATIVE ETA value. This is specified by the resilient propagation
        /// algorithm. This is the percentage by which the deltas are increased by if
        /// the partial derivative is less than zero.
        /// </summary>
        ///
        public const double NegativeEta = 0.5d;

        /// <summary>
        /// The minimum delta value for a weight matrix value.
        /// </summary>
        ///
        public const double DeltaMin = 1e-6d;

        /// <summary>
        /// The starting update for a delta.
        /// </summary>
        ///
        public const double DefaultInitialUpdate = 0.1d;

        /// <summary>
        /// The maximum amount a delta can reach.
        /// </summary>
        ///
        public const double DefaultMaxStep = 50;
    }
}
