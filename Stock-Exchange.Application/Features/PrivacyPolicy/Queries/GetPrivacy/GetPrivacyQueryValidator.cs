using FluentValidation;

namespace Stock_Exchange.Application.Features.PrivacyPolicy.Queries.GetPrivacy
{
    public class GetPrivacyQueryValidator : AbstractValidator<GetPrivacyQuery>
    {
        public GetPrivacyQueryValidator()
        {
        }
    }
}
