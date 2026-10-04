using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Experts.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Experts.Commands.AddExpert
{
    public class AddExpertCommandHandler : IRequestHandler<AddExpertCommand, Result<ExpertDto>>
    {
        private readonly IExpertRepository _expertRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddExpertCommandHandler(
            IExpertRepository expertRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _expertRepository = expertRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ExpertDto>> Handle(AddExpertCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var expert = new Expert
            {
                FullNameEn = request.FullNameEn.Trim(),
                FullNameAr = request.FullNameAr.Trim(),
                TitleEn = request.TitleEn.Trim(),
                TitleAr = request.TitleAr.Trim(),
                AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim(),
                DisplayOrder = request.DisplayOrder,
                IsFeaturedOnHome = request.IsFeaturedOnHome
            };

            if (!request.IsActive)
            {
                expert.SetActiveState(false, _currentUserService.UserId.ToString());
            }

            await _expertRepository.AddAsync(expert);
            await _unitOfWork.SaveChangesAsync();

            return Result<ExpertDto>.Success(new ExpertDto
            {
                Id = expert.Id,
                FullNameEn = expert.FullNameEn,
                FullNameAr = expert.FullNameAr,
                TitleEn = expert.TitleEn,
                TitleAr = expert.TitleAr,
                AvatarUrl = expert.AvatarUrl,
                DisplayOrder = expert.DisplayOrder,
                IsFeaturedOnHome = expert.IsFeaturedOnHome,
                IsActive = expert.IsActive,
                CreatedAt = expert.CreatedAt
            });
        }
    }
}
