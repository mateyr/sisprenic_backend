using Microsoft.EntityFrameworkCore;

using Sisprenic.Api.Database;
using Sisprenic.Api.Entities;

namespace Sisprenic.Api.Modules.Loans.Shared;

/// <summary>
/// Loads a loan acquiring a pessimistic row lock (<c>SELECT ... FOR UPDATE</c>).
/// </summary>
/// <remarks>
/// Must be invoked inside an open transaction: the lock is held until commit/rollback.
/// It serves as the serialization point for operations that recompute the loan balance
/// (creating/deleting payments). While one transaction holds the lock, any other that tries
/// to lock the same loan waits, preventing two concurrent payments from reading the same
/// balance, overpaying the principal, and flipping the loan to Paid more than once.
/// </remarks>
public static class LoanLockService
{
    public static async Task<Loan?> LoadForUpdateAsync(
        SisprenicContext dbContext,
        int loanId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Loan
            .FromSqlInterpolated(
                $"SELECT * FROM loan WHERE id = {loanId} AND is_deleted = false FOR UPDATE")
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(cancellationToken);
    }
}
