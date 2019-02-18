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
using static DTIService.Config.EnvManager;

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

        public async Task UploadInstalledProgramsAsync(WinServiceComputer computer, String csvPath)
        {
            Dictionary<string, string> contentParam = new Dictionary<string, string>
            {
                { APICommandField.COMMAND, APIActionField.INSTALLED_PROGRAMS },
                { APICommandField.SECURE_KEY, EnvManager.Instance.Environment.AccessKey },
                { APICommandField.MAC_ADDRESS, computer.Mac },
                { APICommandField.MACHINE_NAME, computer.Hostname },
                { APICommandField.CLIENT_VERSION, Assembly.GetExecutingAssembly().GetName().Version.ToString() },
                { APICommandField.DESCRIPTION, computer.Description }
            };
            Dictionary<string, byte[]> binaryParam = new Dictionary<string, byte[]>
            {
                { APICommandField.BIN_REPORT, File.ReadAllBytes(csvPath) }
            };
            string response = await SendFormAsync(EnvManager.Instance.Environment.ApiUri, contentParam, binaryParam);
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
            string response = await SendFormAsync(EnvManager.Instance.Environment.ApiUri, contentParam);
            if (!response.Equals(""))
            {
                LogWriter.Instance.Write("Registro de estação. Resposta: " + response);
            }
        }

        public async Task SendProdKeyAsync(WinServiceKey key)
        {
            Dictionary<string, string> contentParam = new Dictionary<string, string>
            {
                { APICommandField.COMMAND, APIActionField.PRODUCT_KEY },
                { APICommandField.SECURE_KEY, EnvManager.Instance.Environment.AccessKey },
                { APICommandField.MAC_ADDRESS, key.Computer.Mac },
                { APICommandField.PRODUCT_KEY, key.ProductKey },
                { APICommandField.PRODUCT_KEY_TYPE, key.Type },
                { APICommandField.DESCRIPTION, key.ProductName },
                { APICommandField.CLIENT_VERSION,  Assembly.GetExecutingAssembly().GetName().Version.ToString() }
            };
            string response = await SendFormAsync(EnvManager.Instance.Environment.ApiUri, contentParam);
            if (!response.Equals(""))
            {
                LogWriter.Instance.Write("Send key response: " + response);
            }
        }

        public async Task SendAdministratorAsync(WinServiceAdmin admin)
        {
            Dictionary<string, string> contentParam = new Dictionary<string, string>
            {
                { APICommandField.COMMAND, APIActionField.COMPUTER_ADMIN },
                { APICommandField.SECURE_KEY, EnvManager.Instance.Environment.AccessKey },
                { APICommandField.MAC_ADDRESS, admin.Computer.Mac },
                { APICommandField.ADMIN_NAME, admin.Name },
                { APICommandField.ADMIN_FULLNAME, admin.FullName },
                { APICommandField.ADMIN_TYPE, admin.Type },
                { APICommandField.ADMIN_LAST_LOGIN, admin.LastLogin },
                { APICommandField.CLIENT_VERSION,  Assembly.GetExecutingAssembly().GetName().Version.ToString() }
            };
            string response = await SendFormAsync(EnvManager.Instance.Environment.ApiUri, contentParam);
            if (!response.Equals(""))
            {
                LogWriter.Instance.Write("Send admin response: " + response);
            }
        }

        public async Task SendMachineStatus(WinServiceComputer computer, string status)
        {
            Dictionary<string, string> contentParam = new Dictionary<string, string>
            {
                { APICommandField.COMMAND, APIActionField.MACHINE_STATUS },
                { APICommandField.SECURE_KEY, EnvManager.Instance.Environment.AccessKey },
                { APICommandField.MAC_ADDRESS, computer.Mac },
                { APICommandField.LAST_STATUS, status },
                { APICommandField.CLIENT_VERSION,  Assembly.GetExecutingAssembly().GetName().Version.ToString() }
            };
            string response = await SendFormAsync(EnvManager.Instance.Environment.ApiUri, contentParam);
            if (!response.Equals(""))
            {
                LogWriter.Instance.Write("Machine status response: " + response);
            }
        }

        protected async Task<String> SendFormAsync(String URI, Dictionary<String, String> formField, Dictionary<String, byte[]> binFormField = null)
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
                    if (field.Key != APICommandField.SECURE_KEY)
                    {

                        LogWriter.Instance.Write("\"" + field.Key + "\" => \"" + field.Value + "\"");
                    } else
                    {
                        LogWriter.Instance.Write("\"" + field.Key + "\" => \"" + EnvManager.Instance.Environment + "." + APICommandField.SECURE_KEY + "\"");
                    }
                }
                LogWriter.Instance.Write("Exception: " + ex);
            }
            return "";
        }

    }
}
