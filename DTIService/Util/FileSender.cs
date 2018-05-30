using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DTIService.Util
{
    class FileSender
    {
        private static readonly FileSender instance = new FileSender();
        private FileSender() { }

        public static FileSender Instance
        {
            get
            {
                return instance;
            }
        }

        public async Task UploadInstalledProgramsAsync(String csvPath)
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent("api_command"), "1");
            form.Add(new StringContent("secure_key"), "f105b52eae569b272423faabe1bf83bc315324b9aeade3f4a1f5d437d608a841");
            form.Add(new StringContent("machine_name"), Environment.MachineName);
            byte[] report = File.ReadAllBytes(csvPath);
            form.Add(new ByteArrayContent(report, 0, report.Length), "installed_report", Environment.MachineName + ".csv");
            HttpResponseMessage response = await httpClient.PostAsync("PostUrl", form);
            response.EnsureSuccessStatusCode();
            httpClient.Dispose();
            string sd = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(sd);
        }
    }
}
