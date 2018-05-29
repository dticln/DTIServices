using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTIService
{
    class LogWriter
    {
        private static readonly LogWriter instance = new LogWriter();
        private StreamWriter writer;

        private LogWriter() {}

        public void Write(String log)
        {
            try
            {
                this.writer = new StreamWriter(@"c:\dtiservice.log", true);
                this.writer.WriteLine(log);
                this.writer.Flush();
                this.writer.Close();
            }
            catch
            {
                this.writer = null;
                Console.WriteLine("Falha ao registrar log.");
            }
        }

        public static LogWriter Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
