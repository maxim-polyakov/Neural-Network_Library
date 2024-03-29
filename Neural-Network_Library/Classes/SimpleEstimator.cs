﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
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
}
