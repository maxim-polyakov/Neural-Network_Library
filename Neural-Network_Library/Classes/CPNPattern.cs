﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class CPNPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The tag for the INSTAR layer.
        /// </summary>
        ///
        public const String TagInstar = "INSTAR";

        /// <summary>
        /// The tag for the OUTSTAR layer.
        /// </summary>
        ///
        public const String TagOutstar = "OUTSTAR";

        /// <summary>
        /// The number of neurons in the hidden layer.
        /// </summary>
        ///
        private int _inputCount;

        /// <summary>
        /// The number of neurons in the instar layer.
        /// </summary>
        ///
        private int _instarCount;

        /// <summary>
        /// The number of neurons in the outstar layer.
        /// </summary>
        ///
        private int _outstarCount;

        /// <summary>
        /// Set the number of neurons in the instar layer. This level is essentially
        /// a hidden layer.
        /// </summary>
        public int InstarCount
        {
            set { _instarCount = value; }
        }

        /// <summary>
        /// Set the number of neurons in the outstar level, this level is mapped to
        /// the "output" level.
        /// </summary>
        public int OutstarCount
        {
            set { _outstarCount = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Not used, will throw an error. CPN networks already have a predefined
        /// hidden layer called the instar layer.
        /// </summary>
        ///
        /// <param name="count">NOT USED</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError(
                "A CPN already has a predefined hidden layer.  No additional"
                + "specification is needed.");
        }

        /// <summary>
        /// Clear any parameters that were set.
        /// </summary>
        ///
        public void Clear()
        {
            _inputCount = 0;
            _instarCount = 0;
            _outstarCount = 0;
        }

        /// <summary>
        /// Generate the network.
        /// </summary>
        ///
        /// <returns>The generated network.</returns>
        public IMLMethod Generate()
        {
            return new CPNNetwork(_inputCount, _instarCount, _outstarCount, 1);
        }

        /// <summary>
        /// This method will throw an error. The CPN network uses predefined
        /// activation functions.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A CPN network will use the BiPolar & competitive activation "
                    + "functions, no activation function needs to be specified.");
            }
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            set { _inputCount = value; }
        }


        /// <summary>
        /// Set the number of output neurons. Calling this method maps to setting the
        /// number of neurons in the outstar layer.
        /// </summary>
        public int OutputNeurons
        {
            set { _outstarCount = value; }
        }

        #endregion
    }
}
