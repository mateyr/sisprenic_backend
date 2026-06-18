using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Sisprenic.Api.Authorization;

public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

        if (policy is not null) {
            return policy;
        }


        return new AuthorizationPolicyBuilder()
            .RequireClaim(Permissions.ClaimType, policyName)
            .Build();
    }
}
