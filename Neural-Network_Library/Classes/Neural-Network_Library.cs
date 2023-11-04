using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Collections;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace Neural_Network_Library
{
    [Serializable]
    public class NominalItem
    {
        /// <summary>
        /// The high value for the range.
        /// </summary>
        private readonly double _high;

        /// <summary>
        /// The input field used to verify against the range.
        /// </summary>
        private readonly IInputField _inputField;

        /// <summary>
        /// The low value for the range.
        /// </summary>
        private readonly double _low;

        /// <summary>
        /// Construct a empty range item.  Used mainly for reflection.
        /// </summary>
        public NominalItem()
        {
        }

        /// <summary>
        /// Create a nominal item.
        /// </summary>
        /// <param name="inputField">The field that this item is based on.</param>
        /// <param name="high">The high value.</param>
        /// <param name="low">The low value.</param>
        public NominalItem(IInputField inputField, double low,
                           double high)
        {
            _high = high;
            _low = low;
            _inputField = inputField;
        }

        /// <summary>
        /// The high value.
        /// </summary>
        public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// The input field value.
        /// </summary>
        public IInputField InputField
        {
            get { return _inputField; }
        }

        /// <summary>
        /// The low value.
        /// </summary>
        public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// Begin a row.
        /// </summary>
        public void BeginRow()
        {
        }

        /// <summary>
        /// Determine if the specified value is in range.
        /// </summary>
        /// <returns>True if this item is within range.</returns>
        public bool IsInRange()
        {
            double currentValue = _inputField.CurrentValue;
            return ((currentValue >= _low) && (currentValue <= _high));
        }
    }

    [Serializable]
    public class OutputEquilateral : BasicOutputField
    {
        /// <summary>
        /// The high value to map into.
        /// </summary>
        private readonly double _high;

        /// <summary>
        /// The nominal items.
        /// </summary>
        private readonly IList<NominalItem> _items = new List<NominalItem>();

        /// <summary>
        /// The low value to map into.
        /// </summary>
        private readonly double _low;

        /// <summary>
        /// The current value, which nominal item is selected.
        /// </summary>
        private int _currentValue;

        /// <summary>
        /// The current equilateral matrix.
        /// </summary>
        private Equilateral _equilateral;

        /// <summary>
        /// Prodvide a default constructor for reflection.
        /// Use -1 for low and +1 for high.
        /// </summary>
        public OutputEquilateral()
            : this(1, -1)
        {
        }

        /// <summary>
        /// Create an equilateral output field with the specified high and low output
        /// values. These will often be 0 to 1 or -1 to 1.
        /// </summary>
        /// <param name="high">The high output value.</param>
        /// <param name="low">The low output value.</param>
        public OutputEquilateral(double low, double high)
        {
            _high = high;
            _low = low;
        }

        /// <summary>
        /// The equalateral table being used.
        /// </summary>
        public Equilateral Equilateral
        {
            get { return _equilateral; }
        }

        /// <summary>
        /// This is the total number of nominal items minus 1.
        /// </summary>
        public override int SubfieldCount
        {
            get { return _items.Count - 1; }
        }

        /// <summary>
        /// Add a nominal value based on a single value.  This creates a 0.1 range
        /// around this value.
        /// </summary>
        /// <param name="inputField">The input field this is based on.</param>
        /// <param name="value">The value.</param>
        public void AddItem(IInputField inputField, double value)
        {
            AddItem(inputField, value - 0.1, value + 0.1);
        }

        /// <summary>
        /// Add a nominal item based on a range.
        /// </summary>
        /// <param name="inputField">The input field to use.</param>
        /// <param name="low">The low value of the range.</param>
        /// <param name="high">The high value of the range.</param>
        public void AddItem(IInputField inputField, double low,
                            double high)
        {
            var item = new NominalItem(inputField, low, high);
            _items.Add(item);
        }

        /// <summary>
        /// Calculate the value for the specified subfield.
        /// </summary>
        /// <param name="subfield">The subfield to calculate for.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            return _equilateral.Syntesis(_currentValue)[subfield];
        }

        /// <summary>
        /// The high value of the range.
        /// </summary>
        public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// The low value of the range.
        /// </summary>
        public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// Determine which item's index is the value.
        /// </summary>
        public override void RowInit()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                NominalItem item = _items[i];
                if (item.IsInRange())
                {
                    _currentValue = i;
                    break;
                }
            }

            if (_equilateral == null)
            {
                _equilateral = new Equilateral(_items.Count, _high,
                                              _low);
            }
        }
    }

    [Serializable]
    public class OutputOneOf : BasicOutputField
    {
        /// <summary>
        /// What is the true value, often just "0" or "-1".
        /// </summary>
        private readonly double _falseValue;

        /// <summary>
        /// The nominal items to represent.
        /// </summary>
        private readonly IList<NominalItem> _items = new List<NominalItem>();

        /// <summary>
        /// What is the true value, often just "1".
        /// </summary>
        private readonly double _trueValue;

        /// <summary>
        /// Default constructor for reflection.  Use 1 for true, -1 for false.
        /// </summary>
        public OutputOneOf() : this(1, -1)
        {
        }

        /// <summary>
        /// Construct a one-of field and specify the true and false value.
        /// </summary>
        /// <param name="trueValue">The true value.</param>
        /// <param name="falseValue">The false value.</param>
        public OutputOneOf(double trueValue, double falseValue)
        {
            _trueValue = trueValue;
            _falseValue = falseValue;
        }

        /// <summary>
        /// The false value.
        /// </summary>
        public double FalseValue
        {
            get { return _falseValue; }
        }

        /// <summary>
        /// The number of subfields, or nominal classes.
        /// </summary>
        /// <returns></returns>
        public override int SubfieldCount
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Add a nominal value specifying a single value, the high and low values
        /// will be 0.5 below and 0.5 above.
        /// </summary>
        /// <param name="inputField">The input field to use.</param>
        /// <param name="value">The value to calculate the high and low values off of.</param>
        public void AddItem(IInputField inputField, double value)
        {
            AddItem(inputField, value - 0.5, value + 0.5);
        }

        /// <summary>
        /// Add a nominal item, specify the low and high values.
        /// </summary>
        /// <param name="inputField">The input field to base everything from.</param>
        /// <param name="low">The high value for this nominal item.</param>
        /// <param name="high">The low value for this nominal item.</param>
        public void AddItem(IInputField inputField, double low,
                            double high)
        {
            var item = new NominalItem(inputField, low, high);
            _items.Add(item);
        }

        /// <summary>
        /// Calculate the value for the specified subfield.
        /// </summary>
        /// <param name="subfield">The subfield to calculate for.</param>
        /// <returns>The calculated value for this field.</returns>
        public override double Calculate(int subfield)
        {
            NominalItem item = _items[subfield];
            return item.IsInRange() ? _trueValue : _falseValue;
        }

        /// <summary>
        /// The true value.
        /// </summary>
        /// <returns></returns>
        public double TrueValue
        {
            get { return _trueValue; }
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }

    [Serializable]
    public class OutputFieldZAxis : OutputFieldGrouped
    {
        /// <summary>
        /// Construct a ZAxis output field.
        /// </summary>
        /// <param name="group">The group this field belongs to.</param>
        /// <param name="field">The input field this is based on.</param>
        public OutputFieldZAxis(IOutputFieldGroup group,
                                IInputField field)
            : base(group, field)
        {
            if (!(group is ZAxisGroup))
            {
                throw new NormalizationError(
                    "Must use ZAxisGroup with OutputFieldZAxis.");
            }
        }

        /// <summary>
        /// The subfield count, which is one, as this field type does not
        /// have subfields.
        /// </summary>
        public override int SubfieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Calculate the current value for this field. 
        /// </summary>
        /// <param name="subfield">Ignored, this field type does not have subfields.</param>
        /// <returns>The current value for this field.</returns>
        public override double Calculate(int subfield)
        {
            return (SourceField.CurrentValue * ((ZAxisGroup)Group)
                                                 .Multiplier);
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }

    [Serializable]
    public class OutputFieldZAxisSynthetic : OutputFieldGrouped
    {
        /// <summary>
        /// Construct a synthetic output field for Z-Axis.
        /// </summary>
        /// <param name="group">The Z-Axis group that this belongs to.</param>
        public OutputFieldZAxisSynthetic(IOutputFieldGroup group)
            : base(group, null)
        {
            if (!(group is ZAxisGroup))
            {
                throw new NormalizationError(
                    "Must use ZAxisGroup with OutputFieldZAxisSynthetic.");
            }
        }

        /// <summary>
        /// The subfield count, which is one, as this field type does not
        /// have subfields.
        /// </summary>
        public override int SubfieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Calculate the synthetic value for this Z-Axis normalization.
        /// </summary>
        /// <param name="subfield">Not used.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            double l = ((ZAxisGroup)Group).Length;
            double f = ((ZAxisGroup)Group).Multiplier;
            double n = Group.GroupedFields.Count;
            double result = f * Math.Sqrt(n - (l * l));
            return double.IsInfinity(result) || double.IsNaN(result) ? 0 : result;
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }

    [Serializable]
    public class ZAxisGroup : BasicOutputFieldGroup
    {
        /// <summary>
        /// The calculated length.
        /// </summary>
        private double _length;

        /// <summary>
        /// The multiplier, which is the value that all other values will be
        /// multiplied to become normalized.
        /// </summary>
        private double _multiplier;

        /// <summary>
        /// The vector length.
        /// </summary>
        public double Length
        {
            get { return _length; }
        }

        /// <summary>
        /// The value to multiply the other values by to normalize them.
        /// </summary>
        public double Multiplier
        {
            get { return _multiplier; }
        }

        /// <summary>
        /// Initialize this group for a new row.
        /// </summary>
        public override void RowInit()
        {
            double value = (from field in GroupedFields
                            where !(field is OutputFieldZAxisSynthetic)
                            where field.SourceField != null
                            select (field.SourceField.CurrentValue * field.SourceField.CurrentValue)).Sum();

            _length = Math.Sqrt(value);
            _multiplier = 1.0 / Math.Sqrt(GroupedFields.Count);
        }
    }

    [Serializable]
    public class OutputFieldDirect : BasicOutputField
    {
        /// <summary>
        /// The source field.
        /// </summary>
        private readonly IInputField _sourceField;


        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public OutputFieldDirect()
        {
        }

        /// <summary>
        /// Construct a direct output field.
        /// </summary>
        /// <param name="sourceField">The source field to pass directly on.</param>
        public OutputFieldDirect(IInputField sourceField)
        {
            _sourceField = sourceField;
        }

        /// <summary>
        /// Always returns 1, as subfields are not used.
        /// </summary>
        public override int SubfieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Calculate the value for this field. This will simply be the
        /// value from the input field. 
        /// </summary>
        /// <param name="subfield">Not used, as this output field type does not
        /// support subfields.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            return _sourceField.CurrentValue;
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }

    public class OutputFieldRangeMapped : BasicOutputField, IRequireTwoPass
    {
        /// <summary>
        /// The input field to scale.
        /// </summary>
        private readonly IInputField _field;

        /// <summary>
        /// The high value of the field.
        /// </summary>
        private readonly double _high;

        /// <summary>
        /// The low value of the field.
        /// </summary>
        private readonly double _low;

        /// <summary>
        /// Default constructor, used mainly for reflection.
        /// </summary>
        public OutputFieldRangeMapped()
        {
        }

        /// <summary>
        /// Construct a range mapped output field.
        /// </summary>
        /// <param name="field">The input field to base this on.</param>
        /// <param name="low">The low value.</param>
        /// <param name="high">The high value.</param>
        public OutputFieldRangeMapped(IInputField field, double low,
                                      double high)
        {
            _field = field;
            _low = low;
            _high = high;
        }

        /// <summary>
        /// Construct the output field with -1 for low and +1 for high.
        /// </summary>
        /// <param name="f">The input field.</param>
        public OutputFieldRangeMapped(IInputField f) : this(f, -1, 1)
        {
        }

        /// <summary>
        /// The field that this output is based on.
        /// </summary>
        public IInputField Field
        {
            get { return _field; }
        }

        /// <summary>
        /// The high value of the range to map into.
        /// </summary>
        public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// The low value of the range to map into.
        /// </summary>
        public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// This field only produces one value, so this will return 1.
        /// </summary>
        public override int SubfieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Calculate this output field.
        /// </summary>
        /// <param name="subfield">Not used.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            return ((_field.CurrentValue - _field.Min) / (_field
                                                          .Max - _field.Min))
                   * (_high - _low) + _low;
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }

        /// <summary>
        /// Convert a number back after its been normalized.
        /// </summary>
        /// <param name="data">The number to convert back.</param>
        /// <returns>The result.</returns>
        public double ConvertBack(double data)
        {
            double result = ((_field.Min - _field.Max) * data - _high
                                                            * _field.Min + _field.Max * _low)
                            / (_low - _high);
            return result;
        }
    }

    [Serializable]
    public class IndexRangeSegregator : IndexSegregator
    {
        /// <summary>
        /// The ending index.
        /// </summary>        
        private readonly int _endingIndex;

        /// <summary>
        /// The starting index.
        /// </summary>
        private readonly int _startingIndex;

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public IndexRangeSegregator()
        {
        }

        /// <summary>
        /// Construct an index range segregator.
        /// </summary>
        /// <param name="startingIndex">The starting index to allow.</param>
        /// <param name="endingIndex">The ending index to allow.</param>
        public IndexRangeSegregator(int startingIndex, int endingIndex)
        {
            _startingIndex = startingIndex;
            _endingIndex = endingIndex;
        }

        /// <summary>
        /// The ending index.
        /// </summary>
        public int EndingIndex
        {
            get { return _endingIndex; }
        }

        /// <summary>
        /// The starting index.
        /// </summary>
        public int StartingIndex
        {
            get { return _startingIndex; }
        }

        /// <summary>
        /// Determines if the current row should be included.
        /// </summary>
        /// <returns>True if the current row should be included.</returns>
        public override bool ShouldInclude()
        {
            bool result = ((CurrentIndex >= _startingIndex) && (CurrentIndex <= _endingIndex));
            RollIndex();
            return result;
        }
    }

    [Serializable]
    public class IndexSampleSegregator : IndexSegregator
    {
        /// <summary>
        /// The ending index (within a sample).
        /// </summary>
        private readonly int _endingIndex;

        /// <summary>
        /// The sample size.
        /// </summary>
        private readonly int _sampleSize;

        /// <summary>
        /// The starting index (within a sample).
        /// </summary>
        private readonly int _startingIndex;

        /// <summary>
        /// The default constructor, for reflection.
        /// </summary>
        public IndexSampleSegregator()
        {
        }

        /// <summary>
        /// Construct an index sample segregator.
        /// </summary>
        /// <param name="startingIndex">The starting index.</param>
        /// <param name="endingIndex">The ending index.</param>
        /// <param name="sampleSize">The sample size.</param>
        public IndexSampleSegregator(int startingIndex,
                                     int endingIndex, int sampleSize)
        {
            _sampleSize = sampleSize;
            _startingIndex = startingIndex;
            _endingIndex = endingIndex;
        }

        /// <summary>
        /// The ending index.
        /// </summary>
        public int EndingIndex
        {
            get { return _endingIndex; }
        }

        /// <summary>
        /// The sample size.
        /// </summary>
        public int SampleSize
        {
            get { return _sampleSize; }
        }

        /// <summary>
        /// The starting index.
        /// </summary>
        public int StartingIndex
        {
            get { return _startingIndex; }
        }

        /// <summary>
        /// Should this row be included.
        /// </summary>
        /// <returns>True if this row should be included.</returns>
        public override bool ShouldInclude()
        {
            int sampleIndex = CurrentIndex % _sampleSize;
            RollIndex();
            return ((sampleIndex >= _startingIndex) && (sampleIndex <= _endingIndex));
        }
    }

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

    [Serializable]
    public class SegregationRange
    {
        /// <summary>
        /// The high end of this range.
        /// </summary>
        private readonly double _high;

        /// <summary>
        /// Should this range be included.
        /// </summary>
        private readonly bool _include;

        /// <summary>
        /// The low end of this range.
        /// </summary>
        private readonly double _low;

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public SegregationRange()
        {
        }

        /// <summary>
        /// Construct a segregation range.
        /// </summary>
        /// <param name="low">The low end of the range.</param>
        /// <param name="high">The high end of the range.</param>
        /// <param name="include">Specifies if the range should be included.</param>
        public SegregationRange(double low, double high,
                                bool include)
        {
            _low = low;
            _high = high;
            _include = include;
        }

        /// <summary>
        /// The high end of the range.
        /// </summary>
        public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// The low end of the range.
        /// </summary>
        public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// True if this range should be included.
        /// </summary>
        public bool IsIncluded
        {
            get { return _include; }
        }

        /// <summary>
        /// Is this value within the range. 
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is within the range.</returns>
        public bool InRange(double value)
        {
            return ((value >= _low) && (value <= _high));
        }
    }

    [Serializable]
    public class NormalizationStorageArray1D : INormalizationStorage
    {
        /// <summary>
        /// The array to store to.
        /// </summary>
        private readonly double[] _array;

        /// <summary>
        /// The current index.
        /// </summary>
        private int _currentIndex;


        /// <summary>
        /// Construct an object to store to a 2D array.
        /// </summary>
        /// <param name="array">The array to store to.</param>
        public NormalizationStorageArray1D(double[] array)
        {
            _array = array;
            _currentIndex = 0;
        }

        #region INormalizationStorage Members

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Open()
        {
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount">How much of the data is input.</param>
        public void Write(double[] data, int inputCount)
        {
            _array[_currentIndex++] = data[0];
        }

        #endregion

        public double[] GetArray()
        {
            return _array;
        }
    }

    [Serializable]
    public class NormalizationStorageArray2D : INormalizationStorage
    {
        /// <summary>
        /// The array to output to.
        /// </summary>
        private readonly double[][] _array;

        /// <summary>
        /// The current data.
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// Construct an object to store to a 2D array.
        /// </summary>
        /// <param name="array">The array to store to.</param>
        public NormalizationStorageArray2D(double[][] array)
        {
            _array = array;
            _currentIndex = 0;
        }

        #region INormalizationStorage Members

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Open()
        {
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount">How much of the data is input.</param>
        public void Write(double[] data, int inputCount)
        {
            for (int i = 0; i < data.Length; i++)
            {
                _array[_currentIndex][i] = data[i];
            }
            _currentIndex++;
        }

        #endregion

        /// <summary>
        /// Get the underlying array.
        /// </summary>
        /// <returns>The underlying array.</returns>
        public double[][] GetArray()
        {
            return this._array;
        }
    }

    [Serializable]
    public class NormalizationStorageCSV : INormalizationStorage
    {
        /// <summary>
        /// The CSV format to use.
        /// </summary>
        private readonly CSVFormat _format;

        /// <summary>
        /// The output file.
        /// </summary> 
        private readonly String _outputFile;

        /// <summary>
        /// The output writer.
        /// </summary>
        private StreamWriter _output;

        /// <summary>
        /// Construct a CSV storage object from the specified file.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <param name="file">The file to write the CSV to.</param>
        public NormalizationStorageCSV(CSVFormat format, String file)
        {
            _format = format;
            _outputFile = file;
        }

        /// <summary>
        /// Construct a CSV storage object from the specified file.
        /// </summary>
        /// <param name="file">The file to write the CSV to.</param>
        public NormalizationStorageCSV(String file)
        {
            _format = CSVFormat.English;
            _outputFile = file;
        }

        #region INormalizationStorage Members

        /// <summary>
        /// Close the CSV file.
        /// </summary>
        public void Close()
        {
            _output.Close();
        }

        /// <summary>
        /// Open the CSV file.
        /// </summary>
        public void Open()
        {
            _output = new StreamWriter(_outputFile);
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount"> How much of the data is input.</param>
        public void Write(double[] data, int inputCount)
        {
            var result = new StringBuilder();
            NumberList.ToList(_format, result, data);
            _output.WriteLine(result.ToString());
        }

        #endregion
    }

    [Serializable]
    public class NormalizationStorageMLDataSet : INormalizationStorage
    {
        /// <summary>
        /// The data set to add to.
        /// </summary>
        [NonSerialized]
        private readonly IMLDataSet _dataset;

        /// <summary>
        /// The ideal count.
        /// </summary>
        private readonly int _idealCount;

        /// <summary>
        /// The input count.
        /// </summary>
        private readonly int _inputCount;

        /// <summary>
        /// Construct a new NeuralDataSet based on the parameters specified.
        /// </summary>
        /// <param name="inputCount">The input count.</param>
        /// <param name="idealCount">The output count.</param>
        public NormalizationStorageMLDataSet(int inputCount,
                                                 int idealCount)
        {
            _inputCount = inputCount;
            _idealCount = idealCount;
            _dataset = new BasicMLDataSet();
        }

        /// <summary>
        /// Construct a normalized neural storage class to hold data.
        /// </summary>
        /// <param name="dataset">The data set to store to. This uses an existing data set.</param>
        public NormalizationStorageMLDataSet(IMLDataSet dataset)
        {
            _dataset = dataset;
            _inputCount = _dataset.InputSize;
            _idealCount = _dataset.IdealSize;
        }

        /// <summary>
        /// The data set being used.
        /// </summary>
        public IMLDataSet DataSet
        {
            get { return _dataset; }
        }

        #region INormalizationStorage Members

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Open()
        {
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount">How much of the data is input.</param>
        public void Write(double[] data, int inputCount)
        {
            if (_idealCount == 0)
            {
                var inputData = new BasicMLData(data);
                _dataset.Add(inputData);
            }
            else
            {
                var inputData = new BasicMLData(
                    _inputCount);
                var idealData = new BasicMLData(
                    _idealCount);

                int index = 0;
                for (int i = 0; i < _inputCount; i++)
                {
                    inputData[i] = data[index++];
                }

                for (int i = 0; i < _idealCount; i++)
                {
                    idealData[i] = data[index++];
                }

                _dataset.Add(inputData, idealData);
            }
        }

        #endregion
    }

    [Serializable]
    public class DataNormalization
    {
        /// <summary>
        /// Hold a map between the InputFieldCSV objects and the corresponding
        /// ReadCSV object. There will likely be many fields read from a single file.
        /// This allows only one ReadCSV object to need to be created per actual CSV
        /// file.
        /// </summary>
        [NonSerialized]
        private IDictionary<IInputField, ReadCSV> _csvMap;


        /// <summary>
        /// Map each of the input fields to an internally-build NeuralDataFieldHolder object.
        /// The NeuralDataFieldHolder object holds an Iterator, InputField and last 
        /// NeuralDataPair object loaded.
        /// </summary>
        [NonSerialized]
        private IDictionary<IInputField, MLDataFieldHolder> _dataSetFieldMap;

        /// <summary>
        /// Map each of the NeuralDataSet Iterators to an internally-build NeuralDataFieldHolder 
        /// object. The NeuralDataFieldHolder object holds an Iterator, InputField and last 
        /// NeuralDataPair object loaded.
        /// </summary>
        [NonSerialized]
        private IDictionary<IEnumerator<IMLDataPair>, MLDataFieldHolder> _dataSetIteratorMap;

        /// <summary>
        /// Output fields can be grouped together, if the value of one output field might 
        /// affect all of the others.  This collection holds a list of all of the output 
        /// field groups.
        /// </summary>
        private readonly IList<IOutputFieldGroup> _groups = new List<IOutputFieldGroup>();

        /// <summary>
        /// The input fields.
        /// </summary>
        private readonly IList<IInputField> _inputFields = new List<IInputField>();

        /// <summary>
        /// The output fields.
        /// </summary>
        private readonly IList<IOutputField> _outputFields = new List<IOutputField>();

        /// <summary>
        /// Keep a collection of all of the ReadCSV classes to support all of the
        /// distinct CSV files that are to be read.
        /// </summary>
        [NonSerialized]
        private ICollection<ReadCSV> _readCSV;

        /// <summary>
        /// For each InputFieldNeuralDataSet input field an Iterator must be kept to
        /// actually access the data. Only one Iterator should be kept per data set
        /// actually used.
        /// </summary>
        [NonSerialized]
        private ICollection<IEnumerator<IMLDataPair>> _readDataSet;

        /// <summary>
        /// A list of the segregators.
        /// </summary>
        private readonly IList<ISegregator> _segregators = new List<ISegregator>();

        /// <summary>
        /// The format to use for all CSV files.
        /// </summary>
        private CSVFormat _csvFormat = CSVFormat.English;

        /// <summary>
        /// The current record's index.
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// How long has it been since the last report.  This filters so that
        /// every single record does not produce a message.
        /// </summary>
        private int _lastReport;

        /// <summary>
        /// The number of records that were found in the first pass.
        /// </summary>
        private int _recordCount;

        /// <summary>
        /// The object to report the progress of the normalization to.
        /// </summary>
        [NonSerialized]
        private IStatusReportable _report = new NullStatusReportable();

        /// <summary>
        /// Where the final output from the normalization is sent.
        /// </summary>
        private INormalizationStorage _storage;

        /// <summary>
        /// The CSV format being used.
        /// </summary>
        public CSVFormat CSVFormatUsed
        {
            get { return _csvFormat; }
            set { _csvFormat = value; }
        }


        /// <summary>
        /// The object groups.
        /// </summary>
        public IList<IOutputFieldGroup> Groups
        {
            get { return _groups; }
        }

        /// <summary>
        /// The input fields.
        /// </summary>
        public IList<IInputField> InputFields
        {
            get { return _inputFields; }
        }

        /// <summary>
        /// The output fields.
        /// </summary>
        public IList<IOutputField> OutputFields
        {
            get { return _outputFields; }
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int RecordCount
        {
            get { return _recordCount; }
        }

        /// <summary>
        /// The class that progress will be reported to.
        /// </summary>
        public IStatusReportable Report
        {
            get { return _report; }
            set { _report = value; }
        }

        /// <summary>
        /// The segregators in use.
        /// </summary>
        public IList<ISegregator> Segregators
        {
            get { return _segregators; }
        }

        /// <summary>
        /// The place that the normalization output will be stored.
        /// </summary>
        public INormalizationStorage Storage
        {
            get { return _storage; }
            set { _storage = value; }
        }

        /// <summary>
        /// Add an input field.
        /// </summary>
        /// <param name="f">The input field to add.</param>
        public void AddInputField(IInputField f)
        {
            _inputFields.Add(f);
        }

        /// <summary>
        ///  Add an output field.  This output field will be added as a 
        /// "neural network input field", not an "ideal output field".
        /// </summary>
        /// <param name="outputField">The output field to add.</param>
        public void AddOutputField(IOutputField outputField)
        {
            AddOutputField(outputField, false);
        }

        /// <summary>
        /// Add a field and allow it to be specified as an "ideal output field".
        /// An "ideal" field is the expected output that the neural network is
        /// training towards.
        /// </summary>
        /// <param name="outputField">The output field.</param>
        /// <param name="ideal">True if this is an ideal field.</param>
        public void AddOutputField(IOutputField outputField,
                                   bool ideal)
        {
            _outputFields.Add(outputField);
            outputField.Ideal = ideal;
            if (outputField is OutputFieldGrouped)
            {
                var ofg = (OutputFieldGrouped)outputField;
                _groups.Add(ofg.Group);
            }
        }

        /// <summary>
        ///  Add a segregator.
        /// </summary>
        /// <param name="segregator">The segregator to add.</param>
        public void AddSegregator(ISegregator segregator)
        {
            _segregators.Add(segregator);
            segregator.Init(this);
        }

        /// <summary>
        /// Called internally to allow each of the input fields to update their
        /// min/max values in the first pass.
        /// </summary>
        private void ApplyMinMax()
        {
            foreach (IInputField field in _inputFields)
            {
                double value = field.CurrentValue;
                field.ApplyMinMax(value);
            }
        }

        /// <summary>
        /// Build "input data for a neural network" based on the input values
        /// provided.  This allows  input for a neural network to be normalized.
        /// This is typically used when data is to be presented to a trained
        /// neural network.
        /// </summary>
        /// <param name="data">The input values to be normalized.</param>
        /// <returns>The data to be sent to the neural network.</returns>
        public IMLData BuildForNetworkInput(double[] data)
        {
            // feed the input fields
            int index = 0;
            foreach (IInputField field in _inputFields)
            {
                if (field.UsedForNetworkInput)
                {
                    if (index >= data.Length)
                    {

                    }
                    field.CurrentValue = data[index++];
                }
            }

            // count the output fields
            int outputCount = 0;
            foreach (IOutputField ofield in _outputFields)
            {
                if (!ofield.Ideal)
                {
                    for (int sub = 0; sub < ofield.SubfieldCount; sub++)
                    {
                        outputCount++;
                    }
                }
            }

            // process the output fields

            InitForOutput();

            IMLData result = new BasicNeuralData(outputCount);

            // write the value
            int outputIndex = 0;
            foreach (IOutputField ofield in _outputFields)
            {
                if (!ofield.Ideal)
                {
                    for (int sub = 0; sub < ofield.SubfieldCount; sub++)
                    {
                        result.Data[outputIndex++] = ofield.Calculate(sub);
                    }
                }
            }

            return result;
        }



        private void DetermineInputFieldValue(IInputField field, int index, bool headers)
        {
            double result;

            if (field is InputFieldCSV)
            {
                var fieldCSV = (InputFieldCSV)field;
                ReadCSV csv = _csvMap[field];
                result = csv.GetDouble(fieldCSV.ColumnName);

            }
            else if (field is InputFieldMLDataSet)
            {
                var mlField = (InputFieldMLDataSet)field;
                MLDataFieldHolder holder = _dataSetFieldMap
                    [field];
                IMLDataPair pair = holder.Pair;
                int offset = mlField.Offset;
                if (offset < pair.Input.Count)
                {
                    result = pair.Input[offset];
                }
                else
                {
                    offset -= pair.Input.Count;
                    result = pair.Ideal[offset];
                }
            }
            else
            {
                result = field.GetValue(index);
            }

            field.CurrentValue = result;
            return;
        }
        /// <summary>
        /// Called internally to obtain the current value for an input field.
        /// </summary>
        /// <param name="field">The input field to determine.</param>
        /// <param name="index">The current index.</param>
        /// <returns>The value for this input field.</returns>
        private void DetermineInputFieldValue(IInputField field, int index)
        {
            double result;

            if (field is InputFieldCSV)
            {
                var fieldCSV = (InputFieldCSV)field;
                ReadCSV csv = _csvMap[field];
                result = csv.GetDouble(fieldCSV.Offset);

            }
            else if (field is InputFieldMLDataSet)
            {
                var mlField = (InputFieldMLDataSet)field;
                MLDataFieldHolder holder = _dataSetFieldMap
                    [field];
                IMLDataPair pair = holder.Pair;
                int offset = mlField.Offset;
                if (offset < pair.Input.Count)
                {
                    result = pair.Input[offset];
                }
                else
                {
                    offset -= pair.Input.Count;
                    result = pair.Ideal[offset];
                }
            }
            else
            {
                result = field.GetValue(index);
            }

            field.CurrentValue = result;
            return;
        }

        /// <summary>
        /// Called internally to determine all of the input field values.
        /// </summary>
        /// <param name="index">The current index.</param>
        private void DetermineInputFieldValues(int index)
        {
            foreach (IInputField field in _inputFields)
            {
                DetermineInputFieldValue(field, index);
            }
        }


        /// <summary>
        /// Called internally to determine all of the input field values.
        /// </summary>
        /// <param name="index">The current index.</param>
        /// <param name="headers">if set to <c>true</c> [headers].</param>
        private void DetermineInputFieldValues(int index, bool headers)
        {
            foreach (IInputField field in _inputFields)
            {
                DetermineInputFieldValue(field, index, headers);
            }
        }


        /// <summary>
        /// Find an input field by its class.
        /// </summary>
        /// <param name="clazz">The input field class type you are looking for.</param>
        /// <param name="count">The instance of the input field needed, 0 for the first.</param>
        /// <returns>The input field if found, otherwise null.</returns>
        public IInputField FindInputField(Type clazz, int count)
        {
            int i = 0;
            foreach (IInputField field in _inputFields)
            {
                if (field.GetType().IsInstanceOfType(clazz))
                {
                    if (i == count)
                    {
                        return field;
                    }
                    i++;
                }
            }

            return null;
        }

        /// <summary>
        /// Find an output field by its class.
        /// </summary>
        /// <param name="clazz">The output field class type you are looking for.</param>
        /// <param name="count">The instance of the output field needed, 0 for the first.</param>
        /// <returns>The output field if found, otherwise null.</returns>
        public IOutputField FindOutputField(Type clazz, int count)
        {
            int i = 0;
            foreach (IOutputField field in _outputFields)
            {
                if (field.GetType().IsInstanceOfType(clazz) || field.GetType() == clazz)
                {
                    if (i == count)
                    {
                        return field;
                    }
                    i++;
                }
            }

            return null;
        }

        /// <summary>
        /// First pass, count everything, establish min/max.
        /// </summary>
        private void FirstPass(bool headers)
        {
            OpenCSV(headers);
            OpenDataSet();

            _currentIndex = -1;
            _recordCount = 0;

            if (_report != null)
            {
                _report.Report(0, 0, "Analyzing file");
            }
            _lastReport = 0;
            int index = 0;

            InitForPass();

            // loop over all of the records
            while (Next())
            {
                DetermineInputFieldValues(index, headers);

                if (ShouldInclude())
                {
                    ApplyMinMax();
                    _recordCount++;
                    ReportResult("First pass, analyzing file", 0, _recordCount);
                }
                index++;
            }
        }




        /// <summary>
        /// First pass, count everything, establish min/max.
        /// This version doesn't read column names in csvinputfields.
        /// </summary>
        private void FirstPass()
        {
            OpenCSV();
            OpenDataSet();

            _currentIndex = -1;
            _recordCount = 0;

            if (_report != null)
            {
                _report.Report(0, 0, "Analyzing file");
            }
            _lastReport = 0;
            int index = 0;

            InitForPass();

            // loop over all of the records
            while (Next())
            {
                DetermineInputFieldValues(index);

                if (ShouldInclude())
                {
                    ApplyMinMax();
                    _recordCount++;
                    ReportResult("First pass, analyzing file", 0, _recordCount);
                }
                index++;
            }
        }





        /// <summary>
        /// Calculate the number of output fields that are not used as ideal
        /// values, these will be the input to the neural network.
        /// This is the input layer size for the neural network.
        /// </summary>
        /// <returns>The input layer size.</returns>
        public int GetNetworkInputLayerSize()
        {
            return _outputFields.Where(field => !field.Ideal).Sum(field => field.SubfieldCount);
        }

        /// <summary>
        /// The number of output fields that are used as ideal
        /// values, these will be the ideal output from the neural network.
        /// This is the output layer size for the neural network.
        /// </summary>
        /// <returns>The output layer size.</returns>
        public int GetNetworkOutputLayerSize()
        {
            return _outputFields.Where(field => field.Ideal).Sum(field => field.SubfieldCount);
        }

        /// <summary>
        /// The total size of all output fields.  This takes into
        /// account output fields that generate more than one value.
        /// </summary>
        /// <returns>The output field count.</returns>
        public int GetOutputFieldCount()
        {
            return _outputFields.Sum(field => field.SubfieldCount);
        }

        /// <summary>
        /// Setup the row for output.
        /// </summary>
        public void InitForOutput()
        {
            // init groups
            foreach (IOutputFieldGroup group in _groups)
            {
                group.RowInit();
            }

            // init output fields
            foreach (IOutputField field in _outputFields)
            {
                field.RowInit();
            }
        }

        /// <summary>
        /// Called internally to advance to the next row.
        /// </summary>
        /// <returns>True if there are more rows to reed.</returns>
        private bool Next()
        {
            // see if any of the CSV readers want to stop
            if (_readCSV.Any(csv => !csv.Next()))
            {
                return false;
            }

            // see if any of the data sets want to stop
            foreach (var iterator in _readDataSet)
            {
                if (!iterator.MoveNext())
                {
                    return false;
                }
                MLDataFieldHolder holder = _dataSetIteratorMap
                    [iterator];
                IMLDataPair pair = iterator.Current;
                holder.Pair = pair;
            }

            // see if any of the arrays want to stop
            if (_inputFields.OfType<IHasFixedLength>().Any(fl => (_currentIndex + 1) >= fl.Length))
            {
                return false;
            }

            _currentIndex++;

            return true;
        }

        /// <summary>
        /// Called internally to open the CSV file.
        /// </summary>
        private void OpenCSV()
        {
            // clear out any CSV files already there
            _csvMap.Clear();
            _readCSV.Clear();

            // only add each CSV once
            IDictionary<String, ReadCSV> uniqueFiles = new Dictionary<String, ReadCSV>();

            // find the unique files
            foreach (IInputField field in _inputFields)
            {
                if (field is InputFieldCSV)
                {
                    var csvField = (InputFieldCSV)field;
                    String file = csvField.File;
                    if (!uniqueFiles.ContainsKey(file))
                    {
                        var csv = new ReadCSV(file, false,
                                              _csvFormat);
                        uniqueFiles[file] = csv;
                        _readCSV.Add(csv);
                    }
                    _csvMap[csvField] = uniqueFiles[file];
                }
            }
        }

        /// <summary>
        /// Called internally to open the CSV file with header.
        /// </summary>
        private void OpenCSV(bool headers)
        {
            // clear out any CSV files already there
            _csvMap.Clear();
            _readCSV.Clear();

            // only add each CSV once
            IDictionary<String, ReadCSV> uniqueFiles = new Dictionary<String, ReadCSV>();

            // find the unique files
            foreach (IInputField field in _inputFields)
            {
                if (field is InputFieldCSV)
                {
                    var csvField = (InputFieldCSV)field;
                    String file = csvField.File;
                    if (!uniqueFiles.ContainsKey(file))
                    {
                        var csv = new ReadCSV(file, headers,
                                              _csvFormat);
                        uniqueFiles[file] = csv;
                        _readCSV.Add(csv);
                    }
                    _csvMap[csvField] = uniqueFiles[file];
                }
            }
        }


        /// <summary>
        /// Open any datasets that were used by the input layer.
        /// </summary>
        private void OpenDataSet()
        {
            // clear out any data sets already there
            _readDataSet.Clear();
            _dataSetFieldMap.Clear();
            _dataSetIteratorMap.Clear();

            // only add each iterator once
            IDictionary<IMLDataSet, MLDataFieldHolder> uniqueSets = new Dictionary<IMLDataSet, MLDataFieldHolder>();

            // find the unique files
            foreach (IInputField field in _inputFields)
            {
                if (field is InputFieldMLDataSet)
                {
                    var dataSetField = (InputFieldMLDataSet)field;
                    IMLDataSet dataSet = dataSetField.NeuralDataSet;
                    if (!uniqueSets.ContainsKey(dataSet))
                    {
                        IEnumerator<IMLDataPair> iterator = dataSet
                            .GetEnumerator();
                        var holder = new MLDataFieldHolder(
                            iterator, dataSetField);
                        uniqueSets[dataSet] = holder;
                        _readDataSet.Add(iterator);
                    }

                    MLDataFieldHolder holder2 = uniqueSets[dataSet];

                    _dataSetFieldMap[dataSetField] = holder2;
                    _dataSetIteratorMap[holder2.GetEnumerator()] = holder2;
                }
            }
        }

        /// <summary>
        /// Call this method to begin the normalization process.  Any status 
        /// updates will be sent to the class specified in the constructor.
        /// </summary>
        public void Process()
        {
            Init();
            if (TwoPassesNeeded())
            {
                FirstPass();
            }
            SecondPass();
        }
        /// <summary>
        /// Call this method to begin the normalization process.  Any status 
        /// updates will be sent to the class specified in the constructor.
        /// this version uses headers.
        /// </summary>
        public void Process(bool headers)
        {
            Init();
            if (TwoPassesNeeded())
            {
                FirstPass(headers);
            }
            SecondPass(headers);
        }
        /// <summary>
        /// Report on the current progress.
        /// </summary>
        /// <param name="message">The message to report.</param>
        /// <param name="total">The total number of records to process, 0 for unknown.</param>
        /// <param name="current"> The current record.</param>
        private void ReportResult(String message, int total,
                                  int current)
        {
            // count the records, report status
            _lastReport++;
            if (_lastReport >= 10000)
            {
                _report.Report(total, current, message);
                _lastReport = 0;
            }
        }

        /// <summary>
        /// The second pass actually writes the data to the output files.
        /// </summary>
        private void SecondPass()
        {
            bool twopass = TwoPassesNeeded();

            // move any CSV and datasets files back to the beginning.
            OpenCSV();
            OpenDataSet();
            InitForPass();

            _currentIndex = -1;

            // process the records
            int size = GetOutputFieldCount();
            var output = new double[size];

            _storage.Open();
            _lastReport = 0;
            int index = 0;
            int current = 0;
            while (Next())
            {
                // read the value
                foreach (IInputField field in _inputFields)
                {
                    DetermineInputFieldValue(field, index);
                }

                if (ShouldInclude())
                {
                    // handle groups
                    InitForOutput();

                    // write the value
                    int outputIndex = 0;
                    foreach (IOutputField ofield in _outputFields)
                    {
                        for (int sub = 0; sub < ofield.SubfieldCount; sub++)
                        {
                            output[outputIndex++] = ofield.Calculate(sub);
                        }
                    }

                    ReportResult(twopass ? "Second pass, normalizing data" : "Processing data (single pass)",
                                 _recordCount, ++current);
                    _storage.Write(output, 0);
                }

                index++;
            }
            _storage.Close();
        }




        /// <summary>
        /// The second pass actually writes the data to the output files.
        /// </summary>
        private void SecondPass(bool headers)
        {
            bool twopass = TwoPassesNeeded();

            // move any CSV and datasets files back to the beginning.
            OpenCSV(headers);
            OpenDataSet();
            InitForPass();

            _currentIndex = -1;

            // process the records
            int size = GetOutputFieldCount();
            var output = new double[size];

            _storage.Open();
            _lastReport = 0;
            int index = 0;
            int current = 0;
            while (Next())
            {
                // read the value
                foreach (IInputField field in _inputFields)
                {
                    DetermineInputFieldValue(field, index, headers);
                }

                if (ShouldInclude())
                {
                    // handle groups
                    InitForOutput();

                    // write the value
                    int outputIndex = 0;
                    foreach (IOutputField ofield in _outputFields)
                    {
                        for (int sub = 0; sub < ofield.SubfieldCount; sub++)
                        {
                            output[outputIndex++] = ofield.Calculate(sub);
                        }
                    }

                    ReportResult(twopass ? "Second pass, normalizing data" : "Processing data (single pass)",
                                 _recordCount, ++current);
                    _storage.Write(output, 0);
                }

                index++;
            }
            _storage.Close();
        }


        /// <summary>
        /// Should this row be included? Check the segregatprs.
        /// </summary>
        /// <returns>True if the row should be included.</returns>
        private bool ShouldInclude()
        {
            return _segregators.All(segregator => segregator.ShouldInclude());
        }


        /// <summary>
        /// Setup the row for output.
        /// </summary>
        public void InitForPass()
        {
            // init segregators
            foreach (ISegregator segregator in _segregators)
            {
                segregator.PassInit();
            }
        }

        /// <summary>
        /// Determine if two passes will be needed.
        /// </summary>
        /// <returns>True if two passes will be needed.</returns>
        public bool TwoPassesNeeded()
        {
            return _outputFields.OfType<IRequireTwoPass>().Any();
        }

        private void Init()
        {
            _csvMap = new Dictionary<IInputField, ReadCSV>();
            _dataSetFieldMap = new Dictionary<IInputField, MLDataFieldHolder>();
            _dataSetIteratorMap = new Dictionary<IEnumerator<IMLDataPair>, MLDataFieldHolder>();
            _readCSV = new List<ReadCSV>();
            _readDataSet = new List<IEnumerator<IMLDataPair>>();


        }
    }

    public class NormalizationError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public NormalizationError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public NormalizationError(Exception e)
            : base(e)
        {
        }
    }

    public class SyntUtility
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private SyntUtility()
        {
        }

        /// <summary>
        /// Convert a CSV file to a binary training file.
        /// </summary>
        /// <param name="csvFile">The CSV file.</param>
        /// <param name="format">The format.</param>
        /// <param name="binFile">The binary file.</param>
        /// <param name="inputCount">The number of input values.</param>
        /// <param name="outputCount">The number of output values.</param>
        /// <param name="headers">True, if there are headers on the3 CSV.</param>
        /// <param name="expectSignificance">Should a significance column be expected.</param>
        public static void ConvertCSV2Binary(String csvFile, CSVFormat format,
                                             String binFile, int inputCount, int outputCount,
                                             bool headers, bool expectSignificance)
        {
            new FileInfo(binFile).Delete();

            var csv = new CSVMLDataSet(csvFile,
                                       inputCount, outputCount, false, format, expectSignificance);
            var buffer = new BufferedMLDataSet(binFile);
            buffer.BeginLoad(inputCount, outputCount);
            foreach (IMLDataPair pair in csv)
            {
                buffer.Add(pair);
            }
            buffer.EndLoad();
        }

        /// <summary>
        /// Convert a CSV file to binary.
        /// </summary>
        /// <param name="csvFile">The CSV file to convert.</param>
        /// <param name="format">The format.</param>
        /// <param name="binFile">The binary file.</param>
        /// <param name="input">The input.</param>
        /// <param name="ideal">The ideal.</param>
        /// <param name="headers">True, if headers are present.</param>
        public static void ConvertCSV2Binary(FileInfo csvFile, CSVFormat format,
                                             FileInfo binFile, int[] input, int[] ideal, bool headers)
        {
            binFile.Delete();
            var csv = new ReadCSV(csvFile.ToString(), headers, format);

            var buffer = new BufferedMLDataSet(binFile.ToString());
            buffer.BeginLoad(input.Length, ideal.Length);
            while (csv.Next())
            {
                var inputData = new BasicMLData(input.Length);
                var idealData = new BasicMLData(ideal.Length);

                // handle input data
                for (int i = 0; i < input.Length; i++)
                {
                    inputData[i] = csv.GetDouble(input[i]);
                }

                // handle input data
                for (int i = 0; i < ideal.Length; i++)
                {
                    idealData[i] = csv.GetDouble(ideal[i]);
                }

                // add to dataset

                buffer.Add(inputData, idealData);
            }
            buffer.EndLoad();
            buffer.Close();
            csv.Close();
        }

        /// <summary>
        /// Load CSV to memory.
        /// </summary>
        /// <param name="filename">The CSV file to load.</param>
        /// <param name="input">The input count.</param>
        /// <param name="ideal">The ideal count.</param>
        /// <param name="headers">True, if headers are present.</param>
        /// <param name="format">The loaded dataset.</param>
        /// <param name="expectSignificance">The loaded dataset.</param>
        /// <returns></returns>
        public static IMLDataSet LoadCSV2Memory(String filename, int input, int ideal, bool headers, CSVFormat format, bool expectSignificance)
        {
            IDataSetCODEC codec = new CSVDataCODEC(filename, format, headers, input, ideal, expectSignificance);
            var load = new MemoryDataLoader(codec);
            IMLDataSet dataset = load.External2Memory();
            return dataset;
        }

        /// <summary>
        /// Evaluate the network and display (to the console) the output for every
        /// value in the training set. Displays ideal and actual.
        /// </summary>
        /// <param name="network">The network to evaluate.</param>
        /// <param name="training">The training set to evaluate.</param>
        public static void Evaluate(IMLRegression network,
                                    IMLDataSet training)
        {
            foreach (IMLDataPair pair in training)
            {
                IMLData output = network.Compute(pair.Input);
                Console.WriteLine(@"Input="
                                  + FormatNeuralData(pair.Input)
                                  + @", Actual=" + FormatNeuralData(output)
                                  + @", Ideal="
                                  + FormatNeuralData(pair.Ideal));
            }
        }

        /// <summary>
        /// Format neural data as a list of numbers.
        /// </summary>
        /// <param name="data">The neural data to format.</param>
        /// <returns>The formatted neural data.</returns>
        public static String FormatNeuralData(IMLData data)
        {
            var result = new StringBuilder();
            for (int i = 0; i < data.Count; i++)
            {
                if (i != 0)
                {
                    result.Append(',');
                }
                result.Append(Format.FormatDouble(data[i], 4));
            }
            return result.ToString();
        }

        /// <summary>
        /// Create a simple feedforward neural network.
        /// </summary>
        /// <param name="input">The number of input neurons.</param>
        /// <param name="hidden1">The number of hidden layer 1 neurons.</param>
        /// <param name="hidden2">The number of hidden layer 2 neurons.</param>
        /// <param name="output">The number of output neurons.</param>
        /// <param name="tanh">True to use hyperbolic tangent activation function, false to
        /// use the sigmoid activation function.</param>
        /// <returns>The neural network.</returns>
        public static BasicNetwork SimpleFeedForward(int input,
                                                     int hidden1, int hidden2, int output,
                                                     bool tanh)
        {
            var pattern = new FeedForwardPattern { InputNeurons = input, OutputNeurons = output };
            if (tanh)
            {
                pattern.ActivationFunction = new ActivationTANH();
            }
            else
            {
                pattern.ActivationFunction = new ActivationSigmoid();
            }

            if (hidden1 > 0)
            {
                pattern.AddHiddenLayer(hidden1);
            }
            if (hidden2 > 0)
            {
                pattern.AddHiddenLayer(hidden2);
            }

            var network = (BasicNetwork)pattern.Generate();
            network.Reset();
            return network;
        }

        /// <summary>
        /// Train the neural network, using SCG training, and output status to the
        /// console.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set.</param>
        /// <param name="minutes">The number of minutes to train for.</param>
        public static void TrainConsole(BasicNetwork network,
                                        IMLDataSet trainingSet, int minutes)
        {
            Propagation train = new ResilientPropagation(network,
                                                         trainingSet)
            { ThreadCount = 0 };
            TrainConsole(train, network, trainingSet, minutes);
        }


        /// <summary>
        /// Train the neural network, using SCG training, and output status to the
        /// console.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set.</param>
        /// <param name="seconds">The seconds.</param>
        public static void TrainConsole(BasicNetwork network,
                                        IMLDataSet trainingSet, double seconds)
        {
            Propagation train = new ResilientPropagation(network,
                                                         trainingSet)
            { ThreadCount = 0 };
            TrainConsole(train, network, trainingSet, seconds);
        }

        /// <summary>
        /// Train the network, using the specified training algorithm, and send the
        /// output to the console.
        /// </summary>
        /// <param name="train">The training method to use.</param>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set.</param>
        /// <param name="minutes">The number of minutes to train for.</param>
        public static void TrainConsole(IMLTrain train,
                                        BasicNetwork network, IMLDataSet trainingSet,
                                        int minutes)
        {
            int epoch = 1;
            long remaining;

            Console.WriteLine(@"Beginning training...");
            long start = Environment.TickCount;
            do
            {
                train.Iteration();

                long current = Environment.TickCount;
                long elapsed = (current - start) / 1000;
                remaining = minutes - elapsed / 60;

                Console.WriteLine(@"Iteration #" + Format.FormatInteger(epoch)
                                  + @" Error:" + Format.FormatPercent(train.Error)
                                  + @" elapsed time = " + Format.FormatTimeSpan((int)elapsed)
                                  + @" time left = "
                                  + Format.FormatTimeSpan((int)remaining * 60));
                epoch++;
            } while (remaining > 0 && !train.TrainingDone);
            train.FinishTraining();
        }


        /// <summary>
        /// Train the network, using the specified training algorithm, and send the
        /// output to the console.
        /// </summary>
        /// <param name="train">The training method to use.</param>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set.</param>
        /// <param name="seconds">The second to train for.</param>
        public static void TrainConsole(IMLTrain train, BasicNetwork network, IMLDataSet trainingSet, double seconds)
        {
            int epoch = 1;
            double remaining;

            Console.WriteLine(@"Beginning training...");
            long start = Environment.TickCount;
            do
            {
                train.Iteration();

                double current = Environment.TickCount;
                double elapsed = (current - start) / 1000;
                remaining = seconds - elapsed;

                Console.WriteLine(@"Iteration #" + Format.FormatInteger(epoch)
                                  + @" Error:" + Format.FormatPercent(train.Error)
                                  + @" elapsed time = " + Format.FormatTimeSpan((int)elapsed)
                                  + @" time left = "
                                  + Format.FormatTimeSpan((int)remaining));
                epoch++;
            } while (remaining > 0 && !train.TrainingDone);
            train.FinishTraining();
        }

        /// <summary>
        /// Train using RPROP and display progress to a dialog box.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set to use.</param>
        public static void TrainDialog(BasicNetwork network,
                                       IMLDataSet trainingSet)
        {
            Propagation train = new ResilientPropagation(network,
                                                         trainingSet)
            { ThreadCount = 0 };
            TrainDialog(train, network, trainingSet);
        }

        /// <summary>
        /// Train, using the specified training method, display progress to a dialog
        /// box.
        /// </summary>
        /// <param name="train">The training method to use.</param>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set to use.</param>
        public static void TrainDialog(IMLTrain train,
                                       BasicNetwork network, IMLDataSet trainingSet)
        {
         //   var dialog = new TrainingDialog { Train = train };
          //  dialog.ShowDialog();
        }

        /// <summary>
        /// Train the network, to a specific error, send the output to the console.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="trainingSet">The training set to use.</param>
        /// <param name="error">The error level to train to.</param>
        public static void TrainToError(BasicNetwork network,
                                        IMLDataSet trainingSet, double error)
        {
            Propagation train = new ResilientPropagation(network,
                                                         trainingSet)
            { ThreadCount = 0 };
            TrainToError(train, trainingSet, error);
        }

        /// <summary>
        /// Train to a specific error, using the specified training method, send the
        /// output to the console.
        /// </summary>
        /// <param name="train">The training method.</param>
        /// <param name="trainingSet">The training set to use.</param>
        /// <param name="error">The desired error level.</param>
        public static void TrainToError(IMLTrain train,
                                        IMLDataSet trainingSet,
                                        double error)
        {
            int epoch = 1;

            Console.WriteLine(@"Beginning training...");

            do
            {
                train.Iteration();

                Console.WriteLine(@"Iteration #" + Format.FormatInteger(epoch)
                                  + @" Error:" + Format.FormatPercent(train.Error)
                                  + @" Target Error: " + Format.FormatPercent(error));
                epoch++;
            } while (train.Error > error && !train.TrainingDone);
            train.FinishTraining();
        }

        /// <summary>
        /// Calculate a regression error.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <param name="data">The data to check.</param>
        /// <returns>The error.</returns>
        public static double CalculateRegressionError(IMLRegression method,
                                                      IMLDataSet data)
        {
            var errorCalculation = new ErrorCalculation();
            if (method is IMLContext)
                ((IMLContext)method).ClearContext();


            foreach (IMLDataPair pair in data)
            {
                IMLData actual = method.Compute(pair.Input);
                errorCalculation.UpdateError(actual.Data, pair.Ideal.Data, pair.Significance);
            }
            return errorCalculation.Calculate();
        }

        /// <summary>
        /// Save the dataset to a CSV file.
        /// </summary>
        /// <param name="targetFile">The target file.</param>
        /// <param name="format">The format to use.</param>
        /// <param name="set">The data set.</param>
        public static void SaveCSV(FileInfo targetFile, CSVFormat format, IMLDataSet set)
        {

        }

        /// <summary>
        /// Calculate an error for a method that makes use of classification.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <param name="data">The data to check.</param>
        /// <returns>The error.</returns>
        public static double CalculateClassificationError(IMLClassification method,
                                                          IMLDataSet data)
        {
            int total = 0;
            int correct = 0;


            foreach (IMLDataPair pair in data)
            {
                var ideal = (int)pair.Ideal[0];
                int actual = method.Classify(pair.Input);
                if (actual == ideal)
                    correct++;
                total++;
            }
            return (total - correct) / (double)total;
        }

        /// <summary>
        /// Load an EGB file to memory.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <returns>A memory data set.</returns>
        public static IMLDataSet LoadEGB2Memory(FileInfo filename)
        {
            var buffer = new BufferedMLDataSet(filename.ToString());
            var result = buffer.LoadToMemory();
            buffer.Close();
            return result;
        }

        /// <summary>
        /// Train to a specific error, using the specified training method, send the
        /// output to the console.
        /// </summary>
        ///
        /// <param name="train">The training method.</param>
        /// <param name="error">The desired error level.</param>
        public static void TrainToError(IMLTrain train, double error)
        {

            int epoch = 1;

            Console.Out.WriteLine(@"Beginning training...");

            do
            {
                train.Iteration();

                Console.Out.WriteLine(@"Iteration #" + Format.FormatInteger(epoch)
                        + @" Error:" + Format.FormatPercent(train.Error)
                        + @" Target Error: " + Format.FormatPercent(error));
                epoch++;
            } while ((train.Error > error) && !train.TrainingDone);
            train.FinishTraining();
        }

        /// <summary>
        /// Save the training set to an EGB file.
        /// </summary>
        /// <param name="egbFile">The EGB file to save to.</param>
        /// <param name="data">The training data to save.</param>
        public static void SaveEGB(FileInfo egbFile, IMLDataSet data)
        {
            var binary = new BufferedMLDataSet(egbFile.ToString());
            binary.Load(data);
            data.Close();
        }
    }
    
    public class TrainingSetUtil
    {
        /// <summary>
        /// Load a CSV file into a memory dataset.  
        /// </summary>
        ///
        /// <param name="format">The CSV format to use.</param>
        /// <param name="filename">The filename to load.</param>
        /// <param name="headers">True if there is a header line.</param>
        /// <param name="inputSize">The input size.  Input always comes first in a file.</param>
        /// <param name="idealSize">The ideal size, 0 for unsupervised.</param>
        /// <returns>A NeuralDataSet that holds the contents of the CSV file.</returns>
        public static IMLDataSet LoadCSVTOMemory(CSVFormat format, String filename,
                                                bool headers, int inputSize, int idealSize)
        {
            IMLDataSet result = new BasicMLDataSet();
            var csv = new ReadCSV(filename, headers, format);
            while (csv.Next())
            {
                IMLData ideal = null;
                int index = 0;

                IMLData input = new BasicMLData(inputSize);
                for (int i = 0; i < inputSize; i++)
                {
                    double d = csv.GetDouble(index++);
                    input[i] = d;
                }

                if (idealSize > 0)
                {
                    ideal = new BasicMLData(idealSize);
                    for (int i = 0; i < idealSize; i++)
                    {
                        double d = csv.GetDouble(index++);
                        ideal[i] = d;
                    }
                }

                IMLDataPair pair = new BasicMLDataPair(input, ideal);
                result.Add(pair);
            }

            return result;
        }

        /// <summary>
        /// Convert a training set to an array.
        /// </summary>
        /// <param name="training"></param>
        /// <returns></returns>
        public static ObjectPair<double[][], double[][]> TrainingToArray(
            IMLDataSet training)
        {
            var length = (int)training.Count;
            double[][] a = EngineArray.AllocateDouble2D(length, training.InputSize);
            double[][] b = EngineArray.AllocateDouble2D(length, training.IdealSize);

            int index = 0;

            foreach (IMLDataPair pair in training)
            {
                EngineArray.ArrayCopy(pair.InputArray, a[index]);
                EngineArray.ArrayCopy(pair.IdealArray, b[index]);
                index++;
            }

            return new ObjectPair<double[][], double[][]>(a, b);
        }
    }

    internal class DateUtil
    {
        /// <summary>
        /// January is 1.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime CreateDate(int month, int day, int year)
        {
            var result = new DateTime(year, month, day);
            return result;
        }

        /// <summary>
        /// Truncate a date, remove the time.
        /// </summary>
        /// <param name="date">The date to truncate.</param>
        /// <returns>The date without the time.</returns>
        public static DateTime TruncateDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }
    }

    internal class EnglishTimeUnitNames : ITimeUnitNames
    {
        #region ITimeUnitNames Members

        public String Code(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return "sec";
                case TimeUnit.Minutes:
                    return "min";
                case TimeUnit.Hours:
                    return "hr";
                case TimeUnit.Days:
                    return "d";
                case TimeUnit.Weeks:
                    return "w";
                case TimeUnit.Fortnights:
                    return "fn";
                case TimeUnit.Months:
                    return "m";
                case TimeUnit.Years:
                    return "y";
                case TimeUnit.Decades:
                    return "dec";
                case TimeUnit.Scores:
                    return "sc";
                case TimeUnit.Centuries:
                    return "c";
                case TimeUnit.Millennia:
                    return "m";
                default:
                    return "unk";
            }
        }

        public String Plural(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return "seconds";
                case TimeUnit.Minutes:
                    return "minutes";
                case TimeUnit.Hours:
                    return "hours";
                case TimeUnit.Days:
                    return "days";
                case TimeUnit.Weeks:
                    return "weeks";
                case TimeUnit.Fortnights:
                    return "fortnights";
                case TimeUnit.Months:
                    return "months";
                case TimeUnit.Years:
                    return "years";
                case TimeUnit.Decades:
                    return "decades";
                case TimeUnit.Scores:
                    return "scores";
                case TimeUnit.Centuries:
                    return "centures";
                case TimeUnit.Millennia:
                    return "millennia";
                default:
                    return "unknowns";
            }
        }

        public String Singular(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return "second";
                case TimeUnit.Minutes:
                    return "minute";
                case TimeUnit.Hours:
                    return "hour";
                case TimeUnit.Days:
                    return "day";
                case TimeUnit.Weeks:
                    return "week";
                case TimeUnit.Fortnights:
                    return "fortnight";
                case TimeUnit.Months:
                    return "month";
                case TimeUnit.Years:
                    return "year";
                case TimeUnit.Decades:
                    return "decade";
                case TimeUnit.Scores:
                    return "score";
                case TimeUnit.Centuries:
                    return "century";
                case TimeUnit.Millennia:
                    return "millenium";
                default:
                    return "unknown";
            }
        }

        #endregion
    }

    public static class NumericDateUtil
    {
        /// <summary>
        /// The numeric offset for a year.
        /// </summary>
        public const uint YearOffset = 10000;

        /// <summary>
        /// The numeric offset for a month.
        /// </summary>
        public const uint MonthOffset = 100;

        /// <summary>
        /// The numeric offset for an hour.
        /// </summary>
        public const uint HourOffset = 10000;

        /// <summary>
        /// The numeric offset for a minute.
        /// </summary>
        public const uint MinuteOffset = 100;

        /// <summary>
        /// Convert a date/time to a long.
        /// </summary>
        /// <param name="time">The time to convert.</param>
        /// <returns>A numeric date.</returns>
        public static ulong DateTime2Long(DateTime time)
        {
            return (ulong)(time.Day + (time.Month * MonthOffset) + (time.Year * YearOffset));
        }

        /// <summary>
        /// Convert a numeric date time to a regular date time.
        /// </summary>
        /// <param name="l">The numeric date time.</param>
        /// <returns>The converted date/time.</returns>
        public static DateTime Long2DateTime(ulong l)
        {
            var rest = (long)l;
            var year = (int)(rest / YearOffset);
            rest -= year * YearOffset;
            var month = (int)(rest / MonthOffset);
            rest -= month * MonthOffset;
            var day = (int)rest;
            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Strip the time element.
        /// </summary>
        /// <param name="dt">The time-date element to strip.</param>
        /// <returns>A new date-time with the time stripped.</returns>
        public static DateTime StripTime(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }

        /// <summary>
        /// Determine of two values have the same date.
        /// </summary>
        /// <param name="d1">The first date/time.</param>
        /// <param name="d2">The second date/time.</param>
        /// <returns>True, if they have the same date.</returns>
        public static bool HaveSameDate(DateTime d1, DateTime d2)
        {
            return ((d1.Day == d2.Day) && (d1.Month == d2.Month) && (d1.Year == d2.Year));
        }

        /// <summary>
        /// Convert an int to a time.
        /// </summary>
        /// <param name="date">The date-time that provides date information.</param>
        /// <param name="i">The int that holds the time.</param>
        /// <returns>The converted date/time.</returns>
        internal static DateTime Int2Time(DateTime date, uint i)
        {
            uint rest = i;
            var hour = (int)(rest / HourOffset);
            rest -= (uint)(hour * HourOffset);
            var minute = (int)(rest / MonthOffset);
            rest -= (uint)(minute * MinuteOffset);
            var second = (int)rest;
            return new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
        }

        /// <summary>
        /// Convert a time to an int.
        /// </summary>
        /// <param name="time">The time to convert.</param>
        /// <returns>The time as an int.</returns>
        internal static uint Time2Int(DateTime time)
        {
            return (uint)(time.Second + (time.Minute * MinuteOffset) + (time.Hour * HourOffset));
        }

        /// <summary>
        /// Get the year part of a numeric date.
        /// </summary>
        /// <param name="date">The numeric date.</param>
        /// <returns>The year.</returns>
        public static int GetYear(ulong date)
        {
            return (int)(date / YearOffset);
        }

        /// <summary>
        /// Get the year month of a numeric date.
        /// </summary>
        /// <param name="l">The numeric date.</param>
        /// <returns>The month.</returns>
        public static int GetMonth(ulong l)
        {
            var rest = (long)l;
            var year = (int)(rest / YearOffset);
            rest -= year * YearOffset;
            return (int)(rest / MonthOffset);
        }

        /// <summary>
        /// Get the minute period.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="period">The period size, in minutes.</param>
        /// <returns>The number of minutes per period.</returns>
        public static int GetMinutePeriod(uint time, int period)
        {
            uint rest = time;
            var hour = (int)(rest / HourOffset);
            rest -= (uint)(hour * HourOffset);
            var minute = (int)(rest / MonthOffset);

            int minutes = minute + (hour * 60);
            return minutes / period;
        }

        /// <summary>
        /// Combine a date and a time.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="time">The time.</param>
        /// <returns>The combined time.</returns>
        public static ulong Combine(ulong date, uint time)
        {
            return (date * 1000000) + time;
        }

        /// <summary>
        /// Get the day of the week for the specified numeric date.
        /// </summary>
        /// <param name="p">The time to check.</param>
        /// <returns>The day of the week, 0 is a sunday.</returns>
        public static int GetDayOfWeek(ulong p)
        {
            DateTime t = Long2DateTime(p);
            switch (t.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 0;
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                default:
                    // no way this should happen!
                    return -1;
            }
        }
    }

    internal class TimeSpanUtil
    {
        private DateTime _from;
        private DateTime _to;

        public TimeSpanUtil(DateTime from, DateTime to)
        {
            _from = from;
            _to = to;
        }

        public DateTime From
        {
            get { return _from; }
        }

        public DateTime To
        {
            get { return _to; }
        }


        public long GetSpan(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Ticks:
                    return GetSpanTicks();
                case TimeUnit.Seconds:
                    return GetSpanSeconds();
                case TimeUnit.Minutes:
                    return GetSpanMinutes();
                case TimeUnit.Hours:
                    return GetSpanHours();
                case TimeUnit.Days:
                    return GetSpanDays();
                case TimeUnit.Weeks:
                    return GetSpanWeeks();
                case TimeUnit.Fortnights:
                    return GetSpanFortnights();
                case TimeUnit.Months:
                    return GetSpanMonths();
                case TimeUnit.Years:
                    return GetSpanYears();
                case TimeUnit.Scores:
                    return GetSpanScores();
                case TimeUnit.Centuries:
                    return GetSpanCenturies();
                case TimeUnit.Millennia:
                    return GetSpanMillennia();
                default:
                    return 0;
            }
        }

        private long GetSpanTicks()
        {
            TimeSpan span = _to.Subtract(_from);
            return span.Ticks;
        }

        private long GetSpanSeconds()
        {
            TimeSpan span = _to.Subtract(_from);
            return span.Ticks / TimeSpan.TicksPerSecond;
        }

        private long GetSpanMinutes()
        {
            return GetSpanSeconds() / 60;
        }

        private long GetSpanHours()
        {
            return GetSpanMinutes() / 60;
        }

        private long GetSpanDays()
        {
            return GetSpanHours() / 24;
        }

        private long GetSpanWeeks()
        {
            return GetSpanDays() / 7;
        }

        private long GetSpanFortnights()
        {
            return GetSpanWeeks() / 2;
        }

        private long GetSpanMonths()
        {
            return (_to.Month - _from.Month) + (_to.Year - _from.Year) * 12;
        }

        private long GetSpanYears()
        {
            return GetSpanMonths() / 12;
        }

        private long GetSpanScores()
        {
            return GetSpanYears() / 20;
        }

        private long GetSpanCenturies()
        {
            return GetSpanYears() / 100;
        }

        private long GetSpanMillennia()
        {
            return GetSpanYears() / 1000;
        }
    }

    public class ValidateNetwork
    {
        /// <summary>
        /// Validate that the specified data can be used with the method.
        /// </summary>
        /// <param name="method">The method to validate.</param>
        /// <param name="training">The training data.</param>
        public static void ValidateMethodToData(IMLMethod method, IMLDataSet training)
        {
            if (!(method is IMLInput) || !(method is IMLOutput))
            {
                throw new SyntError(
                    "This machine learning method is not compatible with the provided data.");
            }

            int trainingInputCount = training.InputSize;
            int trainingOutputCount = training.IdealSize;
            int methodInputCount = 0;
            int methodOutputCount = 0;

            if (method is IMLInput)
            {
                methodInputCount = ((IMLInput)method).InputCount;
            }

            if (method is IMLOutput)
            {
                methodOutputCount = ((IMLOutput)method).OutputCount;
            }

            if (methodInputCount != trainingInputCount)
            {
                throw new SyntError(
                    "The machine learning method has an input length of "
                    + methodInputCount + ", but the training data has "
                    + trainingInputCount + ". They must be the same.");
            }

            if (!(method is BasicPNN))
            {
                if (trainingOutputCount > 0 && methodOutputCount != trainingOutputCount)
                {
                    throw new SyntError(
                        "The machine learning method has an output length of "
                        + methodOutputCount
                        + ", but the training data has "
                        + trainingOutputCount + ". They must be the same.");
                }
            }
        }
    }

    public static class DirectoryUtil
    {
        /// <summary>
        /// Default buffer size for read/write operations.
        /// </summary>
        public const int BufferSize = 1024;

        /// <summary>
        /// Copy the specified file.
        /// </summary>
        /// <param name="source">The file to copy.</param>
        /// <param name="target">The target of the copy.</param>
        public static void CopyFile(String source, String target)
        {
            try
            {
                var buffer = new byte[BufferSize];

                // open the files before the copy
                Stream inFile = new FileStream(source, FileMode.Open);
                Stream outFile = new FileStream(target, FileMode.OpenOrCreate);

                // perform the copy
                int packetSize = 0;

                while (packetSize != -1)
                {
                    packetSize = inFile.Read(buffer, 0, buffer.Length);
                    if (packetSize != -1)
                    {
                        outFile.Write(buffer, 0, packetSize);
                    }
                }

                // close the files after the copy
                inFile.Close();
                outFile.Close();
            }
            catch (IOException e)
            {
                throw new SyntError(e);
            }
        }


        /// <summary>
        /// Read the entire contents of a stream into a string.
        /// </summary>
        /// <param name="istream">The input stream to read from.</param>
        /// <returns>The string that was read in.</returns>
        public static String ReadStream(Stream istream)
        {
            try
            {
                var sb = new StringBuilder(1024);

                var chars = new byte[BufferSize];
                while ((istream.Read(chars, 0, chars.Length)) > -1)
                {
                    string s = Encoding.ASCII.GetString(chars);
                    sb.Append(s);
                }

                return sb.ToString();
            }
            catch (IOException e)
            {
#if logging
                LOGGER.Error("Exception", e);
#endif
                throw new SyntError(e);
            }
        }

        /// <summary>
        /// Read the entire contents of a stream into a string.
        /// </summary>
        /// <param name="filename">The input stream to read from.</param>
        /// <returns>The string that was read in.</returns>
        public static String ReadTextFile(String filename)
        {
            Stream stream = new FileStream(filename, FileMode.Open);
            String result = ReadStream(stream);
            stream.Close();
            return result;
        }
    }

    public sealed class SyntValidate
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        ///
        private SyntValidate()
        {
        }

        /// <summary>
        /// Validate a network for training.
        /// </summary>
        ///
        /// <param name="network">The network to validate.</param>
        /// <param name="training">The training set to validate.</param>
        public static void ValidateNetworkForTraining(IContainsFlat network,
                                                      IMLDataSet training)
        {
            int inputCount = network.Flat.InputCount;
            int outputCount = network.Flat.OutputCount;

            if (inputCount != training.InputSize)
            {
                throw new NeuralNetworkError("The input layer size of "
                                             + inputCount + " must match the training input size of "
                                             + training.InputSize + ".");
            }

            if ((training.IdealSize > 0)
                && (outputCount != training.IdealSize))
            {
                throw new NeuralNetworkError("The output layer size of "
                                             + outputCount + " must match the training input size of "
                                             + training.IdealSize + ".");
            }
        }
    }

    public class EngineArray
    {
        public const int DoubleSize = sizeof(double);

        /// <summary>
        /// Copy a double array.
        /// </summary>
        /// <param name="input">The array to copy.</param>
        /// <returns>The result of the copy.</returns>
        public static double[] ArrayCopy(double[] input)
        {
            var result = new double[input.Length];
            ArrayCopy(input, result);
            return result;
        }

        /// <summary>
        /// Copy an int array.
        /// </summary>
        /// <param name="input">The array to copy.</param>
        /// <returns>The result of the copy.</returns>
        public static int[] ArrayCopy(int[] input)
        {
            var result = new int[input.Length];
            ArrayCopy(input, result);
            return result;
        }


        /// <summary>
        /// Completely copy one array into another. 
        /// </summary>
        /// <param name="src">Source array.</param>
        /// <param name="dst">Destination array.</param>
        public static void ArrayCopy(double[] src, double[] dst)
        {
            Array.Copy(src, dst, src.Length);
        }

        /// <summary>
        /// Completely copy one array into another. 
        /// </summary>
        /// <param name="src">Source array.</param>
        /// <param name="dst">Destination array.</param>
        public static void ArrayCopy(int[] src, int[] dst)
        {
            Array.Copy(src, dst, src.Length);
        }

        /// <summary>
        /// Calculate the product of two vectors (a scalar value).
        /// </summary>
        /// <param name="a">First vector to multiply.</param>
        /// <param name="b">Second vector to multiply.</param>
        /// <returns>The product of the two vectors (a scalar value).</returns>
        public static double VectorProduct(double[] a, double[] b)
        {
            int length = a.Length;
            double value = 0;

            for (int i = 0; i < length; ++i)
                value += a[i] * b[i];

            return value;
        }

        /// <summary>
        /// Allocate a 2D array of doubles.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="cols">The number of columns.</param>
        /// <returns>The array.</returns>
        public static double[][] AllocateDouble2D(int rows, int cols)
        {
            var result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
            }
            return result;
        }

        /// <summary>
        /// Allocate a 2D array of bools.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="cols">The number of columns.</param>
        /// <returns>The array.</returns>
        public static bool[][] AllocateBool2D(int rows, int cols)
        {
            var result = new bool[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new bool[cols];
            }
            return result;
        }

        /// <summary>
        /// Copy an array of doubles.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="sourceIndex">The source index.</param>
        /// <param name="output">The output array.</param>
        /// <param name="targetIndex">The output index.</param>
        /// <param name="size">The size to copy.</param>
        public static void ArrayCopy(double[] source, int sourceIndex, double[] output, int targetIndex, int size)
        {
            //Array.Copy(source, sourceIndex, output, targetIndex, size);
            Buffer.BlockCopy(source, sourceIndex * DoubleSize, output, targetIndex * DoubleSize, size * DoubleSize);
        }

        /// <summary>
        /// Convert the collection to an array list of doubles.
        /// </summary>
        /// <param name="list">The list to convert.</param>
        /// <returns>The array of doubles.</returns>
        public static double[] ListToDouble(IList<double> list)
        {
            var result = new double[list.Count];
            int index = 0;
            foreach (double obj in list)
            {
                result[index++] = obj;
            }

            return result;
        }

        /// <summary>
        /// Fill the specified array with the specified value.
        /// </summary>
        /// <param name="p">The array to fill.</param>
        /// <param name="v">The value to fill.</param>
        internal static void Fill(double[] p, double v)
        {
            for (int i = 0; i < p.Length; i++)
                p[i] = v;
        }

        /// <summary>
        /// Search for a string in an array. 
        /// </summary>
        /// <param name="search">Where to search.</param>
        /// <param name="searchFor">What we are looking for.</param>
        /// <returns>The index that the string occurs at.</returns>
        public static int FindStringInArray(String[] search, String searchFor)
        {
            for (int i = 0; i < search.Length; i++)
            {
                if (search[i].Equals(searchFor))
                    return i;
            }
            return -1;
        }


        /// <summary>
        /// Gets the last N closing values of a double serie;
        /// copied in a new double serie.
        /// </summary>
        /// <param name="lenth">The lenth to get.</param>
        /// <param name="closes"></param>
        /// <returns>a double serie with the last n requested values.</returns>
        public double[] TransferNvaluesOfSerie(int lenth, double[] closes)
        {
            if (closes != null)
            {
                double[] output;

                if (closes.Length > lenth)
                {
                    //we have more closing values than our length so we'll return values based on last - Length.
                    int startIndex = closes.Length - lenth;
                    output = new double[lenth];
                    EngineArray.ArrayCopy(closes, startIndex, output, 0, lenth);
                    return output;
                }
                if (closes.Length == lenth)
                {
                    //we have the same values , so we just return the full closing values.
                    int startIndex = closes.Length - lenth;
                    output = new double[lenth];
                    EngineArray.ArrayCopy(closes, startIndex, output, 0, lenth);
                    return output;
                }
            }

            //we didn't get any right values to return N lenght of the serie.
            return null;

        }

        /// <summary>
        /// Copy a 2d array.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The result.</returns>
        public static double[][] ArrayCopy(double[][] source)
        {
            double[][] result = AllocateDouble2D(source.Length, source[0].Length);

            for (int row = 0; row < source.Length; row++)
            {
                for (int col = 0; col < source[0].Length; col++)
                {
                    result[row][col] = source[row][col];
                }
            }

            return result;
        }

        /// <summary>
        /// Copy a float array to a double array.
        /// </summary>
        /// <param name="source">The double array.</param>
        /// <param name="target">The float array.</param>
        public static void ArrayCopy(float[] source, double[] target)
        {
            for (int i = 0; i < source.Length; i++)
            {
                target[i] = source[i];
            }
        }

        /// <summary>
        /// Copy a double array to a float array.
        /// </summary>
        /// <param name="source">The double array.</param>
        /// <param name="target">The float array.</param>
        public static void ArrayCopy(double[] source, float[] target)
        {
            for (int i = 0; i < source.Length; i++)
            {
                target[i] = (float)source[i];
            }
        }

        /// <summary>
        /// Fill the array with the specified value.
        /// </summary>
        /// <param name="target">The array to fill.</param>
        /// <param name="v">The value to fill.</param>
        public static void Fill(double[] target, int v)
        {
            for (int i = 0; i < target.Length; i++)
                target[i] = v;
        }

        /// <summary>
        /// Fill the array with the specified value.
        /// </summary>
        /// <param name="target">The array to fill.</param>
        /// <param name="v">The value to fill.</param>
        public static void Fill(float[] target, int v)
        {
            for (int i = 0; i < target.Length; i++)
                target[i] = v;
        }

        /// <summary>
        /// Get the index of the largest value in the array.
        /// </summary>
        /// <param name="data">The array to search.</param>
        /// <returns>The index.</returns>
        public static int IndexOfLargest(double[] data)
        {
            int result = -1;

            for (int i = 0; i < data.Length; i++)
            {
                if (result == -1 || data[i] > data[result])
                    result = i;
            }

            return result;
        }
        /// <summary>
        /// Get the min value in an array.
        /// </summary>
        /// <param name="weights">The array to search.</param>
        /// <returns>The result.</returns>
        public static double Min(double[] weights)
        {
            double result = double.MaxValue;
            for (int i = 0; i < weights.Length; i++)
            {
                result = Math.Min(result, weights[i]);
            }
            return result;
        }

        /// <summary>
        /// Get the max value from an array.
        /// </summary>
        /// <param name="weights">The array to search.</param>
        /// <returns>The value.</returns>
        public static double Max(double[] weights)
        {
            double result = Double.MinValue;
            for (int i = 0; i < weights.Length; i++)
            {
                result = Math.Max(result, weights[i]);
            }
            return result;
        }

        /// <summary>
        /// Put all elements from one dictionary to another.
        /// </summary>
        /// <typeparam name="TK">The key type.</typeparam>
        /// <typeparam name="TV">The value type.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="target">The target dictionary.</param>
        public static void PutAll<TK, TV>(IDictionary<TK, TV> source, IDictionary<TK, TV> target)
        {
            foreach (var x in source)
            {
                target.Add(x);
            }
        }

        /// <summary>
        /// Determine if the array contains the specified number.
        /// </summary>
        /// <param name="array">The array to search.</param>
        /// <param name="target">The number being searched for.</param>
        /// <returns>True, if the number was found.</returns>
        public static bool Contains(int[] array, int target)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == target)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the index of the max value in the array.
        /// </summary>
        /// <param name="data">The array to search.</param>
        /// <returns>The result</returns>
        public static int MaxIndex(double[] data)
        {
            int result = -1;
            for (int i = 0; i < data.Length; i++)
            {
                if (result == -1 || data[i] > data[result])
                {
                    result = i;
                }
            }
            return result;
        }

        /// <summary>
        /// Get the index of the max value in the array.
        /// </summary>
        /// <param name="data">The array to search.</param>
        /// <returns>The result</returns>
        public static int MaxIndex(int[] data)
        {
            int result = -1;
            for (int i = 0; i < data.Length; i++)
            {
                if (result == -1 || data[i] > data[result])
                {
                    result = i;
                }
            }
            return result;
        }


        /// <summary>
        /// Creates a jagged array and initializes it.
        /// You can virtually create any kind of jagged array up to N dimension.
        /// double[][] resultingArray = CreateArray  <double[ ]> (2, () => CreateArray<double>(100, () => 0));
        /// Create a double[2] [100] , with all values at 0..
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnt">The CNT.</param>
        /// <param name="itemCreator">The item creator.</param>
        /// <returns></returns>
        public static T[] CreateArray<T>(int cnt, Func<T> itemCreator)
        {
            T[] result = new T[cnt];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = itemCreator();
            }
            return result;
        }

        /// <summary>
        /// Fill the array with the specified value.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="v">The value.</param>
        public static void Fill(bool[] array, bool v)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = v;
            }
        }

        /// <summary>
        /// Add two vectors.
        /// </summary>
        /// <param name="d">First vector.</param>
        /// <param name="m">Second vector.</param>
        /// <returns>Result vector.</returns>
        public static double[] Add(double[] d, double[] m)
        {
            double[] result = new double[d.Length];
            for (int i = 0; i < d.Length; i++)
            {
                result[i] = d[i] + m[i];
            }
            return result;
        }

        /// <summary>
        /// Subtract two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Result vector.</returns>
        public static double[] Subtract(double[] a, double[] b)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] - b[i];
            }
            return result;
        }

        internal static int[][] AllocateInt2D(int rows, int cols)
        {
            var result = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new int[cols];
            }
            return result;

        }

        public static double[][][] AllocDouble3D(int x, int y, int z)
        {
            var result = new double[x][][];
            for (int i = 0; i < x; i++)
            {
                result[i] = new double[y][];
                for (int j = 0; j < y; j++)
                {
                    result[i][j] = new double[z];
                }
            }
            return result;

        }

        /// <summary>
        /// Copy one double 2d array to another.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="target">The target array.</param>
        public static void ArrayCopy(double[][] source, double[][] target)
        {
            for (var row = 0; row < source.Length; row++)
            {
                for (var col = 0; col < source[row].Length; col++)
                {
                    target[row][col] = source[row][col];
                }
            }
        }

        /// <summary>
        /// Calculate the Euclidean distance between two vectors.
        /// </summary>
        /// <param name="p1">The first vector.</param>
        /// <param name="p2">The second vector.</param>
        /// <returns>The distance.</returns>
        public static double EuclideanDistance(double[] p1, double[] p2)
        {
            double sum = 0;
            for (int i = 0; i < p1.Length; i++)
            {
                double d = p1[i] - p2[i];
                sum += d * d;
            }
            return Math.Sqrt(sum);
        }
    }

    public class Format
    {
        /// <summary>
        /// One hundred percent.
        /// </summary>
        public const double HundredPercent = 100.0;

        /// <summary>
        /// Bytes in a KB.
        /// </summary>
        public const long MemoryK = 1024;

        /// <summary>
        /// Bytes in a MB.
        /// </summary>
        public const long MemoryMeg = (1024 * MemoryK);

        /// <summary>
        /// Bytes in a GB.
        /// </summary>
        public const long MemoryGig = (1024 * MemoryMeg);

        /// <summary>
        /// Bytes in a TB.
        /// </summary>
        public const long MemoryTera = (1024 * MemoryGig);

        /// <summary>
        /// Seconds in a minute.
        /// </summary>
        public const int SecondsInaMinute = 60;

        /// <summary>
        /// Seconds in an hour.
        /// </summary>
        public const int SecondsInaHour = SecondsInaMinute * 60;

        /// <summary>
        /// Seconds in a day.
        /// </summary>
        public const int SecondsInaDay = SecondsInaHour * 24;

        /// <summary>
        /// Miliseconds in a day.
        /// </summary>
        public const long MiliInSec = 1000;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private Format()
        {
        }

        /// <summary>
        /// Format a double.
        /// </summary>
        /// <param name="d">The double value to format.</param>
        /// <param name="i">The number of decimal places.</param>
        /// <returns>The double as a string.</returns>
        public static String FormatDouble(double d, int i)
        {
            if (Double.IsNaN(d) || Double.IsInfinity(d))
                return "NaN";
            return d.ToString("N" + i);
        }


        /// <summary>
        /// Format a memory amount, to something like 32 MB.
        /// </summary>
        /// <param name="memory">The amount of bytes.</param>
        /// <returns>The formatted memory size.</returns>
        public static String FormatMemory(long memory)
        {
            if (memory < MemoryK)
            {
                return memory + " bytes";
            }
            if (memory < MemoryMeg)
            {
                return FormatDouble((memory) / ((double)MemoryK), 2) + " KB";
            }
            if (memory < MemoryGig)
            {
                return FormatDouble((memory) / ((double)MemoryMeg), 2) + " MB";
            }
            if (memory < MemoryTera)
            {
                return FormatDouble((memory) / ((double)MemoryGig), 2) + " GB";
            }
            return FormatDouble((memory) / ((double)MemoryTera), 2) + " TB";
        }

        /// <summary>
        /// Format an integer.
        /// </summary>
        /// <param name="i">The integer.</param>
        /// <returns>The string.</returns>
        public static String FormatInteger(int i)
        {
            return String.Format("{0:n0}", i);
        }

        /// <summary>
        /// Format a percent.  Using 6 decimal places.
        /// </summary>
        /// <param name="e">The percent to format.</param>
        /// <returns>The formatted percent.</returns>
        public static String FormatPercent(double e)
        {
            if (Double.IsNaN(e) || Double.IsInfinity(e))
                return "NaN";
            return (e * 100.0).ToString("N6") + "%";
        }

        /// <summary>
        /// Format a percent with no decimal places.
        /// </summary>
        /// <param name="e">The format to percent.</param>
        /// <returns>The formatted percent.</returns>
        public static String FormatPercentWhole(double e)
        {
            if (Double.IsNaN(e) || Double.IsInfinity(e))
                return "NaN";
            return (e * 100.0).ToString("N0") + "%";
        }

        /// <summary>
        /// Format a time span as seconds, minutes, hours and days.
        /// </summary>
        /// <param name="seconds">The number of seconds in the timespan.</param>
        /// <returns>The formatted timespan.</returns>
        public static String FormatTimeSpan(int seconds)
        {
            int secondsCount = seconds;
            int days = seconds / SecondsInaDay;
            secondsCount -= days * SecondsInaDay;
            int hours = secondsCount / SecondsInaHour;
            secondsCount -= hours * SecondsInaHour;
            int minutes = secondsCount / SecondsInaMinute;
            secondsCount -= minutes * SecondsInaMinute;

            var result = new StringBuilder();

            if (days > 0)
            {
                result.Append(days);
                result.Append(days > 1 ? " days " : " day ");
            }

            result.Append(hours.ToString("00"));
            result.Append(':');
            result.Append(minutes.ToString("00"));
            result.Append(':');
            result.Append(secondsCount.ToString("00"));

            return result.ToString();
        }

        /// <summary>
        /// Format a boolean to yes/no.
        /// </summary>
        /// <param name="p">The default answer.</param>
        /// <returns>A string form of the boolean.</returns>
        public static string FormatYesNo(bool p)
        {
            return p ? "Yes" : "No";
        }
    }

    public class HTMLReport
    {
        /// <summary>
        /// Text.
        /// </summary>
        private readonly StringBuilder _text;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public HTMLReport()
        {
            _text = new StringBuilder();
        }

        /// <summary>
        /// Begin an HTML tag.
        /// </summary>
        public void BeginHTML()
        {
            _text.Append("<html>");
        }

        /// <summary>
        /// End an HTML tag.
        /// </summary>
        public void EndHTML()
        {
            _text.Append("</html>");
        }

        /// <summary>
        /// Set the title.
        /// </summary>
        /// <param name="str">The title.</param>
        public void Title(String str)
        {
            _text.Append("<head><title>");
            _text.Append(str);
            _text.Append("</title></head>");
        }

        /// <summary>
        /// Begin an HTML para.
        /// </summary>
        public void BeginPara()
        {
            _text.Append("<p>");
        }

        /// <summary>
        /// End an HTML para.
        /// </summary>
        public void EndPara()
        {
            _text.Append("</p>");
        }

        /// <summary>
        /// Display in bold.
        /// </summary>
        /// <param name="str"></param>
        public void Bold(String str)
        {
            _text.Append("<b>");
            _text.Append(Syntesis(str));
            _text.Append("</b>");
        }

        /// <summary>
        /// Display a para.
        /// </summary>
        /// <param name="str">The para to display.</param>
        public void Para(String str)
        {
            _text.Append("<p>");
            _text.Append(Syntesis(str));
            _text.Append("</p>");
        }

        /// <summary>
        /// Clear the report.
        /// </summary>
        public void Clear()
        {
            _text.Length = 0;
        }

        /// <summary>
        /// Convert the report to a string.
        /// </summary>
        /// <returns>The report text.</returns>
        public override String ToString()
        {
            return _text.ToString();
        }

        /// <summary>
        /// Begin the HTML body.
        /// </summary>
        public void BeginBody()
        {
            _text.Append("<body>");
        }

        /// <summary>
        /// End the HTML body.
        /// </summary>
        public void EndBody()
        {
            _text.Append("</body>");
        }

        /// <summary>
        /// Create a H1.
        /// </summary>
        /// <param name="title"></param>
        public void H1(String title)
        {
            _text.Append("<h1>");
            _text.Append(Syntesis(title));
            _text.Append("</h1>");
        }

        /// <summary>
        /// Begin a table.
        /// </summary>
        public void BeginTable()
        {
            _text.Append("<table border=\"1\">");
        }

        /// <summary>
        /// End a table.
        /// </summary>
        public void EndTable()
        {
            _text.Append("</table>");
        }

        /// <summary>
        /// Begin a row of a table.
        /// </summary>
        public void BeginRow()
        {
            _text.Append("<tr>");
        }

        /// <summary>
        /// End a row of a table.
        /// </summary>
        public void EndRow()
        {
            _text.Append("</tr>");
        }

        /// <summary>
        /// Add a header cell.
        /// </summary>
        /// <param name="head">The text to use.</param>
        public void Header(String head)
        {
            _text.Append("<th>");
            _text.Append(Syntesis(head));
            _text.Append("</th>");
        }

        /// <summary>
        /// Add a cell, no column span.
        /// </summary>
        /// <param name="head">The head of that call.</param>
        public void Cell(String head)
        {
            Cell(head, 0);
        }

        /// <summary>
        /// Add a cell to a table.
        /// </summary>
        /// <param name="head">The text for the cell.</param>
        /// <param name="colSpan">The col span.</param>
        public void Cell(String head, int colSpan)
        {
            _text.Append("<td");
            if (colSpan > 0)
            {
                _text.Append(" colspan=\"");
                _text.Append(colSpan);
                _text.Append("\"");
            }
            _text.Append(">");
            _text.Append(Syntesis(head));
            _text.Append("</td>");
        }

        /// <summary>
        /// Add a name-value pair to a table.  This includes a row.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="v">The value.</param>
        public void TablePair(String name, String v)
        {
            BeginRow();
            _text.Append("<td><b>" + Syntesis(name) + "</b></td>");
            Cell(v);
            EndRow();
        }

        /// <summary>
        /// Add a H2.
        /// </summary>
        /// <param name="title">The title.</param>
        public void H2(String title)
        {
            _text.Append("<h2>");
            _text.Append(Syntesis(title));
            _text.Append("</h2>");
        }

        /// <summary>
        /// Add a H3.
        /// </summary>
        /// <param name="title">The title.</param>
        public void H3(String title)
        {
            _text.Append("<h3>");
            _text.Append(Syntesis(title));
            _text.Append("</h3>");
        }

        /// <summary>
        /// Begin a list.
        /// </summary>
        public void BeginList()
        {
            _text.Append("<ul>");
        }

        /// <summary>
        /// Add a list item.
        /// </summary>
        /// <param name="str">The item added.</param>
        public void ListItem(String str)
        {
            _text.Append("<li>");
            _text.Append(Syntesis(str));
        }

        /// <summary>
        /// End a list.
        /// </summary>
        public void EndList()
        {
            _text.Append("</ul>");
        }

        /// <summary>
        /// Begin a new table in a cell.
        /// </summary>
        /// <param name="colSpan">The column span.</param>
        public void BeginTableInCell(int colSpan)
        {
            _text.Append("<td");
            if (colSpan > 0)
            {
                _text.Append(" colspan=\"");
                _text.Append(colSpan);
                _text.Append("\"");
            }
            _text.Append(">");
            _text.Append("<table border=\"1\" width=\"100%\">");
        }

        /// <summary>
        /// End a table in a cell.
        /// </summary>
        public void EndTableInCell()
        {
            _text.Append("</table></td>");
        }

        /// <summary>
        /// Syntesis a string for HTML.
        /// </summary>
        /// <param name="str">The string to Syntesis.</param>
        /// <returns>The Syntesisd string.</returns>
        public static String Syntesis(String str)
        {
            var result = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];

                if (ch == '<')
                {
                    result.Append("&lt;");
                }
                else if (ch == '>')
                {
                    result.Append("&gt;");
                }
                else if (ch == '&')
                {
                    result.Append("&amp;");
                }
                else
                {
                    result.Append(ch);
                }

            }
            return result.ToString();
        }
    }

    public class ObjectCloner
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private ObjectCloner()
        {
        }

        /// <summary>
        /// Perform a deep copy.
        /// </summary>
        /// <param name="oldObj">The old object.</param>
        /// <returns>The new object.</returns>
        public static Object DeepCopy(Object oldObj)
        {
            var formatter = new BinaryFormatter();

            using (var memory = new MemoryStream())
            {
                try
                {
                    // serialize and pass the object
                    formatter.Serialize(memory, oldObj);
                    memory.Flush();
                    memory.Position = 0;

                    // return the new object
                    return formatter.Deserialize(memory);
                }
                catch (Exception e)
                {
                    throw new SyntError(e);
                }
            }
        }
    }

    public class ObjectPair<TA, TB>
    {
        /// <summary>
        /// The first object.
        /// </summary>
        private readonly TA _a;

        /// <summary>
        /// The second object.
        /// </summary>
        private readonly TB _b;

        /// <summary>
        /// Construct an object pair. 
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        public ObjectPair(TA a, TB b)
        {
            _a = a;
            _b = b;
        }

        /// <summary>
        /// The first object.
        /// </summary>
        public TA A
        {
            get { return _a; }
        }

        /// <summary>
        /// The second object.
        /// </summary>
        public TB B
        {
            get { return _b; }
        }
    }

    public class ParamsHolder
    {
        /// <summary>
        /// The format that numbers will be in.
        /// </summary>
        ///
        private readonly CSVFormat _format;

        /// <summary>
        /// The params that are to be parsed.
        /// </summary>
        ///
        private readonly IDictionary<String, String> _paras;

        /// <summary>
        /// Construct the object. Allow the format to be specified.
        /// </summary>
        ///
        /// <param name="theParams">The params to be used.</param>
        /// <param name="theFormat">The format to be used.</param>
        public ParamsHolder(IDictionary<String, String> theParams, CSVFormat theFormat)
        {
            _paras = theParams;
            _format = theFormat;
        }

        /// <summary>
        /// Construct the object. Allow the format to be specified.
        /// </summary>
        ///
        /// <param name="theParams">The params to be used.</param>
        public ParamsHolder(IDictionary<String, String> theParams) : this(theParams, CSVFormat.EgFormat)
        {
        }


        /// <value>the params</value>
        public IDictionary<String, String> Params
        {
            get { return _paras; }
        }


        /// <summary>
        /// Get a param as a string.
        /// </summary>
        ///
        /// <param name="name">The name of the string.</param>
        /// <param name="required">True if this value is required.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        public String GetString(String name, bool required, String defaultValue)
        {
            if (_paras.ContainsKey(name))
            {
                return _paras[name];
            }
            if (required)
            {
                throw new SyntError("Missing property: " + name);
            }
            return defaultValue;
        }

        /// <summary>
        /// Get a param as a integer.
        /// </summary>
        ///
        /// <param name="name">The name of the integer.</param>
        /// <param name="required">True if this value is required.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        public int GetInt(String name, bool required, int defaultValue)
        {
            String str = GetString(name, required, null);

            if (str == null)
                return defaultValue;

            try
            {
                return Int32.Parse(str);
            }
            catch (FormatException)
            {
                throw new SyntError("Property " + name
                                     + " has an invalid value of " + str
                                     + ", should be valid integer.");
            }
        }

        /// <summary>
        /// Get a param as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the double.</param>
        /// <param name="required">True if this value is required.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        public double GetDouble(String name, bool required, double defaultValue)
        {
            String str = GetString(name, required, null);

            if (str == null)
                return defaultValue;

            try
            {
                return _format.Parse(str);
            }
            catch (FormatException)
            {
                throw new SyntError("Property " + name
                                     + " has an invalid value of " + str
                                     + ", should be valid floating point.");
            }
        }

        /// <summary>
        /// Get a param as a boolean.
        /// </summary>
        ///
        /// <param name="name">The name of the double.</param>
        /// <param name="required">True if this value is required.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        public bool GetBoolean(String name, bool required,
                               bool defaultValue)
        {
            String str = GetString(name, required, null);

            if (str == null)
                return defaultValue;

            if (!str.Equals("true", StringComparison.InvariantCultureIgnoreCase) &&
                !str.Equals("false", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SyntError("Property " + name
                                     + " has an invalid value of " + str
                                     + ", should be true/false.");
            }

            return str.Equals("true", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public class ReflectionUtil
    {
        /// <summary>
        /// Path to the activation functions.
        /// </summary>
        public const String AfPath = "Synt.Engine.Network.Activation.";

        /// <summary>
        /// Path to RBF's.
        /// </summary>
        public const String RBFPath = "Synt.MathUtil.RBF.";

        /// <summary>
        /// A map between short class names and the full path names.
        /// </summary>
        private static readonly IDictionary<String, String> ClassMap = new Dictionary<String, String>();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private ReflectionUtil()
        {
        }


        /// <summary>
        /// Find the specified field, look also in superclasses.
        /// </summary>
        /// <param name="c">The class to search.</param>
        /// <param name="name">The name of the field we are looking for.</param>
        /// <returns>The field.</returns>
        public static FieldInfo FindField(Type c, String name)
        {
            ICollection<FieldInfo> list = GetAllFields(c);
            return list.FirstOrDefault(field => field.Name.Equals(name));
        }

        /// <summary>
        /// Get all of the fields from the specified class as a collection.
        /// </summary>
        /// <param name="c">The class to access.</param>
        /// <returns>All of the fields from this class and subclasses.</returns>
        public static IList<FieldInfo> GetAllFields(Type c)
        {
            IList<FieldInfo> result = new List<FieldInfo>();
            GetAllFields(c, result);
            return result;
        }

        /// <summary>
        /// Get all of the fields for the specified class and recurse to check the base class.
        /// </summary>
        /// <param name="c">The class to scan.</param>
        /// <param name="result">A list of fields.</param>
        public static void GetAllFields(Type c, IList<FieldInfo> result)
        {
            foreach (
                FieldInfo field in c.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                result.Add(field);
            }

            if (c.BaseType != null)
                GetAllFields(c.BaseType, result);
        }

        /// <summary>
        /// Load the classmap file. This allows classes to be resolved using just the
        /// simple name.
        /// </summary>
        public static void LoadClassmap()
        {
            {
                Stream istream = ResourceLoader.CreateStream("Synt.Resources.classes.txt");
                var sr = new StreamReader(istream);

                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    int idx = line.LastIndexOf('.');
                    if (idx != -1)
                    {
                        String simpleName = line.Substring(idx + 1);
                        ClassMap[simpleName] = line;
                    }
                }
                sr.Close();
                istream.Close();
            }
        }

        /// <summary>
        /// Resolve an Synt class using its simple name.
        /// </summary>
        /// <param name="name">The simple name of the class.</param>
        /// <returns>The class requested.</returns>
        public static String ResolveSyntClass(String name)
        {
            if (ClassMap.Count == 0)
            {
                LoadClassmap();
            }

            return !ClassMap.ContainsKey(name) ? null : ClassMap[name];
        }


        /// <summary>
        /// Determine if the specified field has the specified attribute.
        /// </summary>
        /// <param name="field">The field to check.</param>
        /// <param name="t">See if the field has this attribute.</param>
        /// <returns>True if the field has the specified attribute.</returns>
        public static bool HasAttribute(FieldInfo field, Type t)
        {
            return field.GetCustomAttributes(true).Any(obj => obj.GetType() == t);
        }


        /// <summary>
        /// Determine if the specified type contains the specified attribute.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>True if the type contains the attribute.</returns>
        public static bool HasAttribute(Type t, Type attribute)
        {
            return t.GetCustomAttributes(true).Any(obj => obj.GetType() == attribute);
        }

        /// <summary>
        /// Resolve an enumeration.
        /// </summary>
        /// <param name="field">The field to resolve.</param>
        /// <param name="v">The value to get the enum for.</param>
        /// <returns>The enum that was resolved.</returns>
        public static Object ResolveEnum(FieldInfo field, FieldInfo v)
        {
            Type type = field.GetType();
            Object[] objs = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
            return objs.Cast<MemberInfo>().FirstOrDefault(obj => obj.Name.Equals(v));
        }

        /// <summary>
        /// Loop over all loaded assembles and try to create the class.
        /// </summary>
        /// <param name="name">The class to create.</param>
        /// <returns>The created class.</returns>
        public static Object LoadObject(String name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Object result = null;

            foreach (Assembly a in assemblies)
            {
                result = a.CreateInstance(name);
                if (result != null)
                    break;
            }

            return result;
        }
    }

    public class SerializeObject
    {
        /// <summary>
        /// Private constructor, call everything statically.
        /// </summary>
        private SerializeObject()
        {
        }

        /// <summary>
        /// Load the specified filename.
        /// </summary>
        /// <param name="filename">The filename to load from.</param>
        /// <returns>The object loaded from that file.</returns>
        public static object Load(string filename)
        {
            Stream s = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
            var b = new BinaryFormatter();
            object obj = b.Deserialize(s);
            s.Close();
            return obj;
        }

        /// <summary>
        /// Save the specified object.
        /// </summary>
        /// <param name="filename">The filename to save to.</param>
        /// <param name="obj">The object to save.</param>
        public static void Save(string filename, object obj)
        {
            Stream s = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            var b = new BinaryFormatter();
            b.Serialize(s, obj);
            s.Close();
        }
    }

    public class SimpleParser
    {
        /// <summary>
        /// The current position.
        /// </summary>
        private int _currentPosition;

        /// <summary>
        /// The marked position.
        /// </summary>
        private int _marked;

        /// <summary>
        /// Construct the object for the specified line.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        public SimpleParser(String line)
        {
            Line = line;
        }

        /// <summary>
        /// The line being parsed.
        /// </summary>
        public String Line { get; set; }

        /// <summary>
        /// The number of characters remaining.
        /// </summary>
        /// <returns>The number of characters remaining.</returns>
        public int Remaining()
        {
            return Math.Max(Line.Length - _currentPosition, 0);
        }

        /// <summary>
        /// Parse through a comma.
        /// </summary>
        /// <returns>True, if the comma was found.</returns>
        public bool ParseThroughComma()
        {
            EatWhiteSpace();
            if (!EOL())
            {
                if (Peek() == ',')
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// CHeck to see if the next character is an identifier.
        /// </summary>
        /// <returns>True, if the next char is an identifier.</returns>
        public bool IsIdentifier()
        {
            if (EOL())
                return false;

            return char.IsLetterOrDigit(Peek()) || Peek() == '_';
        }

        /// <summary>
        /// Peek ahead to see the next character.  But do not advance beyond it.
        /// </summary>
        /// <returns>The next character.</returns>
        public char Peek()
        {
            if (EOL())
                return (char)0;
            if (_currentPosition >= Line.Length)
                return (char)0;
            return Line[_currentPosition];
        }

        /// <summary>
        /// Advance beyond the next character.
        /// </summary>
        public void Advance()
        {
            if (_currentPosition < Line.Length)
            {
                _currentPosition++;
            }
        }

        /// <summary>
        /// Returns true if the next character is a white space.
        /// </summary>
        /// <returns>True, if the next character is a white space.</returns>
        public bool IsWhiteSpace()
        {
            return " \t\n\r".IndexOf(Peek()) != -1;
        }

        /// <summary>
        /// Returns true of there are no more characters to read.
        /// </summary>
        /// <returns>True, if we have reached end of line.</returns>
        public bool EOL()
        {
            return (_currentPosition >= Line.Length);
        }

        /// <summary>
        /// Strip any white space from the current position.
        /// </summary>
        public void EatWhiteSpace()
        {
            while (!EOL() && IsWhiteSpace())
                Advance();
        }

        /// <summary>
        /// Read the next character.
        /// </summary>
        /// <returns>The next character.</returns>
        public char ReadChar()
        {
            if (EOL())
                return (char)0;

            char ch = Peek();
            Advance();
            return ch;
        }

        /// <summary>
        /// Read text up to the next white space.
        /// </summary>
        /// <returns>The text read up to the next white space.</returns>
        public String ReadToWhiteSpace()
        {
            var result = new StringBuilder();

            while (!IsWhiteSpace() && !EOL())
            {
                result.Append(ReadChar());
            }

            return result.ToString();
        }

        /// <summary>
        /// Look ahead to see if the specified string is present.
        /// </summary>
        /// <param name="str">The string searching for.</param>
        /// <param name="ignoreCase">True if case is to be ignored.</param>
        /// <returns>True if the string is present.</returns>
        public bool LookAhead(String str, bool ignoreCase)
        {
            if (Remaining() < str.Length)
                return false;
            for (int i = 0; i < str.Length; i++)
            {
                char c1 = str[i];
                char c2 = Line[_currentPosition + i];

                if (ignoreCase)
                {
                    c1 = char.ToLower(c1);
                    c2 = char.ToLower(c2);
                }

                if (c1 != c2)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Advance the specified number of characters.
        /// </summary>
        /// <param name="p">The number of characters to advance.</param>
        public void Advance(int p)
        {
            _currentPosition = Math.Min(Line.Length, _currentPosition + p);
        }

        /// <summary>
        /// Mark the current position.
        /// </summary>
        public void Mark()
        {
            _marked = _currentPosition;
        }

        /// <summary>
        /// Reset back to the marked position.
        /// </summary>
        public void Reset()
        {
            _currentPosition = _marked;
        }

        /// <summary>
        /// Read a quoted string.
        /// </summary>
        /// <returns>The string that was read.</returns>
        public String ReadQuotedString()
        {
            if (Peek() != '\"')
                return "";

            var result = new StringBuilder();

            Advance();
            while (Peek() != '\"' && !EOL())
            {
                result.Append(ReadChar());
            }
            Advance();

            return result.ToString();
        }

        /// <summary>
        /// Read forward to the specified characters.
        /// </summary>
        /// <param name="chs">The characters to stop at.</param>
        /// <returns>The string that was read.</returns>
        public String ReadToChars(String chs)
        {
            StringBuilder result = new StringBuilder();

            while (chs.IndexOf(this.Peek()) == -1 && !EOL())
            {
                result.Append(ReadChar());
            }

            return result.ToString();
        }

    }

    public class StringUtil
    {
        /// <summary>
        /// Compare two strings, ignore case.
        /// </summary>
        /// <param name="a">The first string.</param>
        /// <param name="b">The second string.</param>
        /// <returns></returns>
        public static Boolean EqualsIgnoreCase(String a, String b)
        {
            return a.Equals(b, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Simple utility to take an array of ASCII bytes and convert to
        /// a String.  Works with Silverlight as well.
        /// </summary>
        /// <param name="b">The byte array.</param>
        /// <returns>The string created from the byte array.</returns>
        public static String FromBytes(byte[] b)
        {
            var b2 = new byte[b.Length * 2];
            for (int i = 0; i < b.Length; i++)
            {
                b2[i * 2] = b[i];
                b2[(i * 2) + 1] = 0;
            }

            return (new UnicodeEncoding()).GetString(b2, 0, b2.Length);
        }
    }

    public class YahooSearch
    {
        ///// <summary>
        ///// Perform a Yahoo search.
        ///// </summary>
        ///// <param name="url">The REST URL.</param>
        ///// <returns>The search results.</returns>
        //private ICollection<Uri> DoSearch(Uri url)
        //{
        //    ICollection<Uri> result = new List<Uri>();
        //    // submit the search
        //    //WebRequest http = WebRequest.Create(url);
        //    //var response = (HttpWebResponse)http.GetResponse();

        //    using (Stream istream = response.GetResponseStream())
        //    {
        //        var parse = new ReadHTML(istream);
        //        var buffer = new StringBuilder();
        //        bool capture = false;

        //        // parse the results
        //        int ch;
        //        while ((ch = parse.Read()) != -1)
        //        {
        //            if (ch == 0)
        //            {
        //                Tag tag = parse.LastTag;
        //                if (tag.Name.Equals("Url", StringComparison.CurrentCultureIgnoreCase))
        //                {
        //                    buffer.Length = 0;
        //                    capture = true;
        //                }
        //                else if (tag.Name.Equals("/Url", StringComparison.CurrentCultureIgnoreCase))
        //                {
        //                    result.Add(new Uri(buffer.ToString()));
        //                    buffer.Length = 0;
        //                    capture = false;
        //                }
        //            }
        //            else
        //            {
        //                if (capture)
        //                {
        //                    buffer.Append((char)ch);
        //                }
        //            }
        //        }
        //    }

        //    //response.Close();

        //    return result;
        //}

        ///// <summary>
        ///// Perform a Yahoo search.
        ///// </summary>
        ///// <param name="searchFor">What are we searching for.</param>
        ///// <returns>The URLs that contain the specified item.</returns>
        //public ICollection<Uri> Search(String searchFor)
        //{
        //    ICollection<Uri> result = null;

        //    // build the Uri
        //    var mstream = new MemoryStream();
        //    var form = new FormUtility(mstream, null);
        //    form.Add("appid", "YahooDemo");
        //    form.Add("results", "100");
        //    form.Add("query", searchFor);
        //    form.Complete();

        //    var enc = new ASCIIEncoding();

        //    String str = enc.GetString(mstream.GetBuffer());
        //    mstream.Dispose();

        //    var uri = new Uri(
        //        "http://search.yahooapis.com/WebSearchService/V1/webSearch?"
        //        + str);

        //    int tries = 0;
        //    bool done = false;
        //    while (!done)
        //    {
        //        try
        //        {
        //            result = DoSearch(uri);
        //            done = true;
        //        }
        //        catch (IOException e)
        //        {
        //            if (tries == 5)
        //            {
        //                throw;
        //            }
        //            Thread.Sleep(5000);
        //        }
        //        tries++;
        //    }

        //    return result;
        //}
    }

    public class ConsoleStatusReportable : IStatusReportable
    {
        #region IStatusReportable Members

        /// <summary>
        /// Simply display any status reports.
        /// </summary>
        /// <param name="total">Total amount.</param>
        /// <param name="current">Current item.</param>
        /// <param name="message">Current message.</param>
        public void Report(int total, int current,
                           String message)
        {
            if (total == 0)
            {
                Console.WriteLine(current + " : " + message);
            }
            else
            {
                Console.WriteLine(current + "/" + total + " : " + message);
            }
        }

        #endregion
    }

    public class SyntError : Exception
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public SyntError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public SyntError(Exception e)
            : base("Nested Exception", e)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="e">The exception.</param>
        public SyntError(String msg, Exception e)
            : base(msg, e)
        {
        }
    }

    public class SyntFramework
    {
        /// <summary>
        /// The current engog version, this should be read from the properties.
        /// </summary>
        public static string Version = "3.1.0";

        /// <summary>
        /// The platform.
        /// </summary>
        public static string PLATFORM = "DotNet";

        /// <summary>
        /// The current engog file version, this should be read from the properties.
        /// </summary>
        private const string FileVersion = "1";


        /// <summary>
        /// The default precision to use for compares.
        /// </summary>
        public const int DefaultPrecision = 10;

        /// <summary>
        /// Default point at which two doubles are equal.
        /// </summary>
        public const double DefaultDoubleEqual = 0.0000001;

        /// <summary>
        /// The version of the Synt JAR we are working with. Given in the form
        /// x.x.x.
        /// </summary>
        public const string SyntVersion = "Synt.version";

        /// <summary>
        /// The Synt file version. This determines of an Synt file can be read.
        /// This is simply an integer, that started with zero and is incramented each
        /// time the format of the Synt data file changes.
        /// </summary>
        public static string SyntFileVersion = "Synt.file.version";

        /// <summary>
        /// The instance.
        /// </summary>
        private static SyntFramework _instance = new SyntFramework();

        /// <summary>
        /// The current logging plugin.
        /// </summary>
        ///
        private ISyntPluginLogging1 _loggingPlugin;

        /// <summary>
        /// The plugins.
        /// </summary>
        ///
        private readonly IList<SyntPluginBase> _plugins;

        /// <summary>
        /// Get the instance to the singleton.
        /// </summary>
        public static SyntFramework Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Get the properties as a Map.
        /// </summary>
        private readonly IDictionary<string, string> _properties =
            new Dictionary<string, string>();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private SyntFramework()
        {
            _properties[SyntVersion] = Version;
            _properties[SyntFileVersion] = FileVersion;

            _plugins = new List<SyntPluginBase>();
            RegisterPlugin(new SystemLoggingPlugin());
            RegisterPlugin(new SystemMethodsPlugin());
            RegisterPlugin(new SystemTrainingPlugin());
            RegisterPlugin(new SystemActivationPlugin());
        }

        /// <summary>
        /// The Synt properties.  Contains version information.
        /// </summary>
        public IDictionary<string, string> Properties
        {
            get { return _properties; }
        }


        /// <summary>
        /// Shutdown Synt.
        /// </summary>
        public void Shutdown()
        {
        }

        /// <value>the loggingPlugin</value>
        public ISyntPluginLogging1 LoggingPlugin
        {
            get { return _loggingPlugin; }
        }

        /// <summary>
        /// Register a plugin. If this plugin provides a core service, such as
        /// calculation or logging, this will remove the old plugin.
        /// </summary>
        ///
        /// <param name="plugin">The plugin to register.</param>
        public void RegisterPlugin(SyntPluginBase plugin)
        {
            // is it not a general plugin?
            if (plugin.PluginServiceType != SyntPluginBaseConst.SERVICE_TYPE_GENERAL)
            {
                if (plugin.PluginServiceType == SyntPluginBaseConst.SERVICE_TYPE_LOGGING)
                {
                    // remove the old logging plugin
                    if (_loggingPlugin != null)
                    {
                        _plugins.Remove(_loggingPlugin);
                    }
                    _loggingPlugin = (ISyntPluginLogging1)plugin;
                }
            }
            // add to the plugins
            _plugins.Add(plugin);
        }

        /// <summary>
        /// Unregister a plugin. If you unregister the current logging or calc
        /// plugin, a new system one will be created. Synt will crash without a
        /// logging or system plugin.
        /// </summary>
        public void UnregisterPlugin(SyntPluginBase plugin)
        {
            // is it a special plugin?
            // if so, replace with the system, Synt will crash without these
            if (plugin == _loggingPlugin)
            {
                _loggingPlugin = new SystemLoggingPlugin();
            }

            // remove it
            _plugins.Remove(plugin);
        }

        /// <summary>
        /// The plugins.
        /// </summary>
        public IList<SyntPluginBase> Plugins
        {
            get { return _plugins; }
        }
    }

    public class NullStatusReportable : IStatusReportable
    {
        #region IStatusReportable Members

        /// <summary>
        /// Simply ignore any status reports.
        /// </summary>
        /// <param name="total">Not used.</param>
        /// <param name="current">Not used.</param>
        /// <param name="message">Not used.</param>
        public void Report(int total, int current,
                           String message)
        {
        }

        #endregion
    }

    public class ListExtractListener : IExtractListener
    {
        /// <summary>
        /// The list to extract into.
        /// </summary>
        private readonly IList<Object> _list = new List<Object>();

        /// <summary>
        /// The list of words extracted.
        /// </summary>
        public IList<Object> List
        {
            get { return _list; }
        }

        #region IExtractListener Members

        /// <summary>
        /// Called when a word is found, add it to the list.
        /// </summary>
        /// <param name="obj">The word found.</param>
        public void FoundData(Object obj)
        {
            _list.Add(obj);
        }

        #endregion
    }

    public class Div : DocumentRange
    {
        /// <summary>
        /// Construct a range to hold the DIV tag.
        /// </summary>
        /// <param name="source">The web page this range was found on.</param>
        public Div(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[Div:class=");
            result.Append(ClassAttribute);
            result.Append(",id=");
            result.Append(IdAttribute);
            result.Append(",elements=");
            result.Append(Elements.Count);
            result.Append("]");
            return result.ToString();
        }
    }

    public class DocumentRange
    {
        /// <summary>
        /// Sub elements of this range.
        /// </summary>
        private readonly IList<DocumentRange> _elements = new List<DocumentRange>();

        /// <summary>
        /// The source page for this range.
        /// </summary>
        private WebPage _source;

        /// <summary>
        /// Construct a document range from the specified WebPage.
        /// </summary>
        /// <param name="source">The web page that this range belongs to.</param>
        public DocumentRange(WebPage source)
        {
            _source = source;
        }

        /// <summary>
        /// The beginning of this attribute.
        /// </summary>
        public int Begin { get; set; }

        /// <summary>
        /// The HTML class attribiute for this element.
        /// </summary>
        public String ClassAttribute { get; set; }

        /// <summary>
        /// The elements of this document range. 
        /// </summary>
        public IList<DocumentRange> Elements
        {
            get { return _elements; }
        }

        /// <summary>
        /// The ending index.
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// The HTML id for this element.
        /// </summary>
        public String IdAttribute { get; set; }

        /// <summary>
        /// The web page that owns this class.
        /// </summary>
        public DocumentRange Parent { get; set; }

        /// <summary>
        /// The web page that this range is owned by.
        /// </summary>
        public WebPage Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>
        /// Add an element.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void AddElement(DocumentRange element)
        {
            Elements.Add(element);
            element.Parent = this;
        }

        /// <summary>
        /// Get the text from this range.
        /// </summary>
        /// <returns>The text from this range.</returns>
        public String GetTextOnly()
        {
            var result = new StringBuilder();

            for (int i = Begin; i < End; i++)
            {
                DataUnit du = _source.Data[i];
                if (du is TextDataUnit)
                {
                    result.Append(du.ToString());
                    result.Append("\n");
                }
            }

            return result.ToString();
        }


        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            return GetTextOnly();
        }
    }

    public class Form : DocumentRange
    {
        #region FormMethod enum

        /// <summary>
        /// The method for this form.
        /// </summary>
        public enum FormMethod
        {
            /// <summary>
            /// This form is to be POSTed.
            /// </summary>
            Post,
            /// <summary>
            /// This form is to sent using a GET.
            /// </summary>
            Get
        };

        #endregion

        /// <summary>
        /// Construct a form on the specified web page.
        /// </summary>
        /// <param name="source">The web page that contains this form.</param>
        public Form(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// The URL to send the form to.
        /// </summary>
        public Address Action { get; set; }

        /// <summary>
        /// The method, GET or POST.
        /// </summary>
        public FormMethod Method { get; set; }

        /// <summary>
        /// Find the form input by type.
        /// </summary>
        /// <param name="type">The type of input we want.</param>
        /// <param name="index">The index to begin searching at.</param>
        /// <returns>The Input object that was found.</returns>
        public Input FindType(String type, int index)
        {
            int i = index;

            foreach (DocumentRange element in Elements)
            {
                if (element is Input)
                {
                    var input = (Input)element;
                    if (String.Compare(input.Type, type, true) == 0)
                    {
                        if (i <= 0)
                        {
                            return input;
                        }
                        i--;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// The object as a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[Form:");
            builder.Append("method=");
            builder.Append(Method);
            builder.Append(",action=");
            builder.Append(Action);
            foreach (DocumentRange element in Elements)
            {
                builder.Append("\n\t");
                builder.Append(element.ToString());
            }
            builder.Append("]");
            return builder.ToString();
        }
    }

    public class Input : FormElement
    {
        /// <summary>
        /// The type of input element that this is.
        /// </summary>
        private String _type;

        /// <summary>
        /// Construct this Input element.
        /// </summary>
        /// <param name="source">The source for this input ent.</param>
        public Input(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// The type of this input.
        /// </summary>
        public String Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// True if this is autosend, which means that the type is NOT
        /// submit. This prevents a form that has multiple submit buttons
        /// from sending ALL of them in a single post.
        /// </summary>
        public override bool AutoSend
        {
            get { return string.Compare(_type, "submit", true) != 0; }
        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[Input:");
            builder.Append("type=");
            builder.Append(Type);
            builder.Append(",name=");
            builder.Append(Name);
            builder.Append(",value=");
            builder.Append(Value);
            builder.Append("]");
            return builder.ToString();
        }
    }

    public class Link : DocumentRange
    {
        /// <summary>
        /// The target address for this link.
        /// </summary>
        private Address _target;

        /// <summary>
        /// Construct a link from the specified web page.
        /// </summary>
        /// <param name="source">The web page this link is from.</param>
        public Link(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// The target of this link.
        /// </summary>
        public Address Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[Link:");
            result.Append(_target);
            result.Append("|");
            result.Append(GetTextOnly());
            result.Append("]");
            return result.ToString();
        }
    }

    public class Span : DocumentRange
    {
        /// <summary>
        /// Construct a span range from the specified web page.
        /// </summary>
        /// <param name="source">The source web page.</param>
        public Span(WebPage source)
            : base(source)
        {
        }

        /// <summary>
        /// This object as a string. 
        /// </summary>
        /// <returns>This object as a string. </returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[Span:class=");
            result.Append(ClassAttribute);
            result.Append(",id=");
            result.Append(IdAttribute);
            result.Append(",elements=");
            result.Append(Elements.Count);
            result.Append("]");
            return result.ToString();
        }
    }

    public class Address
    {
        /// <summary>
        /// The original text from the address.
        /// </summary>
        private readonly String _original;

        /// <summary>
        /// The address as a URL.
        /// </summary>
        private readonly Uri _url;

        /// <summary>
        /// Construct the address from a URL.
        /// </summary>
        /// <param name="u">The URL to use.</param>
        public Address(Uri u)
        {
            _url = u;
            _original = u.ToString();
        }

        /// <summary>
        /// Construct a URL using a perhaps relative URL and a base URL.
        /// </summary>
        /// <param name="b">The base URL.</param>
        /// <param name="original">A full URL or a URL relative to the base.</param>
        public Address(Uri b, String original)
        {
            _original = original;
            _url = b == null ? new Uri(new Uri("http://localhost/"), original) : new Uri(b, original);
        }

        /// <summary>
        /// The original text from this URL.
        /// </summary>
        public String Original
        {
            get { return _original; }
        }

        /// <summary>
        /// The URL.
        /// </summary>
        public Uri Url
        {
            get { return _url; }
        }

        /// <summary>
        /// The object as a string.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return _url != null ? _url.ToString() : _original;
        }
    }

    public class BrowseError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public BrowseError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public BrowseError(Exception e)
            : base(e)
        {
        }
    }

    public class Browser
    {
        /// <summary>
        /// The page that is currently being browsed.
        /// </summary>
        private WebPage _currentPage;


        /// <summary>
        /// The page currently being browsed.
        /// </summary>
        public WebPage CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }

        /// <summary>
        /// Navigate to the specified form by performing a submit of that form.
        /// </summary>
        /// <param name="form">The form to be submitted.</param>
        public void Navigate(Form form)
        {
            Navigate(form, null);
        }

        /// <summary>
        /// Navigate based on a form. Complete and post the form.
        /// </summary>
        /// <param name="form">The form to be posted.</param>
        /// <param name="submit">The submit button on the form to simulate clicking.</param>
        public void Navigate(Form form, Input submit)
        {
//            try
//            {
//#if logging
//                if (logger.IsInfoEnabled)
//                {
//                    logger.Info("Posting a form");
//                }
//#endif
//                Stream istream;
//                Stream ostream;
//                Uri targetURL;
//                WebRequest http = null;

//                if (form.Method == Form.FormMethod.Get)
//                {
//                    ostream = new MemoryStream();
//                }
//                else
//                {
//                    http = WebRequest.Create(form.Action.Url);
//                    http.Timeout = 30000;
//                    http.ContentType = "application/x-www-form-urlSyntesisd";
//                    http.Method = "POST";
//                    ostream = http.GetRequestStream();
//                }

//                // add the parameters if present
//                var formData = new FormUtility(ostream, null);
//                foreach (DocumentRange dr in form.Elements)
//                {
//                    if (dr is FormElement)
//                    {
//                        var element = (FormElement)dr;
//                        if ((element == submit) || element.AutoSend)
//                        {
//                            String name = element.Name;
//                            String value = element.Value;
//                            if (name != null)
//                            {
//                                if (value == null)
//                                {
//                                    value = "";
//                                }
//                                formData.Add(name, value);
//                            }
//                        }
//                    }
//                }

//                // now execute the command
//                if (form.Method == Form.FormMethod.Get)
//                {
//                    String action = form.Action.Url.ToString();
//                    ostream.Close();
//                    action += "?";
//                    action += ostream.ToString();
//                    targetURL = new Uri(action);
//                    http = WebRequest.Create(targetURL);
//                    var response = (HttpWebResponse)http.GetResponse();
//                    istream = response.GetResponseStream();
//                }
//                else
//                {
//                    targetURL = form.Action.Url;
//                    ostream.Close();
//                    var response = (HttpWebResponse)http.GetResponse();
//                    istream = response.GetResponseStream();
//                }

//                Navigate(targetURL, istream);
//                istream.Close();
//            }
//            catch (IOException e)
//            {
//                throw new BrowseError(e);
//            }
        }

        /// <summary>
        /// Navigate to a new page based on a link.
        /// </summary>
        /// <param name="link">The link to navigate to.</param>
 

        /// <summary>
        /// Navigate to a page based on a URL object. This will be an HTTP GET
        /// operation.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>


        /// <summary>
        /// Navigate to a page and post the specified data.
        /// </summary>
        /// <param name="url">The URL to post the data to.</param>
        /// <param name="istream">The data to post to the page.</param>
        public void Navigate(Uri url, Stream istream)
        {
#if logging
            if (logger.IsInfoEnabled)
            {
                logger.Info("POSTing to page:" + url);
            }
#endif
            var load = new LoadWebPage(url);
            _currentPage = load.Load(istream);
        }
    }

    public class LoadWebPage
    {
        /// <summary>
        /// The loaded webpage.
        /// </summary>
        private WebPage _page;

        /// <summary>
        /// The base URL for the page being loaded.
        /// </summary>
        private readonly Uri _baseURL;

        /// <summary>
        /// The last form that was processed.
        /// </summary>
        private Form _lastForm;

        /// <summary>
        /// The last hierarchy element that was processed.
        /// </summary>
        private DocumentRange _lastHierarchyElement;

        /// <summary>
        /// Construct a web page loader with the specified base URL.
        /// </summary>
        /// <param name="baseURL">The base URL to use when loading.</param>
        public LoadWebPage(Uri baseURL)
        {
            _baseURL = baseURL;
        }

        /// <summary>
        /// Add the specified hierarchy element.
        /// </summary>
        /// <param name="element">The hierarchy element to add.</param>
        private void AddHierarchyElement(DocumentRange element)
        {
            if (_lastHierarchyElement == null)
            {
                _page.AddContent(element);
            }
            else
            {
                _lastHierarchyElement.AddElement(element);
            }
            _lastHierarchyElement = element;
        }

        /// <summary>
        /// Create a dataunit to hode the code HTML tag.
        /// </summary>
        /// <param name="str">The code to create the data unit with.</param>
        private void CreateCodeDataUnit(String str)
        {
            if (str.Trim().Length > 0)
            {
                var d = new CodeDataUnit { Code = str };
                _page.AddDataUnit(d);
            }
        }

        /// <summary>
        /// Create a tag data unit.
        /// </summary>
        /// <param name="tag">The tag name to create the data unit for.</param>
        private void CreateTagDataUnit(Tag tag)
        {
            var d = new TagDataUnit { Tag = (Tag)tag.Clone() };

            _page.AddDataUnit(d);
        }

        /// <summary>
        /// Create a text data unit.
        /// </summary>
        /// <param name="str">The text.</param>
        private void CreateTextDataUnit(String str)
        {
            if (str.Trim().Length > 0)
            {
                var d = new TextDataUnit { Text = str };
                _page.AddDataUnit(d);
            }
        }

        /// <summary>
        ///  Find the end tag that lines up to the beginning tag.
        /// </summary>
        /// <param name="index">The index to start the search on. This specifies
        /// the starting data unit.</param>
        /// <param name="tag">The beginning tag that we are seeking the end tag 
        /// for.</param>
        /// <returns>The index that the ending tag was found at. Returns -1
        /// if not found.</returns>
        protected int FindEndTag(int index, Tag tag)
        {
            int depth = 0;
            int count = index;

            while (count < _page.getDataSize())
            {
                DataUnit du = _page.GetDataUnit(count);

                if (du is TagDataUnit)
                {
                    Tag nextTag = ((TagDataUnit)du).Tag;
                    if (String.Compare(tag.Name, nextTag.Name, true) == 0)
                    {
                        if (nextTag.TagType == Tag.Type.End)
                        {
                            if (depth == 0)
                            {
                                return count;
                            }
                            depth--;
                        }
                        else if (nextTag.TagType == Tag.Type.Begin)
                        {
                            depth++;
                        }
                    }
                }
                count++;
            }
            return -1;
        }

        /// <summary>
        /// Load a web page from the specified stream.
        /// </summary>
        /// <param name="istream">The input stream to load from.</param>
        /// <returns>The loaded web page.</returns>
        public WebPage Load(Stream istream)
        {
            _page = new WebPage();

            LoadDataUnits(istream);
            LoadContents();

            return _page;
        }

        /// <summary>
        /// Load the web page from a string that contains HTML.
        /// </summary>
        /// <param name="str">A string containing HTML.</param>
        /// <returns>The loaded WebPage.</returns>
        public WebPage Load(String str)
        {
            try
            {
                byte[] array = Encoding.UTF8.GetBytes(str);
                Stream bis = new MemoryStream(array);

                WebPage result = Load(bis);
                bis.Close();
                return result;
            }
            catch (IOException e)
            {
#if logging
                if (logger.IsDebugEnabled)
                {
                    logger.Debug("Exception", e);
                }
#endif
                throw new BrowseError(e);
            }
        }

        /// <summary>
        /// Using the data units, which should have already been loaded by this 
        /// time, load the contents of the web page.  This includes the title,
        /// any links and forms.  Div tags and spans are also processed.
        /// </summary>
        protected void LoadContents()
        {
            for (int index = 0; index < _page.getDataSize(); index++)
            {
                DataUnit du = _page.GetDataUnit(index);
                if (du is TagDataUnit)
                {
                    Tag tag = ((TagDataUnit)du).Tag;

                    if (tag.TagType != Tag.Type.End)
                    {
                        if (string.Compare(tag.Name, "a", true) == 0)
                        {
                            LoadLink(index, tag);
                        }
                        else if (string.Compare(tag.Name, "title", true) == 0)
                        {
                            LoadTitle(index, tag);
                        }
                        else if (string.Compare(tag.Name, "form", true) == 0)
                        {
                            LoadForm(index, tag);
                        }
                        else if (string.Compare(tag.Name, "input", true) == 0)
                        {
                            LoadInput(index, tag);
                        }
                    }

                    if (tag.TagType == Tag.Type.Begin)
                    {
                        if (String.Compare(tag.Name, "div", true) == 0)
                        {
                            LoadDiv(index, tag);
                        }
                        else if (String.Compare(tag.Name, "span", true) == 0)
                        {
                            LoadSpan(index, tag);
                        }
                    }

                    if (tag.TagType == Tag.Type.End)
                    {
                        if (string.Compare(tag.Name, "div") == 0)
                        {
                            if (_lastHierarchyElement != null)
                            {
                                _lastHierarchyElement =
                                    _lastHierarchyElement.Parent;
                            }
                        }
                        else if (String.Compare(tag.Name, "span", true) == 0)
                        {
                            if (_lastHierarchyElement != null)
                            {
                                _lastHierarchyElement =
                                    _lastHierarchyElement.Parent;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load the data units.  Once the lower level data units have been 
        /// loaded, the contents can be loaded.
        /// </summary>
        /// <param name="istream">The input stream that the data units are loaded from.</param>
        protected void LoadDataUnits(Stream istream)
        {
            var text = new StringBuilder();
            int ch;
            var parse = new ReadHTML(istream);
            bool style = false;
            bool script = false;

            while ((ch = parse.Read()) != -1)
            {
                if (ch == 0)
                {
                    if (style)
                    {
                        CreateCodeDataUnit(text.ToString());
                    }
                    else if (script)
                    {
                        CreateCodeDataUnit(text.ToString());
                    }
                    else
                    {
                        CreateTextDataUnit(text.ToString());
                    }
                    style = false;
                    script = false;

                    text.Length = 0;
                    CreateTagDataUnit(parse.LastTag);
                    if (String.Compare(parse.LastTag.Name, "style", true) == 0)
                    {
                        style = true;
                    }
                    else if (string.Compare(parse.LastTag.Name,
                                            "script", true) == 0)
                    {
                        script = true;
                    }
                }
                else
                {
                    text.Append((char)ch);
                }
            }

            CreateTextDataUnit(text.ToString());
        }


        /// <summary>
        /// Called by loadContents to load a div tag.
        /// </summary>
        /// <param name="index">The index to begin at.</param>
        /// <param name="tag">The beginning div tag.</param>
        private void LoadDiv(int index, Tag tag)
        {
            var div = new Div(_page);
            String classAttribute = tag.GetAttributeValue("class");
            String idAttribute = tag.GetAttributeValue("id");

            div.IdAttribute = idAttribute;
            div.ClassAttribute = (classAttribute);
            div.Begin = index;
            div.End = FindEndTag(index + 1, tag);
            AddHierarchyElement(div);
        }

        /// <summary>
        /// Called by loadContents to load a form on the page.
        /// </summary>
        /// <param name="index">The index to begin loading at.</param>
        /// <param name="tag">The beginning tag.</param>
        protected void LoadForm(int index, Tag tag)
        {
            String method = tag.GetAttributeValue("method");
            String action = tag.GetAttributeValue("action");

            var form = new Form(_page);
            form.Begin = index;
            form.End = FindEndTag(index + 1, tag);

            if ((method == null) || string.Compare(method, "GET", true) == 0)
            {
                form.Method = Form.FormMethod.Get;
            }
            else
            {
                form.Method = Form.FormMethod.Post;
            }

            if (action == null)
            {
                form.Action = new Address(_baseURL);
            }
            else
            {
                form.Action = new Address(_baseURL, action);
            }

            _page.AddContent(form);
            _lastForm = form;
        }

        /// <summary>
        /// Called by loadContents to load an input tag on the form.
        /// </summary>
        /// <param name="index">The index to begin loading at.</param>
        /// <param name="tag">The beginning tag.</param>
        protected void LoadInput(int index, Tag tag)
        {
            String type = tag.GetAttributeValue("type");
            String name = tag.GetAttributeValue("name");
            String value = tag.GetAttributeValue("value");

            var input = new Input(_page);
            input.Type = type;
            input.Name = name;
            input.Value = value;

            if (_lastForm != null)
            {
                _lastForm.AddElement(input);
            }
            else
            {
                _page.AddContent(input);
            }
        }

        /// <summary>
        /// Called by loadContents to load a link on the page.
        /// </summary>
        /// <param name="index">The index to begin loading at.</param>
        /// <param name="tag">The beginning tag.</param>
        protected void LoadLink(int index, Tag tag)
        {
            var link = new Link(_page);
            String href = tag.GetAttributeValue("href");

            if (href != null)
            {
                link.Target = new Address(_baseURL, href);
                link.Begin = index;
                link.End = FindEndTag(index + 1, tag);
                _page.AddContent(link);
            }
        }

        /// <summary>
        /// Called by loadContents to load a span.
        /// </summary>
        /// <param name="index">The index to begin loading at.</param>
        /// <param name="tag">The beginning tag.</param>
        private void LoadSpan(int index, Tag tag)
        {
            var span = new Span(_page);
            String classAttribute = tag.GetAttributeValue("class");
            String idAttribute = tag.GetAttributeValue("id");

            span.IdAttribute = idAttribute;
            span.ClassAttribute = classAttribute;
            span.Begin = index;
            span.End = FindEndTag(index + 1, tag);
            AddHierarchyElement(span);
        }

        /// <summary>
        /// Called by loadContents to load the title of the page.
        /// </summary>
        /// <param name="index">The index to begin loading at.</param>
        /// <param name="tag">The beginning tag.</param>
        protected void LoadTitle(int index, Tag tag)
        {
            var title = new DocumentRange(_page);
            title.Begin = index;
            title.End = FindEndTag(index + 1, tag);
            _page.Title = title;
        }
    }

    public class WebPage
    {
        /// <summary>
        /// The contents of this page, builds upon the list of DataUnits.
        /// </summary>
        private readonly IList<DocumentRange> _contents = new List<DocumentRange>();

        /// <summary>
        /// The data units that make up this page.
        /// </summary>
        private readonly IList<DataUnit> _data = new List<DataUnit>();

        /// <summary>
        /// The title of this HTML page.
        /// </summary>
        private DocumentRange _title;

        /// <summary>
        /// The contents in a list collection.
        /// </summary>
        public IList<DocumentRange> Contents
        {
            get { return _contents; }
        }

        /// <summary>
        /// The data units in a list collection.
        /// </summary>
        public IList<DataUnit> Data
        {
            get { return _data; }
        }

        /// <summary>
        /// The title of this document.
        /// </summary>
        public DocumentRange Title
        {
            get { return _title; }
            set
            {
                _title = value;
                _title.Source = this;
            }
        }

        /// <summary>
        /// Add to the content collection.
        /// </summary>
        /// <param name="span">The range to add to the collection.</param>
        public void AddContent(DocumentRange span)
        {
            span.Source = this;
            _contents.Add(span);
        }

        /// <summary>
        /// Add a data unit to the collection.
        /// </summary>
        /// <param name="unit">The data unit to load.</param>
        public void AddDataUnit(DataUnit unit)
        {
            _data.Add(unit);
        }

        /// <summary>
        /// Find the specified DocumentRange subclass in the contents list.
        /// </summary>
        /// <param name="c">The class type to search for.</param>
        /// <param name="index">The index to search from.</param>
        /// <returns>The document range that was found.</returns>
        public DocumentRange Find(Type c, int index)
        {
            int i = index;
            foreach (DocumentRange span in Contents)
            {
                if (span.GetType() == c)
                {
                    if (i <= 0)
                    {
                        return span;
                    }
                    i--;
                }
            }
            return null;
        }

        /// <summary>
        /// Find the link that contains the specified string.
        /// </summary>
        /// <param name="str">The string to search for.</param>
        /// <returns>The link found.</returns>
        public Link FindLink(String str)
        {
            return Contents.OfType<Link>().FirstOrDefault(link => link.GetTextOnly().Equals(str));
        }

        /// <summary>
        /// Get the number of data items in this collection.
        /// </summary>
        /// <returns>The size of the data unit.</returns>
        public int getDataSize()
        {
            return _data.Count;
        }

        /// <summary>
        /// Get the DataUnit unit at the specified index.
        /// </summary>
        /// <param name="i">The index to use.</param>
        /// <returns>The DataUnit found at the specified index.</returns>
        public DataUnit GetDataUnit(int i)
        {
            return _data[i];
        }


        /// <summary>
        /// The object as a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            foreach (DocumentRange span in Contents)
            {
                result.Append(span.ToString());
                result.Append("\n");
            }
            return result.ToString();
        }
    }

    public class CodeDataUnit : DataUnit
    {
        /// <summary>
        /// The code for this data unit.
        /// </summary>
        private String _code;

        /// <summary>
        /// The code for this data unit.
        /// </summary>
        public String Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            return _code;
        }
    }

    public class DataUnit
    {
    }

    public class TagDataUnit : DataUnit
    {
        /// <summary>
        /// The tag that this data unit is based on.
        /// </summary>
        public Tag Tag { get; set; }
    }

    public class TextDataUnit : DataUnit
    {
        /// <summary>
        /// The text for this data unit.
        /// </summary>
        private String _text;

        /// <summary>
        /// The text for this data unit.
        /// </summary>
        public String Text
        {
            get { return _text; }
            set { _text = value; }
        }


        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            return _text;
        }
    }

    public class RSS
    {
        /// <summary>
        /// All of the attributes for this RSS document.
        /// </summary>
        private readonly Dictionary<String, String> _attributes = new Dictionary<String, String>();

        /// <summary>
        /// All RSS items, or stories, found.
        /// </summary>
        private readonly List<RSSItem> _items = new List<RSSItem>();

        /// <summary>
        /// All of the attributes for this RSS document.
        /// </summary>
        public Dictionary<String, String> Attributes
        {
            get { return _attributes; }
        }

        /// <summary>
        /// All RSS items, or stories, found.
        /// </summary>
        public List<RSSItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Simple utility function that converts a RSS formatted date
        /// into a C# date.
        /// </summary>
        /// <param name="datestr">A date</param>
        /// <returns>A C# DateTime object.</returns>
        public static DateTime ParseDate(String datestr)
        {
            DateTime date = DateTime.Parse(datestr);
            return date;
        }

        /// <summary>
        /// Load the specified RSS item, or story.
        /// </summary>
        /// <param name="item">A XML node that contains a RSS item.</param>
        private void LoadItem(XmlNode item)
        {
            var rssItem = new RSSItem();
            rssItem.Load(item);
            _items.Add(rssItem);
        }

        /// <summary>
        /// Load the channle node.
        /// </summary>
        /// <param name="channel">A node that contains a channel.</param>
        private void LoadChannel(XmlNode channel)
        {
            foreach (XmlNode node in channel.ChildNodes)
            {
                String nodename = node.Name;
                if (String.Compare(nodename, "item", true) == 0)
                {
                    LoadItem(node);
                }
                else
                {
                    _attributes.Remove(nodename);
                    _attributes.Add(nodename, channel.InnerText);
                }
            }
        }

        /// <summary>
        /// Load all RSS data from the specified URL.
        /// </summary>
        /// <param name="url">URL that contains XML data.</param>
        public void Load(Uri url)
        {
            //WebRequest http = WebRequest.Create(url);
            //var response = (HttpWebResponse)http.GetResponse();
            //Stream istream = response.GetResponseStream();

            //var d = new XmlDocument();
            //d.Load(istream);

            //foreach (XmlNode node in d.DocumentElement.ChildNodes)
            //{
            //    String nodename = node.Name;

            //    // RSS 2.0
            //    if (String.Compare(nodename, "channel", true) == 0)
            //    {
            //        LoadChannel(node);
            //    }
            //    // RSS 1.0
            //    else if (String.Compare(nodename, "item", true) == 0)
            //    {
            //        LoadItem(node);
            //    }
            //}
        }

        /// <summary>
        /// Convert the object to a String.
        /// </summary>
        /// <returns>The object as a String.</returns>
        public override String ToString()
        {
            var str = new StringBuilder();

            foreach (String item in _attributes.Keys)
            {
                str.Append(item);
                str.Append('=');
                str.Append(_attributes[item]);
                str.Append('\n');
            }
            str.Append("Items:\n");
            foreach (RSSItem item in _items)
            {
                str.Append(item.ToString());
                str.Append('\n');
            }
            return str.ToString();
        }
    }

    public class RSSItem
    {
        /// <summary>
        /// The date this item was published.
        /// </summary>
        private DateTime _date;

        /// <summary>
        /// The description of this item.
        /// </summary>
        private String _description;

        /// <summary>
        /// The hyperlink to this item.
        /// </summary>
        private String _link;

        /// <summary>
        /// The title of this item.
        /// </summary>
        private String _title;

        /// <summary>
        /// The title of this item.
        /// </summary>
        public String Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// The hyperlink to this item.
        /// </summary>
        public String Link
        {
            get { return _link; }
            set { _link = value; }
        }


        /// <summary>
        /// The description of this item.
        /// </summary>
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// The date this item was published.
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }


        /// <summary>
        /// Load an item from the specified node.
        /// </summary>
        /// <param name="node">The Node to load the item from.</param>
        public void Load(XmlNode node)
        {
            foreach (XmlNode n in node.ChildNodes)
            {
                String name = n.Name;

                if (String.Compare(name, "title", true) == 0)
                    _title = n.InnerText;
                else if (String.Compare(name, "link", true) == 0)
                    _link = n.InnerText;
                else if (String.Compare(name, "description", true) == 0)
                    _description = n.InnerText;
                else if (String.Compare(name, "pubDate", true) == 0)
                {
                    String str = n.InnerText;
                    _date = RSS.ParseDate(str);
                }
            }
        }


        /// <summary>
        /// Convert the object to a String.
        /// </summary>
        /// <returns>The object as a String.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder();
            builder.Append('[');
            builder.Append("title=\"");
            builder.Append(_title);
            builder.Append("\",link=\"");
            builder.Append(_link);
            builder.Append("\",date=\"");
            builder.Append(_date);
            builder.Append("\"]");
            return builder.ToString();
        }
    }

    public class BotError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public BotError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public BotError(Exception e)
            : base(e)
        {
        }
    }

    public class BotUtil
    {
        /// <summary>
        /// How much data to read at once.
        /// </summary>
        public static int BufferSize = 8192;

        /// <summary>
        /// This method is very useful for grabbing information from a HTML page.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="token1">The text, or tag, that comes before the desired text</param>
        /// <param name="token2">The text, or tag, that comes after the desired text</param>
        /// <param name="index">Index in the string to start searching from.</param>
        /// <param name="occurence">What occurence.</param>
        /// <returns>The contents of the URL that was downloaded.</returns>
        public static String ExtractFromIndex(String str, String token1,
                                              String token2, int index, int occurence)
        {
            // convert everything to lower case
            String searchStr = str.ToLower();
            String token1Lower = token1.ToLower();
            String token2Lower = token2.ToLower();

            int count = occurence;

            // now search
            int location1 = index - 1;
            do
            {
                location1 = searchStr.IndexOf(token1Lower, location1 + 1);

                if (location1 == -1)
                {
                    return null;
                }

                count--;
            } while (count > 0);


            // return the result from the original string that has mixed
            // case
            int location2 = searchStr.IndexOf(token2Lower, location1 + 1);
            if (location2 == -1)
            {
                return null;
            }

            return str.Substring(location1 + token1Lower.Length, location2 - (location1 + token1.Length));
        }

        /// <summary>
        /// This method is very useful for grabbing information from a HTML page.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="token1">The text, or tag, that comes before the desired text.</param>
        /// <param name="token2">The text, or tag, that comes after the desired text.</param>
        /// <param name="index">Which occurrence of token1 to use, 1 for the first.</param>
        /// <returns>The contents of the URL that was downloaded.</returns>
        public static String Extract(String str, String token1,
                                     String token2, int index)
        {
            // convert everything to lower case
            String searchStr = str.ToLower();
            String token1Lower = token1.ToLower();
            String token2Lower = token2.ToLower();

            int count = index;

            // now search
            int location1 = -1;
            do
            {
                location1 = searchStr.IndexOf(token1Lower, location1 + 1);

                if (location1 == -1)
                {
                    return null;
                }

                count--;
            } while (count > 0);

            // return the result from the original string that has mixed
            // case
            int location2 = searchStr.IndexOf(token2Lower, location1 + 1);
            if (location2 == -1)
            {
                return null;
            }

            return str.Substring(location1 + token1Lower.Length, location2 - (location1 + token1.Length));
        }

        /// <summary>
        /// Post to a page.
        /// </summary>
        /// <param name="uri">The URI to post to.</param>
        /// <param name="param">The post params.</param>
        /// <returns>The HTTP response.</returns>
   

        /// <summary>
        /// Post bytes to a page.
        /// </summary>
        /// <param name="uri">The URI to post to.</param>
        /// <param name="bytes">The bytes to post.</param>
        /// <param name="length">The length of the posted data.</param>
        /// <returns>The HTTP response.</returns>
  

        /// <summary>
        /// Load the specified web page into a string.
        /// </summary>
        /// <param name="url">The url to load.</param>
        /// <returns>The web page as a string.</returns>
   

        /// <summary>
        /// Private constructor.
        /// </summary>
        private BotUtil()
        {
        }

        /// <summary>
        /// Post to a page.
        /// </summary>
        /// <param name="uri">The URI to post to.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The page returned.</returns>
   
    }

    [Serializable]
    public class ActivationBiPolar : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct the bipolar activation function.
        /// </summary>
        ///
        public ActivationBiPolar()
        {
            _paras = new double[0];
        }

        /// <inheritdoc/>
        public virtual double DerivativeFunction(double b, double a)
        {
            return 1;
        }


        /// <returns>Return true, bipolar has a 1 for derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            for (int i = start; i < start + size; i++)
            {
                if (x[i] > 0)
                {
                    x[i] = 1;
                }
                else
                {
                    x[i] = -1;
                }
            }
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { "slope" };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationBiPolar();
        }
    }

    [Serializable]
    public class ActivationCompetitive : IActivationFunction
    {
        /// <summary>
        /// The offset to the parameter that holds the max winners.
        /// </summary>
        ///
        public const int ParamCompetitiveMaxWinners = 0;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Create a competitive activation function with one winner allowed.
        /// </summary>
        ///
        public ActivationCompetitive()
            : this(1)
        {
        }

        /// <summary>
        /// Create a competitive activation function with the specified maximum
        /// number of winners.
        /// </summary>
        ///
        /// <param name="winners">The maximum number of winners that this function supports.</param>
        public ActivationCompetitive(int winners)
        {
            _paras = new double[1];
            _paras[ParamCompetitiveMaxWinners] = winners;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            var winners = new bool[x.Length];
            double sumWinners = 0;

            // find the desired number of winners
            for (int i = 0; i < _paras[0]; i++)
            {
                double maxFound = Double.NegativeInfinity;
                int winner = -1;

                // find one winner
                for (int j = start; j < start + size; j++)
                {
                    if (!winners[j] && (x[j] > maxFound))
                    {
                        winner = j;
                        maxFound = x[j];
                    }
                }
                sumWinners += maxFound;
                winners[winner] = true;
            }

            // adjust weights for winners and non-winners
            for (int i = start; i < start + size; i++)
            {
                if (winners[i])
                {
                    x[i] = x[i] / sumWinners;
                }
                else
                {
                    x[i] = 0.0d;
                }
            }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        object ICloneable.Clone()
        {
            return new ActivationCompetitive(
                (int)_paras[ParamCompetitiveMaxWinners]);
        }

        /// <inheritdoc/>
        public virtual double DerivativeFunction(double b, double a)
        {
            throw new NeuralNetworkError(
                "Can't use the competitive activation function "
                + "where a derivative is required.");
        }


        /// <summary>
        /// The maximum number of winners this function supports.
        /// </summary>
        public int MaxWinners
        {
            get { return (int)_paras[ParamCompetitiveMaxWinners]; }
        }


        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { "maxWinners" };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }


        /// <returns>False, indication that no derivative is available for thisfunction.</returns>
        public virtual bool HasDerivative()
        {
            return false;
        }
    }

    public class ActivationElliott : IActivationFunction
    {
        /// <summary>
        /// The params.
        /// </summary>
        private readonly double[] _p;

        /// <summary>
        /// Construct a basic Elliott activation function, with a slope of 1.
        /// </summary>
        public ActivationElliott()
        {
            _p = new double[1];
            _p[0] = 1.0;
        }

        #region IActivationFunction Members

        /// <inheritdoc />
        public void ActivationFunction(double[] x, int start,
                                       int size)
        {
            for (int i = start; i < start + size; i++)
            {
                double s = _p[0];
                x[i] = ((x[i] * s) / 2) / (1 + Math.Abs(x[i] * s)) + 0.5;
            }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The object to be cloned.</returns>
        public object Clone()
        {
            return new ActivationElliott();
        }

        /// <inheritdoc />
        public double DerivativeFunction(double b, double a)
        {
            double s = _p[0];
            return s / (2.0 * (1.0 + Math.Abs(b * s)) * (1 + Math.Abs(b * s)));
        }

        /// <inheritdoc />
        public String[] ParamNames
        {
            get
            {
                String[] result = { "Slope" };
                return result;
            }
        }

        /// <inheritdoc />
        public double[] Params
        {
            get { return _p; }
        }

        /// <summary>
        /// Return true, Elliott activation has a derivative.
        /// </summary>
        /// <returns>Return true, Elliott activation has a derivative.</returns>
        public bool HasDerivative()
        {
            return true;
        }

        #endregion

        /// <inheritdoc />
        public void SetParam(int index, double value)
        {
            _p[index] = value;
        }
    }

    public class ActivationElliottSymmetric : IActivationFunction
    {
        /// <summary>
        /// The params.
        /// </summary>
        private readonly double[] _p;

        /// <summary>
        /// Construct a basic Elliott activation function, with a slope of 1.
        /// </summary>
        public ActivationElliottSymmetric()
        {
            _p = new double[1];
            _p[0] = 1.0;
        }

        #region IActivationFunction Members

        /// <inheritdoc />
        public void ActivationFunction(double[] x, int start,
                                       int size)
        {
            for (int i = start; i < start + size; i++)
            {
                double s = _p[0];
                x[i] = (x[i] * s) / (1 + Math.Abs(x[i] * s));
            }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The object to be cloned.</returns>
        public object Clone()
        {
            return new ActivationElliottSymmetric();
        }

        /// <inheritdoc />
        public double DerivativeFunction(double b, double a)
        {
            double s = _p[0];
            double d = (1.0 + Math.Abs(b * s));
            return (s * 1.0) / (d * d);
        }

        /// <inheritdoc />
        public String[] ParamNames
        {
            get
            {
                String[] result = { "Slope" };
                return result;
            }
        }

        /// <inheritdoc />
        public double[] Params
        {
            get { return _p; }
        }

        /// <summary>
        /// Return true, Elliott activation has a derivative.
        /// </summary>
        /// <returns>Return true, Elliott activation has a derivative.</returns>
        public bool HasDerivative()
        {
            return true;
        }

        #endregion

        /// <inheritdoc />
        public void SetParam(int index, double value)
        {
            _p[index] = value;
        }
    }

    [Serializable]
    public class ActivationGaussian : IActivationFunction
    {
        /// <summary>
        /// The offset to the parameter that holds the width.
        /// </summary>
        ///
        public const int ParamGaussianCenter = 0;

        /// <summary>
        /// The offset to the parameter that holds the peak.
        /// </summary>
        ///
        public const int ParamGaussianPeak = 1;

        /// <summary>
        /// The offset to the parameter that holds the width.
        /// </summary>
        ///
        public const int ParamGaussianWidth = 2;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Create an empty activation gaussian.
        /// </summary>
        public ActivationGaussian()
        {
        }

        /// <summary>
        /// Create a gaussian activation function.
        /// </summary>
        ///
        /// <param name="center">The center of the curve.</param>
        /// <param name="peak">The peak of the curve.</param>
        /// <param name="width">The width of the curve.</param>
        public ActivationGaussian(double center, double peak,
                                  double width)
        {
            _paras = new double[3];
            _paras[ParamGaussianCenter] = center;
            _paras[ParamGaussianPeak] = peak;
            _paras[ParamGaussianWidth] = width;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationGaussian(Center, Peak,
                                          Width);
        }


        /// <summary>
        /// The width of the function.
        /// </summary>
        private double Width
        {
            get { return Params[ParamGaussianWidth]; }
        }


        /// <summary>
        /// The center of the function.
        /// </summary>
        private double Center
        {
            get { return Params[ParamGaussianCenter]; }
        }


        /// <summary>
        /// The peak of the function.
        /// </summary>
        private double Peak
        {
            get { return Params[ParamGaussianPeak]; }
        }


        /// <summary>
        /// Return true, gaussian has a derivative.
        /// </summary>
        /// <returns>Return true, gaussian has a derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            for (int i = start; i < start + size; i++)
            {
                x[i] = _paras[ParamGaussianPeak]
                       * BoundMath
                            .Exp(-Math.Pow(x[i]
                                           - _paras[ParamGaussianCenter], 2)
                                 / (2.0d * _paras[ParamGaussianWidth] * _paras[ParamGaussianWidth]));
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            double width = _paras[ParamGaussianWidth];
            double peak = _paras[ParamGaussianPeak];
            return Math.Exp(-0.5d * width * width * b * b) * peak * width * width
                   * (width * width * b * b - 1);
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { "center", "peak", "width" };
                return result;
            }
        }


        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        ///
        public virtual double[] Params
        {
            get { return _paras; }
        }
    }

    [Serializable]
    public class ActivationLinear : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct a linear activation function, with a slope of 1.
        /// </summary>
        ///
        public ActivationLinear()
        {
            _paras = new double[0];
        }


        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationLinear();
        }


        /// <returns>Return true, linear has a 1 derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            for (int i = start; i < start + size; i++)
            {
                x[i] = x[i];
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            return 1;
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }
    }

    [Serializable]
    public class ActivationLOG : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct the activation function.
        /// </summary>
        ///
        public ActivationLOG()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationLOG();
        }


        /// <returns>Return true, log has a derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            for (int i = start; i < start + size; i++)
            {
                if (x[i] >= 0)
                {
                    x[i] = BoundMath.Log(1 + x[i]);
                }
                else
                {
                    x[i] = -BoundMath.Log(1 - x[i]);
                }
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            if (b >= 0)
            {
                return 1 / (1 + b);
            }
            return 1 / (1 - b);
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }
    }

    [Serializable]
    public class ActivationRamp : IActivationFunction
    {
        /// <summary>
        /// The ramp high threshold parameter.
        /// </summary>
        ///
        public const int ParamRampHighThreshold = 0;

        /// <summary>
        /// The ramp low threshold parameter.
        /// </summary>
        ///
        public const int ParamRampLowThreshold = 1;

        /// <summary>
        /// The ramp high parameter.
        /// </summary>
        ///
        public const int ParamRampHigh = 2;

        /// <summary>
        /// The ramp low parameter.
        /// </summary>
        ///
        public const int ParamRampLow = 3;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct a ramp activation function.
        /// </summary>
        ///
        /// <param name="thresholdHigh">The high threshold value.</param>
        /// <param name="thresholdLow">The low threshold value.</param>
        /// <param name="high">The high value, replaced if the high threshold is exceeded.</param>
        /// <param name="low">The low value, replaced if the low threshold is exceeded.</param>
        public ActivationRamp(double thresholdHigh,
                              double thresholdLow, double high, double low)
        {
            _paras = new double[4];
            _paras[ParamRampHighThreshold] = thresholdHigh;
            _paras[ParamRampLowThreshold] = thresholdLow;
            _paras[ParamRampHigh] = high;
            _paras[ParamRampLow] = low;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public ActivationRamp()
            : this(1, 0, 1, 0)
        {
        }


        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationRamp(
                _paras[ParamRampHighThreshold],
                _paras[ParamRampLowThreshold],
                _paras[ParamRampHigh],
                _paras[ParamRampLow]);
        }


        /// <summary>
        /// The high value.
        /// </summary>
        public double High
        {
            get { return _paras[ParamRampHigh]; }
            set { _paras[ParamRampHigh] = value; }
        }


        /// <summary>
        /// The low value.
        /// </summary>
        public double Low
        {
            get { return _paras[ParamRampLow]; }
            set { _paras[ParamRampLow] = value; }
        }


        /// <summary>
        /// Set the threshold high.
        /// </summary>
        public double ThresholdHigh
        {
            get { return _paras[ParamRampHighThreshold]; }
            set { _paras[ParamRampHighThreshold] = value; }
        }


        /// <summary>
        /// The threshold low.
        /// </summary>
        public double ThresholdLow
        {
            get { return _paras[ParamRampLowThreshold]; }
            set { _paras[ParamRampLowThreshold] = value; }
        }


        /// <summary>
        /// True, as this function does have a derivative.
        /// </summary>
        /// <returns>True, as this function does have a derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            double slope = (_paras[ParamRampHighThreshold] - _paras[ParamRampLowThreshold])
                           / (_paras[ParamRampHigh] - _paras[ParamRampLow]);

            for (int i = start; i < start + size; i++)
            {
                if (x[i] < _paras[ParamRampLowThreshold])
                {
                    x[i] = _paras[ParamRampLow];
                }
                else if (x[i] > _paras[ParamRampHighThreshold])
                {
                    x[i] = _paras[ParamRampHigh];
                }
                else
                {
                    x[i] = (slope * x[i]);
                }
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            return 1.0d;
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = {
                                      "thresholdHigh", "thresholdLow", "high",
                                      "low"
                                  };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }
    }

    [Serializable]
    public class ActivationSigmoid : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct a basic sigmoid function, with a slope of 1.
        /// </summary>
        ///
        public ActivationSigmoid()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationSigmoid();
        }

        /// <returns>True, sigmoid has a derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            for (int i = start; i < start + size; i++)
            {
                x[i] = 1.0d / (1.0d + BoundMath.Exp(-1 * x[i]));
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            return a * (1.0d - a);
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] results = { };
                return results;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }


    }

    [Serializable]
    public class ActivationSIN : IActivationFunction
    {
        /// <summary>
        /// Construct the sin activation function.
        /// </summary>
        ///
        public ActivationSIN()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationSIN();
        }


        /// <returns>Return true, sin has a derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            for (int i = start; i < start + size; i++)
            {
                x[i] = BoundMath.Sin(x[i]);
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            return BoundMath.Cos(b);
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }
    }

    [Serializable]
    public class ActivationSoftMax : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct the soft-max activation function.
        /// </summary>
        ///
        public ActivationSoftMax()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationSoftMax();
        }

        /// <returns>Return false, softmax has no derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            double sum = 0;
            for (int i = start; i < start + size; i++)
            {
                x[i] = BoundMath.Exp(x[i]);
                sum += x[i];
            }
            for (int i = start; i < start + size; i++)
            {
                x[i] = x[i] / sum;
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            return 1.0d;
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }
    }

    [Serializable]
    public class ActivationStep : IActivationFunction
    {
        /// <summary>
        /// The step center parameter.
        /// </summary>
        ///
        public const int ParamStepCenter = 0;

        /// <summary>
        /// The step low parameter.
        /// </summary>
        ///
        public const int ParamStepLow = 1;

        /// <summary>
        /// The step high parameter.
        /// </summary>
        ///
        public const int ParamStepHigh = 2;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct a step activation function.
        /// </summary>
        ///
        /// <param name="low">The low of the function.</param>
        /// <param name="center">The center of the function.</param>
        /// <param name="high">The high of the function.</param>
        public ActivationStep(double low, double center, double high)
        {
            _paras = new double[3];
            _paras[ParamStepCenter] = center;
            _paras[ParamStepLow] = low;
            _paras[ParamStepHigh] = high;
        }

        /// <summary>
        /// Create a basic step activation with low=0, center=0, high=1.
        /// </summary>
        ///
        public ActivationStep()
            : this(0.0d, 0.0d, 1.0d)
        {
        }

        /// <summary>
        /// Set the center of this function.
        /// </summary>
        public double Center
        {
            get { return _paras[ParamStepCenter]; }
            set { _paras[ParamStepCenter] = value; }
        }


        /// <summary>
        /// Set the low of this function.
        /// </summary>
        public double Low
        {
            get { return _paras[ParamStepLow]; }
            set { _paras[ParamStepLow] = value; }
        }


        /// <summary>
        /// Set the high of this function.
        /// </summary>
        public double High
        {
            get { return _paras[ParamStepHigh]; }
            set { _paras[ParamStepHigh] = value; }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            var result = new ActivationStep(Low, Center,
                                            High);
            return result;
        }

        /// <returns>Returns true, this activation function has a derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            for (int i = start; i < start + size; i++)
            {
                if (x[i] >= _paras[ParamStepCenter])
                {
                    x[i] = _paras[ParamStepHigh];
                }
                else
                {
                    x[i] = _paras[ParamStepLow];
                }
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            return 1.0d;
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { "center", "low", "high" };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }

    }

    [Serializable]
    public class ActivationTANH : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct a basic HTAN activation function, with a slope of 1.
        /// </summary>
        ///
        public ActivationTANH()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationTANH();
        }


        /// <returns>Return true, TANH has a derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            for (int i = start; i < start + size; i++)
            {
                x[i] = 2.0 / (1.0 + BoundMath.Exp(-2.0 * x[i])) - 1.0; //3x faster than Math.Tanh(x[i]);
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            return (1.0d - a * a);
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }
    }

    public class BIFDefinition
    {
        /// <summary>
        /// Given definitions.
        /// </summary>
        private readonly IList<String> _givenDefinitions = new List<String>();

        /// <summary>
        /// The table of probabilities.
        /// </summary>
        private double[] _table;

        /// <summary>
        /// The "for" definition.
        /// </summary>
        public String ForDefinition { get; set; }

        /// <summary>
        /// The table of probabilities.
        /// </summary>
        public double[] Table
        {
            get { return _table; }
        }

        /// <summary>
        /// The given defintions.
        /// </summary>
        public IList<String> GivenDefinitions
        {
            get { return _givenDefinitions; }
        }

        /// <summary>
        /// Set the probabilities as a string.
        /// </summary>
        /// <param name="s">A space separated string.</param>
        public void SetTable(String s)
        {
            // parse a space separated list of numbers
            String[] tok = s.Split(' ');
            IList<Double> list = new List<Double>();
            foreach (String str in tok)
            {
                // support both radix formats
                if (str.IndexOf(",") != -1)
                {
                    list.Add(CSVFormat.DecimalComma.Parse(str));
                }
                else
                {
                    list.Add(CSVFormat.DecimalComma.Parse(str));
                }
            }

            // now copy to regular array
            _table = new double[list.Count];
            for (int i = 0; i < _table.Length; i++)
            {
                _table[i] = list[i];
            }
        }

        /// <summary>
        /// Add a given.
        /// </summary>
        /// <param name="s">The given to add.</param>
        public void AddGiven(String s)
        {
            _givenDefinitions.Add(s);
        }
    }

    public class BIFVariable
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Options for this variable.
        /// </summary>
        private IList<String> Options { get; set; }

        /// <summary>
        /// Construct the variable.
        /// </summary>
        public BIFVariable()
        {
            Options = new List<String>();
        }


        /// <summary>
        /// Add an option to the variable.
        /// </summary>
        /// <param name="s">The option to add.</param>
        public void AddOption(String s)
        {
            Options.Add(s);
        }
    }

    public class ParsedChoice
    {
        /// <summary>
        /// The label for this choice.
        /// </summary>
        private readonly String _label;

        /// <summary>
        /// The max value for this choice.
        /// </summary>
        private readonly double _max;

        /// <summary>
        /// The min value for this choice.
        /// </summary>
        private readonly double _min;

        /// <summary>
        /// Construct a continuous choice, with a min and max. 
        /// </summary>
        /// <param name="label">The label, for this chocie.</param>
        /// <param name="min">The min value, for this choice.</param>
        /// <param name="max">The max value, for this choice.</param>
        public ParsedChoice(String label, double min, double max)
        {
            _label = label;
            _min = min;
            _max = max;
        }

        /// <summary>
        /// Construct a discrete value for this choice.
        /// </summary>
        /// <param name="label">The choice label.</param>
        /// <param name="index">The index.</param>
        public ParsedChoice(String label, int index)
        {
            _label = label;
            _min = index;
            _max = index;
        }

        /// <summary>
        /// The label.
        /// </summary>
        public String Label
        {
            get { return _label; }
        }

        /// <summary>
        /// The min value.
        /// </summary>
        public double Min
        {
            get { return _min; }
        }

        /// <summary>
        /// The max value.
        /// </summary>
        public double Max
        {
            get { return _max; }
        }

        /// <summary>
        /// True, if this choice is indexed, or discrete.
        /// </summary>
        public bool IsIndex
        {
            get { return Math.Abs(_min - _max) < SyntFramework.DefaultDoubleEqual; }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            return _label;
        }
    }

    public class ParsedEvent
    {
        /// <summary>
        /// The event label.
        /// </summary>
        private readonly String label;

        /// <summary>
        /// The event value.
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// The choices.
        /// </summary>
        private readonly IList<ParsedChoice> list = new List<ParsedChoice>();

        /// <summary>
        /// Construct a parsed even with the specified label.
        /// </summary>
        /// <param name="theLabel">The label.</param>
        public ParsedEvent(String theLabel)
        {
            this.label = theLabel;
        }

        /// <summary>
        /// The label for this event.
        /// </summary>
        public String Label
        {
            get
            {
                return label;
            }
        }

        /// <summary>
        /// Resolve the event to an actual value.
        /// </summary>
        /// <param name="actualEvent">The actual event.</param>
        /// <returns>The value.</returns>
        public int ResolveValue(BayesianEvent actualEvent)
        {
            int result = 0;

            if (this.Value == null)
            {
                throw new BayesianError("Value is undefined for " + this.label + " should express a value with +, - or =.");
            }

            foreach (BayesianChoice choice in actualEvent.Choices)
            {
                if (this.Value.Equals(choice.Label))
                {
                    return result;
                }
                result++;
            }

            // resolve true/false if not found, probably came from +/- notation
            if (String.Compare(Value, "true", true) == 0)
            {
                return 0;
            }
            else if (String.Compare(Value, "false", true) == 0)
            {
                return 1;
            }

            // try to resolve numeric index
            try
            {
                int i = int.Parse(this.Value);
                if (i < actualEvent.Choices.Count)
                {
                    return i;
                }
            }
            catch (FormatException ex)
            {
                // well, we tried
            }

            // error out if nothing found
            throw new BayesianError("Can'f find choice " + this.Value + " in the event " + this.label);
        }


        /// <summary>
        /// A list of choices.
        /// </summary>
        public IList<ParsedChoice> ChoiceList
        {
            get
            {
                return list;
            }
        }

        /// <inheritdoc/>
        public String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[ParsedEvent:label=");
            result.Append(this.label);
            result.Append(",value=");
            result.Append(this.Value);
            result.Append("]");
            return result.ToString();
        }

    }

    public class ParsedProbability
    {
        /// <summary>
        /// The base events.
        /// </summary>
        private readonly IList<ParsedEvent> baseEvents = new List<ParsedEvent>();

        /// <summary>
        /// The given events.
        /// </summary>
        private readonly IList<ParsedEvent> givenEvents = new List<ParsedEvent>();

        /// <summary>
        /// Add a given event.
        /// </summary>
        /// <param name="theEvent">The event to add.</param>
        public void AddGivenEvent(ParsedEvent theEvent)
        {
            this.givenEvents.Add(theEvent);
        }


        /// <summary>
        /// Add a base event.
        /// </summary>
        /// <param name="theEvent"The base event to add.></param>
        public void AddBaseEvent(ParsedEvent theEvent)
        {
            this.baseEvents.Add(theEvent);
        }

        /// <summary>
        /// Get the arguments to this event.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <returns>The arguments.</returns>
        public int[] GetArgs(BayesianNetwork network)
        {
            int[] result = new int[givenEvents.Count];

            for (int i = 0; i < givenEvents.Count; i++)
            {
                ParsedEvent givenEvent = this.givenEvents[i];
                BayesianEvent actualEvent = network.GetEvent(givenEvent.Label);
                result[i] = givenEvent.ResolveValue(actualEvent);
            }

            return result;
        }

        /// <summary>
        /// The child events.
        /// </summary>
        public ParsedEvent ChildEvent
        {
            get
            {
                if (this.baseEvents.Count > 1)
                {
                    throw new BayesianError("Only one base event may be used to define a probability, i.e. P(a), not P(a,b).");
                }

                if (this.baseEvents.Count == 0)
                {
                    throw new BayesianError("At least one event must be provided, i.e. P() or P(|a,b,c) is not allowed.");
                }

                return this.baseEvents[0];
            }
        }

        /// <summary>
        /// Define the truth table. 
        /// </summary>
        /// <param name="network">The bayesian network.</param>
        /// <param name="result">The resulting probability.</param>
        public void DefineTruthTable(BayesianNetwork network, double result)
        {

            ParsedEvent childParsed = ChildEvent;
            BayesianEvent childEvent = network.RequireEvent(childParsed.Label);

            // define truth table line
            int[] args = GetArgs(network);
            childEvent.Table.AddLine(result, childParsed.ResolveValue(childEvent), args);

        }

        /// <summary>
        /// The base events.
        /// </summary>
        public IList<ParsedEvent> BaseEvents
        {
            get
            {
                return baseEvents;
            }
        }

        /// <summary>
        /// The given events.
        /// </summary>
        public IList<ParsedEvent> GivenEvents
        {
            get
            {
                return givenEvents;
            }
        }

        /// <summary>
        /// Define the relationships.
        /// </summary>
        /// <param name="network">The network.</param>
        public void DefineRelationships(BayesianNetwork network)
        {
            // define event relations, if they are not there already
            ParsedEvent childParsed = ChildEvent;
            BayesianEvent childEvent = network.RequireEvent(childParsed.Label);
            foreach (ParsedEvent e in this.givenEvents)
            {
                BayesianEvent parentEvent = network.RequireEvent(e.Label);
                network.CreateDependency(parentEvent, childEvent);
            }

        }

        /// <inheritdoc/>
        public String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[ParsedProbability:baseEvents=");
            result.Append(this.baseEvents.ToString());
            result.Append(",givenEvents=");
            result.Append(this.givenEvents.ToString());
            result.Append("]");
            return result.ToString();
        }

    }

    public class ParseProbability
    {
        /// <summary>
        /// The network used.
        /// </summary>
        private readonly BayesianNetwork network;

        /// <summary>
        /// Parse the probability for the specified network. 
        /// </summary>
        /// <param name="theNetwork">The network to parse for.</param>
        public ParseProbability(BayesianNetwork theNetwork)
        {
            this.network = theNetwork;
        }

        /// <summary>
        /// Add events, as they are pased.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="results">The events found.</param>
        /// <param name="delim">The delimiter to use.</param>
        private void AddEvents(SimpleParser parser, IList<ParsedEvent> results, String delim)
        {
            bool done = false;
            StringBuilder l = new StringBuilder();

            while (!done && !parser.EOL())
            {
                char ch = parser.Peek();
                if (delim.IndexOf(ch) != -1)
                {
                    if (ch == ')' || ch == '|')
                        done = true;

                    ParsedEvent parsedEvent;

                    // deal with a value specified by + or -
                    if (l.Length > 0 && l[0] == '+')
                    {
                        String l2 = l.ToString().Substring(1);
                        parsedEvent = new ParsedEvent(l2.Trim());
                        parsedEvent.Value = "true";
                    }
                    else if (l.Length > 0 && l[0] == '-')
                    {
                        String l2 = l.ToString().Substring(1);
                        parsedEvent = new ParsedEvent(l2.Trim());
                        parsedEvent.Value = "false";
                    }
                    else
                    {
                        String l2 = l.ToString();
                        parsedEvent = new ParsedEvent(l2.Trim());
                    }

                    // parse choices
                    if (ch == '[')
                    {
                        parser.Advance();
                        int index = 0;
                        while (ch != ']' && !parser.EOL())
                        {

                            String labelName = parser.ReadToChars(":,]");
                            if (parser.Peek() == ':')
                            {
                                parser.Advance();
                                parser.EatWhiteSpace();
                                double min = double.Parse(parser.ReadToWhiteSpace());
                                parser.EatWhiteSpace();
                                if (!parser.LookAhead("to", true))
                                {
                                    throw new BayesianError("Expected \"to\" in probability choice range.");
                                }
                                parser.Advance(2);
                                double max = CSVFormat.EgFormat.Parse(parser.ReadToChars(",]"));
                                parsedEvent.ChoiceList.Add(new ParsedChoice(labelName, min, max));

                            }
                            else
                            {
                                parsedEvent.ChoiceList.Add(new ParsedChoice(labelName, index++));
                            }
                            parser.EatWhiteSpace();
                            ch = parser.Peek();

                            if (ch == ',')
                            {
                                parser.Advance();
                            }
                        }
                    }

                    // deal with a value specified by =
                    if (parser.Peek() == '=')
                    {
                        parser.ReadChar();
                        String value = parser.ReadToChars(delim);
                        //					BayesianEvent evt = this.network.getEvent(parsedEvent.getLabel());
                        parsedEvent.Value = value;
                    }

                    if (ch == ',')
                    {
                        parser.Advance();
                    }

                    if (ch == ']')
                    {
                        parser.Advance();
                    }

                    if (parsedEvent.Label.Length > 0)
                    {
                        results.Add(parsedEvent);
                    }
                    l.Length = 0;
                }
                else
                {
                    parser.Advance();
                    l.Append(ch);
                }
            }

        }

        /// <summary>
        /// Parse the given line.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The parsed probability.</returns>
        public ParsedProbability Parse(String line)
        {

            ParsedProbability result = new ParsedProbability();

            SimpleParser parser = new SimpleParser(line);
            parser.EatWhiteSpace();
            if (!parser.LookAhead("P(", true))
            {
                throw new SyntError("Bayes table lines must start with P(");
            }
            parser.Advance(2);

            // handle base
            AddEvents(parser, result.BaseEvents, "|,)=[]");

            // handle conditions
            if (parser.Peek() == '|')
            {
                parser.Advance();
                AddEvents(parser, result.GivenEvents, ",)=[]");

            }

            if (parser.Peek() != ')')
            {
                throw new BayesianError("Probability not properly terminated.");
            }

            return result;

        }

        /// <summary>
        /// Parse a probability list. 
        /// </summary>
        /// <param name="network">The network to parse for.</param>
        /// <param name="line">The line to parse.</param>
        /// <returns>The parsed list.</returns>
        public static IList<ParsedProbability> ParseProbabilityList(BayesianNetwork network, String line)
        {
            IList<ParsedProbability> result = new List<ParsedProbability>();

            StringBuilder prob = new StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if (ch == ')')
                {
                    prob.Append(ch);
                    ParseProbability parse = new ParseProbability(network);
                    ParsedProbability parsedProbability = parse.Parse(prob.ToString());
                    result.Add(parsedProbability);
                    prob.Length = 0;
                }
                else
                {
                    prob.Append(ch);
                }
            }
            return result;
        }
    }

    [Serializable]
    public class EnumerationQuery : BasicQuery
    {
        /// <summary>
        /// The events that we will enumerate over.
        /// </summary>
        private readonly IList<EventState> _enumerationEvents = new List<EventState>();

        /// <summary>
        /// The calculated probability.
        /// </summary>
        private double _probability;

        /// <summary>
        /// Construct the enumeration query.
        /// </summary>
        /// <param name="theNetwork">The Bayesian network to query.</param>
        public EnumerationQuery(BayesianNetwork theNetwork)
            : base(theNetwork)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EnumerationQuery()
        {
        }

        /// <inheritdoc/>
        public override double Probability
        {
            get { return _probability; }
        }

        /// <summary>
        /// Reset the enumeration events. Always reset the hidden events. Optionally
        /// reset the evidence and outcome.
        /// </summary>
        /// <param name="includeEvidence">True if the evidence is to be reset.</param>
        /// <param name="includeOutcome">True if the outcome is to be reset.</param>
        public void ResetEnumeration(bool includeEvidence, bool includeOutcome)
        {
            _enumerationEvents.Clear();

            foreach (EventState state in Events.Values)
            {
                if (state.CurrentEventType == EventType.Hidden)
                {
                    _enumerationEvents.Add(state);
                    state.Value = 0;
                }
                else if (includeEvidence
                         && state.CurrentEventType == EventType.Evidence)
                {
                    _enumerationEvents.Add(state);
                    state.Value = 0;
                }
                else if (includeOutcome
                         && state.CurrentEventType == EventType.Outcome)
                {
                    _enumerationEvents.Add(state);
                    state.Value = 0;
                }
                else
                {
                    state.Value = state.CompareValue;
                }
            }
        }

        /// <summary>
        /// Roll the enumeration events forward by one.
        /// </summary>
        /// <returns>False if there are no more values to roll into, which means we're
        /// done.</returns>
        public bool Forward()
        {
            int currentIndex = 0;
            bool done = false;
            bool eof = false;

            if (_enumerationEvents.Count == 0)
            {
                done = true;
                eof = true;
            }

            while (!done)
            {
                EventState state = _enumerationEvents[currentIndex];
                int v = state.Value;
                v++;
                if (v >= state.Event.Choices.Count)
                {
                    state.Value = 0;
                }
                else
                {
                    state.Value = v;
                    break;
                }

                currentIndex++;

                if (currentIndex >= _enumerationEvents.Count)
                {
                    done = true;
                    eof = true;
                }
            }

            return !eof;
        }

        /// <summary>
        /// Obtain the arguments for an event.
        /// </summary>
        /// <param name="theEvent">The event.</param>
        /// <returns>The arguments.</returns>
        private int[] ObtainArgs(BayesianEvent theEvent)
        {
            var result = new int[theEvent.Parents.Count];

            int index = 0;
            foreach (BayesianEvent parentEvent in theEvent.Parents)
            {
                EventState state = GetEventState(parentEvent);
                result[index++] = state.Value;
            }
            return result;
        }

        /// <summary>
        /// Calculate the probability for a state.
        /// </summary>
        /// <param name="state">The state to calculate.</param>
        /// <returns>The probability.</returns>
        private double CalculateProbability(EventState state)
        {
            int[] args = ObtainArgs(state.Event);

            foreach (TableLine line in state.Event.Table.Lines)
            {
                if (line.CompareArgs(args))
                {
                    if (Math.Abs(line.Result - state.Value) < SyntFramework.DefaultDoubleEqual)
                    {
                        return line.Probability;
                    }
                }
            }

            throw new BayesianError("Could not determine the probability for "
                                    + state);
        }

        /// <summary>
        /// Perform a single enumeration. 
        /// </summary>
        /// <returns>The result.</returns>
        private double PerformEnumeration()
        {
            double result = 0;

            do
            {
                bool first = true;
                double prob = 0;
                foreach (EventState state in Events.Values)
                {
                    if (first)
                    {
                        prob = CalculateProbability(state);
                        first = false;
                    }
                    else
                    {
                        prob *= CalculateProbability(state);
                    }
                }
                result += prob;
            } while (Forward());
            return result;
        }

        /// <inheritdoc/>
        public override void Execute()
        {
            LocateEventTypes();
            ResetEnumeration(false, false);
            double numerator = PerformEnumeration();
            ResetEnumeration(false, true);
            double denominator = PerformEnumeration();
            _probability = numerator / denominator;
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[SamplingQuery: ");
            result.Append(Problem);
            result.Append("=");
            result.Append(Format.FormatPercent(Probability));
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// Roll the enumeration events forward by one.
        /// </summary>
        /// <param name="enumerationEvents">The events to roll.</param>
        /// <param name="args">The arguments to roll.</param>
        /// <returns>False if there are no more values to roll into, which means we're
        ///         done.</returns>
        public static bool Roll(IList<BayesianEvent> enumerationEvents, int[] args)
        {
            int currentIndex = 0;
            bool done = false;
            bool eof = false;

            if (enumerationEvents.Count == 0)
            {
                done = true;
                eof = true;
            }

            while (!done)
            {
                BayesianEvent e = enumerationEvents[currentIndex];
                int v = args[currentIndex];
                v++;
                if (v >= e.Choices.Count)
                {
                    args[currentIndex] = 0;
                }
                else
                {
                    args[currentIndex] = v;
                    break;
                }

                currentIndex++;

                if (currentIndex >= args.Length)
                {
                    done = true;
                    eof = true;
                }
            }

            return !eof;
        }

        /// <summary>
        /// A clone of this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override IBayesianQuery Clone()
        {
            return new EnumerationQuery(Network);
        }
    }

    [Serializable]
    public class SamplingQuery : BasicQuery
    {
        /// <summary>
        /// The default sample size.
        /// </summary>
        public const int DefaultSampleSize = 100000;

        /// <summary>
        /// The number of samples that matched the result the query is looking for.
        /// </summary>
        private int _goodSamples;

        /// <summary>
        /// The total number of samples generated. This should match sampleSize at
        /// the end of a query.
        /// </summary>
        private int _totalSamples;

        /// <summary>
        /// The number of usable samples. This is the set size for the average
        /// probability.
        /// </summary>
        private int _usableSamples;

        /// <summary>
        /// Construct a sampling query. 
        /// </summary>
        /// <param name="theNetwork">The network that will be queried.</param>
        public SamplingQuery(BayesianNetwork theNetwork)
            : base(theNetwork)
        {
            SampleSize = DefaultSampleSize;
        }

        /// <summary>
        /// The sample size.
        /// </summary>
        public int SampleSize { get; set; }

        /// <inheritdoc/>
        public override double Probability
        {
            get { return _goodSamples / (double)_usableSamples; }
        }

        /// <summary>
        /// Obtain the arguments for an event. 
        /// </summary>
        /// <param name="e">The event.</param>
        /// <returns>The arguments for that event, based on the other event values.</returns>
        private int[] ObtainArgs(BayesianEvent e)
        {
            var result = new int[e.Parents.Count];

            int index = 0;
            foreach (BayesianEvent parentEvent in e.Parents)
            {
                EventState state = GetEventState(parentEvent);
                if (!state.IsCalculated)
                    return null;
                result[index++] = state.Value;
            }
            return result;
        }

        /// <summary>
        /// Set all events to random values, based on their probabilities. 
        /// </summary>
        /// <param name="eventState">The event state.</param>
        private void RandomizeEvents(EventState eventState)
        {
            // first, has this event already been randomized
            if (!eventState.IsCalculated)
            {
                // next, see if we can randomize the event passed
                int[] args = ObtainArgs(eventState.Event);
                if (args != null)
                {
                    eventState.Randomize(args);
                }
            }

            // randomize children
            foreach (BayesianEvent childEvent in eventState.Event.Children)
            {
                RandomizeEvents(GetEventState(childEvent));
            }
        }

        /// <summary>
        /// The number of events that are still uncalculated.
        /// </summary>
        /// <returns>The uncalculated count.</returns>
        private int CountUnCalculated()
        {
            return Events.Values.Count(state => !state.IsCalculated);
        }

        /// <inheritdoc/>
        public override void Execute()
        {
            LocateEventTypes();
            _usableSamples = 0;
            _goodSamples = 0;
            _totalSamples = 0;

            for (int i = 0; i < SampleSize; i++)
            {
                Reset();

                int lastUncalculated = int.MaxValue;
                int uncalculated;
                do
                {
                    foreach (EventState state in Events.Values)
                    {
                        RandomizeEvents(state);
                    }
                    uncalculated = CountUnCalculated();
                    if (uncalculated == lastUncalculated)
                    {
                        throw new BayesianError(
                            "Unable to calculate all nodes in the graph.");
                    }
                    lastUncalculated = uncalculated;
                } while (uncalculated > 0);

                // System.out.println("Sample:\n" + this.dumpCurrentState());
                _totalSamples++;
                if (IsNeededEvidence)
                {
                    _usableSamples++;
                    if (SatisfiesDesiredOutcome)
                    {
                        _goodSamples++;
                    }
                }
            }
        }

        /// <summary>
        /// The current state as a string.
        /// </summary>
        /// <returns>The state.</returns>
        public String DumpCurrentState()
        {
            var result = new StringBuilder();
            foreach (EventState state in Events.Values)
            {
                result.Append(state.ToString());
                result.Append("\n");
            }
            return result.ToString();
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns></returns>
        public override IBayesianQuery Clone()
        {
            return new SamplingQuery(Network);
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[SamplingQuery: ");
            result.Append(Problem);
            result.Append("=");
            result.Append(Format.FormatPercent(Probability));
            result.Append(" ;good/usable=");
            result.Append(Format.FormatInteger(_goodSamples));
            result.Append("/");
            result.Append(Format.FormatInteger(_usableSamples));
            result.Append(";totalSamples=");
            result.Append(Format.FormatInteger(_totalSamples));
            return result.ToString();
        }
    }

    [Serializable]
    public class EventState
    {
        /// <summary>
        /// The event that this state is connected to.
        /// </summary>
        private readonly BayesianEvent _event;

        /// <summary>
        /// The current value of this event.
        /// </summary>
        private int _value;

        /// <summary>
        /// Construct an event state for the specified event. 
        /// </summary>
        /// <param name="theEvent">The event to create a state for.</param>
        public EventState(BayesianEvent theEvent)
        {
            _event = theEvent;
            CurrentEventType = EventType.Hidden;
            IsCalculated = false;
        }

        /// <summary>
        /// Has this event been calculated yet?
        /// </summary>
        public bool IsCalculated { get; set; }

        /// <summary>
        /// The type of event that this is for the query.
        /// </summary>
        public EventType CurrentEventType { get; set; }

        /// <summary>
        /// The value that we are comparing to, for probability.
        /// </summary>
        public int CompareValue { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                IsCalculated = true;
                _value = value;
            }
        }


        /// <summary>
        /// The event.
        /// </summary>
        public BayesianEvent Event
        {
            get { return _event; }
        }


        /// <summary>
        /// Is this event satisified.
        /// </summary>
        public bool IsSatisfied
        {
            get
            {
                if (CurrentEventType == EventType.Hidden)
                {
                    throw new BayesianError(
                        "Satisfy can't be called on a hidden event.");
                }
                return Math.Abs(CompareValue - _value) < SyntFramework.DefaultDoubleEqual;
            }
        }

        /// <summary>
        /// Randomize according to the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Randomize(params int[] args)
        {
            Value = _event.Table.GenerateRandom(args);
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[EventState:event=");
            result.Append(_event.ToString());
            result.Append(",type=");
            result.Append(CurrentEventType.ToString());
            result.Append(",value=");
            result.Append(Format.FormatDouble(_value, 2));
            result.Append(",compare=");
            result.Append(Format.FormatDouble(CompareValue, 2));
            result.Append(",calc=");
            result.Append(IsCalculated ? "y" : "n");
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// Convert a state to a simple string. (probability expression) 
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>A probability expression as a string.</returns>
        public static String ToSimpleString(EventState state)
        {
            return BayesianEvent.FormatEventName(state.Event, state.CompareValue);
        }
    }

    [Serializable]
    public class BayesianTable
    {
        /// <summary>
        /// The event that owns this truth table.
        /// </summary>
        private readonly BayesianEvent _event;

        /// <summary>
        /// The lines of the truth table.
        /// </summary>
        private readonly IList<TableLine> _lines = new List<TableLine>();

        /// <summary>
        /// Construct a Bayes truth table.
        /// </summary>
        /// <param name="theEvent">The lines of the truth table.</param>
        public BayesianTable(BayesianEvent theEvent)
        {
            _event = theEvent;
            Reset();
        }

        /// <summary>
        /// Reset the truth table to zero.
        /// </summary>
        public void Reset()
        {
            _lines.Clear();
            IList<BayesianEvent> parents = _event.Parents;
            int l = parents.Count;

            int[] args = new int[l];

            do
            {
                for (int k = 0; k < _event.Choices.Count; k++)
                {
                    AddLine(0, k, args);
                }
            } while (EnumerationQuery.Roll(parents, args));
        }

        /// <summary>
        /// Add a new line.
        /// </summary>
        /// <param name="prob">The probability.</param>
        /// <param name="result">The resulting probability.</param>
        /// <param name="args">The arguments.</param>
        public void AddLine(double prob, bool result, params bool[] args)
        {
            int[] d = new int[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                d[i] = args[i] ? 0 : 1;
            }

            AddLine(prob, result ? 0 : 1, d);
            AddLine(1.0 - prob, result ? 1 : 0, d);
        }


        /// <summary>
        /// Add a new line.
        /// </summary>
        /// <param name="prob">The probability.</param>
        /// <param name="result">The resulting probability.</param>
        /// <param name="args">The arguments.</param>
        public void AddLine(double prob, int result, params bool[] args)
        {
            int[] d = new int[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                d[i] = args[i] ? 0 : 1;
            }

            AddLine(prob, result, d);
        }


        /// <summary>
        /// Add a new line.
        /// </summary>
        /// <param name="prob">The probability.</param>
        /// <param name="result">The resulting probability.</param>
        /// <param name="args">The arguments.</param>
        public void AddLine(double prob, int result, params int[] args)
        {
            if (args.Length != _event.Parents.Count)
            {
                throw new BayesianError("Truth table line with " + args.Length
                        + ", specified for event with "
                        + _event.Parents.Count
                        + " parents.  These numbers must be the same");
            }

            TableLine line = FindLine(result, args);

            if (line == null)
            {
                if (_lines.Count == this.MaxLines)
                {
                    throw new BayesianError("This truth table is already full.");
                }

                line = new TableLine(prob, result, args);
                _lines.Add(line);
            }
            else
            {
                line.Probability = prob;
            }
        }

        /// <summary>
        /// Validate the truth table.
        /// </summary>
        public void Validate()
        {
            if (_lines.Count != this.MaxLines)
            {
                throw new BayesianError("Truth table for " + _event.ToString()
                        + " only has " + _lines.Count
                        + " line(s), should have " + this.MaxLines
                        + " line(s).");
            }

        }

        /// <summary>
        /// Generate a random sampling based on this truth table.
        /// </summary>
        /// <param name="args">The arguemtns.</param>
        /// <returns>The result.</returns>
        public int GenerateRandom(params int[] args)
        {
            double r = ThreadSafeRandom.NextDouble();
            double limit = 0;

            foreach (TableLine line in _lines)
            {
                if (line != null && line.CompareArgs(args))
                {
                    limit += line.Probability;
                    if (r < limit)
                    {
                        return line.Result;
                    }
                }
            }

            throw new BayesianError("Incomplete logic table for event: "
                    + _event.ToString());
        }

        /// <inheritdoc/>
        public String ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (TableLine line in _lines)
            {
                result.Append(line.ToString());
                result.Append("\n");
            }
            return result.ToString();
        }

        /// <summary>
        /// The lines of this truth table.
        /// </summary>
        public IList<TableLine> Lines
        {
            get
            {
                return _lines;
            }
        }

        /// <summary>
        /// Find the specified truth table line.
        /// </summary>
        /// <param name="result">The result sought.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The line that matches.</returns>
        public TableLine FindLine(int result, int[] args)
        {

            foreach (TableLine line in _lines)
            {
                if (line != null && line.CompareArgs(args))
                {
                    if (Math.Abs(line.Result - result) < SyntFramework.DefaultDoubleEqual)
                    {
                        return line;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// The maximum number of lines this truth table would have.
        /// </summary>
        public int MaxLines
        {
            get
            {
                return _event.CalculateParameterCount() * _event.Choices.Count;
            }
        }
    }

    [Serializable]
    public class TableLine
    {
        /// <summary>
        /// The arguments.
        /// </summary>
        private readonly int[] _arguments;

        /// <summary>
        /// The result.
        /// </summary>
        private readonly int _result;

        /// <summary>
        /// Construct a truth table line. 
        /// </summary>
        /// <param name="prob">The probability.</param>
        /// <param name="result">The result.</param>
        /// <param name="args">The arguments.</param>
        public TableLine(double prob, int result, int[] args)
        {
            Probability = prob;
            _result = result;
            _arguments = EngineArray.ArrayCopy(args);
        }

        /// <summary>
        /// The probability.
        /// </summary>
        public double Probability { get; set; }


        /// <summary>
        /// Arguments.
        /// </summary>
        public int[] Arguments
        {
            get { return _arguments; }
        }

        /// <summary>
        /// Result.
        /// </summary>
        public int Result
        {
            get { return _result; }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var r = new StringBuilder();
            r.Append("result=");
            r.Append(_result);
            r.Append(",probability=");
            r.Append(Format.FormatDouble(Probability, 2));
            r.Append("|");
            foreach (int t in _arguments)
            {
                r.Append(Format.FormatDouble(t, 2));
                r.Append(" ");
            }
            return r.ToString();
        }

        /// <summary>
        /// Compare this truth line's arguments to others. 
        /// </summary>
        /// <param name="args">The other arguments to compare to.</param>
        /// <returns>True if the same.</returns>
        public bool CompareArgs(int[] args)
        {
            if (args.Length != _arguments.Length)
            {
                return false;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (Math.Abs(_arguments[i] - args[i]) > SyntFramework.DefaultDoubleEqual)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class EstimatorNone : IBayesEstimator
    {
        /// <inheritdoc/>
        public void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork, IMLDataSet theData)
        {
        }

       

        /// <inheritdoc/>
        public bool Iteration()
        {
            return false;
        }
    }

    public class SimpleEstimator : IBayesEstimator
    {
        private IMLDataSet _data;
        private int _index;
        private BayesianNetwork _network;

        #region IBayesEstimator Members

        /// <inheritdoc/>
        public void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork, IMLDataSet theData)
        {
            _network = theNetwork;
            _data = theData;
            _index = 0;
        }


        /// <inheritdoc/>
        public bool Iteration()
        {
            BayesianEvent e = _network.Events[_index];
            foreach (TableLine line in e.Table.Lines)
            {
                line.Probability = (CalculateProbability(e, line.Result, line.Arguments));
            }
            _index++;

            return _index < _network.Events.Count;
        }

        #endregion

        /// <summary>
        /// Calculate the probability.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <param name="result">The result.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The probability.</returns>
        public double CalculateProbability(BayesianEvent e, int result, int[] args)
        {
            int eventIndex = _network.Events.IndexOf(e);
            int x = 0;
            int y = 0;

            // calculate overall probability
            foreach (IMLDataPair pair in _data)
            {
                int[] d = _network.DetermineClasses(pair.Input);

                if (args.Length == 0)
                {
                    x++;
                    if (d[eventIndex] == result)
                    {
                        y++;
                    }
                }
                else if (d[eventIndex] == result)
                {
                    x++;

                    int i = 0;
                    bool givenMatch = true;
                    foreach (BayesianEvent givenEvent in e.Parents)
                    {
                        int givenIndex = _network.GetEventIndex(givenEvent);
                        if (args[i] != d[givenIndex])
                        {
                            givenMatch = false;
                            break;
                        }
                        i++;
                    }

                    if (givenMatch)
                    {
                        y++;
                    }
                }
            }

            double num = y + 1;
            double den = x + e.Choices.Count;


            return num / den;
        }
    }

    public class SearchK2 : IBayesSearch
    {
        /// <summary>
        /// The node ordering.
        /// </summary>
        private readonly IList<BayesianEvent> _nodeOrdering = new List<BayesianEvent>();

        /// <summary>
        /// The data to use.
        /// </summary>
        private IMLDataSet _data;

        /// <summary>
        /// The current index.
        /// </summary>
        private int _index = -1;

        /// <summary>
        /// The last calculated value for p.
        /// </summary>
        private double _lastCalculatedP;

        /// <summary>
        /// The network to optimize.
        /// </summary>
        private BayesianNetwork _network;

        /// <summary>
        /// The trainer being used.
        /// </summary>
        private TrainBayesian _train;

        #region IBayesSearch Members

        /// <inheritdoc/>
        public void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork, IMLDataSet theData)
        {
            _network = theNetwork;
            _data = theData;
            _train = theTrainer;
            OrderNodes();
            _index = -1;
        }

        /// <inheritdoc/>
        public bool Iteration()
        {
            if (_index == -1)
            {
                OrderNodes();
            }
            else
            {
                BayesianEvent e = _nodeOrdering[_index];
                double oldP = CalculateG(_network, e, e.Parents);

                while (e.Parents.Count < _train.MaximumParents)
                {
                    BayesianEvent z = FindZ(e, _index, oldP);
                    if (z != null)
                    {
                        _network.CreateDependency(z, e);
                        oldP = _lastCalculatedP;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            _index++;
            return (_index < _data.InputSize);
        }

        #endregion

        /// <summary>
        /// Basically the goal here is to get the classification target, if it exists,
        /// to go first. This will greatly enhance K2's effectiveness.
        /// </summary>
        private void OrderNodes()
        {
            _nodeOrdering.Clear();

            // is there a classification target?
            if (_network.ClassificationTarget != -1)
            {
                _nodeOrdering.Add(_network.ClassificationTargetEvent);
            }


            // now add the others
            foreach (BayesianEvent e in _network.Events)
            {
                if (!_nodeOrdering.Contains(e))
                {
                    _nodeOrdering.Add(e);
                }
            }
        }

        /// <summary>
        /// Find the value for z.
        /// </summary>
        /// <param name="e">The event that we are clauclating for.</param>
        /// <param name="n">The value for n.</param>
        /// <param name="old">The old value.</param>
        /// <returns>The new value for z.</returns>
        private BayesianEvent FindZ(BayesianEvent e, int n, double old)
        {
            BayesianEvent result = null;
            double maxChildP = double.NegativeInfinity;
            //System.out.println("Finding parent for: " + event.toString());
            for (int i = 0; i < n; i++)
            {
                BayesianEvent trialParent = _nodeOrdering[i];
                IList<BayesianEvent> parents = new List<BayesianEvent>();
                parents.CopyTo(e.Parents.ToArray(), 0);
                parents.Add(trialParent);
                //System.out.println("Calculating adding " + trialParent.toString() + " to " + event.toString());
                _lastCalculatedP = CalculateG(_network, e, parents);
                //System.out.println("lastP:" + this.lastCalculatedP);
                //System.out.println("old:" + old);
                if (_lastCalculatedP > old && _lastCalculatedP > maxChildP)
                {
                    result = trialParent;
                    maxChildP = _lastCalculatedP;
                    //System.out.println("Current best is: " + result.toString());
                }
            }

            _lastCalculatedP = maxChildP;
            return result;
        }


        /// <summary>
        /// Calculate the value N, which is the number of cases, from the training data, where the
        /// desiredValue matches the training data.  Only cases where the parents match the specifed
        /// parent instance are considered.
        /// </summary>
        /// <param name="network">The network to calculate for.</param>
        /// <param name="e">The event we are calculating for. (variable i)</param>
        /// <param name="parents">The parents of the specified event we are considering.</param>
        /// <param name="parentInstance">The parent instance we are looking for.</param>
        /// <param name="desiredValue">The desired value.</param>
        /// <returns>The value N. </returns>
        public int CalculateN(BayesianNetwork network, BayesianEvent e,
                              IList<BayesianEvent> parents, int[] parentInstance, int desiredValue)
        {
            int result = 0;
            int eventIndex = network.GetEventIndex(e);

            foreach (IMLDataPair pair in _data)
            {
                int[] d = _network.DetermineClasses(pair.Input);

                if (d[eventIndex] == desiredValue)
                {
                    bool reject = false;

                    for (int i = 0; i < parentInstance.Length; i++)
                    {
                        BayesianEvent parentEvent = parents[i];
                        int parentIndex = network.GetEventIndex(parentEvent);
                        if (parentInstance[i] != d[parentIndex])
                        {
                            reject = true;
                            break;
                        }
                    }

                    if (!reject)
                    {
                        result++;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Calculate the value N, which is the number of cases, from the training data, where the
        /// desiredValue matches the training data.  Only cases where the parents match the specifed
        /// parent instance are considered.
        /// </summary>
        /// <param name="network">The network to calculate for.</param>
        /// <param name="e">The event we are calculating for. (variable i)</param>
        /// <param name="parents">The parents of the specified event we are considering.</param>
        /// <param name="parentInstance">The parent instance we are looking for.</param>
        /// <returns>The value N. </returns>
        public int CalculateN(BayesianNetwork network, BayesianEvent e,
                              IList<BayesianEvent> parents, int[] parentInstance)
        {
            int result = 0;

            foreach (IMLDataPair pair in _data)
            {
                int[] d = _network.DetermineClasses(pair.Input);

                bool reject = false;

                for (int i = 0; i < parentInstance.Length; i++)
                {
                    BayesianEvent parentEvent = parents[i];
                    int parentIndex = network.GetEventIndex(parentEvent);
                    if (parentInstance[i] != (d[parentIndex]))
                    {
                        reject = true;
                        break;
                    }
                }

                if (!reject)
                {
                    result++;
                }
            }
            return result;
        }


        /// <summary>
        /// Calculate G. 
        /// </summary>
        /// <param name="network">The network to calculate for.</param>
        /// <param name="e">The event to calculate for.</param>
        /// <param name="parents">The parents.</param>
        /// <returns>The value for G.</returns>
        public double CalculateG(BayesianNetwork network,
                                 BayesianEvent e, IList<BayesianEvent> parents)
        {
            double result = 1.0;
            int r = e.Choices.Count;

            var args = new int[parents.Count];

            do
            {
                double n = SyntMath.Factorial(r - 1);
                double d = SyntMath.Factorial(CalculateN(network, e,
                                                          parents, args) + r - 1);
                double p1 = n / d;

                double p2 = 1;
                for (int k = 0; k < e.Choices.Count; k++)
                {
                    p2 *= SyntMath.Factorial(CalculateN(network, e, parents, args, k));
                }

                result *= p1 * p2;
            } while (EnumerationQuery.Roll(parents, args));

            return result;
        }
    }

    public class SearchNone : IBayesSearch
    {
        #region IBayesSearch Members

        /// <inheritdoc/>
        public void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork,
                         IMLDataSet theData)
        {
        }

        /// <inheritdoc/>
        public bool Iteration()
        {
            return false;
        }

        #endregion
    }

    public sealed class TrainBayesian : BasicTraining
    {
        /// <summary>
        /// The data used for training.
        /// </summary>
        private readonly IMLDataSet _data;

        /// <summary>
        /// The method used to estimate the probabilities.
        /// </summary>
        private readonly IBayesEstimator _estimator;

        /// <summary>
        /// The maximum parents a node should have.
        /// </summary>
        private readonly int _maximumParents;

        /// <summary>
        /// The network to train.
        /// </summary>
        private readonly BayesianNetwork _network;

        /// <summary>
        /// The method used to search for the best network structure.
        /// </summary>
        private readonly IBayesSearch _search;

        /// <summary>
        /// Used to hold the query.
        /// </summary>
        private String _holdQuery;

        /// <summary>
        /// The method used to setup the initial Bayesian network.
        /// </summary>
        private BayesianInit _initNetwork = BayesianInit.InitNaiveBayes;

        /// <summary>
        /// The phase that training is currently in.
        /// </summary>
        private Phase _p = Phase.Init;

        /// <summary>
        /// Construct a Bayesian trainer. Use K2 to search, and the SimpleEstimator
        /// to estimate probability.  Init as Naive Bayes
        /// </summary>
        /// <param name="theNetwork">The network to train.</param>
        /// <param name="theData">The data to train.</param>
        /// <param name="theMaximumParents">The max number of parents.</param>
        public TrainBayesian(BayesianNetwork theNetwork, IMLDataSet theData,
                             int theMaximumParents)
            : this(theNetwork, theData, theMaximumParents,
                   BayesianInit.InitNaiveBayes, new SearchK2(),
                   new SimpleEstimator())
        {
        }

        /// <summary>
        /// Construct a Bayesian trainer. 
        /// </summary>
        /// <param name="theNetwork">The network to train.</param>
        /// <param name="theData">The data to train with.</param>
        /// <param name="theMaximumParents">The maximum number of parents.</param>
        /// <param name="theInit">How to init the new Bayes network.</param>
        /// <param name="theSearch">The search method.</param>
        /// <param name="theEstimator">The estimation mehod.</param>
        public TrainBayesian(BayesianNetwork theNetwork, IMLDataSet theData,
                             int theMaximumParents, BayesianInit theInit, IBayesSearch theSearch,
                             IBayesEstimator theEstimator)
            : base(TrainingImplementationType.Iterative)
        {
            _network = theNetwork;
            _data = theData;
            _maximumParents = theMaximumParents;

            _search = theSearch;
            _search.Init(this, theNetwork, theData);

            _estimator = theEstimator;
            _estimator.Init(this, theNetwork, theData);

            _initNetwork = theInit;
            Error = 1.0;
        }

        /// <inheritdoc/>
        public override bool TrainingDone
        {
            get { return base.TrainingDone || _p == Phase.Terminated; }
        }

        /// <inheritdoc/>
        public override bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }

        /// <summary>
        /// Returns the network.
        /// </summary>
        public BayesianNetwork Network
        {
            get { return _network; }
        }

        /// <summary>
        /// The maximum parents a node can have.
        /// </summary>
        public int MaximumParents
        {
            get { return _maximumParents; }
        }

        /// <summary>
        /// The search method.
        /// </summary>
        public IBayesSearch Search
        {
            get { return _search; }
        }

        /// <summary>
        /// The init method.
        /// </summary>
        public BayesianInit InitNetwork
        {
            get { return _initNetwork; }
            set { _initNetwork = value; }
        }

        /// <summary>
        /// Init to Naive Bayes.
        /// </summary>
        private void InitNaiveBayes()
        {
            // clear out anything from before
            _network.RemoveAllRelations();

            // locate the classification target event
            BayesianEvent classificationTarget = _network
                .ClassificationTargetEvent;

            // now link everything to this event
            foreach (BayesianEvent e in _network.Events)
            {
                if (e != classificationTarget)
                {
                    _network.CreateDependency(classificationTarget, e);
                }
            }

            _network.FinalizeStructure();
        }

        /// <summary>
        /// Handle iterations for the Init phase.
        /// </summary>
        private void IterationInit()
        {
            _holdQuery = _network.ClassificationStructure;

            switch (_initNetwork)
            {
                case BayesianInit.InitEmpty:
                    _network.RemoveAllRelations();
                    _network.FinalizeStructure();
                    break;
                case BayesianInit.InitNoChange:
                    break;
                case BayesianInit.InitNaiveBayes:
                    InitNaiveBayes();
                    break;
            }
            _p = Phase.Search;
        }

        /// <summary>
        /// Handle iterations for the Search phase.
        /// </summary>
        private void IterationSearch()
        {
            if (!_search.Iteration())
            {
                _p = Phase.SearchDone;
            }
        }

        /// <summary>
        /// Handle iterations for the Search Done phase.
        /// </summary>
        private void IterationSearchDone()
        {
            _network.FinalizeStructure();
            _network.Reset();
            _p = Phase.Probability;
        }

        /// <summary>
        /// Handle iterations for the Probability phase.
        /// </summary>
        private void IterationProbability()
        {
            if (!_estimator.Iteration())
            {
                _p = Phase.Finish;
            }
        }

        /// <summary>
        /// Handle iterations for the Finish phase.
        /// </summary>
        private void IterationFinish()
        {
            _network.DefineClassificationStructure(_holdQuery);
            Error = _network.CalculateError(_data);
            _p = Phase.Terminated;
        }

        /// <inheritdoc/>
        public override void Iteration()
        {
            PreIteration();

            switch (_p)
            {
                case Phase.Init:
                    IterationInit();
                    break;
                case Phase.Search:
                    IterationSearch();
                    break;
                case Phase.SearchDone:
                    IterationSearchDone();
                    break;
                case Phase.Probability:
                    IterationProbability();
                    break;
                case Phase.Finish:
                    IterationFinish();
                    break;
            }

            PostIteration();
        }

        /// <inheritdoc/>
        public override TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }

        #region Nested type: Phase

        /// <summary>
        /// What phase of training are we in?
        /// </summary>
        private enum Phase
        {
            /// <summary>
            /// Init phase.
            /// </summary>
            Init,
            /// <summary>
            /// Searching for a network structure.
            /// </summary>
            Search,
            /// <summary>
            /// Search complete.
            /// </summary>
            SearchDone,
            /// <summary>
            /// Finding probabilities.
            /// </summary>
            Probability,
            /// <summary>
            /// Finished training.
            /// </summary>
            Finish,
            /// <summary>
            /// Training terminated.
            /// </summary>
            Terminated
        };

        #endregion
    }

    [Serializable]
    public class BayesianChoice : IComparable<BayesianChoice>
    {
        /// <summary>
        /// The label for this choice.
        /// </summary>
        private readonly String _label;

        /// <summary>
        /// The max values, if continuous, or the index if discrete.
        /// </summary>
        private readonly double _max;

        /// <summary>
        /// The min values, if continuous, or the index if discrete.
        /// </summary>
        private readonly double _min;

        /// <summary>
        /// Construct a continuous choice that covers the specified range. 
        /// </summary>
        /// <param name="label">The label for this choice.</param>
        /// <param name="min">The minimum value for this range.</param>
        /// <param name="max">The maximum value for this range.</param>
        public BayesianChoice(String label, double min, double max)
        {
            _label = label;
            _min = min;
            _max = max;
        }

        /// <summary>
        /// Construct a discrete choice for the specified index. 
        /// </summary>
        /// <param name="label">The label for this choice.</param>
        /// <param name="index">The index for this choice.</param>
        public BayesianChoice(String label, int index)
        {
            _label = label;
            _min = index;
            _max = index;
        }

        /// <summary>
        /// Get the label.
        /// </summary>
        public String Label
        {
            get { return _label; }
        }

        /// <summary>
        /// Get the min.
        /// </summary>
        public double Min
        {
            get { return _min; }
        }

        /// <summary>
        /// Get the max.
        /// </summary>
        public double Max
        {
            get { return _max; }
        }

        /// <summary>
        /// True, if this choice has an index, as opposed to min/max. If the
        /// value has an idex, then it is discrete.
        /// </summary>
        public bool IsIndex
        {
            get { return Math.Abs(_min - _max) < SyntFramework.DefaultDoubleEqual; }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            return _label;
        }

        /// <summary>
        /// A string representation of this choice.
        /// </summary>
        /// <returns>A string representation of this choice.</returns>
        public String ToFullString()
        {
            var result = new StringBuilder();
            result.Append(Label);
            if (!IsIndex)
            {
                result.Append(":");
                result.Append(CSVFormat.EgFormat.Format(Min, 4));
                result.Append(" to ");
                result.Append(CSVFormat.EgFormat.Format(Max, 4));
            }
            return result.ToString();
        }

        /// <inheritdoc/>
        public int CompareTo(BayesianChoice other)
        {
            if (_max < other.Max)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    public class BayesianError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public BayesianError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public BayesianError(Exception e)
            : base(e)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="e">The exception.</param>
        public BayesianError(String msg, Exception e)
            : base(msg, e)
        {
        }
    }

    [Serializable]
    public class BayesianEvent
    {
        /// <summary>
        /// The children, or events that use us as a given.
        /// </summary>
        private readonly IList<BayesianEvent> _children = new List<BayesianEvent>();

        /// <summary>
        /// The discrete choices that make up the state of this event.
        /// </summary>
        private readonly ICollection<BayesianChoice> _choices = new SortedSet<BayesianChoice>();

        /// <summary>
        /// The label for this event.
        /// </summary>
        private readonly String _label;

        /// <summary>
        /// The parents, or given.
        /// </summary>
        private readonly IList<BayesianEvent> _parents = new List<BayesianEvent>();

        /// <summary>
        /// The truth table for this event.
        /// </summary>
        private BayesianTable _table;

        /// <summary>
        /// The value of the maximum choice.
        /// </summary>
        private double _maximumChoice;

        /// <summary>
        /// THe value of the minimum choice.
        /// </summary>
        private double _minimumChoice;

        /// <summary>
        /// The index of the minimum choice.
        /// </summary>
        private int _minimumChoiceIndex;

        /// <summary>
        /// Construct an event with the specified label and choices.
        /// </summary>
        /// <param name="theLabel">The label.</param>
        /// <param name="theChoices">The choices, or states.</param>
        public BayesianEvent(String theLabel, IEnumerable<BayesianChoice> theChoices)
        {
            _label = theLabel;
            foreach (BayesianChoice choice in theChoices)
            {
                _choices.Add(choice);
            }
        }

        /// <summary>
        /// Construct an event with the specified label and choices. 
        /// </summary>
        /// <param name="theLabel">The label.</param>
        /// <param name="theChoices">The choices, or states.</param>
        public BayesianEvent(String theLabel, IEnumerable<string> theChoices)
        {
            _label = theLabel;

            int index = 0;
            foreach (String str in theChoices)
            {
                _choices.Add(new BayesianChoice(str, index++));
            }
        }

        /// <summary>
        /// Construct a boolean event.
        /// </summary>
        /// <param name="theLabel">The label.</param>
        public BayesianEvent(String theLabel)
            : this(theLabel, BayesianNetwork.ChoicesTrueFalse)
        {
        }

        /// <summary>
        /// the parents
        /// </summary>
        public IList<BayesianEvent> Parents
        {
            get { return _parents; }
        }

        /// <summary>
        /// the children
        /// </summary>
        public IList<BayesianEvent> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// the label
        /// </summary>
        public String Label
        {
            get { return _label; }
        }

        /// <summary>
        /// True, if this event has parents.
        /// </summary>
        public bool HasParents
        {
            get { return _parents.Count > 0; }
        }

        /// <summary>
        /// True, if this event has parents.
        /// </summary>
        public bool HasChildren
        {
            get { return _parents.Count > 0; }
        }

        /// <summary>
        /// the choices
        /// </summary>
        public ICollection<BayesianChoice> Choices
        {
            get { return _choices; }
        }

        /// <summary>
        /// the table
        /// </summary>
        public BayesianTable Table
        {
            get { return _table; }
        }

        /// <summary>
        /// True, if this is a boolean event.
        /// </summary>
        public bool IsBoolean
        {
            get { return _choices.Count == 2; }
        }

        /// <summary>
        /// Add a child event.
        /// </summary>
        /// <param name="e">The child event.</param>
        public void AddChild(BayesianEvent e)
        {
            _children.Add(e);
        }

        /// <summary>
        /// Add a parent event.
        /// </summary>
        /// <param name="e">The parent event.</param>
        public void AddParent(BayesianEvent e)
        {
            _parents.Add(e);
        }

        /// <summary>
        /// A full string that contains all info for this event.
        /// </summary>
        /// <returns>A full string that contains all info for this event.</returns>
        public String ToFullString()
        {
            var result = new StringBuilder();

            result.Append("P(");
            result.Append(Label);

            result.Append("[");
            bool first = true;
            foreach (BayesianChoice choice in _choices)
            {
                if (!first)
                {
                    result.Append(",");
                }
                result.Append(choice.ToFullString());
                first = false;
            }
            result.Append("]");

            if (HasParents)
            {
                result.Append("|");
            }

            first = true;
            foreach (BayesianEvent e in _parents)
            {
                if (!first)
                    result.Append(",");
                first = false;
                result.Append(e.Label);
            }

            result.Append(")");
            return result.ToString();
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();

            result.Append("P(");
            result.Append(Label);

            if (HasParents)
            {
                result.Append("|");
            }

            bool first = true;
            foreach (BayesianEvent e in _parents)
            {
                if (!first)
                    result.Append(",");
                first = false;
                result.Append(e.Label);
            }

            result.Append(")");
            return result.ToString();
        }

        /// <summary>
        /// The parameter count.
        /// </summary>
        /// <returns>The parameter count.</returns>
        public int CalculateParameterCount()
        {
            int result = _choices.Count - 1;

            return _parents.Aggregate(result, (current, parent) => current * _choices.Count);
        }

        /// <summary>
        /// Finalize the structure.
        /// </summary>
        public void FinalizeStructure()
        {
            // find min/max choice
            _minimumChoiceIndex = -1;
            _minimumChoice = Double.PositiveInfinity;
            _maximumChoice = Double.NegativeInfinity;

            int index = 0;
            foreach (BayesianChoice choice in _choices)
            {
                if (choice.Min < _minimumChoice)
                {
                    _minimumChoice = choice.Min;
                    _minimumChoiceIndex = index;
                }
                if (choice.Max > _maximumChoice)
                {
                    _maximumChoice = choice.Max;
                }
                index++;
            }

            // build truth table
            if (_table == null)
            {
                _table = new BayesianTable(this);
                _table.Reset();
            }
            else
            {
                _table.Reset();
            }
        }

        /// <summary>
        /// Validate the event.
        /// </summary>
        public void Validate()
        {
            _table.Validate();
        }

        /// <summary>
        /// Roll the specified arguments through all of the possible values, return
        /// false if we are at the final iteration. This is used to enumerate through
        /// all of the possible argument values of this event.
        /// </summary>
        /// <param name="args">The arguments to enumerate.</param>
        /// <returns>True if there are more iterations.</returns>
        public bool RollArgs(double[] args)
        {
            int currentIndex = 0;
            bool done = false;
            bool eof = false;

            if (_parents.Count == 0)
            {
                done = true;
                eof = true;
            }

            while (!done)
            {
                // EventState state = this.parents.get(currentIndex);
                var v = (int)args[currentIndex];
                v++;
                if (v >= _parents[currentIndex].Choices.Count)
                {
                    args[currentIndex] = 0;
                }
                else
                {
                    args[currentIndex] = v;
                    break;
                }

                currentIndex++;

                if (currentIndex >= _parents.Count)
                {
                    done = true;
                    eof = true;
                }
            }

            return !eof;
        }

        /// <summary>
        /// Remove all relations.
        /// </summary>
        public void RemoveAllRelations()
        {
            _children.Clear();
            _parents.Clear();
        }

        /// <summary>
        /// Format the event name with +, - and =.  For example +a or -1, or a=red.
        /// </summary>
        /// <param name="theEvent">The event to format.</param>
        /// <param name="value">The value to format for.</param>
        /// <returns>The formatted name.</returns>
        public static String FormatEventName(BayesianEvent theEvent, int value)
        {
            var str = new StringBuilder();

            if (theEvent.IsBoolean)
            {
                str.Append(value == 0 ? "+" : "-");
            }
            str.Append(theEvent.Label);
            if (!theEvent.IsBoolean)
            {
                str.Append("=");
                str.Append(value);
            }

            return str.ToString();
        }

        /// <summary>
        /// Return true if the event has the specified given event.
        /// </summary>
        /// <param name="l">The event to check for.</param>
        /// <returns>True if the event has the specified given.</returns>
        public bool HasGiven(String l)
        {
            return _parents.Any(e => e.Label.Equals(l));
        }

        /// <summary>
        /// Reset the logic table.
        /// </summary>
        public void Reset()
        {
            if (_table == null)
            {
                _table = new BayesianTable(this);
            }
            _table.Reset();
        }


        /// <summary>
        /// Match a continuous value to a discrete range. This is how floating point
        /// numbers can be used as input to a Bayesian network.
        /// </summary>
        /// <param name="d">The continuous value.</param>
        /// <returns>The range that the value was mapped into.</returns>
        public int MatchChoiceToRange(double d)
        {
            if (Choices.Count > 0 && Choices.First().IsIndex)
            {
                var result = (int)d;
                if (result > Choices.Count)
                {
                    throw new BayesianError("The item id " + result + " is not valid for event " + this.ToString());
                }
                return result;
            }

            var index = 0;
            foreach (var choice in Choices)
            {
                if (d < choice.Max)
                {
                    return index;
                }

                index++;
            }

            return Math.Min(index, Choices.Count - 1);
        }

        /// <summary>
        /// Return the choice specified by the index.  This requires searching
        /// through a list.  Do not call in performance critical areas.
        /// </summary>
        /// <param name="arg">The argument number.</param>
        /// <returns>The bayesian choice found.</returns>
        public BayesianChoice GetChoice(int arg)
        {
            int a = arg;

            foreach (BayesianChoice choice in _choices)
            {
                if (a == 0)
                {
                    return choice;
                }
                a--;
            }
            return null;
        }
    }

    [Serializable]
    public class BayesianNetwork : BasicML, IMLClassification, IMLResettable, IMLError
    {
        /// <summary>
        /// Default choices for a boolean event.
        /// </summary>
        public static readonly String[] ChoicesTrueFalse = { "true", "false" };

        /// <summary>
        /// Mapping between the event string names, and the actual events.
        /// </summary>
        private readonly IDictionary<String, BayesianEvent> _eventMap = new Dictionary<String, BayesianEvent>();

        /// <summary>
        /// A listing of all of the events.
        /// </summary>
        private readonly IList<BayesianEvent> _events = new List<BayesianEvent>();

        /// <summary>
        /// The probabilities of each classification.
        /// </summary>
        private double[] _classificationProbabilities;

        /// <summary>
        /// Specifies the classification target.
        /// </summary>
        private int _classificationTarget;

        /// <summary>
        /// Specifies if each input is present.
        /// </summary>
        private bool[] _inputPresent;

        /// <summary>
        /// Construct a Bayesian network.
        /// </summary>
        public BayesianNetwork()
        {
            Query = new EnumerationQuery(this);
        }

        /// <summary>
        /// The current Bayesian query.
        /// </summary>
        public IBayesianQuery Query { get; set; }

        /// <summary>
        /// The mapping from string names to events.
        /// </summary>
        public IDictionary<String, BayesianEvent> EventMap
        {
            get { return _eventMap; }
        }

        /// <summary>
        /// The events.
        /// </summary>
        public IList<BayesianEvent> Events
        {
            get { return _events; }
        }

        /// <summary>
        /// The contents as a string. Shows both events and dependences.
        /// </summary>
        public String Contents
        {
            get
            {
                var result = new StringBuilder();
                bool first = true;

                foreach (BayesianEvent e in _events)
                {
                    if (!first)
                        result.Append(" ");
                    first = false;
                    result.Append(e.ToFullString());
                }

                return result.ToString();
            }
            set
            {
                IList<ParsedProbability> list = ParseProbability.ParseProbabilityList(this, value);
                IList<String> labelList = new List<String>();

                // ensure that all events are there
                foreach (ParsedProbability prob in list)
                {
                    ParsedEvent parsedEvent = prob.ChildEvent;
                    String eventLabel = parsedEvent.Label;
                    labelList.Add(eventLabel);

                    // create event, if not already here
                    BayesianEvent e = GetEvent(eventLabel);
                    if (e == null)
                    {
                        IList<BayesianChoice> cl = parsedEvent.ChoiceList.Select(c => new BayesianChoice(c.Label, c.Min, c.Max)).ToList();

                        CreateEvent(eventLabel, cl);
                    }
                }


                // now remove all events that were not covered
                foreach (BayesianEvent e in _events)
                {
                    if (!labelList.Contains(e.Label))
                    {
                        RemoveEvent(e);
                    }
                }

                // handle dependencies
                foreach (ParsedProbability prob in list)
                {
                    ParsedEvent parsedEvent = prob.ChildEvent;
                    String eventLabel = parsedEvent.Label;

                    BayesianEvent e = RequireEvent(eventLabel);

                    // ensure that all "givens" are present
                    IList<String> givenList = new List<String>();
                    foreach (ParsedEvent given in prob.GivenEvents)
                    {
                        if (!e.HasGiven(given.Label))
                        {
                            BayesianEvent givenEvent = RequireEvent(given.Label);
                            CreateDependency(givenEvent, e);
                        }
                        givenList.Add(given.Label);
                    }

                    // now remove givens that were not covered
                    foreach (BayesianEvent event2 in e.Parents)
                    {
                        if (!givenList.Contains(event2.Label))
                        {
                            RemoveDependency(event2, e);
                        }
                    }
                }

                // finalize the structure
                FinalizeStructure();
                if (Query != null)
                {
                    Query.FinalizeStructure();
                }
            }
        }

        /// <summary>
        /// Get the classification target. 
        /// </summary>
        public int ClassificationTarget
        {
            get { return _classificationTarget; }
        }

        /// <summary>
        /// The classification target.
        /// </summary>
        public BayesianEvent ClassificationTargetEvent
        {
            get
            {
                if (_classificationTarget == -1)
                {
                    throw new BayesianError("No classification target defined.");
                }

                return _events[_classificationTarget];
            }
        }

        /// <summary>
        /// Returns a string representation of the classification structure.
        ///         Of the form P(a|b,c,d)
        /// </summary>
        public String ClassificationStructure
        {
            get
            {
                var result = new StringBuilder();

                result.Append("P(");
                bool first = true;

                for (int i = 0; i < Events.Count; i++)
                {
                    BayesianEvent e = _events[i];
                    EventState state = Query.GetEventState(e);
                    if (state.CurrentEventType == EventType.Outcome)
                    {
                        if (!first)
                        {
                            result.Append(",");
                        }
                        result.Append(e.Label);
                        first = false;
                    }
                }

                result.Append("|");

                first = true;
                for (int i = 0; i < Events.Count; i++)
                {
                    BayesianEvent e = _events[i];
                    if (Query.GetEventState(e).CurrentEventType == EventType.Evidence)
                    {
                        if (!first)
                        {
                            result.Append(",");
                        }
                        result.Append(e.Label);
                        first = false;
                    }
                }

                result.Append(")");
                return result.ToString();
            }
        }

        /// <summary>
        /// True if this network has a valid classification target.
        /// </summary>
        public bool HasValidClassificationTarget
        {
            get
            {
                if (_classificationTarget < 0
                    || _classificationTarget >= _events.Count)
                {
                    return false;
                }
                return true;
            }
        }

        #region IMLClassification Members

        /// <inheritdoc/>
        public int InputCount
        {
            get { return _events.Count; }
        }

        /// <inheritdoc/>
        public int OutputCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Classify the input. 
        /// </summary>
        /// <param name="input">The input to classify.</param>
        /// <returns>The classification.</returns>
        public int Classify(IMLData input)
        {
            if (_classificationTarget < 0 || _classificationTarget >= _events.Count)
            {
                throw new BayesianError("Must specify classification target by calling setClassificationTarget.");
            }

            int[] d = DetermineClasses(input);

            // properly tag all of the events
            for (int i = 0; i < _events.Count; i++)
            {
                BayesianEvent e = _events[i];
                if (i == _classificationTarget)
                {
                    Query.DefineEventType(e, EventType.Outcome);
                }
                else if (_inputPresent[i])
                {
                    Query.DefineEventType(e, EventType.Evidence);
                    Query.SetEventValue(e, d[i]);
                }
                else
                {
                    Query.DefineEventType(e, EventType.Hidden);
                    Query.SetEventValue(e, d[i]);
                }
            }


            // loop over and try each outcome choice
            BayesianEvent outcomeEvent = _events[_classificationTarget];
            _classificationProbabilities = new double[outcomeEvent.Choices.Count];
            for (int i = 0; i < outcomeEvent.Choices.Count; i++)
            {
                Query.SetEventValue(outcomeEvent, i);
                Query.Execute();
                _classificationProbabilities[i] = Query.Probability;
            }


            return EngineArray.MaxIndex(_classificationProbabilities);
        }

        #endregion

        #region IMLError Members

        /// <inheritdoc/>
        public double CalculateError(IMLDataSet data)
        {
            if (!HasValidClassificationTarget)
                return 1.0;

            // Call the following just to thrown an error if there is no classification target
            ClassificationTarget.ToString();

            int badCount = 0;
            int totalCount = 0;

            foreach (IMLDataPair pair in data)
            {
                int c = Classify(pair.Input);
                totalCount++;
                if (c != pair.Input[_classificationTarget])
                {
                    badCount++;
                }
            }

            return badCount / (double)totalCount;
        }

        #endregion

        #region IMLResettable Members

        /// <inheritdoc/>
        public void Reset()
        {
            Reset(0);
        }

        /// <inheritdoc/>
        public void Reset(int seed)
        {
            foreach (BayesianEvent e in _events)
            {
                e.Reset();
            }
        }

        #endregion

        /// <summary>
        /// Get an event based on the string label. 
        /// </summary>
        /// <param name="label">The label to locate.</param>
        /// <returns>The event found.</returns>
        public BayesianEvent GetEvent(String label)
        {
            if (!_eventMap.ContainsKey(label))
                return null;
            return _eventMap[label];
        }

        /// <summary>
        /// Get an event based on label, throw an error if not found.
        /// </summary>
        /// <param name="label">THe event label to find.</param>
        /// <returns>The event.</returns>
        public BayesianEvent GetEventError(String label)
        {
            if (!EventExists(label))
                throw (new BayesianError("Undefined label: " + label));
            return _eventMap[label];
        }


        /// <summary>
        /// Return true if the specified event exists. 
        /// </summary>
        /// <param name="label">The label we are searching for.</param>
        /// <returns>True, if the event exists by label.</returns>
        public bool EventExists(String label)
        {
            return _eventMap.ContainsKey(label);
        }

        /// <summary>
        /// Create, or register, the specified event with this bayesian network. 
        /// </summary>
        /// <param name="theEvent">The event to add.</param>
        public void CreateEvent(BayesianEvent theEvent)
        {
            if (EventExists(theEvent.Label))
            {
                throw new BayesianError("The label \"" + theEvent.Label
                                        + "\" has already been defined.");
            }

            _eventMap[theEvent.Label] = theEvent;
            _events.Add(theEvent);
        }


        /// <summary>
        /// Create an event specified on the label and options provided. 
        /// </summary>
        /// <param name="label">The label to create this event as.</param>
        /// <param name="options">The options, or states, that this event can have.</param>
        /// <returns>The newly created event.</returns>
        public BayesianEvent CreateEvent(String label, IList<BayesianChoice> options)
        {
            if (label == null)
            {
                throw new BayesianError("Can't create event with null label name");
            }

            if (EventExists(label))
            {
                throw new BayesianError("The label \"" + label
                                        + "\" has already been defined.");
            }

            BayesianEvent e = options.Count == 0 ? new BayesianEvent(label) : new BayesianEvent(label, options);
            CreateEvent(e);
            return e;
        }

        /// <summary>
        /// Create the specified events based on a variable number of options, or choices. 
        /// </summary>
        /// <param name="label">The label of the event to create.</param>
        /// <param name="options">The states that the event can have.</param>
        /// <returns>The newly created event.</returns>
        public BayesianEvent CreateEvent(String label, params String[] options)
        {
            if (label == null)
            {
                throw new BayesianError("Can't create event with null label name");
            }

            if (EventExists(label))
            {
                throw new BayesianError("The label \"" + label
                                        + "\" has already been defined.");
            }

            BayesianEvent e = options.Length == 0 ? new BayesianEvent(label) : new BayesianEvent(label, options);
            CreateEvent(e);
            return e;
        }

        /// <summary>
        /// Create a dependency between two events. 
        /// </summary>
        /// <param name="parentEvent">The parent event.</param>
        /// <param name="childEvent">The child event.</param>
        public void CreateDependency(BayesianEvent parentEvent,
                                     BayesianEvent childEvent)
        {
            // does the dependency exist?
            if (!HasDependency(parentEvent, childEvent))
            {
                // create the dependency
                parentEvent.AddChild(childEvent);
                childEvent.AddParent(parentEvent);
            }
        }

        /// <summary>
        /// Determine if the two events have a dependency. 
        /// </summary>
        /// <param name="parentEvent">The parent event.</param>
        /// <param name="childEvent">The child event.</param>
        /// <returns>True if a dependency exists.</returns>
        private static bool HasDependency(BayesianEvent parentEvent,
                                   BayesianEvent childEvent)
        {
            return (parentEvent.Children.Contains(childEvent));
        }

        /// <summary>
        /// Create a dependency between a parent and multiple children. 
        /// </summary>
        /// <param name="parentEvent">The parent event.</param>
        /// <param name="children">The child events.</param>
        public void CreateDependency(BayesianEvent parentEvent,
                                     params BayesianEvent[] children)
        {
            foreach (BayesianEvent childEvent in children)
            {
                parentEvent.AddChild(childEvent);
                childEvent.AddParent(parentEvent);
            }
        }

        /// <summary>
        /// Create a dependency between two labels.
        /// </summary>
        /// <param name="parentEventLabel">The parent event.</param>
        /// <param name="childEventLabel">The child event.</param>
        public void CreateDependency(String parentEventLabel, String childEventLabel)
        {
            BayesianEvent parentEvent = GetEventError(parentEventLabel);
            BayesianEvent childEvent = GetEventError(childEventLabel);
            CreateDependency(parentEvent, childEvent);
        }

        /// <summary>
        /// Remove a dependency, if it it exists.
        /// </summary>
        /// <param name="parent">The parent event.</param>
        /// <param name="child">The child event.</param>
        private static void RemoveDependency(BayesianEvent parent, BayesianEvent child)
        {
            parent.Children.Remove(child);
            child.Parents.Remove(parent);
        }

        /// <summary>
        /// Remove the specified event.
        /// </summary>
        /// <param name="theEvent">The event to remove.</param>
        private void RemoveEvent(BayesianEvent theEvent)
        {
            foreach (BayesianEvent e in theEvent.Parents)
            {
                e.Children.Remove(theEvent);
            }
            _eventMap.Remove(theEvent.Label);
            _events.Remove(theEvent);
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            bool first = true;

            foreach (BayesianEvent e in _events)
            {
                if (!first)
                    result.Append(" ");
                first = false;
                result.Append(e.ToString());
            }

            return result.ToString();
        }

        ///<summary>
        /// Calculate the parameter count.
        ///</summary>
        ///<returns>The number of parameters in this Bayesian network.</returns>
        public int CalculateParameterCount()
        {
            return _eventMap.Values.Sum(e => e.CalculateParameterCount());
        }

        /// <summary>
        /// Finalize the structure of this Bayesian network.
        /// </summary>
        public void FinalizeStructure()
        {
            foreach (BayesianEvent e in _eventMap.Values)
            {
                e.FinalizeStructure();
            }

            if (Query != null)
            {
                Query.FinalizeStructure();
            }

            _inputPresent = new bool[_events.Count];
            EngineArray.Fill(_inputPresent, true);
            _classificationTarget = -1;
        }

        /// <summary>
        /// Validate the structure of this Bayesian network.
        /// </summary>
        public void Validate()
        {
            foreach (BayesianEvent e in _eventMap.Values)
            {
                e.Validate();
            }
        }

        /// <summary>
        /// Determine if one Bayesian event is in an array of others. 
        /// </summary>
        /// <param name="given">The events to check.</param>
        /// <param name="e">See if e is amoung given.</param>
        /// <returns>True if e is amoung given.</returns>
        private static bool IsGiven(IEnumerable<BayesianEvent> given, BayesianEvent e)
        {
            return given.Any(e2 => e == e2);
        }


        /// <summary>
        /// Determine if one event is a descendant of another.
        /// </summary>
        /// <param name="a">The event to check.</param>
        /// <param name="b">The event that has children.</param>
        /// <returns>True if a is amoung b's children.</returns>
        public bool IsDescendant(BayesianEvent a, BayesianEvent b)
        {
            if (a == b)
                return true;

            return b.Children.Any(e => IsDescendant(a, e));
        }


        /// <summary>
        /// True if this event is given or conditionally dependant on the others. 
        /// </summary>
        /// <param name="given">The others to check.</param>
        /// <param name="e">The event to check.</param>
        /// <returns>True, if is given or descendant.</returns>
        private bool IsGivenOrDescendant(IEnumerable<BayesianEvent> given, BayesianEvent e)
        {
            return given.Any(e2 => IsDescendant(e2, e));
        }


        /// <summary>
        /// Help determine if one event is conditionally independent of another.
        /// </summary>
        /// <param name="previousHead">The previous head, as we traverse the list.</param>
        /// <param name="a">The event to check.</param>
        /// <param name="goal">List of events searched.</param>
        /// <param name="searched"></param>
        /// <param name="given">Given events.</param>
        /// <returns>True if conditionally independent.</returns>
        private bool IsCondIndependent(bool previousHead, BayesianEvent a,
                                       BayesianEvent goal, IDictionary<BayesianEvent, Object> searched,
                                       params BayesianEvent[] given)
        {
            // did we find it?
            if (a == goal)
            {
                return false;
            }

            // search children
            foreach (BayesianEvent e in a.Children)
            {
                if (!searched.ContainsKey(e) || !IsGiven(given, a))
                {
                    searched[e] = null;
                    if (!IsCondIndependent(true, e, goal, searched, given))
                        return false;
                }
            }

            // search parents
            foreach (BayesianEvent e in a.Parents)
            {
                if (!searched.ContainsKey(e))
                {
                    searched[e] = null;
                    if (!previousHead || IsGivenOrDescendant(given, a))
                        if (!IsCondIndependent(false, e, goal, searched, given))
                            return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determine if two events are conditionally independent.
        /// </summary>
        /// <param name="a">The first event.</param>
        /// <param name="b">The second event.</param>
        /// <param name="given">What is "given".</param>
        /// <returns>True of they are cond. independent.</returns>
        public bool IsCondIndependent(BayesianEvent a, BayesianEvent b,
                                      params BayesianEvent[] given)
        {
            IDictionary<BayesianEvent, Object> searched = new Dictionary<BayesianEvent, Object>();
            return IsCondIndependent(false, a, b, searched, given);
        }

        /// <inheritdoc/>
        public double ComputeProbability(IMLData input)
        {
            // copy the input to evidence
            int inputIndex = 0;
            foreach (BayesianEvent e in _events)
            {
                EventState state = Query.GetEventState(e);
                if (state.CurrentEventType == EventType.Evidence)
                {
                    state.Value = ((int)input[inputIndex++]);
                }
            }

            // execute the query
            Query.Execute();

            return Query.Probability;
        }


        /// <summary>
        /// Define the probability for an event.
        /// </summary>
        /// <param name="line">The event.</param>
        /// <param name="probability">The probability.</param>
        public void DefineProbability(String line, double probability)
        {
            var parse = new ParseProbability(this);
            ParsedProbability parsedProbability = parse.Parse(line);
            parsedProbability.DefineTruthTable(this, probability);
        }

        /// <summary>
        /// Define a probability.
        /// </summary>
        /// <param name="line">The line to define the probability.</param>
        public void DefineProbability(String line)
        {
            int index = line.LastIndexOf('=');
            bool error = false;
            double prob = 0.0;
            String left = "";

            if (index != -1)
            {
                left = line.Substring(0, index);
                String right = line.Substring(index + 1);

                try
                {
                    prob = CSVFormat.EgFormat.Parse(right);
                }
                catch (FormatException)
                {
                    error = true;
                }
            }

            if (error || index == -1)
            {
                throw new BayesianError(
                    "Probability must be of the form \"P(event|condition1,condition2,etc.)=0.5\".  Conditions are optional.");
            }
            DefineProbability(left, prob);
        }

        /// <summary>
        /// Require the specified event, thrown an error if it does not exist.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>The event.</returns>
        public BayesianEvent RequireEvent(String label)
        {
            BayesianEvent result = GetEvent(label);
            if (result == null)
            {
                throw new BayesianError("The event " + label + " is not defined.");
            }
            return result;
        }

        /// <summary>
        /// Define a relationship.
        /// </summary>
        /// <param name="line">The relationship to define.</param>
        public void DefineRelationship(String line)
        {
            var parse = new ParseProbability(this);
            ParsedProbability parsedProbability = parse.Parse(line);
            parsedProbability.DefineRelationships(this);
        }

        /// <summary>
        /// Perform a query.
        /// </summary>
        /// <param name="line">The query.</param>
        /// <returns>The probability.</returns>
        public double PerformQuery(String line)
        {
            if (Query == null)
            {
                throw new BayesianError("This Bayesian network does not have a query to define.");
            }

            var parse = new ParseProbability(this);
            ParsedProbability parsedProbability = parse.Parse(line);

            // create a temp query
            IBayesianQuery q = Query.Clone();

            // first, mark all events as hidden
            q.Reset();

            // deal with evidence (input)
            foreach (ParsedEvent parsedEvent in parsedProbability.GivenEvents)
            {
                BayesianEvent e = RequireEvent(parsedEvent.Label);
                q.DefineEventType(e, EventType.Evidence);
                q.SetEventValue(e, parsedEvent.ResolveValue(e));
            }

            // deal with outcome (output)
            foreach (ParsedEvent parsedEvent in parsedProbability.BaseEvents)
            {
                BayesianEvent e = RequireEvent(parsedEvent.Label);
                q.DefineEventType(e, EventType.Outcome);
                q.SetEventValue(e, parsedEvent.ResolveValue(e));
            }

            q.LocateEventTypes();

            q.Execute();
            return q.Probability;
        }

        /// <inheritdoc/>
        public override void UpdateProperties()
        {
            // Not needed		
        }

        ///<summary>
        /// Get the index of the given event.
        ///</summary>
        ///<param name="theEvent">The event to get the index of.</param>
        ///<returns>The index of the event.</returns>
        public int GetEventIndex(BayesianEvent theEvent)
        {
            for (int i = 0; i < _events.Count; i++)
            {
                if (theEvent == _events[i])
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Remove all relations between nodes.
        /// </summary>
        public void RemoveAllRelations()
        {
            foreach (BayesianEvent e in _events)
            {
                e.RemoveAllRelations();
            }
        }


        /// <summary>
        /// Determine the classes for the specified input. 
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>An array of class indexes.</returns>
        public int[] DetermineClasses(IMLData input)
        {
            var result = new int[input.Count];

            for (int i = 0; i < input.Count; i++)
            {
                BayesianEvent e = _events[i];
                int classIndex = e.MatchChoiceToRange(input[i]);
                result[i] = classIndex;
            }

            return result;
        }

        /// <summary>
        /// Determine if the specified input is present. 
        /// </summary>
        /// <param name="idx">The index of the input.</param>
        /// <returns>True, if the input is present.</returns>
        public bool IsInputPresent(int idx)
        {
            return _inputPresent[idx];
        }

        /// <summary>
        /// Define a classification structure of the form P(A|B) = P(C)
        /// </summary>
        /// <param name="line">The structure.</param>
        public void DefineClassificationStructure(String line)
        {
            IList<ParsedProbability> list = ParseProbability.ParseProbabilityList(this, line);

            if (list.Count > 1)
            {
                throw new BayesianError("Must only define a single probability, not a chain.");
            }

            if (list.Count == 0)
            {
                throw new BayesianError("Must define at least one probability.");
            }

            // first define everything to be hidden
            foreach (BayesianEvent e in _events)
            {
                Query.DefineEventType(e, EventType.Hidden);
            }

            // define the base event
            ParsedProbability prob = list[0];

            if (prob.BaseEvents.Count == 0)
            {
                return;
            }

            BayesianEvent be = GetEvent(prob.ChildEvent.Label);
            _classificationTarget = _events.IndexOf(be);
            Query.DefineEventType(be, EventType.Outcome);

            // define the given events
            foreach (ParsedEvent parsedGiven in prob.GivenEvents)
            {
                BayesianEvent given = GetEvent(parsedGiven.Label);
                Query.DefineEventType(given, EventType.Evidence);

            }

            Query.LocateEventTypes();

            // set the values
            foreach (ParsedEvent parsedGiven in prob.GivenEvents)
            {
                BayesianEvent e = GetEvent(parsedGiven.Label);
                Query.SetEventValue(e, ParseInt(parsedGiven.Value));
            }

            Query.SetEventValue(be, ParseInt(prob.BaseEvents[0].Value));
        }

        private int ParseInt(String str)
        {
            if (str == null)
            {
                return 0;
            }

            try
            {
                return int.Parse(str);
            }
            catch (FormatException ex)
            {
                return 0;
            }
        }

    }

    public class PersistBayes : ISyntPersistor
    {
        /// <summary>
        /// The file version.
        /// </summary>
        public int FileVersion
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public Object Read(Stream istream)
        {
            BayesianNetwork result = new BayesianNetwork();
            SyntReadHelper input = new SyntReadHelper(istream);
            SyntFileSection section;
            String queryType = "";
            String queryStr = "";
            String contentsStr = "";

            while ((section = input.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("BAYES-NETWORK")
                        && section.SubSectionName.Equals("BAYES-PARAM"))
                {
                    IDictionary<String, String> p = section.ParseParams();
                    queryType = p["queryType"];
                    queryStr = p["query"];
                    contentsStr = p["contents"];
                }
                if (section.SectionName.Equals("BAYES-NETWORK")
                        && section.SubSectionName.Equals("BAYES-TABLE"))
                {

                    result.Contents = contentsStr;

                    // first, define relationships (1st pass)
                    foreach (String line in section.Lines)
                    {
                        result.DefineRelationship(line);
                    }

                    result.FinalizeStructure();

                    // now define the probabilities (2nd pass)
                    foreach (String line in section.Lines)
                    {
                        result.DefineProbability(line);
                    }
                }
                if (section.SectionName.Equals("BAYES-NETWORK")
                        && section.SubSectionName.Equals("BAYES-PROPERTIES"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
            }

            // define query, if it exists
            if (queryType.Length > 0)
            {
                IBayesianQuery query = null;
                if (queryType.Equals("EnumerationQuery"))
                {
                    query = new EnumerationQuery(result);
                }
                else
                {
                    query = new SamplingQuery(result);
                }

                if (query != null && queryStr.Length > 0)
                {
                    result.Query = query;
                    result.DefineClassificationStructure(queryStr);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            SyntWriteHelper o = new SyntWriteHelper(os);
            BayesianNetwork b = (BayesianNetwork)obj;
            o.AddSection("BAYES-NETWORK");
            o.AddSubSection("BAYES-PARAM");
            String queryType = "";
            String queryStr = b.ClassificationStructure;

            if (b.Query != null)
            {
                queryType = b.Query.GetType().Name;
            }

            o.WriteProperty("queryType", queryType);
            o.WriteProperty("query", queryStr);
            o.WriteProperty("contents", b.Contents);
            o.AddSubSection("BAYES-PROPERTIES");
            o.AddProperties(b.Properties);

            o.AddSubSection("BAYES-TABLE");
            foreach (BayesianEvent e in b.Events)
            {
                foreach (TableLine line in e.Table.Lines)
                {
                    if (line == null)
                        continue;
                    StringBuilder str = new StringBuilder();
                    str.Append("P(");

                    str.Append(BayesianEvent.FormatEventName(e, line.Result));

                    if (e.Parents.Count > 0)
                    {
                        str.Append("|");
                    }

                    int index = 0;
                    bool first = true;
                    foreach (BayesianEvent parentEvent in e.Parents)
                    {
                        if (!first)
                        {
                            str.Append(",");
                        }
                        first = false;
                        int arg = line.Arguments[index++];
                        if (parentEvent.IsBoolean)
                        {
                            if (arg == 0)
                            {
                                str.Append("+");
                            }
                            else
                            {
                                str.Append("-");
                            }
                        }
                        str.Append(parentEvent.Label);
                        if (!parentEvent.IsBoolean)
                        {
                            str.Append("=");
                            if (arg >= parentEvent.Choices.Count)
                            {
                                throw new BayesianError("Argument value " + arg + " is out of range for event " + parentEvent.ToString());
                            }
                            str.Append(parentEvent.GetChoice(arg));
                        }
                    }
                    str.Append(")=");
                    str.Append(line.Probability);
                    str.Append("\n");
                    o.Write(str.ToString());
                }
            }

            o.Flush();
        }

        /// <inheritdoc/>
        public String PersistClassString
        {
            get
            {
                return "BayesianNetwork";
            }
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(BayesianNetwork); }
        }

    }

    [Serializable]
    public class BasicMLComplexData : IMLComplexData
    {
        /// <summary>
        /// The data held by this object.
        /// </summary>
        private ComplexNumber[] _data;

        /// <summary>
        /// Construct this object with the specified data.  Use only real numbers. 
        /// </summary>
        /// <param name="d">The data to construct this object with.</param>
        public BasicMLComplexData(double[] d)
            : this(d.Length)
        {
        }

        /// <summary>
        /// Construct this object with the specified data. Use complex numbers. 
        /// </summary>
        /// <param name="d">The data to construct this object with.</param>
        public BasicMLComplexData(ComplexNumber[] d)
        {
            _data = d;
        }

        /// <summary>
        /// Construct this object with blank data and a specified size. 
        /// </summary>
        /// <param name="size">The amount of data to store.</param>
        public BasicMLComplexData(int size)
        {
            _data = new ComplexNumber[size];
        }

        /// <summary>
        /// Construct a new BasicMLData object from an existing one. This makes a
        /// copy of an array. If MLData is not complex, then only reals will be 
        /// created. 
        /// </summary>
        /// <param name="d">The object to be copied.</param>
        public BasicMLComplexData(IMLData d)
        {
            if (d is IMLComplexData)
            {
                var c = (IMLComplexData)d;
                for (int i = 0; i < d.Count; i++)
                {
                    _data[i] = new ComplexNumber(c.GetComplexData(i));
                }
            }
            else
            {
                for (int i = 0; i < d.Count; i++)
                {
                    _data[i] = new ComplexNumber(d[i], 0);
                }
            }
        }

        #region IMLComplexData Members

        /// <summary>
        /// Clear all values to zero.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = new ComplexNumber(0, 0);
            }
        }

        /// <inheritdoc/>
        public Object Clone()
        {
            return new BasicMLComplexData(this);
        }


        /// <summary>
        /// The complex numbers.
        /// </summary>
        public ComplexNumber[] ComplexData
        {
            get { return _data; }
            set { _data = value; }
        }


        /// <inheritdoc/>
        public ComplexNumber GetComplexData(int index)
        {
            return _data[index];
        }

        /// <summary>
        /// Set a data element to a complex number. 
        /// </summary>
        /// <param name="index">The index to set.</param>
        /// <param name="d">The complex number.</param>
        public void SetComplexData(int index, ComplexNumber d)
        {
            _data[index] = d;
        }

        /// <summary>
        /// Set the complex data array.
        /// </summary>
        /// <param name="d">A new complex data array.</param>
        public void SetComplexData(ComplexNumber[] d)
        {
            _data = d;
        }

        /// <summary>
        /// Access the data by index.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        public virtual double this[int x]
        {
            get { return _data[x].Real; }
            set { _data[x] = new ComplexNumber(value, 0); }
        }

        /// <summary>
        /// Get the data as an array.
        /// </summary>
        public virtual double[] Data
        {
            get
            {
                var d = new double[_data.Length];
                for (int i = 0; i < d.Length; i++)
                {
                    d[i] = _data[i].Real;
                }
                return d;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    _data[i] = new ComplexNumber(value[i], 0);
                }
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get { return _data.Count(); }
        }

        #endregion

        /// <inheritdoc/>
        public String ToString()
        {
            var builder = new StringBuilder("[");
            builder.Append(GetType().Name);
            builder.Append(":");
            for (int i = 0; i < _data.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }
                builder.Append(_data[i].ToString());
            }
            builder.Append("]");
            return builder.ToString();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns>Nothing.</returns>
        public ICentroid<IMLData> CreateCentroid()
        {
            return null;
        }
    }

    [Serializable]
    public class BasicMLData : IMLData
    {
        private double[] _data;

        /// <summary>
        /// Construct this object with the specified data. 
        /// </summary>
        /// <param name="d">The data to construct this object with.</param>
        public BasicMLData(double[] d)
            : this(d.Length)
        {
            for (int i = 0; i < d.Length; i++)
            {
                _data[i] = d[i];
            }
        }


        /// <summary>
        /// Construct this object with blank data and a specified size.
        /// </summary>
        /// <param name="size">The amount of data to store.</param>
        public BasicMLData(int size)
        {
            _data = new double[size];
        }

        /// <summary>
        /// Access the data by index.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        public virtual double this[int x]
        {
            get { return _data[x]; }
            set { _data[x] = value; }
        }

        /// <summary>
        /// Get the data as an array.
        /// </summary>
        public virtual double[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Get the count of data items.
        /// </summary>
        public virtual int Count
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// Convert the object to a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append('[');
            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                    result.Append(',');
                result.Append(Data[i]);
            }
            result.Append(']');
            return result.ToString();
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public object Clone()
        {
            var result = new BasicMLData(_data);
            return result;
        }

        /// <summary>
        /// Clear to zero.
        /// </summary>
        public void Clear()
        {
            EngineArray.Fill(_data, 0);
        }

        /// <inheritdoc/>
        public ICentroid<IMLData> CreateCentroid()
        {
            return new BasicMLDataCentroid(this);
        }

        /// <summary>
        /// Add one data element to another.  This does not modify the object.
        /// </summary>
        /// <param name="o">The other data element</param>
        /// <returns>The result.</returns>
        public IMLData Plus(IMLData o)
        {
            if (Count != o.Count)
                throw new SyntError("Lengths must match.");

            var result = new BasicMLData(Count);
            for (int i = 0; i < Count; i++)
                result[i] = this[i] + o[i];

            return result;
        }

        /// <summary>
        /// Multiply one data element with another.  This does not modify the object.
        /// </summary>
        /// <param name="d">The other data element</param>
        /// <returns>The result.</returns>
        public IMLData Times(double d)
        {
            IMLData result = new BasicMLData(Count);

            for (int i = 0; i < Count; i++)
                result[i] = this[i] * d;

            return result;
        }

        /// <summary>
        /// Subtract one data element from another.  This does not modify the object.
        /// </summary>
        /// <param name="o">The other data element</param>
        /// <returns>The result.</returns>
        public IMLData Minus(IMLData o)
        {
            if (Count != o.Count)
                throw new SyntError("Counts must match.");

            IMLData result = new BasicMLData(Count);
            for (int i = 0; i < Count; i++)
                result[i] = this[i] - o[i];

            return result;
        }

    }

    public class BasicMLDataCentroid : ICentroid<IMLData>
    {
        /// <summary>
        /// The value this centroid is based on.
        /// </summary>
        private BasicMLData value;

        /// <summary>
        /// Construct the centroid. 
        /// </summary>
        /// <param name="o">The object to base the centroid on.</param>
        public BasicMLDataCentroid(IMLData o)
        {
            this.value = (BasicMLData)o.Clone();
        }

        /// <inheritdoc/>
        public void Add(IMLData d)
        {
            double[] a = d.Data;

            for (int i = 0; i < value.Count; i++)
                value.Data[i] =
                    ((value.Data[i] * value.Count + a[i]) / (value.Count + 1));
        }

        /// <inheritdoc/>
        public void Remove(IMLData d)
        {
            double[] a = d.Data;

            for (int i = 0; i < value.Count; i++)
                value[i] =
                    ((value[i] * value.Count - a[i]) / (value.Count - 1));
        }

        /// <inheritdoc/>
        public double Distance(IMLData d)
        {
            IMLData diff = value.Minus(d);
            double sum = 0.0;

            for (int i = 0; i < diff.Count; i++)
                sum += diff[i] * diff[i];

            return Math.Sqrt(sum);
        }
    }

    [Serializable]
    public class BasicMLDataPair : IMLDataPair
    {
        /// <summary>
        /// The the expected output from the neural network, or null
        /// for unsupervised training.
        /// </summary>
        private readonly IMLData _ideal;

        /// <summary>
        /// The training input to the neural network.
        /// </summary>
        private readonly IMLData _input;

        /// <summary>
        /// The significance.
        /// </summary>
        private double _significance = 1.0;

        /// <summary>
        /// Construct a BasicMLDataPair class with the specified input
        /// and ideal values.
        /// </summary>
        /// <param name="input">The input to the neural network.</param>
        /// <param name="ideal">The expected results from the neural network.</param>
        public BasicMLDataPair(IMLData input, IMLData ideal)
        {
            _input = input;
            _ideal = ideal;
        }

        /// <summary>
        /// Construct a data pair that only includes input. (unsupervised)
        /// </summary>
        /// <param name="input">The input data.</param>
        public BasicMLDataPair(IMLData input)
        {
            _input = input;
            _ideal = null;
        }

        /// <summary>
        /// The input data.
        /// </summary>
        public virtual IMLData Input
        {
            get { return _input; }
        }

        /// <summary>
        /// The ideal data.
        /// </summary>
        public virtual IMLData Ideal
        {
            get { return _ideal; }
        }

        /// <summary>
        /// Convert object to a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append('[');
            result.Append("Input:");
            result.Append(Input);
            result.Append(",Ideal:");
            result.Append(Ideal);
            result.Append(']');
            return result.ToString();
        }

        /// <summary>
        /// Deterimine if this pair is supervised or unsupervised.
        /// </summary>
        /// <returns>True if this is a supervised pair.</returns>
        public bool IsSupervised
        {
            get { return _ideal != null; }
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public object Clone()
        {
            Object result;

            if (Ideal == null)
                result = new BasicMLDataPair((IMLData)_input.Clone());
            else
                result = new BasicMLDataPair((IMLData)_input.Clone(),
                                             (IMLData)_ideal.Clone());

            return result;
        }

        /// <summary>
        /// Create a new neural data pair object of the correct size for the neural
        /// network that is being trained. This object will be passed to the getPair
        /// method to allow the neural data pair objects to be copied to it.
        /// </summary>
        /// <param name="inputSize">The size of the input data.</param>
        /// <param name="idealSize">The size of the ideal data.</param>
        /// <returns>A new neural data pair object.</returns>
        public static IMLDataPair CreatePair(int inputSize, int idealSize)
        {
            IMLDataPair result;

            if (idealSize > 0)
            {
                result = new BasicMLDataPair(new BasicMLData(inputSize),
                                             new BasicMLData(idealSize));
            }
            else
            {
                result = new BasicMLDataPair(new BasicMLData(inputSize));
            }

            return result;
        }

        /// <summary>
        /// The supervised ideal data.
        /// </summary>
        public double[] IdealArray
        {
            get
            {
                return _ideal == null ? null : _ideal.Data;
            }
            set { _ideal.Data = value; }
        }

        /// <summary>
        /// The input array.
        /// </summary>
        public double[] InputArray
        {
            get { return _input.Data; }
            set { _input.Data = value; }
        }

        /// <summary>
        /// Returns true, if supervised.
        /// </summary>
        public bool Supervised
        {
            get { return _ideal != null; }
        }

        /// <summary>
        /// The significance of this training element.
        /// </summary>
        public double Significance
        {
            get { return _significance; }
            set { _significance = value; }
        }

        /// <inheritdoc/>
        public ICentroid<IMLDataPair> CreateCentroid()
        {
            if (!(Input is BasicMLData))
            {
                throw new SyntError("The input data type of " + Input.GetType().Name + " must be BasicMLData.");
            }
            return new BasicMLDataPairCentroid(this);
        }
    }

    public class BasicMLDataPairCentroid : ICentroid<IMLDataPair>
    {
        /// <summary>
        /// The value the centroid is based on.
        /// </summary>
        private readonly BasicMLData _value;

        /// <summary>
        /// Construct the centroid. 
        /// </summary>
        /// <param name="o"> The pair to base the centroid on.</param>
        public BasicMLDataPairCentroid(BasicMLDataPair o)
        {
            _value = (BasicMLData)o.Input.Clone();
        }

        /// <inheritdoc/>
        public void Remove(IMLDataPair d)
        {
            double[] a = d.InputArray;

            for (int i = 0; i < _value.Count; i++)
                _value[i] =
                    ((_value[i] * _value.Count - a[i]) / (_value.Count - 1));
        }

        /// <inheritdoc/>
        public double Distance(IMLDataPair d)
        {
            IMLData diff = _value.Minus(d.Input);
            double sum = 0.0;

            for (int i = 0; i < diff.Count; i++)
                sum += diff[i] * diff[i];

            return Math.Sqrt(sum);
        }

        /// <inheritdoc/>
        public void Add(IMLDataPair d)
        {
            double[] a = d.InputArray;

            for (int i = 0; i < _value.Count; i++)
                _value[i] =
                    ((_value[i] * _value.Count) + a[i]) / (_value.Count + 1);
        }

    }

    [Serializable]
    public class BasicMLDataSet : IMLDataSet, IEnumerable<IMLDataPair>
    {
        /// <summary>
        /// The enumerator for the basic neural data set.
        /// </summary>
        [Serializable]
        public class BasicNeuralEnumerator : IEnumerator<IMLDataPair>
        {
            /// <summary>
            /// The current index.
            /// </summary>
            private int _current;

            /// <summary>
            /// The owner.
            /// </summary>
            private readonly BasicMLDataSet _owner;

            /// <summary>
            /// Construct an enumerator.
            /// </summary>
            /// <param name="owner">The owner of the enumerator.</param>
            public BasicNeuralEnumerator(BasicMLDataSet owner)
            {
                _current = -1;
                _owner = owner;
            }

            /// <summary>
            /// The current data item.
            /// </summary>
            public IMLDataPair Current
            {
                get { return _owner._data[_current]; }
            }

            /// <summary>
            /// Dispose of this object.
            /// </summary>
            public void Dispose()
            {
                // nothing needed
            }

            /// <summary>
            /// The current item.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (_current < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return _owner._data[_current];
                }
            }

            /// <summary>
            /// Move to the next item.
            /// </summary>
            /// <returns>True if there is a next item.</returns>
            public bool MoveNext()
            {
                _current++;
                if (_current >= _owner._data.Count)
                    return false;
                return true;
            }

            /// <summary>
            /// Reset to the beginning.
            /// </summary>
            public void Reset()
            {
                _current = -1;
            }
        }

        /// <summary>
        /// Access to the list of data items.
        /// </summary>
        public IList<IMLDataPair> Data
        {
            get { return _data; }
            set { _data = value; }
        }


        /// <summary>
        /// The data held by this object.
        /// </summary>
        private IList<IMLDataPair> _data = new List<IMLDataPair>();

        /// <summary>
        /// Construct a data set from an already created list. Mostly used to
        /// duplicate this class.
        /// </summary>
        /// <param name="data">The data to use.</param>
        public BasicMLDataSet(IList<IMLDataPair> data)
        {
            _data = data;
        }

        /// <summary>
        /// Copy whatever dataset type is specified into a memory dataset.
        /// </summary>
        ///
        /// <param name="set">The dataset to copy.</param>
        public BasicMLDataSet(IMLDataSet set)
        {
            _data = new List<IMLDataPair>();
            int inputCount = set.InputSize;
            int idealCount = set.IdealSize;


            foreach (IMLDataPair pair in set)
            {
                BasicMLData input = null;
                BasicMLData ideal = null;

                if (inputCount > 0)
                {
                    input = new BasicMLData(inputCount);
                    EngineArray.ArrayCopy(pair.InputArray, input.Data);
                }

                if (idealCount > 0)
                {
                    ideal = new BasicMLData(idealCount);
                    EngineArray.ArrayCopy(pair.IdealArray, ideal.Data);
                }

                Add(new BasicMLDataPair(input, ideal));
            }
        }


        /// <summary>
        /// Construct a data set from an input and idea array.
        /// </summary>
        /// <param name="input">The input into the neural network for training.</param>
        /// <param name="ideal">The idea into the neural network for training.</param>
        public BasicMLDataSet(double[][] input, double[][] ideal)
        {
            for (int i = 0; i < input.Length; i++)
            {
                var tempInput = new double[input[0].Length];
                double[] tempIdeal;

                for (int j = 0; j < tempInput.Length; j++)
                {
                    tempInput[j] = input[i][j];
                }

                BasicMLData idealData = null;

                if (ideal != null)
                {
                    tempIdeal = new double[ideal[0].Length];
                    for (int j = 0; j < tempIdeal.Length; j++)
                    {
                        tempIdeal[j] = ideal[i][j];
                    }
                    idealData = new BasicMLData(tempIdeal);
                }

                var inputData = new BasicMLData(tempInput);

                Add(inputData, idealData);
            }
        }

        /// <summary>
        /// Construct a basic neural data set.
        /// </summary>
        public BasicMLDataSet()
        {
        }

        /// <summary>
        /// Get the ideal size, or zero for unsupervised.
        /// </summary>
        public virtual int IdealSize
        {
            get
            {
                if (_data == null || _data.Count == 0)
                {
                    return 0;
                }

                IMLDataPair pair = _data[0];

                if (pair.IdealArray == null)
                {
                    return 0;
                }

                return pair.Ideal.Count;
            }
        }

        /// <summary>
        /// Get the input size.
        /// </summary>
        public virtual int InputSize
        {
            get
            {
                if (_data == null || _data.Count == 0)
                    return 0;
                IMLDataPair pair = _data[0];
                return pair.Input.Count;
            }
        }

        /// <summary>
        /// Add the specified data to the set.  Add unsupervised data.
        /// </summary>
        /// <param name="data1">The data to add to the set.</param>
        public virtual void Add(IMLData data1)
        {
            IMLDataPair pair = new BasicMLDataPair(data1, null);
            _data.Add(pair);
        }

        /// <summary>
        /// Add supervised data to the set.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <param name="idealData">The ideal data.</param>
        public virtual void Add(IMLData inputData, IMLData idealData)
        {
            IMLDataPair pair = new BasicMLDataPair(inputData, idealData);
            _data.Add(pair);
        }

        /// <summary>
        /// Add a pair to the set.
        /// </summary>
        /// <param name="inputData">The pair to add to the set.</param>
        public virtual void Add(IMLDataPair inputData)
        {
            _data.Add(inputData);
        }

        /// <summary>
        /// Close the neural data set.
        /// </summary>
        public void Close()
        {
            // not needed
        }

        /// <summary>
        /// Get an enumerator to access the data with.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            return new BasicNeuralEnumerator(this);
        }

        /// <summary>
        /// Get an enumerator to access the data with.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BasicNeuralEnumerator(this);
        }

        /// <summary>
        /// Determine if the dataset is supervised.  It is assumed that all pairs
        /// are either supervised or not.  So we can determine the entire set by
        /// looking at the first item.  If the set is empty then return false, or
        /// unsupervised.
        /// </summary>
        public bool IsSupervised
        {
            get
            {
                if (_data.Count == 0)
                    return false;
                return (_data[0].Supervised);
            }
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public object Clone()
        {
            var result = new BasicMLDataSet();
            foreach (IMLDataPair pair in Data)
            {
                result.Add((IMLDataPair)pair.Clone());
            }
            return result;
        }

        /// <summary>
        /// The number of records in this data set.
        /// </summary>
        public int Count
        {
            get { return _data.Count; }
        }

        /// <summary>
        /// Get one record from the data set.
        /// </summary>
        /// <param name="index">The index to read.</param>
        /// <param name="pair">The pair to read into.</param>
        public void GetRecord(int index, IMLDataPair pair)
        {
            IMLDataPair source = _data[index];
            pair.InputArray = source.Input.Data;
            if (pair.IdealArray != null)
            {
                pair.IdealArray = source.Ideal.Data;
            }
        }

        /// <summary>
        /// Open an additional instance of this dataset.
        /// </summary>
        /// <returns>The new instance of this dataset.</returns>
        public IMLDataSet OpenAdditional()
        {
            return new BasicMLDataSet(Data);
        }


        /// <summary>
        /// Return true if supervised.
        /// </summary>
        public bool Supervised
        {
            get
            {
                if (_data.Count == 0)
                {
                    return false;
                }
                return _data[0].Supervised;
            }
        }

        public IMLDataPair this[int x]
        {
            get { return _data[x]; }
        }
    }
    //__________________
    [Serializable]
    public class BasicMLSequenceSet : IMLSequenceSet
    {
        /// <summary>
        /// The data held by this object.
        /// </summary>
        private readonly IList<IMLDataSet> _sequences = new List<IMLDataSet>();

        private IMLDataSet _currentSequence;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BasicMLSequenceSet()
        {
            _currentSequence = new BasicMLDataSet();
            _sequences.Add(_currentSequence);
        }

        public BasicMLSequenceSet(BasicMLSequenceSet other)
        {
            _sequences = other._sequences;
            _currentSequence = other._currentSequence;
        }

        /// <summary>
        /// Construct a data set from an input and ideal array.
        /// </summary>
        /// <param name="input">The input into the machine learning method for training.</param>
        /// <param name="ideal">The ideal output for training.</param>
        public BasicMLSequenceSet(double[][] input, double[][] ideal)
        {
            _currentSequence = new BasicMLDataSet(input, ideal);
            _sequences.Add(_currentSequence);
        }

        /// <summary>
        /// Construct a data set from an already created list. Mostly used to
        /// duplicate this class.
        /// </summary>
        /// <param name="theData">The data to use.</param>
        public BasicMLSequenceSet(IList<IMLDataPair> theData)
        {
            _currentSequence = new BasicMLDataSet(theData);
            _sequences.Add(_currentSequence);
        }

        /// <summary>
        /// Copy whatever dataset type is specified into a memory dataset. 
        /// </summary>
        /// <param name="set">The dataset to copy.</param>
        public BasicMLSequenceSet(IMLDataSet set)
        {
            _currentSequence = new BasicMLDataSet();
            _sequences.Add(_currentSequence);

            int inputCount = set.InputSize;
            int idealCount = set.IdealSize;

            foreach (IMLDataPair pair in set)
            {
                BasicMLData input = null;
                BasicMLData ideal = null;

                if (inputCount > 0)
                {
                    input = new BasicMLData(inputCount);
                    EngineArray.ArrayCopy(pair.InputArray, input.Data);
                }

                if (idealCount > 0)
                {
                    ideal = new BasicMLData(idealCount);
                    EngineArray.ArrayCopy(pair.IdealArray, ideal.Data);
                }

                _currentSequence.Add(new BasicMLDataPair(input, ideal));
            }
        }

        #region IMLSequenceSet Members

        /// <inheritdoc/>
        public void Add(IMLData theData)
        {
            _currentSequence.Add(theData);
        }

        /// <inheritdoc/>
        public void Add(IMLData inputData, IMLData idealData)
        {
            IMLDataPair pair = new BasicMLDataPair(inputData, idealData);
            _currentSequence.Add(pair);
        }

        /// <inheritdoc/>
        public void Add(IMLDataPair inputData)
        {
            _currentSequence.Add(inputData);
        }

        /// <inheritdoc/>
        public void Close()
        {
            // nothing to close
        }


        /// <inheritdoc/>
        public int IdealSize
        {
            get
            {
                if (_sequences[0].Count == 0)
                {
                    return 0;
                }
                return _sequences[0].IdealSize;
            }
        }

        /// <inheritdoc/>
        public int InputSize
        {
            get
            {
                if (_sequences[0].Count == 0)
                {
                    return 0;
                }
                return _sequences[0].IdealSize;
            }
        }

        /// <inheritdoc/>
        public void GetRecord(int index, IMLDataPair pair)
        {
            int recordIndex = index;
            int sequenceIndex = 0;

            while (_sequences[sequenceIndex].Count < recordIndex)
            {
                recordIndex -= _sequences[sequenceIndex].Count;
                sequenceIndex++;
                if (sequenceIndex > _sequences.Count)
                {
                    throw new MLDataError("Record out of range: " + index);
                }
            }

            _sequences[sequenceIndex].GetRecord(recordIndex, pair);
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return _sequences.Sum(ds => ds.Count);
            }
        }

        /// <inheritdoc/>
        public bool Supervised
        {
            get
            {
                if (_sequences[0].Count == 0)
                {
                    return false;
                }
                return _sequences[0].Supervised;
            }
        }

        /// <inheritdoc/>
        public IMLDataSet OpenAdditional()
        {
            return new BasicMLSequenceSet(this);
        }

        public void StartNewSequence()
        {
            if (_currentSequence.Count > 0)
            {
                _currentSequence = new BasicMLDataSet();
                _sequences.Add(_currentSequence);
            }
        }

        /// <inheritdoc/>
        public int SequenceCount
        {
            get { return _sequences.Count; }
        }

        /// <inheritdoc/>
        public IMLDataSet GetSequence(int i)
        {
            return _sequences[i];
        }

        /// <inheritdoc/>
        public ICollection<IMLDataSet> Sequences
        {
            get { return _sequences; }
        }

        /// <inheritdoc/>
        public void Add(IMLDataSet sequence)
        {
            foreach (IMLDataPair pair in sequence)
            {
                Add(pair);
            }
        }

        /// <summary>
        /// Get an enumerator to access the data with.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            return new BasicMLSequenceSetEnumerator(this);
        }

        /// <inheritdoc/>
        public IMLDataPair this[int x]
        {
            get
            {
                IMLDataPair result = BasicMLDataPair.CreatePair(InputSize, IdealSize);
                GetRecord(x, result);
                return result;
            }
        }

        #endregion

        /// <inheritdoc/>
        public Object Clone()
        {
            return ObjectCloner.DeepCopy(this);
        }

        #region Nested type: BasicMLSequenceSetEnumerator

        /// <summary>
        /// Enumerate.
        /// </summary>
        public class BasicMLSequenceSetEnumerator : IEnumerator<IMLDataPair>
        {
            /// <summary>
            /// The owner.
            /// </summary>
            private readonly BasicMLSequenceSet _owner;

            /// <summary>
            /// The index that the iterator is currently at.
            /// </summary>
            private int _currentIndex;

            /// <summary>
            /// The sequence index.
            /// </summary>
            private int _currentSequenceIndex;

            /// <summary>
            /// Construct an enumerator.
            /// </summary>
            /// <param name="owner">The owner of the enumerator.</param>
            public BasicMLSequenceSetEnumerator(BasicMLSequenceSet owner)
            {
                Reset();
                _owner = owner;
            }

            #region IEnumerator<IMLDataPair> Members

            /// <summary>
            /// The current data item.
            /// </summary>
            public IMLDataPair Current
            {
                get
                {
                    if (_currentSequenceIndex >= _owner.SequenceCount)
                    {
                        throw new InvalidOperationException("Trying to read past the end of the dataset.");
                    }

                    if (_currentIndex < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return _owner.GetSequence(_currentSequenceIndex)[_currentIndex];
                }
            }

            /// <summary>
            /// Dispose of this object.
            /// </summary>
            public void Dispose()
            {
                // nothing needed
            }

            /// <summary>
            /// The current item.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (_currentSequenceIndex >= _owner.SequenceCount)
                    {
                        throw new InvalidOperationException("Trying to read past the end of the dataset.");
                    }

                    if (_currentIndex < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return _owner.GetSequence(_currentSequenceIndex)[_currentIndex];
                }
            }

            /// <summary>
            /// Move to the next item.
            /// </summary>
            /// <returns>True if there is a next item.</returns>
            public bool MoveNext()
            {
                if (_currentSequenceIndex >= _owner.SequenceCount)
                {
                    return false;
                }

                IMLDataSet current = _owner.GetSequence(_currentSequenceIndex);
                _currentIndex++;

                if (_currentIndex >= current.Count)
                {
                    _currentIndex = 0;
                    _currentSequenceIndex++;
                }

                if (_currentSequenceIndex >= _owner.SequenceCount)
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Reset to the beginning.
            /// </summary>
            public void Reset()
            {
                _currentIndex = -1;
                _currentSequenceIndex = 0;
            }

            #endregion
        }

        #endregion
    }

    public class ArrayDataCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The ideal array.
        /// </summary>
        private double[][] _ideal;

        /// <summary>
        /// The number of ideal elements.
        /// </summary>
        private int _idealSize;

        /// <summary>
        /// The current index.
        /// </summary>
        private int _index;

        /// <summary>
        /// The input array.
        /// </summary>
        private double[][] _input;

        /// <summary>
        /// The number of input elements.
        /// </summary>
        private int _inputSize;

        /// <summary>
        /// Construct an array CODEC. 
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <param name="ideal">The ideal array.</param>
        public ArrayDataCODEC(double[][] input, double[][] ideal)
        {
            _input = input;
            _ideal = ideal;
            _inputSize = input[0].Length;
            _idealSize = ideal[0].Length;
            _index = 0;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ArrayDataCODEC()
        {
        }

        /// <inheritdoc/>
        public double[][] Input
        {
            get { return _input; }
        }

        /// <inheritdoc/>
        public double[][] Ideal
        {
            get { return _ideal; }
        }

        #region IDataSetCODEC Members

        /// <inheritdoc/>
        public int InputSize
        {
            get { return _inputSize; }
        }

        /// <inheritdoc/>
        public int IdealSize
        {
            get { return _idealSize; }
        }

        /// <inheritdoc/>
        public bool Read(double[] input, double[] ideal, ref double significance)
        {
            if (_index >= _input.Length)
            {
                return false;
            }
            EngineArray.ArrayCopy(_input[_index], input);
            EngineArray.ArrayCopy(_ideal[_index], ideal);
            _index++;
            significance = 1.0;
            return true;
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
            EngineArray.ArrayCopy(input, _input[_index]);
            EngineArray.ArrayCopy(ideal, _ideal[_index]);
            _index++;
        }

        /// <inheritdoc/>
        public void PrepareWrite(int recordCount,
                                 int inputSize, int idealSize)
        {
            _input = EngineArray.AllocateDouble2D(recordCount, inputSize);
            _ideal = EngineArray.AllocateDouble2D(recordCount, idealSize);
            _inputSize = inputSize;
            _idealSize = idealSize;
            _index = 0;
        }

        /// <inheritdoc/>
        public void PrepareRead()
        {
        }

        /// <inheritdoc/>
        public void Close()
        {
        }

        #endregion
    }

    public class CSVDataCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The external CSV file.
        /// </summary>
        private readonly String _file;

        /// <summary>
        /// The CSV format to use.
        /// </summary>
        private readonly CSVFormat _format;

        /// <summary>
        /// True, if headers are present in the CSV file.
        /// </summary>
        private readonly bool _headers;

        /// <summary>
        /// The size of the ideal data.
        /// </summary>
        private int _idealCount;

        /// <summary>
        /// The size of the input data.
        /// </summary>
        private int _inputCount;

        /// <summary>
        /// A file used to output the CSV file.
        /// </summary>
        private TextWriter _output;

        /// <summary>
        /// The utility to assist in reading the CSV file.
        /// </summary>
        private ReadCSV _readCSV;

        /// <summary>
        /// Should a significance column be expected.
        /// </summary>
        private bool _significance;

        /// <summary>
        /// Create a CODEC to load data from CSV to binary. 
        /// </summary>
        /// <param name="file">The CSV file to load.</param>
        /// <param name="format">The format that the CSV file is in.</param>
        /// <param name="headers">True, if there are headers.</param>
        /// <param name="inputCount">The number of input columns.</param>
        /// <param name="idealCount">The number of ideal columns.</param>
        /// <param name="significance">Is there a signficance column.</param>
        public CSVDataCODEC(
            String file,
            CSVFormat format,
            bool headers,
            int inputCount, int idealCount, bool significance)
        {
            if (_inputCount != 0)
            {
                throw new BufferedDataError(
                    "To export CSV, you must use the CSVDataCODEC constructor that does not specify input or ideal sizes.");
            }
            _file = file;
            _format = format;
            _inputCount = inputCount;
            _idealCount = idealCount;
            _headers = headers;
            _significance = significance;
        }

        /// <summary>
        /// Constructor to create CSV from binary.
        /// </summary>
        /// <param name="file">The CSV file to create.</param>
        /// <param name="format">The format for that CSV file.</param>
        public CSVDataCODEC(String file, CSVFormat format, bool significance)
        {
            _file = file;
            _format = format;
            _significance = significance;
        }

        #region IDataSetCODEC Members

        /// <inheritdoc/>
        public bool Read(double[] input, double[] ideal, ref double significance)
        {
            if (_readCSV.Next())
            {
                int index = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    input[i] = _readCSV.GetDouble(index++);
                }

                for (int i = 0; i < ideal.Length; i++)
                {
                    ideal[i] = _readCSV.GetDouble(index++);
                }

                if (_significance)
                {
                    significance = _readCSV.GetDouble(index++);
                }
                else
                {
                    significance = 1;
                }
                return true;
            }
            return false;
        }


        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
            if (_significance)
            {
                var record = new double[input.Length + ideal.Length + 1];
                EngineArray.ArrayCopy(input, record);
                EngineArray.ArrayCopy(ideal, 0, record, input.Length, ideal.Length);
                record[record.Length - 1] = significance;
                var result = new StringBuilder();
                NumberList.ToList(_format, result, record);
                _output.WriteLine(result.ToString());
            }
            else
            {
                var record = new double[input.Length + ideal.Length];
                EngineArray.ArrayCopy(input, record);
                EngineArray.ArrayCopy(ideal, 0, record, input.Length, ideal.Length);
                var result = new StringBuilder();
                NumberList.ToList(_format, result, record);
                _output.WriteLine(result.ToString());
            }
        }

        /// <summary>
        /// Prepare to write to a CSV file. 
        /// </summary>
        /// <param name="recordCount">The total record count, that will be written.</param>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size.</param>
        public void PrepareWrite(
            int recordCount,
            int inputSize,
            int idealSize)
        {
            try
            {
                _inputCount = inputSize;
                _idealCount = idealSize;
                _output = new StreamWriter(new FileStream(_file, FileMode.Create));
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Prepare to read from the CSV file.
        /// </summary>
        public void PrepareRead()
        {
            if (_inputCount == 0)
            {
                throw new BufferedDataError(
                    "To import CSV, you must use the CSVDataCODEC constructor that specifies input and ideal sizes.");
            }
            _readCSV = new ReadCSV(_file, _headers,
                                  _format);
        }

        /// <inheritDoc/>
        public int InputSize
        {
            get { return _inputCount; }
        }

        /// <inheritDoc/>
        public int IdealSize
        {
            get { return _idealCount; }
        }

        /// <inheritDoc/>
        public void Close()
        {
            if (_readCSV != null)
            {
                _readCSV.Close();
                _readCSV = null;
            }

            if (_output != null)
            {
                _output.Close();
                _output = null;
            }
        }

        #endregion
    }

    public class NeuralDataSetCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The dataset.
        /// </summary>
        private readonly IMLDataSet _dataset;

        /// <summary>
        /// The iterator used to read through the dataset.
        /// </summary>
        private IEnumerator<IMLDataPair> _enumerator;

        /// <summary>
        /// The number of ideal elements.
        /// </summary>
        private int _idealSize;

        /// <summary>
        /// The number of input elements.
        /// </summary>
        private int _inputSize;

        /// <summary>
        /// Construct a CODEC. 
        /// </summary>
        /// <param name="dataset">The dataset to use.</param>
        public NeuralDataSetCODEC(IMLDataSet dataset)
        {
            _dataset = dataset;
            _inputSize = dataset.InputSize;
            _idealSize = dataset.IdealSize;
        }

        #region IDataSetCODEC Members

        /// <inheritdoc/>
        public int InputSize
        {
            get { return _inputSize; }
        }

        /// <inheritdoc/>
        public int IdealSize
        {
            get { return _idealSize; }
        }

        /// <inheritdoc/>
        public bool Read(double[] input, double[] ideal, ref double significance)
        {
            if (!_enumerator.MoveNext())
            {
                return false;
            }
            else
            {
                IMLDataPair pair = _enumerator.Current;
                EngineArray.ArrayCopy(pair.Input.Data, input);
                EngineArray.ArrayCopy(pair.Ideal.Data, ideal);
                significance = pair.Significance;
                return true;
            }
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
            IMLDataPair pair = BasicMLDataPair.CreatePair(_inputSize,
                                                         _idealSize);
            EngineArray.ArrayCopy(input, pair.Input.Data);
            EngineArray.ArrayCopy(ideal, pair.Ideal.Data);
            pair.Significance = significance;
        }

        /// <inheritdoc/>
        public void PrepareWrite(int recordCount,
                                 int inputSize, int idealSize)
        {
            _inputSize = inputSize;
            _idealSize = idealSize;
        }

        /// <inheritdoc/>
        public void PrepareRead()
        {
            _enumerator = _dataset.GetEnumerator();
        }

        /// <inheritdoc/>
        public void Close()
        {
        }

        #endregion
    }

    public class SQLCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The database connection.
        /// </summary>
        private readonly DbConnection _connection;

        /// <summary>
        /// What is the size of the ideal data?
        /// </summary>
        private readonly int _idealSize;

        /// <summary>
        /// What is the size of the input data?
        /// </summary>
        private readonly int _inputSize;

        /// <summary>
        /// The SQL statement being used.
        /// </summary>
        private readonly DbCommand _statement;

        /// <summary>
        /// Holds results from the SQL query.
        /// </summary>
        private DbDataReader _results;

        /// <summary>
        /// Create a SQL neural data set.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="inputSize">The size of the input data being read.</param>
        /// <param name="idealSize">The size of the ideal output data being read.</param>
        /// <param name="connectString">The connection string.</param>
        public SQLCODEC(String sql, int inputSize,
                        int idealSize, String connectString)
        {
            _inputSize = inputSize;
            _idealSize = idealSize;
            _connection = new OleDbConnection(connectString);
            _connection.Open();
            _statement = _connection.CreateCommand();
            _statement.CommandText = sql;
            _statement.Prepare();
            _statement.Connection = _connection;
        }

        #region IDataSetCODEC Members

        /// <summary>
        /// Read a record.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="ideal">The ideal data.</param>
        /// <returns></returns>
        public bool Read(double[] input, double[] ideal, ref double significance)
        {
            if (!_results.NextResult())
                return false;

            for (int i = 1; i <= _inputSize; i++)
            {
                input[i - 1] = _results.GetDouble(i);
            }

            if (_idealSize > 0)
            {
                for (int i = 1; i <= _idealSize; i++)
                {
                    ideal[i - 1] =
                        _results.GetDouble(i + _inputSize);
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prepare to write.
        /// </summary>
        /// <param name="recordCount">The record count.</param>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size.</param>
        public void PrepareWrite(int recordCount, int inputSize, int idealSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prepare to read.
        /// </summary>
        public void PrepareRead()
        {
            _results = _statement.ExecuteReader();
        }

        /// <summary>
        /// The input size.
        /// </summary>
        public int InputSize
        {
            get { return _inputSize; }
        }

        /// <summary>
        /// The ideal size.
        /// </summary>
        public int IdealSize
        {
            get { return _idealSize; }
        }

        /// <summary>
        /// Close the codec.
        /// </summary>
        public void Close()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
            if (_results != null)
            {
                _results.Close();
            }
        }

        #endregion
    }

    public class BinaryDataLoader
    {
        /// <summary>
        /// The CODEC to use.
        /// </summary>
        private readonly IDataSetCODEC _codec;

        /// <summary>
        /// Construct a loader with the specified CODEC. 
        /// </summary>
        /// <param name="codec">The codec to use.</param>
        public BinaryDataLoader(IDataSetCODEC codec)
        {
            _codec = codec;
            Status = new NullStatusReportable();
        }

        /// <summary>
        /// Used to report the status.
        /// </summary>
        public IStatusReportable Status { get; set; }

        /// <summary>
        /// The CODEC that is being used.
        /// </summary>
        public IDataSetCODEC CODEC
        {
            get { return _codec; }
        }

        /// <summary>
        /// Convert an external file format, such as CSV, to the Synt binary
        /// training format. 
        /// </summary>
        /// <param name="binaryFile">The binary file to create.</param>
        public void External2Binary(String binaryFile)
        {
            Status.Report(0, 0, "Importing to binary file: "
                                + binaryFile);

            var egb = new SyntEGBFile(binaryFile);

            egb.Create(_codec.InputSize, _codec.IdealSize);

            var input = new double[_codec.InputSize];
            var ideal = new double[_codec.IdealSize];

            _codec.PrepareRead();

            int currentRecord = 0;
            int lastUpdate = 0;
            double significance = 0;

            while (_codec.Read(input, ideal, ref significance))
            {
                egb.Write(input);
                egb.Write(ideal);

                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    Status.Report(0, currentRecord, "Importing...");
                }
                egb.Write(significance);
            }

            egb.Close();
            _codec.Close();
            Status.Report(0, 0, "Done importing to binary file: "
                                + binaryFile);
        }

        /// <summary>
        /// Convert an Synt binary file to an external form, such as CSV. 
        /// </summary>
        /// <param name="binaryFile">THe binary file to use.</param>
        public void Binary2External(String binaryFile)
        {
            Status.Report(0, 0, "Exporting binary file: " + binaryFile);

            var egb = new SyntEGBFile(binaryFile);
            egb.Open();

            _codec.PrepareWrite(egb.NumberOfRecords, egb.InputCount,
                               egb.IdealCount);

            int inputCount = egb.InputCount;
            int idealCount = egb.IdealCount;

            var input = new double[inputCount];
            var ideal = new double[idealCount];

            int currentRecord = 0;
            int lastUpdate = 0;

            // now load the data
            for (int i = 0; i < egb.NumberOfRecords; i++)
            {
                for (int j = 0; j < inputCount; j++)
                {
                    input[j] = egb.Read();
                }

                for (int j = 0; j < idealCount; j++)
                {
                    ideal[j] = egb.Read();
                }

                double significance = egb.Read();

                _codec.Write(input, ideal, significance);

                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    Status.Report(egb.NumberOfRecords, currentRecord,
                                  "Exporting...");
                }
            }

            egb.Close();
            _codec.Close();
            Status.Report(0, 0, "Done exporting binary file: "
                                + binaryFile);
        }
    }

    public class BufferedDataError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public BufferedDataError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public BufferedDataError(Exception e)
            : base(e)
        {
        }
    }

    public class BufferedMLDataSet : IMLDataSet
    {
        /// <summary>
        /// Error message for ADD.
        /// </summary>
        public const String ErrorAdd = "Add can only be used after calling beginLoad.";

        /// <summary>
        /// True, if we are in the process of loading.
        /// </summary>
        [NonSerialized]
        private bool _loading;

        /// <summary>
        /// The file being used.
        /// </summary>
        private readonly String _file;

        /// <summary>
        /// The EGB file we are working wtih.
        /// </summary>
        [NonSerialized]
        private SyntEGBFile _egb;

        /// <summary>
        /// Additional sets that were opened.
        /// </summary>
        [NonSerialized]
        private readonly IList<BufferedMLDataSet> _additional = new List<BufferedMLDataSet>();

        /// <summary>
        /// The owner.
        /// </summary>
        [NonSerialized]
        private BufferedMLDataSet _owner;


        /// <summary>
        /// Construct a buffered dataset using the specified file. 
        /// </summary>
        /// <param name="binaryFile">The file to read/write binary data to/from.</param>
        public BufferedMLDataSet(String binaryFile)
        {
            _file = binaryFile;
            _egb = new SyntEGBFile(binaryFile);
            if (File.Exists(_file))
            {
                _egb.Open();
            }
        }


        /// <summary>
        /// Create an enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            if (_loading)
            {
                throw new IMLDataError(
                    "Can't create enumerator while loading, call EndLoad first.");
            }
            var result = new BufferedNeuralDataSetEnumerator(this);
            return result;
        }


        /// <summary>
        /// Open the binary file for reading.
        /// </summary>
        public void Open()
        {
            _egb.Open();
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int Count
        {
            get
            {
                return _egb == null ? 0 : _egb.NumberOfRecords;
            }
        }

        /// <summary>
        /// Read an individual record. 
        /// </summary>
        /// <param name="index">The zero-based index. Specify 0 for the first record, 1 for
        /// the second, and so on.</param>
        /// <param name="pair">The data to read.</param>
        public void GetRecord(int index, IMLDataPair pair)
        {
            double[] inputTarget = pair.InputArray;
            double[] idealTarget = pair.IdealArray;

            _egb.SetLocation(index);
            _egb.Read(inputTarget);
            if (idealTarget != null)
            {
                _egb.Read(idealTarget);
            }
            pair.Significance = _egb.Read();
        }

        /// <summary>
        /// Open an additional training set.
        /// </summary>
        /// <returns>An additional training set.</returns>
        public IMLDataSet OpenAdditional()
        {
            var result = new BufferedMLDataSet(_file) { _owner = this };
            _additional.Add(result);
            return result;
        }

        /// <summary>
        /// Add only input data, for an unsupervised dataset. 
        /// </summary>
        /// <param name="data1">The data to be added.</param>
        public void Add(IMLData data1)
        {
            if (!_loading)
            {
                throw new IMLDataError(ErrorAdd);
            }

            _egb.Write(data1.Data);
            _egb.Write(1.0);
        }


        /// <summary>
        /// Add both the input and ideal data. 
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <param name="idealData">The ideal data.</param>
        public void Add(IMLData inputData, IMLData idealData)
        {
            if (!_loading)
            {
                throw new IMLDataError(ErrorAdd);
            }

            _egb.Write(inputData.Data);
            _egb.Write(idealData.Data);
            _egb.Write(1.0);
        }

        /// <summary>
        /// Add a data pair of both input and ideal data. 
        /// </summary>
        /// <param name="pair">The pair to add.</param>
        public void Add(IMLDataPair pair)
        {
            if (!_loading)
            {
                throw new IMLDataError(ErrorAdd);
            }

            _egb.Write(pair.Input.Data);
            _egb.Write(pair.Ideal.Data);
            _egb.Write(pair.Significance);
        }

        /// <summary>
        /// Close the dataset.
        /// </summary>
        public void Close()
        {
            Object[] obj = _additional.ToArray();

            foreach (var set in obj.Cast<BufferedMLDataSet>())
            {
                set.Close();
            }

            _additional.Clear();

            if (_owner != null)
            {
                _owner.RemoveAdditional(this);
            }

            _egb.Close();
            _egb = null;
        }

        /// <summary>
        /// The ideal data size.
        /// </summary>
        public int IdealSize
        {
            get
            {
                return _egb == null ? 0 : _egb.IdealCount;
            }
        }

        /// <summary>
        /// The input data size.
        /// </summary>
        public int InputSize
        {
            get
            {
                return _egb == null ? 0 : _egb.InputCount;
            }
        }

        /// <summary>
        /// True if this dataset is supervised.
        /// </summary>
        public bool Supervised
        {
            get
            {
                if (_egb == null)
                {
                    return false;
                }
                return _egb.IdealCount > 0;
            }
        }


        /// <summary>
        /// Remove an additional dataset that was created. 
        /// </summary>
        /// <param name="child">The additional dataset to remove.</param>
        public void RemoveAdditional(BufferedMLDataSet child)
        {
            lock (this)
            {
                _additional.Remove(child);
            }
        }

        /// <summary>
        /// Begin loading to the binary file. After calling this method the add
        /// methods may be called. 
        /// </summary>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size.</param>
        public void BeginLoad(int inputSize, int idealSize)
        {
            _egb.Create(inputSize, idealSize);
            _loading = true;
        }

        /// <summary>
        /// This method should be called once all the data has been loaded. The
        /// underlying file will be closed. The binary fill will then be opened for
        /// reading.
        /// </summary>
        public void EndLoad()
        {
            if (!_loading)
            {
                throw new BufferedDataError("Must call beginLoad, before endLoad.");
            }

            _egb.Close();
            _loading = false;
        }

        /// <summary>
        /// The binary file used.
        /// </summary>
        public String BinaryFile
        {
            get { return _file; }
        }

        /// <summary>
        /// The EGB file to use.
        /// </summary>
        public SyntEGBFile EGB
        {
            get { return _egb; }
        }

        /// <summary>
        /// Load the binary dataset to memory.  Memory access is faster. 
        /// </summary>
        /// <returns>A memory dataset.</returns>
        public IMLDataSet LoadToMemory()
        {
            var result = new BasicMLDataSet();

            foreach (IMLDataPair pair in this)
            {
                result.Add(pair);
            }

            return result;
        }

        /// <summary>
        /// Load the specified training set. 
        /// </summary>
        /// <param name="training">The training set to load.</param>
        public void Load(IMLDataSet training)
        {
            BeginLoad(training.InputSize, training.IdealSize);
            foreach (IMLDataPair pair in training)
            {
                Add(pair);
            }
            EndLoad();
        }

        /// <summary>
        /// The owner.  Set when create additional is used.
        /// </summary>
        public BufferedMLDataSet Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        /// <inheritdoc/>
        public IMLDataPair this[int x]
        {
            get
            {
                IMLDataPair result = BasicMLDataPair.CreatePair(InputSize, IdealSize);
                GetRecord(x, result);
                return result;
            }
        }
    }

    public class LinearErrorFunction : IErrorFunction
    {
        /// <inheritdoc/>
        public void CalculateError(double[] ideal, double[] actual, double[] error)
        {
            for (int i = 0; i < actual.Length; i++)
            {
                error[i] = ideal[i] - actual[i];
            }

        }
    }

    public class BufferedNeuralDataSetEnumerator : IEnumerator<IMLDataPair>
    {
        /// <summary>
        /// The dataset being iterated over.
        /// </summary>
        private readonly BufferedMLDataSet _data;

        /// <summary>
        /// The current record.
        /// </summary>
        private int _current;

        /// <summary>
        /// The current record.
        /// </summary>
        private IMLDataPair _currentRecord;

        /// <summary>
        /// Construct the buffered enumerator. This is where the file is actually
        /// opened.
        /// </summary>
        /// <param name="owner">The object that created this enumeration.</param>
        public BufferedNeuralDataSetEnumerator(BufferedMLDataSet owner)
        {
            _data = owner;
            _current = 0;
        }

        #region IEnumerator<MLDataPair> Members

        /// <summary>
        /// Get the current record
        /// </summary>
        public IMLDataPair Current
        {
            get { return _currentRecord; }
        }

        /// <summary>
        /// Dispose of the enumerator.
        /// </summary>
        public void Dispose()
        {
        }


        object IEnumerator.Current
        {
            get
            {
                if (_currentRecord == null)
                {
                    throw new IMLDataError("Can't read current record until MoveNext is called once.");
                }
                return _currentRecord;
            }
        }

        /// <summary>
        /// Move to the next element.
        /// </summary>
        /// <returns>True if there are more elements to read.</returns>
        public bool MoveNext()
        {
            try
            {
                if (_current >= _data.Count)
                    return false;

                _currentRecord = BasicMLDataPair.CreatePair(_data
                                                               .InputSize, _data.IdealSize);
                _data.GetRecord(_current++, _currentRecord);
                return true;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Close the enumerator, and the underlying file.
        /// </summary>
        public void Close()
        {
        }
    }

    public class SyntEGBFile
    {
        /// <summary>
        /// The size of a double.
        /// </summary>
        public const int DoubleSize = sizeof(double);

        /// <summary>
        /// The size of the file header.
        /// </summary>
        public const int HeaderSize = DoubleSize * 3;

        /// <summary>
        /// The file that we are working with.
        /// </summary>
        private readonly String _file;

        /// <summary>
        /// The binary reader.
        /// </summary>
        private BinaryReader _binaryReader;

        /// <summary>
        /// The binary writer.
        /// </summary>
        private BinaryWriter _binaryWriter;

        /// <summary>
        /// The number of ideal values per record.
        /// </summary>
        private int _idealCount;

        /// <summary>
        /// The number of input values per record.
        /// </summary>
        private int _inputCount;

        /// <summary>
        /// The number of records int he file.
        /// </summary>
        private int _numberOfRecords;

        /// <summary>
        /// The number of values in a record, this is the input and ideal combined.
        /// </summary>
        private int _recordCount;

        /// <summary>
        /// The size of a record.
        /// </summary>
        private int _recordSize;

        /// <summary>
        /// The underlying file.
        /// </summary>
        private FileStream _stream;

        /// <summary>
        /// Construct an EGB file. 
        /// </summary>
        /// <param name="file">The file.</param>
        public SyntEGBFile(String file)
        {
            _file = file;
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get { return _inputCount; }
        }

        /// <summary>
        /// The ideal count.
        /// </summary>
        public int IdealCount
        {
            get { return _idealCount; }
        }

        /// <summary>
        /// The stream.
        /// </summary>
        public FileStream Stream
        {
            get { return _stream; }
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int RecordCount
        {
            get { return _recordCount; }
        }

        /// <summary>
        /// The record size.
        /// </summary>
        public int RecordSize
        {
            get { return _recordSize; }
        }

        /// <summary>
        /// The number of records.
        /// </summary>
        public int NumberOfRecords
        {
            get { return _numberOfRecords; }
        }

        /// <summary>
        /// Create a new RGB file. 
        /// </summary>
        /// <param name="inputCount">The input count.</param>
        /// <param name="idealCount">The ideal count.</param>
        public void Create(int inputCount, int idealCount)
        {
            try
            {
                _inputCount = inputCount;
                _idealCount = idealCount;

                var input = new double[inputCount];
                var ideal = new double[idealCount];

                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }

                _stream = new FileStream(_file, FileMode.Create, FileAccess.ReadWrite);
                _binaryWriter = new BinaryWriter(_stream);
                _binaryReader = null;

                _binaryWriter.Write((byte)'E');
                _binaryWriter.Write((byte)'N');
                _binaryWriter.Write((byte)'C');
                _binaryWriter.Write((byte)'O');
                _binaryWriter.Write((byte)'G');
                _binaryWriter.Write((byte)'-');
                _binaryWriter.Write((byte)'0');
                _binaryWriter.Write((byte)'0');

                _binaryWriter.Write((double)input.Length);
                _binaryWriter.Write((double)ideal.Length);

                _numberOfRecords = 0;
                _recordCount = _inputCount + _idealCount + 1;
                _recordSize = _recordCount * DoubleSize;
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Open an existing EGB file.
        /// </summary>
        public void Open()
        {
            try
            {
                _stream = new FileStream(_file, FileMode.Open, FileAccess.Read);
                _binaryReader = new BinaryReader(_stream);
                _binaryWriter = null;

                bool isSyntFile = true;

                isSyntFile = isSyntFile ? _binaryReader.ReadByte() == 'E' : false;
                isSyntFile = isSyntFile ? _binaryReader.ReadByte() == 'N' : false;
                isSyntFile = isSyntFile ? _binaryReader.ReadByte() == 'C' : false;
                isSyntFile = isSyntFile ? _binaryReader.ReadByte() == 'O' : false;
                isSyntFile = isSyntFile ? _binaryReader.ReadByte() == 'G' : false;
                isSyntFile = isSyntFile ? _binaryReader.ReadByte() == '-' : false;

                if (!isSyntFile)
                {
                    throw new BufferedDataError(
                        "File is not a valid Synt binary file:"
                        + _file);
                }

                var v1 = (char)_binaryReader.ReadByte();
                var v2 = (char)_binaryReader.ReadByte();
                String versionStr = "" + v1 + v2;

                try
                {
                    int version = int.Parse(versionStr);
                    if (version > 0)
                    {
                        throw new BufferedDataError(
                            "File is from a newer version of Synt than is currently in use.");
                    }
                }
                catch (Exception)
                {
                    throw new BufferedDataError("File has invalid version number.");
                }

                _inputCount = (int)_binaryReader.ReadDouble();
                _idealCount = (int)_binaryReader.ReadDouble();

                _recordCount = _inputCount + _idealCount + 1;
                _recordSize = _recordCount * DoubleSize;
                _numberOfRecords = (int)((_stream.Length - HeaderSize) / _recordSize);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Close the file.
        /// </summary>
        public void Close()
        {
            try
            {
                if (_binaryWriter != null)
                {
                    _binaryWriter.Close();
                    _binaryWriter = null;
                }

                if (_binaryReader != null)
                {
                    _binaryReader.Close();
                    _binaryReader = null;
                }

                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Calculate the index for the specified row. 
        /// </summary>
        /// <param name="row">The row to calculate for.</param>
        /// <returns>The index.</returns>
        private long CalculateIndex(long row)
        {
            return (long)HeaderSize + (row * (long)_recordSize);
        }

        /// <summary>
        /// Read a row and column. 
        /// </summary>
        /// <param name="row">The row, or record, to read.</param>
        /// <param name="col">The column to read.</param>
        /// <returns>THe value read.</returns>
        private int CalculateIndex(int row, int col)
        {
            return HeaderSize + (row * _recordSize)
                   + (col * DoubleSize);
        }

        /// <summary>
        /// Set the current location to the specified row. 
        /// </summary>
        /// <param name="row">The row.</param>
        public void SetLocation(int row)
        {
            try
            {
                _stream.Position = CalculateIndex(row);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write the specified row and column. 
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The column.</param>
        /// <param name="v">The value.</param>
        public void Write(int row, int col, double v)
        {
            try
            {
                _stream.Position = CalculateIndex(row, col);
                _binaryWriter.Write(v);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write an array at the specified record.
        /// </summary>
        /// <param name="row">The record to write.</param>
        /// <param name="v">The array to write.</param>
        public void Write(int row, double[] v)
        {
            try
            {
                _stream.Position = CalculateIndex(row, 0);
                foreach (double t in v)
                {
                    _binaryWriter.Write(t);
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write an array. 
        /// </summary>
        /// <param name="v">The array to write.</param>
        public void Write(double[] v)
        {
            try
            {
                foreach (double t in v)
                {
                    _binaryWriter.Write(t);
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write a byte. 
        /// </summary>
        /// <param name="b">The byte to write.</param>
        public void Write(byte b)
        {
            try
            {
                _binaryWriter.Write(b);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write a double. 
        /// </summary>
        /// <param name="d">The double to write.</param>
        public void Write(double d)
        {
            try
            {
                _binaryWriter.Write(d);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Read a row and column. 
        /// </summary>
        /// <param name="row">The row to read.</param>
        /// <param name="col">The column to read.</param>
        /// <returns>The value read.</returns>
        public double Read(int row, int col)
        {
            try
            {
                _stream.Position = CalculateIndex(row, col);
                return _binaryReader.ReadDouble();
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Read a double array at the specified record. 
        /// </summary>
        /// <param name="row">The record to read.</param>
        /// <param name="d">The array to read into.</param>
        public void Read(int row, double[] d)
        {
            try
            {
                _stream.Position = CalculateIndex(row, 0);

                for (int i = 0; i < _recordCount; i++)
                {
                    d[i] = _binaryReader.ReadDouble();
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Read an array of doubles. 
        /// </summary>
        /// <param name="d">The array to read into.</param>
        public void Read(double[] d)
        {
            try
            {
                for (int i = 0; i < d.Length; i++)
                {
                    d[i] = _binaryReader.ReadDouble();
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Read a single double. 
        /// </summary>
        /// <returns>The double read.</returns>
        public double Read()
        {
            try
            {
                return _binaryReader.ReadDouble();
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }
    }

    public class MemoryDataLoader
    {
        /// <summary>
        /// The CODEC to use.
        /// </summary>
        private readonly IDataSetCODEC _codec;

        /// <summary>
        /// Construct a loader with the specified CODEC. 
        /// </summary>
        /// <param name="codec">The codec to use.</param>
        public MemoryDataLoader(IDataSetCODEC codec)
        {
            _codec = codec;
            Status = new NullStatusReportable();
        }

        /// <summary>
        /// Used to report the status.
        /// </summary>
        private IStatusReportable Status { get; set; }

        /// <summary>
        /// The dataset to load to.
        /// </summary>
        public BasicMLDataSet Result { get; set; }

        /// <summary>
        /// The CODEC that is being used.
        /// </summary>
        public IDataSetCODEC CODEC
        {
            get { return _codec; }
        }

        /// <summary>
        /// Convert an external file format, such as CSV, to an Synt memory training set. 
        /// </summary>
        public IMLDataSet External2Memory()
        {
            Status.Report(0, 0, "Importing to memory");

            if (Result == null)
            {
                Result = new BasicMLDataSet();
            }

            var input = new double[_codec.InputSize];
            var ideal = new double[_codec.IdealSize];

            _codec.PrepareRead();

            int currentRecord = 0;
            int lastUpdate = 0;
            double significance = 1.0;

            while (_codec.Read(input, ideal, ref significance))
            {
                IMLData b = null;

                IMLData a = new BasicMLData(input);

                if (_codec.IdealSize > 0)
                    b = new BasicMLData(ideal);

                IMLDataPair pair = new BasicMLDataPair(a, b);
                pair.Significance = significance;
                Result.Add(pair);

                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    Status.Report(0, currentRecord, "Importing...");
                }
            }

            _codec.Close();
            Status.Report(0, 0, "Done importing to memory");
            return Result;
        }
    }
    public class FoldedDataSet : IMLDataSet
    {
        /// <summary>
        /// Error message: adds are not supported.
        /// </summary>
        public const String AddNotSupported = "Direct adds to the folded dataset are not supported.";

        /// <summary>
        /// The underlying dataset.
        /// </summary>
        private readonly IMLDataSet _underlying;

        /// <summary>
        /// The fold that we are currently on.
        /// </summary>
        private int _currentFold;

        /// <summary>
        /// The offset to the current fold.
        /// </summary>
        private int _currentFoldOffset;

        /// <summary>
        /// The size of the current fold.
        /// </summary>
        private int _currentFoldSize;

        /// <summary>
        /// The size of all folds, except the last fold, the last fold may have a
        /// different number.
        /// </summary>
        private int _foldSize;

        /// <summary>
        /// The size of the last fold.
        /// </summary>
        private int _lastFoldSize;

        /// <summary>
        /// The total number of folds. Or 0 if the data has not been folded yet.
        /// </summary>
        private int _numFolds;

        /// <summary>
        /// Create a folded dataset. 
        /// </summary>
        /// <param name="underlying">The underlying folded dataset.</param>
        public FoldedDataSet(IMLDataSet underlying)
        {
            _underlying = underlying;
            Fold(1);
        }

        /// <summary>
        /// The owner object(from openAdditional)
        /// </summary>
        public FoldedDataSet Owner { get; set; }

        /// <summary>
        /// The current fold.
        /// </summary>
        public int CurrentFold
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.CurrentFold;
                }
                return _currentFold;
            }
            set
            {
                if (Owner != null)
                {
                    throw new TrainingError("Can't set the fold on a non-top-level set.");
                }

                if (value >= _numFolds)
                {
                    throw new TrainingError(
                        "Can't set the current fold to be greater than the number of folds.");
                }
                _currentFold = value;
                _currentFoldOffset = _foldSize * _currentFold;

                _currentFoldSize = _currentFold == (_numFolds - 1) ? _lastFoldSize : _foldSize;
            }
        }

        /// <summary>
        /// The current fold offset.
        /// </summary>
        public int CurrentFoldOffset
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.CurrentFoldOffset;
                }
                return _currentFoldOffset;
            }
        }

        /// <summary>
        /// The current fold size.
        /// </summary>
        public int CurrentFoldSize
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.CurrentFoldSize;
                }
                return _currentFoldSize;
            }
        }

        /// <summary>
        /// The number of folds.
        /// </summary>
        public int NumFolds
        {
            get { return _numFolds; }
        }

        /// <summary>
        /// The underlying dataset.
        /// </summary>
        public IMLDataSet Underlying
        {
            get { return _underlying; }
        }

        #region MLDataSet Members

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="data1">Not used.</param>
        public void Add(IMLData data1)
        {
            throw new TrainingError(AddNotSupported);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        /// <param name="idealData">Not used.</param>
        public void Add(IMLData inputData, IMLData idealData)
        {
            throw new TrainingError(AddNotSupported);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        public void Add(IMLDataPair inputData)
        {
            throw new TrainingError(AddNotSupported);
        }

        /// <summary>
        /// Close the dataset.
        /// </summary>
        public void Close()
        {
            _underlying.Close();
        }


        /// <summary>
        /// The ideal size.
        /// </summary>
        public int IdealSize
        {
            get { return _underlying.IdealSize; }
        }

        /// <summary>
        /// The input size.
        /// </summary>
        public int InputSize
        {
            get { return _underlying.InputSize; }
        }

        /// <summary>
        /// Get a record.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="pair">The record.</param>
        public void GetRecord(int index, IMLDataPair pair)
        {
            _underlying.GetRecord(CurrentFoldOffset + index, pair);
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int Count
        {
            get { return CurrentFoldSize; }
        }

        /// <summary>
        /// True if this is a supervised set.
        /// </summary>
        public bool Supervised
        {
            get { return _underlying.Supervised; }
        }


        /// <summary>
        /// Open an additional dataset.
        /// </summary>
        /// <returns>The dataset.</returns>
        public IMLDataSet OpenAdditional()
        {
            var folded = new FoldedDataSet(_underlying.OpenAdditional()) { Owner = this };
            return folded;
        }


        /// <summary>
        /// Get an enumberator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            return new FoldedEnumerator(this);
        }

        #endregion

        /// <summary>
        /// Fold the dataset. Must be done before the dataset is used. 
        /// </summary>
        /// <param name="numFolds">The number of folds.</param>
        public void Fold(int numFolds)
        {
            _numFolds = Math.Min(numFolds, _underlying
                                               .Count);
            _foldSize = _underlying.Count / _numFolds;
            _lastFoldSize = _underlying.Count - (_foldSize * _numFolds);
            CurrentFold = 0;
        }

        /// <inheritdoc/>
        public IMLDataPair this[int x]
        {
            get
            {
                IMLDataPair result = BasicMLDataPair.CreatePair(InputSize, IdealSize);
                this.GetRecord(x, result);
                return result;
            }
        }
    }

    public class FoldedEnumerator : IEnumerator<IMLDataPair>
    {
        /// <summary>
        /// The owner.
        /// </summary>
        private readonly FoldedDataSet _owner;

        /// <summary>
        /// The current index.
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// The current data item.
        /// </summary>
        private IMLDataPair _currentPair;

        /// <summary>
        /// Construct an enumerator.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public FoldedEnumerator(FoldedDataSet owner)
        {
            _owner = owner;
            _currentIndex = -1;
        }

        #region IEnumerator<MLDataPair> Members

        /// <summary>
        /// The current object.
        /// </summary>
        public IMLDataPair Current
        {
            get
            {
                if (_currentIndex < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return _currentPair;
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Move to the next record.
        /// </summary>
        /// <returns>True, if we were able to move to the next record.</returns>
        public bool MoveNext()
        {
            if (HasNext())
            {
                IMLDataPair pair = BasicMLDataPair.CreatePair(
                    _owner.InputSize, _owner.IdealSize);
                _owner.GetRecord(_currentIndex++, pair);
                _currentPair = pair;
                return true;
            }
            _currentPair = null;
            return false;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The current object.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                if (_currentIndex < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return _currentPair;
            }
        }

        #endregion

        /// <summary>
        /// Determine if there is a next record.
        /// </summary>
        /// <returns>True, if there is a next record.</returns>
        public bool HasNext()
        {
            return _currentIndex < _owner.CurrentFoldSize;
        }
    }

    public class ImageMLData : BasicMLData
    {
        /// <summary>
        /// Construct an object based on an image.
        /// </summary>
        /// <param name="image">The image to use.</param>
        public ImageMLData(Bitmap image)
            : base(1)
        {
            Image = image;
        }

        /// <summary>
        /// The image associated with this class.
        /// </summary>
        public Bitmap Image { get; set; }


        /// <summary>
        /// Downsample, and copy, the image contents into the data of this object.
        /// Calling this method has no effect on the image, as the same image can be
        /// downsampled multiple times to different resolutions.
        /// </summary>
        /// <param name="downsampler">The downsampler object to use.</param>
        /// <param name="findBounds">Should the bounds be located and cropped.</param>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to.</param>
        /// <param name="hi">The high value to normalize to.</param>
        /// <param name="lo">The low value to normalize to.</param>
        public void Downsample(IDownSample downsampler,
                               bool findBounds, int height, int width,
                               double hi, double lo)
        {
     
        }

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        /// <returns>The string form of this object.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder("[ImageNeuralData:");
            for (int i = 0; i < Data.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }
                builder.Append(Data[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }
    }

    public class ImageMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// Error message to inform the caller that only ImageNeuralData objects can
        /// be used with this collection.
        /// </summary>
        public const String MUST_USE_IMAGE =
            "This data set only supports ImageNeuralData or Image objects.";

        /// <summary>
        /// The downsampler to use.
        /// </summary>
        private readonly IDownSample downsampler;

        /// <summary>
        /// Should the bounds be found and cropped.
        /// </summary>
        private readonly bool findBounds;

        /// <summary>
        /// The high value to normalize to.
        /// </summary>
        private readonly double hi;

        /// <summary>
        /// The low value to normalize to.
        /// </summary>
        private readonly double lo;

        /// <summary>
        /// The height to downsample to.
        /// </summary>
        private int height;

        /// <summary>
        /// The width to downsample to.
        /// </summary>
        private int width;


        /// <summary>
        /// Construct this class with the specified downsampler.
        /// </summary>
        /// <param name="downsampler">The downsampler to use.</param>
        /// <param name="findBounds">Should the bounds be found and clipped.</param>
        /// <param name="hi">The high value to normalize to.</param>
        /// <param name="lo">The low value to normalize to.</param>
        public ImageMLDataSet(IDownSample downsampler,
                              bool findBounds, double hi, double lo)
        {
            this.downsampler = downsampler;
            this.findBounds = findBounds;
            height = -1;
            width = -1;
            this.hi = hi;
            this.lo = lo;
        }

        /// <summary>
        /// The height.
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// The width.
        /// </summary>
        public int Width
        {
            get { return width; }
        }


        /// <summary>
        /// Add the specified data, must be an ImageNeuralData class.
        /// </summary>
        /// <param name="data">The data The object to add.</param>
        public override void Add(IMLData data)
        {
            if (!(data is ImageMLData))
            {
                throw new NeuralNetworkError(MUST_USE_IMAGE);
            }

            base.Add(data);
        }

        /// <summary>
        /// Add the specified input and ideal object to the collection.
        /// </summary>
        /// <param name="inputData">The image to train with.</param>
        /// <param name="idealData">The expected otuput form this image.</param>
        public override void Add(IMLData inputData, IMLData idealData)
        {
            if (!(inputData is ImageMLData))
            {
                throw new NeuralNetworkError(MUST_USE_IMAGE);
            }

            base.Add(inputData, idealData);
        }

        /// <summary>
        /// Add input and expected output. This is used for supervised training.
        /// </summary>
        /// <param name="inputData">The input data to train on.</param>
        public override void Add(IMLDataPair inputData)
        {
            if (!(inputData.Input is ImageMLData))
            {
                throw new NeuralNetworkError(MUST_USE_IMAGE);
            }

            base.Add(inputData);
        }


        /// <summary>
        /// Downsample all images and generate training data.
        /// </summary>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to.</param>
        public void Downsample(int height, int width)
        {
            this.height = height;
            this.width = width;

            foreach (IMLDataPair pair in this)
            {
                if (!(pair.Input is ImageMLData))
                {
                    throw new NeuralNetworkError(
                        "Invalid class type found in ImageNeuralDataSet, only "
                        + "ImageNeuralData items are allowed.");
                }

                var input = (ImageMLData)pair.Input;
                input.Downsample(downsampler, findBounds, height, width,
                                 hi, lo);
            }
        }
    }

    public class CSVFinal : IMarketLoader
    {

        #region IMarketLoader Members


        string Precision { get; set; }
        public static string LoadedFile { get; set; }


        /// <summary>
        /// Reads the CSV and call loader.
        /// Used internally to load the csv and place data in the marketdataset.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="neededTypes">The needed types.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="File">The file.</param>
        /// <returns></returns>
        ICollection<LoadedMarketData> ReadAndCallLoader(
            TickerSymbol symbol,
            IEnumerable<MarketDataType> neededTypes,
            DateTime from,
            DateTime to,
            string File)
        {
            //We got a file, lets load it.

            ICollection<LoadedMarketData> result = new List<LoadedMarketData>();
            ReadCSV csv = new ReadCSV(File, true, CSVFormat.English);
            //In case we want to use a different date format...and have used the SetDateFormat method, our DateFormat must then not be null..
            //We will use the ?? operator to check for nullables.
            csv.DateFormat = DateFormat ?? "yyyy-MM-dd HH:mm:ss";
            csv.TimeFormat = "HH:mm:ss";

            DateTime ParsedDate = from;
            bool writeonce = true;

            while (csv.Next())
            {
                DateTime date = csv.GetDate(0);
                ParsedDate = date;

                if (writeonce)
                {
                    Console.WriteLine(@"First parsed date in csv:" + ParsedDate.ToShortDateString());
                    Console.WriteLine(@"Stopping at date:" + to.ToShortDateString());
                    Console.WriteLine(@"Current DateTime:" + ParsedDate.ToShortDateString() + @" Time:" +
                                      ParsedDate.ToShortTimeString() + @"  Asked Start date was " +
                                      from.ToShortDateString());
                    writeonce = false;
                }
                if (ParsedDate >= from && ParsedDate <= to)
                {
                    DateTime datex = csv.GetDate(0);
                    double open = csv.GetDouble(1);
                    double close = csv.GetDouble(2);
                    double high = csv.GetDouble(3);
                    double low = csv.GetDouble(4);
                    double volume = csv.GetDouble(5);
                    double range = Math.Abs(open - close);
                    double HighLowRange = Math.Abs(high - low);
                    double DirectionalRange = close - open;
                    LoadedMarketData data = new LoadedMarketData(datex, symbol);
                    data.SetData(MarketDataType.Open, open);
                    data.SetData(MarketDataType.High, high);
                    data.SetData(MarketDataType.Low, low);
                    data.SetData(MarketDataType.Close, close);
                    data.SetData(MarketDataType.Volume, volume);
                    data.SetData(MarketDataType.RangeHighLow, Math.Round(HighLowRange, 6));
                    data.SetData(MarketDataType.RangeOpenClose, Math.Round(range, 6));
                    data.SetData(MarketDataType.RangeOpenCloseNonAbsolute, Math.Round(DirectionalRange, 6));
                    result.Add(data);


                }

            }

            csv.Close();
            return result;
        }

        /// <summary>
        /// Gets or sets the date format for the whole csv file.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        public string DateFormat { get; set; }
        /// <summary>
        /// Sets the date format for the csv file.
        /// </summary>
        /// <param name="stringFormat">The string format.</param>
        public void SetDateFormat(string stringFormat)
        {
            DateFormat = stringFormat;
            return;
        }

        public ICollection<LoadedMarketData> Load(TickerSymbol ticker, IList<MarketDataType> dataNeeded, DateTime from, DateTime to)
        {


            return File.Exists(LoadedFile) ? (ReadAndCallLoader(ticker, dataNeeded, from, to, LoadedFile)) : null;
        }



        #endregion

        #region IMarketLoader Members




        #endregion

        #region IMarketLoader Members


        /// <summary>
        /// Gets the file we want to parse.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public string GetFile(string file)
        {
            if (File.Exists(file))
                LoadedFile = file;
            return LoadedFile;
        }

        #endregion

    }

    public class CSVLoader : IMarketLoader
    {

        #region IMarketLoader Members  


        string Precision { get; set; }
        string LoadedFile { get; set; }
        public List<MarketDataType> TypesLoaded = new List<MarketDataType>();
        CSVFormat LoadedFormat { get; set; }
        string DateTimeFormat { get; set; }

        public ICollection<LoadedMarketData> ReadAndCallLoader(TickerSymbol symbol, IList<MarketDataType> neededTypes, DateTime from, DateTime to, string File)
        {
            try
            {


                //We got a file, lets load it.



                ICollection<LoadedMarketData> result = new List<LoadedMarketData>();
                ReadCSV csv = new ReadCSV(File, true, LoadedFormat);


                csv.DateFormat = DateTimeFormat.Normalize();
                //  Time,Open,High,Low,Close,Volume
                while (csv.Next())
                {
                    DateTime date = csv.GetDate("Time");
                    double open = csv.GetDouble("Open");
                    double close = csv.GetDouble("High");
                    double high = csv.GetDouble("Low");
                    double low = csv.GetDouble("Close");
                    double volume = csv.GetDouble("Volume");
                    LoadedMarketData data = new LoadedMarketData(date, symbol);
                    data.SetData(MarketDataType.Open, open);
                    data.SetData(MarketDataType.High, high);
                    data.SetData(MarketDataType.Low, low);
                    data.SetData(MarketDataType.Close, close);
                    data.SetData(MarketDataType.Volume, volume);
                    result.Add(data);
                }

                csv.Close();
                return result;
            }

            catch (Exception ex)
            {

                Console.WriteLine("Something went wrong reading the csv");
                Console.WriteLine("Something went wrong reading the csv:" + ex.Message);
            }

            Console.WriteLine("Something went wrong reading the csv");
            return null;
        }

        public CSVFormat fromStringCSVFormattoCSVFormat(string csvformat)
        {
            switch (csvformat)
            {
                case "Decimal Point":
                    LoadedFormat = CSVFormat.DecimalPoint;
                    break;
                case "Decimal Comma":
                    LoadedFormat = CSVFormat.DecimalComma;
                    break;
                case "English Format":
                    LoadedFormat = CSVFormat.English;
                    break;
                case "EG Format":
                    LoadedFormat = CSVFormat.EgFormat;
                    break;

                default:
                    break;
            }

            return LoadedFormat;
        }

        public ICollection<LoadedMarketData> Load(TickerSymbol ticker, IList<MarketDataType> dataNeeded, DateTime from, DateTime to)
        {
            //ICollection<LoadedMarketData> result = new List<LoadedMarketData>();


            //CSVFormLoader formLoader = new CSVFormLoader();
            //if (File.Exists(formLoader.Chosenfile))
            //{
            //    LoadedFormat = formLoader.format;

            //    //Lets add all the marketdatatypes we selected in the form.
            //    foreach (MarketDataType item in formLoader.TypesLoaded)
            //    {
            //        TypesLoaded.Add(item);

            //    }
            //    DateTimeFormat = formLoader.DateTimeFormatTextBox.Text;
            //    result = ReadAndCallLoader(ticker, dataNeeded, from, to, formLoader.Chosenfile);
            //    return result;
            //}


            return null;
        }



        #endregion

        #region IMarketLoader Members


        public string GetFile(string file)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class LoadedMarketData : IComparable<LoadedMarketData>
    {
        /// <summary>
        /// The data that was collection for the sample date.
        /// </summary>
        private readonly IDictionary<MarketDataType, Double> _data;

        /// <summary>
        /// What is the ticker symbol for this data sample.
        /// </summary>
        private readonly TickerSymbol _ticker;

        /// <summary>
        /// Construct one sample of market data.
        /// </summary>
        /// <param name="when">When was this sample taken.</param>
        /// <param name="ticker">What is the ticker symbol for this data.</param>
        public LoadedMarketData(DateTime when, TickerSymbol ticker)
        {
            When = when;
            _ticker = ticker;
            _data = new Dictionary<MarketDataType, Double>();
        }

        /// <summary>
        /// When is this data from.
        /// </summary>
        public DateTime When { get; set; }

        /// <summary>
        /// The ticker symbol that this data was from.
        /// </summary>
        public TickerSymbol Ticker
        {
            get { return _ticker; }
        }

        /// <summary>
        /// The data that was downloaded.
        /// </summary>
        public IDictionary<MarketDataType, Double> Data
        {
            get { return _data; }
        }

        #region IComparable<LoadedMarketData> Members

        /// <summary>
        /// Compare this object with another of the same type.
        /// </summary>
        /// <param name="other">The other object to compare.</param>
        /// <returns>Zero if equal, greater or less than zero to indicate order.</returns>
        public int CompareTo(LoadedMarketData other)
        {
            return When.CompareTo(other.When);
        }

        #endregion

        /// <summary>
        /// Set the specified type of data.
        /// </summary>
        /// <param name="t">The type of data to set.</param>
        /// <param name="d">The value to set.</param>
        public void SetData(MarketDataType t, double d)
        {
            _data[t] = d;
        }

        /// <summary>
        /// Get the specified data type.
        /// </summary>
        /// <param name="t">The type of data to get.</param>
        /// <returns>The value.</returns>
        public double GetData(MarketDataType t)
        {
            return _data[t];
        }
    }

    public class LoaderError : MarketError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public LoaderError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public LoaderError(Exception e)
            : base(e)
        {
        }
    }

    public class YahooFinanceLoader : IMarketLoader
    {
        #region IMarketLoader Members

        /// <summary>
        /// Load the specified financial data. 
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="dataNeeded">The financial data needed.</param>
        /// <param name="from">The beginning date to load data from.</param>
        /// <param name="to">The ending date to load data to.</param>
        /// <returns>A collection of LoadedMarketData objects that represent the data
        /// loaded.</returns>
        public ICollection<LoadedMarketData> Load(TickerSymbol ticker,
                                                  IList<MarketDataType> dataNeeded, DateTime from,
                                                  DateTime to)
        {
            ICollection<LoadedMarketData> result =
                new List<LoadedMarketData>();
            //Uri url = BuildURL(ticker, from, to);
            //WebRequest http = WebRequest.Create(url);
            //var response = (HttpWebResponse)http.GetResponse();

            //using (Stream istream = response.GetResponseStream())
            //{
            //    var csv = new ReadCSV(istream, true, CSVFormat.DecimalPoint);

            //    while (csv.Next())
            //    {
            //        DateTime date = csv.GetDate("date");
            //        double adjClose = csv.GetDouble("adj close");
            //        double open = csv.GetDouble("open");
            //        double close = csv.GetDouble("close");
            //        double high = csv.GetDouble("high");
            //        double low = csv.GetDouble("low");
            //        double volume = csv.GetDouble("volume");

            //        var data =
            //            new LoadedMarketData(date, ticker);
            //        data.SetData(MarketDataType.AdjustedClose, adjClose);
            //        data.SetData(MarketDataType.Open, open);
            //        data.SetData(MarketDataType.Close, close);
            //        data.SetData(MarketDataType.High, high);
            //        data.SetData(MarketDataType.Low, low);
            //        data.SetData(MarketDataType.Open, open);
            //        data.SetData(MarketDataType.Volume, volume);
            //        result.Add(data);
            //    }

            //    csv.Close();
            //    istream.Close();
            //}
            return result;
        }

        #endregion

        /// <summary>
        /// This method builds a URL to load data from Yahoo Finance for a neural
        /// network to train with.
        /// </summary>
        /// <param name="ticker">The ticker symbol to access.</param>
        /// <param name="from">The beginning date.</param>
        /// <param name="to">The ending date.</param>
        /// <returns>The URL to read from</returns>
        private static Uri BuildURL(TickerSymbol ticker, DateTime from,
                             DateTime to)
        {
            // construct the URL
            var mstream = new MemoryStream();
            var form = new FormUtility(mstream, null);

            form.Add("s", ticker.Symbol.ToUpper());
            form.Add("a", "" + (from.Month - 1));
            form.Add("b", "" + from.Day);
            form.Add("c", "" + from.Year);
            form.Add("d", "" + (to.Month - 1));
            form.Add("e", "" + to.Day);
            form.Add("f", "" + to.Year);
            form.Add("g", "d");
            form.Add("ignore", ".csv");
            mstream.Close();
            byte[] b = mstream.GetBuffer();

            String str = "http://ichart.finance.yahoo.com/table.csv?"
                         + StringUtil.FromBytes(b);
            return new Uri(str);
        }

        #region IMarketLoader Members


        public string GetFile(string file)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class MarketDataDescription : TemporalDataDescription
    {
        /// <summary>
        /// The type of data to be loaded from the specified ticker symbol.
        /// </summary>
        private readonly MarketDataType _dataType;

        /// <summary>
        /// The ticker symbol to be loaded.
        /// </summary>
        private readonly TickerSymbol _ticker;

        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="type">The normalization type.</param>
        /// <param name="activationFunction"> The activation function to apply to this data, can be null.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public MarketDataDescription(TickerSymbol ticker,
                                     MarketDataType dataType, Type type,
                                     IActivationFunction activationFunction, bool input,
                                     bool predict)
            : base(activationFunction, type, input, predict)
        {
            _ticker = ticker;
            _dataType = dataType;
        }


        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="type">The normalization type.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public MarketDataDescription(TickerSymbol ticker,
                                     MarketDataType dataType, Type type, bool input,
                                     bool predict)
            : this(ticker, dataType, type, null, input, predict)
        {
        }

        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public MarketDataDescription(TickerSymbol ticker,
                                     MarketDataType dataType, bool input,
                                     bool predict)
            : this(ticker, dataType, Type.PercentChange, null, input, predict)
        {
        }

        /// <summary>
        /// The ticker symbol.
        /// </summary>
        public TickerSymbol Ticker
        {
            get { return _ticker; }
        }

        /// <summary>
        /// The data type that this is.
        /// </summary>
        public MarketDataType DataType
        {
            get { return _dataType; }
        }
    }

    public class MarketError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public MarketError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public MarketError(Exception e)
            : base(e)
        {
        }
    }

    public sealed class MarketMLDataSet : TemporalMLDataSet
    {
        /// <summary>
        /// The loader to use to obtain the data.
        /// </summary>
        private readonly IMarketLoader _loader;

        /// <summary>
        /// A map between the data points and actual data.
        /// </summary>
        private readonly IDictionary<Int64, TemporalPoint> _pointIndex =
            new Dictionary<Int64, TemporalPoint>();

        /// <summary>
        /// Construct a market data set object.
        /// </summary>
        /// <param name="loader">The loader to use to get the financial data.</param>
        /// <param name="inputWindowSize">The input window size, that is how many datapoints do we use to predict.</param>
        /// <param name="predictWindowSize">How many datapoints do we want to predict.</param>
        public MarketMLDataSet(IMarketLoader loader, Int64 inputWindowSize, Int64 predictWindowSize)
            : base((int)inputWindowSize, (int)predictWindowSize)
        {
            _loader = loader;
            SequenceGrandularity = TimeUnit.Days;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketMLDataSet"/> class.
        /// </summary>
        /// <param name="loader">The loader.</param>
        /// <param name="inputWindowSize">Size of the input window.</param>
        /// <param name="predictWindowSize">Size of the predict window.</param>
        /// <param name="unit">The time unit to use.</param>
        public MarketMLDataSet(IMarketLoader loader, Int64 inputWindowSize, Int64 predictWindowSize, TimeUnit unit)
            : base((int)inputWindowSize, (int)predictWindowSize)
        {

            _loader = loader;
            SequenceGrandularity = unit;
        }

        /// <summary>
        /// The loader that is being used for this set.
        /// </summary>
        public IMarketLoader Loader
        {
            get { return _loader; }
        }

        /// <summary>
        /// Add one description of the type of market data that we are seeking at
        /// each datapoint.
        /// </summary>
        /// <param name="desc"></param>
        public override void AddDescription(TemporalDataDescription desc)
        {
            if (!(desc is MarketDataDescription))
            {
                throw new MarketError(
                    "Only MarketDataDescription objects may be used "
                    + "with the MarketMLDataSet container.");
            }
            base.AddDescription(desc);
        }


        /// <summary>
        /// Create a datapoint at the specified date.
        /// </summary>
        /// <param name="when">The date to create the point at.</param>
        /// <returns>Returns the TemporalPoint created for the specified date.</returns>
        public override TemporalPoint CreatePoint(DateTime when)
        {
            Int64 sequence = (Int64)GetSequenceFromDate(when);
            TemporalPoint result;

            if (_pointIndex.ContainsKey(sequence))
            {
                result = _pointIndex[sequence];
            }
            else
            {
                result = base.CreatePoint(when);
                _pointIndex[(int)result.Sequence] = result;
            }

            return result;
        }


        /// <summary>
        /// Load data from the loader.
        /// </summary>
        /// <param name="begin">The beginning date.</param>
        /// <param name="end">The ending date.</param>
        public void Load(DateTime begin, DateTime end)
        {
            // define the starting point if it is not already defined
            if (StartingPoint == DateTime.MinValue)
            {
                StartingPoint = begin;
            }

            // clear out any loaded points
            Points.Clear();

            // first obtain a collection of symbols that need to be looked up
            IDictionary<TickerSymbol, object> symbolSet = new Dictionary<TickerSymbol, object>();
            foreach (MarketDataDescription desc in Descriptions)
            {
                if (symbolSet.Count == 0)
                {
                    symbolSet[desc.Ticker] = null;
                }
                foreach (TickerSymbol ts in symbolSet.Keys)
                {
                    if (!ts.Equals(desc.Ticker))
                    {
                        symbolSet[desc.Ticker] = null;
                        break;
                    }
                }
            }

            // now loop over each symbol and load the data
            foreach (TickerSymbol symbol in symbolSet.Keys)
            {
                LoadSymbol(symbol, begin, end);
            }

            // resort the points
            SortPoints();
        }


        /// <summary>
        /// Load one point of market data.
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="point">The point to load at.</param>
        /// <param name="item">The item being loaded.</param>
        private void LoadPointFromMarketData(TickerSymbol ticker,
                                             TemporalPoint point, LoadedMarketData item)
        {
            foreach (TemporalDataDescription desc in Descriptions)
            {
                var mdesc = (MarketDataDescription)desc;

                if (mdesc.Ticker.Equals(ticker))
                {
                    point.Data[mdesc.Index] = item.Data[mdesc.DataType];
                }
            }
        }

        /// <summary>
        /// Load one ticker symbol.
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="from">Load data from this date.</param>
        /// <param name="to">Load data to this date.</param>
        private void LoadSymbol(TickerSymbol ticker, DateTime from,
                                DateTime to)
        {
            IList<MarketDataType> types = new List<MarketDataType>();
            foreach (MarketDataDescription desc in Descriptions)
            {
                if (desc.Ticker.Equals(ticker))
                {
                    types.Add(desc.DataType);
                }
            }
            ICollection<LoadedMarketData> data = Loader.Load(ticker, types, from, to);
            foreach (LoadedMarketData item in data)
            {
                TemporalPoint point = CreatePoint(item.When);

                LoadPointFromMarketData(ticker, point, item);
            }
        }
    }

    public class MarketPoint : TemporalPoint
    {
        /// <summary>
        /// When to hold the data from.
        /// </summary>
        private readonly DateTime _when;


        /// <summary>
        /// Construct a MarketPoint with the specified date and size.
        /// </summary>
        /// <param name="when">When is this data from.</param>
        /// <param name="size">What is the size of the data.</param>
        public MarketPoint(DateTime when, int size)
            : base(size)
        {
            _when = when;
        }

        /// <summary>
        /// When is this point from.
        /// </summary>
        public DateTime When
        {
            get { return _when; }
        }
    }

    public class TickerSymbol
    {
        /// <summary>
        /// The exchange.
        /// </summary>
        private readonly String _exchange;

        /// <summary>
        /// The ticker symbol.
        /// </summary>
        private readonly String _symbol;


        /// <summary>
        /// Construct a ticker symbol with no exchange.
        /// </summary>
        /// <param name="symbol">The ticker symbol</param>
        public TickerSymbol(String symbol)
        {
            _symbol = symbol;
            _exchange = null;
        }

        /// <summary>
        /// Construct a ticker symbol with exchange.
        /// </summary>
        /// <param name="symbol">The ticker symbol.</param>
        /// <param name="exchange">The exchange.</param>
        public TickerSymbol(String symbol, String exchange)
        {
            _symbol = symbol;
            _exchange = exchange;
        }

        /// <summary>
        /// The stock symbol.
        /// </summary>
        public String Symbol
        {
            get { return _symbol; }
        }

        /// <summary>
        /// The exchange that this stock is on.
        /// </summary>
        public String Exchange
        {
            get { return _exchange; }
        }


        /// <summary>
        /// Determine if two ticker symbols equal each other.
        /// </summary>
        /// <param name="other">The other ticker symbol.</param>
        /// <returns>True if the two symbols equal.</returns>
        public bool Equals(TickerSymbol other)
        {
            // if the symbols do not even match then they are not equal
            if (!other.Symbol.Equals(this.Symbol))
            {
                return false;
            }

            // if the symbols match then we need to compare the exchanges
            if (other.Exchange == null && other.Exchange == null)
            {
                return true;
            }

            if (other.Exchange == null || this.Exchange == null)
            {
                return false;
            }

            return other.Exchange.Equals(this.Exchange);
        }
    }

    [Serializable]
    public class BiPolarMLData : IMLData
    {
        /// <summary>
        /// The data held by this object.
        /// </summary>
        private bool[] _data;

        /// <summary>
        /// Construct this object with the specified data. 
        /// </summary>
        /// <param name="d">The data to create this object with.</param>
        public BiPolarMLData(bool[] d)
        {
            _data = new bool[d.Length];
            for (int i = 0; i < d.Length; i++)
            {
                _data[i] = d[i];
            }
        }

        /// <summary>
        /// Construct a data object with the specified size.
        /// </summary>
        /// <param name="size">The size of this data object.</param>
        public BiPolarMLData(int size)
        {
            _data = new bool[size];
        }

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        /// <param name="x">The index.</param>
        /// <returns>The value at the specified index.</returns>
        public double this[int x]
        {
            get { return BiPolarUtil.Bipolar2double(_data[x]); }
            set { _data[x] = BiPolarUtil.Double2bipolar(value); }
        }

        /// <summary>
        /// Get the data as an array.
        /// </summary>
        public double[] Data
        {
            get { return BiPolarUtil.Bipolar2double(_data); }
            set { _data = BiPolarUtil.Double2bipolar(value); }
        }

        /// <summary>
        /// The size of the array.
        /// </summary>
        public int Count
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// Get the specified data item as a boolean.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool GetBoolean(int i)
        {
            return _data[i];
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public virtual object Clone()
        {
            return new BiPolarMLData(_data);
        }

        /// <summary>
        /// Set the value as a boolean.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="b">The boolean value.</param>
        public void SetBoolean(int index, bool b)
        {
            _data[index] = b;
        }

        /// <summary>
        /// Clear to false.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = false;
            }
        }

        /// <inheritdoc/>
        public String ToString()
        {
            var result = new StringBuilder();
            result.Append('[');
            for (var i = 0; i < Count; i++)
            {
                result.Append(this[i] > 0 ? "T" : "F");
                if (i != Count - 1)
                {
                    result.Append(",");
                }
            }
            result.Append(']');
            return (result.ToString());
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns>Nothing.</returns>
        public ICentroid<IMLData> CreateCentroid()
        {
            return null;
        }

    }

    public class CSVMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// The CSV filename to read from.
        /// </summary>
        private readonly String _filename;

        /// <summary>
        /// The format that separates the columns, defaults to a comma.
        /// </summary>
        private readonly CSVFormat _format;

        /// <summary>
        /// Specifies if headers are present on the first row.
        /// </summary>
        private readonly bool _headers;

        /// <summary>
        /// The number of columns of ideal data.
        /// </summary>
        private readonly int _idealSize;

        /// <summary>
        /// The number of columns of input data.
        /// </summary>
        private readonly int _inputSize;

        /// <summary>
        /// Construct this data set using a comma as a delimiter.
        /// </summary>
        /// <param name="filename">The CSV filename to read.</param>
        /// <param name="inputSize">The number of columns that make up the input set.</param>
        /// <param name="idealSize">The number of columns that make up the ideal set.</param>
        /// <param name="headers">True if headers are present on the first line.</param>
        public CSVMLDataSet(String filename, int inputSize,
                            int idealSize, bool headers)
            : this(filename, inputSize, idealSize, headers, CSVFormat.English, false)
        {
        }

        /// <summary>
        /// Construct this data set using a comma as a delimiter.
        /// </summary>
        /// <param name="filename">The CSV filename to read.</param>
        /// <param name="inputSize">The number of columns that make up the input set.</param>
        /// <param name="idealSize">The number of columns that make up the ideal set.</param>
        /// <param name="headers">True if headers are present on the first line.</param>
        /// <param name="format">The format to use.</param>
        public CSVMLDataSet(String filename, int inputSize,
                            int idealSize, bool headers, CSVFormat format, bool expectSignificance)
        {
            _filename = filename;
            _inputSize = inputSize;
            _idealSize = idealSize;
            _format = format;
            _headers = headers;

            IDataSetCODEC codec = new CSVDataCODEC(filename, format, headers, inputSize, idealSize, expectSignificance);
            var load = new MemoryDataLoader(codec) { Result = this };
            load.External2Memory();
        }

        /// <summary>
        /// Get the filename for the CSV file.
        /// </summary>
        public String Filename
        {
            get { return _filename; }
        }

        /// <summary>
        /// The delimiter.
        /// </summary>
        public CSVFormat Format
        {
            get { return _format; }
        }

        /// <summary>
        /// True if the first row specifies field names.
        /// </summary>
        public bool Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// The amount of ideal data.
        /// </summary>
        public override int IdealSize
        {
            get { return _idealSize; }
        }

        /// <summary>
        /// The amount of input data.
        /// </summary>
        public override int InputSize
        {
            get { return _inputSize; }
        }
    }

    public class SQLMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// Create a SQL neural data set.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="inputSize">The size of the input data being read.</param>
        /// <param name="idealSize">The size of the ideal output data being read.</param>
        /// <param name="connectString">The connection string.</param>
        public SQLMLDataSet(String sql, int inputSize,
                            int idealSize, String connectString)
        {
            IDataSetCODEC codec = new SQLCODEC(sql, inputSize, idealSize, connectString);
            var load = new MemoryDataLoader(codec) { Result = this };
            load.External2Memory();
        }
    }

    public class TemporalDataDescription
    {
        #region Type enum

        /// <summary>
        /// The type of data requested.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Data in its raw, unmodified form.
            /// </summary>
            Raw,
            /// <summary>
            /// The percent change.
            /// </summary>
            PercentChange,
            /// <summary>
            /// The difference change.
            /// </summary>
            DeltaChange,
        }

        #endregion

        /// <summary>
        /// Should an activation function be used?
        /// </summary>
        private readonly IActivationFunction _activationFunction;

        /// <summary>
        /// What type of data is requested?
        /// </summary>
        private readonly Type _type;

        /// <summary>
        /// Construct a data description item. Set both low and high to zero for
        /// unbounded.
        /// </summary>
        /// <param name="activationFunction">What activation function should be used?</param>
        /// <param name="low">What is the lowest allowed value.</param>
        /// <param name="high">What is the highest allowed value.</param>
        /// <param name="type">What type of data is this.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(IActivationFunction activationFunction,
                                       double low, double high, Type type,
                                       bool input, bool predict)
        {
            Low = low;
            _type = type;
            High = high;
            IsInput = input;
            IsPredict = predict;
            _activationFunction = activationFunction;
        }

        /// <summary>
        /// Construct a data description with an activation function, but no range.
        /// </summary>
        /// <param name="activationFunction">The activation function.</param>
        /// <param name="type">The type of data.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(IActivationFunction activationFunction,
                                       Type type, bool input, bool predict)
            : this(activationFunction, 0, 0, type, input, predict)
        {
        }

        /// <summary>
        /// Construct a data description with no activation function or range.
        /// </summary>
        /// <param name="type">The type of data.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(Type type, bool input,
                                       bool predict)
            : this(null, 0, 0, type, input, predict)
        {
        }

        /// <summary>
        /// The lowest allowed data.
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// The highest allowed value.
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// Is this data input?  Or is it to be predicted.
        /// </summary>
        public bool IsInput { get; set; }

        /// <summary>
        /// Determine if this is a predicted value.
        /// </summary>
        public bool IsPredict { get; set; }

        /// <summary>
        /// Get the index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The type of data this is.
        /// </summary>
        public Type DescriptionType
        {
            get { return _type; }
        }

        /// <summary>
        /// The activation function for this layer.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            get { return _activationFunction; }
        }
    }

    public class TemporalError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public TemporalError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public TemporalError(Exception e)
            : base(e)
        {
        }
    }

    public class TemporalMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// Error message: adds are not supported.
        /// </summary>
        public const String AddNotSupported = "Direct adds to the temporal dataset are not supported.  "
                                                + "Add TemporalPoint objects and call generate.";

        /// <summary>
        /// Descriptions of the data needed.
        /// </summary>
        private readonly IList<TemporalDataDescription> _descriptions =
            new List<TemporalDataDescription>();

        /// <summary>
        /// The temporal points at which we have data.
        /// </summary>
        private readonly List<TemporalPoint> _points = new List<TemporalPoint>();

        /// <summary>
        /// How big would we like the input size to be.
        /// </summary>
        private int _desiredSetSize;

        /// <summary>
        /// The highest sequence.
        /// </summary>
        private int _highSequence;

        /// <summary>
        /// How many input neurons will be used.
        /// </summary>
        private int _inputNeuronCount;

        /// <summary>
        /// The size of the input window, this is the data being used to predict.
        /// </summary>
        private int _inputWindowSize;

        /// <summary>
        /// The lowest sequence.
        /// </summary>
        private int _lowSequence;

        /// <summary>
        /// How many output neurons will there be.
        /// </summary>
        private int _outputNeuronCount;

        /// <summary>
        /// The size of the prediction window.
        /// </summary>
        private int _predictWindowSize;

        /// <summary>
        /// What is the granularity of the temporal points? Days, months, years,
        /// etc?
        /// </summary>
        private TimeUnit _sequenceGrandularity;

        /// <summary>
        /// What is the date for the first temporal point.
        /// </summary>
        private DateTime _startingPoint = DateTime.MinValue;

        /// <summary>
        /// Construct a dataset.
        /// </summary>
        /// <param name="inputWindowSize">What is the input window size.</param>
        /// <param name="predictWindowSize">What is the prediction window size.</param>
        public TemporalMLDataSet(int inputWindowSize,
                                     int predictWindowSize)
        {
            _inputWindowSize = inputWindowSize;
            _predictWindowSize = predictWindowSize;
            _lowSequence = int.MinValue;
            _highSequence = int.MaxValue;
            _desiredSetSize = int.MaxValue;
            _startingPoint = DateTime.MinValue;
            _sequenceGrandularity = TimeUnit.Days;
        }


        /// <summary>
        /// The data descriptions.
        /// </summary>
        public virtual IList<TemporalDataDescription> Descriptions
        {
            get { return _descriptions; }
        }

        /// <summary>
        /// The points, or time slices to take data from.
        /// </summary>
        public virtual IList<TemporalPoint> Points
        {
            get { return _points; }
        }

        /// <summary>
        /// Get the size of the input window.
        /// </summary>
        public virtual int InputWindowSize
        {
            get { return _inputWindowSize; }
            set { _inputWindowSize = value; }
        }

        /// <summary>
        /// The prediction window size.
        /// </summary>
        public virtual int PredictWindowSize
        {
            get { return _predictWindowSize; }
            set { _predictWindowSize = value; }
        }

        /// <summary>
        /// The low value for the sequence.
        /// </summary>
        public virtual int LowSequence
        {
            get { return _lowSequence; }
            set { _lowSequence = value; }
        }

        /// <summary>
        /// The high value for the sequence.
        /// </summary>
        public virtual int HighSequence
        {
            get { return _highSequence; }
            set { _highSequence = value; }
        }

        /// <summary>
        /// The desired dataset size.
        /// </summary>
        public virtual int DesiredSetSize
        {
            get { return _desiredSetSize; }
            set { _desiredSetSize = value; }
        }

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public virtual int InputNeuronCount
        {
            get { return _inputNeuronCount; }
            set { _inputNeuronCount = value; }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public virtual int OutputNeuronCount
        {
            get { return _outputNeuronCount; }
            set { _outputNeuronCount = value; }
        }

        /// <summary>
        /// The starting point.
        /// </summary>
        public virtual DateTime StartingPoint
        {
            get { return _startingPoint; }
            set { _startingPoint = value; }
        }

        /// <summary>
        /// The size of the timeslices.
        /// </summary>
        public virtual TimeUnit SequenceGrandularity
        {
            get { return _sequenceGrandularity; }
            set { _sequenceGrandularity = value; }
        }


        /// <summary>
        /// Add a data description.
        /// </summary>
        /// <param name="desc">The data description to add.</param>
        public virtual void AddDescription(TemporalDataDescription desc)
        {
            if (_points.Count > 0)
            {
                throw new TemporalError(
                    "Can't add anymore descriptions, there are "
                    + "already temporal points defined.");
            }

            int index = _descriptions.Count;
            desc.Index = index;

            _descriptions.Add(desc);
            CalculateNeuronCounts();
        }

        /// <summary>
        /// Clear the entire dataset.
        /// </summary>
        public virtual void Clear()
        {
            _descriptions.Clear();
            _points.Clear();
            Data.Clear();
        }

        /// <summary>
        /// Adding directly is not supported. Rather, add temporal points and
        /// generate the training data.
        /// </summary>
        /// <param name="inputData">Not used</param>
        /// <param name="idealData">Not used</param>
        public override void Add(IMLData inputData, IMLData idealData)
        {
            throw new TemporalError(AddNotSupported);
        }

        /// <summary>
        /// Adding directly is not supported. Rather, add temporal points and
        /// generate the training data.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        public override void Add(IMLDataPair inputData)
        {
            throw new TemporalError(AddNotSupported);
        }

        /// <summary>
        /// Adding directly is not supported. Rather, add temporal points and
        /// generate the training data.
        /// </summary>
        /// <param name="data">Not used.</param>
        public override void Add(IMLData data)
        {
            throw new TemporalError(AddNotSupported);
        }

        /// <summary>
        /// Create a temporal data point using a sequence number. They can also be
        /// created using time. No two points should have the same sequence number.
        /// </summary>
        /// <param name="sequence">The sequence number.</param>
        /// <returns>A new TemporalPoint object.</returns>
        public virtual TemporalPoint CreatePoint(int sequence)
        {
            var point = new TemporalPoint(_descriptions.Count) { Sequence = sequence };
            _points.Add(point);
            return point;
        }

        /// <summary>
        /// Create a sequence number from a time. The first date will be zero, and
        /// subsequent dates will be increased according to the grandularity
        /// specified. 
        /// </summary>
        /// <param name="when">The date to generate the sequence number for.</param>
        /// <returns>A sequence number.</returns>
        public virtual int GetSequenceFromDate(DateTime when)
        {
            int sequence;

            if (_startingPoint != DateTime.MinValue)
            {
                var span = new TimeSpanUtil(_startingPoint, when);
                sequence = (int)span.GetSpan(_sequenceGrandularity);
            }
            else
            {
                _startingPoint = when;
                sequence = 0;
            }

            return sequence;
        }


        /// <summary>
        /// Create a temporal point from a time. Using the grandularity each date is
        /// given a unique sequence number. No two dates that fall in the same
        /// grandularity should be specified.
        /// </summary>
        /// <param name="when">The time that this point should be created at.</param>
        /// <returns>The point TemporalPoint created.</returns>
        public virtual TemporalPoint CreatePoint(DateTime when)
        {
            int sequence = GetSequenceFromDate(when);
            var point = new TemporalPoint(_descriptions.Count) { Sequence = sequence };
            _points.Add(point);
            return point;
        }

        /// <summary>
        /// Calculate how many points are in the high and low range. These are the
        /// points that the training set will be generated on.
        /// </summary>
        /// <returns>The number of points.</returns>
        public virtual int CalculatePointsInRange()
        {
            return _points.Count(IsPointInRange);
        }

        /// <summary>
        /// Calculate the actual set size, this is the number of training set entries
        /// that will be generated.
        /// </summary>
        /// <returns>The size of the training set.</returns>
        public virtual int CalculateActualSetSize()
        {
            int result = CalculatePointsInRange();
            result = Math.Min(_desiredSetSize, result);
            return result;
        }

        /// <summary>
        /// Calculate how many input and output neurons will be needed for the
        /// current data.
        /// </summary>
        public virtual void CalculateNeuronCounts()
        {
            _inputNeuronCount = 0;
            _outputNeuronCount = 0;

            foreach (TemporalDataDescription desc in _descriptions)
            {
                if (desc.IsInput)
                {
                    _inputNeuronCount += _inputWindowSize;
                }
                if (desc.IsPredict)
                {
                    _outputNeuronCount += _predictWindowSize;
                }
            }
        }

        /// <summary>
        /// Is the specified point within the range. If a point is in the selection
        /// range, then the point will be used to generate the training sets.
        /// </summary>
        /// <param name="point">The point to consider.</param>
        /// <returns>True if the point is within the range.</returns>
        public virtual bool IsPointInRange(TemporalPoint point)
        {
            return ((point.Sequence >= LowSequence) && (point.Sequence <= HighSequence));
        }

        /// <summary>
        /// Generate input neural data for the specified index.
        /// </summary>
        /// <param name="index">The index to generate neural data for.</param>
        /// <returns>The input neural data generated.</returns>
        public virtual BasicNeuralData GenerateInputNeuralData(int index)
        {
            if (index + _inputWindowSize > _points.Count)
            {
                throw new TemporalError("Can't generate input temporal data "
                                        + "beyond the end of provided data.");
            }

            var result = new BasicNeuralData(_inputNeuronCount);
            int resultIndex = 0;

            for (int i = 0; i < _inputWindowSize; i++)
            {
                foreach (TemporalDataDescription desc in _descriptions)
                {
                    if (desc.IsInput)
                    {
                        result[resultIndex++] = FormatData(desc, index
                                                                 + i);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get data between two points in raw form.
        /// </summary>
        /// <param name="desc">The data description.</param>
        /// <param name="index">The index to get data from.</param>
        /// <returns>The requested data.</returns>
        private double GetDataRaw(TemporalDataDescription desc,
                                  int index)
        {
            TemporalPoint point = _points[index - 1];
            return point[desc.Index];
        }

        /// <summary>
        /// Get data between two points in delta form.
        /// </summary>
        /// <param name="desc">The data description.</param>
        /// <param name="index">The index to get data from.</param>
        /// <returns>The requested data.</returns>
        private double GetDataDeltaChange(TemporalDataDescription desc,
                                          int index)
        {
            if (index == 0)
            {
                return 0.0;
            }
            TemporalPoint point = _points[index];
            TemporalPoint previousPoint = _points[index - 1];
            return point[desc.Index]
                   - previousPoint[desc.Index];
        }


        /// <summary>
        /// Get data between two points in percent form.
        /// </summary>
        /// <param name="desc">The data description.</param>
        /// <param name="index">The index to get data from.</param>
        /// <returns>The requested data.</returns>
        private double GetDataPercentChange(TemporalDataDescription desc,
                                            int index)
        {
            if (index == 0)
            {
                return 0.0;
            }
            TemporalPoint point = _points[index];
            TemporalPoint previousPoint = _points[index - 1];
            double currentValue = point[desc.Index];
            double previousValue = previousPoint[desc.Index];
            return (currentValue - previousValue) / previousValue;
        }

        /// <summary>
        /// Format data according to the type specified in the description.
        /// </summary>
        /// <param name="desc">The data description.</param>
        /// <param name="index">The index to format the data at.</param>
        /// <returns>The formatted data.</returns>
        private double FormatData(TemporalDataDescription desc,
                                  int index)
        {
            var result = new double[1];

            switch (desc.DescriptionType)
            {
                case TemporalDataDescription.Type.DeltaChange:
                    result[0] = GetDataDeltaChange(desc, index);
                    break;
                case TemporalDataDescription.Type.PercentChange:
                    result[0] = GetDataPercentChange(desc, index);
                    break;
                case TemporalDataDescription.Type.Raw:
                    result[0] = GetDataRaw(desc, index);
                    break;
                default:
                    throw new TemporalError("Unsupported data type.");
            }

            if (desc.ActivationFunction != null)
            {
                desc.ActivationFunction.ActivationFunction(result, 0, 1);
            }

            return result[0];
        }

        /// <summary>
        /// Generate neural ideal data for the specified index.
        /// </summary>
        /// <param name="index">The index to generate for.</param>
        /// <returns>The neural data generated.</returns>
        public virtual BasicNeuralData GenerateOutputNeuralData(int index)
        {
            if (index + _predictWindowSize > _points.Count)
            {
                throw new TemporalError("Can't generate prediction temporal data "
                                        + "beyond the end of provided data.");
            }

            var result = new BasicNeuralData(_outputNeuronCount);
            int resultIndex = 0;

            for (int i = 0; i < _predictWindowSize; i++)
            {
                foreach (TemporalDataDescription desc in _descriptions)
                {
                    if (desc.IsPredict)
                    {
                        result[resultIndex++] = FormatData(desc, index
                                                                 + i);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Calculate the index to start at.
        /// </summary>
        /// <returns>The starting point.</returns>
        public virtual int CalculateStartIndex()
        {
            for (int i = 0; i < _points.Count; i++)
            {
                TemporalPoint point = _points[i];
                if (IsPointInRange(point))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Sort the points.
        /// </summary>
        public virtual void SortPoints()
        {
            _points.Sort();
        }

        /// <summary>
        /// Generate the training sets.
        /// </summary>
        public virtual void Generate()
        {
            //SortPoints();
            //int start = CalculateStartIndex() + 1;
            //int setSize = CalculateActualSetSize();
            //int range = start
            //            + (setSize - _predictWindowSize - _inputWindowSize);

            //for (int i = start; i < range; i++)
            //{
            //    BasicNeuralData input = GenerateInputNeuralData(i);
            //    BasicNeuralData ideal = GenerateOutputNeuralData(i
            //                                                     + _inputWindowSize);
            //    var pair = new BasicNeuralDataPair(input, ideal);
            //    base.Add(pair);
            //}
        }
    }

    public class TemporalPoint : IComparable<TemporalPoint>
    {
        /// <summary>
        /// The data for this point.
        /// </summary>
        private double[] _data;

        /// <summary>
        /// The sequence number for this point.
        /// </summary>
        private int _sequence;

        /// <summary>
        /// Construct a temporal point of the specified size.
        /// </summary>
        /// <param name="size">The size to create the temporal point for.</param>
        public TemporalPoint(int size)
        {
            _data = new double[size];
        }

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        public double[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// The sequence number, used to sort.
        /// </summary>
        public int Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        /// <param name="x">The index.</param>
        /// <returns>The data at the specified index.</returns>
        public double this[int x]
        {
            get { return _data[x]; }
            set { _data[x] = value; }
        }

        #region IComparable<TemporalPoint> Members

        /// <summary>
        /// Compare two temporal points.
        /// </summary>
        /// <param name="that">The other temporal point to compare.</param>
        /// <returns>Returns 0 if they are equal, less than 0 if this point is less,
        /// greater than zero if this point is greater.</returns>
        public int CompareTo(TemporalPoint that)
        {
            if (Sequence == that.Sequence)
            {
                return 0;
            }
            if (Sequence < that.Sequence)
            {
                return -1;
            }
            return 1;
        }

        #endregion

        /**
         * Convert this point to string form.
         * @return This point as a string.
         */

        public override String ToString()
        {
            var builder = new StringBuilder("[TemporalPoint:");
            builder.Append("Seq:");
            builder.Append(_sequence);
            builder.Append(",Data:");
            for (int i = 0; i < _data.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }
                builder.Append(_data[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }
    }

    public class IMLDataError : SyntError
    {
       
        public IMLDataError(String str)
            : base(str)
        {
        }

       
        public IMLDataError(Exception e)
            : base(e)
        {
        }
    }

    public class MLDataError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public MLDataError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public MLDataError(Exception e)
            : base(e)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="e">The exception.</param>
        public MLDataError(String msg, Exception e)
            : base(msg, e)
        {
        }
    }

    public class BayesianFactory
    {
        /// <summary>
        /// Create a bayesian network.
        /// </summary>
        /// <param name="architecture">The architecture to use.</param>
        /// <param name="input">The input neuron count.</param>
        /// <param name="output">The output neuron count.</param>
        /// <returns>The new bayesian network.</returns>
        public IMLMethod Create(String architecture, int input,
                                int output)
        {
            var method = new BayesianNetwork { Contents = architecture };
            return method;
        }
    }

    public class FeedforwardFactory
    {
        /// <summary>
        /// Error.
        /// </summary>
        ///
        public const String CantDefineAct = "Can't define activation function before first layer.";

        /// <summary>
        /// The activation function factory to use.
        /// </summary>
        private MLActivationFactory _factory = new MLActivationFactory();

        /// <summary>
        /// Create a feed forward network.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The feedforward network.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            var result = new BasicNetwork();
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            IActivationFunction af = new ActivationLinear();

            int questionPhase = 0;

            foreach (String layerStr in layers)
            {
                // determine default
                int defaultCount = questionPhase == 0 ? input : output;

                ArchitectureLayer layer = ArchitectureParse.ParseLayer(
                    layerStr, defaultCount);
                bool bias = layer.Bias;

                String part = layer.Name;
                part = part != null ? part.Trim() : "";

                IActivationFunction lookup = _factory.Create(part);

                if (lookup != null)
                {
                    af = lookup;
                }
                else
                {
                    if (layer.UsedDefault)
                    {
                        questionPhase++;
                        if (questionPhase > 2)
                        {
                            throw new SyntError("Only two ?'s may be used.");
                        }
                    }

                    if (layer.Count == 0)
                    {
                        throw new SyntError("Unknown architecture element: "
                                             + architecture + ", can't parse: " + part);
                    }

                    result.AddLayer(new BasicLayer(af, bias, layer.Count));
                }
            }

            result.Structure.FinalizeStructure();
            result.Reset();

            return result;
        }
    }

    public class PNNFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MaxLayers = 3;

        /// <summary>
        /// Create a PNN network.
        /// </summary>
        ///
        /// <param name="architecture">THe architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The RBF network.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MaxLayers)
            {
                throw new SyntError(
                    "PNN Networks must have exactly three elements, "
                    + "separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer pnnLayer = ArchitectureParse.ParseLayer(
                layers[1], -1);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            PNNKernelType kernel;
            PNNOutputMode outmodel;

            if (pnnLayer.Name.Equals("c", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Classification;
            }
            else if (pnnLayer.Name.Equals("r", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Regression;
            }
            else if (pnnLayer.Name.Equals("u", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Unsupervised;
            }
            else
            {
                throw new NeuralNetworkError("Unknown model: " + pnnLayer.Name);
            }

            var holder = new ParamsHolder(pnnLayer.Params);

            String kernelStr = holder.GetString("KERNEL", false, "gaussian");

            if (kernelStr.Equals("gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                kernel = PNNKernelType.Gaussian;
            }
            else if (kernelStr.Equals("reciprocal", StringComparison.InvariantCultureIgnoreCase))
            {
                kernel = PNNKernelType.Reciprocal;
            }
            else
            {
                throw new NeuralNetworkError("Unknown kernel: " + kernelStr);
            }

            var result = new BasicPNN(kernel, outmodel, inputCount,
                                      outputCount);

            return result;
        }
    }

    public class RBFNetworkFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MaxLayers = 3;

        /// <summary>
        /// Create a RBF network.
        /// </summary>
        ///
        /// <param name="architecture">THe architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The RBF network.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MaxLayers)
            {
                throw new SyntError(
                    "RBF Networks must have exactly three elements, "
                    + "separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer rbfLayer = ArchitectureParse.ParseLayer(
                layers[1], -1);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            RBFEnum t;

            if (rbfLayer.Name.Equals("Gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Gaussian;
            }
            else if (rbfLayer.Name.Equals("Multiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Multiquadric;
            }
            else if (rbfLayer.Name.Equals("InverseMultiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.InverseMultiquadric;
            }
            else if (rbfLayer.Name.Equals("MexicanHat", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.MexicanHat;
            }
            else
            {
                throw new NeuralNetworkError("Unknown RBF: " + rbfLayer.Name);
            }

            var holder = new ParamsHolder(rbfLayer.Params);

            int rbfCount = holder.GetInt("C", true, 0);

            var result = new RBFNetwork(inputCount, rbfCount,
                                        outputCount, t);

            return result;
        }
    }

    public class SOMFactory
    {
        /// <summary>
        /// Create a SOM.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SOM.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != 2)
            {
                throw new SyntError(
                    "SOM's must have exactly two elements, separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[1], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            var pattern = new SOMPattern { InputNeurons = inputCount, OutputNeurons = outputCount };
            return pattern.Generate();
        }
    }

    public class SRNFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MaxLayers = 3;

        /// <summary>
        /// Create the SRN.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SRN.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MaxLayers)
            {
                throw new SyntError(
                    "SRN Networks must have exactly three elements, "
                    + "separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer rbfLayer = ArchitectureParse.ParseLayer(
                layers[1], -1);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            RBFEnum t;

            if (rbfLayer.Name.Equals("Gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Gaussian;
            }
            else if (rbfLayer.Name.Equals("Multiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Multiquadric;
            }
            else if (rbfLayer.Name.Equals("InverseMultiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.InverseMultiquadric;
            }
            else if (rbfLayer.Name.Equals("MexicanHat", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.MexicanHat;
            }
            else
            {
                t = RBFEnum.Gaussian;
            }

            var result = new RBFNetwork(inputCount,
                                        rbfLayer.Count, outputCount, t);

            return result;
        }
    }

    public class SVMFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MAX_LAYERS = 3;

        /// <summary>
        /// Create the SVM.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SVM.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MAX_LAYERS)
            {
                throw new SyntError(
                    "SVM's must have exactly three elements, separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer paramsLayer = ArchitectureParse.ParseLayer(
                layers[1], input);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            String name = paramsLayer.Name;
            String kernelStr = paramsLayer.Params.ContainsKey("KERNEL") ? paramsLayer.Params["KERNEL"] : null;
            String svmTypeStr = paramsLayer.Params.ContainsKey("TYPE") ? paramsLayer.Params["TYPE"] : null;

            SVMType svmType = SVMType.NewSupportVectorClassification;
            KernelType kernelType = KernelType.RadialBasisFunction;

            bool useNew = true;

            if (svmTypeStr == null)
            {
                useNew = true;
            }
            else if (svmTypeStr.Equals("NEW", StringComparison.InvariantCultureIgnoreCase))
            {
                useNew = true;
            }
            else if (svmTypeStr.Equals("OLD", StringComparison.InvariantCultureIgnoreCase))
            {
                useNew = false;
            }
            else
            {
                throw new SyntError("Unsupported type: " + svmTypeStr
                                     + ", must be NEW or OLD.");
            }

            if (name.Equals("C", StringComparison.InvariantCultureIgnoreCase))
            {
                if (useNew)
                {
                    svmType = SVMType.NewSupportVectorClassification;
                }
                else
                {
                    svmType = SVMType.SupportVectorClassification;
                }
            }
            else if (name.Equals("R", StringComparison.InvariantCultureIgnoreCase))
            {
                if (useNew)
                {
                    svmType = SVMType.NewSupportVectorRegression;
                }
                else
                {
                    svmType = SVMType.EpsilonSupportVectorRegression;
                }
            }
            else
            {
                throw new SyntError("Unsupported mode: " + name
                                     + ", must be C for classify or R for regression.");
            }

            if (kernelStr == null)
            {
                kernelType = KernelType.RadialBasisFunction;
            }
            else if ("linear".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Linear;
            }
            else if ("poly".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Poly;
            }
            else if ("precomputed".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Precomputed;
            }
            else if ("rbf".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.RadialBasisFunction;
            }
            else if ("sigmoid".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Sigmoid;
            }
            else
            {
                throw new SyntError("Unsupported kernel: " + kernelStr
                                     + ", must be linear,poly,precomputed,rbf or sigmoid.");
            }

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            if (outputCount != 1)
            {
                throw new SyntError("SVM can only have an output size of 1.");
            }

            var result = new SupportVectorMachine(inputCount, svmType, kernelType);

            return result;
        }
    }

    public class ArchitectureLayer
    {
        /// <summary>
        /// Holds any paramaters that were specified for the layer.
        /// </summary>
        ///
        private readonly IDictionary<String, String> _paras;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public ArchitectureLayer()
        {
            _paras = new Dictionary<String, String>();
        }


        /// <value>the count to set</value>
        public int Count { get; set; }


        /// <value>the name to set</value>
        public String Name { get; set; }


        /// <value>the params</value>
        public IDictionary<String, String> Params
        {
            get { return _paras; }
        }


        /// <value>the bias to set</value>
        public bool Bias { get; set; }


        /// <value>the usedDefault to set</value>
        public bool UsedDefault { get; set; }
    }

    public static class ArchitectureParse
    {
        /// <summary>
        /// parse a layer.
        /// </summary>
        ///
        /// <param name="line">The line to parse.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The parsed ArchitectureLayer.</returns>
        public static ArchitectureLayer ParseLayer(String line,
                                                   int defaultValue)
        {
            var layer = new ArchitectureLayer();

            String check = line.Trim().ToUpper();

            // first check for bias
            if (check.EndsWith(":B"))
            {
                check = check.Substring(0, (check.Length - 2) - (0));
                layer.Bias = true;
            }

            // see if simple number
            try
            {
                layer.Count = Int32.Parse(check);
                if (layer.Count < 0)
                {
                    throw new SyntError("Count cannot be less than zero.");
                }
            }
            catch (FormatException f)
            {
                SyntLogging.Log(f);
            }

            // see if it is a default
            if ("?".Equals(check))
            {
                if (defaultValue < 0)
                {
                    throw new SyntError("Default (?) in an invalid location.");
                }
                layer.Count = defaultValue;
                layer.UsedDefault = true;
                return layer;
            }

            // single item, no function
            int startIndex = check.IndexOf('(');
            int endIndex = check.LastIndexOf(')');
            if (startIndex == -1)
            {
                layer.Name = check;
                return layer;
            }

            // function
            if (endIndex == -1)
            {
                throw new SyntError("Illegal parentheses.");
            }

            layer.Name = check.Substring(0, (startIndex) - (0)).Trim();

            String paramStr = check.Substring(startIndex + 1, (endIndex) - (startIndex + 1));
            IDictionary<String, String> paras = ParseParams(paramStr);
            EngineArray.PutAll(paras, layer.Params);
            return layer;
        }

        /// <summary>
        /// Parse all layers from a line of text.
        /// </summary>
        ///
        /// <param name="line">The line of text.</param>
        /// <returns>A list of the parsed layers.</returns>
        public static IList<String> ParseLayers(String line)
        {
            IList<String> result = new List<String>();

            int bs = 0;
            bool done = false;

            do
            {
                String part;
                int index = line.IndexOf("->", bs);
                if (index != -1)
                {
                    part = line.Substring(bs, (index) - (bs)).Trim();
                    bs = index + 2;
                }
                else
                {
                    part = line.Substring(bs).Trim();
                    done = true;
                }

                bool bias = part.EndsWith("b");
                if (bias)
                {
                    part = part.Substring(0, (part.Length - 1) - (0));
                }

                result.Add(part);
            } while (!done);

            return result;
        }

        /// <summary>
        /// Parse a name.
        /// </summary>
        ///
        /// <param name="parser">The parser to use.</param>
        /// <returns>The name.</returns>
        private static String ParseName(SimpleParser parser)
        {
            var result = new StringBuilder();
            parser.EatWhiteSpace();
            while (parser.IsIdentifier())
            {
                result.Append(parser.ReadChar());
            }
            return result.ToString();
        }

        /// <summary>
        /// Parse parameters.
        /// </summary>
        ///
        /// <param name="line">The line to parse.</param>
        /// <returns>The parsed values.</returns>
        public static IDictionary<String, String> ParseParams(String line)
        {
            IDictionary<String, String> result = new Dictionary<String, String>();

            var parser = new SimpleParser(line);

            while (!parser.EOL())
            {
                String name = ParseName(parser)
                    .ToUpper();

                parser.EatWhiteSpace();
                if (!parser.LookAhead("=", false))
                {
                    throw new SyntError("Missing equals(=) operator.");
                }
                parser.Advance();

                String v = ParseValue(parser);

                result[name.ToUpper()] = v;

                if (!parser.ParseThroughComma())
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Parse a value.
        /// </summary>
        ///
        /// <param name="parser">The parser to use.</param>
        /// <returns>The newly parsed value.</returns>
        private static String ParseValue(SimpleParser parser)
        {
            bool quoted = false;
            var str = new StringBuilder();

            parser.EatWhiteSpace();

            if (parser.Peek() == '\"')
            {
                quoted = true;
                parser.Advance();
            }

            while (!parser.EOL())
            {
                if (parser.Peek() == '\"')
                {
                    if (quoted)
                    {
                        parser.Advance();
                        if (parser.Peek() == '\"')
                        {
                            str.Append(parser.ReadChar());
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        str.Append(parser.ReadChar());
                    }
                }
                else if (!quoted
                         && (parser.IsWhiteSpace() || (parser.Peek() == ',')))
                {
                    break;
                }
                else
                {
                    str.Append(parser.ReadChar());
                }
            }
            return str.ToString();
        }
    }

    public class AnnealFactory
    {
        /// <summary>
        /// Create an annealing trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is BasicNetwork))
            {
                throw new TrainingError(
                    "Invalid method type, requires BasicNetwork");
            }

            ICalculateScore score = new TrainingSetScore(training);

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            double startTemp = holder.GetDouble(
                MLTrainFactory.PropertyTemperatureStart, false, 10);
            double stopTemp = holder.GetDouble(
                MLTrainFactory.PropertyTemperatureStop, false, 2);

            int cycles = holder.GetInt(MLTrainFactory.Cycles, false, 100);

            IMLTrain train = new NeuralSimulatedAnnealing(
                (BasicNetwork)method, score, startTemp, stopTemp, cycles);

            return train;
        }
    }

    public class BackPropFactory
    {
        /// <summary>
        /// Create a backpropagation trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 0.7d);
            double momentum = holder.GetDouble(
                MLTrainFactory.PropertyLearningMomentum, false, 0.3d);

            return new Backpropagation((BasicNetwork)method, training,
                                       learningRate, momentum);
        }
    }

    public class ClusterSOMFactory
    {
        /// <summary>
        /// Create a cluster SOM trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is SOMNetwork))
            {
                throw new SyntError(
                    "Cluster SOM training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new SOMClusterCopyTraining((SOMNetwork)method, training);
        }
    }

    public class GFactory
    {
        /// <summary>
        /// Create an annealing trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is BasicNetwork))
            {
                throw new TrainingError(
                    "Invalid method type, requires BasicNetwork");
            }

            ICalculateScore score = new TrainingSetScore(training);

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            int populationSize = holder.GetInt(
                MLTrainFactory.PropertyPopulationSize, false, 5000);
            double mutation = holder.GetDouble(
                MLTrainFactory.PropertyMutation, false, 0.1d);
            double mate = holder.GetDouble(MLTrainFactory.PropertyMate,
                                           false, 0.25d);

            IMLTrain train = new NeuralGAlgorithm((BasicNetwork)method,
                                                       (IRandomizer)(new RangeRandomizer(-1, 1)), score, populationSize, mutation,
                                                       mate);

            return train;
        }
    }

    public class LMAFactory
    {
        /// <summary>
        /// Create a LMA trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is BasicNetwork))
            {
                throw new SyntError(
                    "LMA training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            var result = new LevenbergMarquardtTraining(
                (BasicNetwork)method, training);
            return result;
        }
    }

    public class ManhattanFactory
    {
        /// <summary>
        /// Create a Manhattan trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 0.1d);

            return new ManhattanPropagation((BasicNetwork)method, training,
                                            learningRate);
        }
    }

    public class NeighborhoodSOMFactory
    {
        /// <summary>
        /// Create a LMA trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is SOMNetwork))
            {
                throw new SyntError(
                    "Neighborhood training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 0.7d);
            String neighborhoodStr = holder.GetString(
                MLTrainFactory.PropertyNeighborhood, false, "rbf");
            String rbfTypeStr = holder.GetString(
                MLTrainFactory.PropertyRBFType, false, "gaussian");

            RBFEnum t;

            if (rbfTypeStr.Equals("Gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Gaussian;
            }
            else if (rbfTypeStr.Equals("Multiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Multiquadric;
            }
            else if (rbfTypeStr.Equals("InverseMultiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.InverseMultiquadric;
            }
            else if (rbfTypeStr.Equals("MexicanHat", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.MexicanHat;
            }
            else
            {
                t = RBFEnum.Gaussian;
            }

            INeighborhoodFunction nf = null;

            if (neighborhoodStr.Equals("bubble", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodBubble(1);
            }
            else if (neighborhoodStr.Equals("rbf", StringComparison.InvariantCultureIgnoreCase))
            {
                String str = holder.GetString(
                    MLTrainFactory.PropertyDimensions, true, null);
                int[] size = NumberList.FromListInt(CSVFormat.EgFormat, str);
                nf = new NeighborhoodRBF(size, t);
            }
            else if (neighborhoodStr.Equals("rbf1d", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodRBF1D(t);
            }
            if (neighborhoodStr.Equals("single", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodSingle();
            }

            var result = new BasicTrainSOM((SOMNetwork)method,
                                           learningRate, training, nf);

            if (args.ContainsKey(MLTrainFactory.PropertyIterations))
            {
                int plannedIterations = holder.GetInt(
                    MLTrainFactory.PropertyIterations, false, 1000);
                double startRate = holder.GetDouble(
                    MLTrainFactory.PropertyStartLearningRate, false, 0.05d);
                double endRate = holder.GetDouble(
                    MLTrainFactory.PropertyEndLearningRate, false, 0.05d);
                double startRadius = holder.GetDouble(
                    MLTrainFactory.PropertyStartRadius, false, 10);
                double endRadius = holder.GetDouble(
                    MLTrainFactory.PropertyEndRadius, false, 1);
                result.SetAutoDecay(plannedIterations, startRate, endRate,
                                    startRadius, endRadius);
            }

            return result;
        }
    }

    public class NelderMeadFactory
    {
        ///// <summary>
        ///// Create a Nelder Mead trainer.
        ///// </summary>
        ///// <param name="method">The method to use.</param>
        ///// <param name="training">The training data to use.</param>
        ///// <param name="argsStr">The arguments to use.</param>
        ///// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            //final double learningRate = holder.getDouble(
            //		MLTrainFactory.PROPERTY_LEARNING_RATE, false, 0.1);

            return new NelderMeadTraining((BasicNetwork)method, training);
        }
    }

    public class PNNTrainFactory
    {
        /// <summary>
        /// Create a PNN trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String args)
        {
            if (!(method is BasicPNN))
            {
                throw new SyntError(
                    "PNN training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new TrainBasicPNN((BasicPNN)method, training);
        }
    }

    public class YTraining : GAlgorithm, IMLTrain
    {
        /// <summary>
        /// The number of inputs.
        /// </summary>
        private readonly int inputCount;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        private readonly int outputCount;

        /// <summary>
        /// The average fit adjustment.
        /// </summary>
        private double averageFitAdjustment;

        /// <summary>
        /// The best ever network.
        /// </summary>
        private YNetwork bestEverNetwork;

        /// <summary>
        /// The best ever score.
        /// </summary>
        private double bestEverScore;

        /// <summary>
        /// The iteration number.
        /// </summary>
        private int iteration;

        /// <summary>
        /// The activation mutation rate.
        /// </summary>
        private double paramActivationMutationRate = 0.1;

        /// <summary>
        /// The likelyhood of adding a link.
        /// </summary>
        private double paramChanceAddLink = 0.07;

        /// <summary>
        /// The likelyhood of adding a node.
        /// </summary>
        private double paramChanceAddNode = 0.04;

        /// <summary>
        /// The likelyhood of adding a recurrent link.
        /// </summary>
        private double paramChanceAddRecurrentLink = 0.05;

        /// <summary>
        /// The compatibility threshold for a species.
        /// </summary>
        private double paramCompatibilityThreshold = 0.26;

        /// <summary>
        /// The crossover rate.
        /// </summary>
        private double paramCrossoverRate = 0.7;

        /// <summary>
        /// The max activation perturbation.
        /// </summary>
        private double paramMaxActivationPerturbation = 0.1;

        /// <summary>
        /// The maximum number of species.
        /// </summary>
        private int paramMaxNumberOfSpecies;

        /// <summary>
        /// The maximum number of neurons.
        /// </summary>
        private double paramMaxPermittedNeurons = 100;

        /// <summary>
        /// The maximum weight perturbation.
        /// </summary>
        private double paramMaxWeightPerturbation = 0.5;

        /// <summary>
        /// The mutation rate.
        /// </summary>
        private double paramMutationRate = 0.2;

        /// <summary>
        /// The number of link add attempts.
        /// </summary>
        private int paramNumAddLinkAttempts = 5;

        /// <summary>
        /// The number of generations allowed with no improvement.
        /// </summary>
        private int paramNumGensAllowedNoImprovement = 15;

        /// <summary>
        /// The number of tries to find a looped link.
        /// </summary>
        private int paramNumTrysToFindLoopedLink = 5;

        /// <summary>
        /// The number of tries to find an old link.
        /// </summary>
        private int paramNumTrysToFindOldLink = 5;

        /// <summary>
        /// The probability that the weight will be totally replaced.
        /// </summary>
        private double paramProbabilityWeightReplaced = 0.1;

        /// <summary>
        /// Determines if we are using snapshot mode.
        /// </summary>
        private bool snapshot;

        /// <summary>
        /// The total fit adjustment.
        /// </summary>
        private double totalFitAdjustment;

        /// <summary>
        /// Construct a Y trainer with a new population. The new population is
        /// created from the specified parameters.
        /// </summary>
        /// <param name="calculateScore">The score calculation object.</param>
        /// <param name="inputCount">The input neuron count.</param>
        /// <param name="outputCount">The output neuron count.</param>
        /// <param name="populationSize">The population size.</param>
        public YTraining(ICalculateScore calculateScore,
                            int inputCount, int outputCount,
                            int populationSize)
        {
            this.inputCount = inputCount;
            this.outputCount = outputCount;

            CalculateScore = new GScoreAdapter(calculateScore);
            Comparator = new TComparator(CalculateScore);
            Population = new YPopulation(inputCount, outputCount,
                                            populationSize);

            Init();
        }

        /// <summary>
        /// Construct Y training with an existing population.
        /// </summary>
        /// <param name="calculateScore">The score object to use.</param>
        /// <param name="population">The population to use.</param>
        public YTraining(ICalculateScore calculateScore,
                            IPopulation population)
        {
            if (population.Size() < 1)
            {
                throw new TrainingError("Population can not be empty.");
            }

            var T = (YT)population.Ts[0];
            CalculateScore = new GScoreAdapter(calculateScore);
            Comparator = new TComparator(CalculateScore);
            Population = (population);
            inputCount = T.InputCount;
            outputCount = T.OutputCount;

            Init();
        }

        /// <summary>
        /// The innovations.
        /// </summary>
        public YInnovationList Innovations
        {
            get { return (YInnovationList)Population.Innovations; }
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get { return inputCount; }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public int OutputCount
        {
            get { return outputCount; }
        }

        /// <summary>
        /// Set the activation mutation rate.
        /// </summary>
        public double ParamActivationMutationRate
        {
            get { return paramActivationMutationRate; }
            set { paramActivationMutationRate = value; }
        }


        /// <summary>
        /// Set the chance to add a link.
        /// </summary>
        public double ParamChanceAddLink
        {
            get { return paramChanceAddLink; }
            set { paramChanceAddLink = value; }
        }


        /// <summary>
        /// Set the chance to add a node.
        /// </summary>
        public double ParamChanceAddNode
        {
            get { return paramChanceAddNode; }
            set { paramChanceAddNode = value; }
        }

        /// <summary>
        /// Set the chance to add a recurrent link.
        /// </summary>
        public double ParamChanceAddRecurrentLink
        {
            get { return paramChanceAddRecurrentLink; }
            set { paramChanceAddRecurrentLink = value; }
        }


        /// <summary>
        /// Set the compatibility threshold for species.
        /// </summary>
        public double ParamCompatibilityThreshold
        {
            get { return paramCompatibilityThreshold; }
            set { paramCompatibilityThreshold = value; }
        }


        /// <summary>
        /// Set the cross over rate.
        /// </summary>
        public double ParamCrossoverRate
        {
            get { return paramCrossoverRate; }
            set { paramCrossoverRate = value; }
        }


        /// <summary>
        /// Set the max activation perturbation.
        /// </summary>
        public double ParamMaxActivationPerturbation
        {
            get { return paramMaxActivationPerturbation; }
            set { paramMaxActivationPerturbation = value; }
        }

        /// <summary>
        /// Set the maximum number of species.
        /// </summary>
        public int ParamMaxNumberOfSpecies
        {
            get { return paramMaxNumberOfSpecies; }
            set { paramMaxNumberOfSpecies = value; }
        }

        /// <summary>
        /// Set the max permitted neurons.
        /// </summary>
        public double ParamMaxPermittedNeurons
        {
            get { return paramMaxPermittedNeurons; }
            set { paramMaxPermittedNeurons = value; }
        }

        /// <summary>
        /// Set the max weight perturbation.
        /// </summary>
        public double ParamMaxWeightPerturbation
        {
            get { return paramMaxWeightPerturbation; }
            set { paramMaxWeightPerturbation = value; }
        }

        /// <summary>
        /// Set the mutation rate.
        /// </summary>
        public double ParamMutationRate
        {
            get { return paramMutationRate; }
            set { paramMutationRate = value; }
        }

        /// <summary>
        /// Set the number of attempts to add a link.
        /// </summary>
        public int ParamNumAddLinkAttempts
        {
            get { return paramNumAddLinkAttempts; }
            set { paramNumAddLinkAttempts = value; }
        }

        /// <summary>
        /// Set the number of no-improvement generations allowed.
        /// </summary>
        public int ParamNumGensAllowedNoImprovement
        {
            get { return paramNumGensAllowedNoImprovement; }
            set { paramNumGensAllowedNoImprovement = value; }
        }

        /// <summary>
        /// Set the number of tries to create a looped link.
        /// </summary>
        public int ParamNumTrysToFindLoopedLink
        {
            get { return paramNumTrysToFindLoopedLink; }
            set { paramNumTrysToFindLoopedLink = value; }
        }


        /// <summary>
        /// Set the number of tries to try an old link.
        /// </summary>
        public int ParamNumTrysToFindOldLink
        {
            get { return paramNumTrysToFindOldLink; }
            set { paramNumTrysToFindOldLink = value; }
        }


        /// <summary>
        /// Set the probability to replace a weight.
        /// </summary>
        public double ParamProbabilityWeightReplaced
        {
            get { return paramProbabilityWeightReplaced; }
            set { paramProbabilityWeightReplaced = value; }
        }

        /// <summary>
        /// Set if we are using snapshot mode.
        /// </summary>
        public bool Snapshot
        {
            get { return snapshot; }
            set { snapshot = value; }
        }

        #region MLTrain Members

        /// <inheritdoc/>
        public void AddStrategy(IStrategy strategy)
        {
            throw new TrainingError(
                "Strategies are not supported by this training method.");
        }

        /// <inheritdoc/>
        public bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public void FinishTraining()
        {
        }

        /// <summary>
        /// The error for the best T.
        /// </summary>
        public double Error
        {
            get { return bestEverScore; }
            set { bestEverScore = value; }
        }

        /// <inheritdoc/>
        public TrainingImplementationType ImplementationType
        {
            get { return TrainingImplementationType.Iterative; }
        }

        /// <inheritdoc/>
        public int IterationNumber
        {
            get { return iteration; }
            set { iteration = value; }
        }

        /// <summary>
        /// A network created for the best T.
        /// </summary>
        public IMLMethod Method
        {
            get { return bestEverNetwork; }
        }

        /// <inheritdoc/>
        public IList<IStrategy> Strategies
        {
            get { return new List<IStrategy>(); }
        }

        /// <summary>
        /// Returns null, does not use a training set, rather uses a score function.
        /// </summary>
        public IMLDataSet Training
        {
            get { return null; }
        }

        /// <inheritdoc/>
        public bool TrainingDone
        {
            get { return false; }
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {
            iteration++;
            IList<YT> newPop = new List<YT>();

            int numSpawnedSoFar = 0;

            foreach (ISpecies s in Population.Species)
            {
                if (numSpawnedSoFar < Population.Size())
                {
                    var numToSpawn = (int)Math.Round(s.NumToSpawn);

                    bool bChosenBestYet = false;

                    while ((numToSpawn--) > 0)
                    {
                        YT baby = null;

                        if (!bChosenBestYet)
                        {
                            baby = (YT)s.Leader;

                            bChosenBestYet = true;
                        }

                        else
                        {
                            // if the number of individuals in this species is only
                            // one
                            // then we can only perform mutation
                            if (s.Members.Count == 1)
                            {
                                // spawn a child
                                baby = new YT((YT)s.ChooseParent());
                            }
                            else
                            {
                                var g1 = (YT)s.ChooseParent();

                                if (ThreadSafeRandom.NextDouble() < paramCrossoverRate)
                                {
                                    var g2 = (YT)s.ChooseParent();

                                    int numAttempts = 5;

                                    while ((g1.TID == g2.TID)
                                           && ((numAttempts--) > 0))
                                    {
                                        g2 = (YT)s.ChooseParent();
                                    }

                                    if (g1.TID != g2.TID)
                                    {
                                        baby = Crossover(g1, g2);
                                    }
                                }

                                else
                                {
                                    baby = new YT(g1);
                                }
                            }

                            if (baby != null)
                            {
                                baby.TID = Population.AssignTID();

                                if (baby.Neurons.Size() < paramMaxPermittedNeurons)
                                {
                                    baby.AddNeuron(paramChanceAddNode,
                                                   paramNumTrysToFindOldLink);
                                }

                                // now there's the chance a link may be added
                                baby.AddLink(paramChanceAddLink,
                                             paramChanceAddRecurrentLink,
                                             paramNumTrysToFindLoopedLink,
                                             paramNumAddLinkAttempts);

                                // mutate the weights
                                baby.MutateWeights(paramMutationRate,
                                                   paramProbabilityWeightReplaced,
                                                   paramMaxWeightPerturbation);

                                baby.MutateActivationResponse(
                                    paramActivationMutationRate,
                                    paramMaxActivationPerturbation);
                            }
                        }

                        if (baby != null)
                        {
                            // sort the baby's genes by their innovation numbers
                            baby.SortGenes();

                            // add to new pop
                            // if (newPop.contains(baby)) {
                            // throw new SyntError("readd");
                            // }
                            newPop.Add(baby);

                            ++numSpawnedSoFar;

                            if (numSpawnedSoFar == Population.Size())
                            {
                                numToSpawn = 0;
                            }
                        }
                    }
                }
            }

            while (newPop.Count < Population.Size())
            {
                newPop.Add(TournamentSelection(Population.Size() / 5));
            }

            Population.Clear();
            foreach (YT T in newPop)
            {
                Population.Add(T);
            }

            ResetAndKill();
            SortAndRecord();
            SpeciateAndCalculateSpawnLevels();
        }

        /// <inheritdoc/>
        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        /// <inheritdoc/>
        public TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public void Resume(TrainingContinuation state)
        {
        }

        #endregion

        /// <summary>
        /// Add the specified neuron id.
        /// </summary>
        /// <param name="nodeID">The neuron to add.</param>
        /// <param name="vec">The list to add to.</param>
        public void AddNeuronID(long nodeID, IList<long> vec)
        {
            for (int i = 0; i < vec.Count; i++)
            {
                if (vec[i] == nodeID)
                {
                    return;
                }
            }

            vec.Add(nodeID);

            return;
        }

        /// <summary>
        /// Adjust the compatibility threshold.
        /// </summary>
        public void AdjustCompatibilityThreshold()
        {
            // has this been disabled (unlimited species)
            if (paramMaxNumberOfSpecies < 1)
            {
                return;
            }

            double thresholdIncrement = 0.01;

            if (Population.Species.Count > paramMaxNumberOfSpecies)
            {
                paramCompatibilityThreshold += thresholdIncrement;
            }

            else if (Population.Species.Count < 2)
            {
                paramCompatibilityThreshold -= thresholdIncrement;
            }
        }

        /// <summary>
        /// Adjust each species score.
        /// </summary>
        public void AdjustSpeciesScore()
        {
            foreach (ISpecies s in Population.Species)
            {
                // loop over all Ts and adjust scores as needed
                foreach (IT member in s.Members)
                {
                    double score = member.Score;

                    // apply a youth bonus
                    if (s.Age < Population.YoungBonusAgeThreshold)
                    {
                        score = Comparator.ApplyBonus(score,
                                                      Population.YoungScoreBonus);
                    }

                    // apply an old age penalty
                    if (s.Age > Population.OldAgeThreshold)
                    {
                        score = Comparator.ApplyPenalty(score,
                                                        Population.OldAgePenalty);
                    }

                    double adjustedScore = score / s.Members.Count;

                    member.AdjustedScore = adjustedScore;
                }
            }
        }

        /// <summary>
        /// Perform a cross over.  
        /// </summary>
        /// <param name="mom">The mother T.</param>
        /// <param name="dad">The father T.</param>
        /// <returns></returns>
        public new YT Crossover(YT mom, YT dad)
        {
            YParent best;

            // first determine who is more fit, the mother or the father?
            if (mom.Score == dad.Score)
            {
                if (mom.NumGenes == dad.NumGenes)
                {
                    if (ThreadSafeRandom.NextDouble() > 0)
                    {
                        best = YParent.Mom;
                    }
                    else
                    {
                        best = YParent.Dad;
                    }
                }

                else
                {
                    if (mom.NumGenes < dad.NumGenes)
                    {
                        best = YParent.Mom;
                    }
                    else
                    {
                        best = YParent.Dad;
                    }
                }
            }
            else
            {
                if (Comparator.IsBetterThan(mom.Score, dad.Score))
                {
                    best = YParent.Mom;
                }

                else
                {
                    best = YParent.Dad;
                }
            }

            var babyNeurons = new Q();
            var babyGenes = new Q();

            var vecNeurons = new List<long>();

            int curMom = 0;
            int curDad = 0;

            YLinkGene momGene;
            YLinkGene dadGene;

            YLinkGene selectedGene = null;

            while ((curMom < mom.NumGenes) || (curDad < dad.NumGenes))
            {
                if (curMom < mom.NumGenes)
                {
                    momGene = (YLinkGene)mom.Links.Get(curMom);
                }
                else
                {
                    momGene = null;
                }

                if (curDad < dad.NumGenes)
                {
                    dadGene = (YLinkGene)dad.Links.Get(curDad);
                }
                else
                {
                    dadGene = null;
                }

                if ((momGene == null) && (dadGene != null))
                {
                    if (best == YParent.Dad)
                    {
                        selectedGene = dadGene;
                    }
                    curDad++;
                }
                else if ((dadGene == null) && (momGene != null))
                {
                    if (best == YParent.Mom)
                    {
                        selectedGene = momGene;
                    }
                    curMom++;
                }
                else if (momGene.InnovationId < dadGene.InnovationId)
                {
                    if (best == YParent.Mom)
                    {
                        selectedGene = momGene;
                    }
                    curMom++;
                }
                else if (dadGene.InnovationId < momGene.InnovationId)
                {
                    if (best == YParent.Dad)
                    {
                        selectedGene = dadGene;
                    }
                    curDad++;
                }
                else if (dadGene.InnovationId == momGene.InnovationId)
                {
                    if (ThreadSafeRandom.NextDouble() < 0.5f)
                    {
                        selectedGene = momGene;
                    }

                    else
                    {
                        selectedGene = dadGene;
                    }
                    curMom++;
                    curDad++;
                }

                if (babyGenes.Size() == 0)
                {
                    babyGenes.Add(selectedGene);
                }

                else
                {
                    if (((YLinkGene)babyGenes.Get(babyGenes.Size() - 1))
                            .InnovationId != selectedGene.InnovationId)
                    {
                        babyGenes.Add(selectedGene);
                    }
                }

                // Check if we already have the nodes referred to in SelectedGene.
                // If not, they need to be added.
                AddNeuronID(selectedGene.FromNeuronID, vecNeurons);
                AddNeuronID(selectedGene.ToNeuronID, vecNeurons);
            } // end while

            // now create the required nodes. First sort them into order
            vecNeurons.Sort();

            for (int i = 0; i < vecNeurons.Count; i++)
            {
                babyNeurons.Add(Innovations.CreateNeuronFromID(
                    vecNeurons[i]));
            }

            // finally, create the T
            var babyT = new YT(Population
                                                .AssignTID(), babyNeurons, babyGenes, mom.InputCount,
                                            mom.OutputCount);
            babyT.GA = this;
            babyT.Population = Population;

            return babyT;
        }

        /// <summary>
        /// Init the training.
        /// </summary>
        private void Init()
        {
            if (CalculateScore.ShouldMinimize)
            {
                bestEverScore = Double.MaxValue;
            }
            else
            {
                bestEverScore = Double.MinValue;
            }

            // check the population
            foreach (IT obj in Population.Ts)
            {
                if (!(obj is YT))
                {
                    throw new TrainingError(
                        "Population can only contain objects of YT.");
                }

                var Y = (YT)obj;

                if ((Y.InputCount != inputCount)
                    || (Y.OutputCount != outputCount))
                {
                    throw new TrainingError(
                        "All YT's must have the same input and output sizes as the base network.");
                }
                Y.GA = this;
            }

            Population.Claim(this);

            ResetAndKill();
            SortAndRecord();
            SpeciateAndCalculateSpawnLevels();
        }

        /// <summary>
        /// Reset counts and kill Ts with worse scores.
        /// </summary>
        public void ResetAndKill()
        {
            totalFitAdjustment = 0;
            averageFitAdjustment = 0;

            var speciesArray = new ISpecies[Population.Species.Count];

            for (int i = 0; i < Population.Species.Count; i++)
            {
                speciesArray[i] = Population.Species[i];
            }

            foreach (Object element in speciesArray)
            {
                var s = (ISpecies)element;
                s.Purge();

                if ((s.GensNoImprovement > paramNumGensAllowedNoImprovement)
                    && Comparator.IsBetterThan(bestEverScore,
                                               s.BestScore))
                {
                    Population.Species.Remove(s);
                }
            }
        }

        /// <summary>
        /// Sort the Ts.
        /// </summary>
        public void SortAndRecord()
        {
            foreach (IT g in Population.Ts)
            {
                g.Decode();
                PerformCalculateScore(g);
            }

            Population.Sort();

            IT T = Population.Best;
            double currentBest = T.Score;

            if (Comparator.IsBetterThan(currentBest, bestEverScore))
            {
                bestEverScore = currentBest;
                bestEverNetwork = ((YNetwork)T.Organism);
            }

            bestEverScore = Comparator.BestScore(Error,
                                                 bestEverScore);
        }

        /// <summary>
        /// Determine the species.
        /// </summary>
        public void SpeciateAndCalculateSpawnLevels()
        {
            // calculate compatibility between Ts and species
            AdjustCompatibilityThreshold();

            // assign Ts to species (if any exist)
            foreach (IT g in Population.Ts)
            {
                var T = (YT)g;
                bool added = false;

                foreach (ISpecies s in Population.Species)
                {
                    double compatibility = T.GetCompatibilityScore((YT)s.Leader);

                    if (compatibility <= paramCompatibilityThreshold)
                    {
                        AddSpeciesMember(s, T);
                        T.SpeciesID = s.SpeciesID;
                        added = true;
                        break;
                    }
                }

                // if this T did not fall into any existing species, create a
                // new species
                if (!added)
                {
                    Population.Species.Add(
                        new BasicSpecies(Population, T,
                                         Population.AssignSpeciesID()));
                }
            }

            AdjustSpeciesScore();

            foreach (IT g in Population.Ts)
            {
                var T = (YT)g;
                totalFitAdjustment += T.AdjustedScore;
            }

            averageFitAdjustment = totalFitAdjustment
                                   / Population.Size();

            foreach (IT g in Population.Ts)
            {
                var T = (YT)g;
                double toSpawn = T.AdjustedScore
                                 / averageFitAdjustment;
                T.AmountToSpawn = toSpawn;
            }

            foreach (ISpecies species in Population.Species)
            {
                species.CalculateSpawnAmount();
            }
        }

        /// <summary>
        /// Select a gene using a tournament.
        /// </summary>
        /// <param name="numComparisons">The number of compares to do.</param>
        /// <returns>The chosen T.</returns>
        public YT TournamentSelection(int numComparisons)
        {
            double bestScoreSoFar = 0;

            int chosenOne = 0;

            for (int i = 0; i < numComparisons; ++i)
            {
                var thisTry = (int)RangeRandomizer.Randomize(0, Population.Size() - 1);

                if (Population.Get(thisTry).Score > bestScoreSoFar)
                {
                    chosenOne = thisTry;

                    bestScoreSoFar = Population.Get(thisTry).Score;
                }
            }

            return (YT)Population.Get(chosenOne);
        }
    }

    public class PSOFactory
    {
        /// <summary>
        /// Create a PSO trainer.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            ParamsHolder holder = new ParamsHolder(args);

            int particles = holder.GetInt(
                    MLTrainFactory.PropertyParticles, false, 20);

            ICalculateScore score = new TrainingSetScore(training);
            IRandomizer randomizer =(IRandomizer)( new NguyenWidrowRandomizer());

            IMLTrain train = new NeuralPSO((BasicNetwork)method, randomizer, score, particles);

            return train;
        }
    }

    public class QuickPropFactory
    {
        /// <summary>
        /// Create a quick propagation trainer.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                               IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 2.0);

            return new QuickPropagation((BasicNetwork)method, training, learningRate);
        }
    }

    public class RBFSVDFactory
    {
        /// <summary>
        /// Create a RBF-SVD trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String args)
        {
            if (!(method is RBFNetwork))
            {
                throw new SyntError(
                    "RBF-SVD training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new SVDTraining((RBFNetwork)method, training);
        }
    }

    public class RPROPFactory
    {
        /// <summary>
        /// Create a RPROP trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is IContainsFlat))
            {
                throw new SyntError(
                    "RPROP training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            double initialUpdate = holder.GetDouble(
                MLTrainFactory.PropertyInitialUpdate, false,
                RPROPConst.DefaultInitialUpdate);
            double maxStep = holder.GetDouble(
                MLTrainFactory.PropertyMaxStep, false,
                RPROPConst.DefaultMaxStep);

            return new ResilientPropagation((IContainsFlat)method, training,
                                            initialUpdate, maxStep);
        }
    }

    public class SVMSearchFactory
    {
        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGamma1 = "GAMMA1";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyC1 = "C1";

        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGamma2 = "GAMMA2";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyC2 = "C2";

        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGammaStep = "GAMMASTEP";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyCStep = "CSTEP";

        /// <summary>
        /// Create a SVM trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is SupportVectorMachine))
            {
                throw new SyntError(
                    "SVM Train training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            new ParamsHolder(args);

            var holder = new ParamsHolder(args);
            double gammaStart = holder.GetDouble(
                PropertyGamma1, false,
                SVMSearchTrain.DefaultGammaBegin);
            double cStart = holder.GetDouble(PropertyC1,
                                             false, SVMSearchTrain.DefaultConstBegin);
            double gammaStop = holder.GetDouble(
                PropertyGamma2, false,
                SVMSearchTrain.DefaultGammaEnd);
            double cStop = holder.GetDouble(PropertyC2,
                                            false, SVMSearchTrain.DefaultConstEnd);
            double gammaStep = holder.GetDouble(
                PropertyGammaStep, false,
                SVMSearchTrain.DefaultGammaStep);
            double cStep = holder.GetDouble(PropertyCStep,
                                            false, SVMSearchTrain.DefaultConstStep);

            var result = new SVMSearchTrain((SupportVectorMachine)method, training)
            {
                GammaBegin = gammaStart,
                GammaEnd = gammaStop,
                GammaStep = gammaStep,
                ConstBegin = cStart,
                ConstEnd = cStop,
                ConstStep = cStep
            };

            return result;
        }
    }

    public class SVMTrain : BasicTraining
    {
        /// <summary>
        /// The default starting number for C.
        /// </summary>
        ///
        public const double DefaultConstBegin = -5;

        /// <summary>
        /// The default ending number for C.
        /// </summary>
        ///
        public const double DefaultConstEnd = 15;

        /// <summary>
        /// The default step for C.
        /// </summary>
        ///
        public const double DefaultConstStep = 2;

        /// <summary>
        /// The default gamma begin.
        /// </summary>
        ///
        public const double DefaultGammaBegin = -10;

        /// <summary>
        /// The default gamma end.
        /// </summary>
        ///
        public const double DefaultGammaEnd = 10;

        /// <summary>
        /// The default gamma step.
        /// </summary>
        ///
        public const double DefaultGammaStep = 1;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly SupportVectorMachine _network;

        /// <summary>
        /// The problem to train for.
        /// </summary>
        ///
        private readonly svm_problem _problem;

        /// <summary>
        /// The const c value.
        /// </summary>
        ///
        private double _c;

        /// <summary>
        /// The number of folds.
        /// </summary>
        ///
        private int _fold;

        /// <summary>
        /// The gamma value.
        /// </summary>
        ///
        private double _gamma;

        /// <summary>
        /// Is the training done.
        /// </summary>
        ///
        private bool _trainingDone;

        /// <summary>
        /// Construct a trainer for an SVM network.
        /// </summary>
        ///
        /// <param name="method">The network to train.</param>
        /// <param name="dataSet">The training data for this network.</param>
        public SVMTrain(SupportVectorMachine method, IMLDataSet dataSet) : base(TrainingImplementationType.OnePass)
        {
            _fold = 0;
            _network = method;
            Training = dataSet;
            _trainingDone = false;

            _problem = SyntesisSVMProblem.Syntesis(dataSet, 0);
            _gamma = 1.0d / _network.InputCount;
            _c = 1.0d;
        }

        /// <inheritdoc/>
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Set the constant C.
        /// </summary>
        public double C
        {
            get { return _c; }
            set
            {
                if (value <= 0 || value < SyntFramework.DefaultDoubleEqual)
                {
                    throw new SyntError("SVM training cannot use a c value less than zero.");
                }

                _c = value;
            }
        }


        /// <summary>
        /// Set the number of folds.
        /// </summary>
        public int Fold
        {
            get { return _fold; }
            set { _fold = value; }
        }


        /// <summary>
        /// Set the gamma.
        /// </summary>
        public double Gamma
        {
            get { return _gamma; }
            set
            {
                if (value <= 0 || value < SyntFramework.DefaultDoubleEqual)
                {
                    throw new SyntError("SVM training cannot use a gamma value less than zero.");
                }
                _gamma = value;
            }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }


        /// <value>The problem being trained.</value>
        public svm_problem Problem
        {
            get { return _problem; }
        }


        /// <value>True if the training is done.</value>
        public override bool TrainingDone
        {
            get { return _trainingDone; }
        }

        /// <summary>
        /// Evaluate the error for the specified model.
        /// </summary>
        ///
        /// <param name="param">The params for the SVN.</param>
        /// <param name="prob">The problem to evaluate.</param>
        /// <param name="target">The output values from the SVN.</param>
        /// <returns>The calculated error.</returns>
        private static double Evaluate(svm_parameter param, svm_problem prob,
                                double[] target)
        {
            int totalCorrect = 0;

            var error = new ErrorCalculation();

            if ((param.svm_type == svm_parameter.EPSILON_SVR)
                || (param.svm_type == svm_parameter.NU_SVR))
            {
                for (int i = 0; i < prob.l; i++)
                {
                    double ideal = prob.y[i];
                    double actual = target[i];
                    error.UpdateError(actual, ideal);
                }
                return error.Calculate();
            }
            for (int i = 0; i < prob.l; i++)
            {
                if (target[i] == prob.y[i])
                {
                    ++totalCorrect;
                }
            }

            return Format.HundredPercent * totalCorrect / prob.l;
        }


        /// <summary>
        /// Perform either a train or a cross validation.  If the folds property is 
        /// greater than 1 then cross validation will be done.  Cross validation does 
        /// not produce a usable model, but it does set the error. 
        /// If you are cross validating try C and Gamma values until you have a good 
        /// error rate.  Then use those values to train, producing the final model.
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            _network.Params.C = _c;
            _network.Params.gamma = _gamma;

            SyntLogging.Log(SyntLogging.LevelInfo, "Training with parameters C = " + _c + ", gamma = " + _gamma);

            if (_fold > 1)
            {
                // cross validate
                var target = new double[_problem.l];

                svm.svm_cross_validation(_problem, _network.Params,
                                         _fold, target);
                _network.Model = null;

                Error = Evaluate(_network.Params, _problem, target);
            }
            else
            {
                // train
                _network.Model = svm.svm_train(_problem,
                                              _network.Params);

                Error = _network.CalculateError(Training);
            }

            _trainingDone = true;
        }

        /// <inheritdoc/>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }
    }

    public class SVMSearchTrain : BasicTraining
    {
        /// <summary>
        /// The default starting number for C.
        /// </summary>
        ///
        public const double DefaultConstBegin = 1;

        /// <summary>
        /// The default ending number for C.
        /// </summary>
        ///
        public const double DefaultConstEnd = 15;

        /// <summary>
        /// The default step for C.
        /// </summary>
        ///
        public const double DefaultConstStep = 2;

        /// <summary>
        /// The default gamma begin.
        /// </summary>
        ///
        public const double DefaultGammaBegin = 1;

        /// <summary>
        /// The default gamma end.
        /// </summary>
        ///
        public const double DefaultGammaEnd = 10;

        /// <summary>
        /// The default gamma step.
        /// </summary>
        ///
        public const double DefaultGammaStep = 1;

        /// <summary>
        /// The internal training object, used for the search.
        /// </summary>
        ///
        private readonly SVMTrain _internalTrain;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly SupportVectorMachine _network;

        /// <summary>
        /// The best values found for C.
        /// </summary>
        ///
        private double _bestConst;

        /// <summary>
        /// The best error.
        /// </summary>
        ///
        private double _bestError;

        /// <summary>
        /// The best values found for gamma.
        /// </summary>
        ///
        private double _bestGamma;

        /// <summary>
        /// The beginning value for C.
        /// </summary>
        ///
        private double _constBegin;

        /// <summary>
        /// The ending value for C.
        /// </summary>
        ///
        private double _constEnd;

        /// <summary>
        /// The step value for C.
        /// </summary>
        ///
        private double _constStep;

        /// <summary>
        /// The current C.
        /// </summary>
        ///
        private double _currentConst;

        /// <summary>
        /// The current gamma.
        /// </summary>
        ///
        private double _currentGamma;

        /// <summary>
        /// The number of folds.
        /// </summary>
        ///
        private int _fold;

        /// <summary>
        /// The beginning value for gamma.
        /// </summary>
        ///
        private double _gammaBegin;

        /// <summary>
        /// The ending value for gamma.
        /// </summary>
        ///
        private double _gammaEnd;

        /// <summary>
        /// The step value for gamma.
        /// </summary>
        ///
        private double _gammaStep;

        /// <summary>
        /// Is the network setup.
        /// </summary>
        ///
        private bool _isSetup;

        /// <summary>
        /// Is the training done.
        /// </summary>
        ///
        private bool _trainingDone;

        /// <summary>
        /// Construct a trainer for an SVM network.
        /// </summary>
        ///
        /// <param name="method">The method to train.</param>
        /// <param name="training">The training data for this network.</param>
        public SVMSearchTrain(SupportVectorMachine method, IMLDataSet training)
            : base(TrainingImplementationType.Iterative)
        {
            _fold = 0;
            _constBegin = DefaultConstBegin;
            _constStep = DefaultConstStep;
            _constEnd = DefaultConstEnd;
            _gammaBegin = DefaultGammaBegin;
            _gammaEnd = DefaultGammaEnd;
            _gammaStep = DefaultGammaStep;
            _network = method;
            Training = training;
            _isSetup = false;
            _trainingDone = false;

            _internalTrain = new SVMTrain(_network, training);
        }

        /// <inheritdoc/>
        public override sealed bool CanContinue
        {
            get { return false; }
        }


        /// <value>the constBegin to set</value>
        public double ConstBegin
        {
            get { return _constBegin; }
            set { _constBegin = value; }
        }


        /// <value>the constEnd to set</value>
        public double ConstEnd
        {
            get { return _constEnd; }
            set { _constEnd = value; }
        }


        /// <value>the constStep to set</value>
        public double ConstStep
        {
            get { return _constStep; }
            set { _constStep = value; }
        }


        /// <value>the fold to set</value>
        public int Fold
        {
            get { return _fold; }
            set { _fold = value; }
        }


        /// <value>the gammaBegin to set</value>
        public double GammaBegin
        {
            get { return _gammaBegin; }
            set { _gammaBegin = value; }
        }


        /// <value>the gammaEnd to set.</value>
        public double GammaEnd
        {
            get { return _gammaEnd; }
            set { _gammaEnd = value; }
        }


        /// <value>the gammaStep to set</value>
        public double GammaStep
        {
            get { return _gammaStep; }
            set { _gammaStep = value; }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }


        /// <value>True if the training is done.</value>
        public override bool TrainingDone
        {
            get { return _trainingDone; }
        }

        /// <inheritdoc/>
        public override sealed void FinishTraining()
        {
            _internalTrain.Gamma = _bestGamma;
            _internalTrain.C = _bestConst;
            _internalTrain.Iteration();
        }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override sealed void Iteration()
        {
            if (!_trainingDone)
            {
                if (!_isSetup)
                {
                    Setup();
                }

                PreIteration();

                _internalTrain.Fold = _fold;

                if (_network.KernelType == KernelType.RadialBasisFunction)
                {
                    _internalTrain.Gamma = _currentGamma;
                    _internalTrain.C = _currentConst;
                    _internalTrain.Iteration();
                    double e = _internalTrain.Error;

                    //System.out.println(this.currentGamma + "," + this.currentConst
                    //		+ "," + e);

                    // new best error?
                    if (!Double.IsNaN(e))
                    {
                        if (e < _bestError)
                        {
                            _bestConst = _currentConst;
                            _bestGamma = _currentGamma;
                            _bestError = e;
                        }
                    }

                    // advance
                    _currentConst += _constStep;
                    if (_currentConst > _constEnd)
                    {
                        _currentConst = _constBegin;
                        _currentGamma += _gammaStep;
                        if (_currentGamma > _gammaEnd)
                        {
                            _trainingDone = true;
                        }
                    }

                    Error = _bestError;
                }
                else
                {
                    _internalTrain.Gamma = _currentGamma;
                    _internalTrain.C = _currentConst;
                    _internalTrain.Iteration();
                }

                PostIteration();
            }
        }

        /// <inheritdoc/>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }

        /// <summary>
        /// Setup to train the SVM.
        /// </summary>
        ///
        private void Setup()
        {
            _currentConst = _constBegin;
            _currentGamma = _gammaBegin;
            _bestError = Double.PositiveInfinity;
            _isSetup = true;

            if (_currentGamma <= 0 || _currentGamma < SyntFramework.DefaultDoubleEqual)
            {
                throw new SyntError("SVM search training cannot use a gamma value less than zero.");
            }

            if (_currentConst <= 0 || _currentConst < SyntFramework.DefaultDoubleEqual)
            {
                throw new SyntError("SVM search training cannot use a const value less than zero.");
            }

            if (_gammaStep < 0)
            {
                throw new SyntError("SVM search gamma step cannot use a const value less than zero.");
            }

            if (_constStep < 0)
            {
                throw new SyntError("SVM search const step cannot use a const value less than zero.");
            }
        }
    }
    
    public class TrainBayesianFactory
    {
        /**
 * Create a K2 trainer.
 * 
 * @param method
 *            The method to use.
 * @param training
 *            The training data to use.
 * @param argsStr
 *            The arguments to use.
 * @return The newly created trainer.
 */
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            ParamsHolder holder = new ParamsHolder(args);

            int maxParents = holder.GetInt(
                    MLTrainFactory.PropertyMaxParents, false, 1);
            String searchStr = holder.GetString("SEARCH", false, "k2");
            String estimatorStr = holder.GetString("ESTIMATOR", false, "simple");
            String initStr = holder.GetString("INIT", false, "naive");

            IBayesSearch search;
            IBayesEstimator estimator;
            BayesianInit init;

            if (string.Compare(searchStr, "k2", true) == 0)
            {
                search = new SearchK2();
            }
            else if (string.Compare(searchStr, "none", true) == 0)
            {
                search = new SearchNone();
            }
            else
            {
                throw new BayesianError("Invalid search type: " + searchStr);
            }

            if (string.Compare(estimatorStr, "simple", true) == 0)
            {
                estimator = new SimpleEstimator();
            }
            else if (string.Compare(estimatorStr, "none", true) == 0)
            {
                estimator = new EstimatorNone();
            }
            else
            {
                throw new BayesianError("Invalid estimator type: " + estimatorStr);
            }

            if (string.Compare(initStr, "simple") == 0)
            {
                init = BayesianInit.InitEmpty;
            }
            else if (string.Compare(initStr, "naive") == 0)
            {
                init = BayesianInit.InitNaiveBayes;
            }
            else if (string.Compare(initStr, "none") == 0)
            {
                init = BayesianInit.InitNoChange;
            }
            else
            {
                throw new BayesianError("Invalid init type: " + initStr);
            }

            return new TrainBayesian((BayesianNetwork)method, training, maxParents, init, search, estimator);
        }
    }

    public class MLActivationFactory
    {
        public const String AF_BIPOLAR = "bipolar";
        public const String AF_COMPETITIVE = "comp";
        public const String AF_GAUSSIAN = "gauss";
        public const String AF_LINEAR = "linear";
        public const String AF_LOG = "log";
        public const String AF_RAMP = "ramp";
        public const String AF_SIGMOID = "sigmoid";
        public const String AF_SIN = "sin";
        public const String AF_SOFTMAX = "softmax";
        public const String AF_STEP = "step";
        public const String AF_TANH = "tanh";

        public IActivationFunction Create(String fn)
        {

            foreach (SyntPluginBase plugin in SyntFramework.Instance.Plugins)
            {
                if (plugin is ISyntPluginService1)
                {
                    IActivationFunction result = ((ISyntPluginService1)plugin).CreateActivationFunction(fn);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
    }

    public class MLMethodFactory
    {
        /// <summary>
        /// String constant for a bayesian neural network.
        /// </summary>
	    public const String TypeBayesian = "bayesian";

        /// <summary>
        /// String constant for feedforward neural networks.
        /// </summary>
        ///
        public const String TypeFeedforward = "feedforward";

        /// <summary>
        /// String constant for RBF neural networks.
        /// </summary>
        ///
        public const String TypeRbfnetwork = "rbfnetwork";

        /// <summary>
        /// String constant for support vector machines.
        /// </summary>
        ///
        public const String TypeSVM = "svm";

        /// <summary>
        /// String constant for SOMs.
        /// </summary>
        ///
        public const String TypeSOM = "som";

        /// <summary>
        /// A probabilistic neural network. Supports both PNN and GRNN.
        /// </summary>
        ///
        public const String TypePNN = "pnn";

        /// <summary>
        /// Create a new machine learning method.
        /// </summary>
        ///
        /// <param name="methodType">The method to create.</param>
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created machine learning method.</returns>
        public IMLMethod Create(String methodType,
                               String architecture, int input, int output)
        {
            foreach (SyntPluginBase plugin in SyntFramework.Instance.Plugins)
            {
                if (plugin is ISyntPluginService1)
                {
                    IMLMethod result = ((ISyntPluginService1)plugin).CreateMethod(
                            methodType, architecture, input, output);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            throw new SyntError("Unknown method type: " + methodType);
        }
    }

    [Serializable]
    public class Q
    {
        /// <summary>
        /// The individual elements of this Q.
        /// </summary>
        ///
        private readonly List<IGene> _genes;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public Q()
        {
            _genes = new List<IGene>();
        }

        /// <summary>
        /// Used the get the entire gene list.
        /// </summary>
        ///
        /// <value>the genes</value>
        public List<IGene> Genes
        {
            get { return _genes; }
        }

        /// <summary>
        /// Add a gene.
        /// </summary>
        ///
        /// <param name="gene">The gene to add.</param>
        public void Add(IGene gene)
        {
            _genes.Add(gene);
        }

        /// <summary>
        /// Get an individual gene.
        /// </summary>
        ///
        /// <param name="i">The index of the gene.</param>
        /// <returns>The gene.</returns>
        public IGene Get(int i)
        {
            return _genes[i];
        }

        /// <summary>
        /// Get the specified gene.
        /// </summary>
        ///
        /// <param name="gene">The specified gene.</param>
        /// <returns>The gene specified.</returns>
        public IGene GetGene(int gene)
        {
            return _genes[gene];
        }


        /// <returns>The number of genes in this Q.</returns>
        public int Size()
        {
            return _genes.Count;
        }
    }

    public class FlatLayer
    {
        /// <summary>
        /// The neuron count.
        /// </summary>
        ///
        private readonly int _count;

        /// <summary>
        /// The bias activation, usually 1 for bias or 0 for no bias.
        /// </summary>
        ///
        private double _biasActivation;

        /// <summary>
        /// The layer that feeds this layer's context.
        /// </summary>
        ///
        private FlatLayer _contextFedBy;

        /// <summary>
        /// Construct a flat layer.
        /// </summary>
        ///
        /// <param name="activation">The activation function.</param>
        /// <param name="count">The neuron count.</param>
        /// <param name="biasActivation">The bias activation.</param>
        public FlatLayer(IActivationFunction activation, int count,
                         double biasActivation)
        {
            Activation = activation;
            _count = count;
            _biasActivation = biasActivation;
            _contextFedBy = null;
        }


        /// <value>the activation to set</value>
        public IActivationFunction Activation { get; set; }


        /// <summary>
        /// Set the bias activation.
        /// </summary>
        public double BiasActivation
        {
            get
            {
                if (HasBias())
                {
                    return _biasActivation;
                }
                return 0;
            }
            set { _biasActivation = value; }
        }


        /// <value>The number of neurons our context is fed by.</value>
        public int ContextCount
        {
            get
            {
                if (_contextFedBy == null)
                {
                    return 0;
                }
                return _contextFedBy.Count;
            }
        }


        /// <summary>
        /// Set the layer that this layer's context is fed by.
        /// </summary>
        public FlatLayer ContextFedBy
        {
            get { return _contextFedBy; }
            set { _contextFedBy = value; }
        }


        /// <value>the count</value>
        public int Count
        {
            get { return _count; }
        }


        /// <value>The total number of neurons on this layer, includes context, bias
        /// and regular.</value>
        public int TotalCount
        {
            get
            {
                if (_contextFedBy == null)
                {
                    return Count + ((HasBias()) ? 1 : 0);
                }
                return Count + ((HasBias()) ? 1 : 0)
                       + _contextFedBy.Count;
            }
        }


        /// <returns>the bias</returns>
        public bool HasBias()
        {
            return Math.Abs(_biasActivation) > SyntFramework.DefaultDoubleEqual;
        }

        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder();
            result.Append("[");
            result.Append(GetType().Name);
            result.Append(": count=");
            result.Append(_count);
            result.Append(",bias=");

            if (HasBias())
            {
                result.Append(_biasActivation);
            }
            else
            {
                result.Append("false");
            }
            if (_contextFedBy != null)
            {
                result.Append(",contextFed=");
                if (_contextFedBy == this)
                {
                    result.Append("itself");
                }
                else
                {
                    result.Append(_contextFedBy);
                }
            }
            result.Append("]");
            return result.ToString();
        }
    }

    public class TComparator : IComparer<IT>
    {
        /// <summary>
        /// The method to calculate the score.
        /// </summary>
        ///
        private readonly ICalculateTScore _calculateScore;

        /// <summary>
        /// Construct the T comparator.
        /// </summary>
        ///
        /// <param name="theCalculateScore">The score calculation object to use.</param>
        public TComparator(ICalculateTScore theCalculateScore)
        {
            _calculateScore = theCalculateScore;
        }

        /// <value>The score calculation object.</value>
        public ICalculateTScore CalculateScore
        {
            get { return _calculateScore; }
        }

        #region IComparer<IT> Members

        /// <summary>
        /// Compare two Ts.
        /// </summary>
        ///
        /// <param name="T1">The first T.</param>
        /// <param name="T2">The second T.</param>
        /// <returns>Zero if equal, or less than or greater than zero to indicate
        /// order.</returns>
        public int Compare(IT T1, IT T2)
        {
            return T1.Score.CompareTo(T2.Score);
        }

        #endregion

        /// <summary>
        /// Apply a bonus, this is a simple percent that is applied in the direction
        /// specified by the "should minimize" property of the score function.
        /// </summary>
        ///
        /// <param name="v">The current value.</param>
        /// <param name="bonus">The bonus.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyBonus(double v, double bonus)
        {
            double amount = v * bonus;
            if (_calculateScore.ShouldMinimize)
            {
                return v - amount;
            }
            return v + amount;
        }

        /// <summary>
        /// Apply a penalty, this is a simple percent that is applied in the
        /// direction specified by the "should minimize" property of the score
        /// function.
        /// </summary>
        ///
        /// <param name="v">The current value.</param>
        /// <param name="bonus">The penalty.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyPenalty(double v, double bonus)
        {
            double amount = v * bonus;
            return _calculateScore.ShouldMinimize ? v - amount : v + amount;
        }

        /// <summary>
        /// Determine the best score from two scores, uses the "should minimize"
        /// property of the score function.
        /// </summary>
        ///
        /// <param name="d1">The first score.</param>
        /// <param name="d2">The second score.</param>
        /// <returns>The best score.</returns>
        public double BestScore(double d1, double d2)
        {
            return _calculateScore.ShouldMinimize ? Math.Min(d1, d2) : Math.Max(d1, d2);
        }


        /// <summary>
        /// Determine if one score is better than the other.
        /// </summary>
        ///
        /// <param name="d1">The first score to compare.</param>
        /// <param name="d2">The second score to compare.</param>
        /// <returns>True if d1 is better than d2.</returns>
        public bool IsBetterThan(double d1, double d2)
        {
            return _calculateScore.ShouldMinimize ? d1 < d2 : d1 > d2;
        }
    }

    public class SCGFactory
    {
        /// <summary>
        /// Create a SCG trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String args)
        {
            if (!(method is BasicNetwork))
            {
                throw new SyntError(
                    "SCG training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new ScaledConjugateGradient((BasicNetwork)method, training);
        }
    }

    public class BasicNeuralData : BasicMLData, INeuralData
    {
        /// <summary>
        /// Construct the object from an array.
        /// </summary>
        /// <param name="d">The array to base on.</param>
        public BasicNeuralData(double[] d) : base(d)
        {
        }

        /// <summary>
        /// Construct an empty array of the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        public BasicNeuralData(int size) : base(size)
        {
        }
    }

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

    public class StopTrainingStrategy : IEndTrainingStrategy
    {
        /// <summary>
        /// The default minimum improvement before training stops.
        /// </summary>
        ///
        public const double DefaultMinImprovement = 0.0000001d;

        /// <summary>
        /// The default number of cycles to tolerate.
        /// </summary>
        ///
        public const int DefaultTolerateCycles = 100;

        /// <summary>
        /// The minimum improvement before training stops.
        /// </summary>
        ///
        private readonly double _minImprovement;

        /// <summary>
        /// The number of cycles to tolerate the minimum improvement.
        /// </summary>
        ///
        private readonly int _toleratedCycles;

        /// <summary>
        /// The number of bad training cycles.
        /// </summary>
        ///
        private int _badCycles;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double _bestError;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double _lastError;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start evaluation.
        /// </summary>
        ///
        private bool _ready;

        /// <summary>
        /// Flag to indicate if training should stop.
        /// </summary>
        ///
        private bool _shouldStop;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private IMLTrain _train;

        /// <summary>
        /// Construct the strategy with default options.
        /// </summary>
        ///
        public StopTrainingStrategy() : this(DefaultMinImprovement, DefaultTolerateCycles)
        {
        }

        /// <summary>
        /// Construct the strategy with the specified parameters.
        /// </summary>
        ///
        /// <param name="minImprovement">The minimum accepted improvement.</param>
        /// <param name="toleratedCycles">The number of cycles to tolerate before stopping.</param>
        public StopTrainingStrategy(double minImprovement,
                                    int toleratedCycles)
        {
            _minImprovement = minImprovement;
            _toleratedCycles = toleratedCycles;
            _badCycles = 0;
            _bestError = Double.MaxValue;
        }

        #region EndTrainingStrategy Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void Init(IMLTrain train)
        {
            _train = train;
            _shouldStop = false;
            _ready = false;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            if (_ready)
            {
                if (Math.Abs(_bestError - _train.Error) < _minImprovement)
                {
                    _badCycles++;
                    if (_badCycles > _toleratedCycles)
                    {
                        _shouldStop = true;
                    }
                }
                else
                {
                    _badCycles = 0;
                }
            }
            else
            {
                _ready = true;
            }

            _lastError = _train.Error;
            _bestError = Math.Min(_lastError, _bestError);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PreIteration()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual bool ShouldStop()
        {
            return _shouldStop;
        }

        #endregion
    }
    [Serializable]
    public class PersistYPopulation : ISyntPersistor
    {
        #region SyntPersistor Members

        /// <summary>
        /// The persistence class string.
        /// </summary>
        public virtual String PersistClassString
        {
            get { return typeof(YPopulation).Name; }
        }


        /// <summary>
        /// Read the object.
        /// </summary>
        /// <param name="mask0">The stream to read the object from.</param>
        /// <returns>The object that was loaded.</returns>
        public virtual Object Read(Stream mask0)
        {
            var result = new YPopulation();
            var innovationList = new YInnovationList { Population = result };
            result.Innovations = innovationList;
            var ins0 = new SyntReadHelper(mask0);
            IDictionary<Int32, ISpecies> speciesMap = new Dictionary<Int32, ISpecies>();
            IDictionary<ISpecies, Int32> leaderMap = new Dictionary<ISpecies, Int32>();
            IDictionary<Int32, IT> TMap = new Dictionary<Int32, IT>();
            SyntFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("Y-POPULATION")
                    && section.SubSectionName.Equals("INNOVATIONS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);
                        var innovation = new YInnovation
                        {
                            InnovationID = Int32.Parse(cols[0]),
                            InnovationType = StringToInnovationType(cols[1]),
                            NeuronType = StringToNeuronType(cols[2]),
                            SplitX = CSVFormat.EgFormat.Parse(cols[3]),
                            SplitY = CSVFormat.EgFormat.Parse(cols[4]),
                            NeuronID = Int32.Parse(cols[5]),
                            FromNeuronID = Int32.Parse(cols[6]),
                            ToNeuronID = Int32.Parse(cols[7])
                        };
                        result.Innovations.Add(innovation);
                    }
                }
                else if (section.SectionName.Equals("Y-POPULATION")
                         && section.SubSectionName.Equals("SPECIES"))
                {
                    foreach (String line in section.Lines)
                    {
                        String[] cols = line.Split(',');
                        var species = new BasicSpecies
                        {
                            SpeciesID = Int32.Parse(cols[0]),
                            Age = Int32.Parse(cols[1]),
                            BestScore = CSVFormat.EgFormat.Parse(cols[2]),
                            GensNoImprovement = Int32.Parse(cols[3]),
                            SpawnsRequired = CSVFormat.EgFormat
                                                  .Parse(cols[4])
                        };

                        species.SpawnsRequired = CSVFormat.EgFormat
                            .Parse(cols[5]);
                        leaderMap[(species)] = (Int32.Parse(cols[6]));
                        result.Species.Add(species);
                        speciesMap[((int)species.SpeciesID)] = (species);
                    }
                }
                else if (section.SectionName.Equals("Y-POPULATION")
                         && section.SubSectionName.Equals("TS"))
                {
                    YT lastT = null;

                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);
                        if (cols[0].Equals("g", StringComparison.InvariantCultureIgnoreCase))
                        {
                            lastT = new YT
                            {
                                NeuronsQ = new Q(),
                                LinksQ = new Q()
                            };
                            lastT.Qs.Add(lastT.NeuronsQ);
                            lastT.Qs.Add(lastT.LinksQ);
                            lastT.TID = Int32.Parse(cols[1]);
                            lastT.SpeciesID = Int32.Parse(cols[2]);
                            lastT.AdjustedScore = CSVFormat.EgFormat
                                .Parse(cols[3]);
                            lastT.AmountToSpawn = CSVFormat.EgFormat
                                .Parse(cols[4]);
                            lastT.NetworkDepth = Int32.Parse(cols[5]);
                            lastT.Score = CSVFormat.EgFormat.Parse(cols[6]);
                            result.Add(lastT);
                            TMap[(int)lastT.TID] = lastT;
                        }
                        else if (cols[0].Equals("n", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var neuronGene = new YNeuronGene
                            {
                                Id = Int32.Parse(cols[1]),
                                NeuronType = StringToNeuronType(cols[2]),
                                Enabled = Int32.Parse(cols[3]) > 0,
                                InnovationId = Int32.Parse(cols[4]),
                                ActivationResponse = CSVFormat.EgFormat
                                                         .Parse(cols[5]),
                                SplitX = CSVFormat.EgFormat.Parse(cols[6]),
                                SplitY = CSVFormat.EgFormat.Parse(cols[7])
                            };
                            lastT.Neurons.Add(neuronGene);
                        }
                        else if (cols[0].Equals("l", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var linkGene = new YLinkGene();
                            linkGene.Id = Int32.Parse(cols[1]);
                            linkGene.Enabled = Int32.Parse(cols[2]) > 0;
                            linkGene.Recurrent = Int32.Parse(cols[3]) > 0;
                            linkGene.FromNeuronID = Int32.Parse(cols[4]);
                            linkGene.ToNeuronID = Int32.Parse(cols[5]);
                            linkGene.Weight = CSVFormat.EgFormat.Parse(cols[6]);
                            linkGene.InnovationId = Int32.Parse(cols[7]);
                            lastT.Links.Add(linkGene);
                        }
                    }
                }
                else if (section.SectionName.Equals("Y-POPULATION")
                         && section.SubSectionName.Equals("CONFIG"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    result.YActivationFunction = SyntFileSection
                        .ParseActivationFunction(paras,
                                                 YPopulation.PropertyYActivation);
                    result.OutputActivationFunction = SyntFileSection
                        .ParseActivationFunction(paras,
                                                 YPopulation.PropertyOutputActivation);
                    result.Snapshot = SyntFileSection.ParseBoolean(paras,
                                                                    PersistConst.Snapshot);
                    result.InputCount = SyntFileSection.ParseInt(paras,
                                                                  PersistConst.InputCount);
                    result.OutputCount = SyntFileSection.ParseInt(paras,
                                                                   PersistConst.OutputCount);
                    result.OldAgePenalty = SyntFileSection.ParseDouble(paras,
                                                                        PopulationConst.PropertyOldAgePenalty);
                    result.OldAgeThreshold = SyntFileSection.ParseInt(paras,
                                                                       PopulationConst.PropertyOldAgeThreshold);
                    result.PopulationSize = SyntFileSection.ParseInt(paras,
                                                                      PopulationConst.PropertyPopulationSize);
                    result.SurvivalRate = SyntFileSection.ParseDouble(paras,
                                                                       PopulationConst.PropertySurvivalRate);
                    result.YoungBonusAgeThreshhold = SyntFileSection.ParseInt(
                        paras, PopulationConst.PropertyYoungAgeThreshold);
                    result.YoungScoreBonus = SyntFileSection.ParseDouble(paras,
                                                                          PopulationConst.PropertyYoungAgeBonus);
                    result.TIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                  PopulationConst.
                                                                                      PropertyNextTID);
                    result.InnovationIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                      PopulationConst.
                                                                                          PropertyNextInnovationID);
                    result.GeneIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                PopulationConst.
                                                                                    PropertyNextGeneID);
                    result.SpeciesIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                   PopulationConst.
                                                                                       PropertyNextSpeciesID);
                }
            }

            // now link everything up


            // first put all the Ts into correct species
            foreach (IT T in result.Ts)
            {
                var YT = (YT)T;
                var speciesId = (int)YT.SpeciesID;
                if (speciesMap.ContainsKey(speciesId))
                {
                    ISpecies s = speciesMap[speciesId];
                    s.Members.Add(YT);
                }

                YT.InputCount = result.InputCount;
                YT.OutputCount = result.OutputCount;
            }


            // set the species leader links
            foreach (ISpecies species in leaderMap.Keys)
            {
                int leaderID = leaderMap[species];
                IT leader = TMap[leaderID];
                species.Leader = leader;
                ((BasicSpecies)species).Population = result;
            }

            return result;
        }

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="os">The stream to write to.</param>
        /// <param name="obj">The object to save.</param>
        public virtual void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var pop = (YPopulation)obj;
            xout.AddSection("Y-POPULATION");
            xout.AddSubSection("CONFIG");
            xout.WriteProperty(PersistConst.Snapshot, pop.Snapshot);
            xout.WriteProperty(YPopulation.PropertyOutputActivation,
                               pop.OutputActivationFunction);
            xout.WriteProperty(YPopulation.PropertyYActivation,
                               pop.YActivationFunction);
            xout.WriteProperty(PersistConst.InputCount, pop.InputCount);
            xout.WriteProperty(PersistConst.OutputCount, pop.OutputCount);
            xout.WriteProperty(PopulationConst.PropertyOldAgePenalty,
                               pop.OldAgePenalty);
            xout.WriteProperty(PopulationConst.PropertyOldAgeThreshold,
                               pop.OldAgeThreshold);
            xout.WriteProperty(PopulationConst.PropertyPopulationSize,
                               pop.PopulationSize);
            xout.WriteProperty(PopulationConst.PropertySurvivalRate,
                               pop.SurvivalRate);
            xout.WriteProperty(PopulationConst.PropertyYoungAgeThreshold,
                               pop.YoungBonusAgeThreshold);
            xout.WriteProperty(PopulationConst.PropertyYoungAgeBonus,
                               pop.YoungScoreBonus);
            xout.WriteProperty(PopulationConst.PropertyNextTID, pop.TIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PropertyNextInnovationID, pop.InnovationIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PropertyNextGeneID, pop.GeneIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PropertyNextSpeciesID, pop.SpeciesIDGenerate.CurrentID);
            xout.AddSubSection("INNOVATIONS");
            if (pop.Innovations != null)
            {
                foreach (IInnovation innovation in pop.Innovations.Innovations)
                {
                    var YInnovation = (YInnovation)innovation;
                    xout.AddColumn(YInnovation.InnovationID);
                    xout.AddColumn(InnovationTypeToString(YInnovation.InnovationType));
                    xout.AddColumn(NeuronTypeToString(YInnovation.NeuronType));
                    xout.AddColumn(YInnovation.SplitX);
                    xout.AddColumn(YInnovation.SplitY);
                    xout.AddColumn(YInnovation.NeuronID);
                    xout.AddColumn(YInnovation.FromNeuronID);
                    xout.AddColumn(YInnovation.ToNeuronID);
                    xout.WriteLine();
                }
            }
            xout.AddSubSection("TS");

            foreach (IT T in pop.Ts)
            {
                var YT = (YT)T;
                xout.AddColumn("g");
                xout.AddColumn(YT.TID);
                xout.AddColumn(YT.SpeciesID);
                xout.AddColumn(YT.AdjustedScore);
                xout.AddColumn(YT.AmountToSpawn);
                xout.AddColumn(YT.NetworkDepth);
                xout.AddColumn(YT.Score);
                xout.WriteLine();


                foreach (IGene neuronGene in YT.Neurons.Genes)
                {
                    var YNeuronGene = (YNeuronGene)neuronGene;
                    xout.AddColumn("n");
                    xout.AddColumn(YNeuronGene.Id);
                    xout.AddColumn(NeuronTypeToString(YNeuronGene.NeuronType));
                    xout.AddColumn(YNeuronGene.Enabled);
                    xout.AddColumn(YNeuronGene.InnovationId);
                    xout.AddColumn(YNeuronGene.ActivationResponse);
                    xout.AddColumn(YNeuronGene.SplitX);
                    xout.AddColumn(YNeuronGene.SplitY);
                    xout.WriteLine();
                }

                foreach (IGene linkGene in YT.Links.Genes)
                {
                    var YLinkGene = (YLinkGene)linkGene;
                    xout.AddColumn("l");
                    xout.AddColumn(YLinkGene.Id);
                    xout.AddColumn(YLinkGene.Enabled);
                    xout.AddColumn(YLinkGene.Recurrent);
                    xout.AddColumn(YLinkGene.FromNeuronID);
                    xout.AddColumn(YLinkGene.ToNeuronID);
                    xout.AddColumn(YLinkGene.Weight);
                    xout.AddColumn(YLinkGene.InnovationId);
                    xout.WriteLine();
                }
            }
            xout.AddSubSection("SPECIES");

            foreach (ISpecies species in pop.Species)
            {
                xout.AddColumn(species.SpeciesID);
                xout.AddColumn(species.Age);
                xout.AddColumn(species.BestScore);
                xout.AddColumn(species.GensNoImprovement);
                xout.AddColumn(species.NumToSpawn);
                xout.AddColumn(species.SpawnsRequired);
                xout.AddColumn(species.Leader.TID);
                xout.WriteLine();
            }
            xout.Flush();
        }

        /// <summary>
        /// The file version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }

        #endregion

        /// <summary>
        /// Convert the neuron type to a string.
        /// </summary>
        /// <param name="t">The neuron type.</param>
        /// <returns>The string.</returns>
        public static String NeuronTypeToString(YNeuronType t)
        {
            switch (t)
            {
                case YNeuronType.Bias:
                    return ("b");
                case YNeuronType.Hidden:
                    return ("h");
                case YNeuronType.Input:
                    return ("i");
                case YNeuronType.None:
                    return ("n");
                case YNeuronType.Output:
                    return ("o");
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert the innovation type to a string.
        /// </summary>
        /// <param name="t">The innovation type.</param>
        /// <returns>The string.</returns>
        public static String InnovationTypeToString(YInnovationType t)
        {
            switch (t)
            {
                case YInnovationType.NewLink:
                    return "l";
                case YInnovationType.NewNeuron:
                    return "n";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert a string to an innovation type.
        /// </summary>
        /// <param name="t">The string to convert.</param>
        /// <returns>The innovation type.</returns>
        public static YInnovationType StringToInnovationType(String t)
        {
            if (t.Equals("l", StringComparison.InvariantCultureIgnoreCase))
            {
                return YInnovationType.NewLink;
            }
            if (t.Equals("n", StringComparison.InvariantCultureIgnoreCase))
            {
                return YInnovationType.NewNeuron;
            }
            return default(YInnovationType) /* was: null */;
        }

        /// <summary>
        /// Convert a string to a neuron type.
        /// </summary>
        /// <param name="t">The string.</param>
        /// <returns>The resulting neuron type.</returns>
        public static YNeuronType StringToNeuronType(String t)
        {
            if (t.Equals("b"))
            {
                return YNeuronType.Bias;
            }
            if (t.Equals("h"))
            {
                return YNeuronType.Hidden;
            }
            if (t.Equals("i"))
            {
                return YNeuronType.Input;
            }
            if (t.Equals("n"))
            {
                return YNeuronType.None;
            }
            if (t.Equals("o"))
            {
                return YNeuronType.Output;
            }
            throw new SyntError("Unknonw neuron type: " + t);
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(YPopulation); }
        }
    }

    public class PersistYNetwork : ISyntPersistor
    {
        #region SyntPersistor Members

        /// <summary>
        /// The file version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }

        /// <summary>
        /// The persist class string.
        /// </summary>
        public virtual String PersistClassString
        {
            get { return "YNetwork"; }
        }

        /// <summary>
        /// Read the object.
        /// </summary>
        /// <param name="mask0">The stream to read from.</param>
        /// <returns>The loaded object.</returns>
        public virtual Object Read(Stream mask0)
        {
            var result = new YNetwork();
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;
            IDictionary<Int32, YNeuron> neuronMap = new Dictionary<Int32, YNeuron>();

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("Y")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    foreach (String key in paras.Keys)
                    {
                        result.Properties.Add(key, paras[key]);
                    }
                }
                if (section.SectionName.Equals("Y")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();

                    result.InputCount = SyntFileSection.ParseInt(p,
                                                                  PersistConst.InputCount);
                    result.OutputCount = SyntFileSection.ParseInt(p,
                                                                   PersistConst.OutputCount);
                    result.ActivationFunction = SyntFileSection
                        .ParseActivationFunction(p,
                                                 PersistConst.ActivationFunction);
                    result.OutputActivationFunction = SyntFileSection
                        .ParseActivationFunction(p,
                                                 YPopulation.PropertyOutputActivation);
                    result.NetworkDepth = SyntFileSection.ParseInt(p,
                                                                    PersistConst.Depth);
                    result.Snapshot = SyntFileSection.ParseBoolean(p,
                                                                    PersistConst.Snapshot);
                }
                else if (section.SectionName.Equals("Y")
                         && section.SubSectionName.Equals("NEURONS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);

                        long neuronID = Int32.Parse(cols[0]);
                        YNeuronType neuronType = PersistYPopulation
                            .StringToNeuronType(cols[1]);
                        double activationResponse = CSVFormat.EgFormat
                            .Parse(cols[2]);
                        double splitY = CSVFormat.EgFormat
                            .Parse(cols[3]);
                        double splitX = CSVFormat.EgFormat
                            .Parse(cols[4]);

                        var YNeuron = new YNeuron(neuronType,
                                                        neuronID, splitY, splitX, activationResponse);
                        result.Neurons.Add(YNeuron);
                        neuronMap[((int)neuronID)] = (YNeuron);
                    }
                }
                else if (section.SectionName.Equals("Y")
                         && section.SubSectionName.Equals("LINKS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);
                        int fromID = Int32.Parse(cols[0]);
                        int toID = Int32.Parse(cols[1]);
                        bool recurrent = Int32.Parse(cols[2]) > 0;
                        double weight = CSVFormat.EgFormat.Parse(cols[3]);
                        YNeuron fromNeuron = (neuronMap[fromID]);
                        YNeuron toNeuron = (neuronMap[toID]);
                        var YLink = new YLink(weight, fromNeuron,
                                                    toNeuron, recurrent);
                        fromNeuron.OutputboundLinks.Add(YLink);
                        toNeuron.InboundLinks.Add(YLink);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="os">The output stream.</param>
        /// <param name="obj">The object to save.</param>
        public virtual void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var Y = (YNetwork)obj;
            xout.AddSection("Y");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(Y.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.InputCount, Y.InputCount);
            xout.WriteProperty(PersistConst.OutputCount, Y.OutputCount);
            xout.WriteProperty(PersistConst.ActivationFunction,
                               Y.ActivationFunction);
            xout.WriteProperty(YPopulation.PropertyOutputActivation,
                               Y.OutputActivationFunction);
            xout.WriteProperty(PersistConst.Depth, Y.NetworkDepth);
            xout.WriteProperty(PersistConst.Snapshot, Y.Snapshot);

            xout.AddSubSection("NEURONS");

            foreach (YNeuron YNeuron in Y.Neurons)
            {
                xout.AddColumn((int)YNeuron.NeuronID);
                xout.AddColumn(PersistYPopulation.NeuronTypeToString(YNeuron.NeuronType));
                xout.AddColumn(YNeuron.ActivationResponse);
                xout.AddColumn(YNeuron.SplitX);
                xout.AddColumn(YNeuron.SplitY);
                xout.WriteLine();
            }

            xout.AddSubSection("LINKS");

            foreach (YNeuron YNeuron in Y.Neurons)
            {
                foreach (YLink link in YNeuron.OutputboundLinks)
                {
                    WriteLink(xout, link);
                }
            }

            xout.Flush();
        }

        #endregion

        /// <summary>
        /// Write a link.
        /// </summary>
        /// <param name="xout">The output file.</param>
        /// <param name="link">The link.</param>
        private static void WriteLink(SyntWriteHelper xout, YLink link)
        {
            xout.AddColumn((int)link.FromNeuron.NeuronID);
            xout.AddColumn((int)link.ToNeuron.NeuronID);
            xout.AddColumn(link.Recurrent);
            xout.AddColumn(link.Weight);
            xout.WriteLine();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(YNetwork); }
        }
    }


    public class PersistCPN : ISyntPersistor
    {
        /// <summary>
        /// The input to instar property.
        /// </summary>
        ///
        internal const String PropertyInputToInstar = "inputToInstar";

        /// <summary>
        /// The instar to input property.
        /// </summary>
        ///
        internal const String PropertyInstarToInput = "instarToInput";

        /// <summary>
        /// The winner count property.
        /// </summary>
        ///
        internal const String PropertyWinnerCount = "winnerCount";

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(CPNNetwork); }
        }

        #region SyntPersistor Members

        /// <inheritdoc/>
        public int FileVersion
        {
            get { return 1; }
        }


        /// <inheritdoc/>
        public String PersistClassString
        {
            get { return "CPN"; }
        }


        /// <inheritdoc/>
        public Object Read(Stream mask0)
        {
            IDictionary<String, String> networkParams = null;
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;
            int inputCount = 0;
            int instarCount = 0;
            int outputCount = 0;
            int winnerCount = 0;
            Matrix m1 = null;
            Matrix m2 = null;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("CPN")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    networkParams = section.ParseParams();
                }
                if (section.SectionName.Equals("CPN")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    inputCount = SyntFileSection.ParseInt(paras,
                                                           PersistConst.InputCount);
                    instarCount = SyntFileSection.ParseInt(paras,
                                                            PersistConst.Instar);
                    outputCount = SyntFileSection.ParseInt(paras,
                                                            PersistConst.OutputCount);
                    winnerCount = SyntFileSection.ParseInt(paras,
                                                            PropertyWinnerCount);
                    m1 = SyntFileSection.ParseMatrix(paras,
                                                      PropertyInputToInstar);
                    m2 = SyntFileSection.ParseMatrix(paras,
                                                      PropertyInstarToInput);
                }
            }

            var result = new CPNNetwork(inputCount, instarCount, outputCount,
                                        winnerCount);
            EngineArray.PutAll(networkParams, result.Properties);
            result.WeightsInputToInstar.Set(m1);
            result.WeightsInstarToOutstar.Set(m2);
            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var cpn = (CPNNetwork)obj;
            xout.AddSection("CPN");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(cpn.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.InputCount, cpn.InputCount);
            xout.WriteProperty(PersistConst.Instar, cpn.InstarCount);
            xout.WriteProperty(PersistConst.OutputCount, cpn.OutputCount);
            xout.WriteProperty(PropertyInputToInstar,
                               cpn.WeightsInputToInstar);
            xout.WriteProperty(PropertyInstarToInput,
                               cpn.WeightsInstarToOutstar);
            xout.WriteProperty(PropertyWinnerCount, cpn.WinnerCount);

            xout.Flush();
        }

        #endregion
    }

    public class PopulationConst
    {
        /// <summary>
        /// Property tag for the next gene id.
        /// </summary>
        ///
        public const String PropertyNextGeneID = "nextGeneID";

        /// <summary>
        /// Property tag for the next T id.
        /// </summary>
        ///
        public const String PropertyNextTID = "nextTID";

        /// <summary>
        /// Property tag for the next innovation id.
        /// </summary>
        ///
        public const String PropertyNextInnovationID = "nextInnovationID";

        /// <summary>
        /// Property tag for the next species id.
        /// </summary>
        ///
        public const String PropertyNextSpeciesID = "nextSpeciesID";

        /// <summary>
        /// Property tag for the old age penalty.
        /// </summary>
        ///
        public const String PropertyOldAgePenalty = "oldAgePenalty";

        /// <summary>
        /// Property tag for the old age threshold.
        /// </summary>
        ///
        public const String PropertyOldAgeThreshold = "oldAgeThreshold";

        /// <summary>
        /// Property tag for the population size.
        /// </summary>
        ///
        public const String PropertyPopulationSize = "populationSize";

        /// <summary>
        /// Property tag for the survival rate.
        /// </summary>
        ///
        public const String PropertySurvivalRate = "survivalRate";

        /// <summary>
        /// Property tag for the young age bonus.
        /// </summary>
        ///
        public const String PropertyYoungAgeBonus = "youngAgeBonus";

        /// <summary>
        /// Property tag for the young age threshold.
        /// </summary>
        ///
        public const String PropertyYoungAgeThreshold = "youngAgeThreshold";

        /// <summary>
        /// Property tag for the Ts collection.
        /// </summary>
        ///
        public const String PropertyTs = "Ts";

        /// <summary>
        /// Property tag for the innovations collection.
        /// </summary>
        ///
        public const String PropertyInnovations = "innovations";

        /// <summary>
        /// Property tag for the species collection.
        /// </summary>
        ///
        public const String PropertySpecies = "species";
    }

    [Serializable]
    public class BasicSpecies : ISpecies
    {
        /// <summary>
        /// The list of Ts.
        /// </summary>
        ///
        private readonly IList<IT> _members;

        /// <summary>
        /// The age of this species.
        /// </summary>
        ///
        private int _age;

        /// <summary>
        /// The best score.
        /// </summary>
        ///
        private double _bestScore;

        /// <summary>
        /// The number of generations with no improvement.
        /// </summary>
        ///
        private int _gensNoImprovement;

        /// <summary>
        /// The leader.
        /// </summary>
        ///
        private IT _leader;

        /// <summary>
        /// The id of the leader.
        /// </summary>
        [NonSerialized]
        private long _leaderID;

        /// <summary>
        /// The owner class.
        /// </summary>
        ///
        private IPopulation _population;

        /// <summary>
        /// The number of spawns required.
        /// </summary>
        ///
        private double _spawnsRequired;

        /// <summary>
        /// The species id.
        /// </summary>
        ///
        private long _speciesID;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public BasicSpecies()
        {
            _members = new List<IT>();
        }

        /// <summary>
        /// Construct a species.
        /// </summary>
        ///
        /// <param name="thePopulation">The population the species belongs to.</param>
        /// <param name="theFirst">The first T in the species.</param>
        /// <param name="theSpeciesID">The species id.</param>
        public BasicSpecies(IPopulation thePopulation, IT theFirst,
                            long theSpeciesID)
        {
            _members = new List<IT>();
            _population = thePopulation;
            _speciesID = theSpeciesID;
            _bestScore = theFirst.Score;
            _gensNoImprovement = 0;
            _age = 0;
            _leader = theFirst;
            _spawnsRequired = 0;
            _members.Add(theFirst);
        }

        /// <value>the population to set</value>
        public IPopulation Population
        {
            get { return _population; }
            set { _population = value; }
        }

        /// <summary>
        /// Set the leader id. This value is not persisted, it is used only for
        /// loading.
        /// </summary>
        ///
        /// <value>the leaderID to set</value>
        public long TempLeaderID
        {
            get { return _leaderID; }
            set { _leaderID = value; }
        }

        #region ISpecies Members

        /// <summary>
        /// Calculate the amount to spawn.
        /// </summary>
        ///
        public void CalculateSpawnAmount()
        {
            _spawnsRequired = 0;

            foreach (IT T in _members)
            {
                _spawnsRequired += T.AmountToSpawn;
            }
        }

        /// <summary>
        /// Choose a parent to mate. Choose from the population, determined by the
        /// survival rate. From this pool, a random parent is chosen.
        /// </summary>
        ///
        /// <returns>The parent.</returns>
        public IT ChooseParent()
        {
            IT baby;

            // If there is a single member, then choose that one.
            if (_members.Count == 1)
            {
                baby = _members[0];
            }
            else
            {
                // If there are many, then choose the population based on survival
                // rate
                // and select a random T.
                int maxIndexSize = (int)(_population.SurvivalRate * _members.Count) + 1;
                var theOne = (int)RangeRandomizer.Randomize(0, maxIndexSize);
                baby = _members[theOne];
            }

            return baby;
        }

        /// <summary>
        /// Set the age of this species.
        /// </summary>
        ///
        /// <value>The age of this species.</value>
        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }


        /// <summary>
        /// Set the best score.
        /// </summary>
        ///
        /// <value>The best score.</value>
        public double BestScore
        {
            get { return _bestScore; }
            set { _bestScore = value; }
        }


        /// <summary>
        /// Set the number of generations with no improvement.
        /// </summary>
        ///
        /// <value>The number of generations.</value>
        public int GensNoImprovement
        {
            get { return _gensNoImprovement; }
            set { _gensNoImprovement = value; }
        }


        /// <summary>
        /// Set the leader.
        /// </summary>
        ///
        /// <value>The new leader.</value>
        public IT Leader
        {
            get { return _leader; }
            set { _leader = value; }
        }


        /// <value>The members of this species.</value>
        public IList<IT> Members
        {
            get { return _members; }
        }


        /// <value>The number to spawn.</value>
        public double NumToSpawn
        {
            get { return _spawnsRequired; }
        }


        /// <summary>
        /// Set the number of spawns required.
        /// </summary>
        public double SpawnsRequired
        {
            get { return _spawnsRequired; }
            set { _spawnsRequired = value; }
        }


        /// <summary>
        /// Purge all members, increase age by one and count the number of
        /// generations with no improvement.
        /// </summary>
        ///
        public void Purge()
        {
            _members.Clear();
            _age++;
            _gensNoImprovement++;
            _spawnsRequired = 0;
        }

        /// <summary>
        /// Set the species id.
        /// </summary>
        public long SpeciesID
        {
            get { return _speciesID; }
            set { _speciesID = value; }
        }

        #endregion
    }

}