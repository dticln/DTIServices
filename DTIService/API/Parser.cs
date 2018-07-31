using DTIService.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DTIService.Util;

namespace DTIService.API
{
    class Parser : IParser
    {
        private static readonly Parser instance = new Parser();
        private const string ACCESS_KEY = "";
        private const string API_URI = "https://www.ufrgs.br/cacln/winService/api";
 
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
                { APICommandField.SECURE_KEY, ACCESS_KEY },
                { APICommandField.MACHINE_NAME, Environment.MachineName }
            };
            Dictionary<string, byte[]> binaryParam = new Dictionary<string, byte[]>
            {
                { APICommandField.BIN_REPORT, File.ReadAllBytes(csvPath) }
            };
            string response = await SendForm(API_URI, contentParam, binaryParam);
            if (!response.Equals(""))
            {
                LogWriter.Instance.Write("Installed Programs response: " + response);
            }
        }

        public async Task RegistrationAsync(string macAddress, string ipv4, string description = "")
        {
            Dictionary<string, string> contentParam = new Dictionary<string, string>
            {
                { APICommandField.COMMAND, APIActionField.REGISTRATION },
                { APICommandField.SECURE_KEY, ACCESS_KEY },
                { APICommandField.MACHINE_NAME, Environment.MachineName },
                { APICommandField.MAC_ADDRESS, macAddress },
                { APICommandField.IPV4, ipv4 },
                { APICommandField.DESCRIPTION, description },
            };
            string response = await SendForm(API_URI, contentParam);
            if (!response.Equals(""))
            {
                LogWriter.Instance.Write("Registration response: " + response);
            }
        }

        public async Task SendProdKeyAsync(string productKey, string keyType = APIProductKeyType.GENERIC, string description = "")
        {
            Dictionary<string, string> contentParam = new Dictionary<string, string>
            {
                { APICommandField.COMMAND, APIActionField.PRODUCT_KEY },
                { APICommandField.SECURE_KEY, ACCESS_KEY },
                { APICommandField.MACHINE_NAME, Environment.MachineName },
                { APICommandField.PRODUCT_KEY, productKey },
                { APICommandField.PRODUCT_KEY_TYPE, keyType },
                { APICommandField.DESCRIPTION, description },
            };
            string response = await SendForm(API_URI, contentParam);
            if (!response.Equals(""))
            {
                LogWriter.Instance.Write("Send Windows Key response: " + response);
            }
        }

        private async Task<String> SendForm(String URI, Dictionary<String, String> formField, Dictionary<String, byte[]> binFormField = null)
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
                HttpResponseMessage response = await httpClient.PostAsync(API_URI, form);
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
