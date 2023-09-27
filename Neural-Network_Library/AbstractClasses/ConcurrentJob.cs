using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public abstract class ConcurrentJob : IMultiThreadable
    {
        /// <summary>
        /// Where to report progress to.
        /// </summary>
        private readonly IStatusReportable _report;

        /// <summary>
        /// Total number of tasks.
        /// </summary>
        private int _totalTasks;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        protected ConcurrentJob(IStatusReportable report)
        {
            _report = report;
        }

        /// <summary>
        /// Has a stop been requested?
        /// </summary>
        public bool ShouldStop { get; set; }

        /// <summary>
        /// Called by a thread to get the next task.
        /// </summary>
        /// <returns>Config info for the next task.</returns>
        public abstract Object RequestNextTask();

        /// <summary>
        /// Load the workload that this job must process.
        /// </summary>
        /// <returns></returns>
        public abstract int LoadWorkload();

        /// <summary>
        /// Perform the actual workload.
        /// </summary>
        /// <param name="context">The workload to execute.</param>
        public abstract void PerformJobUnit(JobUnitContext context);

        /// <summary>
        /// Start the job, block until its done.
        /// </summary>
        public virtual void Process()
        {
            Object task;

            EngineConcurrency.Instance.ThreadCount = ThreadCount;

            TaskGroup group = EngineConcurrency.Instance.CreateTaskGroup();

            _totalTasks = LoadWorkload();
            int currentTask = 0;

            while ((task = RequestNextTask()) != null)
            {
                currentTask++;
                var context = new JobUnitContext { JobUnit = task, Owner = this, TaskNumber = currentTask };

                var worker = new JobUnitWorker(context);
                EngineConcurrency.Instance.ProcessTask(worker, group);
            }

            group.WaitForComplete();
        }

        /// <summary>
        /// Recieve status reports.
        /// </summary>
        /// <param name="context">The context for this job.</param>
        /// <param name="status">The current status for this job.</param>
        public void ReportStatus(JobUnitContext context, String status)
        {
            _report.Report(_totalTasks, context.TaskNumber, status);
        }

        /// <summary>
        /// Set the thread count, 0 for auto, 1 for single-threaded, 
        /// otherwise the number of threads.
        /// </summary>
        public int ThreadCount { get; set; }
    }
}
