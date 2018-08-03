using System.Reflection;

namespace DTIService.Model
{
    class WinServiceComputer
    {
        private string mac;
        private string ipv4;
        private string hostname;
        private string description;
        private string clientVersion;
        private string windowsVersion;

        public string Mac { get => mac; set => mac = value; }
        public string Ipv4 { get => ipv4; set => ipv4 = value; }
        public string Hostname { get => hostname; set => hostname = value; }
        public string Description { get => description; set => description = value; }
        public string ClientVersion { get => clientVersion; set => clientVersion = value; }
        public string WindowsVersion { get => windowsVersion; set => windowsVersion = value; }

        public WinServiceComputer()
        {
            Hostname = System.Environment.MachineName;
            ClientVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Description = "";
        }
    }
}
