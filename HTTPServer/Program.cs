using System;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
            // 2) Start Server
            Server server = new Server(1000, "redirectionRules.txt");
            Console.WriteLine("server started ....");
            server.StartServer();

        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            try
            {
                if (File.Exists("redirectionRules.txt"))
                {
                   
                    using (StreamWriter sw = File.AppendText("redirectionRules.txt"))
                    {
                        sw.WriteLine("aboutus.html,aboutus2.html");
                        sw.Close();
                    }
                }
                else
                {
                    FileStream fs = new FileStream("redirectionRules.txt", FileMode.CreateNew);
                    fs.Close();
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

        }
    }
}
