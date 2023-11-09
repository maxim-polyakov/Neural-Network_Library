using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
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
}
