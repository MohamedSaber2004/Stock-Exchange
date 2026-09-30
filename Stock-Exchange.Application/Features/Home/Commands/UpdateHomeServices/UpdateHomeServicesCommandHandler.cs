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

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeServices
{
    public class UpdateHomeServicesCommandHandler : IRequestHandler<UpdateHomeServicesCommand, Result<List<HomeServiceDto>>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentLanguageService _currentLanguageService;

        public UpdateHomeServicesCommandHandler(
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICurrentLanguageService currentLanguageService)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<List<HomeServiceDto>>> Handle(UpdateHomeServicesCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var existingServices = await _serviceRepository.GetAllAsync(s => s.IsActive).ToListAsync(cancellationToken);
            var requestIds = request.Items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToHashSet();

            // Soft-delete items that were omitted
            foreach (var item in existingServices.Where(e => !requestIds.Contains(e.Id)))
            {
                _serviceRepository.Delete(item);
            }

            // Upsert items from request
            foreach (var item in request.Items)
            {
                if (item.Id.HasValue && existingServices.FirstOrDefault(e => e.Id == item.Id.Value) is { } entity)
                {
                    entity.TitleEn = item.TitleEn.Trim();
                    entity.TitleAr = item.TitleAr.Trim();
                    entity.DescriptionEn = item.DescriptionEn?.Trim() ?? string.Empty;
                    entity.DescriptionAr = item.DescriptionAr?.Trim() ?? string.Empty;
                    entity.IconName = item.IconName?.Trim() ?? string.Empty;
                    entity.ImageUrl = string.IsNullOrWhiteSpace(item.ImageUrl) ? null : item.ImageUrl.Trim();
                    entity.LinkRoute = string.IsNullOrWhiteSpace(item.LinkRoute) ? null : item.LinkRoute.Trim();
                    entity.DisplayOrder = item.DisplayOrder;

                    _serviceRepository.Update(entity);
                }
                else
                {
                    var newEntity = new Service
                    {
                        TitleEn = item.TitleEn.Trim(),
                        TitleAr = item.TitleAr.Trim(),
                        DescriptionEn = item.DescriptionEn?.Trim() ?? string.Empty,
                        DescriptionAr = item.DescriptionAr?.Trim() ?? string.Empty,
                        IconName = item.IconName?.Trim() ?? string.Empty,
                        ImageUrl = string.IsNullOrWhiteSpace(item.ImageUrl) ? null : item.ImageUrl.Trim(),
                        LinkRoute = string.IsNullOrWhiteSpace(item.LinkRoute) ? null : item.LinkRoute.Trim(),
                        DisplayOrder = item.DisplayOrder
                    };

                    await _serviceRepository.AddAsync(newEntity);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var updatedList = await _serviceRepository
                .GetAllAsync(s => s.IsActive)
                .AsNoTracking()
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new HomeServiceDto
                {
                    Id = s.Id,
                    TitleEn = s.TitleEn,
                    TitleAr = s.TitleAr,
                    DescriptionEn = s.DescriptionEn,
                    DescriptionAr = s.DescriptionAr,
                    IconName = s.IconName,
                    ImageUrl = s.ImageUrl,
                    LinkRoute = s.LinkRoute
                })
                .ToListAsync(cancellationToken);

            var language = _currentLanguageService.Language;
            foreach (var item in updatedList)
            {
                item.ApplyLanguageFilter(language);
            }

            return Result<List<HomeServiceDto>>.Success(updatedList);
        }
    }
}
