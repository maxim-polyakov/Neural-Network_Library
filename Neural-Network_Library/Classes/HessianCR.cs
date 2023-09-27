using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class HessianCR : BasicHessian, IMultiThreadable
    {
        /// <summary>
        /// The number of threads to use.
        /// </summary>
        private int _numThreads;

        /// <summary>
        /// The workers.
        /// </summary>
        private ChainRuleWorker[] _workers;

        #region IMultiThreadable Members

        /// <summary>
        /// Set the number of threads. Specify zero to tell Synt to automatically
        /// determine the best number of threads for the processor. If OpenCL is used
        /// as the target device, then this value is not used.
        /// </summary>
        public int ThreadCount
        {
            get { return _numThreads; }
            set { _numThreads = value; }
        }

        #endregion

        /// <inheritdoc/>
        public override void Init(BasicNetwork theNetwork, IMLDataSet theTraining)
        {
            base.Init(theNetwork, theTraining);
            int weightCount = theNetwork.Structure.Flat.Weights.Length;

            training = theTraining;
            network = theNetwork;

            hessianMatrix = new Matrix(weightCount, weightCount);
            hessian = hessianMatrix.Data;

            // create worker(s)
            var determine = new DetermineWorkload(
                _numThreads, (int)training.Count);

            _workers = new ChainRuleWorker[determine.ThreadCount];

            int index = 0;

            // handle CPU
            foreach (IntRange r in determine.CalculateWorkers())
            {
                _workers[index++] = new ChainRuleWorker((FlatNetwork)flat.Clone(),
                                                       training.OpenAdditional(), r.Low,
                                                       r.High);
            }
        }

        /// <inheritdoc/>
        public override void Compute()
        {
            Clear();
            double e = 0;
            int weightCount = network.Flat.Weights.Length;

            for (int outputNeuron = 0; outputNeuron < network.OutputCount; outputNeuron++)
            {
                // handle context
                if (flat.HasContext)
                {
                    _workers[0].Network.ClearContext();
                }

                if (_workers.Length > 1)
                {
                    TaskGroup group = EngineConcurrency.Instance.CreateTaskGroup();

                    foreach (ChainRuleWorker worker in _workers)
                    {
                        worker.OutputNeuron = outputNeuron;
                        EngineConcurrency.Instance.ProcessTask(worker, group);
                    }

                    group.WaitForComplete();
                }
                else
                {
                    _workers[0].OutputNeuron = outputNeuron;
                    _workers[0].Run();
                }

                // aggregate workers

                foreach (ChainRuleWorker worker in _workers)
                {
                    e += worker.Error;
                    for (int i = 0; i < weightCount; i++)
                    {
                        gradients[i] += worker.Gradients[i];
                    }
                    UpdateHessian(worker.Derivative);
                }
            }

            sse = e / 2;
        }
    }
}
