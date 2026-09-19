using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Exceptions.Base;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Common.Exceptions
{
    public class ServiceUnavailableException : BaseCustomException
    {
        public ServiceUnavailableException()
            : base(LocalizationKeys.ExceptionMessages.ServiceUnavailable, StatusCodes.Status503ServiceUnavailable)
        {
        }

        public ServiceUnavailableException(string localizationKey, params object[] args)
            : base(localizationKey, StatusCodes.Status503ServiceUnavailable, args)
        {
        }
    }
}