using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DTIService.Util
{
    class FileSender
    {
        private static readonly FileSender instance = new FileSender();
        private const string ACCESS_KEY = "";
        private const string API_URI = "https://www.ufrgs.br/cacln/winService/api";

        private const string API_COMMAND_FIELD = "api_command";
        private const string API_KEY_FIELD = "secure_key";
        private const string API_MACHINE_FIELD = "machine_name";
        private const string API_REPORT_FIELD = "installed_report";

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
                    { new StringContent("1"), API_COMMAND_FIELD },
                    { new StringContent(ACCESS_KEY), API_KEY_FIELD },
                    { new StringContent(Environment.MachineName), API_MACHINE_FIELD }
                };
                byte[] report = File.ReadAllBytes(csvPath);
                form.Add(new ByteArrayContent(report, 0, report.Length), API_REPORT_FIELD, Environment.MachineName + ".csv");
                HttpResponseMessage response = await httpClient.PostAsync(API_URI, form);
                response.EnsureSuccessStatusCode();
                httpClient.Dispose();
                string sd = response.Content.ReadAsStringAsync().Result;
                LogWriter.Instance.Write("Reposta do upload: " + sd);
            } catch (Exception ex)
            {
                LogWriter.Instance.Write("Erro no upload: " + ex);
            }
        }
    }
}
