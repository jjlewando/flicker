using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FlickerApplication
{
    class FileLogger
    {
        /// <summary>
        /// Create file on disk
        /// </summary>
        public FileLogger()
        {
            DateTime dataCzas = DateTime.Now;
            string log = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Log_[" + dataCzas.Year.ToString() + "_" + dataCzas.Month.ToString() + "_" + dataCzas.Day.ToString() + "-" + dataCzas.Hour.ToString() + "-" + dataCzas.Minute.ToString() + "].txt";
            logFile = new StreamWriter(log);
            running = true;
        }

        private StreamWriter logFile;
        private bool running = false;

        /// <summary>
        /// Add log entry
        /// </summary>
        /// <param name="entry">Comment to log</param>
        public void AddEntry(string entry)
        {
            if (running)
            {
                DateTime dataCzas = DateTime.Now;
                logFile.Write(dataCzas.ToString() + " > " + entry);
                logFile.Flush();
            }
        }

        /// <summary>
        /// Close log on disk
        /// </summary>
        public void StopLogging()
        {
            if (running)
            {
                DateTime dataCzas = DateTime.Now;
                logFile.Write(dataCzas.ToString() + " > " + "* File Closed *");
                logFile.Close();
            }
        }

    }
}
