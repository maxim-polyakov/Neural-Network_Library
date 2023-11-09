using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class RPROPFactory
    {
        /// <summary>
        /// Create a RPROP trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is IContainsFlat))
            {
                throw new SyntError(
                    "RPROP training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            double initialUpdate = holder.GetDouble(
                MLTrainFactory.PropertyInitialUpdate, false,
                RPROPConst.DefaultInitialUpdate);
            double maxStep = holder.GetDouble(
                MLTrainFactory.PropertyMaxStep, false,
                RPROPConst.DefaultMaxStep);

            return new ResilientPropagation((IContainsFlat)method, training,
                                            initialUpdate, maxStep);
        }
    }
}
