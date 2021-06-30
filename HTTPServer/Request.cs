using System;
using System.Collections.Generic;

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
        string[] requestLines;
        public RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;
        public string postContent;
        public string postMethod = "";
        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        public void setRequestString(string s)
        {
            this.requestString = s;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {


            //TODO: parse the receivedRequest using the \r\n delimeter
            string[] blankLine = { "\r\n" };
            requestLines = requestString.Split(blankLine, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3)
                return false;
            // Parse Request line
            if (!ParseRequestLine(requestLines[0]))
                return false;
            // Validate blank line exists
            if (!ValidateBlankLine())
                return false;
            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines())
                return false;
            //else
            return true;
        }

        private bool ParseRequestLine(string requestLine)
        {
            // check for requestMethod
            string[] requesLineContent = requestLine.Split(' ');
            if (requesLineContent.Length < 3)
                return false;
            if (requesLineContent[0].Equals("GET"))
                method = RequestMethod.GET;
            else if (requesLineContent[0].Equals("POST"))
                method = RequestMethod.POST;
            //postMethod = "POST";
            else if (requesLineContent[0].Equals("HEAD"))
                method = RequestMethod.HEAD;
            
            else
                return false;

            // check for Uri
            if (!ValidateIsURI(requesLineContent[1]))
                return false;
            relativeURI = requesLineContent[1];

            // check http version
            string[] HttpVersion = requesLineContent[2].Split('/');

            if (HttpVersion[1] == "1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (HttpVersion[1] == "1.1")
                httpVersion = HTTPVersion.HTTP11;
            else
                httpVersion = HTTPVersion.HTTP09;

            // get content
            postContent = requestLines[requestLines.Length - 1];


            return true;


        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            // if there is headers 
            if (requestLines[1].Equals(""))
                return false;
            string[] HeaderSeparators = { ": " };
            headerLines = new Dictionary<string, string>();
            for (int i = 1; i < requestLines.Length - 2; i++)//descard request line and blank line (assuming GET  *NO CONTENT*) 
            {
                string[] keyvalue = requestLines[i].Split(HeaderSeparators, StringSplitOptions.None);
                headerLines.Add(keyvalue[0], keyvalue[1]);
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            if (requestLines[requestLines.Length - 2].Equals(""))
                return true;
            return false;
        }
    }

}
