using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        IPEndPoint endPoint;
        int numofpacet = 1000;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket

            this.LoadRedirectionRules(redirectionMatrixPath);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(endPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            Console.WriteLine("Listening....");
            serverSocket.Listen(numofpacet);

            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Console.WriteLine("connected....");
                Socket clientSocket = serverSocket.Accept();
                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newThread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            Console.WriteLine("Client: " + clientSocket.RemoteEndPoint + " started the connection");
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] data = new byte[1024];
                    int receivedLength = clientSocket.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        Console.WriteLine("Client: " + clientSocket.RemoteEndPoint + " ended the connection");
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    string req = Encoding.ASCII.GetString(data);
                    Request request = new Request(req);
                    // TODO: Call HandleRequest Method that returns the response
                  
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client
                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }
        Response HandleRequest(Request requ)
        {
            string content;
            string code = "";
            Response re;
            try  
            {
                throw new NotImplementedException();

                if (requ.ParseRequest() == false)
                {
                    code = "400";
                    string physical_pathe = Configuration.RootPath + '\\' + "BadRequest.html";
                    content = File.ReadAllText(physical_pathe); 
                    re = new Response(StatusCode.BadRequest, "text.html", content, physical_pathe);
                }
                //TODO: check for bad request  خلصت
                //TODO: map the relativeURI in request to get the physical path of the resource. خلصت 
                else
                {
                    string[] name = requ.requestLines[1].Split('/');
                    string physical_path = Configuration.RootPath + '\\' + name[1];
                    //TODO: check for redirect خلصت 
                    for (int i = 0; i < Configuration.RedirectionRules.Count; i++)
                    {
                        if (Configuration.RedirectionRules.Keys.ElementAt(i).ToString() == name[1])
                        {
                            code = "301";
                            requ.relativeURI = '/' + Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                            name[1] = Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                            physical_path = Configuration.RootPath + '\\' + name[1];
                            content = File.ReadAllText(physical_path);
                            string location = "http://localhost:1000/" + name[1];
                            Response res = new Response(StatusCode.Redirect, "text.html", content, location);
                            return res;

                        }
                    }
                    if (!File.Exists(physical_path))
                    {
                        physical_path = Configuration.RootPath + '\\' + "NotFound.html";
                        code = "404";
                        content = File.ReadAllText(physical_path);
                        re = new Response(StatusCode.NotFound, "text.html", content, physical_path);

                    }
                    //TODO: read the physical file
                    else
                    {
                        content = File.ReadAllText(physical_path);
                        code = "200";
                        re = new Response(StatusCode.OK, "text.html", content, physical_path);

                    }
                }
                
                
                //TODO: check file exists خلصت 
               
                // Create OK response
                return re;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                Console.WriteLine(ex.StackTrace);
                string physical_path = Configuration.RootPath + '\\' + "InternalError.html";
                code = "500";
                content = File.ReadAllText(physical_path);
                re = new Response(StatusCode.InternalServerError, "text.html", content, physical_path);
                return re;
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. 
            }
        }
        /*
        Response HandleRequest(Request request)
        {
            throw new NotImplementedException();
            string content;
            try
            {
                //TODO: check for bad request 

                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "html", content, "");
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string phyPath = Configuration.RootPath + request.relativeURI;

                //TODO: check for redirect
                string redirectionPath = GetRedirectionPagePathIFExist(request.relativeURI);
                if (!String.IsNullOrEmpty(redirectionPath))//redirection
                {
                    //content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    phyPath = Configuration.RootPath + "/" + redirectionPath;
                    content = File.ReadAllText(phyPath);
                    return new Response(StatusCode.Redirect, "html", content, redirectionPath);
                }

                //TODO: check file exists
                bool exist = CheckFileExistence(phyPath);
                if (!exist)
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, "html", content, "");
                }

                //TODO: read the physical file
                content = File.ReadAllText(phyPath);

                // Create OK response
                return new Response(StatusCode.OK, "html", content, "");
            }

            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, "html", content, "");
            }
        }
        */
        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string RedirectionPath;
            if (relativePath[0] == '/')
            {
                relativePath = relativePath.Substring(1);
            }
            bool exist = Configuration.RedirectionRules.TryGetValue(relativePath, out RedirectionPath);
            if (exist)
            {
                return RedirectionPath;
            }
            return string.Empty;
        }
        private bool CheckFileExistence(string PhysicalPath)
        {
            return File.Exists(PhysicalPath);
        }
        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            bool exist = CheckFileExistence(filePath);

            if (!exist)
            {
                Logger.LogException(new Exception(defaultPageName + " Page not Exist"));
                return "";
            }

            // else read file and return its content
            string content = File.ReadAllText(filePath);
            return content;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string[] RulesArr = File.ReadAllLines(filePath);

                Configuration.RedirectionRules = new Dictionary<string, string>();
                // then fill Configuration.RedirectionRules dictionary 
                for (int i = 0; i < RulesArr.Length; i++)
                {
                    string[] rule = RulesArr[i].Split(',');
                    Configuration.RedirectionRules.Add(rule[0], rule[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
            }
        }
    }
}