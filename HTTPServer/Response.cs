using System;
using System.Collections.Generic;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string lastModefied, string content, string redirectoinPath)
        {

            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add(contentType);
            headerLines.Add(content.Length.ToString());
            headerLines.Add(DateTime.Now.ToString());
            if (!redirectoinPath.Equals(""))
                headerLines.Add(redirectoinPath);

            // TODO: Create the request string
            string status_line = GetStatusLine(code);

            responseString = HTTPVersion.HTTP11 + " " + code.ToString() + " " + status_line + "\r\n" +

                             "Content-Type: " + headerLines[0] + "\r\n" +
                             "Content-Lenght: " + headerLines[1] + "\r\n" +
                               "Date: " + headerLines[2] + "\r\n";

            if (!redirectoinPath.Equals(""))
                responseString += "Location: " + headerLines[3] + "\r\n";

            responseString += "\r\n";//blank line 
            responseString += content;

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine;
            switch (code)
            {
                case StatusCode.OK:
                    statusLine = "OK";
                    break;
                case StatusCode.Redirect:
                    statusLine = "Moved Permenantly";
                    break;
                case StatusCode.NotFound:
                    statusLine = "Not Found";
                    break;
                case StatusCode.InternalServerError:
                    statusLine = "Internal Server Error";
                    break;
                case StatusCode.BadRequest:
                    statusLine = "Bad Request";
                    break;
                default:
                    statusLine = "";
                    break;
            }
            return statusLine;
        }
    }
}
