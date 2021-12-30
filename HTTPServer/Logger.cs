using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        
        public static void LogException(Exception ex)
        {
             StreamWriter sr = new StreamWriter("log.txt");
            string time = DateTime.Now.ToString();
            sr.WriteLine("date time is "+time);
            sr.WriteLine("message is "+ ex.Message);

            sr.Close();
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
        }
    }
}
