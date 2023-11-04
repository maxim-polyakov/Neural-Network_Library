using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class EngineConcurrency : IMultiThreadable
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static EngineConcurrency _instance = new EngineConcurrency();

        /// <summary>
        /// The number of active tasks.
        /// </summary>
        private int _activeTasks;

        private int _currentTaskGroup;

        /// <summary>
        /// The instance to the singleton.
        /// </summary>
        public static EngineConcurrency Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Construct a concurrency object.
        /// </summary>
        public EngineConcurrency()
        {
            SetMaxThreadsToCoreCount();
            _currentTaskGroup = 0;
        }

        /// <summary>
        /// Process the specified task.  It will be processed either now,
        /// or queued to process on the thread pool.  No group is assigned.
        /// </summary>
        /// <param name="task">The task to process.</param>
        public void ProcessTask(IEngineTask task)
        {
            ProcessTask(task, null);
        }

        /// <summary>
        /// Process the specified task.  It will be processed either now,
        /// or queued to process on the thread pool.
        /// </summary>
        /// <param name="task">The task to process.</param>
        /// <param name="group">The group this task belongs to.</param>
        public void ProcessTask(IEngineTask task, TaskGroup group)
        {
            lock (this)
            {
                _activeTasks++;
            }
            if (group != null)
                group.TaskStarting();
            var item = new PoolItem(this, task, group);
            ThreadPool.QueueUserWorkItem(item.ThreadPoolCallback);
        }

        /// <summary>
        /// How many threads should be used?
        /// </summary>
        public int MaxThreads
        {
            get
            {
                int t1, t2;
                ThreadPool.GetMaxThreads(out t1, out t2);
                return t1;
            }
            set
            {
                int threads = value;

                if (threads == 0)
                {
                    threads = Environment.ProcessorCount;
                    if (threads > 1)
                        threads++;
                }

                ThreadPool.SetMaxThreads(threads, threads);
            }
        }

        /// <summary>
        /// Set the max threads to the number of processors.
        /// </summary>
        public void SetMaxThreadsToCoreCount()
        {
            MaxThreads = Environment.ProcessorCount;
        }

        /// <summary>
        /// Create a new task group.
        /// </summary>
        /// <returns>The new task group.</returns>
        public TaskGroup CreateTaskGroup()
        {
            TaskGroup result;
            lock (this)
            {
                _currentTaskGroup++;
                result = new TaskGroup(_currentTaskGroup);
            }
            return result;
        }

        internal void TaskFinished(PoolItem poolItem)
        {
            lock (this)
            {
                _activeTasks--;
            }
        }

        public int ThreadCount { get; set; }
    }
}
