using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MCMS.Base.Auth;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.TypeConfig;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MCMS.Data
{
    public class BaseDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole,
        IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        private readonly EntitiesConfig _entitiesConfig;

        public BaseDbContext(DbContextOptions options, IOptions<EntitiesConfig> entitiesConfig) : base(options)
        {
            _entitiesConfig = entitiesConfig.Value;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var methodInfo = builder.GetType().GetMethods().First(mi =>
                mi.GetParameters().Any(pi =>
                    pi.ParameterType.IsSubclassOfGenericType(typeof(IEntityTypeConfiguration<>))));
            foreach (var entitiesConfigEntityStack in _entitiesConfig.EntityStacks)
            {
                if (entitiesConfigEntityStack.ShouldIgnoreSpecificTypeConfiguration()) continue;

                var genericMethodInfo = methodInfo.MakeGenericMethod(entitiesConfigEntityStack.EntityType);
                genericMethodInfo.Invoke(builder,
                    new object[] { entitiesConfigEntityStack.GetEntityTypeConfigurationInstance() });
            }

            // workaround for a "bug" introduced when upgraded to Net 6
            foreach (var property in builder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
            {
                property.SetColumnType("timestamp without time zone");
            }

            MDbFunctions.Register(builder);
        }

        #region on save triggers

        private void OnSaveChanges()
        {
            AdjustTimeStamps();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            OnSaveChanges();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = new())
        {
            OnSaveChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void AdjustTimeStamps()
        {
            var entries = ChangeTracker.Entries().Where(ee =>
                ee.Entity is IEntity && ee.State is EntityState.Added or EntityState.Modified);
            foreach (var entityEntry in entries)
            {
                if (entityEntry.Entity is not IEntity entity) continue;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.Created = DateTime.Now;
                }

                entity.Updated = DateTime.Now;
            }
        }

        #endregion
    }
}