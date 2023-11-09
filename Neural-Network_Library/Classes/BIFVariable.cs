using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BIFVariable
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Options for this variable.
        /// </summary>
        private IList<String> Options { get; set; }

        /// <summary>
        /// Construct the variable.
        /// </summary>
        public BIFVariable()
        {
            Options = new List<String>();
        }


        /// <summary>
        /// Add an option to the variable.
        /// </summary>
        /// <param name="s">The option to add.</param>
        public void AddOption(String s)
        {
            Options.Add(s);
        }
    }
}
