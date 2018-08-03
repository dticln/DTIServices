namespace DTIService.API
{
    public class APICommandField
    {
        public const string COMMAND = "api_command";
        public const string SECURE_KEY = "secure_key";
        public const string MACHINE_NAME = "machine_name";
        public const string MAC_ADDRESS = "mac_address";
        public const string IPV4 = "ipv4_address";
        public const string DESCRIPTION = "description_generic";
        public const string CLIENT_VERSION = "client_version";
        public const string WINDOWS_VERSION = "windows_version";
        public const string PRODUCT_KEY = "product_key";
        public const string PRODUCT_KEY_TYPE = "product_key_type";
        public const string BIN_REPORT = "installed_report";

    }

    public class APIActionField
    {
        public const string INSTALLED_PROGRAMS = "1";
        public const string REGISTRATION = "2";
        public const string PRODUCT_KEY = "3";
    }

    public class APIProductKeyType
    {
        public const string WINDOWS = "WinProdKey";
        public const string OEM = "OEMKey";
        public const string OFFICE = "OfficeKey";
        public const string GENERIC = "Generic";
    }

}
