using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Database.Security;

namespace TaskManagementApi.Database;

public class TaskManagementContext(DbContextOptions<TaskManagementContext> options)
    : IdentityDbContext<AppUser>(options)
{
    public DbSet<Models.HumanTask> HumanTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Models.HumanTask>().ToTable("humantasks", "sandbox").HasKey(t => t.Id);
    }
    
}