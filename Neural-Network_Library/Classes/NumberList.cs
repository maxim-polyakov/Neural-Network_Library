﻿using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NumberList
    {
        private NumberList()
        {
        }

        /// <summary>
        /// Get an array of double's from a string of comma separated text.
        /// </summary>
        /// <param name="format">The way to format this list.</param>
        /// <param name="str">The string that contains a list of numbers.</param>
        /// <returns>An array of doubles parsed from the string.</returns>
        public static double[] FromList(CSVFormat format, String str)
        {
            if (str.Trim().Length == 0)
            {
                return new double[0];
            }
            // first count the numbers

            String[] tok = str.Split(format.Separator);
            int count = tok.Length;

            // now allocate an object to hold that many numbers
            var result = new double[count];

            // and finally parse the numbers
            for (int index = 0; index < tok.Length; index++)
            {
                try
                {
                    String num = tok[index];
                    double value = format.Parse(num);
                    result[index] = value;
                }
                catch (Exception e)
                {
                    throw new PersistError(e);
                }
            }

            return result;
        }

        /// <summary>
        /// Get an array of ints's from a string of comma separated text.
        /// </summary>
        /// <param name="format">The way to format this list.</param>
        /// <param name="str">The string that contains a list of numbers.</param>
        /// <returns>An array of ints parsed from the string.</returns>
        public static int[] FromListInt(CSVFormat format, String str)
        {
            if (str.Trim().Length == 0)
            {
                return new int[0];
            }
            // first count the numbers

            String[] tok = str.Split(format.Separator);
            int count = tok.Length;

            // now allocate an object to hold that many numbers
            var result = new int[count];

            // and finally parse the numbers
            for (int index = 0; index < tok.Length; index++)
            {
                try
                {
                    String num = tok[index];
                    int value = int.Parse(num);
                    result[index] = value;
                }
                catch (Exception e)
                {
                    throw new PersistError(e);
                }
            }

            return result;
        }

        /// <summary>
        /// Convert an array of doubles to a comma separated list.
        /// </summary>
        /// <param name="format">The way to format this list.</param>
        /// <param name="result">This string will have the values appended to it.</param>
        /// <param name="data">The array of doubles to use.</param>
        public static void ToList(CSVFormat format, StringBuilder result,
                                  double[] data)
        {
            ToList(format, 20, result, data);
        }

        /// <summary>
        /// Convert an array of doubles to a comma separated list.
        /// </summary>
        /// <param name="format">The way to format this list.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="result">This string will have the values appended to it.</param>
        /// <param name="data">The array of doubles to use.</param>
        public static void ToList(CSVFormat format, int precision, StringBuilder result,
                                  double[] data)
        {
            result.Length = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (i != 0)
                {
                    result.Append(format.Separator);
                }
                result.Append(format.Format(data[i], precision));
            }
        }

        /// <summary>
        /// Convert an array of ints to a comma separated list.
        /// </summary>
        /// <param name="format">The way to format this list.</param>
        /// <param name="result">This string will have the values appended to it.</param>
        /// <param name="data">The array of doubles to use.</param>
        public static void ToListInt(CSVFormat format, StringBuilder result,
                                     int[] data)
        {
            result.Length = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (i != 0)
                {
                    result.Append(format.Separator);
                }
                result.Append(data[i]);
            }
        }
    }
}
