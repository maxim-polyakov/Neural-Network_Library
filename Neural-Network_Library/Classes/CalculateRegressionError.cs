using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class CalculateRegressionError
    {
        /// <summary>
        /// Calculate an error for a method that uses regression.
        /// </summary>
        /// <param name="method">The method to evaluate.</param>
        /// <param name="data">The training data to evaluate with.</param>
        /// <returns>The error.</returns>
        public static double CalculateError(IMLRegression method,
                                            IMLDataSet data)
        {
            var errorCalculation = new ErrorCalculation();

            // clear context
            if (method is IMLContext)
            {
                ((IMLContext)method).ClearContext();
            }


            // calculate error
            foreach (IMLDataPair pair in data)
            {
                IMLData actual = method.Compute(pair.Input);
                errorCalculation.UpdateError(actual.Data, pair.Ideal.Data, pair.Significance);
            }
            return errorCalculation.Calculate();
        }
    }
}
