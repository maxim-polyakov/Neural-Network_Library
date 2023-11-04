using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class DetermineWorkload
    {
        /// <summary>
        /// What is the minimum number of workload entries for a thread to be
        /// worthwhile.
        /// </summary>
        ///
        public const int MinWorthwhile = 100;

        /// <summary>
        /// How many threads to use.
        /// </summary>
        ///
        private readonly int _threadCount;

        /// <summary>
        /// What is the total workload size?
        /// </summary>
        ///
        private readonly int _workloadSize;

        /// <summary>
        /// Determine the workload.
        /// </summary>
        ///
        /// <param name="threads">Threads to use, or zero to allow Synt to pick.</param>
        /// <param name="workloadSize">Total workload size.</param>
        public DetermineWorkload(int threads, int workloadSize)
        {
            _workloadSize = workloadSize;
            if (threads == 0)
            {
                var num = (int)(Math.Log(((int)Process.GetCurrentProcess().ProcessorAffinity + 1), 2));

                // if there is more than one processor, use processor count +1
                if (num != 1)
                {
                    num++;
                }
                // if there is a single processor, just use one thread

                // Now see how big the training sets are going to be.
                // We want at least 100 training elements in each.
                // This method will likely be further "tuned" in future versions.

                long recordCount = _workloadSize;
                long workPerThread = recordCount / num;

                if (workPerThread < 100)
                {
                    num = Math.Max(1, (int)(recordCount / 100));
                }

                _threadCount = num;
            }
            else
            {
                _threadCount = Math.Min(threads, workloadSize);
            }
        }


        /// <summary>
        /// The thread count.
        /// </summary>
        public int ThreadCount
        {
            get { return _threadCount; }
        }

        /// <summary>
        /// Calculate the high and low ranges for each worker.
        /// </summary>
        /// <returns>A list of IntRange objects.</returns>
        public IList<IntRange> CalculateWorkers()
        {
            IList<IntRange> result = new List<IntRange>();
            int sizePerThread = _workloadSize / _threadCount;

            // create the workers
            for (int i = 0; i < _threadCount; i++)
            {
                int low = i * sizePerThread;
                int high;

                // if this is the last record, then high to be the last item
                // in the training set.
                if (i == (_threadCount - 1))
                {
                    high = _workloadSize - 1;
                }
                else
                {
                    high = ((i + 1) * sizePerThread) - 1;
                }

                result.Add(new IntRange(high, low));
            }

            return result;
        }
    }
}
