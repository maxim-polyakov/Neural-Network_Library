using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class JobUnitWorker : IEngineTask
    {
        /// <summary>
        /// The context for this job unit.
        /// </summary>
        private readonly JobUnitContext _context;

        /// <summary>
        /// Construct a job unit worker to execute the specified job.
        /// </summary>
        /// <param name="context">The context of the job to execute.</param>
        public JobUnitWorker(JobUnitContext context)
        {
            _context = context;
        }

        #region IEngineTask Members

        /// <summary>
        /// Run this job.
        /// </summary>
        public void Run()
        {
            _context.Owner.PerformJobUnit(_context);
        }

        #endregion
    }
}
