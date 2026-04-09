using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sisprenic.Configurations;
using sisprenic.Entities;
using sisprenic_backend.Configurations;
using sisprenic_backend.Entities;

namespace sisprenic.Database
{
    public class SisprenicContext(DbContextOptions<SisprenicContext> options)
        : IdentityDbContext<IdentityUser>(options)
    {
        public DbSet<Client> Client { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<Loan> Loan { get; set; }
        public DbSet<Payment> Payment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>().ToTable("user");
            modelBuilder.Entity<IdentityRole>().ToTable("role");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("user_role");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("user_claim");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("role_claim");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("user_token");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("user_login");

            // Invoke Configurations
            new ClientConfiguration().Configure(modelBuilder.Entity<Client>());
            new MenuConfiguration().Configure(modelBuilder.Entity<Menu>());
            new LoanConfiguration().Configure(modelBuilder.Entity<Loan>());
            new PaymentConfiguration().Configure(modelBuilder.Entity<Payment>());
        }
    }
}
