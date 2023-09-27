﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public abstract class BasicQuery : IBayesianQuery
    {
        /// <summary>
        /// A mapping of the events to event states.
        /// </summary>
        private readonly IDictionary<BayesianEvent, EventState> _events = new Dictionary<BayesianEvent, EventState>();

        /// <summary>
        /// The evidence events.
        /// </summary>
        private readonly IList<BayesianEvent> _evidenceEvents = new List<BayesianEvent>();

        /// <summary>
        /// The network to be queried.
        /// </summary>
        private readonly BayesianNetwork _network;

        /// <summary>
        /// The outcome events.
        /// </summary>
        private readonly IList<BayesianEvent> _outcomeEvents = new List<BayesianEvent>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected BasicQuery()
        {
            _network = null;
        }

        /// <summary>
        /// Construct a basic query.
        /// </summary>
        /// <param name="theNetwork">The network to use for this query.</param>
        protected BasicQuery(BayesianNetwork theNetwork)
        {
            _network = theNetwork;
            FinalizeStructure();
        }

        /// <summary>
        /// Determines if the evidence events have values that satisfy the
        /// needed case. This is used for sampling.
        /// </summary>
        protected bool IsNeededEvidence
        {
            get
            {
                foreach (BayesianEvent evidenceEvent in _evidenceEvents)
                {
                    EventState state = GetEventState(evidenceEvent);
                    if (!state.IsSatisfied)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// True, if the current state satisifies the desired outcome.
        /// </summary>
        /// <returns></returns>
        protected bool SatisfiesDesiredOutcome
        {
            get
            {
                foreach (BayesianEvent outcomeEvent in _outcomeEvents)
                {
                    EventState state = GetEventState(outcomeEvent);
                    if (!state.IsSatisfied)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        #region IBayesianQuery Members

        /// <summary>
        /// Finalize the query structure.
        /// </summary>
        public void FinalizeStructure()
        {
            _events.Clear();
            foreach (BayesianEvent e in _network.Events)
            {
                _events[e] = new EventState(e);
            }
        }

        /// <inheritdoc/>
        public BayesianNetwork Network
        {
            get { return _network; }
        }

        /// <inheritdoc/>
        public IDictionary<BayesianEvent, EventState> Events
        {
            get { return _events; }
        }


        /// <inheritdoc/>
        public IList<BayesianEvent> EvidenceEvents
        {
            get { return _evidenceEvents; }
        }

        /// <inheritdoc/>
        public IList<BayesianEvent> OutcomeEvents
        {
            get { return _outcomeEvents; }
        }

        /// <summary>
        /// Called to locate the evidence and outcome events.
        /// </summary>
        public void LocateEventTypes()
        {
            _evidenceEvents.Clear();
            _outcomeEvents.Clear();

            foreach (BayesianEvent e in _network.Events)
            {
                switch (GetEventType(e))
                {
                    case EventType.Evidence:
                        _evidenceEvents.Add(e);
                        break;
                    case EventType.Outcome:
                        _outcomeEvents.Add(e);
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            foreach (EventState s in _events.Values)
            {
                s.IsCalculated = false;
            }
        }


        /// <inheritdoc/>
        public void DefineEventType(BayesianEvent theEvent, EventType et)
        {
            GetEventState(theEvent).CurrentEventType = et;
        }

        /// <inheritdoc/>
        public EventState GetEventState(BayesianEvent theEvent)
        {
            if (!_events.ContainsKey(theEvent))
                return null;
            return _events[theEvent];
        }

        /// <inheritdoc/>
        public EventType GetEventType(BayesianEvent theEvent)
        {
            return GetEventState(theEvent).CurrentEventType;
        }

        /// <inheritdoc/>
        public void SetEventValue(BayesianEvent theEvent, bool b)
        {
            SetEventValue(theEvent, b ? 0 : 1);
        }

        /// <inheritdoc/>
        public void SetEventValue(BayesianEvent theEvent, int d)
        {
            if (GetEventType(theEvent) == EventType.Hidden)
            {
                throw new BayesianError("You may only set the value of an evidence or outcome event.");
            }

            GetEventState(theEvent).CompareValue = d;
            GetEventState(theEvent).Value = d;
        }

        /// <inheritdoc/>
        public String Problem
        {
            get
            {
                if (_outcomeEvents.Count == 0)
                    return "";

                var result = new StringBuilder();
                result.Append("P(");
                bool first = true;
                foreach (BayesianEvent e in _outcomeEvents)
                {
                    if (!first)
                    {
                        result.Append(",");
                    }
                    first = false;
                    result.Append(EventState.ToSimpleString(GetEventState(e)));
                }
                result.Append("|");

                first = true;
                foreach (BayesianEvent e in _evidenceEvents)
                {
                    if (!first)
                    {
                        result.Append(",");
                    }
                    first = false;
                    EventState state = GetEventState(e);
                    if (state == null)
                        break;
                    result.Append(EventState.ToSimpleString(state));
                }
                result.Append(")");

                return result.ToString();
            }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>A clone of the object.</returns>
        public virtual IBayesianQuery Clone()
        {
            return null;
        }

        /// <inheritdoc/>
        public abstract double Probability { get; }

        /// <inheritdoc/>
        public abstract void Execute();

        #endregion
    }
}
