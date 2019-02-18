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

        public const string ADMIN_NAME = "admin_name";
        public const string ADMIN_FULLNAME = "admin_fullname";
        public const string ADMIN_TYPE = "admin_type";
        public const string ADMIN_LAST_LOGIN = "admin_last_login";

        public const string LAST_STATUS = "last_status";
    }

    public class APIActionField
    {
        public const string INSTALLED_PROGRAMS = "1";
        public const string REGISTRATION = "2";
        public const string PRODUCT_KEY = "3";
        public const string COMPUTER_ADMIN = "4";
        public const string MACHINE_STATUS = "5";
    }

    public class APIProductKeyType
    {
        public const string WINDOWS = "WinProdKey";
        public const string OEM = "OEMKey";
        public const string OFFICE = "OfficeKey";
        public const string GENERIC = "Generic";
    }

    public class APIComputerStatus
    {
        public const string WAKEUP = "1";
        public const string ONLINE = "2";
        public const string SLEEP = "3";
    }

}
