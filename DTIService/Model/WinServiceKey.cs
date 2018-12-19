using DTIService.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTIService.Model
{
    class WinServiceKey
    {
        private string productName;
        private string productId;
        private string productKey;
        private string installationFolder;
        private string servicePack;
        private string modifiedTime;
        private string type;
        private WinServiceComputer computer;

        public string ProductName { get => productName; set => productName = value; }
        public string ProductId { get => productId; set => productId = value; }
        public string ProductKey { get => productKey; set => productKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(value)); }
        public string InstallationFolder { get => installationFolder; set => installationFolder = value; }
        public string ServicePack { get => servicePack; set => servicePack = value; }
        public string ModifiedTime { get => modifiedTime; set => modifiedTime = value; }
        internal WinServiceComputer Computer { get => computer; set => computer = value; }

        public string Type {
            get => type;
            set {
                if (value.Contains("Windows"))
                {
                    if (value.Contains("OEM"))
                    {
                        type = API.APIProductKeyType.OEM;
                    } else
                    {
                        type = API.APIProductKeyType.WINDOWS;
                    }
                } else if (value.Contains("Office"))
                {
                    type = API.APIProductKeyType.OFFICE;
                } else
                {
                    type = API.APIProductKeyType.GENERIC;
                }
            }
        }

        public WinServiceKey(string productName, 
            string productId, 
            string productKey, 
            string installationFolder, 
            string servicePack, 
            string modifiedTime, 
            WinServiceComputer computer)
        {
            ProductName = productName;
            ProductId = productId;
            ProductKey = productKey;
            InstallationFolder = installationFolder;
            ServicePack = servicePack;
            ModifiedTime = modifiedTime;
            Computer = computer;
            Type = productName;
        }
    }
}
