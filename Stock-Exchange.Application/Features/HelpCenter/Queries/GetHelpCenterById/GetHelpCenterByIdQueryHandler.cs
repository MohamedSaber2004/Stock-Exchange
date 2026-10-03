using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenter.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.HelpCenter.Queries.GetHelpCenterById
{
    public class GetHelpCenterByIdQueryHandler : IRequestHandler<GetHelpCenterByIdQuery, Result<HelpCenterDto>>
    {
        private readonly IHelpCenterRepository _helpCenterRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetHelpCenterByIdQueryHandler(
            IHelpCenterRepository helpCenterRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _helpCenterRepository = helpCenterRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<HelpCenterDto>> Handle(GetHelpCenterByIdQuery request, CancellationToken cancellationToken)
        {
            var helpCenter = await _helpCenterRepository
                .GetAllAsync(h => h.Id == request.Id && h.IsActive)
                .AsNoTracking()
                .Select(h => new HelpCenterDto
                {
                    Id = h.Id,
                    TitleEn = h.TitleEn,
                    TitleAr = h.TitleAr,
                    ContentEn = h.ContentEn,
                    ContentAr = h.ContentAr,
                    CategoryId = h.CategoryId,
                    DisplayOrder = h.DisplayOrder
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (helpCenter is null)
                return Result<HelpCenterDto>.Failure(LocalizationKeys.HelpCenterMessages.HelpCenterNotFound, StatusCodes.Status404NotFound);

            if (request.ApplyLanguageFilter ?? true)
            {
                helpCenter.ApplyLanguageFilter(_currentLanguageService.Language);
            }

            return Result<HelpCenterDto>.Success(helpCenter);
        }
    }
}
