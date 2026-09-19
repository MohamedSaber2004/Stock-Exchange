using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Exceptions.Base;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Common.Exceptions
{
    public class UnprocessableEntityException : BaseCustomException
    {
        public UnprocessableEntityException()
            : base(LocalizationKeys.ExceptionMessages.UnprocessableEntity, StatusCodes.Status422UnprocessableEntity)
        {
        }

        public UnprocessableEntityException(string localizationKey, params object[] args)
            : base(localizationKey, StatusCodes.Status422UnprocessableEntity, args)
        {
        }
    }
}