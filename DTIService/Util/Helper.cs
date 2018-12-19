using Microsoft.Win32;
using System;
using System.DirectoryServices;

namespace DTIService.Util
{
    class Helper
    {

        public static string GetOSFriendlyName()
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
        
        public static string HKLMGetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
        }

        public static DirectoryEntry GetGroupFromDirectory(DirectoryEntry directory, string name)
        {
            DirectoryEntry group = null;
            try
            {
                group = directory.Children.Find(name, "group");
            }
            catch (Exception ex) { }
            return group;
        }

    }
}
