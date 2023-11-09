using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
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
}
