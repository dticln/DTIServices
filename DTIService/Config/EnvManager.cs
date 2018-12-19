using System;

namespace DTIService.Config
{
    class EnvManager
    {
        private static EnvType envType = EnvType.DEVELOPMENT;

        private static readonly EnvManager instance = new EnvManager(EnvManager.envType);
        private IEnviroment environment;
        public IEnviroment Environment { get => environment; }
      
        private EnvManager(EnvType envType) {
            switch (envType)
            {
                case EnvType.DEVELOPMENT:
                    this.environment = new Development();
                    break;
                case EnvType.PRODUCTION:
                    this.environment = new Production();
                    break;
                case EnvType.TEST:
                default:
                    throw new Exception("Não foi possível definir o ambiente de desenvolvimento.");
            }
        }

        public static EnvManager Instance
        {
            get
            { 
                return instance;
            }
        }

        public enum EnvType
        {
            DEVELOPMENT = 1,
            PRODUCTION = 2,
            TEST = 3
        }; 
    }


}
