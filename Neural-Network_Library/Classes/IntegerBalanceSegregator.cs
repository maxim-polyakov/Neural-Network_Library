using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class IntegerBalanceSegregator : ISegregator
    {
        /// <summary>
        /// The count per each of the int values for the input field.
        /// </summary>
        private readonly int _count;

        /// <summary>
        /// The running totals.
        /// </summary>
        private readonly IDictionary<int, int> _runningCounts = new Dictionary<int, int>();

        /// <summary>
        /// The input field.
        /// </summary>
        private readonly IInputField _target;

        /// <summary>
        /// The normalization object to use.
        /// </summary>
        private DataNormalization _normalization;

        /// <summary>
        /// Construct a balanced segregator.
        /// </summary>
        /// <param name="target">The input field to base this on, should 
        /// be an integer value.</param>
        /// <param name="count">The number of rows to accept from each 
        /// unique value for the input.</param>
        public IntegerBalanceSegregator(IInputField target, int count)
        {
            _target = target;
            _count = count;
        }

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public IntegerBalanceSegregator()
        {
        }

        /// <summary>
        /// The number of groups found.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// A map of the running count for each group.
        /// </summary>
        public IDictionary<int, int> RunningCounts
        {
            get { return _runningCounts; }
        }

        /// <summary>
        /// The target input field.
        /// </summary>
        public IInputField Target
        {
            get { return _target; }
        }

        #region ISegregator Members

        /// <summary>
        /// The owner of this segregator.
        /// </summary>
        public DataNormalization Owner
        {
            get { return _normalization; }
        }

        /// <summary>
        /// Init the segregator with the owning normalization object.
        /// </summary>
        /// <param name="normalization">The data normalization object to use.</param>
        public void Init(DataNormalization normalization)
        {
            _normalization = normalization;
        }

        /// <summary>
        /// Init for a new pass.
        /// </summary>
        public void PassInit()
        {
            _runningCounts.Clear();
        }

        /// <summary>
        /// Determine of the current row should be included.
        /// </summary>
        /// <returns>True if the current row should be included.</returns>
        public bool ShouldInclude()
        {
            var key = (int)_target.CurrentValue;
            int value = 0;
            if (_runningCounts.ContainsKey(key))
            {
                value = _runningCounts[key];
            }

            if (value < _count)
            {
                value++;
                _runningCounts[key] = value;
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Get information on how many rows fall into each group.
        /// </summary>
        /// <returns>A string that contains the counts for each group.</returns>
        public String DumpCounts()
        {
            var result = new StringBuilder();

            foreach (int key in _runningCounts.Keys)
            {
                int value = _runningCounts[key];
                result.Append(key);
                result.Append(" -> ");
                result.Append(value);
                result.Append(" count\n");
            }

            return result.ToString();
        }
    }
}
