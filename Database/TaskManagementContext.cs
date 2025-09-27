using Microsoft.EntityFrameworkCore;

namespace TaskManagementApi.Database;

public class TaskManagementContext : DbContext
{
    public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
    {
    }

    public DbSet<Models.HumanTask> HumanTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Models.HumanTask>().ToTable("HumanTasks").HasKey(t => t.Id);
    }
    
}