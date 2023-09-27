using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public abstract class SimulatedAnnealing<TUnitType>
    {
        /// <summary>
        /// The number of cycles that will be used.
        /// </summary>
        ///
        private int _cycles;

        /// <summary>
        /// Should the score be minimized.
        /// </summary>
        ///
        private bool _shouldMinimize;

        /// <summary>
        /// The current temperature.
        /// </summary>
        ///
        private double _temperature;

        /// <summary>
        /// Construct the object.  Default ShouldMinimize to true.
        /// </summary>
        protected SimulatedAnnealing()
        {
            _shouldMinimize = true;
        }

        /// <summary>
        /// Subclasses must provide access to an array that makes up the solution.
        /// </summary>
        public abstract TUnitType[] Array
        {
            get;
        }


        /// <summary>
        /// Get a copy of the array.
        /// </summary>
        public abstract TUnitType[] ArrayCopy
        {
            get;
        }


        /// <value>the cycles to set</value>
        public int Cycles
        {
            get { return _cycles; }
            set { _cycles = value; }
        }


        /// <summary>
        /// Set the score.
        /// </summary>
        public double Score { get; set; }


        /// <value>the startTemperature to set</value>
        public double StartTemperature { get; set; }


        /// <value>the stopTemperature to set</value>
        public double StopTemperature { get; set; }


        /// <value>the temperature to set</value>
        public double Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }


        /// <summary>
        /// Should the score be minimized.
        /// </summary>
        public bool ShouldMinimize
        {
            get { return _shouldMinimize; }
            set { _shouldMinimize = value; }
        }

        /// <summary>
        /// Subclasses should provide a method that evaluates the score for the
        /// current solution. Those solutions with a lower score are better.
        /// </summary>
        ///
        /// <returns>Return the score.</returns>
        public abstract double PerformCalculateScore();


        /// <summary>
        /// Called to perform one cycle of the annealing process.
        /// </summary>
        ///
        public void Iteration()
        {
            Score = PerformCalculateScore();
            TUnitType[] bestArray = ArrayCopy;

            _temperature = StartTemperature;

            for (int i = 0; i < _cycles; i++)
            {
                Randomize();
                double curScore = PerformCalculateScore();

                if (_shouldMinimize)
                {
                    if (curScore < Score)
                    {
                        bestArray = ArrayCopy;
                        Score = curScore;
                    }
                }
                else
                {
                    if (curScore > Score)
                    {
                        bestArray = ArrayCopy;
                        Score = curScore;
                    }
                }

                PutArray(bestArray);
                double ratio = Math.Exp(Math.Log(StopTemperature
                                                 / StartTemperature)
                                        / (Cycles - 1));
                _temperature *= ratio;
            }
        }

        /// <summary>
        /// Store the array.
        /// </summary>
        ///
        /// <param name="array">The array to be stored.</param>
        public abstract void PutArray(TUnitType[] array);

        /// <summary>
        /// Randomize the weight matrix.
        /// </summary>
        ///
        public abstract void Randomize();
    }
}
