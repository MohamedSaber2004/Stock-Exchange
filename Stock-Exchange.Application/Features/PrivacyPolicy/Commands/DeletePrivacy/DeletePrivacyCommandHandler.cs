using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.PrivacyPolicy.Commands.DeletePrivacy
{
    public class DeletePrivacyCommandHandler : IRequestHandler<DeletePrivacyCommand, Result<bool>>
    {
        private readonly IPrivacyPolicyRepository _privacyPolicyRepository;
        private readonly IPrivacyPolicySectionRepository _privacyPolicySectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeletePrivacyCommandHandler(
            IPrivacyPolicyRepository privacyPolicyRepository,
            IPrivacyPolicySectionRepository privacyPolicySectionRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _privacyPolicyRepository = privacyPolicyRepository;
            _privacyPolicySectionRepository = privacyPolicySectionRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(DeletePrivacyCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            if (request.Id.HasValue && request.Id.Value != Guid.Empty)
            {
                var section = await _privacyPolicySectionRepository.GetFirstAsync(
                    s => s.Id == request.Id.Value && !s.IsDeleted && s.IsActive,
                    cancellationToken);

                if (section is not null)
                {
                    _privacyPolicySectionRepository.Delete(section);
                    await _unitOfWork.SaveChangesAsync();
                    return Result<bool>.Success(true);
                }

                var policyById = await _privacyPolicyRepository.GetFirstAsync(
                    p => p.Id == request.Id.Value && !p.IsDeleted && p.IsActive,
                    cancellationToken);

                if (policyById is not null)
                {
                    _privacyPolicyRepository.Delete(policyById);
                    await _unitOfWork.SaveChangesAsync();
                    return Result<bool>.Success(true);
                }

                throw new NotFoundException(LocalizationKeys.PrivacyPolicyMessages.PrivacyPolicyNotFound);
            }

            var policy = await _privacyPolicyRepository.GetFirstAsync(
                p => !p.IsDeleted && p.IsActive,
                cancellationToken);

            if (policy is null)
                throw new NotFoundException(LocalizationKeys.PrivacyPolicyMessages.PrivacyPolicyNotFound);

            _privacyPolicyRepository.Delete(policy);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}
