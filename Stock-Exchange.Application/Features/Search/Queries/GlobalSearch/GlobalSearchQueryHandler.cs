using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Search.DTOs;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Search.Queries.GlobalSearch;

public class GlobalSearchQueryHandler : IRequestHandler<GlobalSearchQuery, Result<GlobalSearchResultDto>>
{
    private readonly IStockExchangeDbContext _dbContext;
    private readonly ICurrentLanguageService _currentLanguageService;

    public GlobalSearchQueryHandler(
        IStockExchangeDbContext dbContext,
        ICurrentLanguageService currentLanguageService)
    {
        _dbContext = dbContext;
        _currentLanguageService = currentLanguageService;
    }

    public async Task<Result<GlobalSearchResultDto>> Handle(GlobalSearchQuery request, CancellationToken cancellationToken)
    {
        var term = (request.Query ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(term))
        {
            return Result<GlobalSearchResultDto>.Success(new GlobalSearchResultDto());
        }

        var limit = request.Limit <= 0 ? 5 : Math.Min(request.Limit, 20);
        var isArabic = _currentLanguageService.Language == Language.ar;
        var lowerTerm = term.ToLower();

        // 1. Articles
        var articles = await _dbContext.Articles
            .AsNoTracking()
            .Where(a => !a.IsDeleted && (
                a.TitleEn.ToLower().Contains(lowerTerm) ||
                a.TitleAr.ToLower().Contains(lowerTerm) ||
                a.ExcerptEn.ToLower().Contains(lowerTerm) ||
                a.ExcerptAr.ToLower().Contains(lowerTerm) ||
                a.AuthorName.ToLower().Contains(lowerTerm) ||
                (a.Category != null && (a.Category.CategoryEnName.ToLower().Contains(lowerTerm) || a.Category.CategoryArName.ToLower().Contains(lowerTerm)))
            ))
            .OrderByDescending(a => a.PublishedAt)
            .Take(limit)
            .Select(a => new GlobalSearchItemDto
            {
                Id = a.Id,
                Title = isArabic ? (string.IsNullOrEmpty(a.TitleAr) ? a.TitleEn : a.TitleAr) : (string.IsNullOrEmpty(a.TitleEn) ? a.TitleAr : a.TitleEn),
                Subtitle = !string.IsNullOrEmpty(a.AuthorName) ? a.AuthorName : null,
                Category = a.Category != null ? (isArabic ? a.Category.CategoryArName : a.Category.CategoryEnName) : null,
                ImageUrl = a.ImageUrl,
                Type = "article",
                TargetRoute = $"/articles/{a.Id}",
                Date = a.PublishedAt,
                Badge = isArabic ? "مقال" : "Article"
            })
            .ToListAsync(cancellationToken);

        // 2. Videos
        var videos = await _dbContext.Videos
            .AsNoTracking()
            .Where(v => !v.IsDeleted && (
                v.TitleEn.ToLower().Contains(lowerTerm) ||
                v.TitleAr.ToLower().Contains(lowerTerm) ||
                v.DescriptionEn.ToLower().Contains(lowerTerm) ||
                v.DescriptionAr.ToLower().Contains(lowerTerm) ||
                v.InstructorName.ToLower().Contains(lowerTerm) ||
                (v.Category != null && (v.Category.CategoryEnName.ToLower().Contains(lowerTerm) || v.Category.CategoryArName.ToLower().Contains(lowerTerm)))
            ))
            .OrderByDescending(v => v.CreatedAt)
            .Take(limit)
            .Select(v => new GlobalSearchItemDto
            {
                Id = v.Id,
                Title = isArabic ? (string.IsNullOrEmpty(v.TitleAr) ? v.TitleEn : v.TitleAr) : (string.IsNullOrEmpty(v.TitleEn) ? v.TitleAr : v.TitleEn),
                Subtitle = !string.IsNullOrEmpty(v.InstructorName) ? v.InstructorName : null,
                Category = v.Category != null ? (isArabic ? v.Category.CategoryArName : v.Category.CategoryEnName) : (isArabic ? v.CategoryAr : v.CategoryEn),
                ImageUrl = v.ThumbnailUrl,
                Type = "video",
                TargetRoute = $"/videos/{v.Id}",
                Date = v.CreatedAt,
                Badge = isArabic ? "فيديو" : "Video"
            })
            .ToListAsync(cancellationToken);

        // 3. News
        var news = await _dbContext.News
            .AsNoTracking()
            .Where(n => !n.IsDeleted && (
                n.TitleEn.ToLower().Contains(lowerTerm) ||
                n.TitleAr.ToLower().Contains(lowerTerm) ||
                n.SummaryEn.ToLower().Contains(lowerTerm) ||
                n.SummaryAr.ToLower().Contains(lowerTerm) ||
                n.CategoryEn.ToLower().Contains(lowerTerm) ||
                n.CategoryAr.ToLower().Contains(lowerTerm)
            ))
            .OrderByDescending(n => n.PublishedAt)
            .Take(limit)
            .Select(n => new GlobalSearchItemDto
            {
                Id = n.Id,
                Title = isArabic ? (string.IsNullOrEmpty(n.TitleAr) ? n.TitleEn : n.TitleAr) : (string.IsNullOrEmpty(n.TitleEn) ? n.TitleAr : n.TitleEn),
                Subtitle = isArabic ? n.SummaryAr : n.SummaryEn,
                Category = isArabic ? n.CategoryAr : n.CategoryEn,
                ImageUrl = n.ImageUrl,
                Type = "news",
                TargetRoute = $"/news/{n.Id}",
                Date = n.PublishedAt,
                Badge = isArabic ? "خبر" : "News"
            })
            .ToListAsync(cancellationToken);

        // 4. Users
        var users = await _dbContext.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted && (
                u.FullName.ToLower().Contains(lowerTerm) ||
                (u.Email != null && u.Email.ToLower().Contains(lowerTerm)) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(lowerTerm))
            ))
            .OrderByDescending(u => u.CreatedAt)
            .Take(limit)
            .Select(u => new GlobalSearchItemDto
            {
                Id = u.Id,
                Title = u.FullName,
                Subtitle = u.Email,
                Category = u.PhoneNumber,
                ImageUrl = u.ProfilePictureUrl,
                Type = "user",
                TargetRoute = $"/users/{u.Id}",
                Date = u.CreatedAt,
                Badge = u.IsActive ? (isArabic ? "نشط" : "Active") : (isArabic ? "معطل" : "Inactive")
            })
            .ToListAsync(cancellationToken);

        // 5. Services
        var services = await _dbContext.Services
            .AsNoTracking()
            .Where(s => !s.IsDeleted && (
                s.TitleEn.ToLower().Contains(lowerTerm) ||
                s.TitleAr.ToLower().Contains(lowerTerm) ||
                s.DescriptionEn.ToLower().Contains(lowerTerm) ||
                s.DescriptionAr.ToLower().Contains(lowerTerm)
            ))
            .OrderBy(s => s.DisplayOrder)
            .Take(limit)
            .Select(s => new GlobalSearchItemDto
            {
                Id = s.Id,
                Title = isArabic ? (string.IsNullOrEmpty(s.TitleAr) ? s.TitleEn : s.TitleAr) : (string.IsNullOrEmpty(s.TitleEn) ? s.TitleAr : s.TitleEn),
                Subtitle = isArabic ? s.DescriptionAr : s.DescriptionEn,
                ImageUrl = s.ImageUrl,
                Type = "service",
                TargetRoute = $"/services/{s.Id}",
                Date = s.CreatedAt,
                Badge = isArabic ? "خدمة" : "Service"
            })
            .ToListAsync(cancellationToken);

        // 6. Experts
        var experts = await _dbContext.Experts
            .AsNoTracking()
            .Where(e => !e.IsDeleted && (
                e.FullNameEn.ToLower().Contains(lowerTerm) ||
                e.FullNameAr.ToLower().Contains(lowerTerm) ||
                e.TitleEn.ToLower().Contains(lowerTerm) ||
                e.TitleAr.ToLower().Contains(lowerTerm)
            ))
            .OrderBy(e => e.DisplayOrder)
            .Take(limit)
            .Select(e => new GlobalSearchItemDto
            {
                Id = e.Id,
                Title = isArabic ? (string.IsNullOrEmpty(e.FullNameAr) ? e.FullNameEn : e.FullNameAr) : (string.IsNullOrEmpty(e.FullNameEn) ? e.FullNameAr : e.FullNameEn),
                Subtitle = isArabic ? e.TitleAr : e.TitleEn,
                ImageUrl = e.AvatarUrl,
                Type = "expert",
                TargetRoute = "/experts",
                Date = e.CreatedAt,
                Badge = isArabic ? "خبير" : "Expert"
            })
            .ToListAsync(cancellationToken);

        // 7. Countries
        var countries = await _dbContext.Countries
            .AsNoTracking()
            .Where(c => !c.IsDeleted && (
                c.CountryEnName.ToLower().Contains(lowerTerm) ||
                c.CountryArName.ToLower().Contains(lowerTerm) ||
                c.Code.ToLower().Contains(lowerTerm)
            ))
            .OrderBy(c => c.CountryEnName)
            .Take(limit)
            .Select(c => new GlobalSearchItemDto
            {
                Id = c.Id,
                Title = isArabic ? (string.IsNullOrEmpty(c.CountryArName) ? c.CountryEnName : c.CountryArName) : (string.IsNullOrEmpty(c.CountryEnName) ? c.CountryArName : c.CountryEnName),
                Subtitle = c.Code,
                Type = "country",
                TargetRoute = "/countries",
                Date = c.CreatedAt,
                Badge = isArabic ? "دولة" : "Country"
            })
            .ToListAsync(cancellationToken);

        // 8. Help Center
        var helpCenters = await _dbContext.HelpCenters
            .AsNoTracking()
            .Where(h => !h.IsDeleted && (
                h.TitleEn.ToLower().Contains(lowerTerm) ||
                h.TitleAr.ToLower().Contains(lowerTerm) ||
                h.ContentEn.ToLower().Contains(lowerTerm) ||
                h.ContentAr.ToLower().Contains(lowerTerm) ||
                (h.Category != null && (h.Category.TitleEn.ToLower().Contains(lowerTerm) || h.Category.TitleAr.ToLower().Contains(lowerTerm)))
            ))
            .OrderBy(h => h.DisplayOrder)
            .Take(limit)
            .Select(h => new GlobalSearchItemDto
            {
                Id = h.Id,
                Title = isArabic ? (string.IsNullOrEmpty(h.TitleAr) ? h.TitleEn : h.TitleAr) : (string.IsNullOrEmpty(h.TitleEn) ? h.TitleAr : h.TitleEn),
                Subtitle = h.Category != null ? (isArabic ? h.Category.TitleAr : h.Category.TitleEn) : null,
                Type = "help",
                TargetRoute = "/help-center",
                Date = h.CreatedAt,
                Badge = isArabic ? "مساعدة" : "Help"
            })
            .ToListAsync(cancellationToken);

        var allItems = new List<GlobalSearchItemDto>();
        allItems.AddRange(articles);
        allItems.AddRange(videos);
        allItems.AddRange(news);
        allItems.AddRange(users);
        allItems.AddRange(services);
        allItems.AddRange(experts);
        allItems.AddRange(countries);
        allItems.AddRange(helpCenters);

        var result = new GlobalSearchResultDto
        {
            Query = term,
            TotalCount = allItems.Count,
            Items = allItems,
            Articles = articles,
            Videos = videos,
            News = news,
            Users = users,
            Services = services,
            Experts = experts,
            Countries = countries,
            HelpCenter = helpCenters
        };

        return Result<GlobalSearchResultDto>.Success(result);
    }
}