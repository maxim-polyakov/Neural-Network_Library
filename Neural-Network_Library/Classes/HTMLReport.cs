﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class HTMLReport
    {
        /// <summary>
        /// Text.
        /// </summary>
        private readonly StringBuilder _text;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public HTMLReport()
        {
            _text = new StringBuilder();
        }

        /// <summary>
        /// Begin an HTML tag.
        /// </summary>
        public void BeginHTML()
        {
            _text.Append("<html>");
        }

        /// <summary>
        /// End an HTML tag.
        /// </summary>
        public void EndHTML()
        {
            _text.Append("</html>");
        }

        /// <summary>
        /// Set the title.
        /// </summary>
        /// <param name="str">The title.</param>
        public void Title(String str)
        {
            _text.Append("<head><title>");
            _text.Append(str);
            _text.Append("</title></head>");
        }

        /// <summary>
        /// Begin an HTML para.
        /// </summary>
        public void BeginPara()
        {
            _text.Append("<p>");
        }

        /// <summary>
        /// End an HTML para.
        /// </summary>
        public void EndPara()
        {
            _text.Append("</p>");
        }

        /// <summary>
        /// Display in bold.
        /// </summary>
        /// <param name="str"></param>
        public void Bold(String str)
        {
            _text.Append("<b>");
            _text.Append(Syntesis(str));
            _text.Append("</b>");
        }

        /// <summary>
        /// Display a para.
        /// </summary>
        /// <param name="str">The para to display.</param>
        public void Para(String str)
        {
            _text.Append("<p>");
            _text.Append(Syntesis(str));
            _text.Append("</p>");
        }

        /// <summary>
        /// Clear the report.
        /// </summary>
        public void Clear()
        {
            _text.Length = 0;
        }

        /// <summary>
        /// Convert the report to a string.
        /// </summary>
        /// <returns>The report text.</returns>
        public override String ToString()
        {
            return _text.ToString();
        }

        /// <summary>
        /// Begin the HTML body.
        /// </summary>
        public void BeginBody()
        {
            _text.Append("<body>");
        }

        /// <summary>
        /// End the HTML body.
        /// </summary>
        public void EndBody()
        {
            _text.Append("</body>");
        }

        /// <summary>
        /// Create a H1.
        /// </summary>
        /// <param name="title"></param>
        public void H1(String title)
        {
            _text.Append("<h1>");
            _text.Append(Syntesis(title));
            _text.Append("</h1>");
        }

        /// <summary>
        /// Begin a table.
        /// </summary>
        public void BeginTable()
        {
            _text.Append("<table border=\"1\">");
        }

        /// <summary>
        /// End a table.
        /// </summary>
        public void EndTable()
        {
            _text.Append("</table>");
        }

        /// <summary>
        /// Begin a row of a table.
        /// </summary>
        public void BeginRow()
        {
            _text.Append("<tr>");
        }

        /// <summary>
        /// End a row of a table.
        /// </summary>
        public void EndRow()
        {
            _text.Append("</tr>");
        }

        /// <summary>
        /// Add a header cell.
        /// </summary>
        /// <param name="head">The text to use.</param>
        public void Header(String head)
        {
            _text.Append("<th>");
            _text.Append(Syntesis(head));
            _text.Append("</th>");
        }

        /// <summary>
        /// Add a cell, no column span.
        /// </summary>
        /// <param name="head">The head of that call.</param>
        public void Cell(String head)
        {
            Cell(head, 0);
        }

        /// <summary>
        /// Add a cell to a table.
        /// </summary>
        /// <param name="head">The text for the cell.</param>
        /// <param name="colSpan">The col span.</param>
        public void Cell(String head, int colSpan)
        {
            _text.Append("<td");
            if (colSpan > 0)
            {
                _text.Append(" colspan=\"");
                _text.Append(colSpan);
                _text.Append("\"");
            }
            _text.Append(">");
            _text.Append(Syntesis(head));
            _text.Append("</td>");
        }

        /// <summary>
        /// Add a name-value pair to a table.  This includes a row.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="v">The value.</param>
        public void TablePair(String name, String v)
        {
            BeginRow();
            _text.Append("<td><b>" + Syntesis(name) + "</b></td>");
            Cell(v);
            EndRow();
        }

        /// <summary>
        /// Add a H2.
        /// </summary>
        /// <param name="title">The title.</param>
        public void H2(String title)
        {
            _text.Append("<h2>");
            _text.Append(Syntesis(title));
            _text.Append("</h2>");
        }

        /// <summary>
        /// Add a H3.
        /// </summary>
        /// <param name="title">The title.</param>
        public void H3(String title)
        {
            _text.Append("<h3>");
            _text.Append(Syntesis(title));
            _text.Append("</h3>");
        }

        /// <summary>
        /// Begin a list.
        /// </summary>
        public void BeginList()
        {
            _text.Append("<ul>");
        }

        /// <summary>
        /// Add a list item.
        /// </summary>
        /// <param name="str">The item added.</param>
        public void ListItem(String str)
        {
            _text.Append("<li>");
            _text.Append(Syntesis(str));
        }

        /// <summary>
        /// End a list.
        /// </summary>
        public void EndList()
        {
            _text.Append("</ul>");
        }

        /// <summary>
        /// Begin a new table in a cell.
        /// </summary>
        /// <param name="colSpan">The column span.</param>
        public void BeginTableInCell(int colSpan)
        {
            _text.Append("<td");
            if (colSpan > 0)
            {
                _text.Append(" colspan=\"");
                _text.Append(colSpan);
                _text.Append("\"");
            }
            _text.Append(">");
            _text.Append("<table border=\"1\" width=\"100%\">");
        }

        /// <summary>
        /// End a table in a cell.
        /// </summary>
        public void EndTableInCell()
        {
            _text.Append("</table></td>");
        }

        /// <summary>
        /// Syntesis a string for HTML.
        /// </summary>
        /// <param name="str">The string to Syntesis.</param>
        /// <returns>The Syntesisd string.</returns>
        public static String Syntesis(String str)
        {
            var result = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];

                if (ch == '<')
                {
                    result.Append("&lt;");
                }
                else if (ch == '>')
                {
                    result.Append("&gt;");
                }
                else if (ch == '&')
                {
                    result.Append("&amp;");
                }
                else
                {
                    result.Append(ch);
                }

            }
            return result.ToString();
        }
    }
}
