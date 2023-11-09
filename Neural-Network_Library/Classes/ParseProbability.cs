using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ParseProbability
    {
        /// <summary>
        /// The network used.
        /// </summary>
        private readonly BayesianNetwork network;

        /// <summary>
        /// Parse the probability for the specified network. 
        /// </summary>
        /// <param name="theNetwork">The network to parse for.</param>
        public ParseProbability(BayesianNetwork theNetwork)
        {
            this.network = theNetwork;
        }

        /// <summary>
        /// Add events, as they are pased.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="results">The events found.</param>
        /// <param name="delim">The delimiter to use.</param>
        private void AddEvents(SimpleParser parser, IList<ParsedEvent> results, String delim)
        {
            bool done = false;
            StringBuilder l = new StringBuilder();

            while (!done && !parser.EOL())
            {
                char ch = parser.Peek();
                if (delim.IndexOf(ch) != -1)
                {
                    if (ch == ')' || ch == '|')
                        done = true;

                    ParsedEvent parsedEvent;

                    // deal with a value specified by + or -
                    if (l.Length > 0 && l[0] == '+')
                    {
                        String l2 = l.ToString().Substring(1);
                        parsedEvent = new ParsedEvent(l2.Trim());
                        parsedEvent.Value = "true";
                    }
                    else if (l.Length > 0 && l[0] == '-')
                    {
                        String l2 = l.ToString().Substring(1);
                        parsedEvent = new ParsedEvent(l2.Trim());
                        parsedEvent.Value = "false";
                    }
                    else
                    {
                        String l2 = l.ToString();
                        parsedEvent = new ParsedEvent(l2.Trim());
                    }

                    // parse choices
                    if (ch == '[')
                    {
                        parser.Advance();
                        int index = 0;
                        while (ch != ']' && !parser.EOL())
                        {

                            String labelName = parser.ReadToChars(":,]");
                            if (parser.Peek() == ':')
                            {
                                parser.Advance();
                                parser.EatWhiteSpace();
                                double min = double.Parse(parser.ReadToWhiteSpace());
                                parser.EatWhiteSpace();
                                if (!parser.LookAhead("to", true))
                                {
                                    throw new BayesianError("Expected \"to\" in probability choice range.");
                                }
                                parser.Advance(2);
                                double max = CSVFormat.EgFormat.Parse(parser.ReadToChars(",]"));
                                parsedEvent.ChoiceList.Add(new ParsedChoice(labelName, min, max));

                            }
                            else
                            {
                                parsedEvent.ChoiceList.Add(new ParsedChoice(labelName, index++));
                            }
                            parser.EatWhiteSpace();
                            ch = parser.Peek();

                            if (ch == ',')
                            {
                                parser.Advance();
                            }
                        }
                    }

                    // deal with a value specified by =
                    if (parser.Peek() == '=')
                    {
                        parser.ReadChar();
                        String value = parser.ReadToChars(delim);
                        //					BayesianEvent evt = this.network.getEvent(parsedEvent.getLabel());
                        parsedEvent.Value = value;
                    }

                    if (ch == ',')
                    {
                        parser.Advance();
                    }

                    if (ch == ']')
                    {
                        parser.Advance();
                    }

                    if (parsedEvent.Label.Length > 0)
                    {
                        results.Add(parsedEvent);
                    }
                    l.Length = 0;
                }
                else
                {
                    parser.Advance();
                    l.Append(ch);
                }
            }

        }

        /// <summary>
        /// Parse the given line.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The parsed probability.</returns>
        public ParsedProbability Parse(String line)
        {

            ParsedProbability result = new ParsedProbability();

            SimpleParser parser = new SimpleParser(line);
            parser.EatWhiteSpace();
            if (!parser.LookAhead("P(", true))
            {
                throw new SyntError("Bayes table lines must start with P(");
            }
            parser.Advance(2);

            // handle base
            AddEvents(parser, result.BaseEvents, "|,)=[]");

            // handle conditions
            if (parser.Peek() == '|')
            {
                parser.Advance();
                AddEvents(parser, result.GivenEvents, ",)=[]");

            }

            if (parser.Peek() != ')')
            {
                throw new BayesianError("Probability not properly terminated.");
            }

            return result;

        }

        /// <summary>
        /// Parse a probability list. 
        /// </summary>
        /// <param name="network">The network to parse for.</param>
        /// <param name="line">The line to parse.</param>
        /// <returns>The parsed list.</returns>
        public static IList<ParsedProbability> ParseProbabilityList(BayesianNetwork network, String line)
        {
            IList<ParsedProbability> result = new List<ParsedProbability>();

            StringBuilder prob = new StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if (ch == ')')
                {
                    prob.Append(ch);
                    ParseProbability parse = new ParseProbability(network);
                    ParsedProbability parsedProbability = parse.Parse(prob.ToString());
                    result.Add(parsedProbability);
                    prob.Length = 0;
                }
                else
                {
                    prob.Append(ch);
                }
            }
            return result;
        }
    }
}
