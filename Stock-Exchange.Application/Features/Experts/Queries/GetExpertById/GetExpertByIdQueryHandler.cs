using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Experts.DTOs;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Experts.Queries.GetExpertById
{
    public class GetExpertByIdQueryHandler : IRequestHandler<GetExpertByIdQuery, Result<ExpertDto>>
    {
        private readonly IExpertRepository _expertRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetExpertByIdQueryHandler(
            IExpertRepository expertRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _expertRepository = expertRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<ExpertDto>> Handle(GetExpertByIdQuery request, CancellationToken cancellationToken)
        {
            var expert = await _expertRepository
                .GetAllAsync(e => e.Id == request.Id)
                .AsNoTracking()
                .Select(e => new ExpertDto
                {
                    Id = e.Id,
                    FullNameEn = e.FullNameEn,
                    FullNameAr = e.FullNameAr,
                    TitleEn = e.TitleEn,
                    TitleAr = e.TitleAr,
                    AvatarUrl = e.AvatarUrl,
                    DisplayOrder = e.DisplayOrder,
                    IsFeaturedOnHome = e.IsFeaturedOnHome,
                    IsActive = e.IsActive,
                    CreatedAt = e.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (expert is null)
                throw new NotFoundException("Expert not found");

            var apply = request.ApplyLanguageFilter ?? true;
            if (apply)
            {
                expert.ApplyLanguageFilter(_currentLanguageService.Language);
            }

            return Result<ExpertDto>.Success(expert);
        }
    }
}
