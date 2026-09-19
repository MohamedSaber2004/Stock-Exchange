using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Exceptions.Base;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Common.Exceptions
{
    public class UnAuthorizedException : BaseCustomException
    {
        public UnAuthorizedException()
            : base(LocalizationKeys.ExceptionMessages.Unauthorized, StatusCodes.Status401Unauthorized)
        {
        }

        public UnAuthorizedException(string localizationKey, params object[] args)
            : base(localizationKey, StatusCodes.Status401Unauthorized, args)
        {
        }

        public UnAuthorizedException(string localizationKey, Exception innerException, params object[] args)
            : base(localizationKey, innerException, StatusCodes.Status401Unauthorized, args)
        {
        }
    }
}
