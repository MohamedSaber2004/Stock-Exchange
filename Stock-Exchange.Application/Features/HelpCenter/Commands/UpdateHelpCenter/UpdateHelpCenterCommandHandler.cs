using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenter.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.HelpCenter.Commands.UpdateHelpCenter
{
    public class UpdateHelpCenterCommandHandler : IRequestHandler<UpdateHelpCenterCommand, Result<HelpCenterDto>>
    {
        private readonly IHelpCenterRepository _helpCenterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateHelpCenterCommandHandler(
            IHelpCenterRepository helpCenterRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _helpCenterRepository = helpCenterRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<HelpCenterDto>> Handle(UpdateHelpCenterCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var helpCenter = await _helpCenterRepository.GetFirstAsync(
                h => h.Id == request.Id && !h.IsDeleted && h.IsActive,
                cancellationToken);

            if (helpCenter is null)
                throw new NotFoundException(LocalizationKeys.HelpCenterMessages.HelpCenterNotFound);

            helpCenter.TitleEn = request.TitleEn.Trim();
            helpCenter.TitleAr = request.TitleAr.Trim();
            helpCenter.ContentEn = request.ContentEn.Trim();
            helpCenter.ContentAr = request.ContentAr.Trim();
            helpCenter.CategoryId = request.CategoryId;
            if (request.DisplayOrder.HasValue)
                helpCenter.DisplayOrder = request.DisplayOrder.Value;

            _helpCenterRepository.Update(helpCenter);
            await _unitOfWork.SaveChangesAsync();

            return Result<HelpCenterDto>.Success(new HelpCenterDto
            {
                Id = helpCenter.Id,
                TitleEn = helpCenter.TitleEn,
                TitleAr = helpCenter.TitleAr,
                ContentEn = helpCenter.ContentEn,
                ContentAr = helpCenter.ContentAr,
                CategoryId = helpCenter.CategoryId,
                DisplayOrder = helpCenter.DisplayOrder
            });
        }
    }
}
