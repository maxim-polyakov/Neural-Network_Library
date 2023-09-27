using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public abstract class BasicTraining : IMLTrain
    {
        private readonly TrainingImplementationType _implementationType;

        /// <summary>
        /// The training strategies to use.
        /// </summary>
        ///
        private readonly IList<IStrategy> _strategies;

        /// <summary>
        /// The current iteration.
        /// </summary>
        ///
        private int _iteration;

        /// <summary>
        /// Construct the object, specify the implementation type.
        /// </summary>
        /// <param name="implementationType"></param>
        protected BasicTraining(TrainingImplementationType implementationType)
        {
            _strategies = new List<IStrategy>();
            _implementationType = implementationType;
        }

        #region MLTrain Members

        /// <summary>
        /// Training strategies can be added to improve the training results. There
        /// are a number to choose from, and several can be used at once.
        /// </summary>
        ///
        /// <param name="strategy">The strategy to add.</param>
        public virtual void AddStrategy(IStrategy strategy)
        {
            strategy.Init(this);
            _strategies.Add(strategy);
        }

        /// <summary>
        /// Should be called after training has completed and the iteration method
        /// will not be called any further.
        /// </summary>
        ///
        public virtual void FinishTraining()
        {
        }


        /// <inheritdoc/>
        public virtual double Error { get; set; }


        /// <value>the iteration to set</value>
        public virtual int IterationNumber
        {
            get { return _iteration; }
            set { _iteration = value; }
        }


        /// <value>The strategies to use.</value>
        public virtual IList<IStrategy> Strategies
        {
            get { return _strategies; }
        }


        /// <summary>
        /// Set the training object that this strategy is working with.
        /// </summary>
        public virtual IMLDataSet Training { get; set; }


        /// <value>True if training can progress no further.</value>
        public virtual bool TrainingDone
        {
            get { return _strategies.OfType<IEndTrainingStrategy>().Any(end => end.ShouldStop()); }
        }


        /// <summary>
        /// Perform the specified number of training iterations. This is a basic
        /// implementation that just calls iteration the specified number of times.
        /// However, some training methods, particularly with the GPU, benefit
        /// greatly by calling with higher numbers than 1.
        /// </summary>
        ///
        /// <param name="count">The number of training iterations.</param>
        public virtual void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        /// <summary>
        /// The implementation type.
        /// </summary>
        public virtual TrainingImplementationType ImplementationType
        {
            get { return _implementationType; }
        }


        /// <summary>
        /// from Synt.ml.train.MLTrain
        /// </summary>
        ///
        public abstract bool CanContinue { get; }

        /// <summary>
        /// from Synt.ml.train.MLTrain
        /// </summary>
        ///
        public abstract IMLMethod Method
        {
            get;
        }


        /// <summary>
        /// from Synt.ml.train.MLTrain
        /// </summary>
        ///
        public abstract void Iteration();

        /// <summary>
        /// from Synt.ml.train.MLTrain
        /// </summary>
        ///
        public abstract TrainingContinuation Pause();

        /// <summary>
        /// from Synt.ml.train.MLTrain
        /// </summary>
        ///
        public abstract void Resume(
            TrainingContinuation state);

        #endregion

        /// <summary>
        /// Call the strategies after an iteration.
        /// </summary>
        ///
        public void PostIteration()
        {
            foreach (IStrategy strategy in _strategies)
            {
                strategy.PostIteration();
            }
        }

        /// <summary>
        /// Call the strategies before an iteration.
        /// </summary>
        ///
        public void PreIteration()
        {
            _iteration++;


            foreach (IStrategy strategy in _strategies)
            {
                strategy.PreIteration();
            }
        }
    }
}
