using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class BasicInnovationList : IInnovationList
    {
        /// <summary>
        /// The list of innovations.
        /// </summary>
        ///
        private readonly IList<IInnovation> _list;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public BasicInnovationList()
        {
            _list = new List<IInnovation>();
        }

        #region IInnovationList Members

        /// <summary>
        /// Add an innovation.
        /// </summary>
        ///
        /// <param name="innovation">The innovation to add.</param>
        public void Add(IInnovation innovation)
        {
            _list.Add(innovation);
        }

        /// <summary>
        /// Get a specific innovation, by index.
        /// </summary>
        ///
        /// <param name="id">The innovation index id.</param>
        /// <returns>The innovation.</returns>
        public IInnovation Get(int id)
        {
            return _list[id];
        }


        /// <value>A list of innovations.</value>
        public IList<IInnovation> Innovations
        {
            get { return _list; }
        }

        #endregion
    }
}
