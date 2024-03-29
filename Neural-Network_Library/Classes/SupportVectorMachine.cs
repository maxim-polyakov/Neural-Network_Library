﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class SupportVectorMachine : BasicML, IMLRegression, IMLClassification,
                                         IMLError
    {
        /// <summary>
        /// The default degree.
        /// </summary>
        ///
        public const int DefaultDegree = 3;

        /// <summary>
        /// The default COEF0.
        /// </summary>
        ///
        public const int DefaultCoef0 = 0;

        /// <summary>
        /// The default NU.
        /// </summary>
        ///
        public const double DefaultNu = 0.5d;

        /// <summary>
        /// The default cache size.
        /// </summary>
        ///
        public const int DefaultCacheSize = 100;

        /// <summary>
        /// The default C.
        /// </summary>
        ///
        public const int DefaultC = 1;

        /// <summary>
        /// The default EPS.
        /// </summary>
        ///
        public const double DefaultEps = 1e-3d;

        /// <summary>
        /// The default P.
        /// </summary>
        ///
        public const double DefaultP = 0.1d;

        /// <summary>
        /// The params for the model.
        /// </summary>
        ///
        private readonly svm_parameter _paras;

        /// <summary>
        /// The input count.
        /// </summary>
        ///
        private int _inputCount;

        /// <summary>
        /// The SVM model to use.
        /// </summary>
        ///
        private svm_model _model;

        /// <summary>
        /// Construct the SVM.
        /// </summary>
        ///
        public SupportVectorMachine()
        {
            _paras = new svm_parameter();
        }

        /// <summary>
        /// Construct an SVM network. For regression it will use an epsilon support
        /// vector. Both types will use an RBF kernel.
        /// </summary>
        ///
        /// <param name="theInputCount">The input count.</param>
        /// <param name="regression">True if this network is used for regression.</param>
        public SupportVectorMachine(int theInputCount, bool regression)
            : this(
                theInputCount,
                (regression) ? SVMType.EpsilonSupportVectorRegression : SVMType.SupportVectorClassification,
                KernelType.RadialBasisFunction)
        {
        }

        /// <summary>
        /// Construct a SVM network.
        /// </summary>
        ///
        /// <param name="theInputCount">The input count.</param>
        /// <param name="svmType">The type of SVM.</param>
        /// <param name="kernelType">The SVM kernal type.</param>
        public SupportVectorMachine(int theInputCount, SVMType svmType,
                                    KernelType kernelType)
        {
            _inputCount = theInputCount;

            _paras = new svm_parameter();

            switch (svmType)
            {
                case SVMType.SupportVectorClassification:
                    _paras.svm_type = svm_parameter.C_SVC;
                    break;
                case SVMType.NewSupportVectorClassification:
                    _paras.svm_type = svm_parameter.NU_SVC;
                    break;
                case SVMType.SupportVectorOneClass:
                    _paras.svm_type = svm_parameter.ONE_CLASS;
                    break;
                case SVMType.EpsilonSupportVectorRegression:
                    _paras.svm_type = svm_parameter.EPSILON_SVR;
                    break;
                case SVMType.NewSupportVectorRegression:
                    _paras.svm_type = svm_parameter.NU_SVR;
                    break;
                default:
                    throw new NeuralNetworkError("Invalid svm type");
            }

            switch (kernelType)
            {
                case KernelType.Linear:
                    _paras.kernel_type = svm_parameter.LINEAR;
                    break;
                case KernelType.Poly:
                    _paras.kernel_type = svm_parameter.POLY;
                    break;
                case KernelType.RadialBasisFunction:
                    _paras.kernel_type = svm_parameter.RBF;
                    break;
                case KernelType.Sigmoid:
                    _paras.kernel_type = svm_parameter.SIGMOID;
                    break;
                /*case Synt.ML.SVM.KernelType.Precomputed:
            this.paras.kernel_type = Synt.MathUtil.LIBSVM.svm_parameter.PRECOMPUTED;
            break;*/
                default:
                    throw new NeuralNetworkError("Invalid kernel type");
            }

            // params[i].kernel_type = svm_parameter.RBF;
            _paras.degree = DefaultDegree;
            _paras.coef0 = 0;
            _paras.nu = DefaultNu;
            _paras.cache_size = DefaultCacheSize;
            _paras.C = 1;
            _paras.eps = DefaultEps;
            _paras.p = DefaultP;
            _paras.shrinking = 1;
            _paras.probability = 0;
            _paras.nr_weight = 0;
            _paras.weight_label = new int[0];
            _paras.weight = new double[0];
            _paras.gamma = 1.0d / _inputCount;
        }

        /// <summary>
        /// Construct a SVM from a model.
        /// </summary>
        ///
        /// <param name="theModel">The model.</param>
        public SupportVectorMachine(svm_model theModel)
        {
            _model = theModel;
            _paras = _model.param;
            _inputCount = 0;


            // determine the input count
            foreach (var element in _model.SV)
            {
                foreach (svm_node t in element)
                {
                    _inputCount = Math.Max(t.index, _inputCount);
                }
            }

            //
        }


        /// <value>The kernel type.</value>
        public KernelType KernelType
        {
            get
            {
                switch (_paras.kernel_type)
                {
                    case svm_parameter.LINEAR:
                        return KernelType.Linear;
                    case svm_parameter.POLY:
                        return KernelType.Poly;
                    case svm_parameter.RBF:
                        return KernelType.RadialBasisFunction;
                    case svm_parameter.SIGMOID:
                        return KernelType.Sigmoid;
                    /*                case Synt.MathUtil.LIBSVM.svm_parameter.PRECOMPUTED:
                                        return Synt.ML.SVM.KernelType.Precomputed;*/
                    default:
                        return default(KernelType) /* was: null */;
                }
            }
        }


        /// <summary>
        /// Set the model.
        /// </summary>
        public svm_model Model
        {
            get { return _model; }
            set { _model = value; }
        }


        /// <value>The SVM params for each of the outputs.</value>
        public svm_parameter Params
        {
            get { return _paras; }
        }


        /// <value>The SVM type.</value>
        public SVMType SVMType
        {
            get
            {
                switch (_paras.svm_type)
                {
                    case svm_parameter.C_SVC:
                        return SVMType.SupportVectorClassification;
                    case svm_parameter.NU_SVC:
                        return SVMType.NewSupportVectorClassification;
                    case svm_parameter.ONE_CLASS:
                        return SVMType.SupportVectorOneClass;
                    case svm_parameter.EPSILON_SVR:
                        return SVMType.EpsilonSupportVectorRegression;
                    case svm_parameter.NU_SVR:
                        return SVMType.NewSupportVectorRegression;
                    default:
                        return default(SVMType) /* was: null */;
                }
            }
        }

        #region MLClassification Members

        /// <inheritdoc/>
        public int Classify(IMLData input)
        {
            if (_model == null)
            {
                throw new SyntError(
                    "Can't use the SVM yet, it has not been trained, "
                    + "and no model exists.");
            }

            svm_node[] formattedInput = MakeSparse(input);
            return (int)svm.svm_predict(_model, formattedInput);
        }

        #endregion

        #region MLError Members

        /// <summary>
        /// Calculate the error for this SVM.
        /// </summary>
        ///
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(IMLDataSet data)
        {
            switch (SVMType)
            {
                case SVMType.SupportVectorClassification:
                case SVMType.NewSupportVectorClassification:
                case SVMType.SupportVectorOneClass:
                    return SyntUtility.CalculateClassificationError(this, data);
                case SVMType.EpsilonSupportVectorRegression:
                case SVMType.NewSupportVectorRegression:
                    return SyntUtility.CalculateRegressionError(this, data);
                default:
                    return SyntUtility.CalculateRegressionError(this, data);
            }
        }

        #endregion

        #region MLRegression Members

        /// <summary>
        /// Compute the output for the given input.
        /// </summary>
        ///
        /// <param name="input">The input to the SVM.</param>
        /// <returns>The results from the SVM.</returns>
        public IMLData Compute(IMLData input)
        {
            if (_model == null)
            {
                throw new SyntError(
                    "Can't use the SVM yet, it has not been trained, "
                    + "and no model exists.");
            }

            IMLData result = new BasicMLData(1);

            svm_node[] formattedInput = MakeSparse(input);

            double d = svm.svm_predict(_model, formattedInput);
            result[0] = d;

            return result;
        }

        /// <summary>
        /// Set the input count.
        /// </summary>
        ///
        /// <value>The new input count.</value>
        public int InputCount
        {
            get { return _inputCount; }
            set { _inputCount = value; }
        }

        /// <value>For a SVM, the output count is always one.</value>
        public int OutputCount
        {
            get { return 1; }
        }

        #endregion

        /// <summary>
        /// Convert regular Synt MLData into the "sparse" data needed by an SVM.
        /// </summary>
        ///
        /// <param name="data">The data to convert.</param>
        /// <returns>The SVM sparse data.</returns>
        public svm_node[] MakeSparse(IMLData data)
        {
            var result = new svm_node[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                result[i] = new svm_node { index = i + 1, value_Renamed = data[i] };
            }

            return result;
        }

        /// <summary>
        /// Not needed, no properties to update.
        /// </summary>
        ///
        public override void UpdateProperties()
        {
            // unneeded
        }
    }
}
