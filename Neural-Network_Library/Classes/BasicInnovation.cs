using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class BasicInnovation : IInnovation
    {
        #region IInnovation Members

        /// <summary>
        /// Set the innovation id.
        /// </summary>
        public long InnovationID
        {
            get;
            set;
        }

        #endregion
    }
}
