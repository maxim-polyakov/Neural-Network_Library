using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface ILearningRate
    {
        /// <summary>
        /// Set the learning rate.
        /// </summary>
        double LearningRate
        {
            get;
            set;
        }
    }
}
