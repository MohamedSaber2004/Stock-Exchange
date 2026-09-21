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
            public const string Register = "register";
            public const string Logout = "logout";
            public const string RefreshToken = "refresh-token";
            public const string ForgetPassword = "forget-password";
            public const string VerifyOtp = "verify-otp";
            public const string ResetPassword = "reset-password";
            public const string GetUserProfile = "my-profile";
            public const string UpdateProfile = "update/myprofile";
        }
    }
}
