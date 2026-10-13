# User Hard Delete & Cascade Purge Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use subagent-driven-development (recommended) or executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Transform user deletion into a comprehensive hard delete that permanently erases the user, their refresh tokens, activity logs, identity mappings, and physical profile picture.

**Architecture:** Relax EF Core change tracking for `ApplicationUser` deletions in `StockExchangeDbContext`; update `ActivityLogGenerator` to avoid foreign key violations; introduce a transactional `IUserPurgeService` that cleans related data (`ActivityLogs`, `UserRefreshTokens`, disk image) and executes `_userManager.DeleteAsync(user)`; refactor `DeleteUserCommandHandler` and `DeleteAccountCommandHandler`; update confirmation dialog copy in Admin Frontend.

**Tech Stack:** ASP.NET Core 8, EF Core (SqlServer), ASP.NET Core Identity, MediatR, Vue 3 / Vite (Admin Frontend).

---

### Task 1: Relax Soft-Delete Interception for ApplicationUser in DbContext

**Files:**
- Modify: `Stock-Exchange.Persistance/StockExchangeDbContext.cs:115-138`

- [ ] **Step 1: Remove `EntityState.Deleted` interception on `ApplicationUser`**

In `Stock-Exchange.Persistance/StockExchangeDbContext.cs`, update the `ApplicationUser` change tracking block in `SaveChangesAsync` so that `EntityState.Deleted` is no longer transformed to `EntityState.Modified` with `IsDeleted = true`. Only `Added` and `Modified` should update audit fields, letting `Deleted` proceed natively to issue a SQL `DELETE`.

```csharp
            // 3. ApplicationUser auditing (allow hard delete for Deleted state)
            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is ApplicationUser && e.State != EntityState.Detached))
            {
                var user = (ApplicationUser)entry.Entity;
                switch (entry.State)
                {
                    case EntityState.Added:
                        user.CreatedAt = DateTime.UtcNow;
                        user.CreatedBy = userId;
                        user.IsActive = true;
                        break;
                    case EntityState.Modified:
                        user.UpdatedAt = DateTime.UtcNow;
                        user.UpdatedBy = userId;
                        break;
                }
            }
```

- [ ] **Step 2: Verify project builds**

Run: `dotnet build "e:\Stock Exchange\Stock Exchange\Stock-Exchange.Persistance\Stock-Exchange.Persistance.csproj"`
Expected: Build succeeded with 0 errors.

---

### Task 2: Safeguard ActivityLogGenerator Against Foreign Key Constraint Violations

**Files:**
- Modify: `Stock-Exchange.Persistance/Services/ActivityLogGenerator.cs:203-216`

- [ ] **Step 1: Update `MapUserLog` for `ChangeOperation.Deleted`**

In `Stock-Exchange.Persistance/Services/ActivityLogGenerator.cs`, inside `MapUserLog`:
When a user is being deleted (`op == ChangeOperation.Deleted`), the generated log must NOT set `LogUserId: user.Id`, because `user.Id` will no longer exist in the `Users` table and would cause a foreign key constraint violation. Instead, use `currentUserId` (the administrator performing the action) or `null` (if self-deletion), and do not reference the deleted user's profile image:

```csharp
            if (op == ChangeOperation.Deleted)
            {
                return new EntityLogMeta(
                    ActivityResourceType.Users,
                    $"حذف حساب المستخدم: {userDisplayName}",
                    $"Deleted user account: {userDisplayName}",
                    $"تم حذف حساب المستخدم {userDisplayName} ({user.Email})",
                    $"Deleted user account for {userDisplayName} ({user.Email})",
                    LogUserId: currentUserId,
                    LogUserName: currentUserName,
                    LogUserEmail: currentUserEmail,
                    UserProfilePictureUrl: null);
            }
```

- [ ] **Step 2: Verify project builds**

Run: `dotnet build "e:\Stock Exchange\Stock Exchange\Stock-Exchange.Persistance\Stock-Exchange.Persistance.csproj"`
Expected: Build succeeded with 0 errors.

---

### Task 3: Create IUserPurgeService and Register in Application Layer

**Files:**
- Create: `Stock-Exchange.Application/Common/Interfaces/IUserPurgeService.cs`
- Create: `Stock-Exchange.Application/Common/Services/UserPurgeService.cs`
- Modify: `Stock-Exchange.Application/DependencyInjection.cs`

- [ ] **Step 1: Create `IUserPurgeService.cs`**

Create `Stock-Exchange.Application/Common/Interfaces/IUserPurgeService.cs`:

```csharp
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IUserPurgeService
    {
        Task<Result<bool>> PurgeUserAsync(ApplicationUser user, CancellationToken cancellationToken);
    }
}
```

- [ ] **Step 2: Create `UserPurgeService.cs`**

Create `Stock-Exchange.Application/Common/Services/UserPurgeService.cs`:

```csharp
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
```

- [ ] **Step 3: Register `IUserPurgeService` in `DependencyInjection.cs`**

In `Stock-Exchange.Application/DependencyInjection.cs`, register the scoped service:
```csharp
services.AddScoped<IUserPurgeService, UserPurgeService>();
```

- [ ] **Step 4: Verify project builds**

Run: `dotnet build "e:\Stock Exchange\Stock Exchange\Stock-Exchange.Application\Stock-Exchange.Application.csproj"`
Expected: Build succeeded with 0 errors.

---

### Task 4: Refactor DeleteUserCommandHandler to Use IUserPurgeService

**Files:**
- Modify: `Stock-Exchange.Application/Features/Users/Commands/DeleteUser/DeleteUserCommandHandler.cs`

- [ ] **Step 1: Refactor `DeleteUserCommandHandler.cs`**

Replace the soft-delete logic with a call to `_userPurgeService.PurgeUserAsync(user, cancellationToken)`, preserving security rules (cannot delete self, cannot delete another admin):

```csharp
using MediatR;
using Microsoft.AspNetCore.Identity;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserPurgeService _userPurgeService;

        public DeleteUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService,
            IUserPurgeService userPurgeService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _userPurgeService = userPurgeService;
        }

        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId == request.Id)
            {
                throw new BadRequestException(LocalizationKeys.AuthMessages.CannotDeleteSelf);
            }

            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null || user.IsDeleted)
                throw new NotFoundException(LocalizationKeys.AuthMessages.UserNotFound);

            // Security Rule: An admin cannot delete another admin
            var targetRoles = await _userManager.GetRolesAsync(user);
            var isTargetAdmin = targetRoles.Contains(UserType.Admin.ToString());
            if (isTargetAdmin)
            {
                throw new ForbiddenException(LocalizationKeys.AuthMessages.CannotModifyOtherAdmin);
            }

            return await _userPurgeService.PurgeUserAsync(user, cancellationToken);
        }
    }
}
```

- [ ] **Step 2: Verify project builds**

Run: `dotnet build "e:\Stock Exchange\Stock Exchange\Stock-Exchange.Application\Stock-Exchange.Application.csproj"`
Expected: Build succeeded with 0 errors.

---

### Task 5: Refactor DeleteAccountCommandHandler to Use IUserPurgeService

**Files:**
- Modify: `Stock-Exchange.Application/Features/Auth/Commands/DeleteAccount/DeleteAccountCommandHandler.cs`

- [ ] **Step 1: Refactor `DeleteAccountCommandHandler.cs`**

Replace the soft-delete logic with a call to `_userPurgeService.PurgeUserAsync(user, cancellationToken)`:

```csharp
using MediatR;
using Microsoft.AspNetCore.Identity;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Features.Auth.Commands.DeleteAccount
{
    public sealed class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserPurgeService _userPurgeService;

        public DeleteAccountCommandHandler(
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService,
            IUserPurgeService userPurgeService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _userPurgeService = userPurgeService;
        }

        public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString());
            if (user == null || user.IsDeleted)
                throw new NotFoundException(LocalizationKeys.AuthMessages.UserNotFound);

            var result = await _userPurgeService.PurgeUserAsync(user, cancellationToken);
            return result.IsSuccess;
        }
    }
}
```

- [ ] **Step 2: Verify project builds**

Run: `dotnet build "e:\Stock Exchange\Stock Exchange\Stock-Exchange.Application\Stock-Exchange.Application.csproj"`
Expected: Build succeeded with 0 errors.

---

### Task 6: Update Confirmation Dialog Copy in Admin Frontend

**Files:**
- Modify: `E:\Stock Exchange Admin Frontend\Stock-Exchange\stock-exchange\src\views\users\UsersListView.vue:201-205`
- Modify: `E:\Stock Exchange Admin Frontend\Stock-Exchange\stock-exchange\src\views\users\UserDetailsView.vue:245-250`

- [ ] **Step 1: Update message in `UsersListView.vue`**

Update the confirmation dialog text to accurately state that the account and all related data will be permanently deleted:
```typescript
      title: isAr.value ? `حذف حساب "${u.fullName}"` : `Delete Account ("${u.fullName}")`,
      message: isAr.value
        ? `هل أنت متأكد من رغبتك في حذف حساب "${u.fullName}"؟ سيتم حذف الحساب وكافة البيانات المرتبطة به نهائياً ولا يمكن استرجاعه.`
        : `Are you sure you want to permanently delete the account for "${u.fullName}"? The account and all associated data will be permanently removed.`,
      confirmText: t('common.delete'),
```

- [ ] **Step 2: Update message in `UserDetailsView.vue`**

Ensure the message confirms permanent removal:
```typescript
      title: isAr.value ? `حذف حساب "${user.value.fullName}"` : `Delete Account ("${user.value.fullName}")`,
      message: isAr.value
        ? `هل أنت متأكد من رغبتك في حذف هذا الحساب نهائياً؟ سيتم حذف الحساب وكافة البيانات المرتبطة به ولا يمكن التراجع عن هذه العملية.`
        : `Are you sure you want to permanently delete this account? The account and all associated data will be permanently removed and cannot be undone.`,
      confirmText: t('common.delete'),
```

---

### Task 7: Full Solution Build & Verification

**Files:**
- Solution: `e:\Stock Exchange\Stock Exchange\Stock Exchange.sln`

- [ ] **Step 1: Build the complete backend solution**

Run: `dotnet build "e:\Stock Exchange\Stock Exchange\Stock Exchange.sln"`
Expected: Build succeeded with 0 errors.

- [ ] **Step 2: Commit all changes to git**

Run:
```bash
git add docs/plans/2026-10-13-hard-delete-user.md
git commit -m "docs: add implementation plan for user hard delete"
```
