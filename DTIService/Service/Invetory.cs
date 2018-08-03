using DTIService.Model;
using DTIService.Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DTIService.Service
{
    partial class Invetory : ServiceBase
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

        public Invetory()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {   
            /*
            try
            {
                LogWriter.Instance.Write(KeyDecoder.GetWindowsProductKeyFromRegistry().ToString());
                string query = "select * from SoftwareLicensingService";
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                using (ManagementObjectCollection results = searcher.Get())
                {
                    foreach (ManagementObject result in results)
                    {
                        using (result)
                        {
                            LogWriter.Instance.Write(result.GetPropertyValue("OA3xOriginalProductKey").ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Write(ex.ToString());
            }*/
            this.CreateVersionFile();
            this.baseTimer = new Timer(new TimerCallback(SayHello), null, 15000, 15 * 60000);
            this.searchTimer = new Timer(new TimerCallback(SearchForPrograms), null, 15000, 60 * 60000);
            Task.Factory.StartNew(() =>
                API.Parser.Instance.RegistrationAsync(this.GetComputerInformation())
            );
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
                (File.Exists(this.csvPath) && File.GetLastWriteTime(this.csvPath).Month != DateTime.Now.Month))
            {
                try
                {
                    List<ProgramLog> list = new List<ProgramLog>();
                    GetInstalledAppsAtKey(list, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                    if (Environment.Is64BitOperatingSystem)
                    {
                        GetInstalledAppsAtKey(list, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", true);
                    }
                    LogWriter.Instance.Write("Registrando programas instalados.");
                    StreamWriter writer = new StreamWriter(this.csvPath, false);
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
                        API.Parser.Instance.UploadInstalledProgramsAsync(this.csvPath)
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
                                foreach (string exclude in this.excludedPrograms)
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

        private WinServiceComputer GetComputerInformation()
        {
            WinServiceComputer computer = new WinServiceComputer();
            computer.WindowsVersion = this.GetOSFriendlyName();
            Regex ufrgsNetwork = new Regex(@"143.54.*");
            foreach(NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach(var property in nic.GetIPProperties().UnicastAddresses)
                {
                    if (property.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                        ufrgsNetwork.Match(property.Address.ToString()).Success)
                    {
                        computer.Ipv4 = property.Address.ToString();
                        computer.Mac = nic.GetPhysicalAddress().ToString();
                        break;
                    }
                }
            }
            return computer;
        }

        private void CreateVersionFile()
        {
            File.WriteAllText(@"C:\DTI Services\version.txt", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }


        private string GetOSFriendlyName()
        {
            string ProductName = HKLMGetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            string CSDVersion = HKLMGetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
            if (ProductName != "")
            {
                return (ProductName.StartsWith("Microsoft") ? "" : "Microsoft ") + ProductName +
                            (CSDVersion != "" ? " " + CSDVersion : "");
            }
            return "";
        }

        private string HKLMGetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
        }
    }
}
