using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WishApp.Data.Models.Auth;
using WishApp.Data.Models.Master;
using WishApp.Data.Models.Transaction;

namespace WishApp.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
          : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<SubWish>()
            .HasOne(a => a.Wish)
            .WithMany(b => b.SubWishes);
        builder.Entity<WishOption>()
            .HasOne(a => a.Wish)
            .WithMany(b => b.WishOptions);
        builder.Entity<DailySubWish>()
            .HasOne(a => a.DailyWish)
            .WithMany(b => b.DailySubWishes);
    }

    public DbSet<Wish> Wishes { get; set; }
    public DbSet<SubWish> SubWishes { get; set; }
    public DbSet<WishOption> WishOptions { get; set; }
    public DbSet<DailyWish> DailyWishes { get; set; }
    public DbSet<DailySubWish> DailySubWishes { get; set; }
}
