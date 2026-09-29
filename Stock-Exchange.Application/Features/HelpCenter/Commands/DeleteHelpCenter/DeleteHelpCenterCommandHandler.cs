using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.HelpCenter.Commands.DeleteHelpCenter
{
    public class DeleteHelpCenterCommandHandler : IRequestHandler<DeleteHelpCenterCommand, Result<bool>>
    {
        private readonly IHelpCenterRepository _helpCenterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteHelpCenterCommandHandler(
            IHelpCenterRepository helpCenterRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _helpCenterRepository = helpCenterRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(DeleteHelpCenterCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var helpCenter = await _helpCenterRepository.GetFirstAsync(
                h => h.Id == request.Id && !h.IsDeleted && h.IsActive,
                cancellationToken);

            if (helpCenter is null)
                throw new NotFoundException(LocalizationKeys.HelpCenterMessages.HelpCenterNotFound);

            _helpCenterRepository.Delete(helpCenter);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}
