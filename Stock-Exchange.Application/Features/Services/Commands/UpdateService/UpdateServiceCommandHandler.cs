using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Services.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Services.Commands.UpdateService
{
    public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, Result<ServiceDto>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateServiceCommandHandler(
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ServiceDto>> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var service = await _serviceRepository.GetFirstAsync(s => s.Id == request.Id, cancellationToken);
            if (service is null)
                throw new NotFoundException("Service not found");

            service.TitleEn = request.TitleEn.Trim();
            service.TitleAr = request.TitleAr.Trim();
            service.DescriptionEn = request.DescriptionEn?.Trim() ?? string.Empty;
            service.DescriptionAr = request.DescriptionAr?.Trim() ?? string.Empty;
            if (request.ContentEn != null) service.ContentEn = request.ContentEn.Trim();
            if (request.ContentAr != null) service.ContentAr = request.ContentAr.Trim();
            service.IconName = request.IconName?.Trim() ?? string.Empty;
            service.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
            service.LinkRoute = string.IsNullOrWhiteSpace(request.LinkRoute) ? null : request.LinkRoute.Trim();
            service.DisplayOrder = request.DisplayOrder;

            if (service.IsActive != request.IsActive)
            {
                service.SetActiveState(request.IsActive, _currentUserService.UserId.ToString());
            }

            _serviceRepository.Update(service);
            await _unitOfWork.SaveChangesAsync();

            return Result<ServiceDto>.Success(new ServiceDto
            {
                Id = service.Id,
                TitleEn = service.TitleEn,
                TitleAr = service.TitleAr,
                DescriptionEn = service.DescriptionEn,
                DescriptionAr = service.DescriptionAr,
                ContentEn = service.ContentEn,
                ContentAr = service.ContentAr,
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
