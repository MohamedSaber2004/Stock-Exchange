namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface ILocalizationProvider
    {
        string GetLocalizedString(string key, string? culture = null);

        string GetLocalizedString(string key, string? culture, params object[] args);
    }
}
