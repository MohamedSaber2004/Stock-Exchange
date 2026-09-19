using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Exceptions.Base;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Common.Exceptions
{
    public class PayloadTooLargeException : BaseCustomException
    {
        public PayloadTooLargeException()
            : base(LocalizationKeys.ExceptionMessages.PayloadTooLarge, StatusCodes.Status413PayloadTooLarge)
        {
        }

        public PayloadTooLargeException(string localizationKey, params object[] args)
            : base(localizationKey, StatusCodes.Status413PayloadTooLarge, args)
        {
        }
    }
}