using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Experts.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Experts.Commands.UpdateExpert
{
    public class UpdateExpertCommandHandler : IRequestHandler<UpdateExpertCommand, Result<ExpertDto>>
    {
        private readonly IExpertRepository _expertRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateExpertCommandHandler(
            IExpertRepository expertRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _expertRepository = expertRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ExpertDto>> Handle(UpdateExpertCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var expert = await _expertRepository.GetFirstAsync(e => e.Id == request.Id, cancellationToken);
            if (expert is null)
                throw new NotFoundException("Expert not found");

            expert.FullNameEn = request.FullNameEn.Trim();
            expert.FullNameAr = request.FullNameAr.Trim();
            expert.TitleEn = request.TitleEn.Trim();
            expert.TitleAr = request.TitleAr.Trim();
            expert.AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim();
            expert.DisplayOrder = request.DisplayOrder;
            expert.IsFeaturedOnHome = request.IsFeaturedOnHome;

            if (expert.IsActive != request.IsActive)
            {
                expert.SetActiveState(request.IsActive, _currentUserService.UserId.ToString());
            }

            _expertRepository.Update(expert);
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
