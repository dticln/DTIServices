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
            LogWriter.Instance.Write("Tentando realizar upload do arquivo: " + csvPath);
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "DTI Service App");
                MultipartFormDataContent form = new MultipartFormDataContent
                {
                    { new StringContent("1"), "api_command" },
                    { new StringContent("f105b52eae569b272423faabe1bf83bc315324b9aeade3f4a1f5d437d608a841"), "secure_key" },
                    { new StringContent( Environment.MachineName), "machine_name" }
                };

                byte[] report = File.ReadAllBytes(csvPath);
                form.Add(new ByteArrayContent(report, 0, report.Length), "installed_report", Environment.MachineName + ".csv");
                HttpResponseMessage response = await httpClient.PostAsync("http://localhost/CaCln/winservice/api", form);
                response.EnsureSuccessStatusCode();
                httpClient.Dispose();
                string sd = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(sd);
                LogWriter.Instance.Write("Reposta do upload: " + sd);
            } catch (Exception ex)
            {
                LogWriter.Instance.Write("Erro no upload: " + ex);
            }
        }
    }
}
