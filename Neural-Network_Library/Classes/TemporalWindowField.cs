using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class TemporalWindowField
    {
        /// <summary>
        /// The action that is to be taken on this field.
        /// </summary>
        ///
        private TemporalType _action;

        /// <summary>
        /// The name of this field.
        /// </summary>
        ///
        private String _name;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theName">The name of the field to be considered.</param>
        public TemporalWindowField(String theName)
        {
            _name = theName;
        }


        /// <value>the action to set</value>
        public TemporalType Action
        {
            get { return _action; }
            set { _action = value; }
        }


        /// <value>Returns true, if this field is to be used as part of the input
        /// for a prediction.</value>
        public bool Input
        {
            get { return ((_action == TemporalType.Input) || (_action == TemporalType.InputAndPredict)); }
        }


        /// <value>the lastValue to set</value>
        public String LastValue { get; set; }


        /// <value>the name to set</value>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }


        /// <value>Returns true, if this field is part of what is being predicted.</value>
        public bool Predict
        {
            get { return ((_action == TemporalType.Predict) || (_action == TemporalType.InputAndPredict)); }
        }


        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(_name);
            result.Append(", action=");
            result.Append(_action);

            result.Append("]");
            return result.ToString();
        }
    }
}
