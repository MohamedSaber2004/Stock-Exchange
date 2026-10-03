using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.TermsAndConditions.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.TermsAndConditions.Queries.GetTermsAndConditions
{
    public class GetTermsAndConditionsQueryHandler : IRequestHandler<GetTermsAndConditionsQuery, Result<TermsAndConditionsDto>>
    {
        private readonly ITermsAndConditionsRepository _termsAndConditionsRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetTermsAndConditionsQueryHandler(
            ITermsAndConditionsRepository termsAndConditionsRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _termsAndConditionsRepository = termsAndConditionsRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<TermsAndConditionsDto>> Handle(GetTermsAndConditionsQuery request, CancellationToken cancellationToken)
        {
            var termsAndConditions = await _termsAndConditionsRepository
                .GetAllWithIncluding(t => t.IsActive, t => t.Sections)
                .AsNoTracking()
                .Select(t => new TermsAndConditionsDto
                {
                    Id = t.Id,
                    TitleEn = t.TitleEn,
                    TitleAr = t.TitleAr,
                    DescriptionEn = t.DescriptionEn,
                    DescriptionAr = t.DescriptionAr,
                    Sections = t.Sections
                        .OrderBy(s => s.DisplayOrder)
                        .Select(s => new TermsAndConditionsSectionDto
                        {
                            Id = s.Id,
                            TitleEn = s.TitleEn,
                            TitleAr = s.TitleAr,
                            ContentEn = s.ContentEn,
                            ContentAr = s.ContentAr,
                            DisplayOrder = s.DisplayOrder
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (termsAndConditions is null)
                return Result<TermsAndConditionsDto>.Failure(LocalizationKeys.TermsAndConditionsMessages.TermsAndConditionsNotFound, StatusCodes.Status404NotFound);

            if (request.ApplyLanguageFilter ?? true)
            {
                termsAndConditions.ApplyLanguageFilter(_currentLanguageService.Language);
            }

            return Result<TermsAndConditionsDto>.Success(termsAndConditions);
        }
    }
}
