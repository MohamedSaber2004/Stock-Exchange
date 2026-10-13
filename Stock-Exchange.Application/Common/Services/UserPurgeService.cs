using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Common.Services
{
    public class UserPurgeService : IUserPurgeService
    {
        private const int ProfilePicturePlace = 0;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStockExchangeDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageValidator _imageValidator;

        public UserPurgeService(
            UserManager<ApplicationUser> userManager,
            IStockExchangeDbContext dbContext,
            IUnitOfWork unitOfWork,
            IImageValidator imageValidator)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
            _imageValidator = imageValidator;
        }

        public async Task<Result<bool>> PurgeUserAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            // 1. Delete physical profile picture from disk storage if exists
            if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl) &&
                !user.ProfilePictureUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !user.ProfilePictureUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                await _imageValidator.DeleteImage(user.ProfilePictureUrl, ProfilePicturePlace);
            }

            // 2. Transactional database cascade hard delete
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Purge activity logs related to this user
                await _dbContext.ActivityLogs
                    .Where(a => a.UserId == user.Id)
                    .ExecuteDeleteAsync(cancellationToken);

                // Purge refresh tokens related to this user
                await _dbContext.UserRefreshTokens
                    .Where(t => t.UserId == user.Id)
                    .ExecuteDeleteAsync(cancellationToken);

                // Purge user and ASP.NET Identity relations (roles, claims, logins, tokens)
                var identityResult = await _userManager.DeleteAsync(user);
                if (!identityResult.Succeeded)
                {
                    await _unitOfWork.RollbackAsync();

                    var errors = identityResult.Errors
                        .GroupBy(e => e.Code)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                    throw new BadRequestException(errors, LocalizationKeys.ExceptionMessages.BadRequest);
                }

                await _unitOfWork.CommitAsync();
                return Result<bool>.Success(true);
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
