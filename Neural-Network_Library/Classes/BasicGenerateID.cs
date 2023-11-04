using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class BasicGenerateID : IGenerateID
    {
        /// <summary>
        /// Construct an ID generator.
        /// </summary>
        public BasicGenerateID()
        {
            CurrentID = 1;
        }

        #region IGenerateID Members

        /// <summary>
        /// The current id to generate.  This is the next id returned.
        /// </summary>
        public long CurrentID { get; set; }

        /// <summary>
        /// Generate a unique id.
        /// </summary>
        /// <returns>The unique id.</returns>
        public long Generate()
        {
            lock (this)
            {
                return CurrentID++;
            }
        }

        #endregion
    }
}
