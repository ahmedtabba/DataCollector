using DataCollector.Identity.AppContext;
using DataCollector.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

namespace DataCollector.Data.Interceptors
{
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {

        readonly private IApplicationContext _appContext;
        public AuditableEntityInterceptor(IApplicationContext appContext)
        {
            _appContext = appContext;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }


        public void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = _appContext.GetUserId();
                    entry.Entity.CreationDate = DateTime.UtcNow;
                }

                if (/*entry.State == EntityState.Added || */entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    entry.Entity.LastModifiedBy = _appContext.GetUserId();
                    entry.Entity.LastModifiedDate = DateTime.UtcNow;
                }
            }
        }
    }

    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
            entry.References.Any(r =>
                r.TargetEntry != null &&
                r.TargetEntry.Metadata.IsOwned() &&
                (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }

}
