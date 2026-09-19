using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Exceptions.Base;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Common.Exceptions
{
    public class RequestTimeoutException : BaseCustomException
    {
        public RequestTimeoutException()
            : base(LocalizationKeys.ExceptionMessages.RequestTimeout, StatusCodes.Status408RequestTimeout)
        {
        }

        public RequestTimeoutException(string localizationKey, params object[] args)
            : base(localizationKey, StatusCodes.Status408RequestTimeout, args)
        {
        }
    }
}