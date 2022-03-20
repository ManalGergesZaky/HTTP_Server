using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();

            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])

            string type = "Content-Type: " + contentType;
            string length = "Content-Length: " + content.Length;
            string date = "Date: " + DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
            string location = "Location: " + redirectoinPath;

            headerLines.Add(type);
            headerLines.Add(length.ToString());
            headerLines.Add(date);

            if(code == StatusCode.Redirect)
            {
                headerLines.Add(location);
            }


            // TODO: Create the request string

            responseString = GetStatusLine(code) + "\r\n";

            for(int i = 0; i < headerLines.Count - 1; i++)
            {
                responseString += headerLines[i] + "\r\n";
            }

            responseString += content + "\r\n\r\n";

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;


            if(code == StatusCode.OK)
            {
                statusLine = "HTTP/1.0 200 OK";
            }
            else if(code == StatusCode.Redirect)
            {
                statusLine = "HTTP/1.0 301 Moved Permanently";
            }
            else if(code == StatusCode.BadRequest)
            {
                statusLine = "HTTP/1.0 400 Bad Request";
            }
            else if (code == StatusCode.NotFound)
            {
                statusLine = "HTTP/1.0 404 Not Found";
            }
            else
            {
                statusLine = "HTTP/1.0 500 Internal Server Error";
            }

            return statusLine;
        }
    }
}
