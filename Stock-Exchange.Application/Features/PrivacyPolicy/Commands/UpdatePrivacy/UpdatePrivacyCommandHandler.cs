using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.PrivacyPolicy.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;
using PrivacyPolicyEntity = Stock_Exchange.Domain.Entities.PrivacyPolicy;

namespace Stock_Exchange.Application.Features.PrivacyPolicy.Commands.UpdatePrivacy
{
    public class UpdatePrivacyCommandHandler : IRequestHandler<UpdatePrivacyCommand, Result<PrivacyPolicyDto>>
    {
        private readonly IPrivacyPolicyRepository _privacyPolicyRepository;
        private readonly IPrivacyPolicySectionRepository _privacyPolicySectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdatePrivacyCommandHandler(
            IPrivacyPolicyRepository privacyPolicyRepository,
            IPrivacyPolicySectionRepository privacyPolicySectionRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _privacyPolicyRepository = privacyPolicyRepository;
            _privacyPolicySectionRepository = privacyPolicySectionRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PrivacyPolicyDto>> Handle(UpdatePrivacyCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var privacyPolicy = await _privacyPolicyRepository.GetFirstAsync(p => p.IsActive, cancellationToken);

            if (privacyPolicy is null)
            {
                privacyPolicy = new PrivacyPolicyEntity
                {
                    TitleEn = string.IsNullOrWhiteSpace(request.TitleEn) ? "Privacy Policy" : request.TitleEn.Trim(),
                    TitleAr = string.IsNullOrWhiteSpace(request.TitleAr) ? "سياسة الخصوصية" : request.TitleAr.Trim(),
                    DescriptionEn = string.IsNullOrWhiteSpace(request.DescriptionEn) ? null : request.DescriptionEn.Trim(),
                    DescriptionAr = string.IsNullOrWhiteSpace(request.DescriptionAr) ? null : request.DescriptionAr.Trim()
                };

                await _privacyPolicyRepository.AddAsync(privacyPolicy);
            }
            else
            {
                if (request.TitleEn is not null)
                    privacyPolicy.TitleEn = request.TitleEn.Trim();

                if (request.TitleAr is not null)
                    privacyPolicy.TitleAr = request.TitleAr.Trim();

                if (request.DescriptionEn is not null)
                    privacyPolicy.DescriptionEn = string.IsNullOrWhiteSpace(request.DescriptionEn) ? null : request.DescriptionEn.Trim();

                if (request.DescriptionAr is not null)
                    privacyPolicy.DescriptionAr = string.IsNullOrWhiteSpace(request.DescriptionAr) ? null : request.DescriptionAr.Trim();

                _privacyPolicyRepository.Update(privacyPolicy);
            }

            if (request.Sections is not null)
            {
                var existingSections = await _privacyPolicySectionRepository
                    .GetAllAsync(s => s.PrivacyPolicyId == privacyPolicy.Id)
                    .ToListAsync(cancellationToken);

                foreach (var existingSection in existingSections)
                    _privacyPolicySectionRepository.Delete(existingSection);

                var newSections = request.Sections
                    .Where(s => !string.IsNullOrWhiteSpace(s.TitleEn) || !string.IsNullOrWhiteSpace(s.TitleAr)
                             || !string.IsNullOrWhiteSpace(s.ContentEn) || !string.IsNullOrWhiteSpace(s.ContentAr))
                    .Select((s, index) => new PrivacyPolicySection
                    {
                        PrivacyPolicyId = privacyPolicy.Id,
                        TitleEn = s.TitleEn?.Trim() ?? string.Empty,
                        TitleAr = s.TitleAr?.Trim() ?? string.Empty,
                        ContentEn = s.ContentEn?.Trim() ?? string.Empty,
                        ContentAr = s.ContentAr?.Trim() ?? string.Empty,
                        DisplayOrder = index + 1
                    })
                    .ToList();

                if (newSections.Count > 0)
                    await _privacyPolicySectionRepository.AddRangeAsync(newSections);
            }

            await _unitOfWork.SaveChangesAsync();

            var sections = await _privacyPolicySectionRepository
                .GetAllAsync(s => s.PrivacyPolicyId == privacyPolicy.Id)
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
                .ToListAsync(cancellationToken);

            return Result<PrivacyPolicyDto>.Success(new PrivacyPolicyDto
            {
                Id = privacyPolicy.Id,
                TitleEn = privacyPolicy.TitleEn,
                TitleAr = privacyPolicy.TitleAr,
                DescriptionEn = privacyPolicy.DescriptionEn,
                DescriptionAr = privacyPolicy.DescriptionAr,
                Sections = sections
            });
        }
    }
}
