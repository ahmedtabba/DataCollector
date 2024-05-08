using DataCollector.Data.Interceptors;
using DataCollector.Identity;
using DataCollector.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataCollector.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IServiceProvider _serviceProvider;
        public AppDbContext(DbContextOptions<AppDbContext> options, IServiceProvider serviceProvider)
            : base(options)
        {
            _serviceProvider = serviceProvider;
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StorePhoto> StorePhotos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var auditableEntityInterceptor = _serviceProvider.GetRequiredService<AuditableEntityInterceptor>();
            optionsBuilder.AddInterceptors(auditableEntityInterceptor);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Store>()
       .HasMany(x => x.StorePhotos)
       .WithOne(x => x.Store)
       .HasForeignKey(x => x.StoreId)
       .IsRequired();



            //builder.Entity<StorePhoto>()
            //    .HasOne(x => x.Store)
            //    .WithMany(x => x.StorePhotos);

            base.OnModelCreating(builder);
        }
    }

}
