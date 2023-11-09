using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class TemporalDataDescription
    {
        #region Type enum

        /// <summary>
        /// The type of data requested.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Data in its raw, unmodified form.
            /// </summary>
            Raw,
            /// <summary>
            /// The percent change.
            /// </summary>
            PercentChange,
            /// <summary>
            /// The difference change.
            /// </summary>
            DeltaChange,
        }

        #endregion

        /// <summary>
        /// Should an activation function be used?
        /// </summary>
        private readonly IActivationFunction _activationFunction;

        /// <summary>
        /// What type of data is requested?
        /// </summary>
        private readonly Type _type;

        /// <summary>
        /// Construct a data description item. Set both low and high to zero for
        /// unbounded.
        /// </summary>
        /// <param name="activationFunction">What activation function should be used?</param>
        /// <param name="low">What is the lowest allowed value.</param>
        /// <param name="high">What is the highest allowed value.</param>
        /// <param name="type">What type of data is this.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(IActivationFunction activationFunction,
                                       double low, double high, Type type,
                                       bool input, bool predict)
        {
            Low = low;
            _type = type;
            High = high;
            IsInput = input;
            IsPredict = predict;
            _activationFunction = activationFunction;
        }

        /// <summary>
        /// Construct a data description with an activation function, but no range.
        /// </summary>
        /// <param name="activationFunction">The activation function.</param>
        /// <param name="type">The type of data.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(IActivationFunction activationFunction,
                                       Type type, bool input, bool predict)
            : this(activationFunction, 0, 0, type, input, predict)
        {
        }

        /// <summary>
        /// Construct a data description with no activation function or range.
        /// </summary>
        /// <param name="type">The type of data.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(Type type, bool input,
                                       bool predict)
            : this(null, 0, 0, type, input, predict)
        {
        }

        /// <summary>
        /// The lowest allowed data.
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// The highest allowed value.
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// Is this data input?  Or is it to be predicted.
        /// </summary>
        public bool IsInput { get; set; }

        /// <summary>
        /// Determine if this is a predicted value.
        /// </summary>
        public bool IsPredict { get; set; }

        /// <summary>
        /// Get the index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The type of data this is.
        /// </summary>
        public Type DescriptionType
        {
            get { return _type; }
        }

        /// <summary>
        /// The activation function for this layer.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            get { return _activationFunction; }
        }
    }
}
