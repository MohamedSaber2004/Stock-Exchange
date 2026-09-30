using Stock_Exchange.Routes;

namespace Stock_Exchange.Routes.V1
{
    public static class ApiRoutes
    {
        public static class Attachments
        {
            public const string Base = BaseRoutes.Base + "/attachments";

            public const string Upload = "upload";
            public const string UploadMultiple = "upload-multiple";
            public const string Download = "download";
            public const string Update = "update";
        }

        public static class Authentication
        {
            public const string Base = BaseRoutes.Base + "/authentication";

            public const string Login = "login";
            public const string LoginWithGoogle = "login-with-google";
            public const string Register = "register";
            public const string Logout = "logout";
            public const string RefreshToken = "refresh-token";
            public const string ForgetPassword = "forget-password";
            public const string VerifyOtp = "verify-otp";
            public const string ResetPassword = "reset-password";
            public const string ChangePassword = "change-password";
            public const string GetUserProfile = "my-profile";
            public const string UpdateProfile = "update/myprofile";
        }

        public static class AboutUs
        {
            public const string Base = BaseRoutes.Base + "/about-us";

            public const string Get = "";
            public const string View = "view";
            public const string Update = "";
        }

        public static class HelpCenter
        {
            public const string Base = BaseRoutes.Base + "/help-center";

            public const string GetAll = "";
            public const string GetById = "{id}";
            public const string View = "view";
            public const string Add = "";
            public const string Update = "";
            public const string Delete = "{id}";
        }

        public static class HelpCenterCategories
        {
            public const string Base = BaseRoutes.Base + "/help-center-categories";

            public const string GetAll = "";
            public const string GetById = "{id}";
            public const string Add = "";
            public const string Update = "";
            public const string Delete = "{id}";
        }

        public static class PrivacyPolicy
        {
            public const string Base = BaseRoutes.Base + "/privacy-policy";

            public const string Get = "";
            public const string View = "view";
            public const string Update = "";
            public const string Delete = "";
            public const string DeleteById = "{id}";
        }

        public static class TermsAndConditions
        {
            public const string Base = BaseRoutes.Base + "/terms-and-conditions";

            public const string Get = "";
            public const string View = "view";
            public const string Update = "";
            public const string Delete = "";
            public const string DeleteById = "{id}";
        }

        public static class Home
        {
            public const string Base = BaseRoutes.Base + "/home";

            public const string Get = "";
            public const string UpdateHero = "hero";
            public const string UpdateNews = "news";
            public const string UpdateServices = "services";
            public const string UpdateArticles = "articles";
            public const string UpdateVideos = "videos";
            public const string UpdatePlans = "plans";
            public const string UpdateExperts = "experts";
        }

        public static class Countries
        {
            public const string Base = BaseRoutes.Base + "/countries";

            public const string GetAll = "";
        }

        public static class Articles
        {
            public const string Base = BaseRoutes.Base + "/articles";

            public const string GetAll = "";
            public const string GetById = "{id}";
            public const string Add = "";
            public const string Update = "";
            public const string Delete = "{id}";
        }
    }
}
