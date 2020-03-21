using System;


namespace RBF
{

    public class RBFN
    {

        static double[][] XORInput = {
            new[] {0.001, 0.0,0.1},
            new[] {2.0, 0.0,0.1},
            new[] {0.0, 1.0,0.1},
            new[] {1.0, 1.0, 0.1 }


        };



        static double[][] XORIdeal = {

            new[] {0.0},
            new[] {1.0},
            new[] {1.0},
            new[] {0.0}

        };


        public RBFN(int sizeX, params int[] W)
        {
            XORInput = new double[1][];
            XORIdeal = new double[1][];
            int sizeY = W[W.Length - 1];
            Random rnd1 = new Random();
            double[] XORInput_layer = new double[sizeX];
            for (int i = 0; i < sizeX; i++) XORInput_layer[i] = rnd1.NextDouble() - 0.5;
            XORInput[0] = XORInput_layer;


            Random rnd2 = new Random();
            double[] XORIdeal_layer = new double[sizeY];
            for (int i = 0; i < sizeY; i++) XORIdeal_layer[i] = rnd2.NextDouble() - 0.5;
            XORIdeal[0] = XORIdeal_layer;
        }



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

