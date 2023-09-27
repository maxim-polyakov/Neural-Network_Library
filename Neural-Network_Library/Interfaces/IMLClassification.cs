using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLClassification : IMLInputOutput
    {
        /// <summary>
        /// Classify the input into a group.
        /// </summary>
        ///
        /// <param name="input">The input data to classify.</param>
        /// <returns>The group that the data was classified into.</returns>
        int Classify(IMLData input);
    }
}
