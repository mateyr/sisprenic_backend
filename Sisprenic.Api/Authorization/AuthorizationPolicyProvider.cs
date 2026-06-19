using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Sisprenic.Api.Authorization;

public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly ILogger<AuthorizationPolicyProvider> _logger;

    public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, ILogger<AuthorizationPolicyProvider> logger)
        : base(options)
    {
        _logger = logger;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

        if (policy is not null) {
            return policy;
        }

        if (!Permissions.All.Contains(policyName)) {
            _logger.LogError(
                "RequireAuthorization(\"{PolicyName}\") does not match any registered policy or permission in {Permissions}. This is likely a typo at the call site.",
                policyName, nameof(Permissions));

            throw new InvalidOperationException(
                $"'{policyName}' is not a registered authorization policy or permission. Check for a typo where RequireAuthorization(\"{policyName}\") is called.");
        }

        return new AuthorizationPolicyBuilder()
            .RequireClaim(Permissions.ClaimType, policyName)
            .Build();
    }
}
