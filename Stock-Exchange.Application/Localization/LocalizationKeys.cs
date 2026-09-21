namespace Stock_Exchange.Application.Localization
{
    public static class LocalizationKeys
    {
        public static class ActionResults
        {
            public const string Ok = "ActionResults.Ok";
            public const string Created = "ActionResults.Created";
            public const string Accepted = "ActionResults.Accepted";
            public const string Deleted = "ActionResults.Deleted";
            public const string Updated = "ActionResults.Updated";
        }

        public static class Attachments
        {
            public const string FileEmpty = "Attachments.FileEmpty";
            public const string FileNotFound = "Attachments.FileNotFound";
            public const string UploadFailed = "Attachments.UploadFailed";
            public const string InvalidFormat = "Attachments.InvalidFormat";
            public const string InvalidImageFormat = "Attachments.InvalidImageFormat";
            public const string InvalidVideoFormat = "Attachments.InvalidVideoFormat";
            public const string InvalidAudioFormat = "Attachments.InvalidAudioFormat";
            public const string InvalidFileFormat = "Attachments.InvalidFileFormat";
            public const string NoMediaProvided = "Attachments.NoMediaProvided";
        }

        public static class ExceptionMessages
        {
            public const string Validation = "ExceptionMessages.Validation";
            public const string InvalidModelState = "ExceptionMessages.InvalidModelState";
            public const string NotFound = "ExceptionMessages.NotFound";
            public const string BadRequest = "ExceptionMessages.BadRequest";
            public const string Unauthorized = "ExceptionMessages.Unauthorized";
            public const string Forbidden = "ExceptionMessages.Forbidden";
            public const string Conflict = "ExceptionMessages.Conflict";
            public const string InternalServerError = "ExceptionMessages.InternalServerError";
            public const string UnknownException = "ExceptionMessages.UnknownException";
            public const string TooManyRequests = "ExceptionMessages.TooManyRequests";
            public const string PayloadTooLarge = "ExceptionMessages.PayloadTooLarge";
            public const string UnprocessableEntity = "ExceptionMessages.UnprocessableEntity";
            public const string ServiceUnavailable = "ExceptionMessages.ServiceUnavailable";
            public const string NotAcceptable = "ExceptionMessages.NotAcceptable";
            public const string Gone = "ExceptionMessages.Gone";
            public const string MethodNotAllowed = "ExceptionMessages.MethodNotAllowed";
            public const string UnsupportedMediaType = "ExceptionMessages.UnsupportedMediaType";
            public const string RequestTimeout = "ExceptionMessages.RequestTimeout";
        }

        public static class AuthMessages
        {
            public const string InvalidCredentials = "AuthMessages.InvalidCredentials";
            public const string AccountDeleted = "AuthMessages.AccountDeleted";
            public const string AccountDeactivated = "AuthMessages.AccountDeactivated";
            public const string AccountPendingApproval = "AuthMessages.AccountPendingApproval";
            public const string EmailRequired = "AuthMessages.EmailRequired";
            public const string InvalidEmail = "AuthMessages.InvalidEmail";
            public const string PasswordRequired = "AuthMessages.PasswordRequired";
            public const string FullNameRequired = "AuthMessages.FullNameRequired";
            public const string ConfirmPasswordRequired = "AuthMessages.ConfirmPasswordRequired";
            public const string PasswordsDoNotMatch = "AuthMessages.PasswordsDoNotMatch";
            public const string PhoneNumberRequired = "AuthMessages.PhoneNumberRequired";
            public const string InvalidPhoneNumber = "AuthMessages.InvalidPhoneNumber";
            public const string EmailAlreadyExists = "AuthMessages.EmailAlreadyExists";
            public const string PhoneNumberAlreadyExists = "AuthMessages.PhoneNumberAlreadyExists";
            public const string UserCreationFailed = "AuthMessages.UserCreationFailed";
        }

        public static class EmailMessages
        {
            public const string ResetPasswordSubject = "EmailMessages.ResetPasswordSubject";
        }

        public static class Users
        {
            public const string FullNameEmpty = "Users.FullNameEmpty";
            public const string PasswordResetTokenEmpty = "Users.PasswordResetTokenEmpty";
            public const string VerificationCodeEmpty = "Users.VerificationCodeEmpty";
        }

        public static class Errors
        {
            public const string FullNameEmpty = "Users.FullNameEmpty";
            public const string PasswordResetTokenEmpty = "Users.PasswordResetTokenEmpty";
            public const string VerificationCodeEmpty = "Users.VerificationCodeEmpty";

            public static class User
            {
                public const string FullNameEmpty = "Users.FullNameEmpty";
                public const string PasswordResetTokenEmpty = "Users.PasswordResetTokenEmpty";
                public const string VerificationCodeEmpty = "Users.VerificationCodeEmpty";
            }
        }
    }
}
