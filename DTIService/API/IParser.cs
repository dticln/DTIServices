﻿using DTIService.API;
using DTIService.Model;
using System.Threading.Tasks;

namespace DTIService.Interface
{
    interface IParser
    {
        Task UploadInstalledProgramsAsync(WinServiceComputer computer, string csvPath);
        Task RegistrationAsync(WinServiceComputer computer);
        Task SendProdKeyAsync(string productKey, string keyType = APIProductKeyType.GENERIC, string description = "");
    }
}
