using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomePlans
{
    public class UpdateHomePlansCommandHandler : IRequestHandler<UpdateHomePlansCommand, Result<List<HomePlanDto>>>
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IPlanFeatureRepository _planFeatureRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentLanguageService _currentLanguageService;

        public UpdateHomePlansCommandHandler(
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IPlanFeatureRepository planFeatureRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICurrentLanguageService currentLanguageService)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _planFeatureRepository = planFeatureRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<List<HomePlanDto>>> Handle(UpdateHomePlansCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var existingPlans = await _subscriptionPlanRepository
                .GetAllWithIncluding(p => p.IsActive, p => p.Features)
                .ToListAsync(cancellationToken);

            var requestPlanIds = request.Items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToHashSet();

            // Soft-delete omitted plans and their features
            foreach (var plan in existingPlans.Where(p => !requestPlanIds.Contains(p.Id)))
            {
                foreach (var feature in plan.Features)
                {
                    _planFeatureRepository.Delete(feature);
                }
                _subscriptionPlanRepository.Delete(plan);
            }

            // Upsert requested plans
            foreach (var item in request.Items)
            {
                SubscriptionPlan planEntity;

                if (item.Id.HasValue && existingPlans.FirstOrDefault(p => p.Id == item.Id.Value) is { } existing)
                {
                    existing.NameEn = item.NameEn.Trim();
                    existing.NameAr = item.NameAr.Trim();
                    existing.PriceEgp = item.PriceEgp;
                    existing.Period = item.Period;
                    existing.IsHighlighted = item.IsHighlighted;
                    existing.DisplayOrder = item.DisplayOrder;

                    _subscriptionPlanRepository.Update(existing);
                    planEntity = existing;
                }
                else
                {
                    planEntity = new SubscriptionPlan
                    {
                        NameEn = item.NameEn.Trim(),
                        NameAr = item.NameAr.Trim(),
                        PriceEgp = item.PriceEgp,
                        Period = item.Period,
                        IsHighlighted = item.IsHighlighted,
                        DisplayOrder = item.DisplayOrder
                    };

                    await _subscriptionPlanRepository.AddAsync(planEntity);
                }

                // Sync features for this plan
                var existingFeatures = planEntity.Features?.Where(f => f.IsActive).ToList() ?? new List<PlanFeature>();
                var requestFeatureIds = item.Features.Where(f => f.Id.HasValue).Select(f => f.Id!.Value).ToHashSet();

                foreach (var oldFeature in existingFeatures.Where(f => !requestFeatureIds.Contains(f.Id)))
                {
                    _planFeatureRepository.Delete(oldFeature);
                }

                foreach (var feat in item.Features)
                {
                    if (feat.Id.HasValue && existingFeatures.FirstOrDefault(f => f.Id == feat.Id.Value) is { } existingFeat)
                    {
                        existingFeat.TextEn = feat.TextEn.Trim();
                        existingFeat.TextAr = feat.TextAr.Trim();
                        existingFeat.DisplayOrder = feat.DisplayOrder;

                        _planFeatureRepository.Update(existingFeat);
                    }
                    else
                    {
                        var newFeat = new PlanFeature
                        {
                            PlanId = planEntity.Id,
                            TextEn = feat.TextEn.Trim(),
                            TextAr = feat.TextAr.Trim(),
                            DisplayOrder = feat.DisplayOrder
                        };

                        await _planFeatureRepository.AddAsync(newFeat);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var updatedList = await _subscriptionPlanRepository
                .GetAllWithIncluding(p => p.IsActive, p => p.Features)
                .AsNoTracking()
                .OrderBy(p => p.DisplayOrder)
                .Select(p => new HomePlanDto
                {
                    Id = p.Id,
                    NameEn = p.NameEn,
                    NameAr = p.NameAr,
                    PriceEgp = p.PriceEgp,
                    Period = p.Period == Domain.Enums.SubscriptionPeriod.Yearly ? "yr" : "mo",
                    IsHighlighted = p.IsHighlighted,
                    DisplayOrder = p.DisplayOrder,
                    FeatureItems = p.Features
                        .OrderBy(f => f.DisplayOrder)
                        .Select(f => new HomePlanFeatureDto
                        {
                            Id = f.Id,
                            TextEn = f.TextEn,
                            TextAr = f.TextAr,
                            DisplayOrder = f.DisplayOrder
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            var language = _currentLanguageService.Language;
            foreach (var item in updatedList)
            {
                item.ApplyLanguageFilter(language);
            }

            return Result<List<HomePlanDto>>.Success(updatedList);
        }
    }
}
