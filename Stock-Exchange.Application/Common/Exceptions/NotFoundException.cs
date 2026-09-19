using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Exceptions.Base;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Common.Exceptions
{
    public class NotFoundException : BaseCustomException
    {
        public NotFoundException()
            : base(LocalizationKeys.ExceptionMessages.NotFound, StatusCodes.Status404NotFound)
        {
        }

        public NotFoundException(string localizationKey, params object[] args)
            : base(localizationKey, StatusCodes.Status404NotFound, args)
        {
        }

        public NotFoundException(string localizationKey, Exception innerException, params object[] args)
            : base(localizationKey, innerException, StatusCodes.Status404NotFound, args)
        {
        }

        public NotFoundException(string entityName, object key)
            : base(LocalizationKeys.ExceptionMessages.NotFound, StatusCodes.Status404NotFound, entityName, key)
        {
        }
    }
}
