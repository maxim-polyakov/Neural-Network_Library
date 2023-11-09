using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class TrainingSetUtil
    {
        /// <summary>
        /// Load a CSV file into a memory dataset.  
        /// </summary>
        ///
        /// <param name="format">The CSV format to use.</param>
        /// <param name="filename">The filename to load.</param>
        /// <param name="headers">True if there is a header line.</param>
        /// <param name="inputSize">The input size.  Input always comes first in a file.</param>
        /// <param name="idealSize">The ideal size, 0 for unsupervised.</param>
        /// <returns>A NeuralDataSet that holds the contents of the CSV file.</returns>
        public static IMLDataSet LoadCSVTOMemory(CSVFormat format, String filename,
                                                bool headers, int inputSize, int idealSize)
        {
            IMLDataSet result = new BasicMLDataSet();
            var csv = new ReadCSV(filename, headers, format);
            while (csv.Next())
            {
                IMLData ideal = null;
                int index = 0;

                IMLData input = new BasicMLData(inputSize);
                for (int i = 0; i < inputSize; i++)
                {
                    double d = csv.GetDouble(index++);
                    input[i] = d;
                }

                if (idealSize > 0)
                {
                    ideal = new BasicMLData(idealSize);
                    for (int i = 0; i < idealSize; i++)
                    {
                        double d = csv.GetDouble(index++);
                        ideal[i] = d;
                    }
                }

                IMLDataPair pair = new BasicMLDataPair(input, ideal);
                result.Add(pair);
            }

            return result;
        }

        /// <summary>
        /// Convert a training set to an array.
        /// </summary>
        /// <param name="training"></param>
        /// <returns></returns>
        public static ObjectPair<double[][], double[][]> TrainingToArray(
            IMLDataSet training)
        {
            var length = (int)training.Count;
            double[][] a = EngineArray.AllocateDouble2D(length, training.InputSize);
            double[][] b = EngineArray.AllocateDouble2D(length, training.IdealSize);

            int index = 0;

            foreach (IMLDataPair pair in training)
            {
                EngineArray.ArrayCopy(pair.InputArray, a[index]);
                EngineArray.ArrayCopy(pair.IdealArray, b[index]);
                index++;
            }

            return new ObjectPair<double[][], double[][]>(a, b);
        }
    }
}
