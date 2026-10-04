using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Services.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Services.Commands.AddService
{
    public class AddServiceCommandHandler : IRequestHandler<AddServiceCommand, Result<ServiceDto>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddServiceCommandHandler(
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ServiceDto>> Handle(AddServiceCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var service = new Service
            {
                TitleEn = request.TitleEn.Trim(),
                TitleAr = request.TitleAr.Trim(),
                DescriptionEn = request.DescriptionEn?.Trim() ?? string.Empty,
                DescriptionAr = request.DescriptionAr?.Trim() ?? string.Empty,
                IconName = request.IconName?.Trim() ?? string.Empty,
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
                LinkRoute = string.IsNullOrWhiteSpace(request.LinkRoute) ? null : request.LinkRoute.Trim(),
                DisplayOrder = request.DisplayOrder
            };

            if (!request.IsActive)
            {
                service.SetActiveState(false, _currentUserService.UserId.ToString());
            }

            await _serviceRepository.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();

            return Result<ServiceDto>.Success(new ServiceDto
            {
                Id = service.Id,
                TitleEn = service.TitleEn,
                TitleAr = service.TitleAr,
                DescriptionEn = service.DescriptionEn,
                DescriptionAr = service.DescriptionAr,
                IconName = service.IconName,
                ImageUrl = service.ImageUrl,
                LinkRoute = service.LinkRoute,
                DisplayOrder = service.DisplayOrder,
                IsActive = service.IsActive,
                CreatedAt = service.CreatedAt
            });
        }
    }
}
