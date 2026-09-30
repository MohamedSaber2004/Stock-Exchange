using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Home.Queries.GetHome
{
    public class GetHomeQueryHandler : IRequestHandler<GetHomeQuery, Result<HomeDto>>
    {
        private readonly IHomeRepository _homeRepository;
        private readonly INewsRepository _newsRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IExpertRepository _expertRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentLanguageService _currentLanguageService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetHomeQueryHandler(
            IHomeRepository homeRepository,
            INewsRepository newsRepository,
            IServiceRepository serviceRepository,
            IArticleRepository articleRepository,
            IVideoRepository videoRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IExpertRepository expertRepository,
            ICurrentUserService currentUserService,
            ICurrentLanguageService currentLanguageService,
            UserManager<ApplicationUser> userManager)
        {
            _homeRepository = homeRepository;
            _newsRepository = newsRepository;
            _serviceRepository = serviceRepository;
            _articleRepository = articleRepository;
            _videoRepository = videoRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _expertRepository = expertRepository;
            _currentUserService = currentUserService;
            _currentLanguageService = currentLanguageService;
            _userManager = userManager;
        }

        public async Task<Result<HomeDto>> Handle(GetHomeQuery request, CancellationToken cancellationToken)
        {
            // ── 1. User Greeting Name ──
            var greetingName = string.Empty;
            if (_currentUserService.IsAuthenticated && _currentUserService.UserId != Guid.Empty)
            {
                var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString());
                greetingName = user?.FullName ?? string.Empty;
            }

            // ── 2. Hero Section ──
            var heroEntity = await _homeRepository.GetFirstAsync(h => h.IsActive, cancellationToken);
            var heroDto = heroEntity is not null
                ? new HomeHeroDto
                {
                    TitleEn = heroEntity.HeroTitleEn,
                    TitleAr = heroEntity.HeroTitleAr,
                    SubtitleEn = heroEntity.HeroSubtitleEn,
                    SubtitleAr = heroEntity.HeroSubtitleAr,
                    ImageUrl = heroEntity.HeroImageUrl
                }
                : new HomeHeroDto
                {
                    TitleEn = "Learn. Analyze. Invest Smarter.",
                    TitleAr = "تعلّم. حلّل. استثمر بذكاء.",
                    SubtitleEn = "Your all-in-one platform for financial education, market insights, and expert analysis.",
                    SubtitleAr = "منصتك المتكاملة للتعليم المالي، ورؤى السوق، والتحليلات المتخصصة."
                };

            // ── 3. Latest News (Top 5) ──
            var latestNews = await _newsRepository
                .GetAllAsync(n => n.IsActive && n.IsFeaturedOnHome)
                .AsNoTracking()
                .OrderByDescending(n => n.PublishedAt)
                .ThenBy(n => n.DisplayOrder)
                .Take(5)
                .Select(n => new HomeNewsDto
                {
                    Id = n.Id,
                    TitleEn = n.TitleEn,
                    TitleAr = n.TitleAr,
                    SummaryEn = n.SummaryEn,
                    SummaryAr = n.SummaryAr,
                    CategoryEn = n.CategoryEn,
                    CategoryAr = n.CategoryAr,
                    ImageUrl = n.ImageUrl,
                    PublishedAt = n.PublishedAt
                })
                .ToListAsync(cancellationToken);

            // ── 4. Services ──
            var services = await _serviceRepository
                .GetAllAsync(s => s.IsActive)
                .AsNoTracking()
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new HomeServiceDto
                {
                    Id = s.Id,
                    TitleEn = s.TitleEn,
                    TitleAr = s.TitleAr,
                    DescriptionEn = s.DescriptionEn,
                    DescriptionAr = s.DescriptionAr,
                    IconName = s.IconName,
                    ImageUrl = s.ImageUrl,
                    LinkRoute = s.LinkRoute
                })
                .ToListAsync(cancellationToken);

            // ── 5. Articles (Top 5) ──
            var articles = await _articleRepository
                .GetAllAsync(a => a.IsActive && a.IsFeaturedOnHome)
                .AsNoTracking()
                .OrderBy(a => a.DisplayOrder)
                .ThenByDescending(a => a.PublishedAt)
                .Take(5)
                .Select(a => new HomeArticleDto
                {
                    Id = a.Id,
                    TitleEn = a.TitleEn,
                    TitleAr = a.TitleAr,
                    ExcerptEn = a.ExcerptEn,
                    ExcerptAr = a.ExcerptAr,
                    ImageUrl = a.ImageUrl,
                    AuthorName = a.AuthorName,
                    ReadMinutes = a.ReadMinutes,
                    PublishedAt = a.PublishedAt
                })
                .ToListAsync(cancellationToken);

            // ── 6. Videos ──
            var videos = await _videoRepository
                .GetAllAsync(v => v.IsActive && v.IsFeaturedOnHome)
                .AsNoTracking()
                .OrderBy(v => v.DisplayOrder)
                .Select(v => new HomeVideoDto
                {
                    Id = v.Id,
                    TitleEn = v.TitleEn,
                    TitleAr = v.TitleAr,
                    ThumbnailUrl = v.ThumbnailUrl,
                    VideoUrl = v.VideoUrl,
                    InstructorName = v.InstructorName,
                    CategoryEn = v.CategoryEn,
                    CategoryAr = v.CategoryAr,
                    IsPreviewable = v.IsPreviewable,
                    IsFeaturedOnHome = v.IsFeaturedOnHome
                })
                .ToListAsync(cancellationToken);

            // ── 7. Subscription Plans ──
            var plans = await _subscriptionPlanRepository
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

            // ── 8. Experts ──
            var experts = await _expertRepository
                .GetAllAsync(e => e.IsActive && e.IsFeaturedOnHome)
                .AsNoTracking()
                .OrderBy(e => e.DisplayOrder)
                .Select(e => new HomeExpertDto
                {
                    Id = e.Id,
                    FullNameEn = e.FullNameEn,
                    FullNameAr = e.FullNameAr,
                    TitleEn = e.TitleEn,
                    TitleAr = e.TitleAr,
                    AvatarUrl = e.AvatarUrl
                })
                .ToListAsync(cancellationToken);

            // ── 9. Construct and apply language filtering ──
            var response = new HomeDto
            {
                GreetingName = greetingName,
                Hero = heroDto,
                LatestNews = latestNews,
                Services = services,
                Articles = articles,
                Videos = videos,
                Plans = plans,
                Experts = experts
            };

            response.ApplyLanguageFilter(_currentLanguageService.Language);

            return Result<HomeDto>.Success(response);
        }
    }
}
