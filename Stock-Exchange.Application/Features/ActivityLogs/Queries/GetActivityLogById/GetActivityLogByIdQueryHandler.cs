using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ActivityLogs.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Features.ActivityLogs.Queries.GetActivityLogById
{
    public class GetActivityLogByIdQueryHandler : IRequestHandler<GetActivityLogByIdQuery, Result<ActivityLogDto>>
    {
        private readonly IStockExchangeDbContext _dbContext;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetActivityLogByIdQueryHandler(
            IStockExchangeDbContext dbContext,
            ICurrentLanguageService currentLanguageService)
        {
            _dbContext = dbContext;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<ActivityLogDto>> Handle(GetActivityLogByIdQuery request, CancellationToken cancellationToken)
        {
            var isGuid = Guid.TryParse(request.Id, out var guidId);

            var log = await _dbContext.ActivityLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(a => !a.IsDeleted &&
                    (isGuid ? a.Id == guidId : a.FormattedId == request.Id),
                    cancellationToken);

            if (log == null)
                throw new NotFoundException(LocalizationKeys.ActivityLogMessages.ActivityLogNotFound);

            var dto = new ActivityLogDto
            {
                Id = log.Id,
                FormattedId = log.FormattedId,
                UserId = log.UserId,
                UserName = log.UserName,
                UserEmail = log.UserEmail,
                UserProfilePictureUrl = log.UserProfilePictureUrl,
                Action = log.Action,
                ActionAr = log.ActionAr,
                ActionEn = log.ActionEn,
                ResourceType = log.ResourceType,
                IpAddress = log.IpAddress,
                Device = log.Device,
                CreatedAt = log.CreatedAt,
                Details = log.Details,
                DetailsAr = log.DetailsAr,
                DetailsEn = log.DetailsEn
            };

            dto.ApplyLanguageFilter(_currentLanguageService.Language);

            return Result<ActivityLogDto>.Success(dto);
        }
    }
}