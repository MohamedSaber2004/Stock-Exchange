using FluentValidation;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Auth.Queries.GetUserProfile
{
    public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
    {
        private readonly ICurrentUserService _currentUserService;

        public GetUserProfileQueryValidator(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;

            RuleFor(x => x)
                .Must(_ => _currentUserService.IsAuthenticated && _currentUserService.UserId != Guid.Empty)
                .WithMessage(LocalizationKeys.ExceptionMessages.Unauthorized);
        }
    }
}
