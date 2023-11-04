using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public static class NormalizationActionExtension
    {
        /// <returns>True, if this is a classify.</returns>
        public static bool IsClassify(this NormalizationAction extensionParam)
        {
            return (extensionParam == NormalizationAction.OneOf) || (extensionParam == NormalizationAction.SingleField)
                   || (extensionParam == NormalizationAction.Equilateral);
        }
    }
}
