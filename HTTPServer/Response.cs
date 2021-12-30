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
            
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add(contentType);
            headerLines.Add(content.Length.ToString());
            headerLines.Add(DateTime.Now.ToString());
            string status = GetStatusLine(code);
            if (code == StatusCode.Redirect)
            {

                headerLines.Add(redirectoinPath);
                responseString = status + "/r /n" + " Content_Type: " + "/ r / n" + headerLines[0] + " Content_length: " + headerLines[1] + " /r/n " + " Date : " + headerLines[2] + "/r/n" + " Location: " + headerLines[3] + "/r/n" + "/r/n" + content;
                Console.WriteLine("responseString :" + responseString);
            }
            else
            {
                responseString = status + "/r /n" + " Content_Type: " + "/ r / n" + headerLines[0] + " Content_length: " + headerLines[1] + " /r/n " + " Date : " + headerLines[2] + " /r/n " + "/r/n" + content;
            }

            // TODO: Create the request string

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            switch (code)
            {
                case StatusCode.OK:
                    statusLine = "HTTP/1.1" + " " + code + " " + "ok";
                    break;
                case StatusCode.Redirect:
                    statusLine = "HTTP/1.1" + " " + code + " " + "Redirect";
                    break;
                case StatusCode.BadRequest:
                    statusLine = "HTTP/1.1" + " " + code + " " + "BedRequest";
                    break;
                case StatusCode.InternalServerError:
                    statusLine = "HTTP/1.1" + " " + code + " " + "Internal Server Error";
                    break;
                case StatusCode.NotFound:
                    statusLine = "HTTP/1.1" + " " + code + " " + "Not Found";
                    break;

            }

            return statusLine;
        }
    }
}