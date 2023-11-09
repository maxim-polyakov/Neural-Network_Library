using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class GFactory
    {
        /// <summary>
        /// Create an annealing trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is BasicNetwork))
            {
                throw new TrainingError(
                    "Invalid method type, requires BasicNetwork");
            }

            ICalculateScore score = new TrainingSetScore(training);

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            int populationSize = holder.GetInt(
                MLTrainFactory.PropertyPopulationSize, false, 5000);
            double mutation = holder.GetDouble(
                MLTrainFactory.PropertyMutation, false, 0.1d);
            double mate = holder.GetDouble(MLTrainFactory.PropertyMate,
                                           false, 0.25d);

            IMLTrain train = new NeuralGAlgorithm((BasicNetwork)method,
                                                       (IRandomizer)(new RangeRandomizer(-1, 1)), score, populationSize, mutation,
                                                       mate);

            return train;
        }
    }
}
