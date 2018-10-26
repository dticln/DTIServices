using DTIService.Config;
using DTIService.Model;
using DTIService.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DTIService.External
{
    class ProduKeyConnector
    {
        private static readonly ProduKeyConnector instance = new ProduKeyConnector();
        private ProduKeyConnector() {}

        public static ProduKeyConnector Instance
        {
            get
            {
                return instance;
            }
        }

        private string executableName = @"pk.exe";
        private string executableBinary = @"C:\DTI Services\bin\pk.bin";
        private string executablePath = @"C:\DTI Services\bin\pk.exe";
        private string licenseFile = @"C:\DTI Services\bin\licenses.json";

        private void Start()
        {
            //this.EncryptBinary();
            this.DecryptBinary();
            if (File.Exists(this.executablePath))
            {
                Process process = new Process();
                process.StartInfo.FileName = this.executableName;
                process.StartInfo.Arguments = "/sjson licenses.json";
                process.StartInfo.WorkingDirectory = @"C:\DTI Services\bin";
                process.Start();
                int id = process.Id;
                Process tempProc = Process.GetProcessById(id);
                tempProc.WaitForExit();
            }
        }

        private void End()
        {
            if (File.Exists(this.licenseFile))
            {
                File.Delete(this.licenseFile);
            }
            if (File.Exists(this.executablePath))
            {
                File.Delete(this.executablePath);
            }

        }

        public List<WinServiceKey> FindProductKeys(WinServiceComputer computer)
        {
            List<WinServiceKey> keys = new List<WinServiceKey>();
            try
            {
                this.Start();
                if (File.Exists(this.licenseFile))
                {
                    string plainText = File.ReadAllText(this.licenseFile);
                    JArray json = (JArray)JsonConvert.DeserializeObject(plainText);
                    foreach (JObject obj in json)
                    {
                        keys.Add(new WinServiceKey(
                            obj.Property("Product Name").Value.ToString(),
                            obj.Property("Product ID").Value.ToString(),
                            obj.Property("Product Key").Value.ToString(),
                            obj.Property("Installation Folder ").Value.ToString(),
                            obj.Property("Service Pack").Value.ToString(),
                            obj.Property("Modified Time").Value.ToString(),
                            computer
                        ));
                    }
                }
                this.End();
            } catch (Exception ex){
                LogWriter.Instance.Write(ex.ToString());
            }
            return keys;
        }
        
        private void EncryptBinary()
        {
            byte[] bytes = File.ReadAllBytes(@"C:\DTI Services\bin\ProduKey.exe");
            string enc = StringCipher.Encrypt(bytes, EnvManager.Instance.Environment.ProduKeySecret);
            StreamWriter file = new StreamWriter(this.executableBinary);
            file.Write(enc);
            file.Close();
        }
        
        private void DecryptBinary()
        {
            string enc = File.ReadAllText(this.executableBinary);
            byte[] str = StringCipher.Decrypt(enc, EnvManager.Instance.Environment.ProduKeySecret);
            BinaryWriter file = new BinaryWriter(new FileStream(@"C:\DTI Services\bin\pk.exe", FileMode.Create));
            file.Write(str);
            file.Close();
        }
    }
}
