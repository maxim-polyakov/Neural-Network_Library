using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PSOFactory
    {
        /// <summary>
        /// Create a PSO trainer.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            ParamsHolder holder = new ParamsHolder(args);

            int particles = holder.GetInt(
                    MLTrainFactory.PropertyParticles, false, 20);

            ICalculateScore score = new TrainingSetScore(training);
            IRandomizer randomizer = (IRandomizer)(new NguyenWidrowRandomizer());

            IMLTrain train = new NeuralPSO((BasicNetwork)method, randomizer, score, particles);

            return train;
        }
    }
}
