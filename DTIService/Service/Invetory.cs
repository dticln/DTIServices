using DTIService.External;
using DTIService.Model;
using DTIService.Util;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WinProdKeyFind;

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
        private WinServiceComputer computerInformation;

        public Invetory()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.CreateVersionFile();
            this.computerInformation = this.GetComputerInformation();
            Task.Factory.StartNew(() =>
                API.Parser.Instance.RegistrationAsync(computerInformation)
            );
            Task.Factory.StartNew(() =>
                this.SearchForKeysAsync()
            );
            Task.Factory.StartNew(() =>
                this.SearchForAdministrators()
            );
            this.baseTimer = new Timer(new TimerCallback(SayHello), null, 15000, 15 * 60000);
            this.searchTimer = new Timer(new TimerCallback(SearchForPrograms), null, 15000, 60 * 60000);
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
                        API.Parser.Instance.UploadInstalledProgramsAsync(this.computerInformation, this.csvPath)
                    );
                }
                catch (Exception ex)
                {
                    LogWriter.Instance.Write(ex.ToString());
                }
            }
        }

        private async Task SearchForKeysAsync()
        {
            List<WinServiceKey> keys = ProduKeyConnector.Instance.FindProductKeys(this.computerInformation);
            foreach (WinServiceKey key in keys)
            {
                await API.Parser.Instance.SendProdKeyAsync(key);
            }
        }
        
        private object FindValueInRegistry(string haystack, string needle)
        {
            var baseKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
            using (RegistryKey rk = baseKey.OpenSubKey(haystack))
            {
                if (rk != null)
                {
                    if (rk.GetValue(needle) != null)
                    {
                        return rk.GetValue(needle);
                    } else
                    {
                        foreach (string skName in rk.GetSubKeyNames())
                        {
                            object finded = FindValueInRegistry(haystack + "\\" + skName, needle);
                            if (finded != null)
                            {
                                return finded;
                            }
                        }
                    }
                }
                return null;
            }
        }

        private void GetInstalledAppsAtKey(List<ProgramLog> list, string uninstallKey, bool is64BitsKey = false)
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

        private async Task SearchForAdministrators()
        {
            List<WinServiceAdmin> admins = new List<WinServiceAdmin>();
            using (DirectoryEntry directory = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer"))
            {
                DirectoryEntry group = Helper.GetGroupFromDirectory(directory, "Administrators");
                if (group == null)
                {
                    group = Helper.GetGroupFromDirectory(directory, "Administradores");
                }
                if (group != null)
                {
                    var entries = group.Invoke("Members", null);
                    foreach (var entry in (IEnumerable)entries)
                    {
                        DirectoryEntry member = new DirectoryEntry(entry);
                        admins.Add(
                            new WinServiceAdmin(
                                member.Name, 
                                member.SchemaClassName,
                                member.Properties.Contains("FullName") ? member.Properties["FullName"].Value.ToString() : "",
                                member.Properties.Contains("LastLogin") ? member.Properties["LastLogin"].Value.ToString() : "", 
                                this.computerInformation
                            )
                        );
                    }
                }
            }
            foreach(WinServiceAdmin admin in admins)
            {
                await API.Parser.Instance.SendAdministratorAsync(admin);
            }
        }

        private WinServiceComputer GetComputerInformation()
        {
            WinServiceComputer computer = new WinServiceComputer();
            computer.WindowsVersion = Helper.GetOSFriendlyName();
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
    }
}
