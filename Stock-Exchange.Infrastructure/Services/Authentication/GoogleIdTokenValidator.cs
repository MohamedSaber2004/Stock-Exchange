using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Stock_Exchange.Application.Common.Options;

namespace Stock_Exchange.Infrastructure.Services.Authentication
{
    public class GoogleIdTokenValidator
    {
        private static readonly string[] ValidIssuers =
        [
            "accounts.google.com",
            "https://accounts.google.com"
        ];

        private static readonly TimeSpan KeyCacheLifetime = TimeSpan.FromHours(1);
        private static readonly TimeSpan ClockSkew = TimeSpan.FromMinutes(5);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GoogleAuthSettings _settings;
        private readonly ILogger<GoogleIdTokenValidator> _logger;
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        private IReadOnlyList<SecurityKey> _cachedKeys = Array.Empty<SecurityKey>();
        private DateTimeOffset _keysFetchedAt = DateTimeOffset.MinValue;

        public GoogleIdTokenValidator(
            IHttpClientFactory httpClientFactory,
            IOptions<GoogleAuthSettings> settings,
            ILogger<GoogleIdTokenValidator> logger)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<JsonWebToken> ValidateAsync(string idToken, CancellationToken cancellationToken)
        {
            var validAudiences = GetValidAudiences();
            if (validAudiences.Count == 0)
                throw new InvalidOperationException("GoogleAuthSettings:WebClientId is not configured.");

            JsonWebToken token;
            try
            {
                token = new JsonWebToken(idToken);
            }
            catch (Exception ex)
            {
                throw new SecurityTokenMalformedException("The ID token is not a well-formed JWT.")
                {
                    Data = { ["Inner"] = ex.Message }
                };
            }

            var signingKey = await GetSigningKeyAsync(token.Kid, cancellationToken);

            if (signingKey is null)
                throw new SecurityTokenValidationException(
                    $"No Google signing key matches key id '{token.Kid}'.");

            var result = await new JsonWebTokenHandler().ValidateTokenAsync(
                idToken,
                new TokenValidationParameters
                {
                    ValidAudiences = validAudiences,
                    ValidIssuers = ValidIssuers,
                    IssuerSigningKey = signingKey,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = ClockSkew,
                    RequireSignedTokens = true
                });

            if (!result.IsValid)
                throw new SecurityTokenValidationException(result.Exception?.Message ?? "Token validation failed.");

            return token;
        }

        private IReadOnlyList<string> GetValidAudiences() =>
        [
            .. new[] { _settings.WebClientId }
                .Concat(_settings.WebClientIds ?? [])
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .Distinct(StringComparer.Ordinal)
        ];

        private async Task<SecurityKey?> GetSigningKeyAsync(string? keyId, CancellationToken cancellationToken)
        {
            var keys = await GetKeysAsync(forceRefresh: false, cancellationToken);
            var match = FindKey(keys, keyId);

            if (match is null)
            {
                keys = await GetKeysAsync(forceRefresh: true, cancellationToken);
                match = FindKey(keys, keyId);
            }

            return match;
        }

        private static SecurityKey? FindKey(IReadOnlyList<SecurityKey> keys, string? keyId) =>
            keys.FirstOrDefault(k => string.Equals(k.KeyId, keyId, StringComparison.Ordinal));

        private async Task<IReadOnlyList<SecurityKey>> GetKeysAsync(bool forceRefresh, CancellationToken cancellationToken)
        {
            var isStale = DateTimeOffset.UtcNow - _keysFetchedAt > KeyCacheLifetime;

            if (!forceRefresh && !isStale && _cachedKeys.Count > 0)
                return _cachedKeys;

            await _refreshLock.WaitAsync(cancellationToken);
            try
            {
                if (!forceRefresh && !isStale && _cachedKeys.Count > 0)
                    return _cachedKeys;

                var httpClient = _httpClientFactory.CreateClient();
                var keySet = await httpClient.GetFromJsonAsync<JsonWebKeySet>(GoogleKeysEndpoint, cancellationToken);

                if (keySet is null || keySet.Keys.Count == 0)
                    throw new InvalidOperationException("Google returned an empty signing key set.");

                _cachedKeys = keySet.Keys.Select(k => (SecurityKey)k).ToArray();
                _keysFetchedAt = DateTimeOffset.UtcNow;


                return _cachedKeys;
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        private const string GoogleKeysEndpoint = "https://www.googleapis.com/oauth2/v3/certs";
    }
}
