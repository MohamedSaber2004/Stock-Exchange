# Design Specification: User Hard Delete & Cascade Purge

**Date:** 2026-10-13  
**Status:** Approved by User  
**Author:** Antigravity AI  

---

## 1. Executive Summary & Objective

The goal is to transition User deletion from soft-delete (`IsDeleted = true`, deactivation, token revocation) to a comprehensive, transactional **Hard Delete (permanent purge)**. When a user is deleted, all user records and associated related data must be permanently erased from the database and storage:
1. The `ApplicationUser` entity in `Users`.
2. All session and refresh tokens (`UserRefreshTokens`).
3. All activity audit logs for the user (`ActivityLogs` where `UserId == user.Id`).
4. All ASP.NET Core Identity relationships (`AspNetUserRoles`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserTokens`).
5. The user's physical profile picture file from server disk storage.

This hard delete applies consistently to both:
- **`DeleteUserCommand`**: When an Admin deletes a user from the Admin Dashboard.
- **`DeleteAccountCommand`**: When a user deletes their own account from the Mobile Application or web profile.

---

## 2. Integrated Software Impact Analysis

### 2.1 Mobile Application (`DELETE /api/v1/auth/delete-account`)
- **API Contract:** Remains **100% backward-compatible**.
  - **Route:** `DELETE /api/v1/auth/delete-account`
  - **Headers:** `Authorization: Bearer <token>`
  - **Response Payload:**
    ```json
    {
      "success": true,
      "statusCode": 200,
      "message": "Deleted successfully",
      "data": true
    }
    ```
- **Mobile Client Behavior:** No code changes required in the mobile client. Upon receiving success, the mobile application clears local secure storage and redirects to the Login/Welcome screen as usual. Any subsequent token refresh attempts will immediately fail (HTTP 401/404) because the user and tokens no longer exist.

### 2.2 Admin Dashboard Frontend (`E:\Stock Exchange Admin Frontend`)
- **API Contract:** Remains **100% backward-compatible**.
  - **Route:** `DELETE /api/v1/users/{id}`
  - **Headers:** `Authorization: Bearer <token>`
  - **Response:** `ApiResponse<bool>`
- **UI Enhancements:**
  - Update confirmation dialog copy in `UsersListView.vue` and `UserDetailsView.vue` from "Account will be deactivated" to clear warning that account and all related data will be permanently erased.
  - User list refresh (`fetchUsers()`) seamlessly drops the removed user because they are physically erased from the database.

---

## 3. Architecture & Backend Design

### 3.1 Entity Framework ChangeTracker Relaxation (`StockExchangeDbContext.cs`)
- **Current Behavior:** In `StockExchangeDbContext.SaveChangesAsync`, any `EntityState.Deleted` on `ApplicationUser` was intercepted and forcibly changed to `EntityState.Modified` with `IsDeleted = true`.
- **New Behavior:** Remove this interceptor logic for `EntityState.Deleted` on `ApplicationUser`. Allow `EntityState.Deleted` to proceed natively, executing `DELETE FROM Users WHERE Id = @Id`.

### 3.2 Audit Logging Safeguard (`ActivityLogGenerator.cs`)
- **Current Behavior:** When an entity is deleted, `ActivityLogGenerator` generated an `ActivityLog` with `LogUserId: user.Id`. If the user is physically deleted from the `Users` table in the same transaction, inserting an `ActivityLog` referencing `user.Id` violates the foreign key constraint `FK_ActivityLogs_Users_UserId`.
- **New Behavior:**
  - For `ChangeOperation.Deleted` on `ApplicationUser`:
    - Set `LogUserId: currentUserId` (the Admin performing the delete) or `null` (if user self-deleted).
    - Store the deleted user's metadata (`userDisplayName`, `user.Email`) in `DetailsAr` / `DetailsEn` / `ActionAr` / `ActionEn`.
    - Never assign the deleted user's ID to `ActivityLog.UserId`.

### 3.3 Unified User Purge Service (`IUserPurgeService` & `UserPurgeService`)
To eliminate code duplication and ensure single-responsibility transactional execution:
- **Interface:** `Stock_Exchange.Application.Common.Interfaces.IUserPurgeService`
  ```csharp
  public interface IUserPurgeService
  {
      Task<Result<bool>> PurgeUserAsync(ApplicationUser user, CancellationToken cancellationToken);
  }
  ```
- **Execution Steps:**
  1. **Physical File Removal:** If `ProfilePictureUrl` is local/valid, call `_imageValidator.DeleteImage(user.ProfilePictureUrl, ProfilePicturePlace)`.
  2. **Database Cleanup (Within Execution Strategy / Transaction):**
     - Delete user activity logs:  
       `await _dbContext.ActivityLogs.Where(a => a.UserId == user.Id).ExecuteDeleteAsync(cancellationToken);`
     - Delete user refresh tokens:  
       `await _dbContext.UserRefreshTokens.Where(t => t.UserId == user.Id).ExecuteDeleteAsync(cancellationToken);`
     - Delete user via Identity:  
       `var identityResult = await _userManager.DeleteAsync(user);`
       *(This automatically deletes `AspNetUserRoles`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserTokens` via Identity's cascade behavior).*
     - If `identityResult` fails, throw `BadRequestException`.
  3. Return `Result<bool>.Success(true)`.

### 3.4 Command Handlers Refactoring
- **`DeleteUserCommandHandler`:**
  - Retain validation: Prevent self-delete (`CannotDeleteSelf`), prevent deleting another Admin (`CannotModifyOtherAdmin`), ensure user exists.
  - Delegate execution to `_userPurgeService.PurgeUserAsync(user, cancellationToken)`.
- **`DeleteAccountCommandHandler`:**
  - Retain auth verification: Ensure user is authenticated.
  - Delegate execution to `_userPurgeService.PurgeUserAsync(user, cancellationToken)`.

---

## 4. Error Handling & Security

1. **Self-Deletion Protection for Admins:** Admins cannot delete their own account via `DeleteUserCommand`.
2. **Admin-on-Admin Protection:** Admins cannot delete other Admins via `DeleteUserCommand`.
3. **Transaction Rollback:** If any step fails during the database purge, the transaction rolls back cleanly.
4. **Foreign Key Integrity:** By removing `ActivityLogs` and `UserRefreshTokens` before `_userManager.DeleteAsync(user)` and avoiding referencing `user.Id` in newly generated audit logs, no foreign key violation can occur.

---

## 5. Verification & Validation Plan

1. **Compilation Check:** Run `dotnet build` on the solution to ensure no compilation errors.
2. **Admin User Deletion:**
   - Execute `DELETE /api/v1/users/{id}` for a customer user.
   - Verify user row is removed from `Users`.
   - Verify related `UserRefreshTokens` are removed.
   - Verify related `ActivityLogs` with that `UserId` are removed.
   - Verify physical profile image is removed from disk.
   - Verify Identity roles/claims are removed.
3. **Self-Account Deletion:**
   - Execute `DELETE /api/v1/auth/delete-account` with customer auth token.
   - Verify customer user is completely removed from all tables and files.
4. **Admin Frontend Check:**
   - Confirm confirmation dialog copy is updated in `UsersListView.vue`.
