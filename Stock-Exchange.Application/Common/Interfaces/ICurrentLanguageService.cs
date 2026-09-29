using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface ICurrentLanguageService
    {
        Language Language { get; }
        string LanguageCode { get; }
    }
}
