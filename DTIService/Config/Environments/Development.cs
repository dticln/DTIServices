namespace DTIService.Config
{
    class Development : IEnviroment
    {
        public string EnvName { get { return "ENV_DEVELOPMENT"; } }
        public string AccessKey { get { return "f105b52eae569b272423faabe1bf83bc315324b9aeade3f4a1f5d437d608a841"; } }
        public string ApiUri { get { return "http://localhost/cacln/winService/api"; } }
        public string WinKeySalt { get { return "E213418F9FAFABD1ACD3BC407173A2DD9DCF47FB1910F281B4C51529FF325C74"; } }
    }
}
