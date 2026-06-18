using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Sisprenic.Api.Entities;

namespace Sisprenic.Api.Database;

public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ConvertHardDeletesToSoftDeletes(eventData);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ConvertHardDeletesToSoftDeletes(eventData);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ConvertHardDeletesToSoftDeletes(DbContextEventData eventData)
    {
        if (eventData.Context is null)
            return;

        var entries = eventData.Context
            .ChangeTracker
            .Entries<ISoftDeletable>()
            .Where(e => e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedOn = DateTime.UtcNow;
        }
    }
}
