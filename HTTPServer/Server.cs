using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        String phypath;
        public Server(int portNumber, string redirectionMatrixPath)
        {
          
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket.Bind(hostEndPoint);
        }

        public void StartServer()
        {

            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(50);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.Write("New client accepted: "); Console.Write(clientSocket.RemoteEndPoint); Console.WriteLine();

                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                //Start the thread
                newthread.Start(clientSocket);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSock = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSock.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
           
           
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] data = new byte[1024];
                    int  receivedLength = clientSock.Receive(data);
                    //   Console.WriteLine(Encoding.Default.GetString(data));
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        Console.Write("Client: "); Console.Write(clientSock.RemoteEndPoint); Console.Write(" ended the connection"); Console.WriteLine();
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Request request = new Request(System.Text.Encoding.Default.GetString(data));

                    // my post request 
            //        string s = "POST /main.html HTTP/1.1\r\n" +
            // "Host: " + "chrom" + "\r\n" +
            // "User-Agent: Chrome/22.0.1229.94\r\n"
            //+ "\r\n" + "dumy data";

            //        request.setRequestString(s);

                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client
                    data = Encoding.ASCII.GetBytes(response.ResponseString);
              //      Console.WriteLine(response.ResponseString);
                    clientSock.Send(data);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSock.Close();
        }

        Response HandleRequest(Request request)
        {
            string content;
            Response response;
            try
            {



                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = File.ReadAllText(Path.Combine(Configuration.RootPath, Configuration.BadRequestDefaultPageName));
                    response = new Response(StatusCode.BadRequest, "text/html", "", content, "");
                    return response;
                }

                //Console.WriteLine("physical path "+physicalpath);
                //TODO: check for redirect
                string redirecturi = GetRedirectionPagePathIFExist((request.relativeURI).Remove(0, 1));
                //fount redirection page
                if (!redirecturi.Equals(""))
                {
                    string redirectpath = Configuration.RootPath + '/' + redirecturi;
                    content = File.ReadAllText(redirectpath);
                    response = new Response(StatusCode.Redirect, "text/html", "", content, redirectpath);
                    return response;
                }

                //TODO: check file exists
                phypath = Configuration.RootPath + request.relativeURI;
                if (!File.Exists(phypath))
                {
                    content = File.ReadAllText(Path.Combine(Configuration.RootPath, Configuration.NotFoundDefaultPageName));
                    response = new Response(StatusCode.NotFound, "text/html", "", content, "");
                //    Logger.LogException(new FileNotFoundException());
                    return response;
                }


                //if (!request.postMethod.Equals(""))
                if (request.method == RequestMethod.POST)
                    writeContent(request.postContent);

                //TODO: read the physical file

                content = File.ReadAllText(phypath);
                response = new Response(StatusCode.OK, "text/html", "", content, "");
                return response;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = File.ReadAllText(Path.Combine(Configuration.RootPath, Configuration.InternalErrorDefaultPageName));
                response = new Response(StatusCode.InternalServerError, "text/html", "", content, "");

                return response;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
        
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }

            return string.Empty;
        }


        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string[] filelines = File.ReadAllLines(filePath);

                Configuration.RedirectionRules = new Dictionary<string, string>();
                foreach (string uri in filelines)
                {
                    if (!uri.Equals(""))
                    {
                        string[] keyValue = uri.Split(',');
                        if (!Configuration.RedirectionRules.ContainsKey(keyValue[0]))
                            Configuration.RedirectionRules.Add(keyValue[0], keyValue[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }


        void writeContent(string content)
        {
            // try
            //{
            //    if (File.Exists("testcontent.txt"))
            //    {
            //        using (StreamWriter sw = File.AppendText("testcontent.txt"))
            //        {
            //            sw.WriteLine(content);

            //            sw.Close();
            //        }
            //    }
            //    else
            //    {
            //        FileStream fs = new FileStream("testcontent.txt", FileMode.CreateNew);
            //        fs.Close();
            //    }
            //}
            //catch (Exception Ex)
            //{
            //    Console.WriteLine(Ex.ToString());
            //}
            String[] dumyLines = { "ddd", "'dd" };
            File.WriteAllLines(phypath, dumyLines);
        }
    }
}
