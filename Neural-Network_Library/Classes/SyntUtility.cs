﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SyntUtility
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private SyntUtility()
        {
        }

        /// <summary>
        /// Convert a CSV file to a binary training file.
        /// </summary>
        /// <param name="csvFile">The CSV file.</param>
        /// <param name="format">The format.</param>
        /// <param name="binFile">The binary file.</param>
        /// <param name="inputCount">The number of input values.</param>
        /// <param name="outputCount">The number of output values.</param>
        /// <param name="headers">True, if there are headers on the3 CSV.</param>
        /// <param name="expectSignificance">Should a significance column be expected.</param>
        public static void ConvertCSV2Binary(String csvFile, CSVFormat format,
                                             String binFile, int inputCount, int outputCount,
                                             bool headers, bool expectSignificance)
        {
            new FileInfo(binFile).Delete();

            var csv = new CSVMLDataSet(csvFile,
                                       inputCount, outputCount, false, format, expectSignificance);
            var buffer = new BufferedMLDataSet(binFile);
            buffer.BeginLoad(inputCount, outputCount);
            foreach (IMLDataPair pair in csv)
            {
                buffer.Add(pair);
            }
            buffer.EndLoad();
        }

        /// <summary>
        /// Convert a CSV file to binary.
        /// </summary>
        /// <param name="csvFile">The CSV file to convert.</param>
        /// <param name="format">The format.</param>
        /// <param name="binFile">The binary file.</param>
        /// <param name="input">The input.</param>
        /// <param name="ideal">The ideal.</param>
        /// <param name="headers">True, if headers are present.</param>
        public static void ConvertCSV2Binary(FileInfo csvFile, CSVFormat format,
                                             FileInfo binFile, int[] input, int[] ideal, bool headers)
        {
            binFile.Delete();
            var csv = new ReadCSV(csvFile.ToString(), headers, format);

            var buffer = new BufferedMLDataSet(binFile.ToString());
            buffer.BeginLoad(input.Length, ideal.Length);
            while (csv.Next())
            {
                var inputData = new BasicMLData(input.Length);
                var idealData = new BasicMLData(ideal.Length);

                // handle input data
                for (int i = 0; i < input.Length; i++)
                {
                    inputData[i] = csv.GetDouble(input[i]);
                }

                // handle input data
                for (int i = 0; i < ideal.Length; i++)
                {
                    idealData[i] = csv.GetDouble(ideal[i]);
                }

                // add to dataset

                buffer.Add(inputData, idealData);
            }
            buffer.EndLoad();
            buffer.Close();
            csv.Close();
        }

        /// <summary>
        /// Load CSV to memory.
        /// </summary>
        /// <param name="filename">The CSV file to load.</param>
        /// <param name="input">The input count.</param>
        /// <param name="ideal">The ideal count.</param>
        /// <param name="headers">True, if headers are present.</param>
        /// <param name="format">The loaded dataset.</param>
        /// <param name="expectSignificance">The loaded dataset.</param>
        /// <returns></returns>
        public static IMLDataSet LoadCSV2Memory(String filename, int input, int ideal, bool headers, CSVFormat format, bool expectSignificance)
        {
            IDataSetCODEC codec = new CSVDataCODEC(filename, format, headers, input, ideal, expectSignificance);
            var load = new MemoryDataLoader(codec);
            IMLDataSet dataset = load.External2Memory();
            return dataset;
        }

        /// <summary>
        /// Evaluate the network and display (to the console) the output for every
        /// value in the training set. Displays ideal and actual.
        /// </summary>
        /// <param name="network">The network to evaluate.</param>
        /// <param name="training">The training set to evaluate.</param>
        public static void Evaluate(IMLRegression network,
                                    IMLDataSet training)
        {
            foreach (IMLDataPair pair in training)
            {
                IMLData output = network.Compute(pair.Input);
                Console.WriteLine(@"Input="
                                  + FormatNeuralData(pair.Input)
                                  + @", Actual=" + FormatNeuralData(output)
                                  + @", Ideal="
                                  + FormatNeuralData(pair.Ideal));
            }
        }

        /// <summary>
        /// Format neural data as a list of numbers.
        /// </summary>
        /// <param name="data">The neural data to format.</param>
        /// <returns>The formatted neural data.</returns>
        public static String FormatNeuralData(IMLData data)
        {
            var result = new StringBuilder();
            for (int i = 0; i < data.Count; i++)
            {
                if (i != 0)
                {
                    result.Append(',');
                }
                result.Append(Format.FormatDouble(data[i], 4));
            }
            return result.ToString();
        }

        /// <summary>
        /// Create a simple feedforward neural network.
        /// </summary>
        /// <param name="input">The number of input neurons.</param>
        /// <param name="hidden1">The number of hidden layer 1 neurons.</param>
        /// <param name="hidden2">The number of hidden layer 2 neurons.</param>
        /// <param name="output">The number of output neurons.</param>
        /// <param name="tanh">True to use hyperbolic tangent activation function, false to
        /// use the sigmoid activation function.</param>
        /// <returns>The neural network.</returns>
        public static BasicNetwork SimpleFeedForward(int input,
                                                     int hidden1, int hidden2, int output,
                                                     bool tanh)
        {
            var pattern = new FeedForwardPattern { InputNeurons = input, OutputNeurons = output };
            if (tanh)
            {
                pattern.ActivationFunction = new ActivationTANH();
            }
            else
            {
                pattern.ActivationFunction = new ActivationSigmoid();
            }

            if (hidden1 > 0)
            {
                pattern.AddHiddenLayer(hidden1);
            }
            if (hidden2 > 0)
            {
                pattern.AddHiddenLayer(hidden2);
            }

            var network = (BasicNetwork)pattern.Generate();
            network.Reset();
            return network;
        }

        /// <summary>
        /// Train the neural network, using SCG training, and output status to the
        /// console.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set.</param>
        /// <param name="minutes">The number of minutes to train for.</param>
        public static void TrainConsole(BasicNetwork network,
                                        IMLDataSet trainingSet, int minutes)
        {
            Propagation train = new ResilientPropagation(network,
                                                         trainingSet)
            { ThreadCount = 0 };
            TrainConsole(train, network, trainingSet, minutes);
        }


        /// <summary>
        /// Train the neural network, using SCG training, and output status to the
        /// console.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set.</param>
        /// <param name="seconds">The seconds.</param>
        public static void TrainConsole(BasicNetwork network,
                                        IMLDataSet trainingSet, double seconds)
        {
            Propagation train = new ResilientPropagation(network,
                                                         trainingSet)
            { ThreadCount = 0 };
            TrainConsole(train, network, trainingSet, seconds);
        }

        /// <summary>
        /// Train the network, using the specified training algorithm, and send the
        /// output to the console.
        /// </summary>
        /// <param name="train">The training method to use.</param>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set.</param>
        /// <param name="minutes">The number of minutes to train for.</param>
        public static void TrainConsole(IMLTrain train,
                                        BasicNetwork network, IMLDataSet trainingSet,
                                        int minutes)
        {
            int epoch = 1;
            long remaining;

            Console.WriteLine(@"Beginning training...");
            long start = Environment.TickCount;
            do
            {
                train.Iteration();

                long current = Environment.TickCount;
                long elapsed = (current - start) / 1000;
                remaining = minutes - elapsed / 60;

                Console.WriteLine(@"Iteration #" + Format.FormatInteger(epoch)
                                  + @" Error:" + Format.FormatPercent(train.Error)
                                  + @" elapsed time = " + Format.FormatTimeSpan((int)elapsed)
                                  + @" time left = "
                                  + Format.FormatTimeSpan((int)remaining * 60));
                epoch++;
            } while (remaining > 0 && !train.TrainingDone);
            train.FinishTraining();
        }


        /// <summary>
        /// Train the network, using the specified training algorithm, and send the
        /// output to the console.
        /// </summary>
        /// <param name="train">The training method to use.</param>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set.</param>
        /// <param name="seconds">The second to train for.</param>
        public static void TrainConsole(IMLTrain train, BasicNetwork network, IMLDataSet trainingSet, double seconds)
        {
            int epoch = 1;
            double remaining;

            Console.WriteLine(@"Beginning training...");
            long start = Environment.TickCount;
            do
            {
                train.Iteration();

                double current = Environment.TickCount;
                double elapsed = (current - start) / 1000;
                remaining = seconds - elapsed;

                Console.WriteLine(@"Iteration #" + Format.FormatInteger(epoch)
                                  + @" Error:" + Format.FormatPercent(train.Error)
                                  + @" elapsed time = " + Format.FormatTimeSpan((int)elapsed)
                                  + @" time left = "
                                  + Format.FormatTimeSpan((int)remaining));
                epoch++;
            } while (remaining > 0 && !train.TrainingDone);
            train.FinishTraining();
        }

        /// <summary>
        /// Train using RPROP and display progress to a dialog box.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set to use.</param>
        public static void TrainDialog(BasicNetwork network,
                                       IMLDataSet trainingSet)
        {
            Propagation train = new ResilientPropagation(network,
                                                         trainingSet)
            { ThreadCount = 0 };
            TrainDialog(train, network, trainingSet);
        }

        /// <summary>
        /// Train, using the specified training method, display progress to a dialog
        /// box.
        /// </summary>
        /// <param name="train">The training method to use.</param>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set to use.</param>
        public static void TrainDialog(IMLTrain train,
                                       BasicNetwork network, IMLDataSet trainingSet)
        {
            //   var dialog = new TrainingDialog { Train = train };
            //  dialog.ShowDialog();
        }

        /// <summary>
        /// Train the network, to a specific error, send the output to the console.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set to use.</param>
        /// <param name="error">The error level to train to.</param>
        public static void TrainToError(BasicNetwork network,
                                        IMLDataSet trainingSet, double error)
        {
            Propagation train = new ResilientPropagation(network,
                                                         trainingSet)
            { ThreadCount = 0 };
            TrainToError(train, trainingSet, error);
        }

        /// <summary>
        /// Train to a specific error, using the specified training method, send the
        /// output to the console.
        /// </summary>
        /// <param name="train">The training method.</param>
        /// <param name="trainingSet">The training set to use.</param>
        /// <param name="error">The desired error level.</param>
        public static void TrainToError(IMLTrain train,
                                        IMLDataSet trainingSet,
                                        double error)
        {
            int epoch = 1;

            Console.WriteLine(@"Beginning training...");

            do
            {
                train.Iteration();

                Console.WriteLine(@"Iteration #" + Format.FormatInteger(epoch)
                                  + @" Error:" + Format.FormatPercent(train.Error)
                                  + @" Target Error: " + Format.FormatPercent(error));
                epoch++;
            } while (train.Error > error && !train.TrainingDone);
            train.FinishTraining();
        }

        /// <summary>
        /// Calculate a regression error.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <param name="data">The data to check.</param>
        /// <returns>The error.</returns>
        public static double CalculateRegressionError(IMLRegression method,
                                                      IMLDataSet data)
        {
            var errorCalculation = new ErrorCalculation();
            if (method is IMLContext)
                ((IMLContext)method).ClearContext();


            foreach (IMLDataPair pair in data)
            {
                IMLData actual = method.Compute(pair.Input);
                errorCalculation.UpdateError(actual.Data, pair.Ideal.Data, pair.Significance);
            }
            return errorCalculation.Calculate();
        }

        /// <summary>
        /// Save the dataset to a CSV file.
        /// </summary>
        /// <param name="targetFile">The target file.</param>
        /// <param name="format">The format to use.</param>
        /// <param name="set">The data set.</param>
        public static void SaveCSV(FileInfo targetFile, CSVFormat format, IMLDataSet set)
        {

        }

        /// <summary>
        /// Calculate an error for a method that makes use of classification.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <param name="data">The data to check.</param>
        /// <returns>The error.</returns>
        public static double CalculateClassificationError(IMLClassification method,
                                                          IMLDataSet data)
        {
            int total = 0;
            int correct = 0;


            foreach (IMLDataPair pair in data)
            {
                var ideal = (int)pair.Ideal[0];
                int actual = method.Classify(pair.Input);
                if (actual == ideal)
                    correct++;
                total++;
            }
            return (total - correct) / (double)total;
        }

        /// <summary>
        /// Load an EGB file to memory.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <returns>A memory data set.</returns>
        public static IMLDataSet LoadEGB2Memory(FileInfo filename)
        {
            var buffer = new BufferedMLDataSet(filename.ToString());
            var result = buffer.LoadToMemory();
            buffer.Close();
            return result;
        }

        /// <summary>
        /// Train to a specific error, using the specified training method, send the
        /// output to the console.
        /// </summary>
        ///
        /// <param name="train">The training method.</param>
        /// <param name="error">The desired error level.</param>
        public static void TrainToError(IMLTrain train, double error)
        {

            int epoch = 1;

            Console.Out.WriteLine(@"Beginning training...");

            do
            {
                train.Iteration();

                Console.Out.WriteLine(@"Iteration #" + Format.FormatInteger(epoch)
                        + @" Error:" + Format.FormatPercent(train.Error)
                        + @" Target Error: " + Format.FormatPercent(error));
                epoch++;
            } while ((train.Error > error) && !train.TrainingDone);
            train.FinishTraining();
        }

        /// <summary>
        /// Save the training set to an EGB file.
        /// </summary>
        /// <param name="egbFile">The EGB file to save to.</param>
        /// <param name="data">The training data to save.</param>
        public static void SaveEGB(FileInfo egbFile, IMLDataSet data)
        {
            var binary = new BufferedMLDataSet(egbFile.ToString());
            binary.Load(data);
            data.Close();
        }
    }
}
