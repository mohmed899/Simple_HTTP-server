using System;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static int log_id = 0;
        // static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 

            try
            {
                if (File.Exists("log.txt"))
                {
                    using (StreamWriter sw = File.AppendText("log.txt"))
                    {
                        sw.WriteLine(log_id + ": " + ex.Message + " " + DateTime.Now.ToString());
                        log_id++;
                        sw.Close();
                    }
                }
                else
                {
                    FileStream fs = new FileStream("log.txt", FileMode.CreateNew);
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
