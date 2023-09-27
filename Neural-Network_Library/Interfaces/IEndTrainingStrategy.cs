using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IEndTrainingStrategy : IStrategy
    {
        /// <returns>True if training should stop.</returns>
        bool ShouldStop();
    }
}
