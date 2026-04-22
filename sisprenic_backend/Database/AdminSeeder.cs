using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace sisprenic.Database;

public static class AdminSeeder
{
    private const string AdminEmail = "admin@sisprenic.com";
    private const string AdminPassword = "Admin123*";
    private const string PermissionClaimType = "permission";

    public static async Task SeedAsync(
        UserManager<IdentityUser> userManager,
        SisprenicContext context,
        CancellationToken cancellationToken = default)
    {
        IdentityUser admin = await EnsureAdminUserAsync(userManager);

        await EnsureAdminClaimsAsync(userManager, context, admin, cancellationToken);
    }

    private static async Task<IdentityUser> EnsureAdminUserAsync(UserManager<IdentityUser> userManager)
    {
        IdentityUser? existing = await userManager.FindByEmailAsync(AdminEmail);
        if (existing is not null)
        {
            return existing;
        }

        IdentityUser admin = new IdentityUser
        {
            UserName = AdminEmail,
            Email = AdminEmail
        };

        IdentityResult result = await userManager.CreateAsync(admin, AdminPassword);
        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create admin user: {errors}");
        }

        return admin;
    }

    private static async Task EnsureAdminClaimsAsync(
        UserManager<IdentityUser> userManager,
        SisprenicContext context,
        IdentityUser admin,
        CancellationToken cancellationToken)
    {
        List<string> menuClaims = await context.Menu
            .Where(m => m.RequiredClaim != null)
            .Select(m => m.RequiredClaim!)
            .ToListAsync(cancellationToken);

        IList<Claim> existingClaims = await userManager.GetClaimsAsync(admin);

        HashSet<string> existingPermissions = existingClaims
            .Where(c => c.Type == PermissionClaimType)
            .Select(c => c.Value)
            .ToHashSet();

        List<Claim> claims = menuClaims
            .Where(value => !existingPermissions.Contains(value))
            .Select(value => new Claim(PermissionClaimType, value))
            .ToList();

        if (claims.Count == 0)
        {
            return;
        }

        IdentityResult result = await userManager.AddClaimsAsync(admin, claims);
        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to assign permission claims to admin user: {errors}");
        }
    }
}
