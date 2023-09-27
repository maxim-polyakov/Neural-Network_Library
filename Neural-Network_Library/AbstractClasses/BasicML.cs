using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public abstract class BasicML : IMLProperties
    {

        /// <summary>
        /// Properties about the neural network. Some NeuralLogic classes require
        /// certain properties to be set.
        /// </summary>
        ///
        private readonly IDictionary<String, String> _properties;

        /// <summary>
        /// Construct the object.
        /// </summary>
        protected BasicML()
        {
            _properties = new Dictionary<String, String>();
        }

        #region MLProperties Members

        /// <value>A map of all properties.</value>
        public IDictionary<String, String> Properties
        {
            get { return _properties; }
        }


        /// <summary>
        /// Get the specified property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <returns>The property as a double.</returns>
        public double GetPropertyDouble(String name)
        {
            return (CSVFormat.EgFormat.Parse((_properties[name])));
        }

        /// <summary>
        /// Get the specified property as a long.
        /// </summary>
        ///
        /// <param name="name">The name of the specified property.</param>
        /// <returns>The value of the specified property.</returns>
        public long GetPropertyLong(String name)
        {
            return (Int64.Parse(_properties[name]));
        }

        /// <summary>
        /// Get the specified property as a string.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public String GetPropertyString(String name)
        {
            if (_properties.ContainsKey(name))
            {
                return (_properties[name]);
            }
            return null;
        }

        /// <summary>
        /// Set a property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="d">The value of the property.</param>
        public void SetProperty(String name, double d)
        {
            _properties[name] = CSVFormat.EgFormat.Format(d, SyntFramework.DefaultPrecision);
            UpdateProperties();
        }

        /// <summary>
        /// Set a property as a long.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="l">The value of the property.</param>
        public void SetProperty(String name, long l)
        {
            _properties[name] = "" + l;
            UpdateProperties();
        }

        /// <summary>
        /// Set a property as a double.
        /// </summary>
        ///
        /// <param name="name">The name of the property.</param>
        /// <param name="v">The value of the property.</param>
        public void SetProperty(String name, String v)
        {
            _properties[name] = v;
            UpdateProperties();
        }

        /// <summary>
        /// Update from the propeties stored in the hash map.  Should be called 
        /// whenever the properties change and might need to be reloaded.
        /// </summary>
        public abstract void UpdateProperties();

        #endregion
    }
}
