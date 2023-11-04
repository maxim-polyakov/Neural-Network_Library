using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class BasicInputField : IInputField
    {
        /// <summary>
        /// The minimum value encountered so far for this field.
        /// </summary>
        private double _max = Double.NegativeInfinity;

        /// <summary>
        /// The maximum value encountered so far for this field.
        /// </summary>
        private double _min = Double.PositiveInfinity;

        /// <summary>
        /// True if this field is used to actually generate the input for
        /// the neural network.
        /// </summary>
        private bool _usedForNetworkInput = true;

        #region IInputField Members

        /// <summary>
        /// Given the current value, apply to the min and max values.
        /// </summary>
        /// <param name="d">The current value.</param>
        public void ApplyMinMax(double d)
        {
            _min = Math.Min(_min, d);
            _max = Math.Max(_max, d);
        }


        /// <summary>
        /// The current value of the input field.  This is only valid, 
        /// while the normalization is being performed.
        /// </summary>
        public double CurrentValue { get; set; }

        /// <summary>
        /// The maximum value for all of the input data, this is calculated
        /// during the first pass of normalization.
        /// </summary>
        public double Max
        {
            get { return _max; }
            set { _max = value; }
        }

        /// <summary>
        /// The minimum value for all of the input data, this is calculated
        /// during the first pass of normalization.
        /// </summary>
        public double Min
        {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        /// Not supported for this sort of class, may be implemented in subclasses.
        /// Will throw an exception.
        /// </summary>
        /// <param name="i">The index.  Not used.</param>
        /// <returns>The value at the specified index.</returns>
        public virtual double GetValue(int i)
        {
            throw new NormalizationError("Can't call getValue on "
                                         + GetType().Name);
        }


        /// <summary>
        /// True, if this field is used for network input.  
        /// This is needed so that the buildForNetworkInput method of the 
        /// normalization class knows how many input fields to expect.  For instance, 
        /// fields used only to segregate data are not used for the actual network 
        /// input and may not be provided when the network is actually being queried.
        /// </summary>
        public bool UsedForNetworkInput
        {
            get { return _usedForNetworkInput; }
            set { _usedForNetworkInput = value; }
        }

        #endregion
    }
}
