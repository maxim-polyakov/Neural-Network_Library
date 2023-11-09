﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ParsedProbability
    {
        /// <summary>
        /// The base events.
        /// </summary>
        private readonly IList<ParsedEvent> baseEvents = new List<ParsedEvent>();

        /// <summary>
        /// The given events.
        /// </summary>
        private readonly IList<ParsedEvent> givenEvents = new List<ParsedEvent>();

        /// <summary>
        /// Add a given event.
        /// </summary>
        /// <param name="theEvent">The event to add.</param>
        public void AddGivenEvent(ParsedEvent theEvent)
        {
            this.givenEvents.Add(theEvent);
        }


        /// <summary>
        /// Add a base event.
        /// </summary>
        /// <param name="theEvent"The base event to add.></param>
        public void AddBaseEvent(ParsedEvent theEvent)
        {
            this.baseEvents.Add(theEvent);
        }

        /// <summary>
        /// Get the arguments to this event.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <returns>The arguments.</returns>
        public int[] GetArgs(BayesianNetwork network)
        {
            int[] result = new int[givenEvents.Count];

            for (int i = 0; i < givenEvents.Count; i++)
            {
                ParsedEvent givenEvent = this.givenEvents[i];
                BayesianEvent actualEvent = network.GetEvent(givenEvent.Label);
                result[i] = givenEvent.ResolveValue(actualEvent);
            }

            return result;
        }

        /// <summary>
        /// The child events.
        /// </summary>
        public ParsedEvent ChildEvent
        {
            get
            {
                if (this.baseEvents.Count > 1)
                {
                    throw new BayesianError("Only one base event may be used to define a probability, i.e. P(a), not P(a,b).");
                }

                if (this.baseEvents.Count == 0)
                {
                    throw new BayesianError("At least one event must be provided, i.e. P() or P(|a,b,c) is not allowed.");
                }

                return this.baseEvents[0];
            }
        }

        /// <summary>
        /// Define the truth table. 
        /// </summary>
        /// <param name="network">The bayesian network.</param>
        /// <param name="result">The resulting probability.</param>
        public void DefineTruthTable(BayesianNetwork network, double result)
        {

            ParsedEvent childParsed = ChildEvent;
            BayesianEvent childEvent = network.RequireEvent(childParsed.Label);

            // define truth table line
            int[] args = GetArgs(network);
            childEvent.Table.AddLine(result, childParsed.ResolveValue(childEvent), args);

        }

        /// <summary>
        /// The base events.
        /// </summary>
        public IList<ParsedEvent> BaseEvents
        {
            get
            {
                return baseEvents;
            }
        }

        /// <summary>
        /// The given events.
        /// </summary>
        public IList<ParsedEvent> GivenEvents
        {
            get
            {
                return givenEvents;
            }
        }

        /// <summary>
        /// Define the relationships.
        /// </summary>
        /// <param name="network">The network.</param>
        public void DefineRelationships(BayesianNetwork network)
        {
            // define event relations, if they are not there already
            ParsedEvent childParsed = ChildEvent;
            BayesianEvent childEvent = network.RequireEvent(childParsed.Label);
            foreach (ParsedEvent e in this.givenEvents)
            {
                BayesianEvent parentEvent = network.RequireEvent(e.Label);
                network.CreateDependency(parentEvent, childEvent);
            }

        }

        /// <inheritdoc/>
        public String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[ParsedProbability:baseEvents=");
            result.Append(this.baseEvents.ToString());
            result.Append(",givenEvents=");
            result.Append(this.givenEvents.ToString());
            result.Append("]");
            return result.ToString();
        }

    }
}
