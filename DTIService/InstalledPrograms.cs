using DTIService.Model;
using DTIService.Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DTIService
{
    partial class InstalledPrograms : ServiceBase
    {
        private Timer baseTimer;
        private Timer searchTimer;
        private String csvPath;

        public InstalledPrograms()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.baseTimer = new Timer(new TimerCallback(SayHello), null, 15000, 15 * 60000);
            this.searchTimer = new Timer(new TimerCallback(SearchForPrograms), null, 15000, 60000);
            LogWriter.Instance.Write("Iniciando serviço em " + Environment.MachineName);
            LogWriter.Instance.Write("Sistema: " + Environment.OSVersion.ToString());
            LogWriter.Instance.Write("Inicio em: " + DateTime.Now);
            this.csvPath = @"c:\InstalledPrograms-" + Environment.MachineName + ".csv";
        }

        protected override void OnStop()
        {
            LogWriter.Instance.Write(DateTime.Now + ": encerrando serviço.");
        }

        private void SayHello(object state)
        {
            LogWriter.Instance.Write(DateTime.Now + ": " + Environment.MachineName + " diz \"estou aqui\".");
        }

        private void SearchForPrograms(object state)
        {
            if (!File.Exists(this.csvPath) || 
                (File.Exists(this.csvPath) && File.GetCreationTime(this.csvPath).Month != DateTime.Now.Month )) {

                if(File.Exists(this.csvPath))
                {
                    File.Delete(this.csvPath);
                }

                List<ProgramLog> list = new List<ProgramLog>();
                GetInstalledAppsAtKey(list, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                if (Environment.Is64BitOperatingSystem)
                {
                    GetInstalledAppsAtKey(list, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", true);
                }

                LogWriter.Instance.Write(DateTime.Now + ": registrando programas instalados.");
                StreamWriter writer = new StreamWriter(this.csvPath, true);
                foreach (ProgramLog program in list)
                {
                    writer.WriteLine(
                        "\"" + program.DisplayName + "\",\"" +
                        program.Publisher + "\",\"" +
                        program.Version + "\""
                    );
                }
                writer.Flush();
                writer.Close();

                Task.Factory.StartNew(() => 
                    FileSender.Instance.UploadInstalledProgramsAsync(this.csvPath)
                );
            }
        }


        private void GetInstalledAppsAtKey(List<ProgramLog> list, String uninstallKey, bool is64BitsKey = false)
        {
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallKey))
            {
                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        try
                        {
                            ProgramLog log = new ProgramLog(
                                sk.GetValue("DisplayName").ToString(),
                                sk.GetValue("Publisher").ToString(),
                                sk.GetValue("DisplayVersion").ToString(),
                                is64BitsKey
                            );
                            list.Add(log);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
        }
    }
}
