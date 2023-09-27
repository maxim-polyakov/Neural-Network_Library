using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLDataPair : ICloneable, ICentroidFactory<IMLDataPair>
    {
        /// <summary>
        /// The input that the neural network.
        /// </summary>
        IMLData Input { get; }

        /// <summary>
        /// The ideal data that the neural network should produce
        /// for the specified input.
        /// </summary>
        IMLData Ideal { get; }

        /// <summary>
        /// True if this training pair is supervised.  That is, it has 
        /// both input and ideal data.
        /// </summary>
        bool Supervised { get; }

        /// <summary>
        /// The supervised ideal data.
        /// </summary>
        double[] IdealArray { get; set; }

        /// <summary>
        /// The input array.
        /// </summary>
        double[] InputArray { get; set; }

        /// <summary>
        /// The significance of this training element.
        /// </summary>
        double Significance { get; set; }
    }
}
