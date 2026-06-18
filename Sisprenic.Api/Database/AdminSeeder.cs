using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Authorization;

namespace Sisprenic.Api.Database;

public static class AdminSeeder
{
    public static async Task SeedAsync(
        UserManager<IdentityUser> userManager,
        SisprenicContext context,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        string adminEmail = configuration["AdminUser:Email"]
            ?? throw new InvalidOperationException("Missing configuration value 'AdminUser:Email'.");
        string adminPassword = configuration["AdminUser:Password"]
            ?? throw new InvalidOperationException("Missing configuration value 'AdminUser:Password'.");

        IdentityUser admin = await EnsureAdminUserAsync(userManager, adminEmail, adminPassword);

        await EnsureAdminClaimsAsync(userManager, context, admin, cancellationToken);
    }

    private static async Task<IdentityUser> EnsureAdminUserAsync(
        UserManager<IdentityUser> userManager,
        string adminEmail,
        string adminPassword)
    {
        IdentityUser? existing = await userManager.FindByEmailAsync(adminEmail);
        if (existing is not null)
        {
            return existing;
        }

        IdentityUser admin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail
        };

        IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
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
            .Where(c => c.Type == Permissions.ClaimType)
            .Select(c => c.Value)
            .ToHashSet();

        List<Claim> claims = menuClaims
            .Where(value => !existingPermissions.Contains(value))
            .Select(value => new Claim(Permissions.ClaimType, value))
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
