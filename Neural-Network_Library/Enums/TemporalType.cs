using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public enum TemporalType
    {
        /// <summary>
        /// This field is used as part of the input. However, if you wish to use the
        /// field for prediction as well, specify InputAndPredict.
        /// </summary>
        ///
        Input,

        /// <summary>
        /// This field is used as part of the prediction. However, if you wish to use
        /// the field for input as well, specify InputAndPredict.
        /// </summary>
        ///
        Predict,

        /// <summary>
        /// This field is used for both input and prediction.
        /// </summary>
        ///
        InputAndPredict,

        /// <summary>
        /// This field should be ignored.
        /// </summary>
        ///
        Ignore,

        /// <summary>
        /// This field should pass through, to the output file, without modification.
        /// </summary>
        ///
        PassThrough
    }
}
