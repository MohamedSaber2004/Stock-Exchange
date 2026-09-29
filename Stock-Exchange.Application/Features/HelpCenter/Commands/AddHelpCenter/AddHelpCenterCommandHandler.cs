using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenter.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;
using HelpCenterEntity = Stock_Exchange.Domain.Entities.HelpCenter;

namespace Stock_Exchange.Application.Features.HelpCenter.Commands.AddHelpCenter
{
    public class AddHelpCenterCommandHandler : IRequestHandler<AddHelpCenterCommand, Result<HelpCenterDto>>
    {
        private readonly IHelpCenterRepository _helpCenterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddHelpCenterCommandHandler(
            IHelpCenterRepository helpCenterRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _helpCenterRepository = helpCenterRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<HelpCenterDto>> Handle(AddHelpCenterCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var nextDisplayOrder = await _helpCenterRepository
                .GetAllAsync(h => h.IsActive)
                .Select(h => (int?)h.DisplayOrder)
                .MaxAsync(cancellationToken) ?? 0;

            var helpCenter = new HelpCenterEntity
            {
                TitleEn = request.TitleEn.Trim(),
                TitleAr = request.TitleAr.Trim(),
                ContentEn = request.ContentEn.Trim(),
                ContentAr = request.ContentAr.Trim(),
                CategoryId = request.CategoryId,
                DisplayOrder = nextDisplayOrder + 1
            };

            await _helpCenterRepository.AddAsync(helpCenter);
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
