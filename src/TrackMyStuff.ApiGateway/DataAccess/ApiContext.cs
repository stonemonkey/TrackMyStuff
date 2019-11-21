using Microsoft.EntityFrameworkCore;
using TrackMyStuff.ApiGateway.Queries;

public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options)
        : base(options) { }

    public DbSet<DeviceStatus> DeviceStatus { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceStatus>()
            .HasKey(x => x.DeviceId);
        modelBuilder.Entity<DeviceStatus>()
            .Property(x => x.LastSeenAt)
            .IsRequired();
    }
}