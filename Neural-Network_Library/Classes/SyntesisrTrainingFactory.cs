using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SyntesisrTrainingFactory
    {
        /// <summary>
        /// Generate an Syntesisr training set over the range [0.0,1.0].  This is the range used by
        /// Fahlman.
        /// </summary>
        /// <param name="inputCount">The number of inputs and outputs.</param>
        /// <param name="compl">True if the complement mode should be use.</param>
        /// <returns>The training set.</returns>
        public static IMLDataSet generateTraining(int inputCount, bool compl)
        {
            return GenerateTraining(inputCount, compl, 0, 1.0);
        }

        /// <summary>
        /// Generate an Syntesisr over the specified range. 
        /// </summary>
        /// <param name="inputCount">The number of inputs and outputs.</param>
        /// <param name="compl">True if the complement mode should be use. </param>
        /// <param name="min">The minimum value to use(i.e. 0 or -1)</param>
        /// <param name="max">The maximum value to use(i.e. 1 or 0)</param>
        /// <returns>The training set.</returns>
        public static IMLDataSet GenerateTraining(int inputCount, bool compl, double min, double max)
        {
            return GenerateTraining(inputCount, compl, min, max, min, max);
        }


        public static IMLDataSet GenerateTraining(int inputCount, bool compl, double inputMin, double inputMax, double outputMin, double outputMax)
        {
            double[][] input = EngineArray.AllocateDouble2D(inputCount, inputCount);
            double[][] ideal = EngineArray.AllocateDouble2D(inputCount, inputCount);

            for (int i = 0; i < inputCount; i++)
            {
                for (int j = 0; j < inputCount; j++)
                {
                    if (compl)
                    {
                        input[i][j] = (j == i) ? inputMax : inputMin;
                        ideal[i][j] = (j == i) ? outputMin : outputMax;
                    }
                    else
                    {
                        input[i][j] = (j == i) ? inputMax : inputMin;
                        ideal[i][j] = (j == i) ? inputMax : inputMin;
                    }
                }
            }
            return new BasicMLDataSet(input, ideal);
        }
    }
}
