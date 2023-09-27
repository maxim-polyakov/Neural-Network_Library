using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IOutputFieldGroup
    {
        /// <summary>
        /// All of the output fields in this group.
        /// </summary>
        ICollection<OutputFieldGrouped> GroupedFields { get; }

        /// <summary>
        /// Add an output field to the group.
        /// </summary>
        /// <param name="field">The field to add.</param>
        void AddField(OutputFieldGrouped field);

        /// <summary>
        /// Init the group for a new row.
        /// </summary>
        void RowInit();
    }
}
