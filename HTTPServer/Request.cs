using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        string[] Content;
        string[] lines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //TODO: parse the receivedRequest using the \r\n delimeter   
            string[] BlankSeparators = new string[] { "\r\n\r\n" };
            Content = requestString.Split(BlankSeparators, StringSplitOptions.None);

            string[] stringSeparators = new string[] { "\r\n" };
            lines = Content[0].Split(stringSeparators, StringSplitOptions.None);

            contentLines = Content[1].Split(stringSeparators, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            //TODO: do this

            // Parse Request line
            ParseRequestLine();
            // Validate blank line exists
            ValidateIsURI(relativeURI);

            LoadHeaderLines();
            
            ValidateBlankLine();
            // Load header lines into HeaderLines dictionary
            if (ParseRequestLine() == true && ValidateIsURI(relativeURI) == true && LoadHeaderLines() == true && ValidateBlankLine() == true)
                return true;
            else 
                return false;
        }
        string[] Requests;
        private bool ParseRequestLine()
        {
            string[] lineSeparator = new string[] { " " };
            Requests = lines[0].Split(lineSeparator, StringSplitOptions.None);
            relativeURI = Requests[1];
            if (Requests[0].Equals("GET"))
            {
                method = RequestMethod.GET;
            }
            else if (Requests[0].Equals("HEAD"))
            {
                method = RequestMethod.HEAD;
            }
            else if (Requests[0].Equals("POST"))
            {
                method = RequestMethod.POST;
            }
            else return false;

            if (Requests[2].Equals("HTTP/0.9"))
            {
                httpVersion = HTTPVersion.HTTP09;
            }
            else if (Requests[2].Equals("HTTP/1.0"))
            {
                httpVersion = HTTPVersion.HTTP10;
            }
            else if (Requests[2].Equals("HTTP/1.1"))
            {
                httpVersion = HTTPVersion.HTTP11;
            }
            else { return false; }

            return true;

        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            headerLines = new Dictionary<string, string>();
            int check = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                string[] HeaderSep = new string[] { ": " };
                string[] half = lines[i].Split(HeaderSep, StringSplitOptions.None);

                if (half.Length == 2)
                {
                    headerLines.Add(half[0], half[1]);
                    check++;
                }
            }
            if (check + 1 == lines.Length)
            {
                return true;
            }
            else
                return false;
        }

        private bool ValidateBlankLine()
        {
            if (Content.Length == 2)
                return true;
            else
                return false;
        }

    }
}