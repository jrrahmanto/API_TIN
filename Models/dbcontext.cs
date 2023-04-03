using Microsoft.EntityFrameworkCore;

namespace API_TIN_KBI.Models
{
    public class dbcontext : DbContext
    {
        public dbcontext(DbContextOptions<dbcontext> options) : base(options)
        {

        }
        public DbSet<api_acess> api_acess { get; set; }
        public DbSet<RawTradeFeed> RawTradeFeed { get; set; }
        public DbSet<ClearingMember> ClearingMember { get; set; }
        public DbSet<StagingSellerAllocation> StagingSellerAllocation { get; set; }
        public DbSet<api_log> api_log { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RawTradeFeed>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.BusinessDate,
                    e.TradeFeedID
                }).HasName("XPKRawTradeFeed");

                entity.ToTable("RawTradeFeed", "SKD", tb => tb.HasTrigger("trig_insert_tradefeed"));
                entity.Property(e => e.BusinessDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ClearingMember>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.code
                });
                entity.ToTable("ClearingMember", "SKD");
            });

            modelBuilder.Entity<StagingSellerAllocation>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("StagingSellerAllocation", "SKD");
            });
        }
    }
}
