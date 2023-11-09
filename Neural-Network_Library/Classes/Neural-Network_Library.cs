using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Collections;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace Neural_Network_Library
{    
    //_____________
    public class FeedforwardFactory
    {
        /// <summary>
        /// Error.
        /// </summary>
        ///
        public const String CantDefineAct = "Can't define activation function before first layer.";

        /// <summary>
        /// The activation function factory to use.
        /// </summary>
        private MLActivationFactory _factory = new MLActivationFactory();

        /// <summary>
        /// Create a feed forward network.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The feedforward network.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            var result = new BasicNetwork();
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            IActivationFunction af = new ActivationLinear();

            int questionPhase = 0;

            foreach (String layerStr in layers)
            {
                // determine default
                int defaultCount = questionPhase == 0 ? input : output;

                ArchitectureLayer layer = ArchitectureParse.ParseLayer(
                    layerStr, defaultCount);
                bool bias = layer.Bias;

                String part = layer.Name;
                part = part != null ? part.Trim() : "";

                IActivationFunction lookup = _factory.Create(part);

                if (lookup != null)
                {
                    af = lookup;
                }
                else
                {
                    if (layer.UsedDefault)
                    {
                        questionPhase++;
                        if (questionPhase > 2)
                        {
                            throw new SyntError("Only two ?'s may be used.");
                        }
                    }

                    if (layer.Count == 0)
                    {
                        throw new SyntError("Unknown architecture element: "
                                             + architecture + ", can't parse: " + part);
                    }

                    result.AddLayer(new BasicLayer(af, bias, layer.Count));
                }
            }

            result.Structure.FinalizeStructure();
            result.Reset();

            return result;
        }
    }

    public class PNNFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MaxLayers = 3;

        /// <summary>
        /// Create a PNN network.
        /// </summary>
        ///
        /// <param name="architecture">THe architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The RBF network.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MaxLayers)
            {
                throw new SyntError(
                    "PNN Networks must have exactly three elements, "
                    + "separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer pnnLayer = ArchitectureParse.ParseLayer(
                layers[1], -1);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            PNNKernelType kernel;
            PNNOutputMode outmodel;

            if (pnnLayer.Name.Equals("c", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Classification;
            }
            else if (pnnLayer.Name.Equals("r", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Regression;
            }
            else if (pnnLayer.Name.Equals("u", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Unsupervised;
            }
            else
            {
                throw new NeuralNetworkError("Unknown model: " + pnnLayer.Name);
            }

            var holder = new ParamsHolder(pnnLayer.Params);

            String kernelStr = holder.GetString("KERNEL", false, "gaussian");

            if (kernelStr.Equals("gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                kernel = PNNKernelType.Gaussian;
            }
            else if (kernelStr.Equals("reciprocal", StringComparison.InvariantCultureIgnoreCase))
            {
                kernel = PNNKernelType.Reciprocal;
            }
            else
            {
                throw new NeuralNetworkError("Unknown kernel: " + kernelStr);
            }

            var result = new BasicPNN(kernel, outmodel, inputCount,
                                      outputCount);

            return result;
        }
    }

    public class RBFNetworkFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MaxLayers = 3;

        /// <summary>
        /// Create a RBF network.
        /// </summary>
        ///
        /// <param name="architecture">THe architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The RBF network.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MaxLayers)
            {
                throw new SyntError(
                    "RBF Networks must have exactly three elements, "
                    + "separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer rbfLayer = ArchitectureParse.ParseLayer(
                layers[1], -1);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            RBFEnum t;

            if (rbfLayer.Name.Equals("Gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Gaussian;
            }
            else if (rbfLayer.Name.Equals("Multiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Multiquadric;
            }
            else if (rbfLayer.Name.Equals("InverseMultiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.InverseMultiquadric;
            }
            else if (rbfLayer.Name.Equals("MexicanHat", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.MexicanHat;
            }
            else
            {
                throw new NeuralNetworkError("Unknown RBF: " + rbfLayer.Name);
            }

            var holder = new ParamsHolder(rbfLayer.Params);

            int rbfCount = holder.GetInt("C", true, 0);

            var result = new RBFNetwork(inputCount, rbfCount,
                                        outputCount, t);

            return result;
        }
    }

    public class SOMFactory
    {
        /// <summary>
        /// Create a SOM.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SOM.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != 2)
            {
                throw new SyntError(
                    "SOM's must have exactly two elements, separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[1], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            var pattern = new SOMPattern { InputNeurons = inputCount, OutputNeurons = outputCount };
            return pattern.Generate();
        }
    }

    public class SRNFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MaxLayers = 3;

        /// <summary>
        /// Create the SRN.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SRN.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MaxLayers)
            {
                throw new SyntError(
                    "SRN Networks must have exactly three elements, "
                    + "separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer rbfLayer = ArchitectureParse.ParseLayer(
                layers[1], -1);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            RBFEnum t;

            if (rbfLayer.Name.Equals("Gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Gaussian;
            }
            else if (rbfLayer.Name.Equals("Multiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Multiquadric;
            }
            else if (rbfLayer.Name.Equals("InverseMultiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.InverseMultiquadric;
            }
            else if (rbfLayer.Name.Equals("MexicanHat", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.MexicanHat;
            }
            else
            {
                t = RBFEnum.Gaussian;
            }

            var result = new RBFNetwork(inputCount,
                                        rbfLayer.Count, outputCount, t);

            return result;
        }
    }

    public class SVMFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MAX_LAYERS = 3;

        /// <summary>
        /// Create the SVM.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SVM.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MAX_LAYERS)
            {
                throw new SyntError(
                    "SVM's must have exactly three elements, separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer paramsLayer = ArchitectureParse.ParseLayer(
                layers[1], input);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            String name = paramsLayer.Name;
            String kernelStr = paramsLayer.Params.ContainsKey("KERNEL") ? paramsLayer.Params["KERNEL"] : null;
            String svmTypeStr = paramsLayer.Params.ContainsKey("TYPE") ? paramsLayer.Params["TYPE"] : null;

            SVMType svmType = SVMType.NewSupportVectorClassification;
            KernelType kernelType = KernelType.RadialBasisFunction;

            bool useNew = true;

            if (svmTypeStr == null)
            {
                useNew = true;
            }
            else if (svmTypeStr.Equals("NEW", StringComparison.InvariantCultureIgnoreCase))
            {
                useNew = true;
            }
            else if (svmTypeStr.Equals("OLD", StringComparison.InvariantCultureIgnoreCase))
            {
                useNew = false;
            }
            else
            {
                throw new SyntError("Unsupported type: " + svmTypeStr
                                     + ", must be NEW or OLD.");
            }

            if (name.Equals("C", StringComparison.InvariantCultureIgnoreCase))
            {
                if (useNew)
                {
                    svmType = SVMType.NewSupportVectorClassification;
                }
                else
                {
                    svmType = SVMType.SupportVectorClassification;
                }
            }
            else if (name.Equals("R", StringComparison.InvariantCultureIgnoreCase))
            {
                if (useNew)
                {
                    svmType = SVMType.NewSupportVectorRegression;
                }
                else
                {
                    svmType = SVMType.EpsilonSupportVectorRegression;
                }
            }
            else
            {
                throw new SyntError("Unsupported mode: " + name
                                     + ", must be C for classify or R for regression.");
            }

            if (kernelStr == null)
            {
                kernelType = KernelType.RadialBasisFunction;
            }
            else if ("linear".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Linear;
            }
            else if ("poly".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Poly;
            }
            else if ("precomputed".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Precomputed;
            }
            else if ("rbf".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.RadialBasisFunction;
            }
            else if ("sigmoid".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Sigmoid;
            }
            else
            {
                throw new SyntError("Unsupported kernel: " + kernelStr
                                     + ", must be linear,poly,precomputed,rbf or sigmoid.");
            }

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            if (outputCount != 1)
            {
                throw new SyntError("SVM can only have an output size of 1.");
            }

            var result = new SupportVectorMachine(inputCount, svmType, kernelType);

            return result;
        }
    }

    public class ArchitectureLayer
    {
        /// <summary>
        /// Holds any paramaters that were specified for the layer.
        /// </summary>
        ///
        private readonly IDictionary<String, String> _paras;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public ArchitectureLayer()
        {
            _paras = new Dictionary<String, String>();
        }


        /// <value>the count to set</value>
        public int Count { get; set; }


        /// <value>the name to set</value>
        public String Name { get; set; }


        /// <value>the params</value>
        public IDictionary<String, String> Params
        {
            get { return _paras; }
        }


        /// <value>the bias to set</value>
        public bool Bias { get; set; }


        /// <value>the usedDefault to set</value>
        public bool UsedDefault { get; set; }
    }

    public static class ArchitectureParse
    {
        /// <summary>
        /// parse a layer.
        /// </summary>
        ///
        /// <param name="line">The line to parse.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The parsed ArchitectureLayer.</returns>
        public static ArchitectureLayer ParseLayer(String line,
                                                   int defaultValue)
        {
            var layer = new ArchitectureLayer();

            String check = line.Trim().ToUpper();

            // first check for bias
            if (check.EndsWith(":B"))
            {
                check = check.Substring(0, (check.Length - 2) - (0));
                layer.Bias = true;
            }

            // see if simple number
            try
            {
                layer.Count = Int32.Parse(check);
                if (layer.Count < 0)
                {
                    throw new SyntError("Count cannot be less than zero.");
                }
            }
            catch (FormatException f)
            {
                SyntLogging.Log(f);
            }

            // see if it is a default
            if ("?".Equals(check))
            {
                if (defaultValue < 0)
                {
                    throw new SyntError("Default (?) in an invalid location.");
                }
                layer.Count = defaultValue;
                layer.UsedDefault = true;
                return layer;
            }

            // single item, no function
            int startIndex = check.IndexOf('(');
            int endIndex = check.LastIndexOf(')');
            if (startIndex == -1)
            {
                layer.Name = check;
                return layer;
            }

            // function
            if (endIndex == -1)
            {
                throw new SyntError("Illegal parentheses.");
            }

            layer.Name = check.Substring(0, (startIndex) - (0)).Trim();

            String paramStr = check.Substring(startIndex + 1, (endIndex) - (startIndex + 1));
            IDictionary<String, String> paras = ParseParams(paramStr);
            EngineArray.PutAll(paras, layer.Params);
            return layer;
        }

        /// <summary>
        /// Parse all layers from a line of text.
        /// </summary>
        ///
        /// <param name="line">The line of text.</param>
        /// <returns>A list of the parsed layers.</returns>
        public static IList<String> ParseLayers(String line)
        {
            IList<String> result = new List<String>();

            int bs = 0;
            bool done = false;

            do
            {
                String part;
                int index = line.IndexOf("->", bs);
                if (index != -1)
                {
                    part = line.Substring(bs, (index) - (bs)).Trim();
                    bs = index + 2;
                }
                else
                {
                    part = line.Substring(bs).Trim();
                    done = true;
                }

                bool bias = part.EndsWith("b");
                if (bias)
                {
                    part = part.Substring(0, (part.Length - 1) - (0));
                }

                result.Add(part);
            } while (!done);

            return result;
        }

        /// <summary>
        /// Parse a name.
        /// </summary>
        ///
        /// <param name="parser">The parser to use.</param>
        /// <returns>The name.</returns>
        private static String ParseName(SimpleParser parser)
        {
            var result = new StringBuilder();
            parser.EatWhiteSpace();
            while (parser.IsIdentifier())
            {
                result.Append(parser.ReadChar());
            }
            return result.ToString();
        }

        /// <summary>
        /// Parse parameters.
        /// </summary>
        ///
        /// <param name="line">The line to parse.</param>
        /// <returns>The parsed values.</returns>
        public static IDictionary<String, String> ParseParams(String line)
        {
            IDictionary<String, String> result = new Dictionary<String, String>();

            var parser = new SimpleParser(line);

            while (!parser.EOL())
            {
                String name = ParseName(parser)
                    .ToUpper();

                parser.EatWhiteSpace();
                if (!parser.LookAhead("=", false))
                {
                    throw new SyntError("Missing equals(=) operator.");
                }
                parser.Advance();

                String v = ParseValue(parser);

                result[name.ToUpper()] = v;

                if (!parser.ParseThroughComma())
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Parse a value.
        /// </summary>
        ///
        /// <param name="parser">The parser to use.</param>
        /// <returns>The newly parsed value.</returns>
        private static String ParseValue(SimpleParser parser)
        {
            bool quoted = false;
            var str = new StringBuilder();

            parser.EatWhiteSpace();

            if (parser.Peek() == '\"')
            {
                quoted = true;
                parser.Advance();
            }

            while (!parser.EOL())
            {
                if (parser.Peek() == '\"')
                {
                    if (quoted)
                    {
                        parser.Advance();
                        if (parser.Peek() == '\"')
                        {
                            str.Append(parser.ReadChar());
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        str.Append(parser.ReadChar());
                    }
                }
                else if (!quoted
                         && (parser.IsWhiteSpace() || (parser.Peek() == ',')))
                {
                    break;
                }
                else
                {
                    str.Append(parser.ReadChar());
                }
            }
            return str.ToString();
        }
    }

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

    public class BackPropFactory
    {
        /// <summary>
        /// Create a backpropagation trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 0.7d);
            double momentum = holder.GetDouble(
                MLTrainFactory.PropertyLearningMomentum, false, 0.3d);

            return new Backpropagation((BasicNetwork)method, training,
                                       learningRate, momentum);
        }
    }

    public class ClusterSOMFactory
    {
        /// <summary>
        /// Create a cluster SOM trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is SOMNetwork))
            {
                throw new SyntError(
                    "Cluster SOM training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new SOMClusterCopyTraining((SOMNetwork)method, training);
        }
    }

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

    public class LMAFactory
    {
        /// <summary>
        /// Create a LMA trainer.
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
                throw new SyntError(
                    "LMA training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            var result = new LevenbergMarquardtTraining(
                (BasicNetwork)method, training);
            return result;
        }
    }

    public class ManhattanFactory
    {
        /// <summary>
        /// Create a Manhattan trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 0.1d);

            return new ManhattanPropagation((BasicNetwork)method, training,
                                            learningRate);
        }
    }

    public class NeighborhoodSOMFactory
    {
        /// <summary>
        /// Create a LMA trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is SOMNetwork))
            {
                throw new SyntError(
                    "Neighborhood training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 0.7d);
            String neighborhoodStr = holder.GetString(
                MLTrainFactory.PropertyNeighborhood, false, "rbf");
            String rbfTypeStr = holder.GetString(
                MLTrainFactory.PropertyRBFType, false, "gaussian");

            RBFEnum t;

            if (rbfTypeStr.Equals("Gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Gaussian;
            }
            else if (rbfTypeStr.Equals("Multiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Multiquadric;
            }
            else if (rbfTypeStr.Equals("InverseMultiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.InverseMultiquadric;
            }
            else if (rbfTypeStr.Equals("MexicanHat", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.MexicanHat;
            }
            else
            {
                t = RBFEnum.Gaussian;
            }

            INeighborhoodFunction nf = null;

            if (neighborhoodStr.Equals("bubble", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodBubble(1);
            }
            else if (neighborhoodStr.Equals("rbf", StringComparison.InvariantCultureIgnoreCase))
            {
                String str = holder.GetString(
                    MLTrainFactory.PropertyDimensions, true, null);
                int[] size = NumberList.FromListInt(CSVFormat.EgFormat, str);
                nf = new NeighborhoodRBF(size, t);
            }
            else if (neighborhoodStr.Equals("rbf1d", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodRBF1D(t);
            }
            if (neighborhoodStr.Equals("single", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodSingle();
            }

            var result = new BasicTrainSOM((SOMNetwork)method,
                                           learningRate, training, nf);

            if (args.ContainsKey(MLTrainFactory.PropertyIterations))
            {
                int plannedIterations = holder.GetInt(
                    MLTrainFactory.PropertyIterations, false, 1000);
                double startRate = holder.GetDouble(
                    MLTrainFactory.PropertyStartLearningRate, false, 0.05d);
                double endRate = holder.GetDouble(
                    MLTrainFactory.PropertyEndLearningRate, false, 0.05d);
                double startRadius = holder.GetDouble(
                    MLTrainFactory.PropertyStartRadius, false, 10);
                double endRadius = holder.GetDouble(
                    MLTrainFactory.PropertyEndRadius, false, 1);
                result.SetAutoDecay(plannedIterations, startRate, endRate,
                                    startRadius, endRadius);
            }

            return result;
        }
    }

    public class NelderMeadFactory
    {
        ///// <summary>
        ///// Create a Nelder Mead trainer.
        ///// </summary>
        ///// <param name="method">The method to use.</param>
        ///// <param name="training">The training data to use.</param>
        ///// <param name="argsStr">The arguments to use.</param>
        ///// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            //final double learningRate = holder.getDouble(
            //		MLTrainFactory.PROPERTY_LEARNING_RATE, false, 0.1);

            return new NelderMeadTraining((BasicNetwork)method, training);
        }
    }

    public class PNNTrainFactory
    {
        /// <summary>
        /// Create a PNN trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String args)
        {
            if (!(method is BasicPNN))
            {
                throw new SyntError(
                    "PNN training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new TrainBasicPNN((BasicPNN)method, training);
        }
    }

    public class YTraining : GAlgorithm, IMLTrain
    {
        /// <summary>
        /// The number of inputs.
        /// </summary>
        private readonly int inputCount;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        private readonly int outputCount;

        /// <summary>
        /// The average fit adjustment.
        /// </summary>
        private double averageFitAdjustment;

        /// <summary>
        /// The best ever network.
        /// </summary>
        private YNetwork bestEverNetwork;

        /// <summary>
        /// The best ever score.
        /// </summary>
        private double bestEverScore;

        /// <summary>
        /// The iteration number.
        /// </summary>
        private int iteration;

        /// <summary>
        /// The activation mutation rate.
        /// </summary>
        private double paramActivationMutationRate = 0.1;

        /// <summary>
        /// The likelyhood of adding a link.
        /// </summary>
        private double paramChanceAddLink = 0.07;

        /// <summary>
        /// The likelyhood of adding a node.
        /// </summary>
        private double paramChanceAddNode = 0.04;

        /// <summary>
        /// The likelyhood of adding a recurrent link.
        /// </summary>
        private double paramChanceAddRecurrentLink = 0.05;

        /// <summary>
        /// The compatibility threshold for a species.
        /// </summary>
        private double paramCompatibilityThreshold = 0.26;

        /// <summary>
        /// The crossover rate.
        /// </summary>
        private double paramCrossoverRate = 0.7;

        /// <summary>
        /// The max activation perturbation.
        /// </summary>
        private double paramMaxActivationPerturbation = 0.1;

        /// <summary>
        /// The maximum number of species.
        /// </summary>
        private int paramMaxNumberOfSpecies;

        /// <summary>
        /// The maximum number of neurons.
        /// </summary>
        private double paramMaxPermittedNeurons = 100;

        /// <summary>
        /// The maximum weight perturbation.
        /// </summary>
        private double paramMaxWeightPerturbation = 0.5;

        /// <summary>
        /// The mutation rate.
        /// </summary>
        private double paramMutationRate = 0.2;

        /// <summary>
        /// The number of link add attempts.
        /// </summary>
        private int paramNumAddLinkAttempts = 5;

        /// <summary>
        /// The number of generations allowed with no improvement.
        /// </summary>
        private int paramNumGensAllowedNoImprovement = 15;

        /// <summary>
        /// The number of tries to find a looped link.
        /// </summary>
        private int paramNumTrysToFindLoopedLink = 5;

        /// <summary>
        /// The number of tries to find an old link.
        /// </summary>
        private int paramNumTrysToFindOldLink = 5;

        /// <summary>
        /// The probability that the weight will be totally replaced.
        /// </summary>
        private double paramProbabilityWeightReplaced = 0.1;

        /// <summary>
        /// Determines if we are using snapshot mode.
        /// </summary>
        private bool snapshot;

        /// <summary>
        /// The total fit adjustment.
        /// </summary>
        private double totalFitAdjustment;

        /// <summary>
        /// Construct a Y trainer with a new population. The new population is
        /// created from the specified parameters.
        /// </summary>
        /// <param name="calculateScore">The score calculation object.</param>
        /// <param name="inputCount">The input neuron count.</param>
        /// <param name="outputCount">The output neuron count.</param>
        /// <param name="populationSize">The population size.</param>
        public YTraining(ICalculateScore calculateScore,
                            int inputCount, int outputCount,
                            int populationSize)
        {
            this.inputCount = inputCount;
            this.outputCount = outputCount;

            CalculateScore = new GScoreAdapter(calculateScore);
            Comparator = new TComparator(CalculateScore);
            Population = new YPopulation(inputCount, outputCount,
                                            populationSize);

            Init();
        }

        /// <summary>
        /// Construct Y training with an existing population.
        /// </summary>
        /// <param name="calculateScore">The score object to use.</param>
        /// <param name="population">The population to use.</param>
        public YTraining(ICalculateScore calculateScore,
                            IPopulation population)
        {
            if (population.Size() < 1)
            {
                throw new TrainingError("Population can not be empty.");
            }

            var T = (YT)population.Ts[0];
            CalculateScore = new GScoreAdapter(calculateScore);
            Comparator = new TComparator(CalculateScore);
            Population = (population);
            inputCount = T.InputCount;
            outputCount = T.OutputCount;

            Init();
        }

        /// <summary>
        /// The innovations.
        /// </summary>
        public YInnovationList Innovations
        {
            get { return (YInnovationList)Population.Innovations; }
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get { return inputCount; }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public int OutputCount
        {
            get { return outputCount; }
        }

        /// <summary>
        /// Set the activation mutation rate.
        /// </summary>
        public double ParamActivationMutationRate
        {
            get { return paramActivationMutationRate; }
            set { paramActivationMutationRate = value; }
        }


        /// <summary>
        /// Set the chance to add a link.
        /// </summary>
        public double ParamChanceAddLink
        {
            get { return paramChanceAddLink; }
            set { paramChanceAddLink = value; }
        }


        /// <summary>
        /// Set the chance to add a node.
        /// </summary>
        public double ParamChanceAddNode
        {
            get { return paramChanceAddNode; }
            set { paramChanceAddNode = value; }
        }

        /// <summary>
        /// Set the chance to add a recurrent link.
        /// </summary>
        public double ParamChanceAddRecurrentLink
        {
            get { return paramChanceAddRecurrentLink; }
            set { paramChanceAddRecurrentLink = value; }
        }


        /// <summary>
        /// Set the compatibility threshold for species.
        /// </summary>
        public double ParamCompatibilityThreshold
        {
            get { return paramCompatibilityThreshold; }
            set { paramCompatibilityThreshold = value; }
        }


        /// <summary>
        /// Set the cross over rate.
        /// </summary>
        public double ParamCrossoverRate
        {
            get { return paramCrossoverRate; }
            set { paramCrossoverRate = value; }
        }


        /// <summary>
        /// Set the max activation perturbation.
        /// </summary>
        public double ParamMaxActivationPerturbation
        {
            get { return paramMaxActivationPerturbation; }
            set { paramMaxActivationPerturbation = value; }
        }

        /// <summary>
        /// Set the maximum number of species.
        /// </summary>
        public int ParamMaxNumberOfSpecies
        {
            get { return paramMaxNumberOfSpecies; }
            set { paramMaxNumberOfSpecies = value; }
        }

        /// <summary>
        /// Set the max permitted neurons.
        /// </summary>
        public double ParamMaxPermittedNeurons
        {
            get { return paramMaxPermittedNeurons; }
            set { paramMaxPermittedNeurons = value; }
        }

        /// <summary>
        /// Set the max weight perturbation.
        /// </summary>
        public double ParamMaxWeightPerturbation
        {
            get { return paramMaxWeightPerturbation; }
            set { paramMaxWeightPerturbation = value; }
        }

        /// <summary>
        /// Set the mutation rate.
        /// </summary>
        public double ParamMutationRate
        {
            get { return paramMutationRate; }
            set { paramMutationRate = value; }
        }

        /// <summary>
        /// Set the number of attempts to add a link.
        /// </summary>
        public int ParamNumAddLinkAttempts
        {
            get { return paramNumAddLinkAttempts; }
            set { paramNumAddLinkAttempts = value; }
        }

        /// <summary>
        /// Set the number of no-improvement generations allowed.
        /// </summary>
        public int ParamNumGensAllowedNoImprovement
        {
            get { return paramNumGensAllowedNoImprovement; }
            set { paramNumGensAllowedNoImprovement = value; }
        }

        /// <summary>
        /// Set the number of tries to create a looped link.
        /// </summary>
        public int ParamNumTrysToFindLoopedLink
        {
            get { return paramNumTrysToFindLoopedLink; }
            set { paramNumTrysToFindLoopedLink = value; }
        }


        /// <summary>
        /// Set the number of tries to try an old link.
        /// </summary>
        public int ParamNumTrysToFindOldLink
        {
            get { return paramNumTrysToFindOldLink; }
            set { paramNumTrysToFindOldLink = value; }
        }


        /// <summary>
        /// Set the probability to replace a weight.
        /// </summary>
        public double ParamProbabilityWeightReplaced
        {
            get { return paramProbabilityWeightReplaced; }
            set { paramProbabilityWeightReplaced = value; }
        }

        /// <summary>
        /// Set if we are using snapshot mode.
        /// </summary>
        public bool Snapshot
        {
            get { return snapshot; }
            set { snapshot = value; }
        }

        #region MLTrain Members

        /// <inheritdoc/>
        public void AddStrategy(IStrategy strategy)
        {
            throw new TrainingError(
                "Strategies are not supported by this training method.");
        }

        /// <inheritdoc/>
        public bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public void FinishTraining()
        {
        }

        /// <summary>
        /// The error for the best T.
        /// </summary>
        public double Error
        {
            get { return bestEverScore; }
            set { bestEverScore = value; }
        }

        /// <inheritdoc/>
        public TrainingImplementationType ImplementationType
        {
            get { return TrainingImplementationType.Iterative; }
        }

        /// <inheritdoc/>
        public int IterationNumber
        {
            get { return iteration; }
            set { iteration = value; }
        }

        /// <summary>
        /// A network created for the best T.
        /// </summary>
        public IMLMethod Method
        {
            get { return bestEverNetwork; }
        }

        /// <inheritdoc/>
        public IList<IStrategy> Strategies
        {
            get { return new List<IStrategy>(); }
        }

        /// <summary>
        /// Returns null, does not use a training set, rather uses a score function.
        /// </summary>
        public IMLDataSet Training
        {
            get { return null; }
        }

        /// <inheritdoc/>
        public bool TrainingDone
        {
            get { return false; }
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {
            iteration++;
            IList<YT> newPop = new List<YT>();

            int numSpawnedSoFar = 0;

            foreach (ISpecies s in Population.Species)
            {
                if (numSpawnedSoFar < Population.Size())
                {
                    var numToSpawn = (int)Math.Round(s.NumToSpawn);

                    bool bChosenBestYet = false;

                    while ((numToSpawn--) > 0)
                    {
                        YT baby = null;

                        if (!bChosenBestYet)
                        {
                            baby = (YT)s.Leader;

                            bChosenBestYet = true;
                        }

                        else
                        {
                            // if the number of individuals in this species is only
                            // one
                            // then we can only perform mutation
                            if (s.Members.Count == 1)
                            {
                                // spawn a child
                                baby = new YT((YT)s.ChooseParent());
                            }
                            else
                            {
                                var g1 = (YT)s.ChooseParent();

                                if (ThreadSafeRandom.NextDouble() < paramCrossoverRate)
                                {
                                    var g2 = (YT)s.ChooseParent();

                                    int numAttempts = 5;

                                    while ((g1.TID == g2.TID)
                                           && ((numAttempts--) > 0))
                                    {
                                        g2 = (YT)s.ChooseParent();
                                    }

                                    if (g1.TID != g2.TID)
                                    {
                                        baby = Crossover(g1, g2);
                                    }
                                }

                                else
                                {
                                    baby = new YT(g1);
                                }
                            }

                            if (baby != null)
                            {
                                baby.TID = Population.AssignTID();

                                if (baby.Neurons.Size() < paramMaxPermittedNeurons)
                                {
                                    baby.AddNeuron(paramChanceAddNode,
                                                   paramNumTrysToFindOldLink);
                                }

                                // now there's the chance a link may be added
                                baby.AddLink(paramChanceAddLink,
                                             paramChanceAddRecurrentLink,
                                             paramNumTrysToFindLoopedLink,
                                             paramNumAddLinkAttempts);

                                // mutate the weights
                                baby.MutateWeights(paramMutationRate,
                                                   paramProbabilityWeightReplaced,
                                                   paramMaxWeightPerturbation);

                                baby.MutateActivationResponse(
                                    paramActivationMutationRate,
                                    paramMaxActivationPerturbation);
                            }
                        }

                        if (baby != null)
                        {
                            // sort the baby's genes by their innovation numbers
                            baby.SortGenes();

                            // add to new pop
                            // if (newPop.contains(baby)) {
                            // throw new SyntError("readd");
                            // }
                            newPop.Add(baby);

                            ++numSpawnedSoFar;

                            if (numSpawnedSoFar == Population.Size())
                            {
                                numToSpawn = 0;
                            }
                        }
                    }
                }
            }

            while (newPop.Count < Population.Size())
            {
                newPop.Add(TournamentSelection(Population.Size() / 5));
            }

            Population.Clear();
            foreach (YT T in newPop)
            {
                Population.Add(T);
            }

            ResetAndKill();
            SortAndRecord();
            SpeciateAndCalculateSpawnLevels();
        }

        /// <inheritdoc/>
        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        /// <inheritdoc/>
        public TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public void Resume(TrainingContinuation state)
        {
        }

        #endregion

        /// <summary>
        /// Add the specified neuron id.
        /// </summary>
        /// <param name="nodeID">The neuron to add.</param>
        /// <param name="vec">The list to add to.</param>
        public void AddNeuronID(long nodeID, IList<long> vec)
        {
            for (int i = 0; i < vec.Count; i++)
            {
                if (vec[i] == nodeID)
                {
                    return;
                }
            }

            vec.Add(nodeID);

            return;
        }

        /// <summary>
        /// Adjust the compatibility threshold.
        /// </summary>
        public void AdjustCompatibilityThreshold()
        {
            // has this been disabled (unlimited species)
            if (paramMaxNumberOfSpecies < 1)
            {
                return;
            }

            double thresholdIncrement = 0.01;

            if (Population.Species.Count > paramMaxNumberOfSpecies)
            {
                paramCompatibilityThreshold += thresholdIncrement;
            }

            else if (Population.Species.Count < 2)
            {
                paramCompatibilityThreshold -= thresholdIncrement;
            }
        }

        /// <summary>
        /// Adjust each species score.
        /// </summary>
        public void AdjustSpeciesScore()
        {
            foreach (ISpecies s in Population.Species)
            {
                // loop over all Ts and adjust scores as needed
                foreach (IT member in s.Members)
                {
                    double score = member.Score;

                    // apply a youth bonus
                    if (s.Age < Population.YoungBonusAgeThreshold)
                    {
                        score = Comparator.ApplyBonus(score,
                                                      Population.YoungScoreBonus);
                    }

                    // apply an old age penalty
                    if (s.Age > Population.OldAgeThreshold)
                    {
                        score = Comparator.ApplyPenalty(score,
                                                        Population.OldAgePenalty);
                    }

                    double adjustedScore = score / s.Members.Count;

                    member.AdjustedScore = adjustedScore;
                }
            }
        }

        /// <summary>
        /// Perform a cross over.  
        /// </summary>
        /// <param name="mom">The mother T.</param>
        /// <param name="dad">The father T.</param>
        /// <returns></returns>
        public new YT Crossover(YT mom, YT dad)
        {
            YParent best;

            // first determine who is more fit, the mother or the father?
            if (mom.Score == dad.Score)
            {
                if (mom.NumGenes == dad.NumGenes)
                {
                    if (ThreadSafeRandom.NextDouble() > 0)
                    {
                        best = YParent.Mom;
                    }
                    else
                    {
                        best = YParent.Dad;
                    }
                }

                else
                {
                    if (mom.NumGenes < dad.NumGenes)
                    {
                        best = YParent.Mom;
                    }
                    else
                    {
                        best = YParent.Dad;
                    }
                }
            }
            else
            {
                if (Comparator.IsBetterThan(mom.Score, dad.Score))
                {
                    best = YParent.Mom;
                }

                else
                {
                    best = YParent.Dad;
                }
            }

            var babyNeurons = new Q();
            var babyGenes = new Q();

            var vecNeurons = new List<long>();

            int curMom = 0;
            int curDad = 0;

            YLinkGene momGene;
            YLinkGene dadGene;

            YLinkGene selectedGene = null;

            while ((curMom < mom.NumGenes) || (curDad < dad.NumGenes))
            {
                if (curMom < mom.NumGenes)
                {
                    momGene = (YLinkGene)mom.Links.Get(curMom);
                }
                else
                {
                    momGene = null;
                }

                if (curDad < dad.NumGenes)
                {
                    dadGene = (YLinkGene)dad.Links.Get(curDad);
                }
                else
                {
                    dadGene = null;
                }

                if ((momGene == null) && (dadGene != null))
                {
                    if (best == YParent.Dad)
                    {
                        selectedGene = dadGene;
                    }
                    curDad++;
                }
                else if ((dadGene == null) && (momGene != null))
                {
                    if (best == YParent.Mom)
                    {
                        selectedGene = momGene;
                    }
                    curMom++;
                }
                else if (momGene.InnovationId < dadGene.InnovationId)
                {
                    if (best == YParent.Mom)
                    {
                        selectedGene = momGene;
                    }
                    curMom++;
                }
                else if (dadGene.InnovationId < momGene.InnovationId)
                {
                    if (best == YParent.Dad)
                    {
                        selectedGene = dadGene;
                    }
                    curDad++;
                }
                else if (dadGene.InnovationId == momGene.InnovationId)
                {
                    if (ThreadSafeRandom.NextDouble() < 0.5f)
                    {
                        selectedGene = momGene;
                    }

                    else
                    {
                        selectedGene = dadGene;
                    }
                    curMom++;
                    curDad++;
                }

                if (babyGenes.Size() == 0)
                {
                    babyGenes.Add(selectedGene);
                }

                else
                {
                    if (((YLinkGene)babyGenes.Get(babyGenes.Size() - 1))
                            .InnovationId != selectedGene.InnovationId)
                    {
                        babyGenes.Add(selectedGene);
                    }
                }

                // Check if we already have the nodes referred to in SelectedGene.
                // If not, they need to be added.
                AddNeuronID(selectedGene.FromNeuronID, vecNeurons);
                AddNeuronID(selectedGene.ToNeuronID, vecNeurons);
            } // end while

            // now create the required nodes. First sort them into order
            vecNeurons.Sort();

            for (int i = 0; i < vecNeurons.Count; i++)
            {
                babyNeurons.Add(Innovations.CreateNeuronFromID(
                    vecNeurons[i]));
            }

            // finally, create the T
            var babyT = new YT(Population
                                                .AssignTID(), babyNeurons, babyGenes, mom.InputCount,
                                            mom.OutputCount);
            babyT.GA = this;
            babyT.Population = Population;

            return babyT;
        }

        /// <summary>
        /// Init the training.
        /// </summary>
        private void Init()
        {
            if (CalculateScore.ShouldMinimize)
            {
                bestEverScore = Double.MaxValue;
            }
            else
            {
                bestEverScore = Double.MinValue;
            }

            // check the population
            foreach (IT obj in Population.Ts)
            {
                if (!(obj is YT))
                {
                    throw new TrainingError(
                        "Population can only contain objects of YT.");
                }

                var Y = (YT)obj;

                if ((Y.InputCount != inputCount)
                    || (Y.OutputCount != outputCount))
                {
                    throw new TrainingError(
                        "All YT's must have the same input and output sizes as the base network.");
                }
                Y.GA = this;
            }

            Population.Claim(this);

            ResetAndKill();
            SortAndRecord();
            SpeciateAndCalculateSpawnLevels();
        }

        /// <summary>
        /// Reset counts and kill Ts with worse scores.
        /// </summary>
        public void ResetAndKill()
        {
            totalFitAdjustment = 0;
            averageFitAdjustment = 0;

            var speciesArray = new ISpecies[Population.Species.Count];

            for (int i = 0; i < Population.Species.Count; i++)
            {
                speciesArray[i] = Population.Species[i];
            }

            foreach (Object element in speciesArray)
            {
                var s = (ISpecies)element;
                s.Purge();

                if ((s.GensNoImprovement > paramNumGensAllowedNoImprovement)
                    && Comparator.IsBetterThan(bestEverScore,
                                               s.BestScore))
                {
                    Population.Species.Remove(s);
                }
            }
        }

        /// <summary>
        /// Sort the Ts.
        /// </summary>
        public void SortAndRecord()
        {
            foreach (IT g in Population.Ts)
            {
                g.Decode();
                PerformCalculateScore(g);
            }

            Population.Sort();

            IT T = Population.Best;
            double currentBest = T.Score;

            if (Comparator.IsBetterThan(currentBest, bestEverScore))
            {
                bestEverScore = currentBest;
                bestEverNetwork = ((YNetwork)T.Organism);
            }

            bestEverScore = Comparator.BestScore(Error,
                                                 bestEverScore);
        }

        /// <summary>
        /// Determine the species.
        /// </summary>
        public void SpeciateAndCalculateSpawnLevels()
        {
            // calculate compatibility between Ts and species
            AdjustCompatibilityThreshold();

            // assign Ts to species (if any exist)
            foreach (IT g in Population.Ts)
            {
                var T = (YT)g;
                bool added = false;

                foreach (ISpecies s in Population.Species)
                {
                    double compatibility = T.GetCompatibilityScore((YT)s.Leader);

                    if (compatibility <= paramCompatibilityThreshold)
                    {
                        AddSpeciesMember(s, T);
                        T.SpeciesID = s.SpeciesID;
                        added = true;
                        break;
                    }
                }

                // if this T did not fall into any existing species, create a
                // new species
                if (!added)
                {
                    Population.Species.Add(
                        new BasicSpecies(Population, T,
                                         Population.AssignSpeciesID()));
                }
            }

            AdjustSpeciesScore();

            foreach (IT g in Population.Ts)
            {
                var T = (YT)g;
                totalFitAdjustment += T.AdjustedScore;
            }

            averageFitAdjustment = totalFitAdjustment
                                   / Population.Size();

            foreach (IT g in Population.Ts)
            {
                var T = (YT)g;
                double toSpawn = T.AdjustedScore
                                 / averageFitAdjustment;
                T.AmountToSpawn = toSpawn;
            }

            foreach (ISpecies species in Population.Species)
            {
                species.CalculateSpawnAmount();
            }
        }

        /// <summary>
        /// Select a gene using a tournament.
        /// </summary>
        /// <param name="numComparisons">The number of compares to do.</param>
        /// <returns>The chosen T.</returns>
        public YT TournamentSelection(int numComparisons)
        {
            double bestScoreSoFar = 0;

            int chosenOne = 0;

            for (int i = 0; i < numComparisons; ++i)
            {
                var thisTry = (int)RangeRandomizer.Randomize(0, Population.Size() - 1);

                if (Population.Get(thisTry).Score > bestScoreSoFar)
                {
                    chosenOne = thisTry;

                    bestScoreSoFar = Population.Get(thisTry).Score;
                }
            }

            return (YT)Population.Get(chosenOne);
        }
    }

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
            IRandomizer randomizer =(IRandomizer)( new NguyenWidrowRandomizer());

            IMLTrain train = new NeuralPSO((BasicNetwork)method, randomizer, score, particles);

            return train;
        }
    }

    public class QuickPropFactory
    {
        /// <summary>
        /// Create a quick propagation trainer.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                               IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 2.0);

            return new QuickPropagation((BasicNetwork)method, training, learningRate);
        }
    }

    public class RBFSVDFactory
    {
        /// <summary>
        /// Create a RBF-SVD trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String args)
        {
            if (!(method is RBFNetwork))
            {
                throw new SyntError(
                    "RBF-SVD training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new SVDTraining((RBFNetwork)method, training);
        }
    }

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

    public class SVMSearchFactory
    {
        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGamma1 = "GAMMA1";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyC1 = "C1";

        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGamma2 = "GAMMA2";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyC2 = "C2";

        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGammaStep = "GAMMASTEP";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyCStep = "CSTEP";

        /// <summary>
        /// Create a SVM trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is SupportVectorMachine))
            {
                throw new SyntError(
                    "SVM Train training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            new ParamsHolder(args);

            var holder = new ParamsHolder(args);
            double gammaStart = holder.GetDouble(
                PropertyGamma1, false,
                SVMSearchTrain.DefaultGammaBegin);
            double cStart = holder.GetDouble(PropertyC1,
                                             false, SVMSearchTrain.DefaultConstBegin);
            double gammaStop = holder.GetDouble(
                PropertyGamma2, false,
                SVMSearchTrain.DefaultGammaEnd);
            double cStop = holder.GetDouble(PropertyC2,
                                            false, SVMSearchTrain.DefaultConstEnd);
            double gammaStep = holder.GetDouble(
                PropertyGammaStep, false,
                SVMSearchTrain.DefaultGammaStep);
            double cStep = holder.GetDouble(PropertyCStep,
                                            false, SVMSearchTrain.DefaultConstStep);

            var result = new SVMSearchTrain((SupportVectorMachine)method, training)
            {
                GammaBegin = gammaStart,
                GammaEnd = gammaStop,
                GammaStep = gammaStep,
                ConstBegin = cStart,
                ConstEnd = cStop,
                ConstStep = cStep
            };

            return result;
        }
    }

    public class SVMTrain : BasicTraining
    {
        /// <summary>
        /// The default starting number for C.
        /// </summary>
        ///
        public const double DefaultConstBegin = -5;

        /// <summary>
        /// The default ending number for C.
        /// </summary>
        ///
        public const double DefaultConstEnd = 15;

        /// <summary>
        /// The default step for C.
        /// </summary>
        ///
        public const double DefaultConstStep = 2;

        /// <summary>
        /// The default gamma begin.
        /// </summary>
        ///
        public const double DefaultGammaBegin = -10;

        /// <summary>
        /// The default gamma end.
        /// </summary>
        ///
        public const double DefaultGammaEnd = 10;

        /// <summary>
        /// The default gamma step.
        /// </summary>
        ///
        public const double DefaultGammaStep = 1;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly SupportVectorMachine _network;

        /// <summary>
        /// The problem to train for.
        /// </summary>
        ///
        private readonly svm_problem _problem;

        /// <summary>
        /// The const c value.
        /// </summary>
        ///
        private double _c;

        /// <summary>
        /// The number of folds.
        /// </summary>
        ///
        private int _fold;

        /// <summary>
        /// The gamma value.
        /// </summary>
        ///
        private double _gamma;

        /// <summary>
        /// Is the training done.
        /// </summary>
        ///
        private bool _trainingDone;

        /// <summary>
        /// Construct a trainer for an SVM network.
        /// </summary>
        ///
        /// <param name="method">The network to train.</param>
        /// <param name="dataSet">The training data for this network.</param>
        public SVMTrain(SupportVectorMachine method, IMLDataSet dataSet) : base(TrainingImplementationType.OnePass)
        {
            _fold = 0;
            _network = method;
            Training = dataSet;
            _trainingDone = false;

            _problem = SyntesisSVMProblem.Syntesis(dataSet, 0);
            _gamma = 1.0d / _network.InputCount;
            _c = 1.0d;
        }

        /// <inheritdoc/>
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Set the constant C.
        /// </summary>
        public double C
        {
            get { return _c; }
            set
            {
                if (value <= 0 || value < SyntFramework.DefaultDoubleEqual)
                {
                    throw new SyntError("SVM training cannot use a c value less than zero.");
                }

                _c = value;
            }
        }


        /// <summary>
        /// Set the number of folds.
        /// </summary>
        public int Fold
        {
            get { return _fold; }
            set { _fold = value; }
        }


        /// <summary>
        /// Set the gamma.
        /// </summary>
        public double Gamma
        {
            get { return _gamma; }
            set
            {
                if (value <= 0 || value < SyntFramework.DefaultDoubleEqual)
                {
                    throw new SyntError("SVM training cannot use a gamma value less than zero.");
                }
                _gamma = value;
            }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }


        /// <value>The problem being trained.</value>
        public svm_problem Problem
        {
            get { return _problem; }
        }


        /// <value>True if the training is done.</value>
        public override bool TrainingDone
        {
            get { return _trainingDone; }
        }

        /// <summary>
        /// Evaluate the error for the specified model.
        /// </summary>
        ///
        /// <param name="param">The params for the SVN.</param>
        /// <param name="prob">The problem to evaluate.</param>
        /// <param name="target">The output values from the SVN.</param>
        /// <returns>The calculated error.</returns>
        private static double Evaluate(svm_parameter param, svm_problem prob,
                                double[] target)
        {
            int totalCorrect = 0;

            var error = new ErrorCalculation();

            if ((param.svm_type == svm_parameter.EPSILON_SVR)
                || (param.svm_type == svm_parameter.NU_SVR))
            {
                for (int i = 0; i < prob.l; i++)
                {
                    double ideal = prob.y[i];
                    double actual = target[i];
                    error.UpdateError(actual, ideal);
                }
                return error.Calculate();
            }
            for (int i = 0; i < prob.l; i++)
            {
                if (target[i] == prob.y[i])
                {
                    ++totalCorrect;
                }
            }

            return Format.HundredPercent * totalCorrect / prob.l;
        }


        /// <summary>
        /// Perform either a train or a cross validation.  If the folds property is 
        /// greater than 1 then cross validation will be done.  Cross validation does 
        /// not produce a usable model, but it does set the error. 
        /// If you are cross validating try C and Gamma values until you have a good 
        /// error rate.  Then use those values to train, producing the final model.
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            _network.Params.C = _c;
            _network.Params.gamma = _gamma;

            SyntLogging.Log(SyntLogging.LevelInfo, "Training with parameters C = " + _c + ", gamma = " + _gamma);

            if (_fold > 1)
            {
                // cross validate
                var target = new double[_problem.l];

                svm.svm_cross_validation(_problem, _network.Params,
                                         _fold, target);
                _network.Model = null;

                Error = Evaluate(_network.Params, _problem, target);
            }
            else
            {
                // train
                _network.Model = svm.svm_train(_problem,
                                              _network.Params);

                Error = _network.CalculateError(Training);
            }

            _trainingDone = true;
        }

        /// <inheritdoc/>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }
    }

    public class SVMSearchTrain : BasicTraining
    {
        /// <summary>
        /// The default starting number for C.
        /// </summary>
        ///
        public const double DefaultConstBegin = 1;

        /// <summary>
        /// The default ending number for C.
        /// </summary>
        ///
        public const double DefaultConstEnd = 15;

        /// <summary>
        /// The default step for C.
        /// </summary>
        ///
        public const double DefaultConstStep = 2;

        /// <summary>
        /// The default gamma begin.
        /// </summary>
        ///
        public const double DefaultGammaBegin = 1;

        /// <summary>
        /// The default gamma end.
        /// </summary>
        ///
        public const double DefaultGammaEnd = 10;

        /// <summary>
        /// The default gamma step.
        /// </summary>
        ///
        public const double DefaultGammaStep = 1;

        /// <summary>
        /// The internal training object, used for the search.
        /// </summary>
        ///
        private readonly SVMTrain _internalTrain;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly SupportVectorMachine _network;

        /// <summary>
        /// The best values found for C.
        /// </summary>
        ///
        private double _bestConst;

        /// <summary>
        /// The best error.
        /// </summary>
        ///
        private double _bestError;

        /// <summary>
        /// The best values found for gamma.
        /// </summary>
        ///
        private double _bestGamma;

        /// <summary>
        /// The beginning value for C.
        /// </summary>
        ///
        private double _constBegin;

        /// <summary>
        /// The ending value for C.
        /// </summary>
        ///
        private double _constEnd;

        /// <summary>
        /// The step value for C.
        /// </summary>
        ///
        private double _constStep;

        /// <summary>
        /// The current C.
        /// </summary>
        ///
        private double _currentConst;

        /// <summary>
        /// The current gamma.
        /// </summary>
        ///
        private double _currentGamma;

        /// <summary>
        /// The number of folds.
        /// </summary>
        ///
        private int _fold;

        /// <summary>
        /// The beginning value for gamma.
        /// </summary>
        ///
        private double _gammaBegin;

        /// <summary>
        /// The ending value for gamma.
        /// </summary>
        ///
        private double _gammaEnd;

        /// <summary>
        /// The step value for gamma.
        /// </summary>
        ///
        private double _gammaStep;

        /// <summary>
        /// Is the network setup.
        /// </summary>
        ///
        private bool _isSetup;

        /// <summary>
        /// Is the training done.
        /// </summary>
        ///
        private bool _trainingDone;

        /// <summary>
        /// Construct a trainer for an SVM network.
        /// </summary>
        ///
        /// <param name="method">The method to train.</param>
        /// <param name="training">The training data for this network.</param>
        public SVMSearchTrain(SupportVectorMachine method, IMLDataSet training)
            : base(TrainingImplementationType.Iterative)
        {
            _fold = 0;
            _constBegin = DefaultConstBegin;
            _constStep = DefaultConstStep;
            _constEnd = DefaultConstEnd;
            _gammaBegin = DefaultGammaBegin;
            _gammaEnd = DefaultGammaEnd;
            _gammaStep = DefaultGammaStep;
            _network = method;
            Training = training;
            _isSetup = false;
            _trainingDone = false;

            _internalTrain = new SVMTrain(_network, training);
        }

        /// <inheritdoc/>
        public override sealed bool CanContinue
        {
            get { return false; }
        }


        /// <value>the constBegin to set</value>
        public double ConstBegin
        {
            get { return _constBegin; }
            set { _constBegin = value; }
        }


        /// <value>the constEnd to set</value>
        public double ConstEnd
        {
            get { return _constEnd; }
            set { _constEnd = value; }
        }


        /// <value>the constStep to set</value>
        public double ConstStep
        {
            get { return _constStep; }
            set { _constStep = value; }
        }


        /// <value>the fold to set</value>
        public int Fold
        {
            get { return _fold; }
            set { _fold = value; }
        }


        /// <value>the gammaBegin to set</value>
        public double GammaBegin
        {
            get { return _gammaBegin; }
            set { _gammaBegin = value; }
        }


        /// <value>the gammaEnd to set.</value>
        public double GammaEnd
        {
            get { return _gammaEnd; }
            set { _gammaEnd = value; }
        }


        /// <value>the gammaStep to set</value>
        public double GammaStep
        {
            get { return _gammaStep; }
            set { _gammaStep = value; }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }


        /// <value>True if the training is done.</value>
        public override bool TrainingDone
        {
            get { return _trainingDone; }
        }

        /// <inheritdoc/>
        public override sealed void FinishTraining()
        {
            _internalTrain.Gamma = _bestGamma;
            _internalTrain.C = _bestConst;
            _internalTrain.Iteration();
        }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override sealed void Iteration()
        {
            if (!_trainingDone)
            {
                if (!_isSetup)
                {
                    Setup();
                }

                PreIteration();

                _internalTrain.Fold = _fold;

                if (_network.KernelType == KernelType.RadialBasisFunction)
                {
                    _internalTrain.Gamma = _currentGamma;
                    _internalTrain.C = _currentConst;
                    _internalTrain.Iteration();
                    double e = _internalTrain.Error;

                    //System.out.println(this.currentGamma + "," + this.currentConst
                    //		+ "," + e);

                    // new best error?
                    if (!Double.IsNaN(e))
                    {
                        if (e < _bestError)
                        {
                            _bestConst = _currentConst;
                            _bestGamma = _currentGamma;
                            _bestError = e;
                        }
                    }

                    // advance
                    _currentConst += _constStep;
                    if (_currentConst > _constEnd)
                    {
                        _currentConst = _constBegin;
                        _currentGamma += _gammaStep;
                        if (_currentGamma > _gammaEnd)
                        {
                            _trainingDone = true;
                        }
                    }

                    Error = _bestError;
                }
                else
                {
                    _internalTrain.Gamma = _currentGamma;
                    _internalTrain.C = _currentConst;
                    _internalTrain.Iteration();
                }

                PostIteration();
            }
        }

        /// <inheritdoc/>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }

        /// <summary>
        /// Setup to train the SVM.
        /// </summary>
        ///
        private void Setup()
        {
            _currentConst = _constBegin;
            _currentGamma = _gammaBegin;
            _bestError = Double.PositiveInfinity;
            _isSetup = true;

            if (_currentGamma <= 0 || _currentGamma < SyntFramework.DefaultDoubleEqual)
            {
                throw new SyntError("SVM search training cannot use a gamma value less than zero.");
            }

            if (_currentConst <= 0 || _currentConst < SyntFramework.DefaultDoubleEqual)
            {
                throw new SyntError("SVM search training cannot use a const value less than zero.");
            }

            if (_gammaStep < 0)
            {
                throw new SyntError("SVM search gamma step cannot use a const value less than zero.");
            }

            if (_constStep < 0)
            {
                throw new SyntError("SVM search const step cannot use a const value less than zero.");
            }
        }
    }
    
    public class TrainBayesianFactory
    {
        /**
 * Create a K2 trainer.
 * 
 * @param method
 *            The method to use.
 * @param training
 *            The training data to use.
 * @param argsStr
 *            The arguments to use.
 * @return The newly created trainer.
 */
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            ParamsHolder holder = new ParamsHolder(args);

            int maxParents = holder.GetInt(
                    MLTrainFactory.PropertyMaxParents, false, 1);
            String searchStr = holder.GetString("SEARCH", false, "k2");
            String estimatorStr = holder.GetString("ESTIMATOR", false, "simple");
            String initStr = holder.GetString("INIT", false, "naive");

            IBayesSearch search;
            IBayesEstimator estimator;
            BayesianInit init;

            if (string.Compare(searchStr, "k2", true) == 0)
            {
                search = new SearchK2();
            }
            else if (string.Compare(searchStr, "none", true) == 0)
            {
                search = new SearchNone();
            }
            else
            {
                throw new BayesianError("Invalid search type: " + searchStr);
            }

            if (string.Compare(estimatorStr, "simple", true) == 0)
            {
                estimator = new SimpleEstimator();
            }
            else if (string.Compare(estimatorStr, "none", true) == 0)
            {
                estimator = new EstimatorNone();
            }
            else
            {
                throw new BayesianError("Invalid estimator type: " + estimatorStr);
            }

            if (string.Compare(initStr, "simple") == 0)
            {
                init = BayesianInit.InitEmpty;
            }
            else if (string.Compare(initStr, "naive") == 0)
            {
                init = BayesianInit.InitNaiveBayes;
            }
            else if (string.Compare(initStr, "none") == 0)
            {
                init = BayesianInit.InitNoChange;
            }
            else
            {
                throw new BayesianError("Invalid init type: " + initStr);
            }

            return new TrainBayesian((BayesianNetwork)method, training, maxParents, init, search, estimator);
        }
    }

    public class MLActivationFactory
    {
        public const String AF_BIPOLAR = "bipolar";
        public const String AF_COMPETITIVE = "comp";
        public const String AF_GAUSSIAN = "gauss";
        public const String AF_LINEAR = "linear";
        public const String AF_LOG = "log";
        public const String AF_RAMP = "ramp";
        public const String AF_SIGMOID = "sigmoid";
        public const String AF_SIN = "sin";
        public const String AF_SOFTMAX = "softmax";
        public const String AF_STEP = "step";
        public const String AF_TANH = "tanh";

        public IActivationFunction Create(String fn)
        {

            foreach (SyntPluginBase plugin in SyntFramework.Instance.Plugins)
            {
                if (plugin is ISyntPluginService1)
                {
                    IActivationFunction result = ((ISyntPluginService1)plugin).CreateActivationFunction(fn);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
    }

    public class MLMethodFactory
    {
        /// <summary>
        /// String constant for a bayesian neural network.
        /// </summary>
	    public const String TypeBayesian = "bayesian";

        /// <summary>
        /// String constant for feedforward neural networks.
        /// </summary>
        ///
        public const String TypeFeedforward = "feedforward";

        /// <summary>
        /// String constant for RBF neural networks.
        /// </summary>
        ///
        public const String TypeRbfnetwork = "rbfnetwork";

        /// <summary>
        /// String constant for support vector machines.
        /// </summary>
        ///
        public const String TypeSVM = "svm";

        /// <summary>
        /// String constant for SOMs.
        /// </summary>
        ///
        public const String TypeSOM = "som";

        /// <summary>
        /// A probabilistic neural network. Supports both PNN and GRNN.
        /// </summary>
        ///
        public const String TypePNN = "pnn";

        /// <summary>
        /// Create a new machine learning method.
        /// </summary>
        ///
        /// <param name="methodType">The method to create.</param>
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created machine learning method.</returns>
        public IMLMethod Create(String methodType,
                               String architecture, int input, int output)
        {
            foreach (SyntPluginBase plugin in SyntFramework.Instance.Plugins)
            {
                if (plugin is ISyntPluginService1)
                {
                    IMLMethod result = ((ISyntPluginService1)plugin).CreateMethod(
                            methodType, architecture, input, output);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            throw new SyntError("Unknown method type: " + methodType);
        }
    }

    [Serializable]
    public class Q
    {
        /// <summary>
        /// The individual elements of this Q.
        /// </summary>
        ///
        private readonly List<IGene> _genes;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public Q()
        {
            _genes = new List<IGene>();
        }

        /// <summary>
        /// Used the get the entire gene list.
        /// </summary>
        ///
        /// <value>the genes</value>
        public List<IGene> Genes
        {
            get { return _genes; }
        }

        /// <summary>
        /// Add a gene.
        /// </summary>
        ///
        /// <param name="gene">The gene to add.</param>
        public void Add(IGene gene)
        {
            _genes.Add(gene);
        }

        /// <summary>
        /// Get an individual gene.
        /// </summary>
        ///
        /// <param name="i">The index of the gene.</param>
        /// <returns>The gene.</returns>
        public IGene Get(int i)
        {
            return _genes[i];
        }

        /// <summary>
        /// Get the specified gene.
        /// </summary>
        ///
        /// <param name="gene">The specified gene.</param>
        /// <returns>The gene specified.</returns>
        public IGene GetGene(int gene)
        {
            return _genes[gene];
        }


        /// <returns>The number of genes in this Q.</returns>
        public int Size()
        {
            return _genes.Count;
        }
    }

    public class FlatLayer
    {
        /// <summary>
        /// The neuron count.
        /// </summary>
        ///
        private readonly int _count;

        /// <summary>
        /// The bias activation, usually 1 for bias or 0 for no bias.
        /// </summary>
        ///
        private double _biasActivation;

        /// <summary>
        /// The layer that feeds this layer's context.
        /// </summary>
        ///
        private FlatLayer _contextFedBy;

        /// <summary>
        /// Construct a flat layer.
        /// </summary>
        ///
        /// <param name="activation">The activation function.</param>
        /// <param name="count">The neuron count.</param>
        /// <param name="biasActivation">The bias activation.</param>
        public FlatLayer(IActivationFunction activation, int count,
                         double biasActivation)
        {
            Activation = activation;
            _count = count;
            _biasActivation = biasActivation;
            _contextFedBy = null;
        }


        /// <value>the activation to set</value>
        public IActivationFunction Activation { get; set; }


        /// <summary>
        /// Set the bias activation.
        /// </summary>
        public double BiasActivation
        {
            get
            {
                if (HasBias())
                {
                    return _biasActivation;
                }
                return 0;
            }
            set { _biasActivation = value; }
        }


        /// <value>The number of neurons our context is fed by.</value>
        public int ContextCount
        {
            get
            {
                if (_contextFedBy == null)
                {
                    return 0;
                }
                return _contextFedBy.Count;
            }
        }


        /// <summary>
        /// Set the layer that this layer's context is fed by.
        /// </summary>
        public FlatLayer ContextFedBy
        {
            get { return _contextFedBy; }
            set { _contextFedBy = value; }
        }


        /// <value>the count</value>
        public int Count
        {
            get { return _count; }
        }


        /// <value>The total number of neurons on this layer, includes context, bias
        /// and regular.</value>
        public int TotalCount
        {
            get
            {
                if (_contextFedBy == null)
                {
                    return Count + ((HasBias()) ? 1 : 0);
                }
                return Count + ((HasBias()) ? 1 : 0)
                       + _contextFedBy.Count;
            }
        }


        /// <returns>the bias</returns>
        public bool HasBias()
        {
            return Math.Abs(_biasActivation) > SyntFramework.DefaultDoubleEqual;
        }

        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder();
            result.Append("[");
            result.Append(GetType().Name);
            result.Append(": count=");
            result.Append(_count);
            result.Append(",bias=");

            if (HasBias())
            {
                result.Append(_biasActivation);
            }
            else
            {
                result.Append("false");
            }
            if (_contextFedBy != null)
            {
                result.Append(",contextFed=");
                if (_contextFedBy == this)
                {
                    result.Append("itself");
                }
                else
                {
                    result.Append(_contextFedBy);
                }
            }
            result.Append("]");
            return result.ToString();
        }
    }

    public class TComparator : IComparer<IT>
    {
        /// <summary>
        /// The method to calculate the score.
        /// </summary>
        ///
        private readonly ICalculateTScore _calculateScore;

        /// <summary>
        /// Construct the T comparator.
        /// </summary>
        ///
        /// <param name="theCalculateScore">The score calculation object to use.</param>
        public TComparator(ICalculateTScore theCalculateScore)
        {
            _calculateScore = theCalculateScore;
        }

        /// <value>The score calculation object.</value>
        public ICalculateTScore CalculateScore
        {
            get { return _calculateScore; }
        }

        #region IComparer<IT> Members

        /// <summary>
        /// Compare two Ts.
        /// </summary>
        ///
        /// <param name="T1">The first T.</param>
        /// <param name="T2">The second T.</param>
        /// <returns>Zero if equal, or less than or greater than zero to indicate
        /// order.</returns>
        public int Compare(IT T1, IT T2)
        {
            return T1.Score.CompareTo(T2.Score);
        }

        #endregion

        /// <summary>
        /// Apply a bonus, this is a simple percent that is applied in the direction
        /// specified by the "should minimize" property of the score function.
        /// </summary>
        ///
        /// <param name="v">The current value.</param>
        /// <param name="bonus">The bonus.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyBonus(double v, double bonus)
        {
            double amount = v * bonus;
            if (_calculateScore.ShouldMinimize)
            {
                return v - amount;
            }
            return v + amount;
        }

        /// <summary>
        /// Apply a penalty, this is a simple percent that is applied in the
        /// direction specified by the "should minimize" property of the score
        /// function.
        /// </summary>
        ///
        /// <param name="v">The current value.</param>
        /// <param name="bonus">The penalty.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyPenalty(double v, double bonus)
        {
            double amount = v * bonus;
            return _calculateScore.ShouldMinimize ? v - amount : v + amount;
        }

        /// <summary>
        /// Determine the best score from two scores, uses the "should minimize"
        /// property of the score function.
        /// </summary>
        ///
        /// <param name="d1">The first score.</param>
        /// <param name="d2">The second score.</param>
        /// <returns>The best score.</returns>
        public double BestScore(double d1, double d2)
        {
            return _calculateScore.ShouldMinimize ? Math.Min(d1, d2) : Math.Max(d1, d2);
        }


        /// <summary>
        /// Determine if one score is better than the other.
        /// </summary>
        ///
        /// <param name="d1">The first score to compare.</param>
        /// <param name="d2">The second score to compare.</param>
        /// <returns>True if d1 is better than d2.</returns>
        public bool IsBetterThan(double d1, double d2)
        {
            return _calculateScore.ShouldMinimize ? d1 < d2 : d1 > d2;
        }
    }

    public class SCGFactory
    {
        /// <summary>
        /// Create a SCG trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String args)
        {
            if (!(method is BasicNetwork))
            {
                throw new SyntError(
                    "SCG training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new ScaledConjugateGradient((BasicNetwork)method, training);
        }
    }

    public class BasicNeuralData : BasicMLData, INeuralData
    {
        /// <summary>
        /// Construct the object from an array.
        /// </summary>
        /// <param name="d">The array to base on.</param>
        public BasicNeuralData(double[] d) : base(d)
        {
        }

        /// <summary>
        /// Construct an empty array of the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        public BasicNeuralData(int size) : base(size)
        {
        }
    }

    public class CrossValidationKFold : CrossTraining
    {
        /// <summary>
        /// The flat network to train.
        /// </summary>
        ///
        private readonly FlatNetwork _flatNetwork;

        /// <summary>
        /// The network folds.
        /// </summary>
        ///
        private readonly NetworkFold[] _networks;

        /// <summary>
        /// The underlying trainer to use. This trainer does the actual training.
        /// </summary>
        ///
        private readonly IMLTrain _train;

        /// <summary>
        /// Construct a cross validation trainer.
        /// </summary>
        ///
        /// <param name="train">The training</param>
        /// <param name="k">The number of folds.</param>
        public CrossValidationKFold(IMLTrain train, int k) : base(train.Method, (FoldedDataSet)train.Training)
        {
            _train = train;
            Folded.Fold(k);

            _flatNetwork = ((BasicNetwork)train.Method).Structure.Flat;

            _networks = new NetworkFold[k];
            for (int i = 0; i < _networks.Length; i++)
            {
                _networks[i] = new NetworkFold(_flatNetwork);
            }
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Perform one iteration.
        /// </summary>
        ///
        public override void Iteration()
        {
            double error = 0;

            for (int valFold = 0; valFold < Folded.NumFolds; valFold++)
            {
                //// restore the correct network
                //_networks[valFold].CopyToNetwork(_flatNetwork);

                //// train with non-validation folds
                //for (int curFold = 0; curFold < Folded.NumFolds; curFold++)
                //{
                //    if (curFold != valFold)
                //    {
                //        Folded.CurrentFold = curFold;
                //        _train.Iteration();
                //    }
                //}

                //// evaluate with the validation fold			
                //Folded.CurrentFold = valFold;
                //double e = _flatNetwork.CalculateError(Folded);
                ////System.out.println("Fold " + valFold + ", " + e);
                //error += e;
                //_networks[valFold].CopyFromNetwork(_flatNetwork);
            }

            Error = error / Folded.NumFolds;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void Resume(TrainingContinuation state)
        {
        }
    }

    public class StopTrainingStrategy : IEndTrainingStrategy
    {
        /// <summary>
        /// The default minimum improvement before training stops.
        /// </summary>
        ///
        public const double DefaultMinImprovement = 0.0000001d;

        /// <summary>
        /// The default number of cycles to tolerate.
        /// </summary>
        ///
        public const int DefaultTolerateCycles = 100;

        /// <summary>
        /// The minimum improvement before training stops.
        /// </summary>
        ///
        private readonly double _minImprovement;

        /// <summary>
        /// The number of cycles to tolerate the minimum improvement.
        /// </summary>
        ///
        private readonly int _toleratedCycles;

        /// <summary>
        /// The number of bad training cycles.
        /// </summary>
        ///
        private int _badCycles;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double _bestError;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double _lastError;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start evaluation.
        /// </summary>
        ///
        private bool _ready;

        /// <summary>
        /// Flag to indicate if training should stop.
        /// </summary>
        ///
        private bool _shouldStop;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private IMLTrain _train;

        /// <summary>
        /// Construct the strategy with default options.
        /// </summary>
        ///
        public StopTrainingStrategy() : this(DefaultMinImprovement, DefaultTolerateCycles)
        {
        }

        /// <summary>
        /// Construct the strategy with the specified parameters.
        /// </summary>
        ///
        /// <param name="minImprovement">The minimum accepted improvement.</param>
        /// <param name="toleratedCycles">The number of cycles to tolerate before stopping.</param>
        public StopTrainingStrategy(double minImprovement,
                                    int toleratedCycles)
        {
            _minImprovement = minImprovement;
            _toleratedCycles = toleratedCycles;
            _badCycles = 0;
            _bestError = Double.MaxValue;
        }

        #region EndTrainingStrategy Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void Init(IMLTrain train)
        {
            _train = train;
            _shouldStop = false;
            _ready = false;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            if (_ready)
            {
                if (Math.Abs(_bestError - _train.Error) < _minImprovement)
                {
                    _badCycles++;
                    if (_badCycles > _toleratedCycles)
                    {
                        _shouldStop = true;
                    }
                }
                else
                {
                    _badCycles = 0;
                }
            }
            else
            {
                _ready = true;
            }

            _lastError = _train.Error;
            _bestError = Math.Min(_lastError, _bestError);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PreIteration()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual bool ShouldStop()
        {
            return _shouldStop;
        }

        #endregion
    }
    [Serializable]
    public class PersistYPopulation : ISyntPersistor
    {
        #region SyntPersistor Members

        /// <summary>
        /// The persistence class string.
        /// </summary>
        public virtual String PersistClassString
        {
            get { return typeof(YPopulation).Name; }
        }


        /// <summary>
        /// Read the object.
        /// </summary>
        /// <param name="mask0">The stream to read the object from.</param>
        /// <returns>The object that was loaded.</returns>
        public virtual Object Read(Stream mask0)
        {
            var result = new YPopulation();
            var innovationList = new YInnovationList { Population = result };
            result.Innovations = innovationList;
            var ins0 = new SyntReadHelper(mask0);
            IDictionary<Int32, ISpecies> speciesMap = new Dictionary<Int32, ISpecies>();
            IDictionary<ISpecies, Int32> leaderMap = new Dictionary<ISpecies, Int32>();
            IDictionary<Int32, IT> TMap = new Dictionary<Int32, IT>();
            SyntFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("Y-POPULATION")
                    && section.SubSectionName.Equals("INNOVATIONS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);
                        var innovation = new YInnovation
                        {
                            InnovationID = Int32.Parse(cols[0]),
                            InnovationType = StringToInnovationType(cols[1]),
                            NeuronType = StringToNeuronType(cols[2]),
                            SplitX = CSVFormat.EgFormat.Parse(cols[3]),
                            SplitY = CSVFormat.EgFormat.Parse(cols[4]),
                            NeuronID = Int32.Parse(cols[5]),
                            FromNeuronID = Int32.Parse(cols[6]),
                            ToNeuronID = Int32.Parse(cols[7])
                        };
                        result.Innovations.Add(innovation);
                    }
                }
                else if (section.SectionName.Equals("Y-POPULATION")
                         && section.SubSectionName.Equals("SPECIES"))
                {
                    foreach (String line in section.Lines)
                    {
                        String[] cols = line.Split(',');
                        var species = new BasicSpecies
                        {
                            SpeciesID = Int32.Parse(cols[0]),
                            Age = Int32.Parse(cols[1]),
                            BestScore = CSVFormat.EgFormat.Parse(cols[2]),
                            GensNoImprovement = Int32.Parse(cols[3]),
                            SpawnsRequired = CSVFormat.EgFormat
                                                  .Parse(cols[4])
                        };

                        species.SpawnsRequired = CSVFormat.EgFormat
                            .Parse(cols[5]);
                        leaderMap[(species)] = (Int32.Parse(cols[6]));
                        result.Species.Add(species);
                        speciesMap[((int)species.SpeciesID)] = (species);
                    }
                }
                else if (section.SectionName.Equals("Y-POPULATION")
                         && section.SubSectionName.Equals("TS"))
                {
                    YT lastT = null;

                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);
                        if (cols[0].Equals("g", StringComparison.InvariantCultureIgnoreCase))
                        {
                            lastT = new YT
                            {
                                NeuronsQ = new Q(),
                                LinksQ = new Q()
                            };
                            lastT.Qs.Add(lastT.NeuronsQ);
                            lastT.Qs.Add(lastT.LinksQ);
                            lastT.TID = Int32.Parse(cols[1]);
                            lastT.SpeciesID = Int32.Parse(cols[2]);
                            lastT.AdjustedScore = CSVFormat.EgFormat
                                .Parse(cols[3]);
                            lastT.AmountToSpawn = CSVFormat.EgFormat
                                .Parse(cols[4]);
                            lastT.NetworkDepth = Int32.Parse(cols[5]);
                            lastT.Score = CSVFormat.EgFormat.Parse(cols[6]);
                            result.Add(lastT);
                            TMap[(int)lastT.TID] = lastT;
                        }
                        else if (cols[0].Equals("n", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var neuronGene = new YNeuronGene
                            {
                                Id = Int32.Parse(cols[1]),
                                NeuronType = StringToNeuronType(cols[2]),
                                Enabled = Int32.Parse(cols[3]) > 0,
                                InnovationId = Int32.Parse(cols[4]),
                                ActivationResponse = CSVFormat.EgFormat
                                                         .Parse(cols[5]),
                                SplitX = CSVFormat.EgFormat.Parse(cols[6]),
                                SplitY = CSVFormat.EgFormat.Parse(cols[7])
                            };
                            lastT.Neurons.Add(neuronGene);
                        }
                        else if (cols[0].Equals("l", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var linkGene = new YLinkGene();
                            linkGene.Id = Int32.Parse(cols[1]);
                            linkGene.Enabled = Int32.Parse(cols[2]) > 0;
                            linkGene.Recurrent = Int32.Parse(cols[3]) > 0;
                            linkGene.FromNeuronID = Int32.Parse(cols[4]);
                            linkGene.ToNeuronID = Int32.Parse(cols[5]);
                            linkGene.Weight = CSVFormat.EgFormat.Parse(cols[6]);
                            linkGene.InnovationId = Int32.Parse(cols[7]);
                            lastT.Links.Add(linkGene);
                        }
                    }
                }
                else if (section.SectionName.Equals("Y-POPULATION")
                         && section.SubSectionName.Equals("CONFIG"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    result.YActivationFunction = SyntFileSection
                        .ParseActivationFunction(paras,
                                                 YPopulation.PropertyYActivation);
                    result.OutputActivationFunction = SyntFileSection
                        .ParseActivationFunction(paras,
                                                 YPopulation.PropertyOutputActivation);
                    result.Snapshot = SyntFileSection.ParseBoolean(paras,
                                                                    PersistConst.Snapshot);
                    result.InputCount = SyntFileSection.ParseInt(paras,
                                                                  PersistConst.InputCount);
                    result.OutputCount = SyntFileSection.ParseInt(paras,
                                                                   PersistConst.OutputCount);
                    result.OldAgePenalty = SyntFileSection.ParseDouble(paras,
                                                                        PopulationConst.PropertyOldAgePenalty);
                    result.OldAgeThreshold = SyntFileSection.ParseInt(paras,
                                                                       PopulationConst.PropertyOldAgeThreshold);
                    result.PopulationSize = SyntFileSection.ParseInt(paras,
                                                                      PopulationConst.PropertyPopulationSize);
                    result.SurvivalRate = SyntFileSection.ParseDouble(paras,
                                                                       PopulationConst.PropertySurvivalRate);
                    result.YoungBonusAgeThreshhold = SyntFileSection.ParseInt(
                        paras, PopulationConst.PropertyYoungAgeThreshold);
                    result.YoungScoreBonus = SyntFileSection.ParseDouble(paras,
                                                                          PopulationConst.PropertyYoungAgeBonus);
                    result.TIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                  PopulationConst.
                                                                                      PropertyNextTID);
                    result.InnovationIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                      PopulationConst.
                                                                                          PropertyNextInnovationID);
                    result.GeneIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                PopulationConst.
                                                                                    PropertyNextGeneID);
                    result.SpeciesIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                   PopulationConst.
                                                                                       PropertyNextSpeciesID);
                }
            }

            // now link everything up


            // first put all the Ts into correct species
            foreach (IT T in result.Ts)
            {
                var YT = (YT)T;
                var speciesId = (int)YT.SpeciesID;
                if (speciesMap.ContainsKey(speciesId))
                {
                    ISpecies s = speciesMap[speciesId];
                    s.Members.Add(YT);
                }

                YT.InputCount = result.InputCount;
                YT.OutputCount = result.OutputCount;
            }


            // set the species leader links
            foreach (ISpecies species in leaderMap.Keys)
            {
                int leaderID = leaderMap[species];
                IT leader = TMap[leaderID];
                species.Leader = leader;
                ((BasicSpecies)species).Population = result;
            }

            return result;
        }

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="os">The stream to write to.</param>
        /// <param name="obj">The object to save.</param>
        public virtual void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var pop = (YPopulation)obj;
            xout.AddSection("Y-POPULATION");
            xout.AddSubSection("CONFIG");
            xout.WriteProperty(PersistConst.Snapshot, pop.Snapshot);
            xout.WriteProperty(YPopulation.PropertyOutputActivation,
                               pop.OutputActivationFunction);
            xout.WriteProperty(YPopulation.PropertyYActivation,
                               pop.YActivationFunction);
            xout.WriteProperty(PersistConst.InputCount, pop.InputCount);
            xout.WriteProperty(PersistConst.OutputCount, pop.OutputCount);
            xout.WriteProperty(PopulationConst.PropertyOldAgePenalty,
                               pop.OldAgePenalty);
            xout.WriteProperty(PopulationConst.PropertyOldAgeThreshold,
                               pop.OldAgeThreshold);
            xout.WriteProperty(PopulationConst.PropertyPopulationSize,
                               pop.PopulationSize);
            xout.WriteProperty(PopulationConst.PropertySurvivalRate,
                               pop.SurvivalRate);
            xout.WriteProperty(PopulationConst.PropertyYoungAgeThreshold,
                               pop.YoungBonusAgeThreshold);
            xout.WriteProperty(PopulationConst.PropertyYoungAgeBonus,
                               pop.YoungScoreBonus);
            xout.WriteProperty(PopulationConst.PropertyNextTID, pop.TIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PropertyNextInnovationID, pop.InnovationIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PropertyNextGeneID, pop.GeneIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PropertyNextSpeciesID, pop.SpeciesIDGenerate.CurrentID);
            xout.AddSubSection("INNOVATIONS");
            if (pop.Innovations != null)
            {
                foreach (IInnovation innovation in pop.Innovations.Innovations)
                {
                    var YInnovation = (YInnovation)innovation;
                    xout.AddColumn(YInnovation.InnovationID);
                    xout.AddColumn(InnovationTypeToString(YInnovation.InnovationType));
                    xout.AddColumn(NeuronTypeToString(YInnovation.NeuronType));
                    xout.AddColumn(YInnovation.SplitX);
                    xout.AddColumn(YInnovation.SplitY);
                    xout.AddColumn(YInnovation.NeuronID);
                    xout.AddColumn(YInnovation.FromNeuronID);
                    xout.AddColumn(YInnovation.ToNeuronID);
                    xout.WriteLine();
                }
            }
            xout.AddSubSection("TS");

            foreach (IT T in pop.Ts)
            {
                var YT = (YT)T;
                xout.AddColumn("g");
                xout.AddColumn(YT.TID);
                xout.AddColumn(YT.SpeciesID);
                xout.AddColumn(YT.AdjustedScore);
                xout.AddColumn(YT.AmountToSpawn);
                xout.AddColumn(YT.NetworkDepth);
                xout.AddColumn(YT.Score);
                xout.WriteLine();


                foreach (IGene neuronGene in YT.Neurons.Genes)
                {
                    var YNeuronGene = (YNeuronGene)neuronGene;
                    xout.AddColumn("n");
                    xout.AddColumn(YNeuronGene.Id);
                    xout.AddColumn(NeuronTypeToString(YNeuronGene.NeuronType));
                    xout.AddColumn(YNeuronGene.Enabled);
                    xout.AddColumn(YNeuronGene.InnovationId);
                    xout.AddColumn(YNeuronGene.ActivationResponse);
                    xout.AddColumn(YNeuronGene.SplitX);
                    xout.AddColumn(YNeuronGene.SplitY);
                    xout.WriteLine();
                }

                foreach (IGene linkGene in YT.Links.Genes)
                {
                    var YLinkGene = (YLinkGene)linkGene;
                    xout.AddColumn("l");
                    xout.AddColumn(YLinkGene.Id);
                    xout.AddColumn(YLinkGene.Enabled);
                    xout.AddColumn(YLinkGene.Recurrent);
                    xout.AddColumn(YLinkGene.FromNeuronID);
                    xout.AddColumn(YLinkGene.ToNeuronID);
                    xout.AddColumn(YLinkGene.Weight);
                    xout.AddColumn(YLinkGene.InnovationId);
                    xout.WriteLine();
                }
            }
            xout.AddSubSection("SPECIES");

            foreach (ISpecies species in pop.Species)
            {
                xout.AddColumn(species.SpeciesID);
                xout.AddColumn(species.Age);
                xout.AddColumn(species.BestScore);
                xout.AddColumn(species.GensNoImprovement);
                xout.AddColumn(species.NumToSpawn);
                xout.AddColumn(species.SpawnsRequired);
                xout.AddColumn(species.Leader.TID);
                xout.WriteLine();
            }
            xout.Flush();
        }

        /// <summary>
        /// The file version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }

        #endregion

        /// <summary>
        /// Convert the neuron type to a string.
        /// </summary>
        /// <param name="t">The neuron type.</param>
        /// <returns>The string.</returns>
        public static String NeuronTypeToString(YNeuronType t)
        {
            switch (t)
            {
                case YNeuronType.Bias:
                    return ("b");
                case YNeuronType.Hidden:
                    return ("h");
                case YNeuronType.Input:
                    return ("i");
                case YNeuronType.None:
                    return ("n");
                case YNeuronType.Output:
                    return ("o");
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert the innovation type to a string.
        /// </summary>
        /// <param name="t">The innovation type.</param>
        /// <returns>The string.</returns>
        public static String InnovationTypeToString(YInnovationType t)
        {
            switch (t)
            {
                case YInnovationType.NewLink:
                    return "l";
                case YInnovationType.NewNeuron:
                    return "n";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert a string to an innovation type.
        /// </summary>
        /// <param name="t">The string to convert.</param>
        /// <returns>The innovation type.</returns>
        public static YInnovationType StringToInnovationType(String t)
        {
            if (t.Equals("l", StringComparison.InvariantCultureIgnoreCase))
            {
                return YInnovationType.NewLink;
            }
            if (t.Equals("n", StringComparison.InvariantCultureIgnoreCase))
            {
                return YInnovationType.NewNeuron;
            }
            return default(YInnovationType) /* was: null */;
        }

        /// <summary>
        /// Convert a string to a neuron type.
        /// </summary>
        /// <param name="t">The string.</param>
        /// <returns>The resulting neuron type.</returns>
        public static YNeuronType StringToNeuronType(String t)
        {
            if (t.Equals("b"))
            {
                return YNeuronType.Bias;
            }
            if (t.Equals("h"))
            {
                return YNeuronType.Hidden;
            }
            if (t.Equals("i"))
            {
                return YNeuronType.Input;
            }
            if (t.Equals("n"))
            {
                return YNeuronType.None;
            }
            if (t.Equals("o"))
            {
                return YNeuronType.Output;
            }
            throw new SyntError("Unknonw neuron type: " + t);
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(YPopulation); }
        }
    }

    public class PersistYNetwork : ISyntPersistor
    {
        #region SyntPersistor Members

        /// <summary>
        /// The file version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }

        /// <summary>
        /// The persist class string.
        /// </summary>
        public virtual String PersistClassString
        {
            get { return "YNetwork"; }
        }

        /// <summary>
        /// Read the object.
        /// </summary>
        /// <param name="mask0">The stream to read from.</param>
        /// <returns>The loaded object.</returns>
        public virtual Object Read(Stream mask0)
        {
            var result = new YNetwork();
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;
            IDictionary<Int32, YNeuron> neuronMap = new Dictionary<Int32, YNeuron>();

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("Y")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    foreach (String key in paras.Keys)
                    {
                        result.Properties.Add(key, paras[key]);
                    }
                }
                if (section.SectionName.Equals("Y")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();

                    result.InputCount = SyntFileSection.ParseInt(p,
                                                                  PersistConst.InputCount);
                    result.OutputCount = SyntFileSection.ParseInt(p,
                                                                   PersistConst.OutputCount);
                    result.ActivationFunction = SyntFileSection
                        .ParseActivationFunction(p,
                                                 PersistConst.ActivationFunction);
                    result.OutputActivationFunction = SyntFileSection
                        .ParseActivationFunction(p,
                                                 YPopulation.PropertyOutputActivation);
                    result.NetworkDepth = SyntFileSection.ParseInt(p,
                                                                    PersistConst.Depth);
                    result.Snapshot = SyntFileSection.ParseBoolean(p,
                                                                    PersistConst.Snapshot);
                }
                else if (section.SectionName.Equals("Y")
                         && section.SubSectionName.Equals("NEURONS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);

                        long neuronID = Int32.Parse(cols[0]);
                        YNeuronType neuronType = PersistYPopulation
                            .StringToNeuronType(cols[1]);
                        double activationResponse = CSVFormat.EgFormat
                            .Parse(cols[2]);
                        double splitY = CSVFormat.EgFormat
                            .Parse(cols[3]);
                        double splitX = CSVFormat.EgFormat
                            .Parse(cols[4]);

                        var YNeuron = new YNeuron(neuronType,
                                                        neuronID, splitY, splitX, activationResponse);
                        result.Neurons.Add(YNeuron);
                        neuronMap[((int)neuronID)] = (YNeuron);
                    }
                }
                else if (section.SectionName.Equals("Y")
                         && section.SubSectionName.Equals("LINKS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);
                        int fromID = Int32.Parse(cols[0]);
                        int toID = Int32.Parse(cols[1]);
                        bool recurrent = Int32.Parse(cols[2]) > 0;
                        double weight = CSVFormat.EgFormat.Parse(cols[3]);
                        YNeuron fromNeuron = (neuronMap[fromID]);
                        YNeuron toNeuron = (neuronMap[toID]);
                        var YLink = new YLink(weight, fromNeuron,
                                                    toNeuron, recurrent);
                        fromNeuron.OutputboundLinks.Add(YLink);
                        toNeuron.InboundLinks.Add(YLink);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="os">The output stream.</param>
        /// <param name="obj">The object to save.</param>
        public virtual void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var Y = (YNetwork)obj;
            xout.AddSection("Y");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(Y.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.InputCount, Y.InputCount);
            xout.WriteProperty(PersistConst.OutputCount, Y.OutputCount);
            xout.WriteProperty(PersistConst.ActivationFunction,
                               Y.ActivationFunction);
            xout.WriteProperty(YPopulation.PropertyOutputActivation,
                               Y.OutputActivationFunction);
            xout.WriteProperty(PersistConst.Depth, Y.NetworkDepth);
            xout.WriteProperty(PersistConst.Snapshot, Y.Snapshot);

            xout.AddSubSection("NEURONS");

            foreach (YNeuron YNeuron in Y.Neurons)
            {
                xout.AddColumn((int)YNeuron.NeuronID);
                xout.AddColumn(PersistYPopulation.NeuronTypeToString(YNeuron.NeuronType));
                xout.AddColumn(YNeuron.ActivationResponse);
                xout.AddColumn(YNeuron.SplitX);
                xout.AddColumn(YNeuron.SplitY);
                xout.WriteLine();
            }

            xout.AddSubSection("LINKS");

            foreach (YNeuron YNeuron in Y.Neurons)
            {
                foreach (YLink link in YNeuron.OutputboundLinks)
                {
                    WriteLink(xout, link);
                }
            }

            xout.Flush();
        }

        #endregion

        /// <summary>
        /// Write a link.
        /// </summary>
        /// <param name="xout">The output file.</param>
        /// <param name="link">The link.</param>
        private static void WriteLink(SyntWriteHelper xout, YLink link)
        {
            xout.AddColumn((int)link.FromNeuron.NeuronID);
            xout.AddColumn((int)link.ToNeuron.NeuronID);
            xout.AddColumn(link.Recurrent);
            xout.AddColumn(link.Weight);
            xout.WriteLine();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(YNetwork); }
        }
    }


    public class PersistCPN : ISyntPersistor
    {
        /// <summary>
        /// The input to instar property.
        /// </summary>
        ///
        internal const String PropertyInputToInstar = "inputToInstar";

        /// <summary>
        /// The instar to input property.
        /// </summary>
        ///
        internal const String PropertyInstarToInput = "instarToInput";

        /// <summary>
        /// The winner count property.
        /// </summary>
        ///
        internal const String PropertyWinnerCount = "winnerCount";

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(CPNNetwork); }
        }

        #region SyntPersistor Members

        /// <inheritdoc/>
        public int FileVersion
        {
            get { return 1; }
        }


        /// <inheritdoc/>
        public String PersistClassString
        {
            get { return "CPN"; }
        }


        /// <inheritdoc/>
        public Object Read(Stream mask0)
        {
            IDictionary<String, String> networkParams = null;
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;
            int inputCount = 0;
            int instarCount = 0;
            int outputCount = 0;
            int winnerCount = 0;
            Matrix m1 = null;
            Matrix m2 = null;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("CPN")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    networkParams = section.ParseParams();
                }
                if (section.SectionName.Equals("CPN")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    inputCount = SyntFileSection.ParseInt(paras,
                                                           PersistConst.InputCount);
                    instarCount = SyntFileSection.ParseInt(paras,
                                                            PersistConst.Instar);
                    outputCount = SyntFileSection.ParseInt(paras,
                                                            PersistConst.OutputCount);
                    winnerCount = SyntFileSection.ParseInt(paras,
                                                            PropertyWinnerCount);
                    m1 = SyntFileSection.ParseMatrix(paras,
                                                      PropertyInputToInstar);
                    m2 = SyntFileSection.ParseMatrix(paras,
                                                      PropertyInstarToInput);
                }
            }

            var result = new CPNNetwork(inputCount, instarCount, outputCount,
                                        winnerCount);
            EngineArray.PutAll(networkParams, result.Properties);
            result.WeightsInputToInstar.Set(m1);
            result.WeightsInstarToOutstar.Set(m2);
            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var cpn = (CPNNetwork)obj;
            xout.AddSection("CPN");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(cpn.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.InputCount, cpn.InputCount);
            xout.WriteProperty(PersistConst.Instar, cpn.InstarCount);
            xout.WriteProperty(PersistConst.OutputCount, cpn.OutputCount);
            xout.WriteProperty(PropertyInputToInstar,
                               cpn.WeightsInputToInstar);
            xout.WriteProperty(PropertyInstarToInput,
                               cpn.WeightsInstarToOutstar);
            xout.WriteProperty(PropertyWinnerCount, cpn.WinnerCount);

            xout.Flush();
        }

        #endregion
    }

    public class PopulationConst
    {
        /// <summary>
        /// Property tag for the next gene id.
        /// </summary>
        ///
        public const String PropertyNextGeneID = "nextGeneID";

        /// <summary>
        /// Property tag for the next T id.
        /// </summary>
        ///
        public const String PropertyNextTID = "nextTID";

        /// <summary>
        /// Property tag for the next innovation id.
        /// </summary>
        ///
        public const String PropertyNextInnovationID = "nextInnovationID";

        /// <summary>
        /// Property tag for the next species id.
        /// </summary>
        ///
        public const String PropertyNextSpeciesID = "nextSpeciesID";

        /// <summary>
        /// Property tag for the old age penalty.
        /// </summary>
        ///
        public const String PropertyOldAgePenalty = "oldAgePenalty";

        /// <summary>
        /// Property tag for the old age threshold.
        /// </summary>
        ///
        public const String PropertyOldAgeThreshold = "oldAgeThreshold";

        /// <summary>
        /// Property tag for the population size.
        /// </summary>
        ///
        public const String PropertyPopulationSize = "populationSize";

        /// <summary>
        /// Property tag for the survival rate.
        /// </summary>
        ///
        public const String PropertySurvivalRate = "survivalRate";

        /// <summary>
        /// Property tag for the young age bonus.
        /// </summary>
        ///
        public const String PropertyYoungAgeBonus = "youngAgeBonus";

        /// <summary>
        /// Property tag for the young age threshold.
        /// </summary>
        ///
        public const String PropertyYoungAgeThreshold = "youngAgeThreshold";

        /// <summary>
        /// Property tag for the Ts collection.
        /// </summary>
        ///
        public const String PropertyTs = "Ts";

        /// <summary>
        /// Property tag for the innovations collection.
        /// </summary>
        ///
        public const String PropertyInnovations = "innovations";

        /// <summary>
        /// Property tag for the species collection.
        /// </summary>
        ///
        public const String PropertySpecies = "species";
    }

    [Serializable]
    public class BasicSpecies : ISpecies
    {
        /// <summary>
        /// The list of Ts.
        /// </summary>
        ///
        private readonly IList<IT> _members;

        /// <summary>
        /// The age of this species.
        /// </summary>
        ///
        private int _age;

        /// <summary>
        /// The best score.
        /// </summary>
        ///
        private double _bestScore;

        /// <summary>
        /// The number of generations with no improvement.
        /// </summary>
        ///
        private int _gensNoImprovement;

        /// <summary>
        /// The leader.
        /// </summary>
        ///
        private IT _leader;

        /// <summary>
        /// The id of the leader.
        /// </summary>
        [NonSerialized]
        private long _leaderID;

        /// <summary>
        /// The owner class.
        /// </summary>
        ///
        private IPopulation _population;

        /// <summary>
        /// The number of spawns required.
        /// </summary>
        ///
        private double _spawnsRequired;

        /// <summary>
        /// The species id.
        /// </summary>
        ///
        private long _speciesID;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public BasicSpecies()
        {
            _members = new List<IT>();
        }

        /// <summary>
        /// Construct a species.
        /// </summary>
        ///
        /// <param name="thePopulation">The population the species belongs to.</param>
        /// <param name="theFirst">The first T in the species.</param>
        /// <param name="theSpeciesID">The species id.</param>
        public BasicSpecies(IPopulation thePopulation, IT theFirst,
                            long theSpeciesID)
        {
            _members = new List<IT>();
            _population = thePopulation;
            _speciesID = theSpeciesID;
            _bestScore = theFirst.Score;
            _gensNoImprovement = 0;
            _age = 0;
            _leader = theFirst;
            _spawnsRequired = 0;
            _members.Add(theFirst);
        }

        /// <value>the population to set</value>
        public IPopulation Population
        {
            get { return _population; }
            set { _population = value; }
        }

        /// <summary>
        /// Set the leader id. This value is not persisted, it is used only for
        /// loading.
        /// </summary>
        ///
        /// <value>the leaderID to set</value>
        public long TempLeaderID
        {
            get { return _leaderID; }
            set { _leaderID = value; }
        }

        #region ISpecies Members

        /// <summary>
        /// Calculate the amount to spawn.
        /// </summary>
        ///
        public void CalculateSpawnAmount()
        {
            _spawnsRequired = 0;

            foreach (IT T in _members)
            {
                _spawnsRequired += T.AmountToSpawn;
            }
        }

        /// <summary>
        /// Choose a parent to mate. Choose from the population, determined by the
        /// survival rate. From this pool, a random parent is chosen.
        /// </summary>
        ///
        /// <returns>The parent.</returns>
        public IT ChooseParent()
        {
            IT baby;

            // If there is a single member, then choose that one.
            if (_members.Count == 1)
            {
                baby = _members[0];
            }
            else
            {
                // If there are many, then choose the population based on survival
                // rate
                // and select a random T.
                int maxIndexSize = (int)(_population.SurvivalRate * _members.Count) + 1;
                var theOne = (int)RangeRandomizer.Randomize(0, maxIndexSize);
                baby = _members[theOne];
            }

            return baby;
        }

        /// <summary>
        /// Set the age of this species.
        /// </summary>
        ///
        /// <value>The age of this species.</value>
        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }


        /// <summary>
        /// Set the best score.
        /// </summary>
        ///
        /// <value>The best score.</value>
        public double BestScore
        {
            get { return _bestScore; }
            set { _bestScore = value; }
        }


        /// <summary>
        /// Set the number of generations with no improvement.
        /// </summary>
        ///
        /// <value>The number of generations.</value>
        public int GensNoImprovement
        {
            get { return _gensNoImprovement; }
            set { _gensNoImprovement = value; }
        }


        /// <summary>
        /// Set the leader.
        /// </summary>
        ///
        /// <value>The new leader.</value>
        public IT Leader
        {
            get { return _leader; }
            set { _leader = value; }
        }


        /// <value>The members of this species.</value>
        public IList<IT> Members
        {
            get { return _members; }
        }


        /// <value>The number to spawn.</value>
        public double NumToSpawn
        {
            get { return _spawnsRequired; }
        }


        /// <summary>
        /// Set the number of spawns required.
        /// </summary>
        public double SpawnsRequired
        {
            get { return _spawnsRequired; }
            set { _spawnsRequired = value; }
        }


        /// <summary>
        /// Purge all members, increase age by one and count the number of
        /// generations with no improvement.
        /// </summary>
        ///
        public void Purge()
        {
            _members.Clear();
            _age++;
            _gensNoImprovement++;
            _spawnsRequired = 0;
        }

        /// <summary>
        /// Set the species id.
        /// </summary>
        public long SpeciesID
        {
            get { return _speciesID; }
            set { _speciesID = value; }
        }

        #endregion
    }

}