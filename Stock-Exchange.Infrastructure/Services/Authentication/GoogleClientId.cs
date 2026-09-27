namespace Stock_Exchange.Infrastructure.Services.Authentication
{
    internal static class GoogleClientId
    {
        private const string ClientIdDomainSuffix = ".apps.googleusercontent.com";

        public static IEnumerable<string> ToAudienceForms(string clientId)
        {
            var trimmed = clientId.Trim();
            yield return trimmed;

            if (!trimmed.Contains('.', StringComparison.Ordinal))
                yield return trimmed + ClientIdDomainSuffix;
        }
    }
}
