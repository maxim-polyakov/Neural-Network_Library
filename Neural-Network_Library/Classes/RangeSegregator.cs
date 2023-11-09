using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class RangeSegregator : ISegregator
    {
        /// <summary>
        /// If none of the ranges match, should this data be included.
        /// </summary>
        private readonly bool _include;

        /// <summary>
        /// The ranges.
        /// </summary>
        private readonly ICollection<SegregationRange> _ranges = new List<SegregationRange>();

        /// <summary>
        /// The source field that this is based on.
        /// </summary>
        private readonly IInputField _sourceField;

        /// <summary>
        /// The normalization object.
        /// </summary>
        private DataNormalization _normalization;

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public RangeSegregator()
        {
        }

        /// <summary>
        /// Construct a range segregator.
        /// </summary>
        /// <param name="sourceField">The source field.</param>
        /// <param name="include">Default action, if the data is not in any of the ranges,
        /// should it be included.</param>
        public RangeSegregator(IInputField sourceField, bool include)
        {
            _sourceField = sourceField;
            _include = include;
        }


        /// <summary>
        /// The source field that the ranges are compared against.
        /// </summary>
        public IInputField SourceField
        {
            get { return _sourceField; }
        }

        #region ISegregator Members

        /// <summary>
        /// The normalization object used by this object.
        /// </summary>
        public DataNormalization Owner
        {
            get { return _normalization; }
        }

        /// <summary>
        /// Init the object.
        /// </summary>
        /// <param name="normalization">The normalization object that owns this range.</param>
        public void Init(DataNormalization normalization)
        {
            _normalization = normalization;
        }

        /// <summary>
        /// True if the current row should be included according to this
        /// segregator.
        /// </summary>
        /// <returns></returns>
        public bool ShouldInclude()
        {
            double value = _sourceField.CurrentValue;
            foreach (SegregationRange range in _ranges)
            {
                if (range.InRange(value))
                {
                    return range.IsIncluded;
                }
            }
            return _include;
        }

        /// <summary>
        /// Init for pass... nothing to do fo this class.
        /// </summary>
        public void PassInit()
        {
        }

        #endregion

        /// <summary>
        /// Add a range.
        /// </summary>
        /// <param name="low">The low end of the range.</param>
        /// <param name="high">The high end of the range.</param>
        /// <param name="include">Should this range be included.</param>
        public void AddRange(double low, double high,
                             bool include)
        {
            var range = new SegregationRange(low, high, include);
            AddRange(range);
        }

        /// <summary>
        /// Add a range.
        /// </summary>
        /// <param name="range">The range to add.</param>
        public void AddRange(SegregationRange range)
        {
            _ranges.Add(range);
        }
    }
}
