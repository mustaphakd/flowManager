namespace Analyzer.Core
{
    public static class Constants
    {
        public const string Home = "home";
        public const string Login = "login";
        public const string Register = "register";
        public const string HomeMenu = "menu";

        public const string Separator = "**";

    }

    public static class RestEndpoints
    {
        public const string Login = "security/login";
        public const string Registation = "security/login";
        public const string Logout = "security/logout";
    }

    public static class CacheKeys
    {
        public const string User_Auth = "auth_token";
        public const string User_Profile = "user_prof";
    }

    public static class StoragePathes
    {
        //public const string Products = "Images"+ Constants.Separator +"Products";
    }

}
