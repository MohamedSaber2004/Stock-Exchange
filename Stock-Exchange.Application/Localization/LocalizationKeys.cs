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
            public const string EmailAuthenticationFailed = "ExceptionMessages.EmailAuthenticationFailed";
            public const string EmailServerRejected = "ExceptionMessages.EmailServerRejected";
            public const string NetworkConnectionFailed = "ExceptionMessages.NetworkConnectionFailed";
            public const string GoogleAuthNotConfigured = "ExceptionMessages.GoogleAuthNotConfigured";
            public const string GoogleAuthValidationUnavailable = "ExceptionMessages.GoogleAuthValidationUnavailable";
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
            public const string VerificationCodeRequired = "AuthMessages.VerificationCodeRequired";
            public const string InvalidVerificationCode = "AuthMessages.InvalidVerificationCode";
            public const string VerificationCodeExpired = "AuthMessages.VerificationCodeExpired";
            public const string ResetTokenRequired = "AuthMessages.ResetTokenRequired";
            public const string InvalidResetToken = "AuthMessages.InvalidResetToken";
            public const string ResetTokenExpired = "AuthMessages.ResetTokenExpired";
            public const string PasswordResetFailed = "AuthMessages.PasswordResetFailed";
            public const string PasswordResetSuccess = "AuthMessages.PasswordResetSuccess";
            public const string NewPasswordCannotBeOldPassword = "AuthMessages.NewPasswordCannotBeOldPassword";
            public const string RefreshTokenRequired = "AuthMessages.RefreshTokenRequired";
            public const string InvalidRefreshToken = "AuthMessages.InvalidRefreshToken";
            public const string RefreshTokenExpired = "AuthMessages.RefreshTokenExpired";
            public const string RefreshTokenRevoked = "AuthMessages.RefreshTokenRevoked";
            public const string SessionExpired = "AuthMessages.SessionExpired";
            public const string SessionRevoked = "AuthMessages.SessionRevoked";
            public const string GoogleIdTokenRequired = "AuthMessages.GoogleIdTokenRequired";
            public const string GoogleIdTokenTooLong = "AuthMessages.GoogleIdTokenTooLong";
            public const string InvalidGoogleToken = "AuthMessages.InvalidGoogleToken";
            public const string GoogleEmailRequired = "AuthMessages.GoogleEmailRequired";
            public const string GoogleEmailNotVerified = "AuthMessages.GoogleEmailNotVerified";
            public const string GoogleUserCreationFailed = "AuthMessages.GoogleUserCreationFailed";
            public const string GoogleAccountAlreadyLinked = "AuthMessages.GoogleAccountAlreadyLinked";
            public const string GoogleAccountLinkFailed = "AuthMessages.GoogleAccountLinkFailed";
            public const string GoogleProfileUpdateFailed = "AuthMessages.GoogleProfileUpdateFailed";
            public const string UserNotFound = "AuthMessages.UserNotFound";
            public const string UserInfoUpdated = "AuthMessages.UserInfoUpdated";
            public const string CurrentPasswordRequired = "AuthMessages.CurrentPasswordRequired";
            public const string PasswordChangeFailed = "AuthMessages.PasswordChangeFailed";
            public const string PasswordChangeSuccess = "AuthMessages.PasswordChangeSuccess";
        }

        public static class CountryMessages
        {
            public const string CountryNotFound = "CountryMessages.CountryNotFound";
            public const string CountryCodeRequired = "CountryMessages.CountryCodeRequired";
            public const string CountryArNameRequired = "CountryMessages.CountryArNameRequired";
            public const string CountryEnNameRequired = "CountryMessages.CountryEnNameRequired";
        }

        public static class AboutUsMessages
        {
            public const string AboutUsNotFound = "AboutUsMessages.AboutUsNotFound";
            public const string StoryRequired = "AboutUsMessages.StoryRequired";
            public const string StoryTooLong = "AboutUsMessages.StoryTooLong";
            public const string MissionRequired = "AboutUsMessages.MissionRequired";
            public const string MissionTooLong = "AboutUsMessages.MissionTooLong";
            public const string VisionRequired = "AboutUsMessages.VisionRequired";
            public const string VisionTooLong = "AboutUsMessages.VisionTooLong";
            public const string FeatureTitleRequired = "AboutUsMessages.FeatureTitleRequired";
            public const string FeatureTitleTooLong = "AboutUsMessages.FeatureTitleTooLong";
            public const string FeatureDescriptionRequired = "AboutUsMessages.FeatureDescriptionRequired";
            public const string FeatureDescriptionTooLong = "AboutUsMessages.FeatureDescriptionTooLong";
            public const string FeatureCategoryTooLong = "AboutUsMessages.FeatureCategoryTooLong";
        }

        public static class HelpCenterMessages
        {
            public const string HelpCenterNotFound = "HelpCenterMessages.HelpCenterNotFound";
            public const string IdRequired = "HelpCenterMessages.IdRequired";
            public const string InvalidCategoryId = "HelpCenterMessages.InvalidCategoryId";
            public const string CategoryNotFound = "HelpCenterMessages.CategoryNotFound";
            public const string TitleRequired = "HelpCenterMessages.TitleRequired";
            public const string TitleTooLong = "HelpCenterMessages.TitleTooLong";
            public const string ContentRequired = "HelpCenterMessages.ContentRequired";
            public const string ContentTooLong = "HelpCenterMessages.ContentTooLong";
            public const string SearchTooLong = "HelpCenterMessages.SearchTooLong";
        }

        public static class PrivacyPolicyMessages
        {
            public const string PrivacyPolicyNotFound = "PrivacyPolicyMessages.PrivacyPolicyNotFound";
            public const string TitleRequired = "PrivacyPolicyMessages.TitleRequired";
            public const string TitleTooLong = "PrivacyPolicyMessages.TitleTooLong";
            public const string DescriptionTooLong = "PrivacyPolicyMessages.DescriptionTooLong";
            public const string SectionTitleRequired = "PrivacyPolicyMessages.SectionTitleRequired";
            public const string SectionTitleTooLong = "PrivacyPolicyMessages.SectionTitleTooLong";
            public const string SectionContentRequired = "PrivacyPolicyMessages.SectionContentRequired";
            public const string SectionContentTooLong = "PrivacyPolicyMessages.SectionContentTooLong";
        }

        public static class TermsAndConditionsMessages
        {
            public const string TermsAndConditionsNotFound = "TermsAndConditionsMessages.TermsAndConditionsNotFound";
            public const string TitleRequired = "TermsAndConditionsMessages.TitleRequired";
            public const string TitleTooLong = "TermsAndConditionsMessages.TitleTooLong";
            public const string DescriptionTooLong = "TermsAndConditionsMessages.DescriptionTooLong";
            public const string SectionTitleRequired = "TermsAndConditionsMessages.SectionTitleRequired";
            public const string SectionTitleTooLong = "TermsAndConditionsMessages.SectionTitleTooLong";
            public const string SectionContentRequired = "TermsAndConditionsMessages.SectionContentRequired";
            public const string SectionContentTooLong = "TermsAndConditionsMessages.SectionContentTooLong";
            public const string TermsAndConditionsIdRequired = "TermsAndConditionsMessages.TermsAndConditionsIdRequired";
        }

        public static class HomeMessages
        {
            public const string HomeNotFound = "HomeMessages.HomeNotFound";
            public const string HeroTitleTooLong = "HomeMessages.HeroTitleTooLong";
            public const string HeroSubtitleTooLong = "HomeMessages.HeroSubtitleTooLong";
            public const string HeroImageUrlTooLong = "HomeMessages.HeroImageUrlTooLong";
        }

        public static class ArticleMessages
        {
            public const string ArticleNotFound = "ArticleMessages.ArticleNotFound";
            public const string TitleEnRequired = "ArticleMessages.TitleEnRequired";
            public const string TitleEnTooLong = "ArticleMessages.TitleEnTooLong";
            public const string TitleArRequired = "ArticleMessages.TitleArRequired";
            public const string TitleArTooLong = "ArticleMessages.TitleArTooLong";
            public const string ExcerptEnRequired = "ArticleMessages.ExcerptEnRequired";
            public const string ExcerptEnTooLong = "ArticleMessages.ExcerptEnTooLong";
            public const string ExcerptArRequired = "ArticleMessages.ExcerptArRequired";
            public const string ExcerptArTooLong = "ArticleMessages.ExcerptArTooLong";
            public const string AuthorNameRequired = "ArticleMessages.AuthorNameRequired";
            public const string AuthorNameTooLong = "ArticleMessages.AuthorNameTooLong";
            public const string ImageUrlTooLong = "ArticleMessages.ImageUrlTooLong";
            public const string IdRequired = "ArticleMessages.IdRequired";
            public const string SearchTooLong = "ArticleMessages.SearchTooLong";
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
