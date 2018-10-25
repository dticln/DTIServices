namespace DTIService.Config
{
    public interface IEnviroment
    {

        string AccessKey { get; }
        string ApiUri { get; }
        string EnvName { get; }
        string WinKeySalt { get;  }
    }
}
