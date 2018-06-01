using System;

namespace DTIService.Model
{
    class ProgramLog
    {
        private string displayName;
        private string version;
        private string publisher;
        private bool is64Bits;
        public string DisplayName { get => displayName; set => displayName = value; }
        public string Version { get => version; set => version = value; }
        public bool Is64Bits { get => is64Bits; set => is64Bits = value; }
        public string Publisher { get => publisher; set => publisher = value; }

        public ProgramLog(String displayName, String publisher, String version, bool is64Bits)
        {
            this.displayName = displayName;
            this.publisher = publisher;
            this.version = version;
            this.is64Bits = is64Bits;
        }
    }
}
