
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Models;


namespace TaskManager.Infrastructure
{
    public class TaskManagerDbContext : IdentityDbContext<UserModel, IdentityRole<Guid>, Guid>
    {
        public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
        : base(options)
        {
        }

        public DbSet<ProjectModel> Projects => Set<ProjectModel>();
        public DbSet<TaskItemModel> Tasks => Set<TaskItemModel>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(TaskManagerDbContext).Assembly);
        }
    }
}
