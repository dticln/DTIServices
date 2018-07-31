using DTIService.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTIService.Interface
{
    interface IParser
    {
        Task RegistrationAsync(string macAdress, string ipv4, string description = "");
        Task SendProdKeyAsync(string productKey, string keyType = APIProductKeyType.GENERIC, string description = "");
    }
}
