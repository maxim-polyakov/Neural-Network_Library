﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
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
}
