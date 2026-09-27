namespace Stock_Exchange.Application.Common.Models
{
    /// <summary>
    /// The subset of Google ID token claims this application relies on. Decoupling from the
    /// Google SDK payload keeps the authentication contract independent of how the token is verified.
    /// </summary>
    public sealed record GoogleUserProfile(
        string Subject,
        string? Email,
        string? Name,
        string? Picture,
        bool EmailVerified);
}
