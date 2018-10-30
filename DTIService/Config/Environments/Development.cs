namespace DTIService.Config
{
    class Development : IEnviroment
    {
        public string EnvName { get { return "ENV_DEVELOPMENT"; } }
        public string AccessKey { get { return "f105b52eae569b272423faabe1bf83bc315324b9aeade3f4a1f5d437d608a841"; } }
        public string ApiUri { get { return "http://localhost/cacln/winService/api"; } }
        public string ProduKeySecret { get { return "014372BF0CA859F374EA165B5AF9C8D674F26B1CF2C6110D930D7BDC22F571C6"; } }
    }
}
