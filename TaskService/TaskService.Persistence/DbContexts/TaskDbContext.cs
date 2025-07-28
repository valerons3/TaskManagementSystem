using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Entities;

namespace TaskService.Persistence.DbContexts;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options){}
    
    public DbSet<Job> Jobs => Set<Job>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>(builder =>
        {
            builder.HasKey(j => j.Id);
            builder.Property(j => j.Title).IsRequired().HasMaxLength(200);
            builder.Property(j => j.Description).HasMaxLength(1000);
            builder.Property(j => j.Status).IsRequired();
        });   
    }
}