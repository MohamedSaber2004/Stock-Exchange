using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Exceptions.Base;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Common.Exceptions
{
    public class MethodNotAllowedException : BaseCustomException
    {
        public MethodNotAllowedException()
            : base(LocalizationKeys.ExceptionMessages.MethodNotAllowed, StatusCodes.Status405MethodNotAllowed)
        {
        }

        public MethodNotAllowedException(string localizationKey, params object[] args)
            : base(localizationKey, StatusCodes.Status405MethodNotAllowed, args)
        {
        }
    }
}