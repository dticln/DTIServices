using DTIService.Model;
using DTIService.Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace DTIService
{
    partial class InstalledPrograms : ServiceBase
    {
        private Timer baseTimer;
        private Timer searchTimer;
        private string csvPath = @"c:\DTI Services\InstalledReport\report.csv";
        private string[] excludedPrograms = {
            "Security Update for Windows",
            "Update for Windows",
            "Update for Microsoft .NET",
            "Security Update for Microsoft",
            "Hotfix for Windows",
            "Hotfix for Microsoft .NET Framework",
            "Hotfix for Microsoft Visual Studio 2007 Tools",
            "Hotfix",
            "Update for"
        };

        public InstalledPrograms()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.baseTimer = new Timer(new TimerCallback(SayHello), null, 15000, 15 * 60000);
            this.searchTimer = new Timer(new TimerCallback(SearchForPrograms), null, 15000, 60 * 60000);
            LogWriter.Instance.Write("Iniciando serviço em " + Environment.MachineName);
            LogWriter.Instance.Write("Sistema: " + Environment.OSVersion.ToString());
            LogWriter.Instance.Write("Inicio em: " + DateTime.Now);
        }

        protected override void OnStop()
        {
            LogWriter.Instance.Write("Encerrando serviço.");
        }

        private void SayHello(object state)
        {
            LogWriter.Instance.Write(Environment.MachineName + " diz \"estou aqui\".");
        }

        private void SearchForPrograms(object state)
        {
            if (!File.Exists(this.csvPath) ||
                (File.Exists(this.csvPath) && File.GetCreationTime(this.csvPath).Month != DateTime.Now.Month))
            {
                if (File.Exists(this.csvPath))
                {
                    File.Delete(this.csvPath);
                }
                try
                {
                    List<ProgramLog> list = new List<ProgramLog>();
                    GetInstalledAppsAtKey(list, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                    if (Environment.Is64BitOperatingSystem)
                    {
                        GetInstalledAppsAtKey(list, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", true);
                    }
                    LogWriter.Instance.Write("Registrando programas instalados.");
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
                catch (Exception ex)
                {
                    LogWriter.Instance.Write(ex.ToString());
                }
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
                            string displayName = sk.GetValue("DisplayName").ToString();
                            bool doNotWrite = false;
                            if (displayName.Length > 0)
                            {
                                foreach(string exclude in this.excludedPrograms)
                                {
                                    if (displayName.StartsWith(exclude))
                                    {
                                        doNotWrite = true;
                                        break;
                                    }
                                }
                                if (!doNotWrite)
                                {
                                    ProgramLog log = new ProgramLog(
                                        sk.GetValue("DisplayName").ToString(),
                                        sk.GetValue("Publisher").ToString(),
                                        sk.GetValue("DisplayVersion").ToString(),
                                        is64BitsKey
                                    );
                                    list.Add(log);
                                }
                            }
                            
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
        }
    }
}
