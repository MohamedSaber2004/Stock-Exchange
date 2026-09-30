using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.TermsAndConditions.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;
using TermsAndConditionsEntity = Stock_Exchange.Domain.Entities.TermsAndConditions;

namespace Stock_Exchange.Application.Features.TermsAndConditions.Commands.UpdateTermsAndConditions
{
    public class UpdateTermsAndConditionsCommandHandler : IRequestHandler<UpdateTermsAndConditionsCommand, Result<TermsAndConditionsDto>>
    {
        private readonly ITermsAndConditionsRepository _termsAndConditionsRepository;
        private readonly ITermsAndConditionsSectionRepository _termsAndConditionsSectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateTermsAndConditionsCommandHandler(
            ITermsAndConditionsRepository termsAndConditionsRepository,
            ITermsAndConditionsSectionRepository termsAndConditionsSectionRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _termsAndConditionsRepository = termsAndConditionsRepository;
            _termsAndConditionsSectionRepository = termsAndConditionsSectionRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<TermsAndConditionsDto>> Handle(UpdateTermsAndConditionsCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var termsAndConditions = await _termsAndConditionsRepository.GetFirstAsync(t => t.IsActive, cancellationToken);

            if (termsAndConditions is null)
            {
                termsAndConditions = new TermsAndConditionsEntity
                {
                    TitleEn = string.IsNullOrWhiteSpace(request.TitleEn) ? "Terms & Conditions" : request.TitleEn.Trim(),
                    TitleAr = string.IsNullOrWhiteSpace(request.TitleAr) ? "الشروط والأحكام" : request.TitleAr.Trim(),
                    DescriptionEn = string.IsNullOrWhiteSpace(request.DescriptionEn) ? null : request.DescriptionEn.Trim(),
                    DescriptionAr = string.IsNullOrWhiteSpace(request.DescriptionAr) ? null : request.DescriptionAr.Trim()
                };

                await _termsAndConditionsRepository.AddAsync(termsAndConditions);
            }
            else
            {
                if (request.TitleEn is not null)
                    termsAndConditions.TitleEn = request.TitleEn.Trim();

                if (request.TitleAr is not null)
                    termsAndConditions.TitleAr = request.TitleAr.Trim();

                if (request.DescriptionEn is not null)
                    termsAndConditions.DescriptionEn = string.IsNullOrWhiteSpace(request.DescriptionEn) ? null : request.DescriptionEn.Trim();

                if (request.DescriptionAr is not null)
                    termsAndConditions.DescriptionAr = string.IsNullOrWhiteSpace(request.DescriptionAr) ? null : request.DescriptionAr.Trim();

                _termsAndConditionsRepository.Update(termsAndConditions);
            }

            if (request.Sections is not null)
            {
                var existingSections = await _termsAndConditionsSectionRepository
                    .GetAllAsync(s => s.TermsAndConditionsId == termsAndConditions.Id)
                    .ToListAsync(cancellationToken);

                foreach (var existingSection in existingSections)
                    _termsAndConditionsSectionRepository.Delete(existingSection);

                var newSections = request.Sections
                    .Where(s => !string.IsNullOrWhiteSpace(s.TitleEn) || !string.IsNullOrWhiteSpace(s.TitleAr)
                             || !string.IsNullOrWhiteSpace(s.ContentEn) || !string.IsNullOrWhiteSpace(s.ContentAr))
                    .Select((s, index) => new TermsAndConditionsSection
                    {
                        TermsAndConditionsId = termsAndConditions.Id,
                        TitleEn = s.TitleEn?.Trim() ?? string.Empty,
                        TitleAr = s.TitleAr?.Trim() ?? string.Empty,
                        ContentEn = s.ContentEn?.Trim() ?? string.Empty,
                        ContentAr = s.ContentAr?.Trim() ?? string.Empty,
                        DisplayOrder = index + 1
                    })
                    .ToList();

                if (newSections.Count > 0)
                    await _termsAndConditionsSectionRepository.AddRangeAsync(newSections);
            }

            await _unitOfWork.SaveChangesAsync();

            var sections = await _termsAndConditionsSectionRepository
                .GetAllAsync(s => s.TermsAndConditionsId == termsAndConditions.Id)
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
                .ToListAsync(cancellationToken);

            return Result<TermsAndConditionsDto>.Success(new TermsAndConditionsDto
            {
                Id = termsAndConditions.Id,
                TitleEn = termsAndConditions.TitleEn,
                TitleAr = termsAndConditions.TitleAr,
                DescriptionEn = termsAndConditions.DescriptionEn,
                DescriptionAr = termsAndConditions.DescriptionAr,
                Sections = sections
            });
        }
    }
}
