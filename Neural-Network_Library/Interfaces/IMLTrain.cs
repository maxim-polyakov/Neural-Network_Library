using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLTrain
    {
        /// <value>The training implementation type.</value>
        TrainingImplementationType ImplementationType { get; }


        /// <value>True if training can progress no further.</value>
        bool TrainingDone { get; }


        /// <value>The training data to use.</value>
        IMLDataSet Training { get; }


        /// <summary>
        /// Returns the training error. This value is calculated as the
	    /// training data is evaluated by the iteration function. This has
	    /// two important ramifications. First, the value returned by
	    /// getError() is meaningless prior to a call to iteration. Secondly,
	    /// the error is calculated BEFORE training is applied by the call to
	    /// iteration. The timing of the error calculation is done for
	    /// performance reasons.
        /// </summary>
        double Error { get; set; }


        /// <summary>
        /// Set the current training iteration.
        /// </summary>
        int IterationNumber { get; set; }


        /// <returns>True if the training can be paused, and later continued.</returns>
        bool CanContinue { get; }

        /// <summary>
        /// Get the current best machine learning method from the training.
        /// </summary>
        IMLMethod Method { get; }


        /// <value>The strategies to use.</value>
        IList<IStrategy> Strategies { get; }

        /// <summary>
        /// Perform one iteration of training.
        /// </summary>
        ///
        void Iteration();

        /// <summary>
        /// Should be called once training is complete and no more iterations are
        /// needed. Calling iteration again will simply begin the training again, and
        /// require finishTraining to be called once the new training session is
        /// complete.
        /// It is particularly important to call finishTraining for multithreaded
        /// training techniques.
        /// </summary>
        ///
        void FinishTraining();

        /// <summary>
        /// Perform a number of training iterations.
        /// </summary>
        ///
        /// <param name="count">The number of iterations to perform.</param>
        void Iteration(int count);

        /// <summary>
        /// Pause the training to continue later.
        /// </summary>
        ///
        /// <returns>A training continuation object.</returns>
        TrainingContinuation Pause();

        /// <summary>
        /// Resume training.
        /// </summary>
        ///
        /// <param name="state">The training continuation object to use to continue.</param>
        void Resume(TrainingContinuation state);

        /// <summary>
        /// Training strategies can be added to improve the training results. There
        /// are a number to choose from, and several can be used at once.
        /// </summary>
        ///
        /// <param name="strategy">The strategy to add.</param>
        void AddStrategy(IStrategy strategy);
    }
}
