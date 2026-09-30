using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeExperts
{
    public class UpdateHomeExpertsCommandHandler : IRequestHandler<UpdateHomeExpertsCommand, Result<List<HomeExpertDto>>>
    {
        private readonly IExpertRepository _expertRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentLanguageService _currentLanguageService;

        public UpdateHomeExpertsCommandHandler(
            IExpertRepository expertRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICurrentLanguageService currentLanguageService)
        {
            _expertRepository = expertRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<List<HomeExpertDto>>> Handle(UpdateHomeExpertsCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var existingExperts = await _expertRepository.GetAllAsync(e => e.IsActive).ToListAsync(cancellationToken);
            var requestIds = request.Items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToHashSet();

            // Soft-delete items that were omitted
            foreach (var item in existingExperts.Where(e => !requestIds.Contains(e.Id)))
            {
                _expertRepository.Delete(item);
            }

            // Upsert items from request
            foreach (var item in request.Items)
            {
                if (item.Id.HasValue && existingExperts.FirstOrDefault(e => e.Id == item.Id.Value) is { } entity)
                {
                    entity.FullNameEn = item.FullNameEn.Trim();
                    entity.FullNameAr = item.FullNameAr.Trim();
                    entity.TitleEn = item.TitleEn.Trim();
                    entity.TitleAr = item.TitleAr.Trim();
                    entity.AvatarUrl = string.IsNullOrWhiteSpace(item.AvatarUrl) ? null : item.AvatarUrl.Trim();
                    entity.DisplayOrder = item.DisplayOrder;
                    entity.IsFeaturedOnHome = item.IsFeaturedOnHome;

                    _expertRepository.Update(entity);
                }
                else
                {
                    var newEntity = new Expert
                    {
                        FullNameEn = item.FullNameEn.Trim(),
                        FullNameAr = item.FullNameAr.Trim(),
                        TitleEn = item.TitleEn.Trim(),
                        TitleAr = item.TitleAr.Trim(),
                        AvatarUrl = string.IsNullOrWhiteSpace(item.AvatarUrl) ? null : item.AvatarUrl.Trim(),
                        DisplayOrder = item.DisplayOrder,
                        IsFeaturedOnHome = item.IsFeaturedOnHome
                    };

                    await _expertRepository.AddAsync(newEntity);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var updatedList = await _expertRepository
                .GetAllAsync(e => e.IsActive && e.IsFeaturedOnHome)
                .AsNoTracking()
                .OrderBy(e => e.DisplayOrder)
                .Select(e => new HomeExpertDto
                {
                    Id = e.Id,
                    FullNameEn = e.FullNameEn,
                    FullNameAr = e.FullNameAr,
                    TitleEn = e.TitleEn,
                    TitleAr = e.TitleAr,
                    AvatarUrl = e.AvatarUrl
                })
                .ToListAsync(cancellationToken);

            var language = _currentLanguageService.Language;
            foreach (var item in updatedList)
            {
                item.ApplyLanguageFilter(language);
            }

            return Result<List<HomeExpertDto>>.Success(updatedList);
        }
    }
}
