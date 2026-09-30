using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.PrivacyPolicy.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.PrivacyPolicy.Queries.GetPrivacy
{
    public class GetPrivacyQueryHandler : IRequestHandler<GetPrivacyQuery, Result<PrivacyPolicyDto>>
    {
        private readonly IPrivacyPolicyRepository _privacyPolicyRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetPrivacyQueryHandler(
            IPrivacyPolicyRepository privacyPolicyRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _privacyPolicyRepository = privacyPolicyRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<PrivacyPolicyDto>> Handle(GetPrivacyQuery request, CancellationToken cancellationToken)
        {
            var privacyPolicy = await _privacyPolicyRepository
                .GetAllWithIncluding(p => p.IsActive, p => p.Sections)
                .AsNoTracking()
                .Select(p => new PrivacyPolicyDto
                {
                    Id = p.Id,
                    TitleEn = p.TitleEn,
                    TitleAr = p.TitleAr,
                    DescriptionEn = p.DescriptionEn,
                    DescriptionAr = p.DescriptionAr,
                    Sections = p.Sections
                        .OrderBy(s => s.DisplayOrder)
                        .Select(s => new PrivacyPolicySectionDto
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

            if (privacyPolicy is null)
                return Result<PrivacyPolicyDto>.Failure(LocalizationKeys.PrivacyPolicyMessages.PrivacyPolicyNotFound, StatusCodes.Status404NotFound);

            privacyPolicy.ApplyLanguageFilter(_currentLanguageService.Language);

            return Result<PrivacyPolicyDto>.Success(privacyPolicy);
        }
    }
}
