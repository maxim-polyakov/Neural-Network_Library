using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class BasicART : BasicML
    {
        /// <summary>
        /// Neural network property, the A1 parameter.
        /// </summary>
        ///
        public const String PropertyA1 = "A1";

        /// <summary>
        /// Neural network property, the B1 parameter.
        /// </summary>
        ///
        public const String PropertyB1 = "B1";

        /// <summary>
        /// Neural network property, the C1 parameter.
        /// </summary>
        ///
        public const String PropertyC1 = "C1";

        /// <summary>
        /// Neural network property, the D1 parameter.
        /// </summary>
        ///
        public const String PropertyD1 = "D1";

        /// <summary>
        /// Neural network property, the L parameter.
        /// </summary>
        ///
        public const String PropertyL = "L";

        /// <summary>
        /// Neural network property, the vigilance parameter.
        /// </summary>
        ///
        public const String PropertyVigilance = "VIGILANCE";

        /// <summary>
        /// Neural network property for no winner.
        /// </summary>
        ///
        public const String PropertyNoWinner = "noWinner";

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void UpdateProperties()
        {
            // unneeded
        }
    }
}
