using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Exceptions.Base;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Common.Exceptions
{
    public class UnsupportedMediaTypeException : BaseCustomException
    {
        public UnsupportedMediaTypeException()
            : base(LocalizationKeys.ExceptionMessages.UnsupportedMediaType, StatusCodes.Status415UnsupportedMediaType)
        {
        }

        public UnsupportedMediaTypeException(string localizationKey, params object[] args)
            : base(localizationKey, StatusCodes.Status415UnsupportedMediaType, args)
        {
        }
    }
}