using System;
using System.ServiceProcess;
using DTIService.Service;
using DTIService.Util;

namespace DTIService
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        static void Main()
        {
            LogWriter.Instance.RenewLogFile();
            LogWriter.Instance.Write("Iniciando DTIServices em " + Environment.MachineName);
            LogWriter.Instance.Write("Sistema: " + Environment.OSVersion.ToString());
            LogWriter.Instance.Write("Inicio em: " + DateTime.Now);

            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[]
            {
                new Invetory()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
