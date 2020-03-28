using System;


namespace RBF
{

    public class RBFN
    {

        static double[][] XORInput = {
           
            new[] {0.0, 0.0, 0.1},
            new[] {0.0, 0.1, 0.0},
            new[] {0.1, 0.0, 0.0}


        };



        static double[][] XORIdeal = {

          
            new[] {1.0},
            new[] {1.0},
            new[] {0.0}

        };


   



        class Program
        {

            static void Main(string[] args)
            {

                int dimension = 2;
                int numNeuronsPerDimension = 4;
                double volumeNeuronWidth = 2.0 / numNeuronsPerDimension; ;
                bool includeEdgeRBFs = true;

                RBFNetwork n = new RBFNetwork(dimension, numNeuronsPerDimension, 1, RBFEnum.Gaussian);
                n.SetRBFCentersAndWidthsEqualSpacing(0, 1, RBFEnum.Gaussian, volumeNeuronWidth, includeEdgeRBFs);


                INeuralDataSet trainingSet = new BasicNeuralDataSet(XORInput, XORIdeal);
                SVDTraining train = new SVDTraining(n, trainingSet);

                int epoch = 1;
                do
                {
                    train.Iteration();
                    Console.WriteLine(@"Epoch #" + epoch + @" Error:" + train.Error);
                    epoch++;
                } while ((epoch < 1) && (train.Error > 0.001));

                Console.WriteLine(@"Neural Network Results:");
                foreach (IMLDataPair pair in trainingSet)
                {
                    IMLData output = n.Compute(pair.Input);
                    Console.WriteLine(pair.Input[0] + @"," + pair.Input[1]
                                      + @", actual=" + output[0] + @",ideal=" + pair.Ideal[0]);
                }

                Console.Read();
            }
        }
    }
}

