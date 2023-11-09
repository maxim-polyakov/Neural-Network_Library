using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
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
}
