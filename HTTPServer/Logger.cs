using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            //we log exception details in log.txt

            //path of log.txt file 
            String path = @"C:\Users\Lola\Downloads\Template[2021-2022]\Template[2021-2022]\HTTPServer\bin\Debug\log.txt";

            //open file 
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);


            //write data in file 
            StreamWriter streamWriter = new StreamWriter(fileStream);

            String date = DateTime.Now.ToString();
            streamWriter.WriteLine("Date Time:" + date);
            streamWriter.WriteLine("Message:" + ex.Message);

            //close connection of file
            fileStream.Close();

        }
    }
}
