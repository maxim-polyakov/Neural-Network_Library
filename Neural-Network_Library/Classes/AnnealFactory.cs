using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class AnnealFactory
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
            double startTemp = holder.GetDouble(
                MLTrainFactory.PropertyTemperatureStart, false, 10);
            double stopTemp = holder.GetDouble(
                MLTrainFactory.PropertyTemperatureStop, false, 2);

            int cycles = holder.GetInt(MLTrainFactory.Cycles, false, 100);

            IMLTrain train = new NeuralSimulatedAnnealing(
                (BasicNetwork)method, score, startTemp, stopTemp, cycles);

            return train;
        }
    }
}
