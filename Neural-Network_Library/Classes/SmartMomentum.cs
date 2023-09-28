using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SmartMomentum : IStrategy
    {
        /// <summary>
        /// The minimum improvement to adjust momentum.
        /// </summary>
        ///
        public const double MinImprovement = 0.0001d;

        /// <summary>
        /// The maximum value that momentum can go to.
        /// </summary>
        ///
        public const double MaxMomentum = 4;

        /// <summary>
        /// The starting momentum.
        /// </summary>
        ///
        public const double StartMomentum = 0.1d;

        /// <summary>
        /// How much to increase momentum by.
        /// </summary>
        ///
        public const double MomentumIncrease = 0.01d;

        /// <summary>
        /// How many cycles to accept before adjusting momentum.
        /// </summary>
        ///
        public const double MomentumCycles = 10;

        /// <summary>
        /// The current momentum.
        /// </summary>
        ///
        private double _currentMomentum;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double _lastError;

        /// <summary>
        /// The last improvement in error rate.
        /// </summary>
        ///
        private double _lastImprovement;

        /// <summary>
        /// The last momentum.
        /// </summary>
        ///
        private int _lastMomentum;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start evaluation.
        /// </summary>
        ///
        private bool _ready;

        /// <summary>
        /// The setter used to change momentum.
        /// </summary>
        ///
        private IMomentum _setter;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private IMLTrain _train;

        #region IStrategy Members

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train_0">The training algorithm.</param>
        public void Init(IMLTrain train_0)
        {
            _train = train_0;
            _setter = (IMomentum)train_0;
            _ready = false;
            _setter.Momentum = 0.0d;
            _currentMomentum = 0;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        public void PostIteration()
        {
            if (_ready)
            {
                double currentError = _train.Error;
                _lastImprovement = (currentError - _lastError)
                                  / _lastError;
                SyntLogging.Log(SyntLogging.LevelDebug, "Last improvement: "
                                                           + _lastImprovement);

                if ((_lastImprovement > 0)
                    || (Math.Abs(_lastImprovement) < MinImprovement))
                {
                    _lastMomentum++;

                    if (_lastMomentum > MomentumCycles)
                    {
                        _lastMomentum = 0;
                        if (((int)_currentMomentum) == 0)
                        {
                            _currentMomentum = StartMomentum;
                        }
                        _currentMomentum *= (1.0d + MomentumIncrease);
                        _setter.Momentum = _currentMomentum;
                        SyntLogging.Log(SyntLogging.LevelDebug,
                                         "Adjusting momentum: " + _currentMomentum);
                    }
                }
                else
                {
                    SyntLogging.Log(SyntLogging.LevelDebug,
                                     "Setting momentum back to zero.");

                    _currentMomentum = 0;
                    _setter.Momentum = 0;
                }
            }
            else
            {
                _ready = true;
            }
        }

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        ///
        public void PreIteration()
        {
            _lastError = _train.Error;
        }

        #endregion
    }
}
