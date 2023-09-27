using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public abstract class BasicGene : IGene
    {
        /// <summary>
        /// Is this gene enabled?
        /// </summary>
        ///
        private bool _enabled;

        /// <summary>
        /// ID of this gene, -1 for unassigned.
        /// </summary>
        ///
        private long _id;

        /// <summary>
        /// Innovation ID, -1 for unassigned.
        /// </summary>
        ///
        private long _innovationId;

        /// <summary>
        /// Construct the object.
        /// </summary>
        protected BasicGene()
        {
            _enabled = true;
            _id = -1;
            _innovationId = -1;
        }

        #region IGene Members

        /// <inheritdoc/>
        public int CompareTo(IGene o)
        {
            return ((int)(InnovationId - o.InnovationId));
        }

        /// <summary>
        /// Set the id for this gene.
        /// </summary>
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }


        /// <summary>
        /// Set the innovation id for this gene.
        /// </summary>
        public long InnovationId
        {
            get { return _innovationId; }
            set { _innovationId = value; }
        }


        /// <value>True, if this gene is enabled.</value>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }


        /// <summary>
        /// from Synt.ml.G.genes.Gene
        /// </summary>
        ///
        public abstract void Copy(IGene gene);

        #endregion
    }
}
