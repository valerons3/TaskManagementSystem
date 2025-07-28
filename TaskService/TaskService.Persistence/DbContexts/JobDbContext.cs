using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Entities;

namespace TaskService.Persistence.DbContexts;

public class JobDbContext : DbContext
{
    public JobDbContext(DbContextOptions<JobDbContext> options) : base(options){}
    
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobHistory> JobHistories => Set<JobHistory>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobHistory>(builder =>
        {
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Action).IsRequired().HasMaxLength(200);
            builder.Property(h => h.Timestamp).IsRequired();
        });
        
        modelBuilder.Entity<Job>(builder =>
        {
            builder.HasKey(j => j.Id);
            builder.Property(j => j.Title).IsRequired().HasMaxLength(200);
            builder.Property(j => j.Description).HasMaxLength(1000);
            builder.Property(j => j.Status).IsRequired();
        });   
    }
}