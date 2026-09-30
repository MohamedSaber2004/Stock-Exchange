using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.TermsAndConditions.Commands.DeleteTermsAndConditions
{
    public class DeleteTermsAndConditionsCommandHandler : IRequestHandler<DeleteTermsAndConditionsCommand, Result<bool>>
    {
        private readonly ITermsAndConditionsRepository _termsAndConditionsRepository;
        private readonly ITermsAndConditionsSectionRepository _termsAndConditionsSectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteTermsAndConditionsCommandHandler(
            ITermsAndConditionsRepository termsAndConditionsRepository,
            ITermsAndConditionsSectionRepository termsAndConditionsSectionRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _termsAndConditionsRepository = termsAndConditionsRepository;
            _termsAndConditionsSectionRepository = termsAndConditionsSectionRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(DeleteTermsAndConditionsCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            if (request.Id.HasValue && request.Id.Value != Guid.Empty)
            {
                var section = await _termsAndConditionsSectionRepository.GetFirstAsync(
                    s => s.Id == request.Id.Value && !s.IsDeleted && s.IsActive,
                    cancellationToken);

                if (section is not null)
                {
                    _termsAndConditionsSectionRepository.Delete(section);
                    await _unitOfWork.SaveChangesAsync();
                    return Result<bool>.Success(true);
                }

                var policyById = await _termsAndConditionsRepository.GetFirstAsync(
                    p => p.Id == request.Id.Value && !p.IsDeleted && p.IsActive,
                    cancellationToken);

                if (policyById is not null)
                {
                    _termsAndConditionsRepository.Delete(policyById);
                    await _unitOfWork.SaveChangesAsync();
                    return Result<bool>.Success(true);
                }

                throw new NotFoundException(LocalizationKeys.TermsAndConditionsMessages.TermsAndConditionsNotFound);
            }

            var policy = await _termsAndConditionsRepository.GetFirstAsync(
                p => !p.IsDeleted && p.IsActive,
                cancellationToken);

            if (policy is null)
                throw new NotFoundException(LocalizationKeys.TermsAndConditionsMessages.TermsAndConditionsNotFound);

            _termsAndConditionsRepository.Delete(policy);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}
