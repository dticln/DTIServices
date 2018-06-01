using System;
using System.IO;

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
                this.writer = new StreamWriter(@"c:\DTI Services\service.log", true);
                this.writer.WriteLine(DateTime.Now + ": " + log);
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
