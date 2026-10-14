using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Experts.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
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

            var expert = await _expertRepository
                .GetAllWithIncluding(e => e.Id == request.Id, e => e.ConsultationAreas)
                .FirstOrDefaultAsync(cancellationToken);

            if (expert is null)
                throw new NotFoundException(LocalizationKeys.ExpertMessages.ExpertNotFound);

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

            if (request.ConsultationAreas != null)
            {
                var requestIds = request.ConsultationAreas
                    .Where(a => a.Id.HasValue && a.Id.Value != Guid.Empty)
                    .Select(a => a.Id!.Value)
                    .ToHashSet();

                var toRemove = expert.ConsultationAreas
                    .Where(c => !c.IsDeleted && !requestIds.Contains(c.Id))
                    .ToList();

                foreach (var item in toRemove)
                {
                    item.MarkAsDeleted(_currentUserService.UserId.ToString());
                }

                var order = 1;
                foreach (var area in request.ConsultationAreas)
                {
                    if (string.IsNullOrWhiteSpace(area.TextAr) && string.IsNullOrWhiteSpace(area.TextEn))
                        continue;

                    var existing = area.Id.HasValue && area.Id.Value != Guid.Empty
                        ? expert.ConsultationAreas.FirstOrDefault(c => c.Id == area.Id.Value && !c.IsDeleted)
                        : null;

                    if (existing != null)
                    {
                        existing.TextEn = (area.TextEn ?? string.Empty).Trim();
                        existing.TextAr = (area.TextAr ?? string.Empty).Trim();
                        existing.DisplayOrder = area.DisplayOrder > 0 ? area.DisplayOrder : order++;
                        existing.MarkAsUpdated(_currentUserService.UserId.ToString());
                    }
                    else
                    {
                        expert.ConsultationAreas.Add(new ExpertConsultationArea
                        {
                            TextEn = (area.TextEn ?? string.Empty).Trim(),
                            TextAr = (area.TextAr ?? string.Empty).Trim(),
                            DisplayOrder = area.DisplayOrder > 0 ? area.DisplayOrder : order++
                        });
                    }
                }
            }
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
                CreatedAt = expert.CreatedAt,
                ConsultationAreas = expert.ConsultationAreas
                    .Where(c => !c.IsDeleted)
                    .OrderBy(c => c.DisplayOrder)
                    .Select(c => new ExpertConsultationAreaDto
                    {
                        Id = c.Id,
                        TextEn = c.TextEn,
                        TextAr = c.TextAr,
                        DisplayOrder = c.DisplayOrder
                    }).ToList()
            });
        }
    }
}
