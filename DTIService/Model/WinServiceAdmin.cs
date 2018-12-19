using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTIService.Model
{
    class WinServiceAdmin
    {
        private string name;
        private string type;
        private string fullName;
        private string lastLogin;
        private WinServiceComputer computer;

        public string Name { get => name; set => name = value; }
        public string Type { get => type; set => type = value; }
        public string FullName { get => fullName; set => fullName = value; }
        public string LastLogin { get => lastLogin; set => lastLogin = value; }
        internal WinServiceComputer Computer { get => computer; set => computer = value; }
        
        public WinServiceAdmin(string name, 
            string type, 
            string fullName, 
            string lastLogin, 
            WinServiceComputer computer)
        {
            Name = name;
            Type = type;
            FullName = fullName;
            LastLogin = lastLogin;
            Computer = computer;
        }
    }
}
