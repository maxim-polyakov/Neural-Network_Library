﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
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
}
