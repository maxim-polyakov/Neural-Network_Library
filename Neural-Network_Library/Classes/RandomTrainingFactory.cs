using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class RandomTrainingFactory
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private RandomTrainingFactory()
        {
        }

        /// <summary>
        /// Generate a random training set. 
        /// </summary>
        /// <param name="seed">The seed value to use, the same seed value will always produce
        /// the same results.</param>
        /// <param name="count">How many training items to generate.</param>
        /// <param name="inputCount">How many input numbers.</param>
        /// <param name="idealCount">How many ideal numbers.</param>
        /// <param name="min">The minimum random number.</param>
        /// <param name="max">The maximum random number.</param>
        /// <returns>The random training set.</returns>
        public static BasicMLDataSet Generate(long seed,
                                              int count, int inputCount,
                                              int idealCount, double min, double max)
        {
            var rand =
                new LinearCongruentialGenerator(seed);

            var result = new BasicMLDataSet();
            for (int i = 0; i < count; i++)
            {
                IMLData inputData = new BasicMLData(inputCount);

                for (int j = 0; j < inputCount; j++)
                {
                    inputData.Data[j] = rand.Range(min, max);
                }

                IMLData idealData = new BasicMLData(idealCount);

                for (int j = 0; j < idealCount; j++)
                {
                    idealData[j] = rand.Range(min, max);
                }

                var pair = new BasicMLDataPair(inputData,
                                               idealData);
                result.Add(pair);
            }
            return result;
        }

        /// <summary>
        /// Generate random training into a training set.
        /// </summary>
        /// <param name="training">The training set to generate into.</param>
        /// <param name="seed">The seed to use.</param>
        /// <param name="count">How much data to generate.</param>
        /// <param name="min">The low random value.</param>
        /// <param name="max">The high random value.</param>
        public static void Generate(IMLDataSet training,
                                    long seed,
                                    int count,
                                    double min, double max)
        {
            var rand
                = new LinearCongruentialGenerator(seed);

            int inputCount = training.InputSize;
            int idealCount = training.IdealSize;

            for (int i = 0; i < count; i++)
            {
                IMLData inputData = new BasicMLData(inputCount);

                for (int j = 0; j < inputCount; j++)
                {
                    inputData[j] = rand.Range(min, max);
                }

                IMLData idealData = new BasicMLData(idealCount);

                for (int j = 0; j < idealCount; j++)
                {
                    idealData[j] = rand.Range(min, max);
                }

                var pair = new BasicMLDataPair(inputData,
                                               idealData);
                training.Add(pair);
            }
        }
    }
}
