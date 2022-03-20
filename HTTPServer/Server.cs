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

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it

            ///LoadRedirectionRules: bya5od el gowa el file w y loadha f RedirectionRule
            this.LoadRedirectionRules(redirectionMatrixPath);

            //TODO: initialize this.serverSocket

            /*
             1- create ip address , Any client can connect with server 
             2- initialize object of TCP socket
             3- calling Bind function : da el ip el ay client y2der ykaqlmni 3aleh
             */
            IPEndPoint IEP = new IPEndPoint(IPAddress.Any, portNumber); //anhi port number? 1000
            this. serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(IEP);

        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(100); ///server can listen from 100 client 

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"     
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket Client_Socket = this.serverSocket.Accept();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(Client_Socket);
                //Client_Socket is the parameter of HandleConnection function
                //asta5dmna el threading 3ashan da bymkna nsta5dm akter mn user at the same time.
            }
        }
        ///HandleConnection bta5od el request w btb3at el response
        public void HandleConnection(object obj)
        {
            Console.WriteLine("Connection is accepted");
            // TODO: Create client socket 
            Socket Client_Socket = (Socket)obj;

            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Client_Socket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    /*
                     1- create array of byte
                     2- recieve client message and find its length
                     3- convert this mess to string value
                     */
                    byte[] clientData = new byte[10000];
                    int clientDataLength = Client_Socket.Receive(clientData);
                    string dataSTR = Encoding.ASCII.GetString(clientData);

                    // TODO: break the while loop if receivedLen==0
                    if (clientDataLength == 0) break;

                    // TODO: Create a Request object using received request string
                    Request Client_requestObj = new Request(dataSTR);

                    // TODO: Call HandleRequest Method that returns the response
                    Response Server_responseObj = HandleRequest(Client_requestObj);

                    // TODO: Send Response back to client
                    string str = Server_responseObj.ResponseString;
                    Console.WriteLine("server response: %s", str);
                    byte[] responseByteArr = Encoding.ASCII.GetBytes(str);
                    Client_Socket.Send(responseByteArr);

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }
            // TODO: close client socket
            Client_Socket.Close();
        }

        Response HandleRequest(Request request)
        {
            string content = String.Empty;
            string code = String.Empty;
            string bad_request = "<!DOCTYPE html>< html >< body >< h1 > 400 Bad Request</ h1 >< p > 400 Bad Request</ p ></ body ></ html >";
            try
            {
                //TODO: check for bad request 
                if (request.ParseRequest() == false)
                {
                    code = "400";
                    content = bad_request;
                    return new Response(StatusCode.BadRequest, "text/html", content, "");
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string[] name = request.relativeURI.Split('/');
                string physical_path = Configuration.RootPath + '\\' + name[1];
                //TODO: check for redirect
                for (int i = 0; i < Configuration.RedirectionRules.Count; i++)
                {
                    if (request.relativeURI == '/' + Configuration.RedirectionRules.Keys.ElementAt(i).ToString())
                    {

                        request.relativeURI = '/' + Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                        name[1] = Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                        physical_path = Configuration.RootPath + "\\" + name[1];
                        content = File.ReadAllText(physical_path);

                        return new Response(StatusCode.Redirect, "text/html", content, request.relativeURI);//code-contentType-content-rederiction

                    }
                }//TODO: check file exists
                if (File.Exists(physical_path) == false) //if not exist
                {
                    physical_path = Configuration.RootPath + '\\' + "NotFound.html";
                    code = "404";
                    content = File.ReadAllText(physical_path);
                    return new Response(StatusCode.NotFound, "text/html", content, physical_path);

                }
                //TODO: read the physical file
                else
                {
                    content = File.ReadAllText(physical_path);
                    code = "200";
                    return new Response(StatusCode.OK, "text/html", content, physical_path);
                }


                // Create OK response
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error. 
                Logger.LogException(ex);
                code = "500";
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, "text/html", content, "");
            }

        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            for (int i = 0; i < Configuration.RedirectionRules.Count; i++)
            {
                if (relativePath == '/' + Configuration.RedirectionRules.Keys.ElementAt(i).ToString())
                {
                    string redirectionPath = Configuration.RedirectionRules.Values.ElementAt(i).ToString();
                    string physPath = Configuration.RootPath + "//" + redirectionPath;
                    return physPath;

                }
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string content = "";
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            try
            {
                if (File.Exists(filePath))
                {
                    // else read file and return its content
                    content = File.ReadAllText(filePath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return content;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 

                ///open file stream and read it
                FileStream Fstream = new FileStream(filePath, FileMode.Open); ///handle , file mode
                StreamReader SR = new StreamReader(Fstream);
                Configuration.RedirectionRules = new Dictionary<string, string>();
                // then fill Configuration.RedirectionRules dictionary 

                ///note: peek() >return object without removing it
                ///ReadLine(): a method that reads each line of string or values from a standard input stream.
                while (SR.Peek() != -1)
                {
                    string line = SR.ReadLine(); ///read line by line from fileStream
                    string[] str = line.Split(','); ///splite line by ','
                    if (str[0] != "")
                    {
                        Configuration.RedirectionRules.Add(str[0], str[1]);
                    }
                    else break;
                }
                Fstream.Close();
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Environment.Exit(1);
            }
        }
    }
}