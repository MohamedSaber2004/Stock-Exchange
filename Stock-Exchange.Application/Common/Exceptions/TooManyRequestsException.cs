using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Exceptions.Base;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Common.Exceptions
{
    public class TooManyRequestsException : BaseCustomException
    {
        public TooManyRequestsException()
            : base(LocalizationKeys.ExceptionMessages.TooManyRequests, StatusCodes.Status429TooManyRequests)
        {
        }

        public TooManyRequestsException(string localizationKey, params object[] args)
            : base(localizationKey, StatusCodes.Status429TooManyRequests, args)
        {
        }

        public TooManyRequestsException(string localizationKey, Exception innerException, params object[] args)
            : base(localizationKey, innerException, StatusCodes.Status429TooManyRequests, args)
        {
        }
    }
}
