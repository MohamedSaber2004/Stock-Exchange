using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock_Exchange.Application.Common.Behaviours;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Common.Services;
using Stock_Exchange.Application.Features.Attachments.Commands.DownloadFile;
using Stock_Exchange.Application.Features.Attachments.Commands.UpdateFile;
using Stock_Exchange.Application.Features.Attachments.Commands.UploadFile;
using Stock_Exchange.Application.Features.Attachments.Commands.UploadMultipleFiles;
using Stock_Exchange.Application.Features.AboutUs.Commands.UpdateAboutUs;
using Stock_Exchange.Application.Features.HelpCenter.Commands.AddHelpCenter;
using Stock_Exchange.Application.Features.HelpCenter.Commands.DeleteHelpCenter;
using Stock_Exchange.Application.Features.HelpCenter.Commands.UpdateHelpCenter;
using Stock_Exchange.Application.Features.HelpCenter.Queries.GetAllHelpCenters;
using Stock_Exchange.Application.Features.HelpCenter.Queries.GetHelpCenterById;
using Stock_Exchange.Application.Features.HelpCenterCategories.Commands.AddHelpCenterCategory;
using Stock_Exchange.Application.Features.HelpCenterCategories.Commands.DeleteHelpCenterCategory;
using Stock_Exchange.Application.Features.HelpCenterCategories.Commands.UpdateHelpCenterCategory;
using Stock_Exchange.Application.Features.HelpCenterCategories.Queries.GetAllHelpCenterCategories;
using Stock_Exchange.Application.Features.HelpCenterCategories.Queries.GetAllHelpCenterCategoryById;
using Stock_Exchange.Application.Features.Auth.Commands.ChangePassword;
using Stock_Exchange.Application.Features.Auth.Commands.DeleteAccount;
using Stock_Exchange.Application.Features.Auth.Commands.ForgetPassword;
using Stock_Exchange.Application.Features.Auth.Commands.Login;
using Stock_Exchange.Application.Features.Auth.Commands.LoginWithGoogle;
using Stock_Exchange.Application.Features.Auth.Commands.Logout;
using Stock_Exchange.Application.Features.Auth.Commands.RefreshToken;
using Stock_Exchange.Application.Features.Auth.Commands.Register;
using Stock_Exchange.Application.Features.Auth.Commands.ResetPassword;
using Stock_Exchange.Application.Features.Auth.Commands.UpdateUserInfo;
using Stock_Exchange.Application.Features.Auth.Commands.UploadProfilePicture;
using Stock_Exchange.Application.Features.Auth.Commands.VerifyOtp;
using Stock_Exchange.Application.Features.Auth.Queries.GetUserProfile;
using Stock_Exchange.Application.Features.PrivacyPolicy.Commands.DeletePrivacy;
using Stock_Exchange.Application.Features.PrivacyPolicy.Commands.UpdatePrivacy;
using Stock_Exchange.Application.Features.PrivacyPolicy.Queries.GetPrivacy;
using Stock_Exchange.Application.Features.TermsAndConditions.Commands.DeleteTermsAndConditions;
using Stock_Exchange.Application.Features.TermsAndConditions.Commands.UpdateTermsAndConditions;
using Stock_Exchange.Application.Features.TermsAndConditions.Queries.GetTermsAndConditions;
using Stock_Exchange.Application.Features.Articles.Commands.AddArticle;
using Stock_Exchange.Application.Features.Articles.Commands.DeleteArticle;
using Stock_Exchange.Application.Features.Articles.Commands.UpdateArticle;
using Stock_Exchange.Application.Features.Articles.Queries.GetAllArticles;
using Stock_Exchange.Application.Features.Articles.Queries.GetArticleById;
using Stock_Exchange.Application.Features.Videos.Commands.AddVideo;
using Stock_Exchange.Application.Features.Videos.Commands.DeleteVideo;
using Stock_Exchange.Application.Features.Videos.Commands.UpdateVideo;
using Stock_Exchange.Application.Features.Videos.Queries.GetAllVideos;
using Stock_Exchange.Application.Features.Videos.Queries.GetVideoById;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SecurityOptions>(configuration.GetSection(SecurityOptions.SectionName));
            services.Configure<SecurityHeadersOptions>(configuration.GetSection("Security:Headers"));
            services.Configure<CorsOptions>(configuration.GetSection("Security:Cors"));
            services.Configure<AntiforgeryOptions>(configuration.GetSection("Security:Antiforgery"));
            services.Configure<RequestLimitsOptions>(configuration.GetSection("Security:RequestLimits"));
            services.Configure<HstsOptions>(configuration.GetSection("Security:Hsts"));
            services.Configure<IpRateLimitingOptions>(configuration.GetSection(IpRateLimitingOptions.SectionName));
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.Configure<IdentityOptions>(configuration.GetSection(nameof(IdentityOptions)));
            services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
            services.Configure<GoogleAuthSettings>(configuration.GetSection(nameof(GoogleAuthSettings)));

            services.AddMediatR(typeof(DependencyInjection).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddTransient<IValidator<UploadFileCommand>, UploadFileCommandValidator>();
            services.AddTransient<IValidator<UploadMultipleFilesCommand>, UploadMultipleFilesCommandValidator>();
            services.AddTransient<IValidator<DownloadFileCommand>, DownloadFileCommandValidator>();
            services.AddTransient<IValidator<UpdateFileCommand>, UpdateFileCommandValidator>();
            services.AddTransient<IValidator<LoginCommand>, LoginCommandValidator>();
            services.AddTransient<IValidator<LoginWithGoogleCommand>, LoginWithGoogleCommandValidator>();
            services.AddTransient<IValidator<SignupCommand>, SignupCommandValidator>();
            services.AddTransient<IValidator<ForgetPasswordCommand>, ForgetPasswordCommandValidator>();
            services.AddTransient<IValidator<VerifyOtpCommand>, VerifyOtpCommandValidator>();
            services.AddTransient<IValidator<ResetPasswordCommand>, ResetPasswordCommandValidator>();
            services.AddTransient<IValidator<ChangePasswordCommand>, ChangePasswordCommandValidator>();
            services.AddTransient<IValidator<RefreshTokenCommand>, RefreshTokenCommandValidator>();
            services.AddTransient<IValidator<LogoutCommand>, LogoutCommandValidator>();
            services.AddTransient<IValidator<DeleteAccountCommand>, DeleteAccountCommandValidator>();
            services.AddTransient<IValidator<UpdateUserInfoCommand>, UpdateUserInfoCommandValidator>();
            services.AddTransient<IValidator<UploadProfilePictureCommand>, UploadProfilePictureCommandValidator>();
            services.AddTransient<IValidator<GetUserProfileQuery>, GetUserProfileQueryValidator>();
            services.AddTransient<IValidator<UpdateAboutUsCommand>, UpdateAboutUsCommandValidator>();
            services.AddTransient<IValidator<AddHelpCenterCommand>, AddHelpCenterCommandValidator>();
            services.AddTransient<IValidator<UpdateHelpCenterCommand>, UpdateHelpCenterCommandValidator>();
            services.AddTransient<IValidator<DeleteHelpCenterCommand>, DeleteHelpCenterCommandValidator>();
            services.AddTransient<IValidator<GetAllHelpCentersQuery>, GetAllHelpCentersQueryValidator>();
            services.AddTransient<IValidator<GetHelpCenterByIdQuery>, GetHelpCenterByIdQueryValidator>();
            services.AddTransient<IValidator<AddHelpCenterCategoryCommand>, AddHelpCenterCategoryCommandValidator>();
            services.AddTransient<IValidator<UpdateHelpCenterCategoryCommand>, UpdateHelpCenterCategoryCommandValidator>();
            services.AddTransient<IValidator<DeleteHelpCenterCategoryCommand>, DeleteHelpCenterCategoryCommandValidator>();
            services.AddTransient<IValidator<GetAllHelpCenterCategoriesQuery>, GetAllHelpCenterCategoriesQueryValidator>();
            services.AddTransient<IValidator<GetHelpCenterCategoryByIdQuery>, GetHelpCenterCategoryByIdQueryValidator>();
            services.AddTransient<IValidator<GetAllHelpCenterCategoryQueryById>, GetAllHelpCenterCategoryQueryValidatorById>();
            services.AddTransient<IValidator<GetPrivacyQuery>, GetPrivacyQueryValidator>();
            services.AddTransient<IValidator<UpdatePrivacyCommand>, UpdatePrivacyCommandValidator>();
            services.AddTransient<IValidator<DeletePrivacyCommand>, DeletePrivacyCommandValidator>();
            services.AddTransient<IValidator<GetTermsAndConditionsQuery>, GetTermsAndConditionsQueryValidator>();
            services.AddTransient<IValidator<UpdateTermsAndConditionsCommand>, UpdateTermsAndConditionsCommandValidator>();
            services.AddTransient<IValidator<DeleteTermsAndConditionsCommand>, DeleteTermsAndConditionsCommandValidator>();
            services.AddTransient<IValidator<GetAllArticlesQuery>, GetAllArticlesQueryValidator>();
            services.AddTransient<IValidator<GetArticleByIdQuery>, GetArticleByIdQueryValidator>();
            services.AddTransient<IValidator<AddArticleCommand>, AddArticleCommandValidator>();
            services.AddTransient<IValidator<UpdateArticleCommand>, UpdateArticleCommandValidator>();
            services.AddTransient<IValidator<DeleteArticleCommand>, DeleteArticleCommandValidator>();
            services.AddTransient<IValidator<GetAllVideosQuery>, GetAllVideosQueryValidator>();
            services.AddTransient<IValidator<GetVideoByIdQuery>, GetVideoByIdQueryValidator>();
            services.AddTransient<IValidator<AddVideoCommand>, AddVideoCommandValidator>();
            services.AddTransient<IValidator<UpdateVideoCommand>, UpdateVideoCommandValidator>();
            services.AddTransient<IValidator<DeleteVideoCommand>, DeleteVideoCommandValidator>();

            // Users Validators
            services.AddTransient<IValidator<Stock_Exchange.Application.Features.Users.Queries.GetAllUsers.GetAllUsersQuery>, Stock_Exchange.Application.Features.Users.Queries.GetAllUsers.GetAllUsersQueryValidator>();
            services.AddTransient<IValidator<Stock_Exchange.Application.Features.Users.Queries.GetUserById.GetUserByIdQuery>, Stock_Exchange.Application.Features.Users.Queries.GetUserById.GetUserByIdQueryValidator>();
            services.AddTransient<IValidator<Stock_Exchange.Application.Features.Users.Commands.AddUser.AddUserCommand>, Stock_Exchange.Application.Features.Users.Commands.AddUser.AddUserCommandValidator>();
            services.AddTransient<IValidator<Stock_Exchange.Application.Features.Users.Commands.UpdateUser.UpdateUserCommand>, Stock_Exchange.Application.Features.Users.Commands.UpdateUser.UpdateUserCommandValidator>();
            services.AddTransient<IValidator<Stock_Exchange.Application.Features.Users.Commands.DeleteUser.DeleteUserCommand>, Stock_Exchange.Application.Features.Users.Commands.DeleteUser.DeleteUserCommandValidator>();
            services.AddTransient<IValidator<Stock_Exchange.Application.Features.Users.Commands.AdminChangePassword.AdminChangePasswordCommand>, Stock_Exchange.Application.Features.Users.Commands.AdminChangePassword.AdminChangePasswordCommandValidator>();

            // ActivityLogs Validators
            services.AddTransient<IValidator<Stock_Exchange.Application.Features.ActivityLogs.Queries.GetAllActivityLogs.GetAllActivityLogsQuery>, Stock_Exchange.Application.Features.ActivityLogs.Queries.GetAllActivityLogs.GetAllActivityLogsQueryValidator>();
            services.AddTransient<IValidator<Stock_Exchange.Application.Features.ActivityLogs.Queries.GetActivityLogById.GetActivityLogByIdQuery>, Stock_Exchange.Application.Features.ActivityLogs.Queries.GetActivityLogById.GetActivityLogByIdQueryValidator>();

            services.AddSingleton<ILocalizationProvider, JsonLocalizationProvider>();

            UploadPaths.Configure(configuration);

            return services;
        }
    }
}
