using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            string file_path = @"C:\inetpub\wwwroot\fcis1\redirectionRules.txt";
            // 1) Make server object on port 1000
            Server http_server = new Server(1000, file_path);
            // 2) Start Server
            http_server.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            FileStream file_stream = new FileStream(@"C:\inetpub\wwwroot\fcis1\redirectionRules.txt", FileMode.OpenOrCreate);
            StreamWriter file_writer = new StreamWriter(file_stream);
            file_writer.WriteLine(@"aboutus.html,aboutus2.html");
            file_writer.Close();
        }

    }
}