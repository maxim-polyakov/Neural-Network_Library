using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PoolItem
    {
        /// <summary>
        /// The task group that this item is a part of.
        /// </summary>
        private readonly TaskGroup _group;

        /// <summary>
        /// The concurrency object that started this.
        /// </summary>
        private readonly EngineConcurrency _owner;

        /// <summary>
        /// The task that was executed.
        /// </summary>
        private readonly IEngineTask _task;

        /// <summary>
        /// Construct a pool item.
        /// </summary>
        /// <param name="owner">The owner of this task.</param>
        /// <param name="task">The task to execute.</param>
        /// <param name="group">The group that this task belongs to.</param>
        public PoolItem(EngineConcurrency owner, IEngineTask task, TaskGroup group)
        {
            _owner = owner;
            _task = task;
            _group = group;
        }

        /// <summary>
        /// The thread callback.  This actually executes the task.
        /// </summary>
        /// <param name="threadContext">The thread context, not used.</param>
        public void ThreadPoolCallback(Object threadContext)
        {
            try
            {
                _task.Run();
                _owner.TaskFinished(this);
            }
            finally
            {
                if (_group != null)
                {
                    _group.TaskStopping();
                }
            }
        }
    }
}
