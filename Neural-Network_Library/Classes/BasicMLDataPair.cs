﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class BasicMLDataPair : IMLDataPair
    {
        /// <summary>
        /// The the expected output from the neural network, or null
        /// for unsupervised training.
        /// </summary>
        private readonly IMLData _ideal;

        /// <summary>
        /// The training input to the neural network.
        /// </summary>
        private readonly IMLData _input;

        /// <summary>
        /// The significance.
        /// </summary>
        private double _significance = 1.0;

        /// <summary>
        /// Construct a BasicMLDataPair class with the specified input
        /// and ideal values.
        /// </summary>
        /// <param name="input">The input to the neural network.</param>
        /// <param name="ideal">The expected results from the neural network.</param>
        public BasicMLDataPair(IMLData input, IMLData ideal)
        {
            _input = input;
            _ideal = ideal;
        }

        /// <summary>
        /// Construct a data pair that only includes input. (unsupervised)
        /// </summary>
        /// <param name="input">The input data.</param>
        public BasicMLDataPair(IMLData input)
        {
            _input = input;
            _ideal = null;
        }

        /// <summary>
        /// The input data.
        /// </summary>
        public virtual IMLData Input
        {
            get { return _input; }
        }

        /// <summary>
        /// The ideal data.
        /// </summary>
        public virtual IMLData Ideal
        {
            get { return _ideal; }
        }

        /// <summary>
        /// Convert object to a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append('[');
            result.Append("Input:");
            result.Append(Input);
            result.Append(",Ideal:");
            result.Append(Ideal);
            result.Append(']');
            return result.ToString();
        }

        /// <summary>
        /// Deterimine if this pair is supervised or unsupervised.
        /// </summary>
        /// <returns>True if this is a supervised pair.</returns>
        public bool IsSupervised
        {
            get { return _ideal != null; }
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public object Clone()
        {
            Object result;

            if (Ideal == null)
                result = new BasicMLDataPair((IMLData)_input.Clone());
            else
                result = new BasicMLDataPair((IMLData)_input.Clone(),
                                             (IMLData)_ideal.Clone());

            return result;
        }

        /// <summary>
        /// Create a new neural data pair object of the correct size for the neural
        /// network that is being trained. This object will be passed to the getPair
        /// method to allow the neural data pair objects to be copied to it.
        /// </summary>
        /// <param name="inputSize">The size of the input data.</param>
        /// <param name="idealSize">The size of the ideal data.</param>
        /// <returns>A new neural data pair object.</returns>
        public static IMLDataPair CreatePair(int inputSize, int idealSize)
        {
            IMLDataPair result;

            if (idealSize > 0)
            {
                result = new BasicMLDataPair(new BasicMLData(inputSize),
                                             new BasicMLData(idealSize));
            }
            else
            {
                result = new BasicMLDataPair(new BasicMLData(inputSize));
            }

            return result;
        }

        /// <summary>
        /// The supervised ideal data.
        /// </summary>
        public double[] IdealArray
        {
            get
            {
                return _ideal == null ? null : _ideal.Data;
            }
            set { _ideal.Data = value; }
        }

        /// <summary>
        /// The input array.
        /// </summary>
        public double[] InputArray
        {
            get { return _input.Data; }
            set { _input.Data = value; }
        }

        /// <summary>
        /// Returns true, if supervised.
        /// </summary>
        public bool Supervised
        {
            get { return _ideal != null; }
        }

        /// <summary>
        /// The significance of this training element.
        /// </summary>
        public double Significance
        {
            get { return _significance; }
            set { _significance = value; }
        }

        /// <inheritdoc/>
        public ICentroid<IMLDataPair> CreateCentroid()
        {
            if (!(Input is BasicMLData))
            {
                throw new SyntError("The input data type of " + Input.GetType().Name + " must be BasicMLData.");
            }
            return new BasicMLDataPairCentroid(this);
        }
    }
}
