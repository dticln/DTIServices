using DTIService.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DTIService.Util;
using DTIService.Model;
using System.Reflection;
using DTIService.Config;

namespace DTIService.API
{
    class Parser : IParser
    {
        private static readonly Parser instance = new Parser();

        private Parser() { }

        public static Parser Instance
        {
            get
            {
                return instance;
            }
        }

        public async Task UploadInstalledProgramsAsync(String csvPath)
        {
            Dictionary<string, string> contentParam = new Dictionary<string, string>
            {
                { APICommandField.COMMAND, APIActionField.INSTALLED_PROGRAMS },
                { APICommandField.SECURE_KEY, EnvManager.Instance.Environment.AccessKey },
                { APICommandField.MACHINE_NAME, Environment.MachineName }
            };
            Dictionary<string, byte[]> binaryParam = new Dictionary<string, byte[]>
            {
                { APICommandField.BIN_REPORT, File.ReadAllBytes(csvPath) }
            };
            string response = await SendForm(EnvManager.Instance.Environment.ApiUri, contentParam, binaryParam);
            if (!response.Equals(""))
            {
                LogWriter.Instance.Write("Programas instalados. Resposta: " + response);
            }
        }

        public async Task RegistrationAsync(WinServiceComputer computer)
        {
            Dictionary<string, string> contentParam = new Dictionary<string, string>
            {
                { APICommandField.COMMAND, APIActionField.REGISTRATION },
                { APICommandField.SECURE_KEY, EnvManager.Instance.Environment.AccessKey },
                { APICommandField.MACHINE_NAME, computer.Hostname },
                { APICommandField.MAC_ADDRESS, computer.Mac },
                { APICommandField.IPV4, computer.Ipv4 },
                { APICommandField.DESCRIPTION, computer.Description },
                { APICommandField.CLIENT_VERSION, Assembly.GetExecutingAssembly().GetName().Version.ToString() },
                { APICommandField.WINDOWS_VERSION, computer.WindowsVersion }
            };
            string response = await SendForm(EnvManager.Instance.Environment.ApiUri, contentParam);
            if (!response.Equals(""))
            {
                LogWriter.Instance.Write("Registro de estação. Resposta: " + response);
            }
        }

        public async Task SendProdKeyAsync(string productKey, string keyType = APIProductKeyType.GENERIC, string description = "")
        {
            Dictionary<string, string> contentParam = new Dictionary<string, string>
            {
                { APICommandField.COMMAND, APIActionField.PRODUCT_KEY },
                { APICommandField.SECURE_KEY, EnvManager.Instance.Environment.AccessKey },
                { APICommandField.MACHINE_NAME, Environment.MachineName },
                { APICommandField.PRODUCT_KEY, productKey },
                { APICommandField.PRODUCT_KEY_TYPE, keyType },
                { APICommandField.DESCRIPTION, description },
                { APICommandField.CLIENT_VERSION,  Assembly.GetExecutingAssembly().GetName().Version.ToString() }
            };
            string response = await SendForm(EnvManager.Instance.Environment.ApiUri, contentParam);
            if (!response.Equals(""))
            {
                LogWriter.Instance.Write("Send Windows Key response: " + response);
            }
        }

        protected async Task<String> SendForm(String URI, Dictionary<String, String> formField, Dictionary<String, byte[]> binFormField = null)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "DTI Service App");
                MultipartFormDataContent form = new MultipartFormDataContent();
                foreach (KeyValuePair<String, String> field in formField)
                {
                    form.Add(new StringContent(field.Value), field.Key);
                }
                if (binFormField != null)
                {
                    foreach (KeyValuePair<String, byte[]> field in binFormField)
                    {
                        form.Add(new ByteArrayContent(field.Value, 0, field.Value.Length), field.Key, field.Key);
                    }
                }
                HttpResponseMessage response = await httpClient.PostAsync(EnvManager.Instance.Environment.ApiUri, form);
                response.EnsureSuccessStatusCode();
                httpClient.Dispose();
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Write("Erro ao enviar formulário para: \"" + URI + "\": ");
                LogWriter.Instance.Write("Formulário:");
                foreach (KeyValuePair<String, String> field in formField)
                {
                    LogWriter.Instance.Write("\"" + field.Key + "\" => \"" + field.Value + "\"");
                }
                LogWriter.Instance.Write("Exception: " + ex);
            }
            return "";
        }

    }
}
