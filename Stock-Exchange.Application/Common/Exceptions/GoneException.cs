using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Exceptions.Base;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Common.Exceptions
{
    public class GoneException : BaseCustomException
    {
        public GoneException()
            : base(LocalizationKeys.ExceptionMessages.Gone, StatusCodes.Status410Gone)
        {
        }

        public GoneException(string localizationKey, params object[] args)
            : base(localizationKey, StatusCodes.Status410Gone, args)
        {
        }
    }
}