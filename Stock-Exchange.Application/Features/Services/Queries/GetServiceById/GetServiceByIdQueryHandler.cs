using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Services.DTOs;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Services.Queries.GetServiceById
{
    public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, Result<ServiceDto>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetServiceByIdQueryHandler(
            IServiceRepository serviceRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _serviceRepository = serviceRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<ServiceDto>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var service = await _serviceRepository
                .GetAllAsync(s => s.Id == request.Id)
                .AsNoTracking()
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    TitleEn = s.TitleEn,
                    TitleAr = s.TitleAr,
                    DescriptionEn = s.DescriptionEn,
                    DescriptionAr = s.DescriptionAr,
                    IconName = s.IconName,
                    ImageUrl = s.ImageUrl,
                    LinkRoute = s.LinkRoute,
                    DisplayOrder = s.DisplayOrder,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (service is null)
                throw new NotFoundException("Service not found");

            var apply = request.ApplyLanguageFilter ?? true;
            if (apply)
            {
                service.ApplyLanguageFilter(_currentLanguageService.Language);
            }

            return Result<ServiceDto>.Success(service);
        }
    }
}
