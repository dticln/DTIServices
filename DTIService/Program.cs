using System.ServiceProcess;

namespace DTIService
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new InstalledPrograms()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
