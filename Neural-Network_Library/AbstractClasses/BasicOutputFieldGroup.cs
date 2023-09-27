using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public abstract class BasicOutputFieldGroup : IOutputFieldGroup
    {
        /// <summary>
        /// The fields in this group.
        /// </summary>
        private readonly ICollection<OutputFieldGrouped> _fields = new List<OutputFieldGrouped>();

        #region IOutputFieldGroup Members

        /// <summary>
        /// Add a field to this group.
        /// </summary>
        /// <param name="field">The field to add to the group.</param>
        public void AddField(OutputFieldGrouped field)
        {
            _fields.Add(field);
        }

        /// <summary>
        /// The list of grouped fields.
        /// </summary>
        public ICollection<OutputFieldGrouped> GroupedFields
        {
            get { return _fields; }
        }

        /// <summary>
        /// Init for a new row.
        /// </summary>
        public abstract void RowInit();

        #endregion
    }
}
