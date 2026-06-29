using Sisprenic.Api.Entities;

namespace Sisprenic.Api.Modules.Loans.Shared;

/// <summary>
/// Single source of truth that maps a loan's outstanding balances to its <see cref="LoanStatus"/>.
/// </summary>
/// <remarks>
/// Both creating and deleting a payment recompute the loan balance and must agree on what
/// "fully settled" means. Centralizing the rule here keeps those flows in lockstep: a change
/// to the settlement criterion happens in one place instead of being mirrored by hand.
/// </remarks>
public static class LoanStatusService
{
    public static LoanStatus Resolve(decimal principalOutstanding, decimal interestOutstanding) =>
        principalOutstanding <= 0 && interestOutstanding <= 0
            ? LoanStatus.Paid
            : LoanStatus.Active;
}
