using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Stock_Exchange.Application.Common.Auth;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles = Array.Empty<string>();

        public RoleAuthorizeAttribute()
        {
        }

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public RoleAuthorizeAttribute(params UserType[] roles)
        {
            _roles = roles.Select(r => r.ToString()).ToArray();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous) return;

            var currentUserService = context.HttpContext.RequestServices.GetService(typeof(ICurrentUserService)) as ICurrentUserService;
            var user = context.HttpContext.User;
            var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<Messages>>();

            if (user.Identity == null || !user.Identity.IsAuthenticated || currentUserService == null || !currentUserService.IsAuthenticated)
            {
                var reason = context.HttpContext.Items[AuthFailureReasons.ItemsKey] as string;
                var localizationKey = reason switch
                {
                    AuthFailureReasons.Expired => LocalizationKeys.AuthMessages.SessionExpired,
                    AuthFailureReasons.Revoked => LocalizationKeys.AuthMessages.SessionRevoked,
                    _ => LocalizationKeys.ExceptionMessages.Unauthorized
                };

                var message = localizer[localizationKey];
                var response = ApiResponse<object?>.Error(new Dictionary<string, string[]>(), message, StatusCodes.Status401Unauthorized);
                context.Result = new UnauthorizedObjectResult(response);
                return;
            }

            if (_roles.Any())
            {
                var userTypesClaim = user.FindFirst("UserTypes")?.Value;
                bool hasRequiredRole = false;

                foreach (var roleName in _roles)
                {
                    if (user.IsInRole(roleName))
                    {
                        hasRequiredRole = true;
                        break;
                    }

                    if (int.TryParse(userTypesClaim, out var userTypesInt) &&
                        Enum.TryParse<UserType>(roleName, ignoreCase: true, out var requiredType) &&
                        Enum.IsDefined(typeof(UserType), requiredType))
                    {
                        if ((int)requiredType == 0 && userTypesInt == 0)
                        {
                            hasRequiredRole = true;
                            break;
                        }
                        else if ((int)requiredType != 0 && (userTypesInt & (int)requiredType) != 0)
                        {
                            hasRequiredRole = true;
                            break;
                        }
                        else if (userTypesInt == (int)requiredType)
                        {
                            hasRequiredRole = true;
                            break;
                        }
                    }
                }

                if (!hasRequiredRole)
                {
                    var message = localizer[LocalizationKeys.ExceptionMessages.Forbidden];
                    var response = ApiResponse<object?>.Error(new Dictionary<string, string[]>(), message, StatusCodes.Status403Forbidden);
                    context.Result = new ObjectResult(response)
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                }
            }
        }
    }
}
