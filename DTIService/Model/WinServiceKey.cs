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
        private string key;
        private string salt;
        private string description;
        private string type;
        private WinServiceComputer computer;

        public string Key {
            get { return key; }
            set {
                this.key = Convert.ToBase64String(Encoding.UTF8.GetBytes(value + this.salt));
            }
        }
        public string Description { get => description; set => description = value; }
        public string Type { get => type; set => type = value; }
        internal WinServiceComputer Computer { get => computer; set => computer = value; }

        public WinServiceKey(WinServiceComputer computer)
        {
            this.computer = computer;
            this.salt = EnvManager.Instance.Environment.WinKeySalt;
            this.Type = API.APIProductKeyType.GENERIC;
            this.description = "";
        }

        public WinServiceKey(WinServiceComputer computer, string key)
        {
            this.computer = computer;
            this.salt = EnvManager.Instance.Environment.WinKeySalt;
            this.Key = key;
            this.Type = API.APIProductKeyType.GENERIC;
            this.description = "";
        }

        public WinServiceKey(WinServiceComputer computer, string key, string keyType)
        {
            this.computer = computer;
            this.salt = EnvManager.Instance.Environment.WinKeySalt;
            this.Key = key;
            this.Type = keyType;
            this.description = "";
        }


    }
}
