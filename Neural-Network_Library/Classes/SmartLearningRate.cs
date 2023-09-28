using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SmartLearningRate : IStrategy
    {
        /// <summary>
        /// Learning decay rate.
        /// </summary>
        ///
        public const double LearningDecay = 0.99d;

        /// <summary>
        /// The current learning rate.
        /// </summary>
        ///
        private double _currentLearningRate;

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
        /// The class that is to have the learning rate set for.
        /// </summary>
        ///
        private ILearningRate _setter;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private IMLTrain _train;

        /// <summary>
        /// The training set size, this is used to pick an initial learning rate.
        /// </summary>
        ///
        private long _trainingSize;

        #region IStrategy Members

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train">The training algorithm.</param>
        public void Init(IMLTrain train)
        {
            _train = train;
            _ready = false;
            _setter = (ILearningRate)train;
            _trainingSize = train.Training.Count;
            _currentLearningRate = 1.0d / _trainingSize;
            SyntLogging.Log(SyntLogging.LevelDebug, "Starting learning rate: "
                                                       + _currentLearningRate);
            _setter.LearningRate = _currentLearningRate;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        public void PostIteration()
        {
            if (_ready)
            {
                if (_train.Error > _lastError)
                {
                    _currentLearningRate *= LearningDecay;
                    _setter.LearningRate = _currentLearningRate;
                    SyntLogging.Log(SyntLogging.LevelDebug,
                                     "Adjusting learning rate to {}"
                                     + _currentLearningRate);
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
