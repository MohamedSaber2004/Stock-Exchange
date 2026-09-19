using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Exceptions.Base;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Common.Exceptions
{
    public class NotAcceptableException : BaseCustomException
    {
        public NotAcceptableException()
            : base(LocalizationKeys.ExceptionMessages.NotAcceptable, StatusCodes.Status406NotAcceptable)
        {
        }

        public NotAcceptableException(string localizationKey, params object[] args)
            : base(localizationKey, StatusCodes.Status406NotAcceptable, args)
        {
        }
    }
}