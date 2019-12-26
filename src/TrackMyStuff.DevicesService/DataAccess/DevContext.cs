using Microsoft.EntityFrameworkCore;

namespace TrackMyStuff.DevicesService.DataAccess
{
    public class DevContext : DbContext
    {
        public DevContext(DbContextOptions<DevContext> options)
            : base(options) { }

        public DbSet<HeartBeat> HeartBeat { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HeartBeat>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<HeartBeat>()
                .Property(x => x.DeviceId)
                .IsRequired();
            modelBuilder.Entity<HeartBeat>()
                .Property(x => x.CreatedAt)
                .IsRequired();
        }
    }
}