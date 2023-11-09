using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
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
}
