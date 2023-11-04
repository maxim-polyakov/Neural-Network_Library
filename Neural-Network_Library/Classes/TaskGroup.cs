using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class TaskGroup
    {
        /// <summary>
        /// The event used to sync waiting for tasks to stop.
        /// </summary>
        private readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);

        /// <summary>
        /// The ID for this task group.
        /// </summary>
        private readonly int _id;

        /// <summary>
        /// The number of tasks that have completed.
        /// </summary>
        private int _completedTasks;

        /// <summary>
        /// The total number of tasks in this group.
        /// </summary>
        private int _totalTasks;


        /// <summary>
        /// Create a task group with the specified id.
        /// </summary>
        /// <param name="id">The ID of the task group.</param>
        public TaskGroup(int id)
        {
            _id = id;
            _totalTasks = 0;
        }

        /// <summary>
        /// The ID of the task group.
        /// </summary>
        public int ID
        {
            get { return _id; }
        }

        /// <summary>
        /// Returns true if there are no more tasks.
        /// </summary>
        public bool NoTasks
        {
            get
            {
                lock (this)
                {
                    return _totalTasks == _completedTasks;
                }
            }
        }

        /// <summary>
        /// Notify that a task is starting.
        /// </summary>
        public void TaskStarting()
        {
            lock (this)
            {
                _totalTasks++;
            }
        }

        /// <summary>
        /// Notify that a task is stopping.
        /// </summary>
        public void TaskStopping()
        {
            lock (this)
            {
                _completedTasks++;
                _completeEvent.Set();
            }
        }

        /// <summary>
        /// Wait for all tasks to complete in this group.
        /// </summary>
        public void WaitForComplete()
        {
            while (!NoTasks)
            {
                _completeEvent.WaitOne();
                _completeEvent.Reset();
            }
        }
    }
}
